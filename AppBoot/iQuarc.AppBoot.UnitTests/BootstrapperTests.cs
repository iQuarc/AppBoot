using System;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;
using Moq;
using Xunit;

namespace iQuarc.AppBoot.UnitTests
{
	public class BootstrapperTests
	{
		[Fact]
		public void Run_AfterCalled_ServiceLocatorIsAvailableAsStaticSingleton()
		{
			IServiceLocator serviceLocatorStub = GetFakeServiceLocator();
			Mock<IDependencyContainer> containerStub = GetFakeContainer(serviceLocatorStub);

			Bootstrapper bootstrapper = GetTarget(containerStub);

			bootstrapper.Run();

			IServiceLocator staticLocator = ServiceLocator.Current;
			Assert.Same(serviceLocatorStub, staticLocator);
		}

		[Fact]
		public void Run_AfterCalled_ServiceLocatorRegisteredInTheContainer()
		{
			IServiceLocator serviceLocatorStub = GetFakeServiceLocator();
			Mock<IDependencyContainer> containerMock = GetFakeContainer(serviceLocatorStub);

			Bootstrapper boostrapper = GetTarget(containerMock);

			boostrapper.Run();

		    IServiceLocator sl = boostrapper.ServiceLocator.GetInstance<IServiceLocator>();
            Assert.Same(boostrapper.ServiceLocator, sl);
		}

		[Fact]
		public void Run_RegistrationBehaviorReturnsOneService_TypeRegistered()
		{
			Type testType = typeof (TestType);
			ServiceInfo testSi = new ServiceInfo(testType, testType, "test contract", Lifetime.Instance);
			IRegistrationBehavior regBehaviorStub = GetRegBehaviorStub(testSi);

			Mock<IDependencyContainer> containerMock = GetFakeContainer();
			
			Bootstrapper bootstrapper = GetTargetWithAssembly(containerMock);
			bootstrapper.AddRegistrationBehavior(regBehaviorStub);

			bootstrapper.Run();

			containerMock.Verify(c => c.RegisterService(testSi), Times.AtLeastOnce);
		}

		[Fact]
		public void Dispose_DisposableDependencyContainer_DisposesContainer()
		{
			Mock<IDependencyContainer> containerMock = GetFakeContainer();
			Mock<IDisposable> disposable = containerMock.As<IDisposable>();

			Bootstrapper bootstrapper = GetTargetWithAssembly(containerMock);

			bootstrapper.Dispose();
			disposable.Verify(c => c.Dispose(), Times.Once);
		}

		private static Bootstrapper GetTarget(Mock<IDependencyContainer> fakeContainer)
		{
			return new Bootstrapper(new Assembly[] {}, fakeContainer.Object);
		}

		private Bootstrapper GetTargetWithAssembly(Mock<IDependencyContainer> container)
		{
			Assembly[] assemblies = {typeof (BootstrapperTests).Assembly};
			return new Bootstrapper(assemblies, container.Object);
		}

		private static IServiceLocator GetFakeServiceLocator()
		{
			IModule[] modules = {};

			Mock<IServiceLocator> fakeServiceLocator = new Mock<IServiceLocator>();
			IServiceLocator serviceLocator = fakeServiceLocator.Object;

			fakeServiceLocator.Setup(s => s.GetInstance<Application>())
				.Returns(new Application(modules));
			fakeServiceLocator.Setup(s => s.GetInstance<IServiceLocator>())
				.Returns(serviceLocator);

			return serviceLocator;
		}

		private static Mock<IDependencyContainer> GetFakeContainer(IServiceLocator serviceLocatorStub = null)
		{
			IServiceLocator serviceLocator = serviceLocatorStub ?? GetFakeServiceLocator();

			Mock<IDependencyContainer> containerStub = new Mock<IDependencyContainer>();
			containerStub.Setup(c => c.AsServiceLocator).Returns(serviceLocator);

			containerStub.As<IDisposable>();

			return containerStub;
		}

		private static IRegistrationBehavior GetRegBehaviorStub(ServiceInfo si)
		{
			Mock<IRegistrationBehavior> regBehaviorStub = new Mock<IRegistrationBehavior>();
			regBehaviorStub.Setup(r => r.GetServicesFrom(It.IsAny<Type>()))
				.Returns(new[] {si});

			return regBehaviorStub.Object;
		}


		private class TestType
		{
		}
	}
}