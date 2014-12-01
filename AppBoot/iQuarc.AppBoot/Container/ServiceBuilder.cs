using System;
using System.Collections.Generic;
using System.Linq;

namespace iQuarc.AppBoot
{
	public class ServiceBuilder
	{
		private readonly IContainer<ServiceBuilder> container;
		private readonly Predicate<Type> filter;
		private readonly IList<ExportConfig> configs = new List<ExportConfig>(); 

		internal ServiceBuilder(IContainer<ServiceBuilder> container, Predicate<Type> filter)
		{
			this.container = container;
			this.filter = filter;
		}

		public void Export()
		{
			this.Export(c => {});
		}

		public void Export(Action<ExportBuilder> exportConfiguration)
		{
			configs.Add(new ExportConfig { ContractsProvider = t => new[] { t }, ExportConfiguration = exportConfiguration });
			container.Register(this);
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

			configs.Add(new ExportConfig { ExportConfiguration = exportConfiguration, ContractsProvider = interfaces });
			container.Register(this);
		}

		internal IEnumerable<ServiceInfo> GetServicesFrom(Type type)
		{
			bool isMatch = filter(type);
			if (!isMatch)
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

		internal void RegisterConfig(ExportConfig config)
		{
			this.configs.Add(config);
		}

		internal class ExportConfig
		{
			internal Action<ExportBuilder> ExportConfiguration { get; set; }

			internal Func<Type, IEnumerable<Type>> ContractsProvider { get; set; } 
		}
	}
}