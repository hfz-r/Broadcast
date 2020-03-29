using System;
using System.Threading.Tasks;
using Broadcast.Core.Caching;

namespace Broadcast.Test
{
    /// <summary>
    /// Represents a null mock cache manager
    /// </summary>
    public class TestCacheManager : IStaticCacheManager
    {
        private bool _disposed = false;

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
                //nothing special
            }

            _disposed = true;
        }

        public Task ClearAsync()
        {
            return Task.CompletedTask;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, int? cacheTime = null)
        {
            return await acquire();
        }

        public async Task<bool> IsSetAsync(string key)
        {
            return await Task.FromResult(false);
        }

        public Task RemoveAsync(string key)
        {
            return Task.CompletedTask;
        }

        public Task RemoveByPrefixAsync(string prefix)
        {
            return Task.CompletedTask;
        }

        public Task SetAsync(string key, object data, int cacheTime)
        {
            return Task.CompletedTask;
        }
    }
}