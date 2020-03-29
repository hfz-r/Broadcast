using System;
using System.Threading.Tasks;
using EasyCaching.Core;

namespace Broadcast.Core.Caching
{
    public class MemoryCacheManager : ILocker, IStaticCacheManager
    {
        private bool _disposed = false;
        private readonly IEasyCachingProvider _provider;

        public MemoryCacheManager(IEasyCachingProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Get a cached item or load and cache if unavailable
        /// </summary>
        public async Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, int? cacheTime = null)
        {
            if (cacheTime <= 0)
                return await acquire();

            var t = await _provider.GetAsync(key, acquire,
                TimeSpan.FromMinutes(cacheTime ?? CachingDefaults.CacheTime));
            return t.Value;
        }

        /// <summary>
        /// Adds cache with specified key and object
        /// </summary>
        public async Task SetAsync(string key, object data, int cacheTime)
        {
            if (cacheTime <= 0)
                return;

            await _provider.SetAsync(key, data, TimeSpan.FromMinutes(cacheTime));
        }

        /// <summary>
        /// Gets availability of the cached value with specified key
        /// </summary>
        public async Task<bool> IsSetAsync(string key)
        {
            return await _provider.ExistsAsync(key);
        }

        /// <summary>
        /// Perform action with exclusive in-memory lock
        /// </summary>
        public async Task<bool> PerformActionWithLockAsync(string key, TimeSpan expirationTime, Action action)
        {
            if (await _provider.ExistsAsync(key))
                return false;

            try
            {
                await _provider.SetAsync(key, key, expirationTime);

                action();

                return true;
            }
            finally
            {
                await RemoveAsync(key);
            }
        }

        /// <summary>
        /// Removes cache with specified key 
        /// </summary>
        public async Task RemoveAsync(string key)
        {
            await _provider.RemoveAsync(key);
        }

        /// <summary>
        /// Removes cache by key prefix
        /// </summary>
        public async Task RemoveByPrefixAsync(string prefix)
        {
            await _provider.RemoveByPrefixAsync(prefix);
        }

        /// <summary>
        /// Clear all cache 
        /// </summary>
        public async Task ClearAsync()
        {
            await _provider.FlushAsync();
        }

        #region IDisposable members

        /// <summary>
        /// Dispose cache
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                //nothing special
            }

            _disposed = true;
        }

        #endregion
    }
}