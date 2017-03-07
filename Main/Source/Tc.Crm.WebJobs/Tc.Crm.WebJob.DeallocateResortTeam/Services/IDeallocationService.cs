using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.Common;
using Tc.Crm.Common.Models;
using Tc.Crm.WebJob.DeallocateResortTeam.Models;

namespace Tc.Crm.WebJob.DeallocateResortTeam.Services
{
    public interface IDeallocationService:IDisposable
    {
        IList<BookingDeallocationResponse> GetBookingDeallocations(BookingDeallocationRequest bookingDeallocationRequest);
        IList<BookingDeallocationResponse> PrepareBookingDeallocation(EntityCollection bookingCollection);
        StringBuilder GetDestinationGateways(IList<Guid> destinationGateways);
        void ProcessBookingDeallocations(IList<BookingDeallocationResortTeamRequest> bookingDeallocationResortTeamRequest);
        OwnerType GetOwnerType(EntityReference owner);
        Owner GetOwner(Entity entity, string attributeName, bool isAliasedValue);

    }
}
