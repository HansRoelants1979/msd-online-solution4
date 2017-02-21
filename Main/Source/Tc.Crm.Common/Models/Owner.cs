using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.Common.Models
{
    public class Owner
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public OwnerType OwnerType { get; set; }
    }
}
