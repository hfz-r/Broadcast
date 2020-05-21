using Autofac;
using Broadcast.Core;
using Broadcast.Core.Caching;
using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.Security;
using Broadcast.Core.Infrastructure.TypeFinder;
using Broadcast.Data;
using Broadcast.Infrastructure.Auth.Policies;
using Broadcast.Infrastructure.Behaviors;
using Broadcast.Services.Auth;
using Broadcast.Services.Common;
using Broadcast.Services.Logging;
using Broadcast.Services.Messages;
using Broadcast.Services.Security;
using Broadcast.Services.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;

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

            //authorization
            builder.RegisterType<AuthorizationSchemeHandler>().As<IAuthorizationHandler>().SingleInstance();
            builder.RegisterType<SignedInHandler>().As<IAuthorizationHandler>().SingleInstance();

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
            
            //log service
            builder.RegisterType<DefaultLogger>().As<ILogger>().InstancePerLifetimeScope();
            //domain services
            builder.RegisterType<AuthenticationService>().As<IAuthenticationService>().InstancePerLifetimeScope();
            builder.RegisterType<GenericAttributeService>().As<IGenericAttributeService>().InstancePerLifetimeScope();
            builder.RegisterType<MessageService>().As<IMessageService>().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<PermissionService>().As<IPermissionService>().InstancePerLifetimeScope();
        }

        public int Order => 0;
    }
}