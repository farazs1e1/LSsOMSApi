using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using OMSServices.Data;
using OMSServices.Enum;
using OMSServices.Services;
using OMSServices.Utils;
using System.Collections.Generic;

namespace OMSApi.EventListeners
{
    public static class OnApplicationStoppingListener
    {
        public static void OnApplicationStopping(this IApplicationBuilder app)
        {
            var logger = StaticLogger.CreateInstance("OnApplicationStoppingListener");
            logger.LogInformation("IHostApplicationLifetime ApplicationStopping: OnApplicationStopping Invoked.");

            var staticDataUnsubscribeService = app.ApplicationServices.GetService(typeof(IStaticDataUnsubscribeService)) as IStaticDataUnsubscribeService;

            StaticData.UnLoadAll();

            var queries = new List<QueryType>() { QueryType.Side, QueryType.Destination, QueryType.Account, QueryType.TIF, QueryType.CommType, QueryType.OrdType };

            staticDataUnsubscribeService.UnsubscribeForTraderRelationData(queries);

            staticDataUnsubscribeService.UnsubscribeForOverallData(queries);
        }
    }
}
