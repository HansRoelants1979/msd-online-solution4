namespace Tc.Crm.Common.IntegrationLayer.Service.Synchronisation
{
    public class UpdateCustomerRequestPayloadCreator : EntityCachePayloadCreator
    {
        protected override IEntityCacheMapper Mapper { get; } = new UpdateCustomerRequestMapper();
    }
}