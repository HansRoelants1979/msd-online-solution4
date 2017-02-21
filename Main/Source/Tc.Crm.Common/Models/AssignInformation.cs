using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.Common.Models
{
    public class AssignInformation
    {
        public Guid RecordId { get; set; }

        public string EntityName { get; set; }

        public Owner RecordOwner { get; set; }
    }
}
