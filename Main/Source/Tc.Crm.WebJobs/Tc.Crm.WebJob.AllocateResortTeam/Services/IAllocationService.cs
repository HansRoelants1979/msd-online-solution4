using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;
using Tc.Crm.WebJob.AllocateResortTeam.Models;
using Tc.Crm.Common;
using Tc.Crm.Common.Models;

namespace Tc.Crm.WebJob.AllocateResortTeam.Services
{
    public interface IAllocationService : IDisposable
    {
        IList<BookingAllocationResponse> GetBookingAllocations(BookingAllocationRequest bookingAllocationRequest);
        IList<BookingAllocationResponse> PrepareBookingAllocation(EntityCollection bookingCollection, List<ChildHotelTeam> childTeam);
        StringBuilder GetDestinationGateways(IList<Guid> destinationGateways);
        void ProcessBookingAllocations(IList<BookingAllocationResortTeamRequest> bookingAllocationResortTeamRequest);
        OwnerType GetOwnerType(EntityReference owner);
        Owner GetOwner(Entity entity, string attribute, bool isAliasedValue);
        List<ParentHotelTeam> GetHotelTeams(EntityCollection bookingCollection);
        EntityCollection GetChildTeams(List<ParentHotelTeam> parentHotelTeams);
        string GetLookupConditions(IList<Guid> lookups);
        List<ChildHotelTeam> PrepareChildTeam(EntityCollection childTeams);
        string GetParentTeamCondition(List<ParentHotelTeam> parentHotelTeams);
        string GetBusinessUnitField(Entity booking);
    }
}
