using Broadcast.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Broadcast.Data.Mapping.Users
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(nameof(User));
            builder.HasKey(user => user.Id);

            builder.Property(user => user.AccountName).IsRequired();
            builder.Property(user => user.GivenName).IsRequired();
            builder.Property(user => user.Company).HasMaxLength(100);
        }
    }
}