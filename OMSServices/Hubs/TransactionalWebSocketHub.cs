using LS.WebSocketServer.Implementation;
using LS.WebSocketServer.Models;
using Microsoft.AspNetCore.Http;
using OMSServices.Services;
using System.Threading.Tasks;

namespace OMSServices.Hubs
{
    public class TransactionalWebSocketHub : WebSocketHub
    {
        private readonly ISocketConnectionService socketConnectionService;

        public TransactionalWebSocketHub(ISocketConnectionService socketConnectionService)
        {
            this.socketConnectionService = socketConnectionService;
        }

        public override async Task OnConnectedAsync(HttpContext context, SocketConnection socketConnection)
        {
            if (!await socketConnectionService.AddConnectionAsync(context.User))
            {
                context.Abort();
                return;
            }
            await base.OnConnectedAsync(context, socketConnection);
        }

        public override async Task OnDisconnectedAsync(SocketConnection socketConnection)
        {
            await socketConnectionService.RemoveConnectionAsync(socketConnection.Sub);
            await base.OnDisconnectedAsync(socketConnection);
        }
    }
}
