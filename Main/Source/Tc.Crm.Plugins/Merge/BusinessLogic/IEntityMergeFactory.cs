using Microsoft.Xrm.Sdk;

namespace Tc.Crm.Plugins.Merge.BusinessLogic
{
    public interface IEntityMergeFactory
    {
        EntityMerge GetEntityMerge(EntityCacheType type, IOrganizationService service);
    }
}