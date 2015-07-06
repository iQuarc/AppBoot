using System;
using Microsoft.Practices.Unity;
using Xunit;

namespace iQuarc.AppBoot.Unity.Tests
{
	/// <summar>
	///     These are integration tests which verify how the DisposablesContainerExtension works
	///     with hierarchical unity containers (Parent - Child containers) to dispose built IDisposable objects. This is also
	///     known as Scoped Containers
	///     These tests are also meant to show the use cases when child containers are associated with a scoped operation
	///     (web session, web request, etc.) and all IDisposables built within that operation are disposed when the operation
	///     ends.
	/// </summar>
	public class DisposablesContainerExtensionIntergrationTests
	{
		[Fact]
		public void WhenContainerIsDisposed_BuiltInstanceIsDisposed()
		{
			IService service;
			IUnityContainer container = NewContainer();
			container.RegisterType<IService, DisposableService>(new PerResolveLifetimeManager());

			service = container.Resolve<IService>();
			container.Dispose();

			AssertIsDisposed(service);
		}

		[Fact]
		public void WhenInstanceIsResolvedInChildContainerAndChildContainerIsDisposed_BuiltInstanceIsDisposed()
		{
			IUnityContainer parentContainer = NewContainer();
			parentContainer.RegisterType<IService, DisposableService>(new PerResolveLifetimeManager());

			IService service;
			using (IUnityContainer childContainer = CreateChildContainer(parentContainer))
			{
				service = childContainer.Resolve<IService>();
			}

			AssertIsDisposed(service);
		}

		[Fact]
		public void WhenInstancesAreResolvedInBothContainersAndChildContainerIsDisposed_OnlyInstanceResolvedInChildIsDisposed()
		{
			IUnityContainer parentContainer = NewContainer();
			parentContainer.RegisterType<IService, DisposableService>(new PerResolveLifetimeManager());

			IService outerScopeService = parentContainer.Resolve<IService>();
			IService innerScopeService;

			using (IUnityContainer childContainer = CreateChildContainer(parentContainer))
			{
				innerScopeService = childContainer.Resolve<IService>();
			}

			AssertNotDisposed(outerScopeService);
			AssertIsDisposed(innerScopeService);
		}

		[Fact]
		public void WhenSingletonIsResolveByChildContainerAndChildContainerIsDisposed_SingletonInstancesAreNotDisposed()
		{
			IUnityContainer parentContainer = NewContainer();
			parentContainer.RegisterType<IService, DisposableService>(new ContainerControlledLifetimeManager());
			IService singleton;
			using (IUnityContainer childContainer = CreateChildContainer(parentContainer))
			{
				singleton = childContainer.Resolve<IService>();
			}

			AssertNotDisposed(singleton);
		}

		[Fact]
		public void WhenContainerSingletonsResolvedInBothContainers_InstancesAreDisposedWithEachContainerOnly()
		{
			IUnityContainer parentContainer = NewContainer();
			parentContainer.RegisterType<IService, DisposableService>(new HierarchicalLifetimeManager());

			IService parentContainerSingleton = parentContainer.Resolve<IService>();

			IService childContainerSingleton;
			using (IUnityContainer childContainer = CreateChildContainer(parentContainer))
			{
				childContainerSingleton = childContainer.Resolve<IService>();
			}


			AssertIsDisposed(childContainerSingleton);
			AssertNotDisposed(parentContainerSingleton);

			parentContainer.Dispose();
			AssertIsDisposed(parentContainerSingleton);
		}

		[Fact]
		public void WhenParentContainerIsDisposed_ChildCreatedInstancesAreNotDisposedAgain()
		{
			IUnityContainer parentContainer = NewContainer();
			parentContainer.RegisterType<IService, DisposableService>(new PerResolveLifetimeManager());

			IService service;
			using (IUnityContainer childContainer = CreateChildContainer(parentContainer))
			{
				service = childContainer.Resolve<IService>();
			}
			Assert.Equal(1, service.DisposeCount);

			parentContainer.Dispose();
			Assert.Equal(1, service.DisposeCount);
		}

		private IUnityContainer NewContainer()
		{
			UnityContainer container = new UnityContainer();
			container.AddExtension(new DisposablesContainerExtension());
			return container;
		}

		private IUnityContainer CreateChildContainer(IUnityContainer container)
		{
			IUnityContainer childContainer = container.CreateChildContainer();
			childContainer.AddExtension(new DisposablesContainerExtension());
			return childContainer;
		}

		private static void AssertIsDisposed(IService service)
		{
			Assert.True(service.IsDisposed, "Service is disposed, but NOT expected");
		}

		private static void AssertNotDisposed(IService service)
		{
			Assert.False(service.IsDisposed, "Service is NOT disposed, but expected");
		}

		private interface IService
		{
			bool IsDisposed { get; }
			int DisposeCount { get; }
		}

		private class DisposableService : IService, IDisposable
		{
			private int disposeCount;

			public bool IsDisposed
			{
				get { return disposeCount > 0; }
			}

			public int DisposeCount
			{
				get { return disposeCount; }
			}


			public void Dispose()
			{
				disposeCount++;
			}
		}
	}
}