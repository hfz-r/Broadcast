using System;
using Broadcast.Core.Domain.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Broadcast.Data.Mapping.Messages
{
    public class MessageMap : EntityTypeConfiguration<Message>
    {
        public override void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable(nameof(Message));
            builder.HasKey(msg => msg.Id);

            builder.Property(msg => msg.Project).HasMaxLength(400).IsRequired();
            builder.Property(msg => msg.Title).HasMaxLength(1000).IsRequired();
            builder.Property(msg => msg.Description).HasMaxLength(int.MaxValue).IsRequired();
        }
    }
}