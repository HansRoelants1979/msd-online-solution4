using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Tc.Crm.Service.Services;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk.Query;

namespace Tc.Crm.ServiceTests
{
    public class TestCrmService : ICrmService
    {
        XrmFakedContext context;

        public TestCrmService(XrmFakedContext context)
        {
            this.context = context;
        }
        public Tc.Crm.Service.Models.UpsertResponse Upsert(Entity entity)
        {
            var orgService = context.GetFakedOrganizationService();
            QueryExpression query = new QueryExpression("contact");
            query.ColumnSet = new ColumnSet(true);
            query.Criteria = new FilterExpression(LogicalOperator.And);
            query.Criteria.AddCondition("new_sourcekey", ConditionOperator.Equal, entity["new_sourcekey"].ToString());

            var response = orgService.RetrieveMultiple(query);

            var upsertResponse = new Tc.Crm.Service.Models.UpsertResponse();
            if (response.Entities.Count > 0)
            {
                entity.Id = response.Entities[0].Id;
                orgService.Update(entity);
                upsertResponse.RecordCreated = false;
            }
            else
            {
                orgService.Create(entity);
                upsertResponse.RecordCreated = true;
            }
            upsertResponse.Target = new EntityReference(entity.LogicalName,entity.Id);

            return upsertResponse;
        }
    }
}
