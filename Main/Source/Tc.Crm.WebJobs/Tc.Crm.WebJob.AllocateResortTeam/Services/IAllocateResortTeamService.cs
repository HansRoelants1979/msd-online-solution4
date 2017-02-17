using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.WebJob.AllocateResortTeam.Models;

namespace Tc.Crm.WebJob.AllocateResortTeam.Services
{
    public interface IAllocateResortTeamService:IDisposable
    {
        void Run();
        IList<BookingAllocation> GetBookingAllocations();
        void AllocateBookingToResortTeam(IList<BookingAllocation> bookingAllocations);
    }
}
