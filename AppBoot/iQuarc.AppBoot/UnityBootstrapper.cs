using System.Collections.Generic;
using System.Reflection;

namespace iQuarc.AppBoot
{
	public class UnityBootstrapper : Bootstrapper
	{
		public UnityBootstrapper(IEnumerable<Assembly> applicationAssemblies)
			: base(applicationAssemblies, new UnityContainerAdapter())
		{
		}
	}
}