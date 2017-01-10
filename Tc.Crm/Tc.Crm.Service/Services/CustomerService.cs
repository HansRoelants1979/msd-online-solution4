using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public class CustomerService
    {
        public static Customer GetCustomerFromPayload(string dataJson)
        {
            return JsonConvert.DeserializeObject<Customer>(dataJson);
        }

        public static CustomerUpdateResponse Update(Customer customer)
        {
            KeyAttributeCollection keys = new KeyAttributeCollection();
            keys.Add(Constants.Crm.Contact.Fields.SOURCE_KEY, customer.Id);

            Entity contact = new Entity(Constants.Crm.Contact.LOGICAL_NAME,keys);
            contact[Constants.Crm.Contact.Fields.SOURCE_KEY] = customer.Id;
            contact[Constants.Crm.Contact.Fields.FIRST_NAME] = customer.FirstName;
            contact[Constants.Crm.Contact.Fields.LAST_NAME] = customer.LastName;
            contact[Constants.Crm.Contact.Fields.EMAIL] = customer.Email;

            var response = CrmService.Upsert(contact);

            if (response == null) throw new InvalidOperationException(Constants.Messages.RESPONSE_NULL);
            if (response.RecordCreated)
                return new CustomerUpdateResponse { Created = true};
            return new CustomerUpdateResponse { Created = false };

        }
    }
}