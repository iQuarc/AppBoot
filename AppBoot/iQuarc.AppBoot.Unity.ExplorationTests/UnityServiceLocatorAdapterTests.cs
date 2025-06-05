using System;
using CommonServiceLocator;
using iQuarc.xUnitEx;
using Unity;
using Unity.Lifetime;
using Xunit;

namespace iQuarc.AppBoot.Unity.ExplorationTests
{
    public class UnityServiceLocatorAdapterTests
    {
        [Fact]
        public void DisposingTheContainerDoesNotCrashWhenTheServiceLocatorInstanceIsRegisteredByTheAdapterOnly()
        {
            UnityContainer container = GetNewContainer();
            IServiceLocator sl = new UnityServiceLocator(container);

            container.RegisterType<IService, Service>();
            AssertEx.ShouldNotThrow(() => sl.GetInstance<IService>());

            Action act = container.Dispose;

            act.ShouldNotThrow(); // this will not trigger sl.Dispose() because the sl instance is not controlled by the container
        }

        [Fact]
        public void DisposingTheContainerDoesNotCrashWhenTheServiceLocatorInstanceIsRegisteredWithContainerControlledLifetime()
        {
            UnityContainer container = GetNewContainer();
            IServiceLocator sl = new UnityServiceLocator(container);
            container.RegisterInstance(sl, new ContainerControlledLifetimeManager());

            container.RegisterType<IService, Service>();
            AssertEx.ShouldNotThrow(() => sl.GetInstance<IService>());

            Action act = container.Dispose;

            act.ShouldNotThrow(); // this will trigger sl.Dispose(), which triggers container.Dispose(). In current version this no longer goes into StackOverflowException, but it did in the past.
        }

        [Fact]
        public void AfterDisposingTheContainerAndTheServiceLocatorResultsInObjectDisposedExceptionWhenResolving()
        {
            UnityContainer container = GetNewContainer();
            IServiceLocator sl = new UnityServiceLocator(container);

            container.RegisterType<IService, Service>();
            AssertEx.ShouldNotThrow(() => sl.GetInstance<IService>());

            container.Dispose();
            ((IDisposable) sl).Dispose();

            AssertEx.ShouldThrow(() => sl.GetInstance<IService>());
        }

        [Fact]
        public void AfterDisposingTheContainerTheServiceLocatorCanNotResolve()
        {
            UnityContainer container = GetNewContainer();
            IServiceLocator sl = new UnityServiceLocator(container);

            container.RegisterType<IService, Service>();
            AssertEx.ShouldNotThrow(() => sl.GetInstance<IService>());

            container.Dispose();

            AssertEx.ShouldThrow(() => sl.GetInstance<IService>());
        }

        private static UnityContainerTestWrapper GetNewContainer()
        {
            return new UnityContainerTestWrapper {MaxDisposeRecursiveCalls = 10};
        }


        private class UnityContainerTestWrapper : UnityContainer
        {
            private int calls;

            protected override void Dispose(bool disposing)
            {
                if (Environment.StackTrace.Contains("UnityContainer.Dispose()"))
                    calls++;
                if (calls > MaxDisposeRecursiveCalls)
                    throw new StackOverflowException();

                base.Dispose(disposing);
            }

            public int MaxDisposeRecursiveCalls { get; set; }
        }


        private interface IService
        {
        }

        private class Service : IService
        {
        }
    }
}