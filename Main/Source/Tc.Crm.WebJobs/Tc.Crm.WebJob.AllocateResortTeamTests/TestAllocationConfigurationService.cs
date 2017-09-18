using System;
using Tc.Crm.Common.Services;

namespace Tc.Crm.WebJob.AllocateResortTeamTests
{
    public class TestAllocationConfigurationService : IAllocationConfigurationService
    {
        public Guid DefaultUserId => Guid.Empty;

        public string DefaultUserName => string.Empty;

        public int DepartureDateInNextXDays => 14;

        public string DestinationGatewayIds => "{37c4bfbe-29f8-e611-810b-1458d041f8e8},{fb86aeb3-27f8-e611-810b-1458d041f8e8},{999beff7-27f8-e611-810b-1458d041f8e8},{70fc2806-2af8-e611-810b-1458d041f8e8},{74ffd12f-28f8-e611-810b-1458d041f8e8},{c7a99285-2af8-e611-810b-1458d041f8e8},{ff1b2b35-d7ee-e611-8106-3863bb34fb48},{63e1f2a5-28f8-e611-810b-1458d041f8e8},{de0ba066-28f8-e611-810b-1458d041f8e8},{7049bbf5-28f8-e611-810b-1458d041f8e8}";

        public string TeamRolesToAssignCase
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string UserRolesToAssignCase
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int ExecuteMultipleBatchSize => 50;
    }
}
