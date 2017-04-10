using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.Common.Models
{
    public sealed class Booking : EntityModel
    {
        public override string EntityName
        {
            get
            {
                return Constants.EntityName.Booking;
            }
        }
    }
}
