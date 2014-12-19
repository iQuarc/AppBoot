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
        private readonly IDependencyContainer container;
        private readonly IServiceLocator serviceLocator;

        private readonly IEnumerable<Assembly> applicationAssemblies;
        private readonly List<IRegistrationBehavior> behaviors = new List<IRegistrationBehavior>();

        public Bootstrapper(IEnumerable<Assembly> applicationAssemblies, IDependencyContainer container)
            : this(applicationAssemblies, container, new CallContextStore())
        {
        }

        public Bootstrapper(IEnumerable<Assembly> applicationAssemblies, IDependencyContainer container, IContextStore contextStore)
        {
            this.applicationAssemblies = applicationAssemblies;
            this.container = container;
            this.serviceLocator = container.AsServiceLocator;

            InitContextManager(container, contextStore);
        }

        private static void InitContextManager(IDependencyContainer container, IContextStore contextStore)
        {
            ContextManager.GlobalContainer = container;
            ContextManager.SetContextStore(contextStore);
        }

        public IEnumerable<Assembly> ApplicationAssemblies
        {
            get { return applicationAssemblies; }
        }

        public IServiceLocator ServiceLocator
        {
            get { return serviceLocator; }
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
            Microsoft.Practices.ServiceLocation.ServiceLocator.SetLocatorProvider(GetServiceLocator);
        }

        private IServiceLocator GetServiceLocator()
        {
            if (OperationContext.Current != null)
                return OperationContext.Current.ServiceLocator;

            return serviceLocator;
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
            Application application = serviceLocator.GetInstance<Application>();
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