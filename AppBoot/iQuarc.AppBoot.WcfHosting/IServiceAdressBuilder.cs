using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iQuarc.AppBoot.WcfHosting
{
    /// <summary>
    /// Represents an adress builder for hosting services
    /// </summary>
    interface IServiceAdressBuilder
    {
        /// <summary>
        /// Builds a dynamic service address based on the contract type
        /// </summary>
        /// <param name="contractType">the contract type of the service</param>
        /// <returns>URL string representing the service address</returns>
        string BuildAddress(Type contractType);
    }
}
