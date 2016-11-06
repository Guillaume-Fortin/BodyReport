using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ServiceLayers.Interfaces
{
    public interface ICachesService
    {
        T GetData<T>(string cacheKey);

        bool TryGetData<T>(string cacheKey, out T data);

        void SetData<T>(string cacheName, string cacheKey, T data, MemoryCacheEntryOptions options);

        void RemoveCache(string cacheKey);

        void InvalidateCache(string cacheName);
    }
}
