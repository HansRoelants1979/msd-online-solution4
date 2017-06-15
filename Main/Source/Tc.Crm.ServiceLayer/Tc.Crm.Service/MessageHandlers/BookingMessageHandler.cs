using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.MessageHandlers
{
    public class BookingMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var header = request.Headers.ToString();
            var t = request.Content.ReadAsStringAsync();
            var content = t.Result;

            Trace.TraceInformation(header);

            LogBookingNumberFromBody(content);
            return base.SendAsync(request, cancellationToken);
        }


        private void LogBookingNumberFromBody(string content)
        {
            try
            {
                if (!content.Contains("\"booking\"")) return;
                var bookingInfo = DeserializeContent(content);
                if (bookingInfo == null
                    || bookingInfo.BookingForLog == null
                    || bookingInfo.BookingForLog.BookingIdentifierForLog == null
                    || string.IsNullOrWhiteSpace(bookingInfo.BookingForLog.BookingIdentifierForLog.BookingNumberForLog)) return;
                Trace.TraceInformation(bookingInfo.BookingForLog.BookingIdentifierForLog.BookingNumberForLog);
            }
            catch (Exception)
            {
                Trace.TraceError("Error in Booking Message Handler.");
            }
           
        }

        public static BookingInformationForLog DeserializeContent(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;

            BookingInformationForLog bookingInfo = new BookingInformationForLog();
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer deSerializer = new DataContractJsonSerializer(bookingInfo.GetType());
                bookingInfo = deSerializer.ReadObject(memoryStream) as BookingInformationForLog;
            }

            return bookingInfo;

        }
    }

}