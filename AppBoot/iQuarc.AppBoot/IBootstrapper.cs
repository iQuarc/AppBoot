using System.Collections.Generic;
using System.Reflection;

namespace iQuarc.AppBoot
{
    public interface IBootstrapper
    {
        IEnumerable<Assembly> ApplicationAssemblies { get; }
        BootstrapperConfig Configuration { get; }
        void AddRegistrationBehavior(IRegistrationBehavior behavior);
        void Run();
    }
}