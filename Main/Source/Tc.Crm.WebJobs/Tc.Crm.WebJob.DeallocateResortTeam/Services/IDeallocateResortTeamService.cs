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
        void GetBookingAllocations();
        BookingDeallocationResortTeamRequest PrepareResortTeamRequest(BookingDeallocationResponse bookingDeallocationResponse);
        IList<BookingDeallocationResortTeamRequest> ProcessDeallocationResponse(IList<BookingDeallocationResponse> bookingDeallocationResponse);
        IList<Guid> GetDestinationGateways();
        void AddResortTeamRequest(BookingDeallocationResponse bookingResponse, IList<BookingDeallocationResortTeamRequest> bookingDeallocationResortTeamRequest);
    }
}
