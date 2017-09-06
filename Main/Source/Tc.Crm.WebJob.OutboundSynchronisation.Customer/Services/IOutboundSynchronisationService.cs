using System;
using Tc.Crm.OutboundSynchronisation.Customer.Model;

namespace Tc.Crm.OutboundSynchronisation.Customer.Services
{
    public interface IOutboundSynchronisationService : IDisposable
    {
        /// <summary>
        /// Execute customer outbound synchronisation
        /// </summary>
        void Run();

        void ProcessEntityCache();

        void UpdateEntityCacheStatus(int StateCode, int StatusReason);

        void CreateEntityCacheMessage(EntityCacheMessageModel entityCacheMessageModel);

        void UpdateEntityCacheMessageStatus(int StateCode, int StatusReason);
    }
}