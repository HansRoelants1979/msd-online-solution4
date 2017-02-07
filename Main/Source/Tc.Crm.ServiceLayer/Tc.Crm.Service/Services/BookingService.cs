using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public class BookingService : IBookingService
    {
        public BookingUpdateResponse Update(string bookingData, ICrmService crmService)
        {
            if (string.IsNullOrWhiteSpace(bookingData)) throw new ArgumentNullException(Constants.Parameters.BookingData);
            if (crmService == null) throw new ArgumentNullException(Constants.Parameters.CrmService);

            var response = crmService.ExecuteActionForBookingUpdate(bookingData);
            if (response == null) throw new InvalidOperationException(Constants.Messages.ResponseNull);
            if (!response.Success)
                throw new InvalidOperationException(response.ErrorMessage);
            if (response.Created) return new BookingUpdateResponse { Created = true, Id = response.Id.ToString() };
            return new BookingUpdateResponse { Created = false, Id = response.Id.ToString() };
        }

        public bool DataValid(string data)
        {
            var schema =JSchema.Parse(Constants.General.BookingJsonSchema);
            JObject bookingObject = JObject.Parse(data);
            IList<string> messages;
            StringBuilder messageBuilder = new StringBuilder();
            bool valid = bookingObject.IsValid(schema, out messages);
            if(messages != null && messages.Count>0)
            {
                foreach (var item in messages)
                {
                    messageBuilder.AppendLine(item);
                }
            }
            Trace.TraceError("Error while validating booking json. Error:{0}",messageBuilder.ToString());
            return valid;
        }
    }
}