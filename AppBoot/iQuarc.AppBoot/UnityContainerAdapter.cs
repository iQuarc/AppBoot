using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace iQuarc.AppBoot
{
	internal sealed class UnityContainerAdapter : IDependencyContainer, IDisposable
	{
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
		{
			container = new UnityContainer();
			serviceLocator = new UnityHierarchicalServiceLocator(container);
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

		public void Dispose()
		{
			container.Dispose();
		}
	}
}