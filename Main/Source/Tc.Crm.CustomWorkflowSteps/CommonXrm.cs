using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Query;

namespace Tc.Crm.CustomWorkflowSteps
{
    public class CommonXrm
    {


        /// <summary>
        /// Call this method to create or update record
        /// </summary>
        /// <param name="entityRecord"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public static XrmResponse UpsertEntity(Entity entityRecord, IOrganizationService service)
        {  

            XrmResponse xrmResponse = null;
            if (service != null)
            {
                UpsertRequest request = new UpsertRequest()
                {
                    Target = entityRecord
                };



                // Execute UpsertRequest and obtain UpsertResponse. 
                UpsertResponse response = (UpsertResponse)service.Execute(request);
                if (response.RecordCreated)
                    xrmResponse = new XrmResponse
                    {
                        Id = response.Target.Id.ToString(),
                        EntityName = entityRecord.LogicalName,
                        Create = true

                    };
                else
                    xrmResponse = new XrmResponse()
                    {
                        Id = response.Target.Id.ToString(),
                        EntityName = entityRecord.LogicalName,
                        Create = false
                    };


            }

           
            return xrmResponse;
        }


        /// <summary>
        /// Call this method for bulk create
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public static List<XrmResponse> BulkCreate(EntityCollection entities, IOrganizationService service)
        {
            List<XrmResponse> xrmResponseList = new List<XrmResponse>();
            if (service != null && entities != null && entities.Entities.Count > 0)
            {

                // Add a CreateRequest for each entity to the request collection.
                foreach (var entity in entities.Entities)
                {
                    var createRequest = new CreateRequest { Target = entity };
                    var response =  (CreateResponse)service.Execute(createRequest);
                    var xrmResponse = new XrmResponse
                    {
                        Id = response.id.ToString(),
                        EntityName = response.ResponseName

                    };
                    xrmResponseList.Add(xrmResponse);
                }

            }

            return xrmResponseList;
        }


        /// <summary>
        /// Call this method for bulk delete
        /// </summary>
        /// <param name="entityReferences"></param>
        /// <param name="service"></param>
        public static void BulkDelete(DataCollection<EntityReference> entityReferences, IOrganizationService service)
        {   
            if (service != null && entityReferences != null && entityReferences.Count > 0)
            {

                // Add a DeleteRequest for each entity to the request collection.
                foreach (var entityRef in entityReferences)
                {
                    var deleteRequest = new DeleteRequest { Target = entityRef };
                    service.Execute(deleteRequest);
                }   
               
            }
        }
               
        /// <summary>
        /// To get records by using filter keys and values
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="columns"></param>
        /// <param name="filterKeys"></param>
        /// <param name="filterValues"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public static EntityCollection RetrieveMultipleRecords(string entityName, string[] columns, string[] filterKeys, string[] filterValues, IOrganizationService service)
        {
            var query = new QueryExpression(entityName);
            query.ColumnSet = new ColumnSet(columns);
            
            for (int i = 0; i < filterKeys.Length; i++)
            {
                var condExpr = new ConditionExpression();
                condExpr.AttributeName = filterKeys[i];
                condExpr.Operator = ConditionOperator.Equal;
                condExpr.Values.Add(filterValues[i]);

                var fltrExpr = new FilterExpression(LogicalOperator.And);
                fltrExpr.AddCondition(condExpr);

                query.Criteria.AddFilter(fltrExpr);
            }
           return GetRecordsUsingQuery(query, service);

        }

        /// <summary>
        /// To get records using QueryExpression
        /// </summary>
        /// <param name="queryExpr"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        static EntityCollection GetRecordsUsingQuery(QueryExpression queryExpr, IOrganizationService service)
        {
            EntityCollection entityCollection = null;
            entityCollection = service.RetrieveMultiple(queryExpr);
            return entityCollection;
        }

    }

    public class XrmResponse
    {
        public bool Create { get; set; }
        public string EntityName { get; set; }
        public string Id { get; set; }
        public string Details { get; set; }
        public string Key { get; set; }

    }

}
