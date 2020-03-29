using Broadcast.Core.Domain.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Broadcast.Data.Mapping.Messages
{
    public class MessageTagMap : EntityTypeConfiguration<MessageTag>
    {
        public override void Configure(EntityTypeBuilder<MessageTag> builder)
        {
            builder.ToTable(nameof(MessageTag));
            builder.HasKey(mtag => new { mtag.MessageId, mtag.TagId });

            builder.Property(mtag => mtag.MessageId).HasColumnName("Message_Id");
            builder.Property(mtag => mtag.TagId).HasColumnName("Tag_Id");

            builder.HasOne(mtag => mtag.Message)
                .WithMany(msg => msg.MessageTags)
                .HasForeignKey(mtag => mtag.MessageId);

            builder.HasOne(mtag => mtag.Tag)
                .WithMany(tag => tag.MessageTags)
                .HasForeignKey(mtag => mtag.TagId);

            builder.Ignore(mtag => mtag.Id);
        }
    }
}