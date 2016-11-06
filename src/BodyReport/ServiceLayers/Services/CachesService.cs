using BodyReport.Framework;
using BodyReport.ServiceLayers.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ServiceLayers.Services
{
    public class CachesService : ICachesService
    {
        IMemoryCache _memoryCache;

        public CachesService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T GetData<T>(string cacheKey)
        {
            return _memoryCache.Get<T>(cacheKey);
        }

        public bool TryGetData<T>(string cacheKey, out T data)
        {
            return _memoryCache.TryGetValue<T>(cacheKey, out data);
        }

        public void SetData<T>(string cacheName, string cacheKey, T data, MemoryCacheEntryOptions options)
        {
            if (!string.IsNullOrWhiteSpace(cacheName) && options != null)
            {
                var cts = CacheTokenManager.Instance.GetCacheToken(cacheName);
                options.AddExpirationToken(new CancellationChangeToken(cts.Token));
            }
            _memoryCache.Set<T>(cacheKey, data, options);
        }

        public void RemoveCache(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);
        }

        public void InvalidateCache(string cacheName)
        {
            CacheTokenManager.Instance.InvalidateCacheToken(cacheName);
        }
    }
}
