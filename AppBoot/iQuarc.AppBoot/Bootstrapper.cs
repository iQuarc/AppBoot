using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;

namespace iQuarc.AppBoot
{
	/// <summary>
	///     A class that starts the application and initializes it.
	/// </summary>
	public class Bootstrapper : IDisposable
	{
		private readonly IEnumerable<Assembly> applicationAssemblies;
		private readonly IDependencyContainer container;
		private readonly List<IRegistrationBehavior> behaviors = new List<IRegistrationBehavior>();

		public Bootstrapper(IEnumerable<Assembly> applicationAssemblies, IDependencyContainer container)
		{
			this.applicationAssemblies = applicationAssemblies;
			this.container = container;
		}

		public IEnumerable<Assembly> ApplicationAssemblies
		{
			get { return applicationAssemblies; }
		}

		public IServiceLocator ServiceLocator
		{
			get { return container.AsServiceLocator; }
		}

		public void AddRegistrationBehavior(IRegistrationBehavior behavior)
		{
			behaviors.Add(behavior);
		}

		public virtual void Run()
		{
			SetupServiceLocator();

			RegisterServices();

			InitApplication();
		}

		private void SetupServiceLocator()
		{
			IServiceLocator serviceLocator = container.AsServiceLocator;
			container.RegisterInstance(serviceLocator);
			Microsoft.Practices.ServiceLocation.ServiceLocator.SetLocatorProvider(serviceLocator.GetInstance<IServiceLocator>);
		}

		private void RegisterServices()
		{
			RegistrationsCatalog catalog = new RegistrationsCatalog();

			IEnumerable<Type> types = applicationAssemblies.SelectMany(a => a.GetTypes());

			foreach (Type type in types)
			{
				for (int i = 0; i < behaviors.Count; i++)
				{
					IRegistrationBehavior behavior = behaviors[i];

					IEnumerable<ServiceInfo> registrations = behavior.GetServicesFrom(type);
					foreach (ServiceInfo reg in registrations)
						catalog.Add(reg, i);
				}
			}

			foreach (ServiceInfo registration in catalog)
				container.RegisterService(registration);
		}

		private void InitApplication()
		{
			Application application = container.AsServiceLocator.GetInstance<Application>();
			application.Initialize();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				IDisposable disposable = container as IDisposable;
				if (disposable != null)
					disposable.Dispose();
			}
		}
	}
}