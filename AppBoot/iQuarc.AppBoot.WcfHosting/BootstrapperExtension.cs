using iQuarc.AppBoot.WcfHosting;

namespace iQuarc.AppBoot
{
    public static class BootstrapperExtension
    {
        public static Bootstrapper ConfigureWithWcfHosting(this Bootstrapper bootstrapper, string baseAddress)
        {
            bootstrapper.AddRegistrationBehavior(new WcfHostingRegistrationBehavior(baseAddress));
            return bootstrapper;
        }
    }
}
