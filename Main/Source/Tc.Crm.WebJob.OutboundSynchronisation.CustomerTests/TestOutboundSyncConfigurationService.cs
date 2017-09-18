using Tc.Crm.Common.Services;

namespace Tc.Crm.OutboundSynchronisation.CustomerTests
{
    public class TestOutboundSyncConfigurationService : IOutboundSyncConfigurationService
    {
        public string EntityName => "contact";
        public int BatchSize => 1000;
        public string CreateServiceUrl => "http://localhost:8080/int/GetCacheEntity";
        public string UpdateServiceUrl => "http://localhost:8080/int/GetCacheEntity/1";
    }
}