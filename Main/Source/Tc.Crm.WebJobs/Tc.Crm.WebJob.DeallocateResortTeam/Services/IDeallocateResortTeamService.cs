using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.WebJob.DeallocateResortTeam.Models;

namespace Tc.Crm.WebJob.DeallocateResortTeam.Services
{
    interface IDeallocateResortTeamService : IDisposable
    {
        void Run();
        IList<BookingDeallocation> GetBookingAllocations();
        void DeallocateBookingFromResortTeam(IList<BookingDeallocation> bookingAllocations);
    }
}
