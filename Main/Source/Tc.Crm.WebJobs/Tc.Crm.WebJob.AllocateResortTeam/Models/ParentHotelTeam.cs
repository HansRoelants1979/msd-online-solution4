using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.WebJob.AllocateResortTeam.Models
{
    public class ParentHotelTeam
    {

        public IList<Guid> Team { get; set; }
        public IList<Guid> BusinessUnit { get; set; }
    }
}
