using System;
using System.Diagnostics.CodeAnalysis;

namespace iQuarc.AppBoot
{
	public class ExportBuilder
	{
		private Type contractType;
		private string contractName;
		private Lifetime life = Lifetime.Instance;

		internal ExportBuilder(Type fromType)
		{
			FromType = fromType;
		}

		public Type FromType { get; private set; }

		public ExportBuilder AsContractName(string name)
		{
			this.contractName = name;
			return this;
		}

		public ExportBuilder AsContractType(Type type)
		{
			this.contractType = type;
			return this;
		}

		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Convenience call")]
        public ExportBuilder AsContractType<T>()
		{
			return AsContractType(typeof (T));
		}

		public ExportBuilder WithLifetime(Lifetime lifetime)
		{
			this.life = lifetime;
			return this;
		}

		internal ServiceInfo GetServiceInfo(Type type)
		{
			return new ServiceInfo(contractType ?? type, type, contractName, life);
		}
	}
}