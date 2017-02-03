using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using Microsoft.Xrm.Sdk;

namespace Tc.Crm.CustomWorkflowSteps
{
    public class ProcessBooking
    {
        /// <summary>
        /// To process booking data
        /// </summary>
        /// <param name="json"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public string ProcessPayload(string json, IOrganizationService service)
        {
            string response = string.Empty;
            PayloadBooking bookingInfo = DeSerializeJson(json);

            List<SuccessMessage> successMsg = new List<SuccessMessage>();
            successMsg.Add(ProcessAccount(bookingInfo, service));
            successMsg.AddRange(ProcessAccomodation(bookingInfo, service));
            

            response = SerializeJson(successMsg);
            return response;
        }

        /// <summary>
        /// To serialize object to json
        /// </summary>
        /// <param name="successMsg"></param>
        /// <returns></returns>
        string SerializeJson(List<SuccessMessage> successMsg)
        {
            var jsonSerializer = new JavaScriptSerializer();
            var json = jsonSerializer.Serialize(successMsg);
            return json;
        }


        /// <summary>
        /// To deserialize json to object
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        PayloadBooking DeSerializeJson(string json)
        {
            var jsonSerializer = new JavaScriptSerializer();
            PayloadBooking bookingInfo = new PayloadBooking();            
            bookingInfo = jsonSerializer.Deserialize<PayloadBooking>(json);
            return bookingInfo;
        }

        /// <summary>
        /// To process customer data
        /// </summary>
        /// <param name="bookingInfo"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        SuccessMessage ProcessCustomer(PayloadBooking bookingInfo, IOrganizationService service)
        {
            CommonXrm xrm = new CommonXrm();
            Entity customerEntity = new Entity(EntityName.Contact, "", "");
            customerEntity[Attributes.Contact.FirstName] = bookingInfo.booking.TravelParticipant[0].FirstName;
           
            return xrm.UpsertEntity(customerEntity, service);
        }

        /// <summary>
        /// To process account data
        /// </summary>
        /// <param name="bookingInfo"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        SuccessMessage ProcessAccount(PayloadBooking bookingInfo, IOrganizationService service)
        {
            CommonXrm xrm = new CommonXrm();
            Entity customerEntity = new Entity(EntityName.Account, "", "");
            customerEntity[Attributes.Account.Name] = bookingInfo.booking.TravelParticipant[0].FirstName;

            return xrm.UpsertEntity(customerEntity, service);
        }


        List<SuccessMessage> ProcessAccomodation(PayloadBooking bookingInfo, IOrganizationService service)
        {
            CommonXrm xrm = new CommonXrm();
           

            return xrm.BulkCreate(null, service);
        }
    }
}
