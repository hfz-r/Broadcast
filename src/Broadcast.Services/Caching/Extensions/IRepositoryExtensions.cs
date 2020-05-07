using System;
using System.Threading.Tasks;
using Broadcast.Core.Caching;
using Broadcast.Core.Domain;
using Broadcast.Core.Infrastructure;

namespace Broadcast.Services.Caching.Extensions
{
    public static class IRepositoryExtensions
    {
        public static async Task<TEntity> ToCachedGetByIdAsync<TEntity>(this IRepositoryAsync<TEntity> repositoryAsync, object id, string cacheKey = null) where TEntity : BaseEntity
        {
            var cacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();
            var key = cacheKey ?? string.Format(Core.Caching.CachingDefaults.EntityCacheKey, typeof(TEntity).Name, id);

            return await cacheManager.GetAsync(key, async () => await repositoryAsync.SingleAsync(entity => entity.Id == Convert.ToInt32(id)));
        }
    }
}