using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.WebJob.DeallocateResortTeam.Services
{
    public interface IConfigurationService
    {
        string ConnectionString { get; set; }
        string DestinationGatewayIds { get; set; }
        string DefaultUserId { get; set; }
    }
}
