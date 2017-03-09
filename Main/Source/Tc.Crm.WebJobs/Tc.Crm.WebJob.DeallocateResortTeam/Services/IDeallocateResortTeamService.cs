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
        void GetBookingDeallocations();
        BookingDeallocationResortTeamRequest PrepareResortTeamRemovalRequest(BookingDeallocationResponse bookingDeallocationResponse);
        IList<BookingDeallocationResortTeamRequest> ProcessDeallocationResponse(IList<BookingDeallocationResponse> bookingDeallocationResponse);
        IList<Guid> GetDestinationGateways();
        void AddResortTeamRemovalRequest(BookingDeallocationResponse bookingResponse, IList<BookingDeallocationResortTeamRequest> bookingDeallocationResortTeamRequest);
        string WriteDeallocationResponseLog(BookingDeallocationResponse bookingDeallocationResponse);
        CustomerResortTeamRequest PrepareCustomerResortTeamRemovalRequest(BookingDeallocationResponse bookingResponse);
        BookingResortTeamRequest PrepareBookingResortTeamRemovalRequest(BookingDeallocationResponse bookingResponse);
        bool ValidForProcessing(BookingDeallocationResponse bookingResponse, List<Guid> processedCustomers, string responseLog);

    }
}
