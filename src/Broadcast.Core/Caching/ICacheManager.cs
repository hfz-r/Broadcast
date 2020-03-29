using System;
using System.Threading.Tasks;

namespace Broadcast.Core.Caching
{
    public interface ICacheManager: IDisposable
    {
        Task ClearAsync();
        Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, int? cacheTime = null);
        Task<bool> IsSetAsync(string key);
        Task RemoveAsync(string key);
        Task RemoveByPrefixAsync(string prefix);
        Task SetAsync(string key, object data, int cacheTime);
    }
}