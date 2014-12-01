namespace iQuarc.AppBoot
{
	public interface IContainer<in T>
	{
		void Register(T item);
	}
}