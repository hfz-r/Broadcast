using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Broadcast.Core.Caching;
using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.Paging;
using Microsoft.EntityFrameworkCore;

namespace Broadcast.Services.Caching.Extensions
{
    public static class IQueryableExtensions
    {
        private static IStaticCacheManager CacheManager => EngineContext.Current.Resolve<IStaticCacheManager>();

        public static async Task<IList<T>> ToCachedListAsync<T>(this IQueryable<T> query, string cacheKey)
        {
            return string.IsNullOrEmpty(cacheKey)
                ? await query.ToListAsync()
                : await CacheManager.GetAsync(cacheKey, async () => await query.ToListAsync());
        }

        public static async Task<T[]> ToCachedArrayAsync<T>(this IQueryable<T> query, string cacheKey)
        {
            return string.IsNullOrEmpty(cacheKey)
                ? await query.ToArrayAsync()
                : await CacheManager.GetAsync(cacheKey, async () => await query.ToArrayAsync());
        }

        public static async Task<T> ToCachedFirstOrDefaultAsync<T>(this IQueryable<T> query, string cacheKey)
        {
            return string.IsNullOrEmpty(cacheKey)
                ? await query.FirstOrDefaultAsync()
                : await CacheManager.GetAsync(cacheKey, async () => await query.FirstOrDefaultAsync());
        }

        public static async Task<T> ToCachedSingleAsync<T>(this IQueryable<T> query, string cacheKey)
        {
            return string.IsNullOrEmpty(cacheKey)
                ? await query.SingleAsync()
                : await CacheManager.GetAsync(cacheKey, async () => await query.SingleAsync());
        }

        public static async Task<IPaginate<T>> ToCachedPagedListAsync<T>(this IQueryable<T> query, string cacheKey,
            int pageIndex, int pageSize)
        {
            return string.IsNullOrEmpty(cacheKey)
                ? await query.ToPaginateAsync(pageIndex, pageSize)
                : await CacheManager.GetAsync(cacheKey, async () => await query.ToPaginateAsync(pageIndex, pageSize));
        }

        public static async Task<bool> ToCachedAnyAsync<T>(this IQueryable<T> query, string cacheKey)
        {
            return string.IsNullOrEmpty(cacheKey)
                ? await query.AnyAsync()
                : await CacheManager.GetAsync(cacheKey, async () => await query.AnyAsync());
        }

        public static async Task<int> ToCachedCountAsync<T>(this IQueryable<T> query, string cacheKey)
        {
            return string.IsNullOrEmpty(cacheKey)
                ? await query.CountAsync()
                : await CacheManager.GetAsync(cacheKey, async () => await query.CountAsync());
        }
    }
}