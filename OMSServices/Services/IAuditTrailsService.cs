using System.Threading.Tasks;

namespace OMSServices.Services
{
    public interface IAuditTrailsService
    {
        Task<T> SubscribeAsync<T>(string userIdentifier, string userDesc, string boothId, long qOrderID) where T : class;
        Task<string> UnsubscribeAsync(string userIdentifier, string userDesc, string boothId, long qOrderID);
        Task UnsubscribeAllConnectionsAsync(string identifier);
    }
}
