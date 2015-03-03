using System;
using System.Collections.Generic;
using System.Linq;

namespace iQuarc.AppBoot
{
	public class ServiceBuilder
	{
		private readonly Predicate<Type> filter;
		private readonly IList<ExportConfig> configs = new List<ExportConfig>(); 

		internal ServiceBuilder(Predicate<Type> filter)
		{
			this.filter = filter;
		}

		public void Export()
		{
			this.Export(c => {});
		}

		public void Export(Action<ExportBuilder> exportConfiguration)
		{
			RegisterConfig(new ExportConfig { ContractsProvider = t => new[] { t }, ExportConfiguration = exportConfiguration });
		}

		public void ExportInterfaces()
		{
			this.ExportInterfaces(x => true);
		}

		public void ExportInterfaces(Predicate<Type> interfaceFilter)
		{
			this.ExportInterfaces(interfaceFilter, c => {});
		}

		public void ExportInterfaces(Predicate<Type> interfaceFilter, Action<ExportBuilder> exportConfiguration)
		{
			Func<Type, IEnumerable<Type>> interfaces = t => t.GetInterfaces().Where(x => interfaceFilter(x));

			RegisterConfig(new ExportConfig { ExportConfiguration = exportConfiguration, ContractsProvider = interfaces });
		}

	    public bool IsMatch(Type type)
	    {
	        return filter(type);
	    }

	    internal IEnumerable<ServiceInfo> GetServicesFrom(Type type)
		{
			if (!IsMatch(type))
				yield break;

			foreach (ExportConfig config in configs)
			{
				IEnumerable<Type> contracts = config.ContractsProvider(type);

				foreach (Type contract in contracts)
				{
					ExportBuilder builder = new ExportBuilder();
					builder.AsContractType(contract);

					config.ExportConfiguration(builder);

					yield return builder.GetServiceInfo(type);
				}
			}
		}

	    private void RegisterConfig(ExportConfig config)
		{
			this.configs.Add(config);
		}

	    private class ExportConfig
		{
			internal Action<ExportBuilder> ExportConfiguration { get; set; }

			internal Func<Type, IEnumerable<Type>> ContractsProvider { get; set; } 
		}
	}
}