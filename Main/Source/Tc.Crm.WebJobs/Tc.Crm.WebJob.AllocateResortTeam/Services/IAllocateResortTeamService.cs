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
        void GetBookingAllocations();
        void AllocateBookingToResortTeam(IList<BookingAllocationResortTeamRequest> bookingAllocationResortTeamRequest);
        IList<BookingAllocationResortTeamRequest> PrepareResortTeamRequest(IList<BookingAllocationResponse> bookingAllocationResponse);
    }
}
