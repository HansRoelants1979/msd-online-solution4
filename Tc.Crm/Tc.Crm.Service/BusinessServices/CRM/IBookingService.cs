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
        Task<bool> Update(Booking booking);
        Task<Guid> Create(Booking booking);
        Task<Booking> Upsert(Booking booking);
    }
}
