using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using iQuarc.SystemEx.Priority;

namespace iQuarc.AppBoot
{
	[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "This is initialized by Unity Container")]
	internal sealed class Application
	{
		private readonly IEnumerable<IModule> modules;

		public Application(IModule[] modules)
		{
			this.modules = modules.OrderByPriority();
		}

		public IEnumerable<IModule> Modules
		{
			get { return modules; }
		}

		public void Initialize()
		{
			if (Modules != null)
			{
				foreach (IModule module in Modules)
					module.Initialize();
			}
		}
	}
}