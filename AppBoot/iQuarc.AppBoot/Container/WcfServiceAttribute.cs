using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iQuarc.AppBoot
{
    public class WcfServiceAttribute : Attribute
    {
        public WcfServiceAttribute(Type contractType)
        {
            this.ContractType = contractType;
        }

        public Type ContractType { get; private set; }
    }
}
