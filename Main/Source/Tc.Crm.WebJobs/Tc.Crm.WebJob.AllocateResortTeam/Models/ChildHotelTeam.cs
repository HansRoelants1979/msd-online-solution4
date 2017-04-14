using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.WebJob.AllocateResortTeam.Models
{
    public class ChildHotelTeam
    {
        public Guid ChildTeamId { get; set; }
        public string ChildTeamName { get; set; }
        public Guid ParentTeamId { get; set; }
        public Guid BusinessUnitId { get; set; }
    }
}
