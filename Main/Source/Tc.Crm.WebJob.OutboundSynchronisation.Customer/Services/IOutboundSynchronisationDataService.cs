using System;
using Microsoft.Xrm.Sdk;

namespace Tc.Crm.OutboundSynchronisation.Customer.Services
{
    public interface IOutboundSynchronisationDataService : IDisposable
    {
        /// <summary>
        /// Retrieve active antity cache entities for defined type
        /// </summary>
        /// <param name="type">Type of entity</param>
        /// <param name="numberOfElements">Number of retrieved entities</param>
        /// <returns>active antity cache entities for defined type</returns>
        EntityCollection RetrieveEntityCaches(string type, int numberOfElements);
    }
}