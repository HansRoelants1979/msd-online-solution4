using System;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Common.Services;

namespace Tc.Crm.OutboundSynchronisation.Customer.Services
{
    public class OutboundSynchronisationDataService : IOutboundSynchronisationDataService
    {
        private readonly ICrmService crmService;
        private readonly ILogger logger;

        private bool disposed;

        public OutboundSynchronisationDataService(ILogger logger, ICrmService crmService)
        {
            this.logger = logger;
            this.crmService = crmService;
        }

        public EntityCollection RetrieveEntityCaches(string type, int numberOfElements)
        {
            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException(nameof(type), "Type parameter cannot be empty");

            var query =
                $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                     <entity name='tc_entitycache'>
                       <attribute name='tc_entitycacheid' />
                       <attribute name='tc_name' />
                       <attribute name='createdon' />
                       <attribute name='tc_data' />
                       <order attribute='tc_name' descending='false' />
                       <filter type='and'>
                         <filter type='and'>
                           <condition attribute='tc_type' operator='eq' value='{type}' />
                           <condition attribute='statuscode' operator='eq' value='1' />
                         </filter>
                       </filter>
                     </entity>
                   </fetch>";

            EntityCollection entityCacheCollection = crmService.RetrieveMultipleRecordsFetchXml(query, numberOfElements);
            return entityCacheCollection;
        }

        #region Displosable members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                DisposeObject(logger);
                DisposeObject(crmService);
            }

            disposed = true;
        }

        private void DisposeObject(object obj)
        {
            if (obj != null)
            {
                if (obj is IDisposable)
                    ((IDisposable)obj).Dispose();
                else
                    obj = null;
            }
        }

        #endregion
    }
}