using System;
using Tc.Crm.Common.Services;

namespace Tc.Crm.OutboundSynchronisation.Customer.Services
{
    public class OutboundSynchronisationService : IOutboundSynchronisationService
    {
        private bool disposed;
        private readonly IOutboundSynchronisationDataService outboundSynchronisationDataService;
        private readonly IConfigurationService configurationService;
        private readonly ILogger logger;

        public OutboundSynchronisationService(ILogger logger, IOutboundSynchronisationDataService outboundSynchronisationService, IConfigurationService configurationService)
        {
            this.outboundSynchronisationDataService = outboundSynchronisationService;
            this.logger = logger;
            this.configurationService = configurationService;
        }

        public void Run()
        {
            outboundSynchronisationDataService.RetrieveEntityCaches("contact", 10000);
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
                DisposeObject(outboundSynchronisationDataService);
                DisposeObject(logger);
                DisposeObject(configurationService);
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