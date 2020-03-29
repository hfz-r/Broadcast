using Broadcast.Core.Domain.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Broadcast.Data.Mapping.Logging
{
    public class LogMap : EntityTypeConfiguration<Log>
    {
        public override void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.ToTable(nameof(Log));
            builder.HasKey(logItem => logItem.Id);

            builder.Property(logItem => logItem.ShortMessage).IsRequired();

            builder.Ignore(logItem => logItem.LogLevel);

            builder.HasOne(logItem => logItem.User)
                .WithMany()
                .HasForeignKey(logItem => logItem.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}