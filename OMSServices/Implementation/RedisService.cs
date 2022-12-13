using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OMSServices.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace OMSServices.Implementation
{
    class RedisService : IRedisService, IRedisWriteService
    {
        private readonly IServer redisServer;
        private readonly int _databaseIndex;

        public RedisService(IConfiguration Configuration, ILogger<RedisService> logger)
        {
            string redisConnectionString = Configuration["Redis:ConnectionString"];
            try
            {
                var redisConfiguration = ConfigurationOptions.Parse(redisConnectionString);
                if (redisConfiguration.AllowAdmin)
                {
                    ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConfiguration);

                    _databaseIndex = redis.GetDatabase().Database;

                    redisServer = redis.GetServer(redis.GetEndPoints(true)[0]);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public IAsyncEnumerable<RedisKey> GetAllKeysWithPrefix(string prefix)
        {
            return redisServer.KeysAsync(pattern: $"{prefix}/*");
        }

        public void RemoveAllCacheEntries()
        {
            // Remove all the cache entries(redis only).
            redisServer.FlushDatabase(_databaseIndex);
        }
    }
}
