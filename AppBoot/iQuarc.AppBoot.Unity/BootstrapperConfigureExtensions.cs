namespace iQuarc.AppBoot.Unity
{
	public static class BootstrapperConfigureExtensions
	{
		public static IBootstrapper ConfigureWithUnity(this IBootstrapper bootstrapper)
		{
			return bootstrapper.ConfigureWith(new UnityContainerAdapter());
		}

		public static IBootstrapper ConfigureWithUnity(this IBootstrapper bootstrapper, UnityContainerOptions options)
		{
			if (options.DisposeDisposables)
			{
				IExtensionsFactory disposablesExtension = new ExtensionsFactory(() => new DisposablesContainerExtension());
				return bootstrapper.ConfigureWith(new UnityContainerAdapter(disposablesExtension));
			}

			return ConfigureWithUnity(bootstrapper);
		}

		public class UnityContainerOptions
		{
			public bool DisposeDisposables = false;
		}
	}
}