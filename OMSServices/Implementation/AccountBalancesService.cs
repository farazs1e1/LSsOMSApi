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
using System;
using Microsoft.Extensions.Caching.Distributed;
using OMSServices.Utils;

namespace OMSServices.Implementation
{
    class AccountBalancesService : IAccountBalancesService
    {
        private readonly static object s_locker = new();

        private readonly ISubscriptionKeyManagementService subscriptionKeyManagementService;
        private readonly IRedisService redisService;
        private readonly IDistributedCache distributedCache;
        private readonly IQueryGenerator queryGenerator;
        private readonly IHubContext<TransactionalHub> hubContext;
        private readonly IWebSocketContext webSocketContext;
        private readonly ILogger<AccountBalancesService> logger;

        public AccountBalancesService(
            ISubscriptionKeyManagementService subscriptionKeyManagementService,
            IRedisService redisService,
            IDistributedCache distributedCache,
            IQueryGenerator queryGenerator,
            IHubContext<TransactionalHub> hubContext,
            IWebSocketContext webSocketContext,
            ILogger<AccountBalancesService> logger
        )
        {
            this.subscriptionKeyManagementService = subscriptionKeyManagementService;
            this.redisService = redisService;
            this.distributedCache = distributedCache;
            this.queryGenerator = queryGenerator;
            this.hubContext = hubContext;
            this.webSocketContext = webSocketContext;
            this.logger = logger;
        }

        public async Task<string> UnsubscribeAsync(string userIdentifier, string userDesc, string boothId)
        {
            QueryType queryType = QueryType.NetLimitSummary;
            var endPoint = EndPointManager.Instance.GetEndPoint("QueryEndPoint") as ICommunicationEndPoint<ExpandoObject, ExpandoObject>;

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
                        if (queryType == QueryType.NetLimitSummary)
                            await UnsubscribeAsync(identifier, userDesc, boothId);
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

        public async Task<T> SubscribeAsync<T>(string userIdentifier, string userDesc, string boothId) where T : class
        {
            var endPoint = EndPointManager.Instance.GetEndPoint("QueryEndPoint") as ICommunicationEndPoint<ExpandoObject, ExpandoObject>;

            QueryType queryType = QueryType.NetLimitSummary;
            //Check if the key is already subscribed or not
            bool addedToCache;
            lock (s_locker)
            {
                addedToCache = subscriptionKeyManagementService.CheckAndSaveSubscriptionKeyInCache(userIdentifier, userDesc, boothId, queryType);
            }
            if (addedToCache)
            {
                try
                {
                    var continuousQuery = queryGenerator.GetContinuousQuery(queryType, userDesc, boothId, null) as IDictionary<string, object>;
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
            var queryInfo = queryGenerator.GetOneTimeQuery(queryType, userDesc, boothId, null);
            var result = await endPoint.Send(queryInfo.GetValue<ExpandoObject>("QueryObject")).FirstAsync();
            return result.ConvertExpandoObjectTo<T>();
        }

        private async Task PublishMessageToSocketConnectionsAsync<T>(string userIdentifier, string methodName, ExpandoObject publishMessage) where T : class
        {
            var convertedMessage = publishMessage.ConvertExpandoObjectTo<T>();
            await hubContext.Clients.User(userIdentifier).SendAsync(methodName, convertedMessage);
            await webSocketContext.SendToUserAsync(userIdentifier, new { Method = methodName, Data = convertedMessage });
        }
    }
}
