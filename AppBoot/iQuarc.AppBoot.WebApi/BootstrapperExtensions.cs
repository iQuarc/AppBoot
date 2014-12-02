using System.Web.Http;
using Microsoft.Practices.ServiceLocation;

namespace iQuarc.AppBoot.WebApi
{
	public static class BootstrapperExtensions
	{
		public static Bootstrapper ConfigureWebApi(this Bootstrapper bootstrapper, HttpConfiguration config)
		{
			IServiceLocator serviceLocator = bootstrapper.ServiceLocator;
			config.DependencyResolver = new DependencyContainerResolver(serviceLocator);

			return bootstrapper;
		}
	}
}