using System.Configuration;

namespace Tc.Crm.Common.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public string ConnectionString
        {
            get
            {
                var connectionString = ConfigurationManager.ConnectionStrings["Crm"].ConnectionString;
                if (!string.IsNullOrEmpty(connectionString))
                    return ConfigurationManager.ConnectionStrings["Crm"].ConnectionString;
                throw new ConfigurationErrorsException("Crm connection string was not found in app.config");
            }
        }
    }
}