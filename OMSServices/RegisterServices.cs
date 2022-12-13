using Microsoft.Extensions.DependencyInjection;
using OMSServices.Implementation;
using OMSServices.Querry;
using OMSServices.Services;

namespace OMSServices
{
    public static class RegisterServices
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IOrderManagementService, OrderManagementService>();
            services.AddTransient<IStaticDataService, StaticDataService>();
            services.AddTransient<IStaticDataUnsubscribeService, StaticDataSubscriptionService>();
            services.AddTransient<IStaticDataSubscriptionService, StaticDataSubscriptionService>();
            services.AddTransient<ISocketConnectionService, SocketConnectionService>();
            services.AddTransient<IOrderSubscriptionService, OrderSubscriptionService>();
            services.AddTransient<IRedisService, RedisService>();
            services.AddTransient<IRedisWriteService, RedisService>();
            services.AddTransient<ISubscriptionKeyManagementService, SubscriptionKeyManagementService>();
            services.AddTransient<ILocatesService, LocatesService>();
            services.AddTransient<IAccountBalancesService, AccountBalancesService>();
            services.AddTransient<IAuditTrailsService, AuditTrailsService>();

            services.AddSingleton<IQueryGenerator, QueryGenerator>();
            services.AddSingleton<JwtTokenProvider>();
        }
    }
}
