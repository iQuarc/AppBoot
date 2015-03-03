using System.Collections.Generic;

namespace iQuarc.AppBoot.UnitTests
{
	internal class ServiceEqualityComparer : IEqualityComparer<ServiceInfo>
	{
		public bool Equals(ServiceInfo x, ServiceInfo y)
		{
			return x.ContractName == y.ContractName &&
			       x.From == y.From &&
			       x.To == y.To &&
			       x.InstanceLifetime == y.InstanceLifetime;
		}

		public int GetHashCode(ServiceInfo obj)
		{
			return obj.GetHashCode();
		}
	}
}