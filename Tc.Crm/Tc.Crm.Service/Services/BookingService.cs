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

        public static void Create(Booking b)
        {
            Entity e = new Entity("new_booking");
            e["new_sourcekey"] = b.Id;
            e["new_total"] = new Money(b.TotalAmount);
            if (!string.IsNullOrWhiteSpace(b.CrmCustomerKey))
                e["new_customerid"] = new EntityReference("contact", new Guid(b.CrmCustomerKey));
            var id = CrmService.Create(e);
            b.CrmKey = id.ToString();
        }

        internal static BookingUpdateResponse Update(Booking b)
        {
            //check if customer key is present
            if (!string.IsNullOrEmpty(b.CustomerId))
            {
                //check in CRM if customer is present
                var customerId = CrmService.RetrieveBy("new_sourcekey", b.CustomerId, "contact", "contactid");

                if (customerId != null)
                    b.CrmCustomerKey = customerId.Value.ToString();
            }

            if (!string.IsNullOrEmpty(b.Id))
            {
                //check if the booking exists in CRM
                if (string.IsNullOrEmpty(b.CrmKey))
                {
                    var bookingId = CrmService.RetrieveBy("new_sourcekey", b.Id, "new_booking", "new_bookingid");
                    if (bookingId == null)
                    {
                        Create(b);
                        return new BookingUpdateResponse { Created = true };
                    }
                    b.CrmKey = bookingId.Value.ToString();
                }
                Entity e = new Entity("new_booking");
                e.Id = new Guid(b.CrmKey);
                e["new_sourcekey"] = b.Id;
                e["new_total"] = new Money(b.TotalAmount);

                if (!string.IsNullOrWhiteSpace(b.CrmCustomerKey))
                    e["new_customerid"] = new EntityReference("contact", new Guid(b.CrmCustomerKey));


                CrmService.Update(e);
                return new BookingUpdateResponse { Created = false };
            }
            else
            {
                Create(b);
                return new BookingUpdateResponse { Created = true };
            }
        }
    }
}