using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System.ServiceModel;

namespace Tc.Crm.CustomWorkflowSteps
{
    public class CommonXrm
    {

        const string CreateMessage = "New record is created";
        const string UpdateMessage = "Existing record was updated";
        const string DeleteMessage = "Record got deleted successfully";
        const string CreateStatus = "201";
        const string UpdateStatus = "204";
        const string DeleteStatus = "204";
        

        /// <summary>
        /// Call this method to create or update record
        /// </summary>
        /// <param name="EntityRecord">Entity to Create or Update</param>
        /// <param name="Service">OrganizationServiceProxy to Execute Request</param>
        /// <returns></returns>
        public SuccessMessage UpsertEntity(Entity EntityRecord, IOrganizationService Service)
        {  

            SuccessMessage SuccessMsg = null;
            if (Service != null)
            {
                UpsertRequest request = new UpsertRequest()
                {
                    Target = EntityRecord
                };

                try
                {

                    // Execute UpsertRequest and obtain UpsertResponse. 
                    UpsertResponse response = (UpsertResponse)Service.Execute(request);
                    if (response.RecordCreated)
                        SuccessMsg = new SuccessMessage() { Id = response.Target.Id.ToString(), EntityName = EntityRecord.LogicalName, Message = CreateMessage, Status = CreateStatus };
                    else
                        SuccessMsg = new SuccessMessage() { Id = response.Target.Id.ToString(), EntityName = EntityRecord.LogicalName, Message = UpdateMessage, Status = UpdateStatus };


                }

                // Catch any service fault exceptions that Microsoft Dynamics CRM throws.
                catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>)
                {
                    throw;
                }
            }

           
            return SuccessMsg;
        }


        /// <summary>
        /// Call this method for bulk create
        /// </summary>
        /// <param name="Entities"></param>
        /// <param name="Service"></param>
        /// <returns></returns>
        public List<SuccessMessage> BulkCreate(DataCollection<Entity> Entities, IOrganizationService Service)
        {
            List<SuccessMessage> SuccessMsg = null;
            if (Service != null)
            {
                var requestWithResults = new ExecuteMultipleRequest()
                {
                    // Assign settings that define execution behavior: continue on error, return responses. 
                    Settings = new ExecuteMultipleSettings()
                    {
                        ContinueOnError = false,
                        ReturnResponses = true
                    },
                    // Create an empty organization request collection.
                    Requests = new OrganizationRequestCollection()
                };


                // Add a CreateRequest for each entity to the request collection.
                foreach (var entity in Entities)
                {
                    CreateRequest createRequest = new CreateRequest { Target = entity };
                    requestWithResults.Requests.Add(createRequest);
                }

                try
                {
                    // Execute all the requests in the request collection using a single web method call.
                    ExecuteMultipleResponse responseWithResults =
                        (ExecuteMultipleResponse)Service.Execute(requestWithResults);

                    SuccessMsg = new List<SuccessMessage>();

                    // Get the results returned in the responses.
                    foreach (var responseItem in responseWithResults.Responses)
                    {
                        // A valid response.
                        if (responseItem.Response != null)
                        {
                            SuccessMessage Msg = new SuccessMessage() { Id = requestWithResults.Requests[responseItem.RequestIndex].RequestId.Value.ToString(), EntityName = requestWithResults.Requests[responseItem.RequestIndex].RequestName, Message = CreateMessage, Status = CreateStatus };
                            SuccessMsg.Add(Msg);
                        }
                        // An error has occurred.
                        else if (responseItem.Fault != null)
                        {

                        }
                    }
                }
                // Catch any service fault exceptions that Microsoft Dynamics CRM throws.
                catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>)
                {
                    throw;
                }

            }

            return SuccessMsg;
        }


        /// <summary>
        /// Call this method for bulk delete
        /// </summary>
        /// <param name="service">Org Service</param>
        /// <param name="EntityReferences">Collection of EntityReferences to Delete</param>
        public static void BulkDelete(IOrganizationService service, DataCollection<EntityReference> EntityReferences)
        {
            List<SuccessMessage> SuccessMsg = null;
            if (service != null && EntityReferences != null && EntityReferences.Count > 0)
            {
                // Create an ExecuteMultipleRequest object.
                var requestWithResults = new ExecuteMultipleRequest()
                {
                    // Assign settings that define execution behavior: continue on error, return responses. 
                    Settings = new ExecuteMultipleSettings()
                    {
                        ContinueOnError = false,
                        ReturnResponses = true
                    },
                    // Create an empty organization request collection.
                    Requests = new OrganizationRequestCollection()
                };

                // Add a DeleteRequest for each entity to the request collection.
                foreach (var entityRef in EntityReferences)
                {
                    DeleteRequest deleteRequest = new DeleteRequest { Target = entityRef };
                    requestWithResults.Requests.Add(deleteRequest);
                }

                try {
                    // Execute all the requests in the request collection using a single web method call.
                    ExecuteMultipleResponse responseWithResults = (ExecuteMultipleResponse)service.Execute(requestWithResults);

                    // Get the results returned in the responses.
                    foreach (var responseItem in responseWithResults.Responses)
                    {
                        // A valid response.
                        if (responseItem.Response != null)
                        {
                            SuccessMessage Msg = new SuccessMessage() { Id = requestWithResults.Requests[responseItem.RequestIndex].RequestId.Value.ToString(), EntityName = requestWithResults.Requests[responseItem.RequestIndex].RequestName, Message = DeleteMessage, Status = DeleteStatus };
                            SuccessMsg.Add(Msg);
                        }
                        // An error has occurred.
                        else if (responseItem.Fault != null)
                        {

                        }
                    }
                }
                // Catch any service fault exceptions that Microsoft Dynamics CRM throws.
                catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>)
                {
                    throw;
                }
            }
        }






        public class SuccessMessage
        {
            public string EntityName { get; set; }
            public string Id { get; set; }
            public string Message { get; set; }
            public string Status { get; set; }

        }

    }



    
}
