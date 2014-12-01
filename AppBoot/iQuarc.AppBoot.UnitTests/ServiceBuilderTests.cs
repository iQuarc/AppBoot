using System.Collections.Generic;
using System.Linq;
using iQuarc.xUnitEx;
using Xunit;

namespace iQuarc.AppBoot.UnitTests
{
	public class ServiceBuilderTests
	{
		private readonly ServiceEqualityComparer comparer = new ServiceEqualityComparer();

		[Fact]
		public void Export_NoExportConfiguration_RegisterServiceBuilderInContainer()
		{
			ServiceBuilderContainer container = new ServiceBuilderContainer();

			ServiceBuilder builder = new ServiceBuilder(container, t => t == typeof(ServiceBuilderContainer));
			builder.Export();

			Assert.Equal(1, container.Builders.Count);
		}


		[Fact]
		public void Export_NoExportConfiguration_RegisterService()
		{
			ServiceBuilder builder = new ServiceBuilder(new ServiceBuilderContainer(), t => t == typeof(ServiceBuilderContainer));
			builder.Export();

			List<ServiceInfo> services = builder.GetServicesFrom(typeof(ServiceBuilderContainer)).ToList();
			Assert.Equal(1, services.Count);
		}

		[Fact]
		public void Export_NoExportConfiguration_RegisterSameTypeAsContract()
		{
			ServiceBuilder builder = new ServiceBuilder(new ServiceBuilderContainer(), t => t == typeof(ServiceBuilderContainer));
			builder.Export();

			ServiceInfo service = builder.GetServicesFrom(typeof(ServiceBuilderContainer)).First();

			ServiceInfo expected = new ServiceInfo(typeof(ServiceBuilderContainer), typeof(ServiceBuilderContainer), Lifetime.Instance);

			Assert.Equal(expected, service, comparer);
		}

		[Fact]
		public void Export_ConfigureContractType_RegisterContractType()
		{
			ServiceBuilder builder = new ServiceBuilder(new ServiceBuilderContainer(), t => t == typeof(ServiceBuilderContainer));
			builder.Export(c => c.AsContractType<IContainer<ServiceInfo>>());

			ServiceInfo service = builder.GetServicesFrom(typeof(ServiceBuilderContainer)).First();

			ServiceInfo expected = new ServiceInfo(typeof(ServiceBuilderContainer), typeof(IContainer<ServiceInfo>), Lifetime.Instance);

			Assert.Equal(expected, service, comparer);
		}

		[Fact]
		public void Export_ConfigureContractName_RegisterContractName()
		{
			ServiceBuilder builder = new ServiceBuilder(new ServiceBuilderContainer(), t => t == typeof(ServiceBuilderContainer));
			builder.Export(c => c.AsContractName("MyContract"));

			ServiceInfo service = builder.GetServicesFrom(typeof(ServiceBuilderContainer)).First();

			ServiceInfo expected = new ServiceInfo(typeof(ServiceBuilderContainer), typeof(ServiceBuilderContainer), "MyContract", Lifetime.Instance);

			Assert.Equal(expected, service, comparer);
		}

		[Fact]
		public void Export_ConfigureLifetime_RegisterLifetime()
		{
			ServiceBuilder builder = new ServiceBuilder(new ServiceBuilderContainer(), t => t == typeof(ServiceBuilderContainer));
			builder.Export(c => c.WithLifetime(Lifetime.Application));

			ServiceInfo service = builder.GetServicesFrom(typeof(ServiceBuilderContainer)).First();

			ServiceInfo expected = new ServiceInfo(typeof(ServiceBuilderContainer), typeof(ServiceBuilderContainer), Lifetime.Application);

			Assert.Equal(expected, service, comparer);
		}

		[Fact]
		public void Export_ExportInterfacesSingleInterface_RegisterContractType()
		{
			ServiceBuilder builder = new ServiceBuilder(new ServiceBuilderContainer(), t => t == typeof(ServiceBuilderContainer));
			builder.ExportInterfaces();

			ServiceInfo service = builder.GetServicesFrom(typeof(ServiceBuilderContainer)).First();

			ServiceInfo expected = new ServiceInfo(typeof(ServiceBuilderContainer), typeof(IContainer<ServiceBuilder>), Lifetime.Instance);

			Assert.Equal(expected, service, comparer);
		}

		[Fact]
		public void Export_ExportInterfacesMultipleInterfaces_RegisterMultipleContractTypes()
		{
			ServiceBuilder builder = new ServiceBuilder(new ServiceBuilderContainer(), t => t == typeof(MyService));
			builder.ExportInterfaces();

			List<ServiceInfo> services = builder.GetServicesFrom(typeof(MyService)).ToList();

			List<ServiceInfo> expected = new List<ServiceInfo>
			{
				new ServiceInfo(typeof (MyService), typeof (IMyService1), Lifetime.Instance),
				new ServiceInfo(typeof (MyService), typeof (IMyService2), Lifetime.Instance),
			};

			AssertEx.AreEquivalent(services, (x, y) => comparer.Equals(x, y), expected[0], expected[1]);
		}

		[Fact]
		public void Export_ExportInterfacesWithFilter_RegisterContractType()
		{
			ServiceBuilder builder = new ServiceBuilder(new ServiceBuilderContainer(), t => t == typeof(MyService));
			builder.ExportInterfaces(x => x == typeof(IMyService2));

			List<ServiceInfo> services = builder.GetServicesFrom(typeof(MyService)).ToList();

			ServiceInfo expected = new ServiceInfo(typeof (MyService), typeof (IMyService2), Lifetime.Instance);

			AssertEx.AreEquivalent(services, (x, y) => comparer.Equals(x, y), expected);
		}

		[Fact]
		public void Export_ExportInterfacesMultipleInterfacesWithExportConfig_RegisterConfig()
		{
			ServiceBuilder builder = new ServiceBuilder(new ServiceBuilderContainer(), t => t == typeof(MyService));
			builder.ExportInterfaces(c => true, b => b.WithLifetime(Lifetime.Application));

			List<ServiceInfo> services = builder.GetServicesFrom(typeof(MyService)).ToList();

			List<ServiceInfo> expected = new List<ServiceInfo>
			{
				new ServiceInfo(typeof (MyService), typeof (IMyService1), Lifetime.Application),
				new ServiceInfo(typeof (MyService), typeof (IMyService2), Lifetime.Application),
			};

			AssertEx.AreEquivalent(services, (x, y) => comparer.Equals(x, y), expected[0], expected[1]);
		}

		private class ServiceBuilderContainer : IContainer<ServiceBuilder>
		{
			public ServiceBuilderContainer()
			{
				this.Builders = new List<ServiceBuilder>();
			}

			public IList<ServiceBuilder> Builders { get; set; } 

			public void Register(ServiceBuilder item)
			{
				Builders.Add(item);
			}
		}

		private class MyService : IMyService1, IMyService2
		{
		}

		internal interface IMyService1
		{
		}

		internal interface IMyService2
		{
		}
	}
}
