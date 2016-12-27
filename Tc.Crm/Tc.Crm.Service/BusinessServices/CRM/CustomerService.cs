using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Tc.Crm.Service.Common;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.BusinessServices.CRM
{
    public class CustomerService : ICustomerService
    {
        IDataService service = null;
        public CustomerService()
        {
            service = new CrmDataService();
        }
        public async  Task<Guid> Create(Customer customer)
        {
            var t = await service.Create("contacts", GetBookingJsonObject(customer));
            return t;
        }
        public JObject GetBookingJsonObject(Customer customer)
        {
            JObject jsonBooking = new JObject();
            jsonBooking[Constants.CrmFields.Customer.FirstName] = customer.FirstName;
            jsonBooking[Constants.CrmFields.Customer.LastName] = customer.LastName;

            return jsonBooking;
        }
       
        public async Task<bool> Update(Customer customer)
        {
            var t = await service.Update("contacts", GetBookingJsonObject(customer));
            return t;
        }

        public async Task<Customer> Upsert(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.Id))
            {
                var t = await service.Create("contacts", GetBookingJsonObject(customer));
                customer.Id = t.ToString();
                return customer;
            }
            else
            {
                var t = await service.Update("contacts", GetBookingJsonObject(customer));
                if (t)
                    return customer;
                return null;
            }
        }
    }
}