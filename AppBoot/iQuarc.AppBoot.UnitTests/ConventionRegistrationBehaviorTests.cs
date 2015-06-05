using System.Linq;
using iQuarc.xUnitEx;
using Xunit;

namespace iQuarc.AppBoot.UnitTests
{
    public class ConventionRegistrationBehaviorTests
    {
        private readonly ServiceEqualityComparer comparer = new ServiceEqualityComparer();

        [Fact]
        public void GetServicesFrom_TwoConventionsForType_BothExported()
        {
            ConventionRegistrationBehavior conventions = GetTarget();
            conventions.ForType<MyService>().Export(b => b.AsContractName("Configuration 1"));
            conventions.ForType<MyService>().Export(b => b.AsContractName("Configuration 2"));

            conventions.GetServicesFrom(typeof (MyService));

            var services = conventions.GetServicesFrom(typeof (MyService));

            ServiceInfo[] expected =
            {
                new ServiceInfo(typeof (MyService), typeof (MyService), "Configuration 1", Lifetime.Instance),
                new ServiceInfo(typeof (MyService), typeof (MyService), "Configuration 2", Lifetime.Instance)
            };
            AssertEx.AreEquivalent(services, comparer.Equals, expected);
        }

        [Fact]
        public void GetServicesFrom_NoConventions_NothingExported()
        {
            ConventionRegistrationBehavior conventions = GetTarget();

            conventions.GetServicesFrom(typeof (MyService));

            ServiceInfo[] services = conventions.GetServicesFrom(typeof (MyService)).ToArray();
            Assert.Equal(0, services.Length);
        }

        [Fact]
        public void ForType_ConfigurationForMatchingType_OneMatches()
        {
            ConventionRegistrationBehavior behavior = GetTarget();

            ServiceBuilder serviceBuilder = behavior.ForType<MyService>();

            Assert.True(serviceBuilder.IsMatch(typeof (MyService)));
        }

        [Fact]
        public void ForType_ConfigurationForNonMatchingType_MatchIsFalse()
        {
            ConventionRegistrationBehavior behavior = GetTarget();

            ServiceBuilder serviceBuilder = behavior.ForType<MyService>();

            Assert.False(serviceBuilder.IsMatch(typeof (MyOtherService)));
        }

        [Fact]
        public void ForTypesDerivedFrom_ConfigurationWithBaseAndInheritedTypes_MatchIsTrue()
        {
            ConventionRegistrationBehavior behavior = GetTarget();

            ServiceBuilder serviceBuilder = behavior.ForTypesDerivedFrom<MyBaseService>();

            Assert.True(serviceBuilder.IsMatch(typeof (MyService)));
        }

        [Fact]
        public void ForTypesDerivedFrom_ConfigurationUnrelatedTypes_MatchIsFalse()
        {
            ConventionRegistrationBehavior behavior = GetTarget();

            ServiceBuilder serviceBuilder = behavior.ForTypesDerivedFrom<MyService>();

            Assert.False(serviceBuilder.IsMatch(typeof (MyOtherService)));
        }

		[Fact]
		public void ForTypesMatching_ConfigurationWithServiceSuffix_MatchIsTrue()
		{
			ConventionRegistrationBehavior behavior = GetTarget();

			ServiceBuilder serviceBuilder = behavior.ForTypesMatching(t => t.Name.EndsWith("Service"));

			Assert.True(serviceBuilder.IsMatch(typeof(MyService)));
		}

		[Fact]
		public void ForTypesMatching_ConfigurationWithNoServiceSuffix_MatchIsFalse()
		{
			ConventionRegistrationBehavior behavior = GetTarget();

			ServiceBuilder serviceBuilder = behavior.ForTypesMatching(t => t.Name.EndsWith("Service"));

			Assert.False(serviceBuilder.IsMatch(typeof(Repository)));
		}

        private class MyService : MyBaseService
        {
        }

        private class MyOtherService
        {
        }

        private class MyBaseService
        {
        }

		private class Repository
		{
		}

	    private class Contract : IContract
	    {
	    }

		private interface IContract
		{
		}

        private static ConventionRegistrationBehavior GetTarget()
        {
            return new ConventionRegistrationBehavior();
        }
    }
}