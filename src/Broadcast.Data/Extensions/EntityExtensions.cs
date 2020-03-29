using System;
using Broadcast.Core.Domain;

namespace Broadcast.Data.Extensions
{
    public static class EntityExtensions
    {
        private static bool IsProxy(this BaseEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var type = entity.GetType();
            return type.BaseType != null && type.BaseType.BaseType != null && type.Name.Contains("Proxy") &&
                   type.BaseType.BaseType == typeof(BaseEntity);
        }

        public static Type GetUnproxiedEntityType(this BaseEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            //EF proxy
            var type = entity.IsProxy() ? entity.GetType().BaseType : entity.GetType();

            if (type == null)
                throw new Exception("Original entity type cannot be loaded");

            return type;
        }
    }
}