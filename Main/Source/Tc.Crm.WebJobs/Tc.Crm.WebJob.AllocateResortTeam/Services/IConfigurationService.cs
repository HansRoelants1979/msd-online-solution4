using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.WebJob.AllocateResortTeam.Services
{
    public interface IConfigurationService
    {
        string ConnectionString { get; set; }
        int DepartureDateInNextXDays { get; }
        int ExecuteMultipleBatchSize { get; }
        string DestinationGatewayIds { get; set; }
    }
}
