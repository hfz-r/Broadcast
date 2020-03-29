using System;
using Broadcast.Core.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace Broadcast.Core.Test.Infrastructure
{
    [TestFixture]
    public class SingletonTests
    {
        [Test]
        public void Singleton_IsNullByDefault()
        {
            var instance = Singleton<SingletonTests>.Instance;
            instance.Should().BeNull();
        }

        [Test]
        public void Singletons_ShareSame_SingletonsDictionary()
        {
            Singleton<int>.Instance = 1;
            Singleton<double>.Instance = 2.0;

            BaseSingleton.AllSingletons.Should().BeSameAs(BaseSingleton.AllSingletons);
            BaseSingleton.AllSingletons[typeof(int)].Should().Be(1);
            BaseSingleton.AllSingletons[typeof(double)].Should().Be(2.0M);
        }

        [Test]
        public void SingletonDictionary_IsCreatedByDefault()
        {
            var instance = SingletonDictionary<SingletonTests, object>.Instance;
            instance.Should().NotBeNull();
        }

        [Test]
        public void SingletonDictionary_CanStoreStuff()
        {
            var instance = SingletonDictionary<Type, SingletonTests>.Instance;
            instance[typeof(SingletonTests)] = this;
            instance[typeof(SingletonTests)].Should().BeSameAs(this);
        }
    }
}