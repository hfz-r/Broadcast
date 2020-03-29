using Autofac;
using Broadcast.Core.Infrastructure.TypeFinder;

namespace Broadcast.Core.Infrastructure
{
    public interface IDependencyRegistrar
    {
        void Register(ContainerBuilder builder, ITypeFinder typeFinder);

        int Order { get; }
    }
}