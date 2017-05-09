using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.WebJob.AllocateResortTeam.Models
{
    public class ParentHotelTeam
    {
        public Guid TeamId { get; set; }
        public Guid BusinessUnitId { get; set; }
    }
}
