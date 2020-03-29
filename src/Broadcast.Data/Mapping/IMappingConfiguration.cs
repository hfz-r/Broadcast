using Microsoft.EntityFrameworkCore;

namespace Broadcast.Data.Mapping
{
    public interface IMappingConfiguration
    {
        void ApplyConfiguration(ModelBuilder modelBuilder);
    }
}