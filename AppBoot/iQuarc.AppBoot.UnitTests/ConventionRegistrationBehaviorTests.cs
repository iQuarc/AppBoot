using System.Linq;
using Xunit;

namespace iQuarc.AppBoot.UnitTests
{
	public class ConventionRegistrationBehaviorTests
	{
		private readonly ServiceEqualityComparer comparer = new ServiceEqualityComparer();

		[Fact]
		public void ForType_SpecificType_CreatesServiceBuilderWithSingleType()
		{
			ConventionRegistrationBehavior behavior = new ConventionRegistrationBehavior();
			ServiceBuilder builder = behavior.ForType<MyService>();
			builder.Export();

			ServiceInfo service = builder.GetServicesFrom(typeof(MyService)).First();
			ServiceInfo expected = new ServiceInfo(typeof(MyService), typeof(MyService), Lifetime.Instance);

			Assert.Equal(expected, service, comparer);
		}

		private class MyService
		{
		}
	}
}