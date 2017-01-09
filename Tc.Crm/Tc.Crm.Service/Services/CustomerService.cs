using Microsoft.Xrm.Sdk;
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

        public static JObject GetJObjectFor(Customer c)
        {
            //guard clause
            if (c == null) return null;
            JObject contact = new JObject();
            contact.Add("firstname", c.FirstName);
            contact.Add("lastname", c.LastName);
            contact.Add("emailaddress", c.Email);
            return contact;
        }

        //public static void Create(Customer c)
        //{
        //    var contact = GetJObjectFor(c);
        //    var task = CrmService.Create(contact,"contacts");
        //    var result = task.Result;
        //}

        public static void Create(Customer c)
        {
            Entity e = new Entity("contact");
            e["new_sourcekey"] = c.Id;
            e["firstname"] = c.FirstName;
            e["lastname"] = c.LastName;
            e["emailaddress1"] = c.Email;
            var id = CrmService.Create(e);
            c.CrmKey = id.ToString();
        }

        public static CustomerUpdateResponse Update(Customer c)
        {
            if (!string.IsNullOrEmpty(c.Id))
            {
                //check if the customer exists in CRM
                if (string.IsNullOrEmpty(c.CrmKey))
                {
                    var contactid = CrmService.RetrieveBy("new_sourcekey", c.Id, "contact", "contactid");
                    if (contactid == null)
                    {
                        Create(c);
                        return new CustomerUpdateResponse { Created = true };
                    }
                    c.CrmKey = contactid.Value.ToString();
                }
                Entity e = new Entity("contact");
                e.Id = new Guid(c.CrmKey);
                e["new_sourcekey"] = c.Id;
                e["firstname"] = c.FirstName;
                e["lastname"] = c.LastName;
                e["emailaddress1"] = c.Email;
                
                CrmService.Update(e);
                return new CustomerUpdateResponse { Created = false};
            }
            else
            {
                Create(c);
                return new CustomerUpdateResponse { Created = true };
            }
        }

        //public static void Update(Customer c)
        //{
        //    var contact = GetJObjectFor(c);
        //    var queryOptions = string.Empty;
        //    var task1 = CrmService.Retrieve(queryOptions, "contacts");
        //    var result1 = task1.Result;
        //    var task2 = CrmService.Update(contact, "contacts");
        //    var result1 = task2.Result;
        //}
    }
}