using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using System.Collections.Generic;

using Attributes = Tc.Crm.CustomWorkflowSteps.Attributes;

namespace Tc.Crm.CustomWorkflowSteps
{
    public class MergeCustomerService
    {
        public void MergeCustomers(EntityReference entityMerge, IOrganizationService service, ITracingService trace)
        {
            Entity mergeRecord = service.Retrieve(entityMerge.LogicalName, entityMerge.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(
                Attributes.EntityMerge.Master,
                Attributes.EntityMerge.Subordinate));
            trace.Trace("Retrieved Entity Merge Record: {0}", mergeRecord.Id);

            if (IsEntityMergeRecordValid(mergeRecord))
                ExecuteMerge(mergeRecord, service, trace);
            else
                throw new InvalidPluginExecutionException("The tc_entitymerge record did not contain a valid master and subordinate reference");
        }

        private void ExecuteMerge(Entity mergeRecord, IOrganizationService service, ITracingService trace)
        {
            trace.Trace("Executing merge request for record {0}, master {1}, subordinate {2}", 
                mergeRecord.Id,
                (mergeRecord[Attributes.EntityMerge.Master] as EntityReference).Id,
                (mergeRecord[Attributes.EntityMerge.Subordinate] as EntityReference).Id);

            MergeRequest mergeRequest = new MergeRequest()
            {
                SubordinateId = (mergeRecord[Attributes.EntityMerge.Subordinate] as EntityReference).Id,
                Target = (mergeRecord[Attributes.EntityMerge.Master] as EntityReference)
            };

            Entity masterCustomer = GetCustomerById(service, (mergeRecord[Attributes.EntityMerge.Subordinate] as EntityReference));
            Entity subordinateCustomer = GetCustomerById(service, (mergeRecord[Attributes.EntityMerge.Subordinate] as EntityReference));
            trace.Trace("Retrieved master and subordinate attributes in order to compute 'UpdateContent' property");

            mergeRequest.PerformParentingChecks = false;            
            mergeRequest.UpdateContent = ProcessUpdatedContent(subordinateCustomer);            
            service.Execute(mergeRequest);
            trace.Trace("Executed Merge");
        }

        private bool IsEntityMergeRecordValid(Entity mergeRecord)
        {
            if (mergeRecord == null)
                throw new ArgumentNullException("mergeRecord parameter can not be null");                       

            return
                mergeRecord.Contains(Attributes.EntityMerge.Master) &&
                mergeRecord[Attributes.EntityMerge.Master] != null &&
                mergeRecord.Contains(Attributes.EntityMerge.Subordinate) &&
                mergeRecord[Attributes.EntityMerge.Subordinate] != null;

        }

        private Entity GetCustomerById(IOrganizationService service, EntityReference customer)
        {
            if(customer == null)
                throw new ArgumentNullException("mergeRecord parameter can not be null");

            return service.Retrieve(customer.LogicalName, customer.Id, new ColumnSet(
               GetColumnsByEntityType(customer.LogicalName)));
        }

        private Entity ProcessUpdatedContent(Entity subordinateCustomer)
        {
            Entity updatedContent = new Entity(subordinateCustomer.LogicalName);
            List<string> attributeKeys = subordinateCustomer.Attributes.Keys.ToList();
            for (int i = 0; i < attributeKeys.Count; i++)
            {
                if ((attributeKeys[i] != Attributes.Account.AccountId ||
                    attributeKeys[i] != Attributes.Contact.ContactId) &&
                    subordinateCustomer[attributeKeys[i]] != null)
                {
                    if (subordinateCustomer[attributeKeys[i]] is EntityReference)
                    {
                        EntityReference reference = subordinateCustomer[attributeKeys[i]] as EntityReference;
                        updatedContent.Attributes.Add(attributeKeys[i], new EntityReference(reference.LogicalName, reference.Id));
                    }
                    else
                        updatedContent.Attributes.Add(attributeKeys[i], subordinateCustomer[attributeKeys[i]]);
                }
            }
            return updatedContent;
        }

        private string[] GetColumnsByEntityType(string entityTypeName)
        {
            if (entityTypeName == EntityName.Contact)
            {
                return new string[]
                {
                    Attributes.Contact.Salutation,
                    Attributes.Contact.AcademicTitle,
                    Attributes.Contact.FirstName,
                    Attributes.Contact.MiddleName,
                    Attributes.Contact.LastName,
                    Attributes.Contact.Birthdate,
                    Attributes.Contact.Gender,
                    Attributes.Contact.Language,
                    Attributes.Contact.SourceMarketId,                    
                    Attributes.Contact.EmailAddressAvailable,
                    Attributes.Contact.EmailAddress1,
                    Attributes.Contact.EmailAddress1Type,
                    Attributes.Contact.EmailAddress2,
                    Attributes.Contact.EmailAddress2Type,
                    Attributes.Contact.EmailAddress3,
                    Attributes.Contact.EmailAddress3Type,
                    Attributes.Contact.Telephone1,
                    Attributes.Contact.Telephone1Type,
                    Attributes.Contact.Telephone2,
                    Attributes.Contact.Telephone2Type,
                    Attributes.Contact.Telephone3,
                    Attributes.Contact.Telephone3Type,
                    Attributes.Contact.Segment,
                    Attributes.Contact.DateOfDeath,
                    Attributes.Contact.DisabledIndicator,
                    Attributes.Contact.VIP,
                    Attributes.Contact.Colleague,
                    Attributes.Contact.Address1FlatOrUnitNumber,
                    Attributes.Contact.Address1HouseNumberOrBuilding,
                    Attributes.Contact.Address1Street,
                    Attributes.Contact.Address1AdditionalInformation,
                    Attributes.Contact.Address1Town,
                    Attributes.Contact.Address1County,
                    Attributes.Contact.Address1PostalCode,
                    Attributes.Contact.Address1CountryId,
                    Attributes.Contact.Address2FlatOrUnitNumber,
                    Attributes.Contact.Address2HouseNumberOrBuilding,
                    Attributes.Contact.Address2Street,
                    Attributes.Contact.Address2AdditionalInformation,
                    Attributes.Contact.Address2Town,
                    Attributes.Contact.Address2County,
                    Attributes.Contact.Address2PostalCode,
                    Attributes.Contact.Address2CountryId,
                    Attributes.Contact.ThomasCookMarketingConsent,
                    Attributes.Contact.SendMarketingByEmail,
                    Attributes.Contact.SendMarketingByPost,
                    Attributes.Contact.SendMarketingBySms,
                    Attributes.Contact.MarketingByPhone,
                    Attributes.Contact.MarketingUpdated,
                    Attributes.Contact.MarketingUpdatedBy,
                    Attributes.Contact.MarketingConsentUpdated,
                    Attributes.Contact.NoLongerLivingAtAddress,
                    Attributes.Contact.Deceased,
                    Attributes.Contact.AnnualInsurance,
                    Attributes.Contact.TravelFinance,
                    Attributes.Contact.AnnualInsuranceExpiredDate,
                    Attributes.Contact.PaymentByDirectDebit,
                    Attributes.Contact.StoreCreated,
                    Attributes.Contact.ClusterCreated,
                    Attributes.Contact.RegionCreated,
                    Attributes.Contact.TerrirtoryCreatedBy,
                    Attributes.Contact.StoreModified,
                    Attributes.Contact.ClusterModified,
                    Attributes.Contact.RegionModified,
                    Attributes.Contact.TerritoryModifiedBy
                };
            }
            else
                return new string[] { };
        }
    }
}
