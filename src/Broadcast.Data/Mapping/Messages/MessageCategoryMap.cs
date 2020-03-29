using Broadcast.Core.Domain.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Broadcast.Data.Mapping.Messages
{
    public class MessageCategoryMap: EntityTypeConfiguration<MessageCategory>
    {
        public override void Configure(EntityTypeBuilder<MessageCategory> builder)
        {
            builder.ToTable(nameof(MessageCategory));
            builder.HasKey(cat => new { cat.MessageId, cat.CategoryId });

            builder.Property(cat => cat.MessageId).HasColumnName("Message_Id");
            builder.Property(cat => cat.CategoryId).HasColumnName("Category_Id");

            builder.HasOne(cat => cat.Message)
                .WithMany(msg => msg.MessageCategories)
                .HasForeignKey(cat => cat.MessageId);

            builder.HasOne(cat => cat.Category)
                .WithMany()
                .HasForeignKey(cat => cat.CategoryId);

            builder.Ignore(cat => cat.Id);
        }
    }
}