using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;

using Attributes = Tc.Crm.CustomWorkflowSteps.Attributes;

namespace Tc.Crm.CustomWorkflowSteps
{
    public class MergeCustomerService
    {
        public void MergeCustomers(EntityReference entityMerge, IOrganizationService service, ITracingService trace)
        {
            Entity mergeRecord = service.Retrieve(entityMerge.LogicalName, entityMerge.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(
                Attributes.EntityMerge.Master,
                Attributes.EntityMerge.Subordinate,
                Attributes.EntityMerge.ModifiedOn));

            if (IsEntityMergeRecordValid(mergeRecord))
            {
                ExecuteMerge(mergeRecord, service, trace);
            }
        }

        private void ExecuteMerge(Entity mergeRecord, IOrganizationService service, ITracingService trace)
        {
            MergeRequest mergeRequest = new MergeRequest();
            mergeRequest.SubordinateId = (mergeRecord[Attributes.EntityMerge.Subordinate] as EntityReference).Id;
            mergeRequest.Target = (mergeRecord[Attributes.EntityMerge.Master] as EntityReference);
            mergeRequest.PerformParentingChecks = false;

            Entity masterCustomer = GetCustomerById(service, (mergeRecord[Attributes.EntityMerge.Subordinate] as EntityReference));
            Entity subordinateCustomer = GetCustomerById(service, (mergeRecord[Attributes.EntityMerge.Subordinate] as EntityReference));

            //if(IsSubordinateNewerThanMaster(masterCustomer, subordinateCustomer))           
            //    mergeRequest.UpdateContent = subordinateCustomer;
            
            service.Execute(mergeRequest);
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
                Attributes.Contact.ModifiedOn));
        }

        private bool IsSubordinateNewerThanMaster(Entity masterCustomer, Entity subordinateCustomer)
        {
            DateTime? masterModifiedOn = subordinateCustomer[Attributes.Contact.ModifiedOn] as DateTime?;
            DateTime? subordinateModifiedOn = subordinateCustomer[Attributes.Contact.ModifiedOn] as DateTime?;
            return subordinateModifiedOn.Value > masterModifiedOn.Value;
        }
    }
}
