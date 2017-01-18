using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public static class BookingService
    {
        internal static BookingUpdateResponse Update(Booking booking)
        {
            if (booking == null) throw new ArgumentNullException(Constants.Parameters.Booking);

            KeyAttributeCollection keys = new KeyAttributeCollection();
            keys.Add(Constants.Crm.Fields.Booking.SourceKey, booking.Id);

            //Upsert
            Entity entity = new Entity(Constants.Crm.Booking.LogicalName,keys);
            entity[Constants.Crm.Fields.Booking.SourceKey] = booking.Id;
            entity[Constants.Crm.Fields.Booking.Total] = new Money(booking.TotalAmount);

            entity[Constants.Crm.Fields.Booking.CustomerId] = new EntityReference(Constants.Crm.Contact.LogicalName
                                                                                    , Constants.Crm.Fields.Contact.SourceKey
                                                                                    ,booking.CustomerId);

            var response = CrmService.Upsert(entity);
            if (response == null) throw new InvalidOperationException(Constants.Messages.ResponseNull);
            if (response.RecordCreated) return new BookingUpdateResponse { Created = true,Id = response.Target.Id.ToString() };
            return new BookingUpdateResponse { Created = false, Id = response.Target.Id.ToString() };
        }
    }
}