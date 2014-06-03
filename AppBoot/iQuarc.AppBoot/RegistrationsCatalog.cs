using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace iQuarc.AppBoot
{
	/// <summary>
	///     A catalog of service registrations which keeps the registrations based on contracts and priorities
	///     Lower priority registrations for same contract are overwritten by higher priority registrations.
    /// <para>
    ///    Registration catalog will contain:
    ///         1. For one FromType it has registrations given by ONLY one behavior, and that behavior is the one with the highest priority
    ///         2. Newly added registrations with same FromType, same ContractKey and are ignored
    /// </para>
	/// </summary>
	internal class RegistrationsCatalog : IEnumerable<ServiceInfo>
	{
		private readonly LinkedList<Registration> registrations = new LinkedList<Registration>();

		public void Add(ServiceInfo serviceInfo, int priority)
		{
			Registration newRegistration = new Registration(serviceInfo, priority);
            Add(newRegistration);
		}

	    private void Add(Registration newReg)
	    {
	        bool isNewFrom = true;
	        bool add = true;

	        LinkedListNode<Registration> current = registrations.First;
	        while (current != null)
	        {
	            Registration reg = current.Value;
	            LinkedListNode<Registration> next = current.Next;

	            if (reg.Service.From == newReg.Service.From)
	            {
	                isNewFrom = false;

	                if (reg.Priority < newReg.Priority)
	                    registrations.Remove(current);
	                else if (   reg.Priority > newReg.Priority ||
                                reg.Service.ContractName == newReg.Service.ContractName)
	                {
	                    add = false;
	                    break;
	                }
	            }

	            current = next;
	        }

	        if (isNewFrom || add)
	            registrations.AddLast(newReg);
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