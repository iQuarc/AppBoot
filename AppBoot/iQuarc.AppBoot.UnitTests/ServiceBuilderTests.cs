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
		public void GetServicesFrom_NoExportConfiguration_RegisterService()
		{
			ServiceBuilder builder = new ServiceBuilder(t => t == typeof(MyService));
			builder.Export();

            List<ServiceInfo> services = builder.GetServicesFrom(typeof(MyService)).ToList();
			Assert.Equal(1, services.Count);
		}

		[Fact]
		public void GetServicesFrom_NoExportConfiguration_RegisterSameTypeAsContract()
		{
            ServiceBuilder builder = new ServiceBuilder(t => t == typeof(MyService));
			builder.Export();

            ServiceInfo service = builder.GetServicesFrom(typeof(MyService)).First();

            ServiceInfo expected = new ServiceInfo(typeof(MyService), typeof(MyService), Lifetime.Instance);

			Assert.Equal(expected, service, comparer);
		}

		[Fact]
		public void GetServicesFrom_ConfigureContractType_RegisterContractType()
		{
            ServiceBuilder builder = new ServiceBuilder(t => t == typeof(MyService));
			builder.Export(c => c.AsContractType<IMyService1>());

            ServiceInfo service = builder.GetServicesFrom(typeof(MyService)).First();

            ServiceInfo expected = new ServiceInfo(typeof(MyService), typeof(IMyService1), Lifetime.Instance);

			Assert.Equal(expected, service, comparer);
		}

		[Fact]
		public void GetServicesFrom_ConfigureContractName_RegisterContractName()
		{
            ServiceBuilder builder = new ServiceBuilder(t => t == typeof(MyService));
			builder.Export(c => c.AsContractName("MyContract"));

            ServiceInfo service = builder.GetServicesFrom(typeof(MyService)).First();

            ServiceInfo expected = new ServiceInfo(typeof(MyService), typeof(MyService), "MyContract", Lifetime.Instance);

			Assert.Equal(expected, service, comparer);
		}

		[Fact]
		public void GetServicesFrom_ConfigureLifetime_RegisterLifetime()
		{
            ServiceBuilder builder = new ServiceBuilder(t => t == typeof(MyService));
			builder.Export(c => c.WithLifetime(Lifetime.Application));

            ServiceInfo service = builder.GetServicesFrom(typeof(MyService)).First();

            ServiceInfo expected = new ServiceInfo(typeof(MyService), typeof(MyService), Lifetime.Application);

			Assert.Equal(expected, service, comparer);
		}

	    [Fact]
	    public void GetServicesFrom_ExportInterfacesNoInterfaces_NothingRegistered()
	    {
	        ServiceBuilder builder = new ServiceBuilder(t => t == typeof (MyOtherService));
	        builder.ExportInterfaces();

	        ServiceInfo[] service = builder.GetServicesFrom(typeof (MyOtherService)).ToArray();

	        Assert.Equal(0, service.Length);
	    }

	    [Fact]
		public void GetServicesFrom_ExportInterfacesMultipleInterfaces_RegisterMultipleContractTypes()
		{
			ServiceBuilder builder = new ServiceBuilder(t => t == typeof(MyService));
			builder.ExportInterfaces();

			List<ServiceInfo> services = builder.GetServicesFrom(typeof(MyService)).ToList();

			ServiceInfo[] expected = 
			{
				new ServiceInfo(typeof (MyService), typeof (IMyService1), Lifetime.Instance),
				new ServiceInfo(typeof (MyService), typeof (IMyService2), Lifetime.Instance),
			};

			AssertEx.AreEquivalent(services, comparer.Equals, expected);
		}

		[Fact]
		public void GetServicesFrom_ExportInterfacesWithFilter_RegisterContractType()
		{
			ServiceBuilder builder = new ServiceBuilder(t => t == typeof(MyService));
			builder.ExportInterfaces(x => x == typeof(IMyService2));

			List<ServiceInfo> services = builder.GetServicesFrom(typeof(MyService)).ToList();

			ServiceInfo expected = new ServiceInfo(typeof (MyService), typeof (IMyService2), Lifetime.Instance);

			AssertEx.AreEquivalent(services, comparer.Equals, expected);
		}

		[Fact]
		public void GetServicesFrom_ExportInterfacesMultipleInterfacesWithExportConfig_RegisterConfig()
		{
			ServiceBuilder builder = new ServiceBuilder(t => t == typeof(MyService));
			builder.ExportInterfaces(c => true, b => b.WithLifetime(Lifetime.Application));

			List<ServiceInfo> services = builder.GetServicesFrom(typeof(MyService)).ToList();

			ServiceInfo[] expected = 
			{
				new ServiceInfo(typeof (MyService), typeof (IMyService1), Lifetime.Application),
				new ServiceInfo(typeof (MyService), typeof (IMyService2), Lifetime.Application),
			};

			AssertEx.AreEquivalent(services, comparer.Equals, expected);
		}

        [Fact]
        public void GetServicesFrom_MultipleConfigurations_AllConfigurationsExported()
        {
            ServiceBuilder builder = new ServiceBuilder(t => t == typeof(MyService));
            builder.Export(c => c.AsContractType<IMyService1>());
            builder.Export(c => c.AsContractType<IMyService2>());

            var services = builder.GetServicesFrom(typeof(MyService));


            ServiceInfo[] expected = new[]
	        {
	            new ServiceInfo(typeof (MyService), typeof (IMyService1), Lifetime.Instance),
	            new ServiceInfo(typeof (MyService), typeof (IMyService2), Lifetime.Instance),
	        };
            AssertEx.AreEquivalent(services, comparer.Equals, expected);
        }

	    [Fact]
	    public void GetServicesFrom_ExportBuilderWithMultipleConfigs_AllConfigurationsExported()
	    {
	        ServiceBuilder builder = new ServiceBuilder(t => t == typeof (MyService));
	        builder.Export(c => c.AsContractType<IMyService1>().AsContractName("IMyService1").WithLifetime(Lifetime.Application));

	        var services = builder.GetServicesFrom(typeof (MyService));


	        ServiceInfo[] expected = {new ServiceInfo(typeof (MyService), typeof (IMyService1), "IMyService1", Lifetime.Application)};
	        AssertEx.AreEquivalent(services, comparer.Equals, expected);
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

        private class MyOtherService
        { }
	}
}
