using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.BusinessServices.CRM
{
    interface IBookingService
    {

        Booking GetBookingBy(int id);
        void Update(Booking booking);
        Booking Create(Booking booking);
        Booking Upsert(Booking booking);
    }
}
