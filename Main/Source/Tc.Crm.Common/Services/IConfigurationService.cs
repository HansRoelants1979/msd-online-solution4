using System;

namespace Tc.Crm.Common.Services
{
    public interface IConfigurationService
    {
        string ConnectionString { get; set; }
        int DepartureDateInNextXDays { get; }
        int ExecuteMultipleBatchSize { get; }
        string DestinationGatewayIds { get; set; }
        Guid DefaultUserId { get; }
        string DefaultUserName { get; }

        string TeamRolesToAssignCase { get; }

        string UserRolesToAssignCase { get; }
    }
}
