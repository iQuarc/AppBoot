using System;
using System.Collections.Generic;
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
			this.Logger = new DebugExceptionLogger();
		}

		public IExceptionLogger Logger { get; set; }

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
				Log(ex);
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
				Log(ex);
				return Enumerable.Empty<object>();
			}
		}

	    public IDependencyScope BeginScope()
	    {
	        OperationContext contex = OperationContext.Current;
	        if (contex == null)
	            contex = OperationContext.CreateNew();
	     
            return new DependencyContainerResolver(contex.ServiceLocator);
	    }

	    public IDependencyScope BeginScope_2()
	    {
            // not yet sure how web.api uses this
	        OperationContext contex = OperationContext.CreateNew();
	        return new DependencyContainerResolver(contex.ServiceLocator);
	    }

	    public IDependencyScope BeginScope_o()
		{
			IHierarchicalServiceLocator locator = serviceLocator as IHierarchicalServiceLocator;

			if (locator == null) 
				return this;

			IHierarchicalServiceLocator child = locator.CreateChildServiceLocator();
			return new DependencyContainerResolver(child);
		}

		private void Log(Exception exception)
		{
			if (Logger != null)
				Logger.Log(exception);
		}
	}
}
