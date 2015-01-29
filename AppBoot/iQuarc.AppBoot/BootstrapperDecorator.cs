using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;

namespace iQuarc.AppBoot
{
    public abstract class BootstrapperDecorator : IBootstrapper
    {
        private readonly IBootstrapper bootstrapper;

        protected BootstrapperDecorator(IBootstrapper bootstrapper)
        {
            this.bootstrapper = bootstrapper;
        }

        public IEnumerable<Assembly> ApplicationAssemblies
        {
            get { return bootstrapper.ApplicationAssemblies; }
        }

        public IServiceLocator ServiceLocator
        {
            get { return bootstrapper.ServiceLocator; }
        }

        public BootstrapperConfig Configuration
        {
            get { return bootstrapper.Configuration; }
        }

        public virtual void AddRegistrationBehavior(IRegistrationBehavior behavior)
        {
            bootstrapper.AddRegistrationBehavior(behavior);
        }

        public virtual void Run()
        {
            bootstrapper.Run();
        }
    }
}