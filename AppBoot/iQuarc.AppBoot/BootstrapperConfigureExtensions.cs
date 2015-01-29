namespace iQuarc.AppBoot
{
    public static class BootstrapperConfigureExtensions
    {
        public static IBootstrapper ConfigureWith(this IBootstrapper bootstrapper, IDependencyContainer container)
        {
            bootstrapper.Configuration.SetSettingInstance(container);
            return bootstrapper;
        }

        public static IBootstrapper ConfigureWith(this IBootstrapper bootstrapper, IContextStore contextStore)
        {
            bootstrapper.Configuration.SetSettingInstance(contextStore);
            return bootstrapper;
        }

        public static IBootstrapper ConfigureWithCallContextStore(this IBootstrapper bootstrapper)
        {
            return bootstrapper.ConfigureWith(new CallContextStore());
        }

        public static IBootstrapper AddRegistrationBehavior(this IBootstrapper bootstrapper, IRegistrationBehavior behavior)
        {
            bootstrapper.AddRegistrationBehavior(behavior);
            return bootstrapper;
        }
    }
}