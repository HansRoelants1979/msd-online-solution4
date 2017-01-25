using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public class CustomerService:ICustomerService
    {
        public CustomerUpdateResponse Update(Customer customer,ICrmService crmService)
        {
            if (customer == null) throw new ArgumentNullException(Constants.Parameters.Customer);

            KeyAttributeCollection keys = new KeyAttributeCollection();
            keys.Add(Constants.Crm.Fields.Contact.SourceKey, customer.Id);

            Entity contact = new Entity(Constants.Crm.Contact.LogicalName,keys);
            contact[Constants.Crm.Fields.Contact.SourceKey] = customer.Id;
            contact[Constants.Crm.Fields.Contact.FirstName] = customer.FirstName;
            contact[Constants.Crm.Fields.Contact.LastName] = customer.LastName;
            contact[Constants.Crm.Fields.Contact.Email] = customer.Email;

            var response = crmService.Upsert(contact);

            if (response == null) throw new InvalidOperationException(Constants.Messages.ResponseNull);
            if (response.RecordCreated)
                return new CustomerUpdateResponse { Created = true, Id = response.Target.Id.ToString() };
            return new CustomerUpdateResponse { Created = false , Id = response.Target.Id.ToString() };

        }
    }
}