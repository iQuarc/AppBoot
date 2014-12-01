using System;
using System.Collections.Generic;
using System.Linq;

namespace iQuarc.AppBoot
{
	public class ConventionRegistrationBehavior : IRegistrationBehavior, IContainer<ServiceBuilder>
	{
		private readonly IList<ServiceBuilder> builders = new List<ServiceBuilder>();
 
		public IEnumerable<ServiceInfo> GetServicesFrom(Type type)
		{
			IEnumerable<ServiceInfo> services = builders.SelectMany(x => x.GetServicesFrom(type));
			return services;
		}
		
		public ServiceBuilder ForType(Type type)
		{
			ServiceBuilder builder = CreateServiceBuilder(x => x == type);
			return builder;
		}

		public ServiceBuilder ForType<T>()
		{
			return this.ForType(typeof (T));
		}

		public ServiceBuilder ForTypesDerivedFrom(Type type)
		{
			ServiceBuilder builder = CreateServiceBuilder(x => type.IsAssignableFrom(x));
			return builder;
		}

		public ServiceBuilder ForTypesDerivedFrom<T>()
		{
			return ForTypesDerivedFrom(typeof (T));
		}

		public ServiceBuilder ForTypesMatching(Predicate<Type> typeFilter)
		{
			ServiceBuilder builder = CreateServiceBuilder(typeFilter);
			return builder;
		}

		void IContainer<ServiceBuilder>.Register(ServiceBuilder builder)
		{
			this.builders.Add(builder);
		}

		private ServiceBuilder CreateServiceBuilder(Predicate<Type> typeFilter)
		{
			return new ServiceBuilder(this, typeFilter);
		}
	}
}