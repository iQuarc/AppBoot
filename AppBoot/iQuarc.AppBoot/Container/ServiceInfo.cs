using System;

namespace iQuarc.AppBoot
{
	/// <summary>
	///     Contains information about a service that is going to be registered into the Dependency Injection Container
	/// </summary>
	public class ServiceInfo
	{
		public ServiceInfo(Type from, Type to, Lifetime lifetime)
			: this(from, to, null, lifetime)
		{
		}

		public ServiceInfo(Type from, Type to, string contractName, Lifetime lifetime)
		{
			From = from;
			To = to;
			ContractName = contractName;
			InstanceLifetime = lifetime;
		}

		public Type From { get; private set; }

		public Type To { get; private set; }

		public string ContractName { get; private set; }

		public Lifetime InstanceLifetime { get; private set; }
	}
}