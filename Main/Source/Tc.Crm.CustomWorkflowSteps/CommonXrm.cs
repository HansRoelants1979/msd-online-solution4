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
        /// <param name="entityRecord">Entity to Create or Update</param>
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
        /// <returns></returns>
        public static List<XrmResponse> BulkCreate(EntityCollection entities, IOrganizationService service)
        {
            List<XrmResponse> listXrmResponse = null;
            if (service != null && entities != null && entities.Entities.Count > 0)
            {

                // Add a CreateRequest for each entity to the request collection.
                foreach (var entity in entities.Entities)
                {
                    CreateRequest createRequest = new CreateRequest { Target = entity };
                    CreateResponse response =  (CreateResponse)service.Execute(createRequest);
                    XrmResponse xrmResponse = new XrmResponse
                    {
                        Id = response.id.ToString(),
                        EntityName = response.ResponseName

                    };
                    listXrmResponse.Add(xrmResponse);
                }

            }

            return listXrmResponse;
        }


        /// <summary>
        /// Call this method for bulk delete
        /// </summary>      
        /// <param name="entityReferences">Collection of EntityReferences to Delete</param>
        public static void BulkDelete(DataCollection<EntityReference> entityReferences, IOrganizationService service)
        {   
            if (service != null && entityReferences != null && entityReferences.Count > 0)
            {

                // Add a DeleteRequest for each entity to the request collection.
                foreach (var entityRef in entityReferences)
                {
                    DeleteRequest deleteRequest = new DeleteRequest { Target = entityRef };
                    service.Execute(deleteRequest);
                }   
               
            }
        }
               

        public static EntityCollection RetrieveMultipleRecords(string entityName, string[] columns, string[] filterKeys, string[] filterValues, IOrganizationService service)
        {
            var query = new QueryExpression(entityName);
            query.ColumnSet = new ColumnSet(columns);
            FilterExpression fltrExpr = null;
            for (int i = 0; i < filterKeys.Length; i++)
            {
                ConditionExpression condExpr = new ConditionExpression();
                condExpr.AttributeName = filterKeys[i];
                condExpr.Operator = ConditionOperator.Equal;
                condExpr.Values.Add(filterValues[i]);

                fltrExpr = new FilterExpression(LogicalOperator.And);
                fltrExpr.AddCondition(condExpr);

                query.Criteria.AddFilter(fltrExpr);
            }
           return GetRecordsUsingQuery(query, service);

        }

        static EntityCollection GetRecordsUsingQuery(QueryExpression queryExpr, IOrganizationService service)
        {
            EntityCollection entityCollection = null;
            entityCollection = service.RetrieveMultiple(queryExpr);
            return entityCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public OptionSetValue SetOptionSetValue(int value)
        {
            OptionSetValue optionSetValue = new OptionSetValue(value);
            return optionSetValue;
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
