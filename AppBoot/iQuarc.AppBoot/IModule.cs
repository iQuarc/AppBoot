namespace iQuarc.AppBoot
{
	/// <summary>
	///     Represents a logical module of the application.
	///		At application startup all the implementation of this, which are registered into the DIC are initialized
	/// </summary>
	public interface IModule
	{
		void Initialize();
	}
}