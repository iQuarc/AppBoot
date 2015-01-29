using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;

namespace iQuarc.AppBoot
{
    public interface IBootstrapper
    {
        IEnumerable<Assembly> ApplicationAssemblies { get; }
        IServiceLocator ServiceLocator { get; }
        BootstrapperConfig Configuration { get; }
        void AddRegistrationBehavior(IRegistrationBehavior behavior);
        void Run();
    }
}