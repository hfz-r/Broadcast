using Autofac;
using Broadcast.Core.Caching;
using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.TypeFinder;
using Broadcast.Test;

namespace Broadcast.Services.Test
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<TestCacheManager>().As<ICacheManager>().Named<ICacheManager>("brdcst_cache_static")
                .SingleInstance();
        }

        public int Order => 0;
    }
}