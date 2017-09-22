using Microsoft.Xrm.Sdk;
using System;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessCustomer.Services
{
    public static class ContactHelper
    {
        public static Entity GetContactEntityForCustomerPayload(Customer customer, ITracingService trace, string operationType)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Contact populate fields - start");
            if (customer == null) return null;

            Entity contact = new Entity(EntityName.Contact);

            PopulateIdentityInformation(contact, customer.CustomerIdentity, trace);
            if (operationType.ToUpper() == Enum.GetName(typeof(OperationType), OperationType.POST))
            {
                contact[Attributes.Contact.SourceSystemId] = (customer.CustomerIdentifier != null
                                                                && !string.IsNullOrWhiteSpace(customer.CustomerIdentifier.CustomerId))
                                                                    ? customer.CustomerIdentifier.CustomerId
                                                                    : string.Empty;
                contact[Attributes.Contact.DuplicateSourceSystemId] = (customer.CustomerIdentifier != null
                                                                && !string.IsNullOrWhiteSpace(customer.CustomerIdentifier.CustomerId))
                                                                    ? customer.CustomerIdentifier.CustomerId
                                                                    : string.Empty;
            }
            

            trace.Trace("Contact populate fields - end");

            return contact;
        }

        public static Entity GetContactEntityForCustomerPayload(Entity existingContact, Customer customer, ITracingService trace, string operationType)
        {
            trace.Trace("Contact populate id - start");
            var contact = GetContactEntityForCustomerPayload(customer, trace, operationType);
            contact["contactid"] = existingContact.GetAttributeValue<Guid>("contactid");            
            trace.Trace("Contact populate id - end");
            return contact;
        }

        private static void PopulateIdentityInformation(Entity contact, CustomerIdentity identity, ITracingService trace)
        {
            trace.Trace("Contact populate identity - start");
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            if (identity == null) return;            
            if (!string.IsNullOrWhiteSpace(identity.FirstName))
                contact[Attributes.Contact.FirstName] = identity.FirstName;
            if (!string.IsNullOrWhiteSpace(identity.LastName))
                contact[Attributes.Contact.LastName] = identity.LastName;
            if (!string.IsNullOrWhiteSpace(identity.Language))
                contact[Attributes.Contact.Language] = CommonXrm.GetLanguage(identity.Language);
            if (!string.IsNullOrWhiteSpace(identity.Salutation))
                contact[Attributes.Contact.Salutation] = CommonXrm.GetSalutation(identity.Salutation);
            if (!string.IsNullOrWhiteSpace(identity.Birthdate))
                contact[Attributes.Contact.Birthdate] = Convert.ToDateTime(identity.Birthdate);
            trace.Trace("Contact populate identity - end");
        }

    }
}
