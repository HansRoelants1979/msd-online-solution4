using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.WebJob.DeallocateResortTeam.Models;

namespace Tc.Crm.WebJob.DeallocateResortTeam.Services
{
    class DeallocateResortTeamService : IDeallocateResortTeamService
    {
        ICrmService crmService;
        public DeallocateResortTeamService(ICrmService crmService)
        {
            this.crmService = crmService;
        }
        public void DeallocateBookingFromResortTeam(IList<BookingDeallocation> bookingAllocations)
        {

        }

        public IList<BookingDeallocation> GetBookingAllocations()
        {
            return new List<BookingDeallocation>();
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
                DisposeObject(crmService);
            }

            disposed = true;
        }

        void DisposeObject(Object obj)
        {
            if (obj != null)
            {
                if (obj is IDisposable)
                    ((IDisposable)obj).Dispose();
                else
                    obj = null;
            }

        }
    }
}
