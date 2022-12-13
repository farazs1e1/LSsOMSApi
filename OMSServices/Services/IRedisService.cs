using StackExchange.Redis;
using System.Collections.Generic;

namespace OMSServices.Services
{
    public interface IRedisService
    {
        IAsyncEnumerable<RedisKey> GetAllKeysWithPrefix(string prefix);
    }
    public interface IRedisWriteService
    {
        /// <summary>
        /// remove all the cache entries (redis only).
        /// </summary>
        void RemoveAllCacheEntries();
    }
}
