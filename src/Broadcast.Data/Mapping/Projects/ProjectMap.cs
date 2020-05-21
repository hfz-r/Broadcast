using Broadcast.Core.Domain.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Broadcast.Data.Mapping.Projects
{
    public class ProjectMap : EntityTypeConfiguration<Project>
    {
        public override void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable(nameof(Project));
            builder.HasKey(proj => proj.Id);

            builder.Property(proj => proj.Name).HasMaxLength(400).IsRequired();
            builder.Property(proj => proj.Description).HasMaxLength(1000);
        }
    }
}