using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Tc.Crm.WebJob.AllocateResortTeam.Models;

namespace Tc.Crm.WebJob.AllocateResortTeam.Services
{
    public interface IAllocationService : IDisposable
    {
        IList<BookingAllocationResponse> GetBookingAllocations(BookingAllocationRequest bookingAllocationRequest);
        IList<BookingAllocationResponse> PrepareBookingAllocation(EntityCollection bookingCollection);
        StringBuilder GetDestinationGateways(IList<Guid> destinationGateways);
    }
}
