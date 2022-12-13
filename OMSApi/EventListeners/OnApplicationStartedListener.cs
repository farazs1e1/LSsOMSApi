using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OMSServices.Data;
using OMSServices.Enum;
using OMSServices.Services;
using OMSServices.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace OMSApi.EventListeners
{
    public static class OnApplicationStartedListener
    {
        public static void OnApplicationStarted(this IApplicationBuilder app)
        {
            var logger = StaticLogger.CreateInstance("OnApplicationStartedListener");
            logger.LogInformation("IHostApplicationLifetime ApplicationStarted: OnApplicationStarted Invoked.");

            var staticDataSubscriptionService = app.ApplicationServices.GetRequiredService(typeof(IStaticDataSubscriptionService)) as IStaticDataSubscriptionService;
            var redisWriteService = app.ApplicationServices.GetRequiredService(typeof(IRedisWriteService)) as IRedisWriteService;
            var queryGenerator = app.ApplicationServices.GetRequiredService(typeof(IQueryGenerator)) as IQueryGenerator;

            redisWriteService.RemoveAllCacheEntries();

            StaticData.LoadAll(queryGenerator);

            var queries = new List<QueryType>() { QueryType.Side, QueryType.Destination, QueryType.Account, QueryType.TIF, QueryType.CommType, QueryType.OrdType };

            Task.WhenAll(queries.Select(queryType => staticDataSubscriptionService.FetchAndCacheOverallDataAsync(queryType))).Wait();

            queries.ForEach(queryType => staticDataSubscriptionService.SubscribeForOverallData(queryType));

            queries.ForEach(queryType => staticDataSubscriptionService.SubscribeForTraderRelationData(queryType));
        }
    }
}
