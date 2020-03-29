using Broadcast.Core.Domain.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Broadcast.Data.Mapping.Messages
{
    public class FileMap : EntityTypeConfiguration<File>
    {
        public override void Configure(EntityTypeBuilder<File> builder)
        {
            builder.ToTable(nameof(File));
            builder.HasKey(file => file.Id);

            builder.HasOne(file => file.Message)
                .WithMany(msg => msg.Files)
                .HasForeignKey(file => file.MessageId)
                .IsRequired();
        }
    }
}