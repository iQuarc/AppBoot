using System;
using iQuarc.xUnitEx;
using Microsoft.Practices.ServiceLocation;
using Moq;
using Xunit;

namespace iQuarc.AppBoot.UnitTests
{
    public class OperationContextTests
    {
        private DependencyContainerDouble lastChildContainer = new DependencyContainerDouble(d => { });

        public OperationContextTests()
        {
            ContextManager.GlobalContainer = new DependencyContainerDouble(ChildContainerCreatedCallback);
            ContextManager.SetContextStore(new CallContextStoreDouble());
        }

        private void ChildContainerCreatedCallback(DependencyContainerDouble childContainer)
        {
            this.lastChildContainer = childContainer;
        }

        [Fact]
        public void Items_OnNewContext_IsEmpty()
        {
            OperationContext context = GetNewTarget();

            Assert.Empty(context.Items);
        }

        [Fact]
        public void Items_AddValue_ValueRetrieved()
        {
            OperationContext context = GetNewTarget();
            context.Items.Add("KEY", "some value");

            string actual = (string) context.Items["KEY"];

            Assert.Equal("some value", actual);
        }

        [Fact]
        public void Items_GetValueWithWrongKey_Null()
        {
            OperationContext context = GetNewTarget();
            context.Items.Add("key", "some value");

            string actual = (string) context.Items["wrong key"];

            Assert.Null(actual);
        }

        [Fact]
        public void ServiceLocator_OnNewContext_IsNotNull()
        {
            OperationContext context = GetNewTarget();
            Assert.NotNull(context.ServiceLocator);
        }

        [Fact]
        public void ServiceLocator_NewContextCreatedOnExistentOne_DifferentServiceLocatorInstance()
        {
            OperationContext context1 = GetNewTarget();
            IServiceLocator serviceLocator1 = context1.ServiceLocator;

            OperationContext context2 = OperationContext.CreateNew();
            IServiceLocator serviceLocator2 = context2.ServiceLocator;

            Assert.NotSame(serviceLocator1, serviceLocator2);
        }

        [Fact]
        public void ServiceLocator_AfterCalled_SameWithTheChildContainer()
        {
            OperationContext context = GetNewTarget();

            IServiceLocator childContainerLocator = lastChildContainer.AsServiceLocator;

            Assert.Same(childContainerLocator, context.ServiceLocator);
        }

        [Fact]
        public void Dispose_ItemsContainsMoreDisposablesValues_AllDisposableValuesAreDisposed()
        {
            OperationContext context = GetNewTarget();
            
            Disposable disposable1 = new Disposable();
            context.Items.Add("key1", disposable1);
            
            Disposable disposable2 = new Disposable();
            context.Items.Add("key2", disposable2);
            
            context.Items.Add("key3", "some item");


            context.Dispose();

            Assert.True(disposable1.IsDisposed);
            Assert.True(disposable2.IsDisposed);
        }

        [Fact]
        public void Dispose_ItemsContainsMoreDisposablesKeys_AllDisposableKeysAreDisposed()
        {
            OperationContext context = GetNewTarget();
            
            Disposable disposable1 = new Disposable();
            context.Items.Add(disposable1, "item 1");
            
            Disposable disposable2 = new Disposable();
            context.Items.Add(disposable2, "item 2");
            
            context.Items.Add("some key", "some item");


            context.Dispose();

            Assert.True(disposable1.IsDisposed);
            Assert.True(disposable2.IsDisposed);
        }

        [Fact]
        public void Dispose_ItemsIsEmpty_DoesNotThrow()
        {
            OperationContext context = GetNewTarget();
            
            Action act = context.Dispose;

            AssertEx.ShouldNotThrow(act);
        }

        [Fact]
        public void Dispose_OnNewContext_InnerContainerIsDisposed()
        {
            OperationContext context = OperationContext.CreateNew();

            context.Dispose();

            Assert.True(lastChildContainer.Disposed);
        }

        [Fact]
        public void Current_ContextNotCreated_IsNull()
        {
            Assert.Null(OperationContext.Current);
        }

        [Fact]
        public void CreateNew_AfterCalled_ContainerAsChildOfTheGlobalContainer()
        {
            OperationContext.CreateNew();

            Assert.Same(ContextManager.GlobalContainer, lastChildContainer.Parent);
        }

        [Fact]
        public void CreateNew_AnotherConextExisted_CurrentPointsToTheNewContext()
        {
            OperationContext oldContext = GetNewTarget();

            OperationContext newContext = OperationContext.CreateNew();
            
            Assert.Same(OperationContext.Current, newContext);
        }

        [Fact]
        public void CreateNew_AnotherConextExisted_OldContextDisposed()
        {
            OperationContext oldContext = GetNewTarget();
            Disposable disposable = new Disposable();
            oldContext.Items.Add("key", disposable);

            OperationContext.CreateNew();

            Assert.True(disposable.IsDisposed);
        }

        private static OperationContext GetNewTarget()
        {
            return OperationContext.CreateNew();
        }

        private class DependencyContainerDouble : IDependencyContainer, IDisposable
        {
            private readonly Action<DependencyContainerDouble> childContainerCreatedCallback;

            public DependencyContainerDouble(Action<DependencyContainerDouble> childContainerCreatedCallback)
            {
                this.childContainerCreatedCallback = childContainerCreatedCallback;
                Mock<IServiceLocator> slDouble = new Mock<IServiceLocator>();
                this.AsServiceLocator = slDouble.Object;
            }

            public IServiceLocator AsServiceLocator { get; private set; }

            public DependencyContainerDouble Parent { get; private set; }

            public IDependencyContainer CreateChildContainer()
            {
                var child = new DependencyContainerDouble(childContainerCreatedCallback);
                child.Parent = this;

                childContainerCreatedCallback(child);
                return child;
            }

            public void RegisterService(ServiceInfo service)
            {
                throw new NotImplementedException();
            }

            public void RegisterInstance<T>(T instance)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                Disposed = true;
            }

            public bool Disposed { get; set; }
        }

       

        private class CallContextStoreDouble : IContextStore
        {
            private object value;

            public object Get(string key)
            {
                return value;
            }

            public void Set(object context, string key)
            {
                value = context;
            }
        }

        class Disposable : IDisposable
        {
            public void Dispose()
            {
                IsDisposed = true;
            }

            public bool IsDisposed { get; set; }
        }
    }
}