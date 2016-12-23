using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Controllers
{
    public class CustomerController : ApiController
    {
        Customer[] customers = new Customer[]
           {
                new Customer { FirstName = "John", LastName = "Doe",SourceSystemId = "CU001",Id="CRM101"},
                new Customer { FirstName = "Joe", LastName = "Blog",SourceSystemId = "CU002",Id="CRM102"},
                new Customer { FirstName = "John", LastName = "Jones",SourceSystemId = "CU003",Id="CRM103"},
           };

        [Route("api/customers")]
        [Route("api/allcustomers")]
        [Route("api/v1/customers")]
        [Route("api/v1/allcustomers")]
        public IEnumerable<Customer> GetAllCustomers()
        {
            return customers;
        }
        [Route("api/customers/{id}/customer")]
        [Route("api/v1/customers/{id}/customer")]
        [Route("api/customer/{id}")]
        [Route("api/v1/customer/{id}")]
        public IHttpActionResult GetCustomer(string sourceSystemId)
        {
            var customer = customers.FirstOrDefault((p) => p.Id == sourceSystemId);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        [Route("api/customers/create")]
        [Route("api/v1/customers/create")]
        [Route("api/createcustomer")]
        [Route("api/v1/createcustomer")]
        public HttpResponseMessage CreateBooking(Customer c)
        {
            var b = customers.FirstOrDefault((p) => p.Id == c.Id);
            if (b == null)
            {
                var response = new HttpResponseMessage();
                response.Headers.Add("Message", "Created!!!");
                return response;
            }
            else
            {
                var response = new HttpResponseMessage();
                response.Headers.Add("Message", "Updated!!!");
                return response;
            }

        }
    }
}
