using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
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
        private void ProcessCustomer(){
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

        #region Process Account
        private void ProcessAccount(){
            trace.Trace("Processing Account information - start");
            var customer = payloadCustomer.Customer;
            var operationType = payloadCustomer.OperationType;
            if (operationType.ToUpper() == OperationType.POST.ToString()){
                DoActionsOnPostAccount(customer);
            }
            else if (operationType.ToUpper() == OperationType.PATCH.ToString()){

                DoActionsOnPatchAccount(customer);
            }
            trace.Trace("Processing Account information - end");
        }
        private void DoActionsOnPostAccount(Customer customer){
            if (customer == null) return;
            var accounts = GetAccountsBySourceSystemId(customer.CustomerIdentifier.CustomerId);
            if (accounts != null){
                if (accounts.Entities.Count > 0){
                    payloadCustomer.Response = new XrmUpdateResponse(){
                        Existing = true,
                        Create = false,
                        Id = null
                    };
                    return;
                }
            }
            var account = AccountHelper.GetAccountEntityForCustomerPayload(customer, trace);
            var xrmResponse = CommonXrm.CreateEntity(account, crmService);
            payloadCustomer.CustomerId = xrmResponse.Id;
            payloadCustomer.Response = new XrmUpdateResponse(){
                Existing = false,
                Create = xrmResponse.Create,
                Id = xrmResponse.Id
            };
        }
        private void DoActionsOnPatchAccount(Customer customer){
            if (customer == null) return;
            var accounts = GetAccountsByDuplicateSourceSystemId(customer.CustomerIdentifier.CustomerId);
            if (accounts == null) return;
            if (accounts == null ||
                (accounts != null && accounts.Entities.Count <= 0)){
                payloadCustomer.Response = new XrmUpdateResponse()
                {
                    Existing = false,
                    Updated = false,
                    Id = null
                };
                return;
            }
            var account = AccountPatchHelper.GetAccountEntityForCustomerPayload(customer, trace);
            UpdateAccounts(accounts, account);
        }
        private void UpdateAccounts(EntityCollection accounts, Entity account){
            if (accounts == null) return;
            if (account == null) return;
            ExecuteTransactionRequest multipleRequest = new ExecuteTransactionRequest
            {
                Requests = new OrganizationRequestCollection(),
                ReturnResponses = true
            };
            foreach (Entity existingAccount in accounts.Entities){
                account[Attributes.Account.AccountId] = existingAccount.Id;
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
        private EntityCollection GetAccountsByDuplicateSourceSystemId(string customerId){
            return CommonXrm.RetrieveMultipleRecords(EntityName.Account,
                    new string[] { Attributes.Account.AccountId, Attributes.Account.SourceSystemId },
                    new string[] { Attributes.Account.DuplicateSourceSystemId },
                    new string[] { customerId }, crmService); 
        }
        private EntityCollection GetAccountsBySourceSystemId(string customerId){
            return CommonXrm.RetrieveMultipleRecords(EntityName.Account,
                    new string[] { Attributes.Account.AccountId, Attributes.Account.SourceSystemId },
                    new string[] { Attributes.Account.SourceSystemId },
                    new string[] { customerId }, crmService); 
        }
        #endregion

        #region Process Contact
        private void ProcessContact(){
            trace.Trace("Processing Contact information - start");
            var customer = payloadCustomer.Customer;
            var operationType = payloadCustomer.OperationType;
            if (operationType.ToUpper() == OperationType.POST.ToString())
            {
                DoActionsOnPostContact(customer);
            }
            else if (operationType.ToUpper() == OperationType.PATCH.ToString())
            {

                DoActionsOnPatchContact(customer);
            }
            trace.Trace("Processing Contact information - end");
        }
        private void DoActionsOnPostContact(Customer customer){
            if (customer == null) return;
            var contacts = GetContactsBySourceSystemId(customer.CustomerIdentifier.CustomerId);
            if (contacts != null){
                if (contacts.Entities.Count > 0){
                    payloadCustomer.Response = new XrmUpdateResponse(){
                        Existing = true,
                        Create = false,
                        Id = null
                    };
                    return;
                }
            }
            var contact = ContactHelper.GetContactEntityForCustomerPayload(customer, trace);
            var xrmResponse = CommonXrm.CreateEntity(contact, crmService);
            payloadCustomer.CustomerId = xrmResponse.Id;
            payloadCustomer.Response = new XrmUpdateResponse(){
                Existing = false,
                Create = xrmResponse.Create,
                Id = xrmResponse.Id
            };
        }
        private void DoActionsOnPatchContact(Customer customer){
            if (customer == null) return;
            var contacts = GetContactsByDuplicateSourceSystemId(customer.CustomerIdentifier.CustomerId);
            if (contacts == null) return;
            if (contacts == null ||
                (contacts != null && contacts.Entities.Count <= 0)){
                payloadCustomer.Response = new XrmUpdateResponse(){
                    Existing = false,
                    Updated = false,
                    Id = null
                };
                return;
            }
            var contact = ContactPatchHelper.GetContactEntityForCustomerPayload(customer, trace);
            UpdateContacts(contacts, contact);
        }
        private void UpdateContacts(EntityCollection contacts, Entity contact){
            if (contacts == null) return;
            if (contact == null) return;
            ExecuteTransactionRequest multipleRequest = new ExecuteTransactionRequest{
                Requests = new OrganizationRequestCollection(),
                ReturnResponses = true
            };
            foreach (Entity existingContact in contacts.Entities){
                contact[Attributes.Contact.ContactId] = existingContact.Id;
                var updateRequest = new UpdateRequest { Target = contact };
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
        private EntityCollection GetContactsByDuplicateSourceSystemId(string customerId)
        {
            return CommonXrm.RetrieveMultipleRecords(EntityName.Contact,
                                        new string[] { Attributes.Account.AccountId, Attributes.Contact.SourceSystemId },
                                        new string[] { Attributes.Contact.SourceSystemId },
                                        new string[] { customerId }, crmService);
        }
        private EntityCollection GetContactsBySourceSystemId(string customerId)
        {
            return CommonXrm.RetrieveMultipleRecords(EntityName.Contact,
                                        new string[] { Attributes.Account.AccountId, Attributes.Contact.SourceSystemId },
                                        new string[] { Attributes.Contact.DuplicateSourceSystemId },
                                        new string[] { customerId }, crmService);
        }
        #endregion

        
    }
}
