using System;
using System.Collections;
using Microsoft.Practices.ServiceLocation;

namespace iQuarc.AppBoot
{
    public sealed class OperationContext : IDisposable
    {
        private readonly IServiceLocator serviceLocator;
        private readonly IDependencyContainer container;

        private Hashtable items;

        private bool isDisposed;

        private OperationContext()
        {
            container = ContextManager.GlobalContainer.CreateChildContainer();
            serviceLocator = container.AsServiceLocator;
        }

        public IDictionary Items
        {
            get
            {
                if (items == null)
                    items = new Hashtable();
                return items;
            }
        }

        public IServiceLocator ServiceLocator
        {
            get { return serviceLocator; }
        }

        public void Dispose()
        {
            if (isDisposed)
                return;

            DisposeItems();

            IDisposable c = container as IDisposable;
            if (c != null)
                c.Dispose();

            isDisposed = true;
        }

        private void DisposeItems()
        {
            foreach (var key in Items.Keys)
            {
                IDisposable disposableKey = key as IDisposable;
                if (disposableKey != null)
                    disposableKey.Dispose();

                IDisposable disposableValue = Items[key] as IDisposable;
                if (disposableValue != null)
                    disposableValue.Dispose();
            }
        }

        public static OperationContext Current
        {
            get { return ContextManager.Current; }
        }

        public static OperationContext CreateNew()
        {
            OperationContext operationContext = new OperationContext();
            ContextManager.SwitchContext(operationContext);
            return operationContext;
        }
    }
}