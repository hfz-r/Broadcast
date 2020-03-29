using Autofac;
using Broadcast.Core.Caching;
using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.Security;
using Broadcast.Core.Infrastructure.TypeFinder;
using Broadcast.Data;
using Broadcast.Infrastructure.Behaviors;
using Broadcast.Services.Common;
using Broadcast.Services.Logging;
using MediatR;

namespace Broadcast.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //mediatr behaviors
            builder.RegisterGeneric(typeof(ValidationBehavior<,>)).As(typeof(IPipelineBehavior<,>))
                .InstancePerDependency();
            builder.RegisterGeneric(typeof(DbContextTransactionBehavior<,>)).As(typeof(IPipelineBehavior<,>))
                .InstancePerLifetimeScope();

            //unit-of-work
            builder.RegisterGeneric(typeof(UnitOfWork<>)).As(typeof(IUnitOfWork<>)).InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWork<ApplicationDbContext>>().As<IUnitOfWork>().InstancePerLifetimeScope();
          
            //cache manager for short term caching
            builder.RegisterType<PerRequestCacheManager>().As<ICacheManager>().InstancePerLifetimeScope();
            //cache manager for long term caching
            builder.RegisterType<MemoryCacheManager>().As<ILocker>().As<IStaticCacheManager>().SingleInstance();

            //general helpers
            builder.RegisterType<JwtTokenGenerator>().As<IJwtTokenGenerator>().InstancePerLifetimeScope();
            builder.RegisterType<CurrentUserAccessor>().As<ICurrentUserAccessor>().InstancePerLifetimeScope();

            //domain services
            builder.RegisterType<DefaultLogger>().As<ILogger>().InstancePerLifetimeScope();
            builder.RegisterType<GenericAttributeService>().As<IGenericAttributeService>().InstancePerLifetimeScope();
        }

        public int Order => 0;
    }
}