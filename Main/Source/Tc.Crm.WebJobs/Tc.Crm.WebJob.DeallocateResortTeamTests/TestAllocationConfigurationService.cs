using System;
using Tc.Crm.Common.Services;

namespace Tc.Crm.WebJob.DeallocateResortTeamTests
{
    public class TestAllocationConfigurationService : IAllocationConfigurationService
    {
        public Guid DefaultUserId => new Guid("35ECF2B8-90E4-E611-8105-3863BB34FA70");

        public string DefaultUserName => string.Empty;

        public int DepartureDateInNextXDays => 9;

        public string DestinationGatewayIds => "{37c4bfbe-29f8-e611-810b-1458d041f8e8}, {fb86aeb3-27f8-e611-810b-1458d041f8e8}";

        public string TeamRolesToAssignCase => "Tc.CustomerRelations.Base";

        public string UserRolesToAssignCase => "Tc.CustomerRelations.Agent";
        public int ExecuteMultipleBatchSize => 50;
    }
}
