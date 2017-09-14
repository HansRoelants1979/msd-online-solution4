using System;
using Tc.Crm.Common.Services;

namespace Tc.Crm.OutboundSynchronisation.CustomerTests
{
    public class TestConfigurationService : IConfigurationService
    {
        public string ConnectionString { get; set; }
        public int DepartureDateInNextXDays { get; }
        public int ExecuteMultipleBatchSize { get; }
        public string DestinationGatewayIds { get; set; }
        public Guid DefaultUserId { get; }
        public string DefaultUserName { get; }
        public string TeamRolesToAssignCase { get; }
        public string UserRolesToAssignCase { get; }
        public string OutboundSyncEntityName { get; }
        public int OutboundSyncBatchSize { get; }
    }
}