using System;
using Broadcast.Core.Infrastructure;
using Broadcast.Test;
using Moq;
using NUnit.Framework;

namespace Broadcast.Services.Test
{
    [TestFixture]
    public abstract class ServiceTest
    {
        [SetUp]
        public virtual void SetUp()
        {
        }

        public void RunWithTestServiceProvider(Action action)
        {
            var nopEngine = new Mock<Engine>();
            nopEngine.Setup(x => x.ServiceProvider).Returns(new TestServiceProvider());
            EngineContext.Replace(nopEngine.Object);

            action();

            EngineContext.Replace(null);
        }
    }
}