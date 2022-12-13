using OMSServices.Enum;
using OMSServices.Services;
using OMSServices.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Dynamic;
using Client.Communication;
using Client.Communication.Interfaces;
using Client.Global.Extensions;
using System.Reactive.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using OMSServices.Utils;
using System.Linq;
using OMSServices.Common;
using System;

namespace OMSServices.Implementation
{
    class StaticDataService : IStaticDataService
    {
        private readonly RequestInformation requestInformation;

        private readonly IQueryGenerator queryGenerator;
        private readonly ILogger<StaticDataService> logger;
        private readonly IConfiguration _configuration;

        //private readonly IDistributedCache distributedCache;
        private readonly IMemoryCache memoryCache;

        public StaticDataService(
            RequestInformation requestInformation,
            IQueryGenerator queryGenerator,
            ILogger<StaticDataService> logger,
            IMemoryCache memoryCache,
            IConfiguration configuration
            /*,
            IDistributedCache distributedCache*/)
        {
            this.logger = logger;
            this.queryGenerator = queryGenerator;
            this.requestInformation = requestInformation;
            this.memoryCache = memoryCache;
            _configuration = configuration;
            //this.distributedCache = distributedCache;
        }

        public async Task<ResultDataObject<T>> GetStaticDataAsync<T>(QueryType queryType, string userDesc, string boothID, string userIdentifier) where T : class
        {
            return await requestInformation.WatchRequestTimeAsync($"Request {queryType} static data from provider layer", async () =>
            {
                string key = QueryTypeExtensions.GenerateCacheKeyForStaticData(queryType, userIdentifier);

                ResultDataObject<T> staticDataResult = null;
                object stringResult = memoryCache.Get(key);
                if (stringResult == null)
                {
                    if (queryGenerator.HasContinuousQuery(queryType))
                        return await GetStaticDataOfUser<T>(key, queryType, userDesc, boothID, false);

                    stringResult = await GetDataAsync(queryType, userDesc, boothID, null);
                    if (stringResult == null)
                        return staticDataResult;

                    staticDataResult = JsonConverter.DeserializeObject<ResultDataObject<T>>(stringResult.ToString());
                    if (staticDataResult.EventData.Count > 0)
                    {
                        memoryCache.Set(key, stringResult, new MemoryCacheEntryOptions
                        {
                            SlidingExpiration = _configuration.MemoryCacheSlidingExpiry(),
                            AbsoluteExpiration = _configuration.MemoryCacheAbsoluteExpiry()
                        });
                    }
                }

                if (queryGenerator.HasContinuousQuery(queryType))
                    return await GetStaticDataOfUser<T>(key, queryType, userDesc, boothID, true);

                return staticDataResult ?? JsonConverter.DeserializeObject<ResultDataObject<T>>(stringResult.ToString());
            });
        }

        public async Task<ResultDataObject<T>> GetStaticDataEasyToBorrowAsync<T>(string userDesc, string accountValue, string symbol) where T : class
        {
            string cacheKeyEtbHtb = QueryTypeExtensions.GenerateCacheKeyForEtbHtb(accountValue, symbol);

            if (memoryCache.Get(cacheKeyEtbHtb) is string etbHtbSerialized)
            {
                return JsonConverter.DeserializeObject<ResultDataObject<T>>(etbHtbSerialized);
            }

            string cacheKey = QueryTypeExtensions.GenerateCacheKeyForUnfilteredStaticData(QueryType.Account);
            var overallStaticDataAccounts = memoryCache.Get<IDictionary<string, StaticDataValues>>(cacheKey);

            StaticDataValues account = overallStaticDataAccounts[accountValue];

            ExpandoObject data = await FetchStaticDataAsync(QueryType.ETBHTB, userDesc, account.Booth, symbol);
            if (data == null)
            {
                return null;
            }

            var dataSerialized = JsonConverter.SerializeObject(data);
            var resultantData = JsonConverter.DeserializeObject<ResultDataObject<T>>(dataSerialized);

            if (resultantData.EventData.Any())
            {
                memoryCache.Set(cacheKeyEtbHtb, dataSerialized, new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(2) // TODO get this reviewed.
                });
            }
            return resultantData;
        }

        private async Task<ResultDataObject<T>> GetStaticDataOfUser<T>(string cacheKey, QueryType queryType, string userDesc, string boothId, bool fromCache) where T : class
        {
            List<TraderRelationWith> traderRelationWithList;
            if (fromCache)
            {
                traderRelationWithList = memoryCache.Get<List<TraderRelationWith>>(cacheKey);

                // if the list is empty then we have to grab booth-specific (i.e. fallback) static-data, from cache.
                if (traderRelationWithList.Count < 1)
                    traderRelationWithList = memoryCache.Get<List<TraderRelationWith>>(QueryTypeExtensions.GenerateCacheKeyForFallbackStaticData(queryType, boothId));
            }
            else
            {
                bool fallback = false;
                var queryObject = queryGenerator.GetOneTimeQueryObjectForTraderRelation(queryType, userDesc, boothId, fallback);
                var endPoint = EndPointManager.Instance.GetEndPoint("StaticDataEndPoint") as ICommunicationEndPoint<ExpandoObject, ExpandoObject>;

                if ((await endPoint.Send(queryObject).FirstAsync()) is not IDictionary<string, object> data)
                    return null;

                traderRelationWithList = data["EventData"].GetTraderRelationWithValues().ToList();

                // if the user-specific static-data is empty then we have to fetch booth-specific (i.e. fallback) static-data, from server.
                if (traderRelationWithList.Count < 1)
                {
                    fallback = true;
                    queryObject = queryGenerator.GetOneTimeQueryObjectForTraderRelation(queryType, userDesc, boothId, fallback);
                    if (await endPoint.Send(queryObject).FirstAsync() is not IDictionary<string, object> fallbackData)
                        return null;

                    traderRelationWithList = fallbackData["EventData"].GetTraderRelationWithValues().ToList();
                }

                var options = new MemoryCacheEntryOptions { SlidingExpiration = _configuration.MemoryCacheSlidingExpiry(), AbsoluteExpiration = _configuration.MemoryCacheAbsoluteExpiry() };
                if (fallback)
                {
                    // now save user-specific static-data as empty list,
                    // so the next-time program can know that it has to grab booth-specific (i.e. fallback) static-data from cache.
                    // then save the fetched static-data as booth-specific (i.e. as fallback)

                    memoryCache.Set(cacheKey, new List<TraderRelationWith>(), options);

                    string cacheKeyFallback = QueryTypeExtensions.GenerateCacheKeyForFallbackStaticData(queryType, boothId);
                    memoryCache.Set(cacheKeyFallback, traderRelationWithList, options);
                }
                else
                {
                    memoryCache.Set(cacheKey, traderRelationWithList, options);
                }
            }

            var overallStaticDataDict = memoryCache.Get<IDictionary<string, StaticDataValues>>(QueryTypeExtensions.GenerateCacheKeyForUnfilteredStaticData(queryType));

            var staticDataFiltered = traderRelationWithList.Where(t => overallStaticDataDict.ContainsKey(t.RecordID)).Select(t => overallStaticDataDict[t.RecordID]).ToList();

            var resultantObject = new ResultDataObject<StaticDataValues> { EventType = (int)EventType.CurrentState, EventData = staticDataFiltered };

            return JsonConverter.DeserializeObject<ResultDataObject<T>>(JsonConverter.SerializeObject(resultantObject));
        }

        private async Task<string> GetDataAsync(QueryType query, string userDesc, string boothID, List<string> accountsIfApplicable)
        {
            var queryInfo = queryGenerator.GetOneTimeQuery(query, userDesc, boothID, accountsIfApplicable);
            var endPoint = EndPointManager.Instance.GetEndPoint("StaticDataEndPoint") as ICommunicationEndPoint<ExpandoObject, ExpandoObject>;
            var data = await endPoint.Send(queryInfo.GetValue<ExpandoObject>("QueryObject")).FirstAsync();

            logger.LogInformation($"Static data server returned: {data}");

            if (data != null && data.GetValueOrDefault<List<object>>("EventData")?.Count <= 0)
            {
                var fallbackQueryInfo = queryGenerator.GetOneTimeQuery(query, userDesc, boothID, accountsIfApplicable, true);
                data = await endPoint.Send(fallbackQueryInfo.GetValue<ExpandoObject>("QueryObject")).FirstAsync();
            }

            return data != null ? JsonConverter.SerializeObject(data) : null;
        }

        private async Task<ExpandoObject> FetchStaticDataAsync(QueryType queryType, string userDesc, string boothID, string symbol)
        {
            var oneTimeQuery = queryGenerator.GetOneTimeQuery(queryType, userDesc, boothID, null, symbol);
            var queryObject = oneTimeQuery.GetValue<ExpandoObject>("QueryObject");

            var endPoint = EndPointManager.Instance.GetEndPoint("StaticDataEndPoint") as ICommunicationEndPoint<ExpandoObject, ExpandoObject>;
            var data = await endPoint.Send(queryObject);

            return data;
        }
    }
}
