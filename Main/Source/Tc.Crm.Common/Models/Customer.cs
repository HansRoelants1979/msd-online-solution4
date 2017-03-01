using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.Common.Models
{
    public class Customer
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public CustomerType CustomerType { get; set; }
        public Owner Owner { get; set; }
    }
}
