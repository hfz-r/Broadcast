using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Core.ComponentModel;
using Microsoft.AspNetCore.Http;

namespace Broadcast.Core.Caching
{
    /// <summary>
    /// Represents a manager for caching DURING HTTP request (short term caching)
    /// </summary>
    public class PerRequestCacheManager : ICacheManager
    {
        private bool _disposed = false;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ReaderWriterLockSlim _locker;

        public PerRequestCacheManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            _locker = new ReaderWriterLockSlim();
        }

        #region Utilities

        protected Task<IDictionary<object, object>> GetItemsAsync()
        {
            return Task.FromResult(_httpContextAccessor.HttpContext?.Items);
        }

        #endregion

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, int? cacheTime = null)
        {
            IDictionary<object, object> items;

            if (_disposed)
                return await acquire();

            using (new ReaderWriteLockDisposable(_locker, ReaderWriteLockType.Read))
            {
                items = await GetItemsAsync();
                if (items == null)
                    return await acquire();

                //item already cached, return it
                if (items[key] != null)
                    return (T) items[key];
            }

            var result = await acquire();

            if (result == null || (cacheTime ?? CachingDefaults.CacheTime) <= 0)
                return result;

            //set in cache (if cache time is defined)
            using (new ReaderWriteLockDisposable(_locker))
            {
                items[key] = result;
            }

            return result;
        }

        public async Task SetAsync(string key, object data, int cacheTime)
        {
            if (data == null)
                return;

            using (new ReaderWriteLockDisposable(_locker))
            {
                var items = await GetItemsAsync();
                if (items == null)
                    return;

                items[key] = data;
            }
        }

        public async Task<bool> IsSetAsync(string key)
        {
            using (new ReaderWriteLockDisposable(_locker, ReaderWriteLockType.Read))
            {
                var items = await GetItemsAsync();
                return items?[key] != null;
            }
        }

        public async Task RemoveAsync(string key)
        {
            using (new ReaderWriteLockDisposable(_locker))
            {
                var items = await GetItemsAsync();
                items?.Remove(key);
            }
        }

        public async Task RemoveByPrefixAsync(string prefix)
        {
            using (new ReaderWriteLockDisposable(_locker, ReaderWriteLockType.UpgradeableRead))
            {
                var items = await GetItemsAsync();
                if (items == null)
                    return;

                //get cache keys that matches pattern
                var regex = new Regex(prefix,
                    RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var matchesKeys = items.Keys.Select(p => p.ToString()).Where(key => regex.IsMatch(key)).ToList();

                if (!matchesKeys.Any())
                    return;

                using (new ReaderWriteLockDisposable(_locker))
                {
                    //remove matching values
                    foreach (var key in matchesKeys)
                    {
                        items.Remove(key);
                    }
                }
            }
        }

        public async Task ClearAsync()
        {
            using (new ReaderWriteLockDisposable(_locker))
            {
                var items = await GetItemsAsync();
                items?.Clear();
            }
        }

        #region IDisposable members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _locker?.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}