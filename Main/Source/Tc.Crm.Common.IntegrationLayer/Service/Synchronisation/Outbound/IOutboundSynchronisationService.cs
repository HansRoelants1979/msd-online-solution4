using System;

namespace Tc.Crm.Common.IntegrationLayer.Service.Synchronisation.Outbound
{
    public interface IOutboundSynchronisationService : IDisposable
    {
        /// <summary>
        /// Execute customer outbound synchronisation
        /// </summary>
        void Run();
    }
}