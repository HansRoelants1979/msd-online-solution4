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
        ILogger logger;
        
        public AllocateResortTeamService(ICrmService crmService,ILogger logger)
        {
            this.crmService = crmService;
            this.logger = logger;
        }
        public void AllocateBookingToResortTeam(IList<BookingAllocation> bookingAllocations)
        {
            
        }

        public IList<BookingAllocation> GetBookingAllocations()
        {
           return crmService.GetBookingAllocations(new BookingAllocationRequest { DepartureDateinNextXDays=9,DepartureDate=DateTime.Now,ReturnDate=DateTime.Now,Destination= new List<Guid> { Guid.Empty } });
        }

        public void Run()
        {
            GetBookingAllocations();
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
