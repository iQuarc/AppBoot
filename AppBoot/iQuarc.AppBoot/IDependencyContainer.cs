using Microsoft.Practices.ServiceLocation;

namespace iQuarc.AppBoot
{
    /// <summary>
    ///     Represents an abstraction of a Dependency Injection Container
    ///     Existent container frameworks (like Unity) are adapted to this abstraction
    /// </summary>
    public interface IDependencyContainer
    {
        /// <summary>
        ///     Gets this Dependency Injection Container adapted to IServiceLocator interface.
        ///     This is the interface that is going to be used to request registered service implementations
        /// </summary>
        IServiceLocator AsServiceLocator { get; }

        /// <summary>
        ///     Registers a type to the container based on the service information.
        /// </summary>
        void RegisterService(ServiceInfo service);


        /// <summary>
        ///     Registers the instance into the container as a singleton (Lifetime.Application)
        /// </summary>
        void RegisterInstance<T>(T instance);


        /// <summary>
        ///     Creates a child container from the current instance
        /// </summary>
        IDependencyContainer CreateChildContainer();
    }
}