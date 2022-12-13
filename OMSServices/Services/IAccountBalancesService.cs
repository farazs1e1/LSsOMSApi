using System.Threading.Tasks;

namespace OMSServices.Services
{
    public interface IAccountBalancesService
    {
        Task<T> SubscribeAsync<T>(string userIdentifier, string userDesc, string boothId) where T : class;
        Task<string> UnsubscribeAsync(string userIdentifier, string userDesc, string boothId);
        Task UnsubscribeAllConnectionsAsync(string identifier);
    }
}
