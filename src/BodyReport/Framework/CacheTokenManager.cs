using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BodyReport.Framework
{
    public class CacheTokenManager
    {
        private static CacheTokenManager _instance = null;
        private static Dictionary<string, CancellationTokenSource> _tokens = new Dictionary<string, CancellationTokenSource>();

        public static CacheTokenManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CacheTokenManager();
                return _instance;
            }
        }

        private CacheTokenManager()
        {
        }

        public CancellationTokenSource GetCacheToken(string cacheName)
        {
            CancellationTokenSource cts = null;
            lock (_tokens)
            {   
                if (_tokens.ContainsKey(cacheName))
                {
                    if (_tokens[cacheName] == null || _tokens[cacheName].IsCancellationRequested)
                        _tokens.Remove(cacheName);
                    else
                        cts = _tokens[cacheName];
                }
                if(cts == null)
                {
                    cts = new CancellationTokenSource();
                    _tokens.Add(cacheName, cts);
                }
            }
            return cts;
        }

        public void InvalidateCacheToken(string cacheName)
        {
            lock (_tokens)
            {
                if (_tokens.ContainsKey(cacheName))
                {
                    if (_tokens[cacheName] != null && !_tokens[cacheName].IsCancellationRequested)
                        _tokens[cacheName].Cancel();
                    _tokens.Remove(cacheName);
                }
            }
        }
    }
}
