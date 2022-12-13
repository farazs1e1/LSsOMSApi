using OMSServices.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OMSServices.Services
{
    public interface IOrderSubscriptionService
    {
        Task<T> SubscribeAsync<T>(string userIdentifier, string userDesc, string boothId, QueryType queryType, List<string> accounts = null) where T : class;
        Task<string> UnsubscribeAsync(string userIdentifier, string userDesc, string boothId, QueryType queryType);
        Task UnsubscribeAllConnectionsAsync(string identifier);
    }
}
