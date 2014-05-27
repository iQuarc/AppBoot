using System;
using System.Collections.Generic;

namespace iQuarc.AppBoot
{
	/// <summary>
	///     Represents one of the behaviors used by the Bootstraper to register types into the Dependency Injection Container
	/// </summary>
	public interface IRegistrationBehavior
	{
		/// <summary>
		///     Gets the services information that will be registered for given type
		/// </summary>
		IEnumerable<ServiceInfo> GetServicesFrom(Type type);
	}
}