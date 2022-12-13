using Microsoft.AspNetCore.SignalR;
using OMSServices.Utils;

namespace OMSServices.Implementation
{
    public class SignalRUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.UserIdentifier();
        }
    }
}
