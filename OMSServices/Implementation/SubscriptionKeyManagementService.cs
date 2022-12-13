using Microsoft.Extensions.Caching.Distributed;
using OMSServices.Enum;
using OMSServices.Services;
using OMSServices.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OMSServices.Implementation
{
    class SubscriptionKeyManagementService : ISubscriptionKeyManagementService
    {
        private readonly IDistributedCache distributedCache;

        public SubscriptionKeyManagementService(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        public bool CheckAndSaveSubscriptionKeyInCache(string userIdentifier, string userDesc, string boothId, QueryType queryType)
        {
            string key = GetSubscriptionKey(userIdentifier, userDesc, boothId);
            string queryTypeString = queryType.ToString();

            var subscriptions = distributedCache.Get(key).FromBytes<List<string>>() ?? new List<string>();
            if (subscriptions.Contains(queryTypeString))
            {
                return false;
            }
            subscriptions.Add(queryTypeString);
            distributedCache.Set(key, subscriptions.ToBytes());
            return true;
        }

        public string GetSubscriptionKey(string userIdentifier, string userDesc, string boothId)
        {
            return $"{userIdentifier}/{boothId}_{userDesc}";
        }

        //public async Task<bool> CheckAndSaveSubscriptionKeyInCacheAsync(string userDesc, string boothId, QueryType queryType, bool isProvider)
        //{
        //    string key = GetSubscriptionKey(userDesc, boothId, isProvider);
        //    var subscriptions = (await distributedCache.GetAsync(key)).FromBytes<List<string>>() ?? new List<string>();
        //    if (subscriptions.Contains(queryType.ToString()))
        //        return false;

        //    subscriptions.Add(queryType.ToString());
        //    await distributedCache.SetAsync(key, subscriptions.ToBytes());
        //    return true;
        //}

        public async Task RemoveSubscriptionKeyFromCacheAsync(string userIdentifier, string userDesc, string boothId, QueryType queryType)
        {
            string key = GetSubscriptionKey(userIdentifier, userDesc, boothId);
            var subscriptions = (await distributedCache.GetAsync(key)).FromBytes<List<string>>();
            if (subscriptions != null && subscriptions.Count > 0)
            {
                subscriptions.Remove(queryType.ToString());
                if (subscriptions.Count > 0)
                {
                    await distributedCache.SetAsync(key, subscriptions.ToBytes());
                }
                else
                {
                    await distributedCache.RemoveAsync(key);
                }
            }
        }

        //public async Task RemoveSubscriptionKeyFromCacheAsync(string userDesc, string boothId, QueryType queryType, bool isProvider)
        //{
        //    var key = GetSubscriptionKey(userDesc, boothId, isProvider);
        //    var subscriptions = (await distributedCache.GetAsync(key)).FromBytes<List<string>>();
        //    if (subscriptions != null && subscriptions.Count > 0)
        //    {
        //        subscriptions.Remove(queryType.ToString());
        //        if (subscriptions.Count > 0)
        //        {
        //            await distributedCache.SetAsync(key, subscriptions.ToBytes());
        //        }
        //        else
        //        {
        //            await distributedCache.RemoveAsync(key);
        //        }
        //    }
        //}

        //public string GetSubscriptionKey(string userDesc, string boothId, bool isProvider)
        //{
        //    string prefix = isProvider ? boothId : userDesc;
        //    return $"{prefix}/{boothId}_{userDesc}";
        //}

        //public (string userDesc, string boothId, bool isProvider) DecomposeSubscriptionKey(string subscriptionKey)
        //{
        //    var keys = subscriptionKey.Split("/");
        //    var values = keys[1].Split("_");
        //    return (values[1], values[0], keys[0] == values[0]);
        //}

        //public string GetConnectionKey(string userIdentifier)
        //{
        //    return $"{userIdentifier}_OMSConnections";
        //}
    }
}
