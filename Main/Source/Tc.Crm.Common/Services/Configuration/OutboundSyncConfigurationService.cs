using System.Configuration;

namespace Tc.Crm.Common.Services
{
    public class OutboundSyncConfigurationService : ConfigurationService, IOutboundSyncConfigurationService
    {
        public string EntityName
        {
            get
            {
                var entityName = ConfigurationManager.AppSettings["entityName"];
                if (!string.IsNullOrEmpty(entityName))
                {
                    return entityName;
                }
                throw new ConfigurationErrorsException("OutboundSynchronisation entityName was not found in app.config");
            }
        }

        public int BatchSize
        {
            get
            {
                var batchSizeString = ConfigurationManager.AppSettings["batchSize"];
                if (string.IsNullOrEmpty(batchSizeString))
                {
                    throw new ConfigurationErrorsException("OutboundSynchronisation batchSize was not found in app.config");
                }
                int batchSize;
                if (int.TryParse(batchSizeString, out batchSize))
                {
                    return batchSize;
                }
                throw new ConfigurationErrorsException("OutboundSynchronisation batchSize is not a correct number");
            }
        }

        public string CreateServiceUrl
        {
            get
            {
                var entityName = ConfigurationManager.AppSettings["createServiceUrl"];
                if (!string.IsNullOrEmpty(entityName))
                {
                    return entityName;
                }
                throw new ConfigurationErrorsException("OutboundSynchronisation createServiceUrl was not found in app.config");
            }
        }

        public string UpdateServiceUrl
        {
            get
            {
                var entityName = ConfigurationManager.AppSettings["updateServiceUrl"];
                if (!string.IsNullOrEmpty(entityName))
                {
                    return entityName;
                }
                throw new ConfigurationErrorsException("OutboundSynchronisation updateServiceUrl was not found in app.config");
            }
        }
    }
}