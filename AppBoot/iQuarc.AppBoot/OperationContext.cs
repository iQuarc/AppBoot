using System;
using System.Collections;
using Microsoft.Practices.ServiceLocation;

namespace iQuarc.AppBoot
{
    public class OperationContext : IDisposable
    {
        private readonly IServiceLocator serviceLocator;
        private readonly IDependencyContainer container;

        private Hashtable items;

        private bool isDisposed;

        protected OperationContext()
        {
            container = ContextManager.GlobalContainer.CreateChildContainer();
            serviceLocator = container.AsServiceLocator;
        }

        public IDictionary Items
        {
            get
            {
                if (this.items == null)
                    this.items = new Hashtable();
                return this.items;
            }
        }

        public IServiceLocator ServiceLocator
        {
            get { return serviceLocator; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
                return;

            if (disposing)
            {
                DisposeItems();

                IDisposable c = container as IDisposable;
                if (c != null)
                    c.Dispose();
            }

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

        public static T Create<T>() where T : OperationContext, new()
        {
            T operationContext = new T();
            ContextManager.SwitchContext(operationContext);
            return operationContext;
        }

        public static T Begin<T>(T operationContext) where T : OperationContext
        {
            ContextManager.SwitchContext(operationContext);
            return operationContext;
        }
    }
}