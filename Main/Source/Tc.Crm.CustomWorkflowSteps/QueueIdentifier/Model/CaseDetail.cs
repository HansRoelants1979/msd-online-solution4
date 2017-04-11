using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.CustomWorkflowSteps.QueueIdentifier.Model
{
    public class CaseDetail
    {
        public Guid CaseSourceMarket { get; set; }
        public Guid BookingSourceMarket { get; set; }

        public Guid Owner { get; set; }
    }
}
