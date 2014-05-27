using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace iQuarc.AppBoot
{
	/// <summary>
	///     A catalog of service registrations which keeps the registrations based on contracts and priorities
	///     Lower priority registrations for same contract are overwritten by higher priority registrations
	/// </summary>
	internal class RegistrationsCatalog : IEnumerable<ServiceInfo>
	{
		private readonly List<Registration> registrations = new List<Registration>();

		public void Add(ServiceInfo serviceInfo, int priority)
		{
			Registration newRegistration = new Registration(serviceInfo, priority);

			if (string.IsNullOrEmpty(newRegistration.Service.ContractName))
				AddByType(newRegistration);
			else
				AddByContract(newRegistration);
		}

		private void AddByType(Registration newRegistration)
		{
			int index = registrations.FindIndex(r => string.IsNullOrEmpty(r.Service.ContractName) && r.Service.From == newRegistration.Service.From);

			if (index == -1)
				registrations.Add(newRegistration);
			else if (registrations[index].Priority < newRegistration.Priority)
				registrations[index] = newRegistration;
		}

		private void AddByContract(Registration newRegistration)
		{
			int index = registrations.FindIndex(r => r.Service.ContractName == newRegistration.Service.ContractName);

			if (index == -1)
				registrations.Add(newRegistration);
			else if (registrations[index].Priority < newRegistration.Priority)
				registrations[index] = newRegistration;
		}

		public IEnumerator<ServiceInfo> GetEnumerator()
		{
			return registrations.Select(r => r.Service).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}


		private class Registration
		{
			public Registration(ServiceInfo service, int priority)
			{
				Service = service;
				Priority = priority;
			}

			public int Priority { get; private set; }

			public ServiceInfo Service { get; private set; }
		}
	}
}