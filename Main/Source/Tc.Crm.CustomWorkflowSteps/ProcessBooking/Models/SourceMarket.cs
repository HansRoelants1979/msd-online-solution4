using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models
{
    public class SourceMarket
    {
        public Guid BusinessUnitId { get; set; }
        public Guid SourceMarketId { get; set; }

        public Guid TeamId { get; set; }
    }
}
