using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Tc.Crm.CustomWorkflowSteps.Attributes;

namespace Tc.Crm.CustomWorkflowSteps.GetUsersStore.Service
{
    public class GetUserStoreService
    {
        public Entity GetExternalLogin(EntityReference user, IOrganizationService service, ITracingService trace)
        {
            var query = new QueryExpression
            {
                EntityName = EntityName.ExternalLogin,
                ColumnSet = new ColumnSet(ExternalLogins.Initials, ExternalLogins.StoreId)
            };
           
            var ownerCondition = new ConditionExpression
            {
                AttributeName = ExternalLogins.OwnerId,
                Operator = ConditionOperator.Equal
            };
            ownerCondition.Values.Add(user.Id);
            query.Criteria.AddCondition(ownerCondition);
            trace.Trace("call retrievemultiple on extrernal logins");
            var entityCollection = service.RetrieveMultiple(query);
            return entityCollection.Entities.FirstOrDefault();
        }
    }
}
