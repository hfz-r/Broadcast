using System;
using Broadcast.Core;
using Broadcast.Core.Caching;
using Broadcast.Core.Infrastructure;
using Broadcast.Infrastructure;
using Broadcast.Services.Auth;
using Broadcast.Services.Common;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Broadcast.Test
{
    public class TestServiceProvider : IServiceProvider
    {
        public TestServiceProvider()
        {
        }

        public virtual object GetService(Type serviceType)
        {
            if (serviceType == typeof(IHttpContextAccessor))
                return new Mock<IHttpContextAccessor>().Object;

            if (serviceType == typeof(ICurrentUserAccessor))
                return new CurrentUserAccessor(new Mock<IUnitOfWork>().Object, new Mock<IHttpContextAccessor>().Object);

            if (serviceType == typeof(IGenericAttributeService))
                return new GenericAttributeService(new Mock<IUnitOfWork>().Object);

            if (serviceType == typeof(IStaticCacheManager))
                return new TestCacheManager();

            return null;
        }
    }
}