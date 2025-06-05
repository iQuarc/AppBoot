using System;
using System.Collections.Generic;
using CommonServiceLocator;
using Unity;

namespace iQuarc.AppBoot.Unity
{
	internal sealed class UnityServiceLocator : IServiceLocator, IDisposable
	{
		private readonly IUnityContainer container;
		public UnityServiceLocator(IUnityContainer container)
		{
			this.container = container ?? throw new ArgumentNullException(nameof(container));
		}

		public object GetService(Type serviceType) => GetInstance(serviceType);
		public object GetInstance(Type serviceType)
		{
			return container.Resolve(serviceType);
		}

		public object GetInstance(Type serviceType, string key)
		{
			return container.Resolve(serviceType, key);
		}

		public TService GetInstance<TService>()
		{
			return container.Resolve<TService>();
		}

		public TService GetInstance<TService>(string key)
		{
			return container.Resolve<TService>(key);
		}

		public IEnumerable<object> GetAllInstances(Type serviceType)
		{
			return container.ResolveAll(serviceType);
		}

		public IEnumerable<TService> GetAllInstances<TService>()
		{
			return container.ResolveAll<TService>();
		}

		public void Dispose()
		{
			container.Dispose();
		}
	}
}