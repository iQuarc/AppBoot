using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using Microsoft.Practices.ServiceLocation;

namespace iQuarc.AppBoot.WebApi
{
	public sealed class DependencyContainerResolver : IDependencyResolver
	{
		private readonly DependencyScope rootResolver;

		public DependencyContainerResolver(IServiceLocator serviceLocator)
		{
			this.rootResolver = new DependencyScope(serviceLocator);
		}

		public IExceptionLogger Logger
		{
			get { return rootResolver.Logger; }
			set { rootResolver.Logger = value; }
		}

		public object GetService(Type serviceType)
		{
			return rootResolver.GetService(serviceType);
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			return rootResolver.GetServices(serviceType);
		}

		public void Dispose()
		{
			rootResolver.Dispose();
		}

		public IDependencyScope BeginScope()
		{
			OperationContext context = OperationContext.CreateNew();
			return new DependencyScope(context) {Logger = Logger};	
		}

		private class DependencyScope : IDependencyScope
		{
			private readonly IServiceLocator serviceLocator;
			private readonly OperationContext context;

			public DependencyScope(IServiceLocator serviceLocator)
			{
				this.serviceLocator = serviceLocator;
			}

			public DependencyScope(OperationContext context)
			{
				this.context = context;
				this.serviceLocator = context.ServiceLocator;
			}

			public void Dispose()
			{
				if (context != null)
					context.Dispose();

				IDisposable disposable = serviceLocator as IDisposable;
				if (disposable != null)
					disposable.Dispose();
			}

			public IExceptionLogger Logger { get; set; }

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

			private void Log(Exception exception)
			{
				if (Logger != null)
					Logger.Log(exception);
			}
		}
	}
}