using Microsoft.Practices.ServiceLocation;

namespace iQuarc.AppBoot
{
	public interface IHierarchicalServiceLocator : IServiceLocator
	{
		IHierarchicalServiceLocator CreateChildServiceLocator();
	}
}