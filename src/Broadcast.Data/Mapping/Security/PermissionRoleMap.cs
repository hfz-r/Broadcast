using Broadcast.Core.Domain.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Broadcast.Data.Mapping.Security
{
    public class PermissionRoleMap : EntityTypeConfiguration<PermissionRole>
    {
        public override void Configure(EntityTypeBuilder<PermissionRole> builder)
        {
            builder.ToTable(nameof(PermissionRole));
            builder.HasKey(mapping => new { mapping.PermissionId, mapping.RoleId });

            builder.Property(mapping => mapping.PermissionId).HasColumnName("Permission_Id");
            builder.Property(mapping => mapping.RoleId).HasColumnName("Role_Id");

            builder.HasOne(mapping => mapping.Role)
                .WithMany()
                .HasForeignKey(mapping => mapping.RoleId)
                .IsRequired();

            builder.HasOne(mapping => mapping.Permission)
                .WithMany(permission => permission.PermissionRoles)
                .HasForeignKey(mapping => mapping.PermissionId)
                .IsRequired();

            builder.Ignore(mapping => mapping.Id);
        }
    }
}