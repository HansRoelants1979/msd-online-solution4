using System;

namespace Tc.Crm.Common.Services
{
    public interface IAllocationConfigurationService
    {
        int DepartureDateInNextXDays { get; }
        string DestinationGatewayIds { get; }
        Guid DefaultUserId { get; }
        string DefaultUserName { get; }
        string TeamRolesToAssignCase { get; }
        string UserRolesToAssignCase { get; }
        int ExecuteMultipleBatchSize { get; }
    }
}