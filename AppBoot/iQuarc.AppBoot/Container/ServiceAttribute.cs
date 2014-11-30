using System;
using System.Diagnostics.CodeAnalysis;

namespace iQuarc.AppBoot
{
	/// <summary>
	///     Declares a service implementation, by decorating the class that implements it.
	///     It may also specify the lifetime of the service instance by using the Lifetime enum
	/// </summary>
	[SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "This attribute may be inherited by client applications to extend the registration behaviors")]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ServiceAttribute : Attribute
	{
		public ServiceAttribute()
		{
		}

		public ServiceAttribute(Type exportType)
			: this(null, exportType)
		{
		}

		public ServiceAttribute(string contractName)
			: this(contractName, null)
		{
		}

		public ServiceAttribute(Lifetime lifetime)
			: this(null, null, lifetime)
		{
		}

		public ServiceAttribute(string contractName, Lifetime lifetime)
			: this(contractName, null, lifetime)
		{
		}

		public ServiceAttribute(string contractName, Type exportType)
			: this(contractName, exportType, Lifetime.Instance)
		{
		}

		public ServiceAttribute(Type exportType, Lifetime lifetime)
			: this(null, exportType, lifetime)
		{
		}

		public ServiceAttribute(string contractName, Type exportType, Lifetime lifetime)
		{
			ContractName = contractName;
			ExportType = exportType;
			Lifetime = lifetime;
		}

		public string ContractName { get; private set; }

		public Type ExportType { get; private set; }

		public Lifetime Lifetime { get; private set; }
	}
}