using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.Common.Services;

namespace Tc.Crm.WebJob.DeallocateResortTeamTests
{
    public class TestConfigurationService : IConfigurationService
    {
        public string ConnectionString
        {
            get
            {
                return "my connection string";
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Guid DefaultUserId
        {
            get
            {
                return new Guid("35ECF2B8-90E4-E611-8105-3863BB34FA70");
            }
        }

        public string DefaultUserName
        {
            get
            {
                return string.Empty;
            }
        }

        public int DepartureDateInNextXDays
        {
            get
            {
                return 9;
            }
        }

        public string DestinationGatewayIds
        {
            get
            {
                return "{37c4bfbe-29f8-e611-810b-1458d041f8e8},{fb86aeb3-27f8-e611-810b-1458d041f8e8},{999beff7-27f8-e611-810b-1458d041f8e8},{70fc2806-2af8-e611-810b-1458d041f8e8},{74ffd12f-28f8-e611-810b-1458d041f8e8},{c7a99285-2af8-e611-810b-1458d041f8e8},{ff1b2b35-d7ee-e611-8106-3863bb34fb48},{63e1f2a5-28f8-e611-810b-1458d041f8e8},{de0ba066-28f8-e611-810b-1458d041f8e8},{7049bbf5-28f8-e611-810b-1458d041f8e8}";
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int ExecuteMultipleBatchSize
        {
            get
            {
                return 50;
            }
        }
    }
}
