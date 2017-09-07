using System;
using Tc.Crm.Common;
using Tc.Crm.Common.Models;

namespace Tc.Crm.OutboundSynchronisation.Customer.Services
{
    public interface IOutboundSynchronisationService : IDisposable
    {
        /// <summary>
        /// Execute customer outbound synchronisation
        /// </summary>
        void Run();

        void ProcessEntityCache();

        void UpdateEntityCacheStatus(Guid entityCacheId, Status status, EntityCacheStatusReason statusReason);

        Guid CreateEntityCacheMessage(EntityCacheMessage entityCacheMessageModel);

        void UpdateEntityCacheMessageStatus(Guid entityCacheMessageId, Status status, EntityCacheMessageStatusReason statusReason);
    }
}