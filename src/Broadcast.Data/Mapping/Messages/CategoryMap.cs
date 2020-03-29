using Broadcast.Core.Domain.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Broadcast.Data.Mapping.Messages
{
    public class CategoryMap : EntityTypeConfiguration<Category>
    {
        public override void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable(nameof(Category));
            builder.HasKey(cat => cat.Id);

            builder.Property(cat => cat.Type).HasMaxLength(255);
        }
    }
}