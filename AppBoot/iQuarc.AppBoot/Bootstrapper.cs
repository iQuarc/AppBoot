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
    public sealed class Bootstrapper : IBootstrapper, IDisposable
    {
        private IDependencyContainer container;
        private IServiceLocator serviceLocator;

        private readonly IEnumerable<Assembly> applicationAssemblies;
        private readonly List<IRegistrationBehavior> behaviors = new List<IRegistrationBehavior>();

        public Bootstrapper(IEnumerable<Assembly> applicationAssemblies)
        {
            this.applicationAssemblies = applicationAssemblies;
            this.Configuration = new BootstrapperConfig();
        }

        public IEnumerable<Assembly> ApplicationAssemblies
        {
            get { return applicationAssemblies; }
        }

        public IServiceLocator ServiceLocator
        {
            get { return serviceLocator; }
        }

        public BootstrapperConfig Configuration { get; private set; }

        public void AddRegistrationBehavior(IRegistrationBehavior behavior)
        {
            behaviors.Add(behavior);
        }

        public void Run()
        {
            ConfigureDependencyContainer();
            ConfigureContextManager();

            RegisterServices();

            InitApplication();
        }

        private void ConfigureDependencyContainer()
        {
            container = Configuration.GetSetting<IDependencyContainer>();
            serviceLocator = container.AsServiceLocator;
            Microsoft.Practices.ServiceLocation.ServiceLocator.SetLocatorProvider(GetServiceLocator);
        }

        private IServiceLocator GetServiceLocator()
        {
            if (OperationContext.Current != null)
                return OperationContext.Current.ServiceLocator;

            return serviceLocator;
        }

        private void ConfigureContextManager()
        {
            IContextStore contextStore = Configuration.GetSetting<IContextStore>();

            ContextManager.GlobalContainer = container;
            ContextManager.SetContextStore(contextStore);
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
            IDisposable disposable = container as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }
}