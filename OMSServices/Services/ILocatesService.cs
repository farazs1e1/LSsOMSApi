using OMSServices.Enum;
using OMSServices.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OMSServices.Services
{
    public interface ILocatesService
    {
        Task<object> LocateRequest(BindableOEMessage bindableOEMessage, string userIdentifier);
        Task<object> LocateAcquire(BindableOEMessage bindableOEMessage, string userIdentifier);
        Task<T> SubscribeAsync<T>(string userIdentifier, string userDesc, string boothId, QueryType queryType, string account = null, string symbol = null) where T : class;
        Task<string> UnsubscribeAsync(string userIdentifier, string userDesc, string boothId, QueryType queryType);
        Task UnsubscribeAllConnectionsAsync(string identifier);

    }
}
