namespace iQuarc.AppBoot.Ninject
{
    public static class BootstrapperConfigureExtensions
    {
        public static IBootstrapper ConfigureWithNinject(this IBootstrapper bootstrapper)
        {
            return bootstrapper.ConfigureWith(new NinjectAdapter());
        }
    }
}