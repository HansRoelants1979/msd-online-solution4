using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Tc.Crm.WebJob.AllocateResortTeam.Models;
using Tc.Crm.WebJob.AllocateResortTeam.Services;
using Microsoft.Xrm.Tooling.Connector;
using System.Configuration;

namespace Tc.Crm.WebJob.AllocateResortTeam
{
    public class CrmService : ICrmService
    {
        IOrganizationService organizationService;
        IConfigurationService configurationService;
        ILogger logger;
        public CrmService(IConfigurationService configurationService,ILogger logger)
        {
            this.configurationService = configurationService;
            this.organizationService = GetOrganizationService();
            this.logger = logger;
        }

        public IList<BookingAllocation> GetBookingAllocations()
        {
            throw new NotImplementedException();
        }

        public IOrganizationService GetOrganizationService()
        {
            if (organizationService != null) return organizationService;

            var connectionString = configurationService.ConnectionString;
            CrmServiceClient client = new CrmServiceClient(connectionString);
            return (IOrganizationService)client;
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
                DisposeObject(organizationService);
                DisposeObject(configurationService);
                DisposeObject(logger);
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
