using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;
using Tc.Crm.CustomWorkflowSteps.ProcessCustomer.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessCustomer.Services
{
    public class ProcessCustomerService
    {
        private PayloadCustomer payloadCustomer;
        private ITracingService trace;
        private IOrganizationService crmService;

        public ProcessCustomerService(PayloadCustomer payloadCustomer)
        {
            if (payloadCustomer == null)
                throw new InvalidPluginExecutionException("Cannot create instance of Service - customer payload instance is null.");
            this.payloadCustomer = payloadCustomer;
            trace = payloadCustomer.Trace;
            crmService = payloadCustomer.CrmService;
        }

        public string processPayload(){
            trace.Trace("Processing Process payload - start");
            if (payloadCustomer == null)
                throw new InvalidPluginExecutionException("Cutomer object created from payload json is null;");
            ProcessCustomer();
            trace.Trace("Processing Process payload - end");
            return JsonHelper.SerializeCustomerJson(payloadCustomer.Response, trace);
        }

        private void ProcessCustomer()
        {
            if (payloadCustomer.Customer == null)
                throw new InvalidPluginExecutionException("Customer information is missing.");
            if (payloadCustomer.Customer.CustomerIdentifier == null)
                throw new InvalidPluginExecutionException("Customer Identifier is missing.");
            if (payloadCustomer.Customer.CustomerGeneral == null)
                throw new InvalidPluginExecutionException("Customer General is missing.");
            if (payloadCustomer.Customer.CustomerGeneral.CustomerType == CustomerType.Company){
                ProcessAccount();
            }
            else if (payloadCustomer.Customer.CustomerGeneral.CustomerType == CustomerType.Person){
                ProcessContact();                
            }
            else
                throw new InvalidPluginExecutionException("The specified Customer Type is not recognized.");
        }

        private void ProcessAccount()
        {
            trace.Trace("Processing Account information - start");
            var customer = payloadCustomer.Customer;
            var operationType = payloadCustomer.OperationType;
            if (operationType.ToUpper() == Enum.GetName(typeof(OperationType), OperationType.POST)){
                var existingAccountCollection = CommonXrm.RetrieveMultipleRecords(EntityName.Account,
                    new string[] { Attributes.Account.SourceSystemId },
                    new string[] { Attributes.Account.SourceSystemId },
                    new string[] { customer.CustomerIdentifier.CustomerId }, crmService);
                if (existingAccountCollection != null){
                    if (existingAccountCollection.Entities.Count > 0){
                        payloadCustomer.Response = new XrmUpdateResponse(){
                            Existing = true,
                            Create = false,
                            Id = null
                        };
                        return;
                    }
                }
                var account = AccountHelper.GetAccountEntityForCustomerPayload(customer, trace); 
                var xrmResponse = CommonXrm.CreateEntity(account, crmService) ; 
                payloadCustomer.CustomerId = xrmResponse.Id;
                payloadCustomer.Response = new XrmUpdateResponse(){
                    Existing = false,
                    Create = xrmResponse.Create,
                    Id = xrmResponse.Id
                };
            }
            else if (operationType.ToUpper() == Enum.GetName(typeof(OperationType), OperationType.PATCH)){
                trace.Trace("inside patch");
                var existingAccountCollection = CommonXrm.RetrieveMultipleRecords(EntityName.Account,
                    new string[] { Attributes.Account.AccountId, Attributes.Account.SourceSystemId },
                    new string[] { Attributes.Account.DuplicateSourceSystemId },
                    new string[] { customer.CustomerIdentifier.CustomerId }, crmService);
                if (existingAccountCollection == null ||
                    (existingAccountCollection != null && existingAccountCollection.Entities.Count <= 0)){
                    payloadCustomer.Response = new XrmUpdateResponse(){
                        Existing = false,
                        Updated=false,
                        Id = null
                    };
                    return;
                }
                ExecuteTransactionRequest multipleRequest = new ExecuteTransactionRequest{
                    Requests = new OrganizationRequestCollection(),
                    ReturnResponses = true
                };
                foreach (Entity existingAccount in existingAccountCollection.Entities){ 
                    var account = AccountHelper.GetAccountEntityForCustomerPayload(customer, trace, OperationType.PATCH);
                    account[Attributes.Account.AccountId] = existingAccount.GetAttributeValue<Guid>
                                                                (Attributes.Account.AccountId);
                    trace.Trace(account[Attributes.Account.AccountId].ToString());
                    var updateRequest = new UpdateRequest { Target = account };
                    multipleRequest.Requests.Add(updateRequest);
                    if (multipleRequest.Requests.Count == 500){
                        payloadCustomer.Response = CommonXrm.BulkUpdate(multipleRequest, crmService);
                        multipleRequest.Requests.Clear();
                    }
                }
                if (multipleRequest.Requests.Count > 0){
                    payloadCustomer.Response = CommonXrm.BulkUpdate(multipleRequest, crmService);
                }
            }
            trace.Trace("Processing Account information - end");
        }

        private void ProcessContact(){
            trace.Trace("Processing Contact information - start");
            var customer = payloadCustomer.Customer;
            var operationType = payloadCustomer.OperationType;
            if (operationType.ToUpper() == Enum.GetName(typeof(OperationType), OperationType.POST)){
                var contact = ContactHelper.GetContactEntityForCustomerPayload(customer, trace, operationType);
                var existingContact = CommonXrm.RetrieveMultipleRecords(contact.LogicalName, 
                                        new string[] { Attributes.Contact.SourceSystemId },
                                        new string[] { Attributes.Contact.SourceSystemId }, 
                                        new string[] { customer.CustomerIdentifier.CustomerId }, crmService);
                if (existingContact.Entities.Count > 0){
                    payloadCustomer.Response = new XrmUpdateResponse(){
                        Existing = true,
                        Create = false,
                        Id = null
                    };
                    return;
                }
                var xrmResponse = CommonXrm.CreateEntity(contact, crmService);
                ProcessSocialProfile(new Guid(xrmResponse.Id.ToString()));
                payloadCustomer.CustomerId = xrmResponse.Id;
                payloadCustomer.Response = new XrmUpdateResponse(){
                    Existing = false,
                    Create = xrmResponse.Create,
                    Id = xrmResponse.Id
                };
                ProcessSocialProfile(Guid.Parse(payloadCustomer.Response.Id));
            }
            else if (operationType.ToUpper() == Enum.GetName(typeof(OperationType), OperationType.PATCH)){
                var newContacts = new EntityCollection();

                var existingContacts = CommonXrm.RetrieveMultipleRecords(EntityName.Contact,
                                        new string[] { Attributes.Contact.ContactId, Attributes.Contact.SourceSystemId },
                                        new string[] { Attributes.Contact.DuplicateSourceSystemId },
                                        new string[] { customer.CustomerIdentifier.CustomerId }, crmService);

                if (existingContacts == null ||
                    (existingContacts != null && existingContacts.Entities.Count <= 0)){
                    payloadCustomer.Response = new XrmUpdateResponse(){
                        Existing = false,
                        Updated = false,
                        Id = null
                    };
                    return;
                }
                
                UpdateRequest updateRequest;
                ExecuteTransactionRequest multipleRequest = new ExecuteTransactionRequest{
                    Requests = new OrganizationRequestCollection(),
                    ReturnResponses = true
                }; 
                foreach (var existingContact in existingContacts.Entities){
                    var contact = ContactHelper.GetContactEntityForCustomerPayload(existingContact, 
                                                        customer, trace, operationType);
                    ProcessSocialProfile(contact.Id);
                    updateRequest = new UpdateRequest { Target = contact };
                    multipleRequest.Requests.Add(updateRequest);
                    if (multipleRequest.Requests.Count == 500){
                        payloadCustomer.Response = CommonXrm.BulkUpdate(multipleRequest, crmService);
                        multipleRequest.Requests.Clear();
                    }
                }
                if (multipleRequest.Requests.Count > 0){
                    payloadCustomer.Response = CommonXrm.BulkUpdate(multipleRequest, crmService);
                }
            }
            trace.Trace("Processing Contact information - end");
        }

        private void ProcessSocialProfile(Guid CustomerID)
        {
            trace.Trace("Processing Social profile information - start");
            if (payloadCustomer.Customer.Social == null)
                throw new InvalidPluginExecutionException("Customer Social information is null.");
            if (CustomerID == null)
                throw new InvalidPluginExecutionException("Social Profile Customer ID is null.");
            var entityCollectionsocialProfiles = SocialProfileHelper.GetSocialProfileEntityFromPayload(payloadCustomer.Customer, CustomerID, trace);
            if (entityCollectionsocialProfiles != null && entityCollectionsocialProfiles.Entities.Count > 0)
            {
                foreach (Entity entitySocialProfile in entityCollectionsocialProfiles.Entities)
                {
                    CommonXrm.UpsertEntity(entitySocialProfile, crmService);
                }
            }
            trace.Trace("Processing Social Profile information - end");            
        }
    }
}
