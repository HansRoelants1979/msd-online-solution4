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
        string DepartureDateinNextXDays { get; set; }
        string DestinationGatewayIds { get; set; }
    }
}
