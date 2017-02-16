using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Tc.Crm.WebJob.AllocateResortTeam.Models;
using Tc.Crm.WebJob.AllocateResortTeam.Services;

namespace Tc.Crm.WebJob.AllocateResortTeam
{
    public class CrmService : ICrmService
    {
        IOrganizationService organizationService;
        IConfigurationService configurationService;
        public CrmService(IConfigurationService configurationService)
        {
            this.configurationService = configurationService;

        }

        public IList<BookingAllocation> GetBookingAllocations()
        {
            throw new NotImplementedException();
        }

        public IOrganizationService GetOrganizationService()
        {
            throw new NotImplementedException();
        }

        public void Update(BookingAllocation bookingAllocation)
        {
            throw new NotImplementedException();
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
                if (organizationService != null) ((IDisposable)organizationService).Dispose();
                if (configurationService != null) configurationService = null;
            }

            disposed = true;
        }
    }
}
