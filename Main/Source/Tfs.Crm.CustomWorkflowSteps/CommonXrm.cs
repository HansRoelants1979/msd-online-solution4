using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System.ServiceModel;

namespace Tc.Crm.CustomWorkflowSteps
{
    public class CommonXrm
    {

        const string _createMessage = "New record is created";
        const string _updateMessage = "Existing record was updated";
        const string _deleteMessage = "Record got deleted successfully";
        const string _createStatus = "201";
        const string _updateStatus = "204";
        const string _deleteStatus = "204";


        /// <summary>
        /// Call this method to create or update record
        /// </summary>
        /// <param name="entityRecord">Entity to Create or Update</param>
        /// <param name="service">OrganizationServiceProxy to Execute Request</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Tc.Crm.CustomWorkflowSteps.SuccessMessage.set_Message(System.String)")]
        public SuccessMessage UpsertEntity(Entity entityRecord, IOrganizationService service)
        {  

            SuccessMessage successMsg = null;
            if (service != null)
            {
                UpsertRequest request = new UpsertRequest()
                {
                    Target = entityRecord
                };

                try
                {

                    // Execute UpsertRequest and obtain UpsertResponse. 
                    UpsertResponse response = (UpsertResponse)service.Execute(request);
                    if (response.RecordCreated)
                        successMsg = new SuccessMessage
                        {
                            Id = response.Target.Id.ToString(),
                            EntityName = entityRecord.LogicalName,
                            Message = _createMessage,
                            Status = _createStatus
                        };
                    else
                        successMsg = new SuccessMessage()
                        {
                            Id = response.Target.Id.ToString(),
                            EntityName = entityRecord.LogicalName,
                            Message = _updateMessage,
                            Status = _updateStatus
                        };


                }

                // Catch any service fault exceptions that Microsoft Dynamics CRM throws.
                catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>)
                {
                    throw;
                }
            }

           
            return successMsg;
        }


        /// <summary>
        /// Call this method for bulk create
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public List<SuccessMessage> BulkCreate(DataCollection<Entity> entities, IOrganizationService service)
        {
            List<SuccessMessage> successMsg = null;
            if (service != null && entities != null && entities.Count > 0)
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
                foreach (var entity in entities)
                {
                    CreateRequest createRequest = new CreateRequest { Target = entity };
                    requestWithResults.Requests.Add(createRequest);
                }

                try
                {
                    // Execute all the requests in the request collection using a single web method call.
                    ExecuteMultipleResponse responseWithResults =
                        (ExecuteMultipleResponse)service.Execute(requestWithResults);

                    successMsg = new List<SuccessMessage>();

                    // Get the results returned in the responses.
                    foreach (var responseItem in responseWithResults.Responses)
                    {
                        // A valid response.
                        if (responseItem.Response != null)
                        {
                            var msg = new SuccessMessage
                            {
                                Id = requestWithResults.Requests[responseItem.RequestIndex].RequestId.Value.ToString(),
                                EntityName = requestWithResults.Requests[responseItem.RequestIndex].RequestName,
                                Message = _createMessage,
                                Status = _createStatus
                            };
                            successMsg.Add(msg);
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

            return successMsg;
        }


        /// <summary>
        /// Call this method for bulk delete
        /// </summary>
        /// <param name="service">Org Service</param>
        /// <param name="entityReferences">Collection of EntityReferences to Delete</param>
        public static void BulkDelete(IOrganizationService service, DataCollection<EntityReference> entityReferences)
        {
            List<SuccessMessage> successMsg = null;
            if (service != null && entityReferences != null && entityReferences.Count > 0)
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
                foreach (var entityRef in entityReferences)
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
                            var msg = new SuccessMessage
                            {
                                Id = requestWithResults.Requests[responseItem.RequestIndex].RequestId.Value.ToString(),
                                EntityName = requestWithResults.Requests[responseItem.RequestIndex].RequestName,
                                Message = _deleteMessage,
                                Status = _deleteStatus
                            };
                            successMsg.Add(msg);
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

        

    }


    public class SuccessMessage
    {
        public string EntityName { get; set; }
        public string Id { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }

    }




}
