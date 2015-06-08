using System;
using System.Collections.Generic;
using System.Linq;
using iQuarc.SystemEx;
using System.ServiceModel.Activation;
using System.Web.Routing;

namespace iQuarc.AppBoot.WcfHosting
{
    public class WcfHostingRegistrationBehavior : IRegistrationBehavior
    {
        string baseAddress;
        public WcfHostingRegistrationBehavior(string baseAddress)
        {
            this.baseAddress = baseAddress;
        }

        public IEnumerable<ServiceInfo> GetServicesFrom(Type type)
        {
            IEnumerable<WcfServiceAttribute> attributes = type.GetAttributes<WcfServiceAttribute>(false);
            var services = attributes.Select(a =>
                new ServiceInfo(a.ContractType, type, Lifetime.AlwaysNew));
            foreach (var service in services)
            {
                RouteTable.Routes.Add(new ServiceRoute(baseAddress, new WebServiceHostFactory(), type));
            }
            return services;
        }
    }
}
