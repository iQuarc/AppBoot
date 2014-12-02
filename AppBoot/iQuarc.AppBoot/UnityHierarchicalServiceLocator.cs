using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;

namespace iQuarc.AppBoot
{
	public class UnityHierarchicalServiceLocator : IHierarchicalServiceLocator
	{
		private readonly IUnityContainer container;
		private readonly UnityServiceLocator innerLocator;

		public UnityHierarchicalServiceLocator(IUnityContainer container)
		{
			this.container = container;
			this.innerLocator = new UnityServiceLocator(container);
		}

		public object GetService(Type serviceType)
		{
			return innerLocator.GetService(serviceType);
		}

		public object GetInstance(Type serviceType)
		{
			return innerLocator.GetInstance(serviceType);
		}

		public object GetInstance(Type serviceType, string key)
		{
			return innerLocator.GetInstance(serviceType, key);
		}

		public IEnumerable<object> GetAllInstances(Type serviceType)
		{
			return innerLocator.GetAllInstances(serviceType);
		}

		public TService GetInstance<TService>()
		{
			return innerLocator.GetInstance<TService>();
		}

		public TService GetInstance<TService>(string key)
		{
			return innerLocator.GetInstance<TService>(key);
		}

		public IEnumerable<TService> GetAllInstances<TService>()
		{
			return innerLocator.GetAllInstances<TService>();
		}

		public IHierarchicalServiceLocator CreateChildServiceLocator()
		{
			IUnityContainer child = container.CreateChildContainer();
			return new UnityHierarchicalServiceLocator(child);
		}
	}
}