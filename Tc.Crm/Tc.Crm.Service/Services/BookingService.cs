using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public class BookingService
    {
        public static Booking GetBookingFromPayload(string dataJson)
        {
            if (string.IsNullOrWhiteSpace(dataJson)) throw new ArgumentNullException(Constants.Parameters.DATA_JSON);
            return JsonConvert.DeserializeObject<Booking>(dataJson);
        }

        internal static BookingUpdateResponse Update(Booking booking)
        {
            if (booking == null) throw new ArgumentNullException(Constants.Parameters.BOOKING);

            KeyAttributeCollection keys = new KeyAttributeCollection();
            keys.Add(Constants.Crm.Contact.Fields.SOURCE_KEY, booking.Id);

            //Upsert
            Entity entity = new Entity(Constants.Crm.Booking.LOGICAL_NAME,keys);
            entity[Constants.Crm.Booking.Fields.SOURCE_KEY] = booking.Id;
            entity[Constants.Crm.Booking.Fields.TOTAL] = new Money(booking.TotalAmount);

            entity[Constants.Crm.Booking.Fields.CUSTOMER_ID] = new EntityReference(Constants.Crm.Contact.LOGICAL_NAME
                                                                                    , Constants.Crm.Contact.Fields.SOURCE_KEY
                                                                                    ,booking.CustomerId);

            var response = CrmService.Upsert(entity);
            if (response == null) throw new InvalidOperationException(Constants.Messages.RESPONSE_NULL);
            if (response.RecordCreated) return new BookingUpdateResponse { Created = true,Id = response.Target.Id.ToString() };
            return new BookingUpdateResponse { Created = false, Id = response.Target.Id.ToString() };
        }
    }
}