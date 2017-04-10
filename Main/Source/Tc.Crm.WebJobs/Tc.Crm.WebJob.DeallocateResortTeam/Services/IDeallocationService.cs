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
    public interface IDeallocationService : IDisposable
    {
        /// <summary>
        /// Get bookings, customers and cases to be reassigned to the default team
        /// </summary>
        /// <param name="bookingDeallocationRequest"> deallocation request</param>
        /// <returns>bookings, customers and cases to be assigned to default team</returns>
        DeallocationExecutionRequest FetchBookingsForDeallocation(DeallocationRequest bookingDeallocationRequest);

        /// <summary>
        /// Assign bookings, customers and cases to default team
        /// </summary>
        /// <param name="request"></param>
        void DeallocateEntities(DeallocationExecutionRequest request);
    }
}
