using System.Web.Http;
using CommonServiceLocator;

namespace iQuarc.AppBoot.WebApi
{
	public static class BootstrapperExtensions
	{
		public static Bootstrapper ConfigureWebApi(this Bootstrapper bootstrapper, HttpConfiguration config)
		{
		    bootstrapper.ConfigureWith(new HttpRequestContextStore());

			IServiceLocator serviceLocator = bootstrapper.ServiceLocator;
			config.DependencyResolver = new DependencyContainerResolver(serviceLocator);

			return bootstrapper;
		}
	}
}