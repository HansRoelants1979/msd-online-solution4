using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Tc.Crm.WebJob.DeallocateResortTeam.Models;

namespace Tc.Crm.WebJob.DeallocateResortTeam.Services
{
    public interface ICrmService : IDisposable
    {

        IList<BookingDeallocation> GetBookingDeallocations();
        void Update(BookingDeallocation bookingDeallocation);
        IOrganizationService GetOrganizationService();
    }
}
