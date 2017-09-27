using JsonPatch;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tc.Crm.Service.Filters;
using Tc.Crm.Service.Models;
using Tc.Crm.Service.Services;

namespace Tc.Crm.Service.Controllers
{
    public class CustomerController : ApiController
    {

        ICustomerService customerService;
        ICrmService crmService;

        public CustomerController(ICustomerService customerService, ICrmService crmService)
        {
            this.customerService = customerService;
            this.crmService = crmService;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        [Route("api/customers/customer")]
        [HttpPost]
        [JsonWebTokenAuthorize]
        public HttpResponseMessage Create(CustomerInformation customerInfo)
        {
            try
            {
                var messages = customerService.Validate(customerInfo);
                if (messages != null && messages.Count != 0)
                {
                    var message = customerService.GetStringFrom(messages);
                    Trace.TraceWarning(message);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, message);
                }

                var customer = customerInfo.Customer;
                customerService.ResolveReferences(customer);
                var jsonData = JsonConvert.SerializeObject(customer);
                var response = customerService.Create(jsonData, crmService);
                if (response.Existing)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Constants.Messages.CustomerCreationError);
                if (response.Create && !string.IsNullOrWhiteSpace(response.Id))
                    return Request.CreateResponse(HttpStatusCode.Created, response.Id);
                else
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Unexpected error Customer.Create::Message:{0}||Trace:{1}", ex.Message, ex.StackTrace.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        [Route("api/customers/{gbgId}")]
        [HttpPatch]
        [JsonWebTokenAuthorize]
        public HttpResponseMessage Update(string gbgId, JsonPatchDocument<CustomerInformation> customerInfo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(gbgId)) return Request.CreateResponse(HttpStatusCode.BadRequest, Constants.Messages.CustomerIdIsNull);

                if (customerInfo == null) throw new ArgumentNullException(Constants.Parameters.Customer);

                CustomerInformation customerInformation = ProcessPatchJsonToActualJson(customerInfo);

                var validationMessages = customerService.ValidateCustomerPatchRequest(customerInformation);

                if (validationMessages.Count > 0)
                {
                    var message = customerService.GetStringFrom(validationMessages);
                    Trace.TraceWarning(message);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, message);
                }

                var response = customerService.Update(gbgId, customerInformation, crmService);

                if (response.Updated)
                    return Request.CreateResponse(HttpStatusCode.NoContent, response.Id);
                if (!response.Existing)
                    return Request.CreateResponse(HttpStatusCode.NotFound, Constants.Messages.CustomerUpdateError);
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            }
            catch (Exception ex)
            {
                Trace.TraceError("Unexpected error Customer.Update::Message:{0}||Trace:{1}", ex.Message, ex.StackTrace.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        private static CustomerInformation ProcessPatchJsonToActualJson(JsonPatchDocument<CustomerInformation> customerInfo)
        {
            CustomerInformation customerInformation = new CustomerInformation();
            customerInformation.Customer.CustomerIdentity = new CustomerIdentity();
            customerInformation.Customer.CustomerIdentifier = new CustomerIdentifier();
            customerInformation.Customer.CustomerGeneral = new CustomerGeneral();
            customerInformation.Customer.Company = new Company();
            customerInformation.Customer.Permission = new Permission();
            customerInformation.Customer.Additional = new Additional();
            customerInformation.Customer.Address = new Address[3];
            customerInformation.Customer.Phone = new Phone[3];
            customerInformation.Customer.Email = new Email[3];
            customerInformation.Customer.Social = new Social[3];

            customerInfo.ApplyUpdatesTo(customerInformation);
            return customerInformation;
        }
    }
}
