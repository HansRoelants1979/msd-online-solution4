using Microsoft.Xrm.Sdk;
using System;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessCustomer.Services
{
    public static class AccountHelper
    {
        public static Entity GetAccountEntityForCustomerPayload(Customer customer, ITracingService trace, 
            OperationType operationType = OperationType.POST)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null.");
            trace.Trace("Account populate fields - start");
            if (customer == null) throw new InvalidPluginExecutionException("Customer payload is null.");
            if (customer.CustomerIdentifier == null)
                throw new InvalidPluginExecutionException("Customer Identifier could not be retrieved from payload.");
            Entity account = new Entity(EntityName.Account);
            if (customer.Company != null && !string.IsNullOrWhiteSpace(customer.Company.CompanyName))
                account[Attributes.Account.Name] = customer.Company.CompanyName;
            if (operationType == OperationType.POST)
            {

                account[Attributes.Account.SourceMarketId] = (!string.IsNullOrWhiteSpace(customer.CustomerIdentifier.SourceMarket)) ?
                                                             new EntityReference(EntityName.Country,
                                                             new Guid(customer.CustomerIdentifier.SourceMarket)) : null;
                account[Attributes.Account.SourceSystemId] = (!string.IsNullOrWhiteSpace(customer.CustomerIdentifier.CustomerId)) ?
                                                             customer.CustomerIdentifier.CustomerId : string.Empty;
                account[Attributes.Account.DuplicateSourceSystemId] = (!string.IsNullOrWhiteSpace(customer.CustomerIdentifier.CustomerId)) ?
                                                             customer.CustomerIdentifier.CustomerId : string.Empty;
            }
            trace.Trace("Account populate fields - end");
            return account;
        }
    }
}
