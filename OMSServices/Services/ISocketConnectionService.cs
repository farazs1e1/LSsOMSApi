using System.Security.Claims;
using System.Threading.Tasks;

namespace OMSServices.Services
{
    public interface ISocketConnectionService
    {
        Task<bool> AddConnectionAsync(ClaimsPrincipal claimsPrincipal);
        Task RemoveConnectionAsync(ClaimsPrincipal claimsPrincipal);
        Task RemoveConnectionAsync(string userIdentifier);
    }
}
