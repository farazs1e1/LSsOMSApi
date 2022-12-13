using Client.Communication;
using Client.Communication.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OMSServices.Common;
using OMSServices.Enum;
using OMSServices.Models;
using OMSServices.Services;
using OMSServices.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace OMSServices.Implementation
{
    class StaticDataSubscriptionService : IStaticDataSubscriptionService, IStaticDataUnsubscribeService
    {
        private readonly IQueryGenerator queryGenerator;
        private readonly ILogger<StaticDataService> logger;
        private readonly IMemoryCache memoryCache;
        private readonly MemoryCacheEntryOptions _memoryCacheEntryOptions = new() { Priority = CacheItemPriority.NeverRemove };

        public StaticDataSubscriptionService(IQueryGenerator queryGenerator, ILogger<StaticDataService> logger, IMemoryCache memoryCache)
        {
            this.logger = logger;
            this.queryGenerator = queryGenerator;
            this.memoryCache = memoryCache;

            _memoryCacheEntryOptions.RegisterPostEvictionCallback(PostEvictionCallback);
        }

        public async Task FetchAndCacheOverallDataAsync(QueryType queryType)
        {
            var queryObject = queryGenerator.GetSelectAllQueryObject(queryType);
            if (queryObject == null)
                return;

            var endPoint = EndPointManager.Instance.GetEndPoint("StaticDataEndPoint") as ICommunicationEndPoint<ExpandoObject, ExpandoObject>;

            var data = (await endPoint.Send(queryObject).FirstAsync()) as IDictionary<string, object>;

            if (data == null)
            {
                logger.LogWarning("{QueryType}: Overall-static-data is null.", queryType);
            }

            IDictionary<string, StaticDataValues> overallStaticData = new Dictionary<string, StaticDataValues>();

            foreach (var eventData in data["EventData"].GetStaticDataValues())
                overallStaticData.Add(eventData.Value, eventData);

            memoryCache.Set(QueryTypeExtensions.GenerateCacheKeyForUnfilteredStaticData(queryType), overallStaticData, _memoryCacheEntryOptions);
        }

        public void SubscribeForOverallData(QueryType queryType)
        {
            var queryObject = queryGenerator.GetSubscribeQueryObjectForOverallData(queryType);
            if (queryObject == null)
                return;

            var endPoint = EndPointManager.Instance.GetEndPoint("StaticDataEndPoint") as ICommunicationEndPoint<ExpandoObject, ExpandoObject>;

            _ = endPoint
                .Send(queryObject)
                .Where(x => (long)(x as IDictionary<string, object>)["EventType"] != (long)EventType.CurrentState)
                .Do((ExpandoObject update) =>
                {
                    if (update is not IDictionary<string, object> updateFromDb)
                        return;

                    StaticDataValues eventData = updateFromDb["EventData"].GetStaticDataValues().FirstOrDefault();
                    if (eventData == null)
                        return;

                    string cacheKey = QueryTypeExtensions.GenerateCacheKeyForUnfilteredStaticData(queryType);

                    var overallStaticData = memoryCache.Get<IDictionary<string, StaticDataValues>>(cacheKey);
                    if (overallStaticData == null)
                        return;

                    switch ((long)updateFromDb["EventType"])
                    {
                        case (long)EventType.Created:
                            overallStaticData.Add(eventData.Value, eventData);
                            break;

                        case (long)EventType.Updated:
                            if (overallStaticData.ContainsKey(eventData.Value))
                            {
                                overallStaticData[eventData.Value] = eventData;
                            }
                            break;

                        case (long)EventType.Removed:
                            overallStaticData.Remove(eventData.Value);
                            break;
                    }

                    logger.LogInformation("Successfully updated overallStaticData: cacheKey: {CacheKey}", cacheKey);
                })
                .Subscribe();
        }

        public void SubscribeForTraderRelationData(QueryType queryType)
        {
            IDictionary<string, object> continuousQuery = queryGenerator.GetContinuousQuery(queryType);
            if (continuousQuery == null)
                return;

            var endPoint = EndPointManager.Instance.GetEndPoint("StaticDataEndPoint") as ICommunicationEndPoint<ExpandoObject, ExpandoObject>;

            _ = endPoint
                .Send(continuousQuery["QueryObject"] as ExpandoObject)
                .Where(x => (long)(x as IDictionary<string, object>)["EventType"] != (long)EventType.CurrentState)
                .Do((ExpandoObject update) =>
                {
                    if (update is not IDictionary<string, object> updateFromDb)
                        return;

                    var eventData = updateFromDb["EventData"].GetTraderRelationWithValues().FirstOrDefault();

                    if (eventData == null)
                        return;

                    string cacheKey;
                    // When the Username is null or empty, that means the update must be for booth-specific (i.e. fallback) static-data.
                    if (string.IsNullOrEmpty(eventData.Username))
                        cacheKey = QueryTypeExtensions.GenerateCacheKeyForFallbackStaticData(queryType, eventData.BoothID);
                    else
                        cacheKey = QueryTypeExtensions.GenerateCacheKeyForStaticData(queryType, UserClaims.GenerateUserIdentifier(eventData.Username, eventData.BoothID));

                    var traderRelationWiths = memoryCache.Get<List<TraderRelationWith>>(cacheKey);
                    if (traderRelationWiths == null)
                        return;

                    switch ((long)updateFromDb["EventType"])
                    {
                        case (long)EventType.Created:
                            traderRelationWiths.Add(eventData);
                            break;

                        case (long)EventType.Updated:
                            var updateIndex = traderRelationWiths.FindIndex(t => eventData.Key.Equals(t.Key));
                            if (updateIndex >= 0)
                                traderRelationWiths[updateIndex] = eventData;
                            break;

                        case (long)EventType.Removed:
                            int deleteIndex = traderRelationWiths.FindIndex(t => eventData.Key.Equals(t.Key));
                            if (deleteIndex >= 0)
                                traderRelationWiths.RemoveAt(deleteIndex);
                            break;
                    }

                    logger.LogInformation("Successfully updated traderRelationWiths: cacheKey: {CacheKey}", cacheKey);
                })
                .Subscribe();
        }

        public void UnsubscribeForTraderRelationData(List<QueryType> queryTypes)
        {
            var endPoint = EndPointManager.Instance.GetEndPoint("StaticDataEndPoint") as ICommunicationEndPoint<ExpandoObject, ExpandoObject>;

            foreach (QueryType queryType in queryTypes)
            {
                IDictionary<string, object> unsubscribeQuery = queryGenerator.GetUnsubscribeQuery(queryType);
                if (unsubscribeQuery != null)
                    _ = endPoint.Send(unsubscribeQuery["QueryObject"] as ExpandoObject);
            }
        }

        public void UnsubscribeForOverallData(List<QueryType> queryTypes)
        {
            var endPoint = EndPointManager.Instance.GetEndPoint("StaticDataEndPoint") as ICommunicationEndPoint<ExpandoObject, ExpandoObject>;

            foreach (QueryType queryType in queryTypes)
            {
                var queryObject = queryGenerator.GetUnsubscribeQueryObjectForOverallData(queryType);
                if (queryObject != null)
                    _ = endPoint.Send(queryObject);
            }
        }


        /// <summary>
        /// PRIVATE METHODS
        /// </summary>

        private void PostEvictionCallback(object key, object letter, EvictionReason reason, object state)
        {
            logger.LogWarning("{Key} has been evicted from the cache. Letter: {Letter}. EvictionReason: {EvictionReason}. State: {State}.", key, letter, reason, state);
        }
    }
}
