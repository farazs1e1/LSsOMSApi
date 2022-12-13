using OMSServices.Services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reactive.Linq;
using OMSServices.Enum;
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
using OMSServices.Data;

namespace OMSServices.Implementation
{
    class AuditTrailsService : IAuditTrailsService
    {
        private readonly static object s_locker = new();

        private readonly IDistributedCache distributedCache;
        private readonly ISubscriptionKeyManagementService subscriptionKeyManagementService;
        private readonly IQueryGenerator queryGenerator;
        private readonly IStaticDataService staticDataService;
        private readonly IHubContext<TransactionalHub> hubContext;
        private readonly IWebSocketContext webSocketContext;
        private readonly ILogger<OrderSubscriptionService> logger;

        public AuditTrailsService(
            IDistributedCache distributedCache,
            ISubscriptionKeyManagementService subscriptionKeyManagementService,
            IQueryGenerator queryGenerator,
            IStaticDataService staticDataService,
            IHubContext<TransactionalHub> hubContext,
            IWebSocketContext webSocketContext,
            ILogger<OrderSubscriptionService> logger
        )
        {
            this.distributedCache = distributedCache;
            this.subscriptionKeyManagementService = subscriptionKeyManagementService;
            this.queryGenerator = queryGenerator;
            this.staticDataService = staticDataService;
            this.hubContext = hubContext;
            this.webSocketContext = webSocketContext;
            this.logger = logger;
        }

        public async Task<T> SubscribeAsync<T>(string userIdentifier, string userDesc, string boothId, long qOrderID) where T : class
        {
            //Check if the key is already subscribed or not
            QueryType queryType = QueryType.AuditTrail;
            var endPoint = Client.Communication.EndPointManager.Instance.GetEndPoint("QueryEndPoint") as ICommunicationEndPoint<ExpandoObject, ExpandoObject>;
            var accounts = await GetAccountIdsAsync(userDesc, boothId, userIdentifier);
            string customQueryKey = $"{QueryType.AuditTrail}_{boothId}_{qOrderID}";
            if (accounts == null || accounts.IsEmpty())
            {
                logger.LogWarning("Accounts static data is null or empty. UserDesc: {UserDesc}, BoothId: {BoothId}, QueryType: {QueryType}", userDesc, boothId, queryType);
                return null;
            }

            bool addedToCache = false;
            lock (s_locker)
            {
                addedToCache = AddAuditTrailsOrderIdsInCacheAsync(userIdentifier, queryType, qOrderID).Result;
            }
            if (addedToCache)
            {
                try
                {
                    var continuousQuery = queryGenerator.GetContinuousQuery(new QueryGeneratorArguments
                    {
                        QueryType = queryType,
                        Accounts = accounts,
                        UserDesc = userDesc,
                        BoothID = boothId,
                        Fallback = false,
                        UpdateQuery = $"(QOrderID = {qOrderID}) AND (BoothID = '{boothId}')",
                        OrderBy = "VersionMaj ASC",
                        CustomQueryKey = customQueryKey
                    }) as IDictionary<string, object>;
                    ExpandoObject queryObject = continuousQuery["QueryObject"] as ExpandoObject;
                    endPoint.Send(queryObject)
                        //.Where(x => x.GetValue<Int64>("EventType") != -1)
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

            var parameter = queryGenerator.GetOneTimeQuery(new QueryGeneratorArguments
            {
                QueryType = queryType,
                Accounts = accounts,
                UserDesc = userDesc,
                BoothID = boothId,
                Fallback = false,
                UpdateQuery = $"(QOrderID = {qOrderID}) AND (BoothID = '{boothId}')",
                OrderBy = "VersionMaj ASC",
                CustomQueryKey = customQueryKey
            }) as IDictionary<string, object>;
            ExpandoObject oneTimeQueryObject = parameter["QueryObject"] as ExpandoObject;

            var result = await endPoint.Send(oneTimeQueryObject).FirstAsync();
            return result.ConvertExpandoObjectTo<T>();
        }

        public async Task<string> UnsubscribeAsync(string userIdentifier, string userDesc, string boothId, long qOrderID)
        {
            QueryType queryType = QueryType.AuditTrail;
            var accounts = await GetAccountIdsAsync(userDesc, boothId, userIdentifier);
            var unSubscribeReq = queryGenerator.GetUnsubscribeQuery(new QueryGeneratorArguments
            {
                QueryType = queryType,
                Accounts = accounts,
                UserDesc = userDesc,
                BoothID = boothId,
                Fallback = false,
                UpdateQuery = $"(QOrderID = {qOrderID}) AND (BoothID = '{boothId}')",
                OrderBy = "VersionMaj ASC",
                CustomQueryKey = $"{QueryType.AuditTrail}_{boothId}_{qOrderID}"
            }) as IDictionary<string, object>;
            ExpandoObject queryObject = unSubscribeReq["QueryObject"] as ExpandoObject;
            var endPoint = Client.Communication.EndPointManager.Instance.GetEndPoint("QueryEndPoint") as ICommunicationEndPoint<ExpandoObject, ExpandoObject>;
            _ = endPoint.Send(queryObject);

            //If unsub is successfull then remove the key from cache
            await subscriptionKeyManagementService.RemoveSubscriptionKeyFromCacheAsync(userIdentifier, userDesc, boothId, queryType);
            return "Success!";
        }

        public async Task UnsubscribeAllConnectionsAsync(string identifier)
        {
            var subscribedOrderIds = await GetAuditTrailsOrderIdsFromCacheAsync(identifier, QueryType.AuditTrail);
            (string userDesc, string boothId) = UserClaims.ParseUserIdentifier(identifier);
            foreach (var orderId in subscribedOrderIds)
            {
                await UnsubscribeAsync(identifier, userDesc, boothId, orderId);
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
            return accountsData.EventData.Select(x => x.Value).ToList();
        }

        private async Task<bool> AddAuditTrailsOrderIdsInCacheAsync(string userIdentifier, QueryType queryType, long qOrderID)
        {
            string key = $"{userIdentifier}_{queryType}_subscriptions";
            var subscriptions = (await distributedCache.GetAsync(key)).FromBytes<List<string>>() ?? new List<string>();
            if (subscriptions.Contains(qOrderID.ToString()))
                return false;

            subscriptions.Add(qOrderID.ToString());
            await distributedCache.SetAsync(key, subscriptions.ToBytes());
            return true;
        }

        private async Task<List<long>> GetAuditTrailsOrderIdsFromCacheAsync(string userIdentifier, QueryType queryType)
        {
            string key = $"{userIdentifier}_{queryType}_subscriptions";
            var subscriptions = (await distributedCache.GetAsync(key)).FromBytes<List<string>>() ?? new List<string>();
            return subscriptions.Select(x => long.Parse(x)).ToList();
        }
    }
}
