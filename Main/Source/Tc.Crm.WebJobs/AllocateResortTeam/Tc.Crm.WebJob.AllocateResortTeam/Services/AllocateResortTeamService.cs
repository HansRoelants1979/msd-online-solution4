using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.WebJob.AllocateResortTeam.Models;

namespace Tc.Crm.WebJob.AllocateResortTeam.Services
{
    public class AllocateResortTeamService : IAllocateResortTeamService
    {
        ICrmService crmService;
        public AllocateResortTeamService(ICrmService crmService)
        {
            this.crmService = crmService;
        }
        public void AllocateBookingToResortTeam(IList<BookingAllocation> bookingAllocations)
        {
            
        }

        public IList<BookingAllocation> GetBookingAllocations()
        {
            return new List<BookingAllocation>();
        }

        public void Run()
        {
            
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                //dispose org service
                if (crmService != null) ((IDisposable)crmService).Dispose();
            }

            disposed = true;
        }
    }
}
