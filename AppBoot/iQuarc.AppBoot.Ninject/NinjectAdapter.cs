using System;
using Microsoft.Practices.ServiceLocation;
using Ninject;
using Ninject.Extensions.ChildKernel;
using Ninject.Extensions.NamedScope;
using Ninject.Syntax;

namespace iQuarc.AppBoot.Ninject
{
    internal sealed class NinjectAdapter : IDependencyContainer, IDisposable
    {
        private readonly IKernel kernel;

        public NinjectAdapter() : this(new StandardKernel())
        {
        }

        private NinjectAdapter(IKernel child)
        {
            this.kernel = child;
            this.AsServiceLocator = new NinjectServiceLocator(kernel);
        }

        public void RegisterService(ServiceInfo service)
        {
            IBindingWhenInNamedWithOrOnSyntax<object> config = kernel.Bind(service.From).To(service.To);

            if (service.ContractName != null)
                config.Named(service.ContractName);

            switch (service.InstanceLifetime)
            {
                case Lifetime.Instance:
                    config.InCallScope();
                    break;
                case Lifetime.AlwaysNew:
                    config.InTransientScope();
                    break;
                case Lifetime.Application:
                    config.InSingletonScope();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void RegisterInstance<T>(T instance)
        {
            kernel.Bind<T>().ToConstant(instance);
        }

        public IDependencyContainer CreateChildContainer()
        {
            IKernel child = new ChildKernel(kernel);
            return new NinjectAdapter(child);
        }

        public IServiceLocator AsServiceLocator { get; }

        public void Dispose()
        {
            kernel.Dispose();

            IDisposable disposable = AsServiceLocator as IDisposable;
            disposable?.Dispose();
        }
    }
}