using OMSServices.Services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reactive.Linq;
using OMSServices.Enum;
using Client.Global.Extensions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using OMSServices.Utils;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using OMSServices.Hubs;
using Microsoft.Extensions.Logging;
using LS.WebSocketServer.Services;
using Client.Communication.Interfaces;
using OMSServices.Models;
using Client.Communication;

namespace OMSServices.Implementation
{
    class OrderSubscriptionService : IOrderSubscriptionService
    {
        private readonly static IReadOnlyDictionary<QueryType, object> s_locks = new List<QueryType>() { QueryType.Orders, QueryType.OpenOrders, QueryType.Executions, QueryType.Positions, QueryType.OptionOrders }.ToDictionary(x => x, _ => new object());

        private readonly IDistributedCache distributedCache;
        private readonly IRedisService redisService;
        private readonly ISubscriptionKeyManagementService subscriptionKeyManagementService;
        private readonly IQueryGenerator queryGenerator;
        private readonly IStaticDataService staticDataService;
        private readonly IHubContext<TransactionalHub> hubContext;
        private readonly IWebSocketContext webSocketContext;
        private readonly ILogger<OrderSubscriptionService> logger;

        public OrderSubscriptionService(
            IDistributedCache distributedCache,
            IRedisService redisService,
            ISubscriptionKeyManagementService subscriptionKeyManagementService,
            IQueryGenerator queryGenerator,
            IStaticDataService staticDataService,
            IHubContext<TransactionalHub> hubContext,
            IWebSocketContext webSocketContext,
            ILogger<OrderSubscriptionService> logger
        )
        {
            this.distributedCache = distributedCache;
            this.redisService = redisService;
            this.subscriptionKeyManagementService = subscriptionKeyManagementService;
            this.queryGenerator = queryGenerator;
            this.staticDataService = staticDataService;
            this.hubContext = hubContext;
            this.webSocketContext = webSocketContext;
            this.logger = logger;
        }

        public async Task<T> SubscribeAsync<T>(string userIdentifier, string userDesc, string boothId, QueryType queryType, List<string> accounts = null) where T : class
        {
            var endPoint = Client.Communication.EndPointManager.Instance.GetEndPoint("QueryEndPoint") as ICommunicationEndPoint<ExpandoObject, ExpandoObject>;
            List<string> accountIds = accounts == null || accounts.IsEmpty() ? await GetAccountIdsAsync(userDesc, boothId, userIdentifier) : accounts;
            if (accountIds == null || accountIds.IsEmpty())
            {
                logger.LogWarning("Accounts static data is null or empty. UserDesc: {UserDesc}, BoothId: {BoothId}, QueryType: {QueryType}", userDesc, boothId, queryType);
                return null;
            }

            // Check if the key is already subscribed or not
            lock (s_locks[queryType])
            {
                subscriptionKeyManagementService.CheckAndSaveSubscriptionKeyInCache(userIdentifier, userDesc, boothId, queryType);

                // At first, unsubscribe the previous subscription, so that the last subscription gets removed, with the provided accounts at that time. and then new subscription will now continue to post the updates.
                UnsubscribeAsync(userIdentifier, userDesc, boothId, queryType).Wait();

                try
                {
                    var continuousQuery = queryGenerator.GetContinuousQuery(queryType, userDesc, boothId, accountIds) as IDictionary<string, object>;
                    var queryObject = queryType == QueryType.Positions ? (continuousQuery["QueryObject"] as IDictionary<string, object>)["QueryObject"] : continuousQuery["QueryObject"];

                    endPoint.Send(queryObject as ExpandoObject)
                        .Where(x => x.GetValue<long>("EventType") != (long)EventType.CurrentState)
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
                    subscriptionKeyManagementService.RemoveSubscriptionKeyFromCacheAsync(userIdentifier, userDesc, boothId, queryType).Wait();
                    return null;
                }
            }

            var parameter = queryGenerator.GetOneTimeQuery(queryType, userDesc, boothId, accountIds) as IDictionary<string, object>;
            var oneTimeQueryObject = queryType == QueryType.Positions ? (parameter["QueryObject"] as IDictionary<string, object>)["QueryObject"] : parameter["QueryObject"];
            var result = await endPoint.Send(oneTimeQueryObject as ExpandoObject).FirstAsync();
            return result.ConvertExpandoObjectTo<T>();
        }

        public async Task<string> UnsubscribeAsync(string userIdentifier, string userDesc, string boothId, QueryType queryType)
        {
            var endPoint = Client.Communication.EndPointManager.Instance.GetEndPoint("QueryEndPoint") as ICommunicationEndPoint<ExpandoObject, ExpandoObject>;
            var accountIds = await GetAccountIdsAsync(userDesc, boothId, userIdentifier);

            var unsubscribeRequest = queryGenerator.GetUnsubscribeQuery(queryType, userDesc, boothId, accountIds) as IDictionary<string, object>;
            var unsubscribeQueryObject = unsubscribeRequest["QueryObject"] as IDictionary<string, object>;
            var queryObjectRequest = queryType == QueryType.Positions ? unsubscribeQueryObject["QueryObject"] : unsubscribeRequest["QueryObject"];
            _ = endPoint.Send(queryObjectRequest as ExpandoObject);

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
                        if (queryType == QueryType.Orders || queryType == QueryType.OpenOrders || queryType == QueryType.Positions || queryType == QueryType.Executions)
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
    }
}
