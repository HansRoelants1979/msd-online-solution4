namespace Tc.Crm.Common.IntegrationLayer.Service.Synchronisation
{
    public class CreateCustomerRequestPayloadCreator : EntityCachePayloadCreator
    {
        protected override IEntityCacheMapper Mapper { get; } = new CreateCustomerRequestMapper();
    }
}
