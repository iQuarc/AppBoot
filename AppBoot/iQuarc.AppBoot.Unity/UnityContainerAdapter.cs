using System;
using System.Collections.Generic;
using CommonServiceLocator;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace iQuarc.AppBoot.Unity
{
	internal sealed class UnityContainerAdapter : IDependencyContainer, IDisposable
	{
		private static readonly Dictionary<Lifetime, Func<ServiceInfo, ITypeLifetimeManager>> lifetimeManagers
			= new Dictionary<Lifetime, Func<ServiceInfo, ITypeLifetimeManager>>
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
            serviceLocator = new UnityServiceLocator(container);
		}

	    private UnityContainerAdapter(IUnityContainer child)
	    {
	        this.container = child;
            serviceLocator = new UnityServiceLocator(child);
	    }

		public IServiceLocator AsServiceLocator
		{
			get { return serviceLocator; }
		}

		public void RegisterService(ServiceInfo service)
		{
			ITypeLifetimeManager lifetime = GetLifetime(service);
			container.RegisterType(service.From, service.To, service.ContractName, lifetime, new InjectionMember[] {});
		}

		private static ITypeLifetimeManager GetLifetime(ServiceInfo srv)
		{
			Func<ServiceInfo, ITypeLifetimeManager> factory = lifetimeManagers[srv.InstanceLifetime];
			return factory(srv);
		}

		public void RegisterInstance<T>(T instance)
		{
			container.RegisterInstance(instance);
		}

        public IDependencyContainer CreateChildContainer()
	    {
	        IUnityContainer child = container.CreateChildContainer();
            return new UnityContainerAdapter(child);
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