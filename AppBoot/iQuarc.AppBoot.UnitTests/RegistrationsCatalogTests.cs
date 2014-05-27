using System.Linq;
using Xunit;

namespace iQuarc.AppBoot.UnitTests
{
	public class RegistrationsCatalogTests
	{
		private RegistrationsCatalog catalog;

		public RegistrationsCatalogTests()
		{
			catalog = new RegistrationsCatalog();
		}

		[Fact]
		public void Add_DifferentContractDifferentPriorities_AllAdded()
		{
			ServiceInfo si1 = GetSi<string>("Contract");
			ServiceInfo si2 = GetSi<string>("Some Other Contract");

			catalog.Add(si1, 1);
			catalog.Add(si2, 2);

			Assert.True(catalog.Contains(si1), "Service info not contained, but expected");
			Assert.True(catalog.Contains(si2), "Service info not contained, but expected");
		}

		[Fact]
		public void Add_NoContractHigherPriorityAddedLater_HigherPriorityOverwrites()
		{
			ServiceInfo si1 = GetSi<string>(null);
			ServiceInfo si2 = GetSi<string>(null);

			catalog.Add(si1, 2);
			catalog.Add(si2, 5);

			AssertCatalogContainsOnly(si2);
		}

		[Fact]
		public void Add_NoContractLowerPriorityAddedLater_HigherPriorityIsNotOverwritten()
		{
			ServiceInfo si1 = GetSi<string>(null);
			ServiceInfo si2 = GetSi<string>(null);

			catalog.Add(si2, 5);
			catalog.Add(si1, 2);

			AssertCatalogContainsOnly(si2);
		}

		[Fact]
		public void Add_SameContractHigherPriorityAddedLater_AllLowerPriorityOverwritten()
		{
			ServiceInfo si1 = new ServiceInfo(typeof (string), typeof (int), "Contract", Lifetime.Instance);
			ServiceInfo si2 = new ServiceInfo(typeof (string), typeof (byte), "Contract", Lifetime.Instance);
			ServiceInfo si3 = new ServiceInfo(typeof (string), typeof (byte), "Contract", Lifetime.Instance);

			catalog.Add(si1, 1);
			catalog.Add(si2, 1);
			catalog.Add(si3, 3);

			AssertCatalogContainsOnly(si3);
		}

		[Fact]
		public void Add_SameContractLowerPriorityAddedLater_HigherPrioritiesNotOverwritten()
		{
			ServiceInfo si1 = new ServiceInfo(typeof (string), typeof (int), "Contract1", Lifetime.Instance);
			ServiceInfo si2 = new ServiceInfo(typeof (string), typeof (byte), "Contract2", Lifetime.Instance);
			ServiceInfo si3 = new ServiceInfo(typeof (string), typeof (byte), "Contract1", Lifetime.Instance);

			catalog.Add(si1, 3);
			catalog.Add(si2, 3);
			catalog.Add(si3, 1);

			Assert.True(catalog.Contains(si1), "Service info not contained, but expected");
			Assert.True(catalog.Contains(si2), "Service info not contained, but expected");
		}

		[Fact]
		public void Add_SameContractLowerPriorityAddedLater_LowerPriorityNotAdded()
		{
			ServiceInfo si1 = new ServiceInfo(typeof (string), typeof (int), "Contract", Lifetime.Instance);
			ServiceInfo si2 = new ServiceInfo(typeof (string), typeof (byte), "Contract", Lifetime.Instance);
			ServiceInfo si3 = new ServiceInfo(typeof (string), typeof (byte), "Contract", Lifetime.Instance);

			catalog.Add(si1, 3);
			catalog.Add(si2, 3);
			catalog.Add(si3, 1);

			Assert.False(catalog.Contains(si3), "Service info is contained, but NOT expected");
		}

		[Fact]
		public void Add_SameContractSamePriority_AllAdded()
		{
			ServiceInfo si1 = GetSi<string>("Contract1");
			ServiceInfo si2 = GetSi<string>("Contract2");

			catalog.Add(si2, 1);
			catalog.Add(si1, 1);

			Assert.True(catalog.Contains(si1), "Service info not contained, but expected");
			Assert.True(catalog.Contains(si2), "Service info not contained, but expected");
		}

		[Fact]
		public void Add_SameTypeAddedTwiceWithAndWithoutContract_BothRegistrationsExist()
		{
			ServiceInfo si1 = GetSi<string>(null);
			ServiceInfo si2 = GetSi<string>("Some Contract");

			catalog.Add(si2, 3);
			catalog.Add(si1, 2);

			Assert.True(catalog.Contains(si1), "Service without contract was expected");
			Assert.True(catalog.Contains(si2), "Service with contract was expected");
		}

		public void TestInitialize()
		{
			catalog = new RegistrationsCatalog();
		}

		private static ServiceInfo GetSi<TFrom>(string contractName)
		{
			return new ServiceInfo(typeof (TFrom), typeof (int), contractName, Lifetime.Instance);
		}

		private void AssertCatalogContainsOnly(ServiceInfo si2)
		{
			ServiceInfo[] catalogAsArray = catalog.ToArray();
			Assert.Same(si2, catalogAsArray[0]);
			Assert.True(catalogAsArray.Length == 1, "Catalog contains more registrations than expected");
		}
	}
}