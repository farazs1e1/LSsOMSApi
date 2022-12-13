using OMSServices.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OMSServices.Services
{
    public interface IStaticDataSubscriptionService
    {
        Task FetchAndCacheOverallDataAsync(QueryType queryType);
        void SubscribeForOverallData(QueryType queryType);
        void SubscribeForTraderRelationData(QueryType queryType);
    }

    public interface IStaticDataUnsubscribeService
    {
        void UnsubscribeForOverallData(List<QueryType> queryTypes);
        void UnsubscribeForTraderRelationData(List<QueryType> queryTypes);
    }
}
