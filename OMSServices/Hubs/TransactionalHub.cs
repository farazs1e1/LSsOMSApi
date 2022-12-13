using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OMSServices.Services;
using System.Threading.Tasks;

namespace OMSServices.Hubs
{
    [Authorize]
    public class TransactionalHub : Hub
    {
        private readonly ISocketConnectionService socketConnectionService;

        public TransactionalHub(ISocketConnectionService socketConnectionService)
        {
            this.socketConnectionService = socketConnectionService;
        }

        public override async Task OnConnectedAsync()
        {
            if (!await socketConnectionService.AddConnectionAsync(Context.User))
            {
                Context.Abort();
                return;
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            await socketConnectionService.RemoveConnectionAsync(Context.User);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
