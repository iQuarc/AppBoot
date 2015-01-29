namespace iQuarc.AppBoot.Unity
{
    public static class BootstrapperConfigureExtensions
    {
        public static IBootstrapper ConfigureWithUnity(IBootstrapper bootstrapper)
        {
            return bootstrapper.ConfigureWith(new UnityContainerAdapter());
        }
    }
}