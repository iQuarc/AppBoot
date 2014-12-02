using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http.Dependencies;
using Microsoft.Practices.ServiceLocation;

namespace iQuarc.AppBoot.WebApi
{
	public sealed class DependencyContainerResolver : IDependencyResolver
	{
		private readonly IServiceLocator serviceLocator;

		public DependencyContainerResolver(IServiceLocator serviceLocator)
		{
			this.serviceLocator = serviceLocator;
		}

		public void Dispose()
		{
			IDisposable disposable = serviceLocator as IDisposable;
			if (disposable != null)
				disposable.Dispose();
		}

		public object GetService(Type serviceType)
		{
			try
			{
				return serviceLocator.GetInstance(serviceType);
			}
			catch (ActivationException ex)
			{
				Debug.WriteLine(ex);
				return null;
			}
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			try
			{
				return serviceLocator.GetAllInstances(serviceType);
			}
			catch (ActivationException ex)
			{
				Debug.WriteLine(ex);
				return Enumerable.Empty<object>();
			}
		}

		public IDependencyScope BeginScope()
		{
			IHierarchicalServiceLocator locator = serviceLocator as IHierarchicalServiceLocator;

			if (locator == null) 
				return this;

			IHierarchicalServiceLocator child = locator.CreateChildServiceLocator();
			return new DependencyContainerResolver(child);
		}
	}
}
