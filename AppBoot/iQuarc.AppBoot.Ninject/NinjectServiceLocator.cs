using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Ninject;

namespace iQuarc.AppBoot.Ninject
{
    internal class NinjectServiceLocator : IServiceLocator
    {
        private readonly IKernel kernel;

        public NinjectServiceLocator(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public object GetService(Type serviceType)
        {
            return kernel.Get(serviceType);
        }

        public object GetInstance(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        public object GetInstance(Type serviceType, string key)
        {
            return kernel.Get(serviceType, key);
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        public TService GetInstance<TService>()
        {
            return kernel.Get<TService>();
        }

        public TService GetInstance<TService>(string key)
        {
            return kernel.Get<TService>(key);
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return kernel.GetAll<TService>();
        }
    }
}