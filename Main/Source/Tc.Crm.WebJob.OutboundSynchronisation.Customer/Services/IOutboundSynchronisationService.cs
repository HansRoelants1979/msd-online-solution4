using System;

namespace Tc.Crm.OutboundSynchronisation.Customer.Services
{
    public interface IOutboundSynchronisationService : IDisposable
    {
        /// <summary>
        /// Execute customer outbound synchronisation
        /// </summary>
        void Run();
    }
}