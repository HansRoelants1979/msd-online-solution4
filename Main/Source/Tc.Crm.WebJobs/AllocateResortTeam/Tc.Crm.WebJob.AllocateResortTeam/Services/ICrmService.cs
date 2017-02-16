using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Tc.Crm.WebJob.AllocateResortTeam.Models;

namespace Tc.Crm.WebJob.AllocateResortTeam.Services
{
    public interface ICrmService : IDisposable
    {
        IList<BookingAllocation> GetBookingAllocations();
        void Update(BookingAllocation bookingAllocation);
        IOrganizationService GetOrganizationService();
    }
}
