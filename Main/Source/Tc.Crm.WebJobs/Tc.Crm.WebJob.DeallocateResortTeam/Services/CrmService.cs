using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Tc.Crm.WebJob.DeallocateResortTeam.Models;
using Tc.Crm.WebJob.DeallocateResortTeam.Services;
using Microsoft.Xrm.Tooling.Connector;
using System.Configuration;

namespace Tc.Crm.WebJob.DeallocateResortTeam.Services
{
    public class CrmService : ICrmService
    {
        IOrganizationService organizationService;
        IConfigurationService configurationService;
        public CrmService(IConfigurationService configurationService)
        {
            this.configurationService = configurationService;
            this.organizationService = GetOrganizationService();
        }

        public IList<BookingDeallocation> GetBookingDeallocations()
        {
            throw new NotImplementedException();
        }

        public IOrganizationService GetOrganizationService()
        {
            if (organizationService != null) return organizationService;

            var connectionString = ConfigurationManager.ConnectionStrings["Crm"];
            CrmServiceClient client = new CrmServiceClient(connectionString.ConnectionString);
            return (IOrganizationService)client;
        }

        public void Update(BookingDeallocation bookingDeallocation)
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
