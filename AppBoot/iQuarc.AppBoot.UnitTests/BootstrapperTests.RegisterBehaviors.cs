using System;
using System.Collections.Generic;
using System.Reflection;
using iQuarc.SystemEx;
using Microsoft.Practices.ServiceLocation;
using Moq;
using Xunit;

namespace iQuarc.AppBoot.UnitTests
{
	public class BootstrapperTestsWithMoreRegisterBehaviors
	{
		[Fact]
		public void Run_TypesOnSameInterfaceRegisteredByDifferentBehaviors_LastRegisteredBehaviorOverwrites()
		{
			DummyAssembly assembly = new DummyAssembly(typeof (Implementation), typeof (ProxyImpl));
			DummyContainer dummyContainer = new DummyContainer();
			Bootstrapper bootstrapper = new Bootstrapper(new[] {assembly}, dummyContainer);

			IRegistrationBehavior proxyBeh = new DummyBehavior("Proxy");
			IRegistrationBehavior serviceBeh = new DummyBehavior("Service");
			bootstrapper.AddRegistrationBehavior(proxyBeh);
			bootstrapper.AddRegistrationBehavior(serviceBeh);

			bootstrapper.Run();

			ServiceInfo resolved = dummyContainer.GetRegistration(typeof (IService));
			Assert.Equal(typeof (Implementation), resolved.To);
		}

		private interface IService
		{
		}

		[Service("Service")] //using contract name as annotation
		private class Implementation : IService
		{
		}

		[Service("Proxy")] //using contract name as annotation
		private class ProxyImpl : IService
		{
		}

		private class DummyBehavior : IRegistrationBehavior
		{
			private readonly string annotation;

			public DummyBehavior(string annotation)
			{
				this.annotation = annotation;
			}

			public IEnumerable<ServiceInfo> GetServicesFrom(Type type)
			{
				ServiceAttribute atr = type.GetAttribute<ServiceAttribute>();
				if (atr.ContractName == annotation)
					return new[] {new ServiceInfo(typeof (IService), type, Lifetime.Instance)};

				return new ServiceInfo[] {};
			}
		}


		private class DummyAssembly : Assembly
		{
			private readonly Type[] types;

			public DummyAssembly(params Type[] types)
			{
				this.types = types;
			}

			public override Type[] GetTypes()
			{
				return types;
			}
		}

		private class DummyContainer : IDependencyContainer
		{
			private readonly Dictionary<Type, ServiceInfo> dic = new Dictionary<Type, ServiceInfo>();

			public DummyContainer()
			{
				AsServiceLocator = GetFakeServiceLocator();
			}

			public IServiceLocator AsServiceLocator { get; private set; }

			public void RegisterService(ServiceInfo service)
			{
				dic[service.From] = service;
			}

			public ServiceInfo GetRegistration(Type from)
			{
				return dic[from];
			}

			public void RegisterInstance<T>(T instance)
			{
			}

		    public IDependencyContainer CreateChildContainer()
		    {
		        throw new NotImplementedException();
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
		}
	}
}