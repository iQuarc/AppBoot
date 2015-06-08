using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iQuarc.AppBoot.Container
{
    public class WcfServiceAttribute : Attribute
    {
        public WcfServiceAttribute(Type serviceType)
        {
            this.ServiceType = serviceType;
        }

        public Type ServiceType { get; private set; }
    }
}
