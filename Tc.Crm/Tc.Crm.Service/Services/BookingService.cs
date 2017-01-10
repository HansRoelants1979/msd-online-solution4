using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public class BookingService
    {
        public static Booking GetBookingFromPayload(string dataJson)
        {
            return JsonConvert.DeserializeObject<Booking>(dataJson);
        }

        internal static BookingUpdateResponse Update(Booking booking)
        {
            //get the customer from CRM
            GetCustomerFor(booking);

            KeyAttributeCollection keys = new KeyAttributeCollection();
            keys.Add(Constants.Crm.Contact.Fields.SOURCE_KEY, booking.Id);

            //Upsert
            Entity entity = new Entity(Constants.Crm.Booking.LOGICAL_NAME,keys);
            entity[Constants.Crm.Booking.Fields.SOURCE_KEY] = booking.Id;
            entity[Constants.Crm.Booking.Fields.TOTAL] = new Money(booking.TotalAmount);

            if (!string.IsNullOrWhiteSpace(booking.CrmCustomerKey))
                entity[Constants.Crm.Booking.Fields.CUSTOMER_ID] = new EntityReference(Constants.Crm.Contact.LOGICAL_NAME, new Guid(booking.CrmCustomerKey));

            var response = CrmService.Upsert(entity);
            if (response.RecordCreated) return new BookingUpdateResponse { Created = true };
            return new BookingUpdateResponse { Created = false };
        }

        private static void GetCustomerFor(Booking booking)
        {
            if (!string.IsNullOrEmpty(booking.CustomerId))
            {
                //check in CRM if customer is present
                var customerId = CrmService.RetrieveBy(Constants.Crm.Contact.Fields.SOURCE_KEY
                                                        , booking.CustomerId
                                                        , Constants.Crm.Contact.LOGICAL_NAME
                                                        , Constants.Crm.Contact.Fields.CONTACT_ID);

                if (customerId != null)
                    booking.CrmCustomerKey = customerId.Value.ToString();
            }
        }
    }
}