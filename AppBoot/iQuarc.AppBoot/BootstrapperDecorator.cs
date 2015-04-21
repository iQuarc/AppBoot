using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;

namespace iQuarc.AppBoot
{
    public abstract class BootstrapperDecorator : IBootstrapper, IDisposable
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                IDisposable disposable = bootstrapper as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
        }
    }
}