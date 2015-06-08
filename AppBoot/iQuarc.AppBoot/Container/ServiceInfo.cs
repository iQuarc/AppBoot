﻿using System;

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
			this.From = from;
			this.To = to;
			this.ContractName = contractName;
			this.InstanceLifetime = lifetime;
		}

        public ServiceInfo(Type to, Func<IDependencyContainer, Type, object> factory)
        {
            this.To = to;
            this.InstanceFactory = InstanceFactory;
        }

        public Type From { get; private set; }

		public Type To { get; private set; }

		public string ContractName { get; private set; }

		public Lifetime InstanceLifetime { get; private set; }

        public Func<IDependencyContainer, Type, object> InstanceFactory { get; set; }
	}
}