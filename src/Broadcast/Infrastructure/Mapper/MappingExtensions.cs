using System;
using Broadcast.Domain;

namespace Broadcast.Infrastructure.Mapper
{
    public static class MappingExtensions
    {
        #region Utilities

        private static TDestination Map<TDestination>(this object source)
        {
            return AutoMapperConfiguration.Mapper.Map<TDestination>(source);
        }

        private static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return AutoMapperConfiguration.Mapper.Map(source, destination);
        }

        #endregion

        public static TDto ToDto<TDto>(this BaseEntity entity) where TDto : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return entity.Map<TDto>();
        }

        public static TDto ToDto<TEntity, TDto>(this TEntity entity, TDto model)
            where TEntity : BaseEntity
            where TDto : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return entity.MapTo(model);
        }

        public static TEntity ToEntity<TEntity, TDto>(this TDto dto, TEntity entity)
            where TEntity : BaseEntity 
            where TDto : class 
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return dto.MapTo(entity);
        }
    }
}