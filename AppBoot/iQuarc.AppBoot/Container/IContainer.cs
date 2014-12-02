using System.Collections.Generic;

namespace iQuarc.AppBoot
{
	public interface IContainer<T> : IEnumerable<T>
	{
		void Register(T item);
	}
}