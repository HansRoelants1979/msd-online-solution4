using System;
using System.Configuration;

namespace Tc.Crm.Common.Services
{
    public class AllocationConfigurationService : ConfigurationService, IAllocationConfigurationService
    {
        public Guid DefaultUserId
        {
            get
            {
                Guid defaultUserId;
                var defaultUserIdString = ConfigurationManager.AppSettings["DeallocationOwnerId"];
                if (Guid.TryParse(defaultUserIdString, out defaultUserId))
                    return defaultUserId;
                throw new InvalidOperationException("Default user id provided in configuration is not valid.");
            }
        }

        public string DefaultUserName => ConfigurationManager.AppSettings["DeallocationOwnerName"];

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DepartureDateinNextXDays")]
        public int DepartureDateInNextXDays
        {
            get
            {
                var daysInString = ConfigurationManager.AppSettings["DepartureDateinNextXDays"];
                int days;
                if (int.TryParse(daysInString, out days))
                {
                    return days;
                }
                throw new FormatException("DepartureDateinNextXDays has not been provided in config or has been provided in an incorrect format.");
            }
        }

        public string DestinationGatewayIds
        {
            get
            {
                var destinationGatewayIds = ConfigurationManager.AppSettings["DestinationGatewayIds"];
                if (!string.IsNullOrEmpty(destinationGatewayIds))
                    return destinationGatewayIds;
                throw new ConfigurationErrorsException("Allocation DestinationGatewayIds was not found in app.config");
            }
        }

        public string UserRolesToAssignCase
        {
            get
            {
                var userRolesToAssignCase = ConfigurationManager.AppSettings["UserRolesToAssignCase"];
                if (!string.IsNullOrEmpty(userRolesToAssignCase))
                    return userRolesToAssignCase;
                throw new ConfigurationErrorsException("Allocation UserRolesToAssignCase was not found in app.config");
            }
        }

        public string TeamRolesToAssignCase
        {
            get
            {
                var teamRolesToAssignCase = ConfigurationManager.AppSettings["TeamRolesToAssignCase"];
                if (!string.IsNullOrEmpty(teamRolesToAssignCase))
                    return teamRolesToAssignCase;
                throw new ConfigurationErrorsException("Allocation TeamRolesToAssignCase was not found in app.config");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public int ExecuteMultipleBatchSize
        {
            get
            {
                var batchInString = ConfigurationManager.AppSettings["ExecuteMultipleBatchSize"];
                int batch;
                if (int.TryParse(batchInString, out batch))
                {
                    return batch;
                }
                throw new FormatException("Batch has not been provided in config or has been provided in an incorrect format.");
            }
        }
    }
}