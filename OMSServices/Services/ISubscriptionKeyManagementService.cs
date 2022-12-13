using OMSServices.Enum;
using System.Threading.Tasks;

namespace OMSServices.Services
{
    public interface ISubscriptionKeyManagementService
    {
        bool CheckAndSaveSubscriptionKeyInCache(string userIdentifier, string userDesc, string boothId, QueryType queryType);

        //Task<bool> CheckAndSaveSubscriptionKeyInCacheAsync(string userDesc, string boothId, QueryType queryType, bool isProvider);

        //Task RemoveSubscriptionKeyFromCacheAsync(string userDesc, string boothId, QueryType queryType, bool isProvider);
        Task RemoveSubscriptionKeyFromCacheAsync(string userIdentifier, string userDesc, string boothId, QueryType queryType);

        string GetSubscriptionKey(string userIdentifier, string userDesc, string boothId);

        //(string userDesc, string boothId, bool isProvider) DecomposeSubscriptionKey(string subscriptionKey);

        //string GetConnectionKey(string userIdentifier);
    }
}
