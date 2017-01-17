using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public static class CustomerService
    {
        public static Customer GetCustomerFromPayload(string dataJson)
        {
            if (string.IsNullOrWhiteSpace(dataJson)) throw new ArgumentNullException(Constants.Parameters.DataJson);
            return JsonConvert.DeserializeObject<Customer>(dataJson);
        }

        public static CustomerUpdateResponse Update(Customer customer)
        {
            if (customer == null) throw new ArgumentNullException(Constants.Parameters.Customer);

            KeyAttributeCollection keys = new KeyAttributeCollection();
            keys.Add(Constants.Crm.Fields.Contact.SourceKey, customer.Id);

            Entity contact = new Entity(Constants.Crm.Contact.LogicalName,keys);
            contact[Constants.Crm.Fields.Contact.SourceKey] = customer.Id;
            contact[Constants.Crm.Fields.Contact.FirstName] = customer.FirstName;
            contact[Constants.Crm.Fields.Contact.LastName] = customer.LastName;
            contact[Constants.Crm.Fields.Contact.Email] = customer.Email;

            var response = CrmService.Upsert(contact);

            if (response == null) throw new InvalidOperationException(Constants.Messages.ResponseNull);
            if (response.RecordCreated)
                return new CustomerUpdateResponse { Created = true, Id = response.Target.Id.ToString() };
            return new CustomerUpdateResponse { Created = false , Id = response.Target.Id.ToString() };

        }
    }
}