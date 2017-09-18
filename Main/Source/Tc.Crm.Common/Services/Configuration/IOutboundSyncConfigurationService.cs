namespace Tc.Crm.Common.Services
{
    public interface IOutboundSyncConfigurationService
    {
        string EntityName { get; }
        int BatchSize { get; }
        string CreateServiceUrl { get; }
        string UpdateServiceUrl { get; }
    }
}