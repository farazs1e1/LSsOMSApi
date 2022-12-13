using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OMSServices.Services;
using OMSServices.Utils;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OMSServices.Implementation
{
    class SocketConnectionService : ISocketConnectionService
    {
        private readonly IDistributedCache distributedCache;
        private readonly IOrderSubscriptionService orderSubscriptionService;
        private readonly ILocatesService locatesService;
        private readonly IAccountBalancesService buyingPowerService;
        private readonly IAuditTrailsService auditTrailsService;
        private readonly ILogger<SocketConnectionService> _logger;

        public SocketConnectionService(
            IDistributedCache distributedCache,
            IOrderSubscriptionService orderSubscriptionService,
            ILocatesService locatesService,
            IAccountBalancesService buyingPowerService,
            IAuditTrailsService auditTrailsService,
            ILogger<SocketConnectionService> logger)
        {
            this.distributedCache = distributedCache;
            this.orderSubscriptionService = orderSubscriptionService;
            this.locatesService = locatesService;
            this.buyingPowerService = buyingPowerService;
            this.auditTrailsService = auditTrailsService;
            _logger = logger;
        }

        public async Task<bool> AddConnectionAsync(ClaimsPrincipal claimsPrincipal)
        {
            string connectionKey = null;
            try
            {
                var userIdentifier = claimsPrincipal.UserIdentifier();
                var maxConnectionAllowed = claimsPrincipal.MaxConnectionAllowed();

                connectionKey = GetConnectionKey(userIdentifier);
                var keyString = await distributedCache.GetStringAsync(connectionKey);
                _ = int.TryParse(keyString, out int connections);

                // validate max connection
                if (maxConnectionAllowed <= connections)
                {
                    return false;
                }

                // add connectionId to cache
                await distributedCache.SetStringAsync(connectionKey, (connections + 1).ToString());
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while adding connectionId to cache. ConnectionKey: {0} ExceptionMessage: {1}", connectionKey, ex.Message);
                return false;
            }
        }

        public async Task RemoveConnectionAsync(ClaimsPrincipal claimsPrincipal)
        {
            // remove connection id
            var userIdentifier = claimsPrincipal.UserIdentifier();
            if (string.IsNullOrWhiteSpace(userIdentifier))
                return;

            // remove connection from cache
            var connectionKey = GetConnectionKey(userIdentifier);
            _ = int.TryParse(await distributedCache.GetStringAsync(connectionKey), out int connections);
            if (connections > 0)
            {
                if (connections == 1)
                {
                    await UnsubscribeFromAllAsync(userIdentifier);
                    await distributedCache.RemoveAsync(connectionKey);
                }
                else
                {
                    await distributedCache.SetStringAsync(connectionKey, (connections - 1).ToString());
                }
            }
        }

        public async Task RemoveConnectionAsync(string userIdentifier)
        {
            // remove connection id
            if (string.IsNullOrWhiteSpace(userIdentifier))
                return;

            // remove connection from cache
            var connectionKey = GetConnectionKey(userIdentifier);
            _ = int.TryParse(await distributedCache.GetStringAsync(connectionKey), out int connections);
            if (connections > 0)
            {
                if (connections == 1)
                {
                    await UnsubscribeFromAllAsync(userIdentifier);
                    await distributedCache.RemoveAsync(connectionKey);
                }
                else
                {
                    await distributedCache.SetStringAsync(connectionKey, (connections - 1).ToString());
                }
            }
        }

        private async Task UnsubscribeFromAllAsync(string userIdentifier)
        {
            await orderSubscriptionService.UnsubscribeAllConnectionsAsync(userIdentifier);
            await locatesService.UnsubscribeAllConnectionsAsync(userIdentifier);
            await buyingPowerService.UnsubscribeAllConnectionsAsync(userIdentifier);
            await auditTrailsService.UnsubscribeAllConnectionsAsync(userIdentifier);
        }

        private static string GetConnectionKey(string userIdentifier)
        {
            return $"{userIdentifier}_OMSConnections";
        }
    }
}
