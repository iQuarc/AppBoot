using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace iQuarc.AppBoot.Unity
{
	internal sealed class UnityContainerAdapter : IDependencyContainer, IDisposable
	{
		private readonly IExtensionsFactory extensionFactory;

		private static readonly Dictionary<Lifetime, Func<ServiceInfo, LifetimeManager>> lifetimeManagers
			= new Dictionary<Lifetime, Func<ServiceInfo, LifetimeManager>>
			{
				{Lifetime.Instance, s => new PerResolveLifetimeManager()},
				{Lifetime.AlwaysNew, s => new TransientLifetimeManager()},
				{Lifetime.Application, s => new ContainerControlledLifetimeManager()},
			};

		private readonly IUnityContainer container;
		private readonly IServiceLocator serviceLocator;

		public UnityContainerAdapter()
			: this(new EmptyExtensionsFactory())
		{
		}

		public UnityContainerAdapter(IExtensionsFactory extensionFactory)
		{
			this.extensionFactory = extensionFactory;

			container = new UnityContainer();
			AddExtenssions(container, extensionFactory.GetContainerExtensions());
			serviceLocator = new UnityServiceLocator(container);
		}

		private UnityContainerAdapter(IUnityContainer child, IExtensionsFactory extensionsFactory)
		{
			this.extensionFactory = extensionsFactory;
			AddExtenssions(child, extensionsFactory.GetChildExtensions());

			this.container = child;
			serviceLocator = new UnityServiceLocator(child);
		}

		private static void AddExtenssions(IUnityContainer unityContainer, IEnumerable<UnityContainerExtension> extensionss)
		{
			foreach (var extension in extensionss)
			{
				unityContainer.AddExtension(extension);
			}
		}

		public IServiceLocator AsServiceLocator
		{
			get { return serviceLocator; }
		}

		public void RegisterService(ServiceInfo service)
		{
			LifetimeManager lifetime = GetLifetime(service);
			container.RegisterType(service.From, service.To, service.ContractName, lifetime, new InjectionMember[] {});
		}

		private static LifetimeManager GetLifetime(ServiceInfo srv)
		{
			Func<ServiceInfo, LifetimeManager> factory = lifetimeManagers[srv.InstanceLifetime];
			return factory(srv);
		}

		public void RegisterInstance<T>(T instance)
		{
			container.RegisterInstance(instance);
		}

		public IDependencyContainer CreateChildContainer()
		{
			IUnityContainer child = container.CreateChildContainer();
			return new UnityContainerAdapter(child, extensionFactory);
		}

		public void Dispose()
		{
			container.Dispose();

			IDisposable serviceLocatorAsDisposable = serviceLocator as IDisposable;
			if (serviceLocatorAsDisposable != null)
				serviceLocatorAsDisposable.Dispose();
		}
	}
}