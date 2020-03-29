using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Broadcast.Core;
using Broadcast.Core.Domain;
using Broadcast.Core.Domain.Common;
using Broadcast.Core.Infrastructure;
using Broadcast.Data.Extensions;
using Broadcast.Services.Caching.CachingDefaults;
using Broadcast.Services.Caching.Extensions;

namespace Broadcast.Services.Common
{
    public class GenericAttributeService : IGenericAttributeService
    {
        private readonly IUnitOfWork _worker;

        public GenericAttributeService(IUnitOfWork worker)
        {
            _worker = worker;
        }

        public async Task DeleteAttributeAsync(GenericAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            var repo = _worker.GetRepositoryAsync<GenericAttribute>();
            await repo.DeleteAsync(attribute);

            await _worker.SaveChangesAsync();
        }

        public async Task DeleteAttributesAsync(IList<GenericAttribute> attributes)
        {
            if (attributes == null)
                throw new ArgumentNullException(nameof(attributes));

            var repo = _worker.GetRepositoryAsync<GenericAttribute>();
            await repo.DeleteAsync(attributes);

            await _worker.SaveChangesAsync();
        }

        public async Task InsertAttributeAsync(GenericAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            attribute.CreatedOrUpdatedDateUtc = DateTime.UtcNow;

            var repo = _worker.GetRepositoryAsync<GenericAttribute>();
            await repo.AddAsync(attribute);

            await _worker.SaveChangesAsync();
        }

        public async Task UpdateAttributeAsync(GenericAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            attribute.CreatedOrUpdatedDateUtc = DateTime.UtcNow;

            var repo = _worker.GetRepositoryAsync<GenericAttribute>();
            repo.Update(attribute);

            await _worker.SaveChangesAsync();
        }

        public async Task<IList<GenericAttribute>> GetAttributesForEntityAsync(int entityId, string keyGroup)
        {
            var key = string.Format(CommonCachingDefaults.GenericAttributeCacheKey, entityId, keyGroup);

            var repo = _worker.GetRepositoryAsync<GenericAttribute>();
            var query = await repo.GetQueryableAsync(ga => ga.EntityId == entityId && ga.KeyGroup == keyGroup);

            return await query.ToCachedListAsync(key);
        }

        public async Task SaveAttributeAsync<TPropType>(BaseEntity entity, string key, TPropType value)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var keyGroup = entity.GetUnproxiedEntityType().Name;
          
            var repo = _worker.GetRepositoryAsync<GenericAttribute>();
            var prop = await repo.SingleAsync(ga =>
                ga.EntityId == entity.Id && ga.KeyGroup == keyGroup &&
                ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));

            var valueStr = CommonHelper.To<string>(value);

            if (prop != null)
            {
                if (string.IsNullOrWhiteSpace(valueStr)) await DeleteAttributeAsync(prop);
                else
                {
                    prop.Value = valueStr;
                    await UpdateAttributeAsync(prop);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(valueStr))
                    return;

                prop = new GenericAttribute
                {
                    EntityId = entity.Id,
                    Key = key,
                    KeyGroup = keyGroup,
                    Value = valueStr,
                };

                await InsertAttributeAsync(prop);
            }
        }

        public async Task<TPropType> GetAttributeAsync<TPropType>(BaseEntity entity, string key,
            TPropType defaultValue = default(TPropType))
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var keyGroup = entity.GetUnproxiedEntityType().Name;

            var prop = (await GetAttributesForEntityAsync(entity.Id, keyGroup)).FirstOrDefault(ga =>
                ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));

            return string.IsNullOrEmpty(prop?.Value) ? defaultValue : CommonHelper.To<TPropType>(prop.Value);
        }

        public async Task<TPropType> GetAttributeAsync<TEntity, TPropType>(int entityId, string key,
            TPropType defaultValue = default(TPropType)) where TEntity : BaseEntity
        {
            var entity = (TEntity) Activator.CreateInstance(typeof(TEntity));
            entity.Id = entityId;

            return await GetAttributeAsync(entity, key, defaultValue);
        }
    }
}