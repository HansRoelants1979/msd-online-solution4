using Microsoft.Xrm.Sdk;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps
{
    public static class JsonHelper
    {
        /// <summary>
        /// To serialize object to json
        /// </summary>
        /// <param name="BookingResponse"></param>
        /// <returns></returns>
        public static string SerializeJson(BookingResponse response,ITracingService trace)
        {
            if (response == null) throw new InvalidPluginExecutionException("Booking response is null;");
            trace.Trace("Processing Serialization of BookingResponse - start");
            MemoryStream memoryStream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(BookingResponse));

            serializer.WriteObject(memoryStream, response);
            byte[] json = memoryStream.ToArray();
            memoryStream.Close();
            trace.Trace("Processing Serialization of BookingResponse - end");
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }


        /// <summary>
        /// To deserialize json to object
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Booking DeSerializeJson(string json,ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("trace is null.");
            trace.Trace("Processing DeSerialization of json Payload - start");
            if (string.IsNullOrWhiteSpace(json)) throw new InvalidPluginExecutionException("json is null;");

            Booking bookingInfo = new Booking();
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer deSerializer = new DataContractJsonSerializer(bookingInfo.GetType());
                bookingInfo = deSerializer.ReadObject(memoryStream) as Booking;
            }

            trace.Trace("Processing DeSerialization of json Payload - start");

            return bookingInfo;

        }
    }
}
