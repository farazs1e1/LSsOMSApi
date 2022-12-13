using LS.WebSocketServer.Services;
using OMSServices.Utils;
using System.Security.Claims;

namespace OMSServices.Implementation
{
    public class WebSocketUserIdProvider : IWebSocketUserIdProvider
    {
        public string GetUserId(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal?.UserIdentifier();
        }
    }
}
