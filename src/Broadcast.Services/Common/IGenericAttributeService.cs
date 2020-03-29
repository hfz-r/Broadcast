using System.Collections.Generic;
using System.Threading.Tasks;
using Broadcast.Core.Domain;
using Broadcast.Core.Domain.Common;

namespace Broadcast.Services.Common
{
    public interface IGenericAttributeService
    {
        Task DeleteAttributeAsync(GenericAttribute attribute);
        Task DeleteAttributesAsync(IList<GenericAttribute> attributes);
        Task<TPropType> GetAttributeAsync<TEntity, TPropType>(int entityId, string key, TPropType defaultValue = default(TPropType)) where TEntity : BaseEntity;
        Task<TPropType> GetAttributeAsync<TPropType>(BaseEntity entity, string key, TPropType defaultValue = default(TPropType));
        Task<IList<GenericAttribute>> GetAttributesForEntityAsync(int entityId, string keyGroup);
        Task InsertAttributeAsync(GenericAttribute attribute);
        Task SaveAttributeAsync<TPropType>(BaseEntity entity, string key, TPropType value);
        Task UpdateAttributeAsync(GenericAttribute attribute);
    }
}