using OMSServices.Services;
using System.Dynamic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using OMSServices.Hubs;
using Microsoft.Extensions.Logging;
using LS.WebSocketServer.Services;
using Client.Communication;
using Client.Communication.Interfaces;
using Client.Global.Extensions;
using System.Reactive.Linq;
using System.Collections.Generic;
using OMSServices.Enum;
using OMSServices.Models;
using System;
using Client.Providers.Utilities;
using Microsoft.Extensions.Caching.Distributed;
using OMSServices.Utils;
using System.Linq;
using OMSServices.Data;

namespace OMSServices.Implementation
{
    class LocatesService : ILocatesService
    {
        private readonly static IReadOnlyDictionary<QueryType, object> s_locks = new List<QueryType>() { QueryType.Locates, QueryType.LocateSummary, QueryType.LocateSummaryWithSymbol }.ToDictionary(x => x, _ => new object());

        private readonly ISubscriptionKeyManagementService subscriptionKeyManagementService;
        private readonly IRedisService redisService;
        private readonly IDistributedCache distributedCache;
        private readonly IQueryGenerator queryGenerator;
        private readonly IHubContext<TransactionalHub> hubContext;
        private readonly IWebSocketContext webSocketContext;
        private readonly ILogger<LocatesService> logger;
        private readonly RequestInformation requestInformation;
        private readonly IStaticDataService staticDataService;

        public LocatesService(
            ISubscriptionKeyManagementService subscriptionKeyManagementService,
            IRedisService redisService,
            IDistributedCache distributedCache,
            IQueryGenerator queryGenerator,
            IHubContext<TransactionalHub> hubContext,
            IWebSocketContext webSocketContext,
            ILogger<LocatesService> logger,
            RequestInformation requestInformation,
            IStaticDataService staticDataService
        )
        {
            this.subscriptionKeyManagementService = subscriptionKeyManagementService;
            this.redisService = redisService;
            this.distributedCache = distributedCache;
            this.queryGenerator = queryGenerator;
            this.hubContext = hubContext;
            this.webSocketContext = webSocketContext;
            this.requestInformation = requestInformation;
            this.logger = logger;
            this.staticDataService = staticDataService;
        }

        public async Task<object> LocateRequest(BindableOEMessage bindableOEMessage, string userIdentifier)
        {
            bindableOEMessage = await AddClientIdInBindableOEMessageAsync(bindableOEMessage, userIdentifier);

            return requestInformation.WatchRequestTime("Send data to service provider for locate request", () =>
            {
                return Helper.SendDatatoServiceProvider("OrderExec", DataHelper.WrapBOEMessage(bindableOEMessage.ToDictionary(), bindableOEMessage.BoothID));
            });
        }

        public async Task<object> LocateAcquire(BindableOEMessage bindableOEMessage, string userIdentifier)
        {
            bindableOEMessage = await AddClientIdInBindableOEMessageAsync(bindableOEMessage, userIdentifier);

            return requestInformation.WatchRequestTime("Send data to service provider for locate request", () =>
            {
                return Helper.SendDatatoServiceProvider("OrderExec", DataHelper.WrapBOEMessage(bindableOEMessage.ToDictionary(), bindableOEMessage.BoothID));
            });
        }

        public async Task<string> UnsubscribeAsync(string userIdentifier, string userDesc, string boothId, QueryType queryType)
        {
            var endPoint = EndPointManager.Instance.GetEndPoint("LocateEndPoint") as ICommunicationEndPoint<ExpandoObject, ExpandoObject>;
            
            var unsubscribeRequest = queryGenerator.GetUnsubscribeQuery(queryType, userDesc, boothId, null) as IDictionary<string, object>;
            _ = endPoint.Send(unsubscribeRequest["QueryObject"] as ExpandoObject);

            //If unsub is successfull then remove the key from cache
            await subscriptionKeyManagementService.RemoveSubscriptionKeyFromCacheAsync(userIdentifier, userDesc, boothId, queryType);
            return "Success!";
        }

        public async Task UnsubscribeAllConnectionsAsync(string identifier)
        {
            var keys = redisService.GetAllKeysWithPrefix(identifier).GetAsyncEnumerator();
            (string userDesc, string boothId) = UserClaims.ParseUserIdentifier(identifier);
            try
            {
                while (await keys.MoveNextAsync())
                {
                    //(string userDesc, string boothId, bool isProvider) = subscriptionKeyManagementService.DecomposeSubscriptionKey(keys.Current.ToString());
                    var subscriptions = (await distributedCache.GetAsync(keys.Current.ToString())).FromBytes<List<QueryType>>();
                    foreach (var queryType in subscriptions)
                    {
                        if (queryType == QueryType.Locates || queryType == QueryType.LocateSummary || queryType == QueryType.LocateSummaryWithSymbol)
                            await UnsubscribeAsync(identifier, userDesc, boothId, queryType);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
            finally
            {
                if (keys != null) await keys.DisposeAsync();
            }
        }

        public async Task<T> SubscribeAsync<T>(string userIdentifier, string userDesc, string boothId, QueryType queryType, string account = null, string symbol = null) where T : class
        {
            var endPoint = EndPointManager.Instance.GetEndPoint("LocateEndPoint") as ICommunicationEndPoint<ExpandoObject, ExpandoObject>;
            var accounts = account == null ? await GetAccountIdsAsync(userDesc, boothId, userIdentifier) : new List<string>() { account };
            if (accounts == null || accounts.IsEmpty())
            {
                logger.LogWarning("Accounts static data is null or empty. UserDesc: {UserDesc}, BoothId: {BoothId}, QueryType: {QueryType}", userDesc, boothId, queryType);
                return null;
            }

            //Check if the key is already subscribed or not
            bool addedToCache;
            lock (s_locks[queryType])
            {
                addedToCache = subscriptionKeyManagementService.CheckAndSaveSubscriptionKeyInCache(userIdentifier, userDesc, boothId, queryType);
            }
            if (addedToCache)
            {
                try
                {
                    var continuousQuery = queryGenerator.GetContinuousQuery(queryType, userDesc, boothId, accounts, false, symbol) as IDictionary<string, object>;
                    endPoint.Send(continuousQuery["QueryObject"] as ExpandoObject)
                        .Where(x => x.GetValue<long>("EventType") != -1)
                        .Do(async x =>
                        {
                            await PublishMessageToSocketConnectionsAsync<T>(userIdentifier, queryType.ToString(), x);
                        })
                        .Subscribe();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                    //If failed in subscribing then remove the key from cache
                    await subscriptionKeyManagementService.RemoveSubscriptionKeyFromCacheAsync(userIdentifier, userDesc, boothId, queryType);
                    return null;
                }
            }
            var queryInfo = queryGenerator.GetOneTimeQuery(queryType, userDesc, boothId, accounts, false, symbol);
            var queryObject = queryInfo.GetValue<ExpandoObject>("QueryObject");
            var result = await endPoint.Send(queryObject).FirstAsync();
            return result.ConvertExpandoObjectTo<T>();
        }

        private async Task PublishMessageToSocketConnectionsAsync<T>(string userIdentifier, string methodName, ExpandoObject publishMessage) where T : class
        {
            var convertedMessage = publishMessage.ConvertExpandoObjectTo<T>();
            await hubContext.Clients.User(userIdentifier).SendAsync(methodName, convertedMessage);
            await webSocketContext.SendToUserAsync(userIdentifier, new { Method = methodName, Data = convertedMessage });
        }

        private async Task<List<string>> GetAccountIdsAsync(string userDesc, string boothId, string userIdentifier)
        {
            var accountsData = await staticDataService.GetStaticDataAsync<AccountDetails>(QueryType.Account, userDesc, boothId, userIdentifier);
            var data = new List<string>();

            if (accountsData != null)
                data = accountsData.EventData.Select(x => x.Value).ToList();

            return data;
        }

        private async Task<BindableOEMessage> AddClientIdInBindableOEMessageAsync(BindableOEMessage locateRequest, string userIdentifier)
        {
            if (locateRequest.Account == null || locateRequest.ClientID == null)
            {
                var accounts = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.Account, locateRequest.OriginatingUserDesc, locateRequest.BoothID, userIdentifier);

                //If both are null then set default account with default client id
                if (locateRequest.Account == null)
                {
                    var defaultAccount = accounts.EventData.FirstOrDefault(x => x.IsDefault);
                    if (defaultAccount == null)
                        return null;
                    locateRequest.Account = defaultAccount.Value;
                    locateRequest.ClientID = defaultAccount.Name;
                }
                //Else if account is present then set client id based on that account
                else
                {
                    var reqAccount = accounts.EventData.FirstOrDefault(x => x.Value == locateRequest.Account);
                    //If no respective account is found against the set order.account then copy it as it is.
                    locateRequest.ClientID = reqAccount?.Name ?? locateRequest.Account;
                }
            }
            return locateRequest;
        }
    }
}
