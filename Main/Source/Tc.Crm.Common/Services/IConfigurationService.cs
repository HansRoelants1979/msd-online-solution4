using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
