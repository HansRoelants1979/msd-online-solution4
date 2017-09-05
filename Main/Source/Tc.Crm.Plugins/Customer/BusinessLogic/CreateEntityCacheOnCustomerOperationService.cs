using Microsoft.Xrm.Sdk;
using Tc.Crm.Plugins.MultipleEntities.BusinessLogic;

namespace Tc.Crm.Plugins.Customer.BusinessLogic
{
    public class CreateEntityCacheOnCustomerOperationService : CreateEntityCacheOnEntityOperationService
    {
       
        /// <summary>
        /// To set mapping attributes of entity cache from customer entity
        /// </summary>
        /// <param name="sourceEntity"></param>
        /// <param name="targetEntity"></param>
        public override void SetEntityParameters(Entity sourceEntity, Entity targetEntity)
        {
            if (sourceEntity.Attributes.Contains(Attributes.Customer.FullName) && sourceEntity.Attributes[Attributes.Customer.FullName] != null)
            {
                targetEntity.Attributes[Attributes.EntityCache.Name] = sourceEntity.Attributes[Attributes.Customer.FullName];
            }
            if (sourceEntity.Attributes.Contains(Attributes.Customer.SourceMarketId) && sourceEntity.Attributes[Attributes.Customer.SourceMarketId] != null)
            {
                var iso2Code = GetSourceMarketISO2Code(((EntityReference)sourceEntity.Attributes[Attributes.Customer.SourceMarketId]).Id);
                targetEntity.Attributes[Attributes.EntityCache.SourceMarket] = iso2Code;
            }
        }
    }
}
