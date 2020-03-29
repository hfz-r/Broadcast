using Broadcast.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Broadcast.Data.Mapping
{
    public abstract class EntityTypeConfiguration<TEntity> : IMappingConfiguration, IEntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity
    {
        public abstract void Configure(EntityTypeBuilder<TEntity> builder);

        public void ApplyConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(this);
        }
    }
}