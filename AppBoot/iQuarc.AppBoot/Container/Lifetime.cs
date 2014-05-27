namespace iQuarc.AppBoot
{
	/// <summary>
	///     Specifies the lifetime of an instance of a service
	/// </summary>
	public enum Lifetime
	{
		/// <summary>
		///     New instances are created each time a new object graph is created.
		///     During the scope of build-up of one object graph the created instances are reused.
		/// </summary>
		Instance,

		/// <summary>
		///     Always creates a new instance of this class when it is injected as a dependency.
		/// </summary>
		AlwaysNew,

		/// <summary>
		///     Lives on the application as a singleton instance
		/// </summary>
		Application
	}
}