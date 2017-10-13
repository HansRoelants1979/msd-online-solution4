using System;
using Microsoft.Xrm.Sdk;

namespace Tc.Crm.Plugins.Merge.BusinessLogic
{
    public class EntityMergeFactory : IEntityMergeFactory
    {
        public EntityMerge GetEntityMerge(EntityCacheType type, IOrganizationService service)
        {
            switch (type)
            {
                case EntityCacheType.Customer:
                    return new EntityMergeCustomer(service);
            }
            throw new NotImplementedException();
        }
    }
}