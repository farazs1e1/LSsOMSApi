using Microsoft.Extensions.Configuration;
using System;

namespace OMSServices.Utils
{
    public static class ConfigurationCustomExtension
    {
        public static TimeSpan MemoryCacheSlidingExpiry(this IConfiguration configuration)
        {
            return TimeSpan.FromSeconds(int.Parse(configuration["MemoryCacheExpirySeconds:Sliding"]));
        }

        public static DateTimeOffset MemoryCacheAbsoluteExpiry(this IConfiguration configuration)
        {
            return DateTimeOffset.UtcNow.AddSeconds(int.Parse(configuration["MemoryCacheExpirySeconds:Absolute"]));
        }
    }
}
