using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace Sample_Redis.Services
{
    public class CacheService : ICacheService
    {
        //Used ConcurrentDictionary becouse it is thread safe and efficient
        private static readonly ConcurrentDictionary<string, bool> CacheKeys = new();
        private readonly IDistributedCache distributedCache;

        public CacheService(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            string? cachedValue = await distributedCache
                .GetStringAsync(key, cancellationToken);

            if (cachedValue is null)
            {
                return default(T);
            }

            T? value = JsonConvert.DeserializeObject<T>(cachedValue);

            CacheKeys.TryAdd(key, true);

            return value;
        }

        //Factory pattern for caching, realy comfortable thing in my opinion:
        public async Task<T?> GetAsync<T>(string key, Func<Task<T>> factory, CancellationToken cancellationToken = default) where T : class
        {
            T? cashedValue = await GetAsync<T>(key, cancellationToken);

            if (cashedValue is not null)
            {
                return cashedValue;
            }

            cashedValue = await factory();

            await SetAsync(key, cashedValue, cancellationToken);

            return cashedValue;

        }

        public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
        {
            string cacheValue = JsonConvert.SerializeObject(value);

            await distributedCache.SetStringAsync(key, cacheValue, cancellationToken);

            CacheKeys.TryAdd(key, true);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await distributedCache.RemoveAsync(key, cancellationToken);

            CacheKeys.TryRemove(key, out bool _);
        }

        public async Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
        {
            foreach (string key in CacheKeys.Keys)
            {
                if (key.StartsWith(prefixKey))
                {
                    await RemoveAsync(key, cancellationToken);
                }
            }
        }   
    }
}
