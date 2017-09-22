using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models; 
using Tc.Crm.CustomWorkflowSteps.ProcessSurvey.Models;

namespace Tc.Crm.CustomWorkflowSteps
{
    public static class JsonHelper
    {
        /// <summary>
        /// To serialize object to json
        /// </summary>
        /// <param name="BookingResponse"></param>
        /// <returns></returns>
        public static string SerializeJson(BookingResponse response, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("trace is null.");
            if (response == null) throw new InvalidPluginExecutionException("Booking response is null;");
            trace.Trace("Processing Serialization of BookingResponse - start");
            using (var memoryStream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(BookingResponse));
                serializer.WriteObject(memoryStream, response);
                byte[] json = memoryStream.ToArray();
                trace.Trace("Processing Serialization of BookingResponse - end");
                return Encoding.UTF8.GetString(json, 0, json.Length);
            }
        }


        /// <summary>
        /// To deserialize json to object
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Booking DeserializeJson(string json, ITracingService trace)
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

        public static string SerializeCustomerJson(XrmUpdateResponse response, ITracingService trace){
            if (trace == null) throw new InvalidPluginExecutionException("trace is null;");
            if (response == null) throw new InvalidPluginExecutionException("CustomerResponse is null;");
            trace.Trace("Processing Serialization of CustomerResponse - start");
            using (var memoryStream = new MemoryStream()){
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(XrmUpdateResponse));
                serializer.WriteObject(memoryStream, response);
                byte[] json = memoryStream.ToArray();
                trace.Trace("Processing Serialization of CustomerResponse - end");
                return Encoding.UTF8.GetString(json, 0, json.Length);
            }
        }

        public static Customer DeserializeCustomerJson(string json, ITracingService trace){            
            if (trace == null) throw new InvalidPluginExecutionException("trace is null.");
            trace.Trace("Processing DeSerialization of customer json Payload - start");
            if (string.IsNullOrWhiteSpace(json)) throw new InvalidPluginExecutionException("json is null;");
            Customer customer = new Customer();
            customer.CustomerIdentifier = new CustomerIdentifier();
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json))){
                DataContractJsonSerializer deSerializer = new DataContractJsonSerializer(customer.GetType());
                customer = deSerializer.ReadObject(memoryStream) as Customer;
            }
            trace.Trace("Processing DeSerialization of customer json Payload - end");
            return customer;
        }

        /// <summary>
        /// To deserialize json to object
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Survey DeserializeSurveyJson(string json, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("trace is null.");
            trace.Trace("Processing DeSerialization of json survey Payload - start");
            if (string.IsNullOrWhiteSpace(json)) throw new InvalidPluginExecutionException("json is null;");

            Survey survey = new Survey();
            survey.Responses = new List<Response>();
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer deSerializer = new DataContractJsonSerializer(survey.Responses.GetType());
                survey.Responses = deSerializer.ReadObject(memoryStream) as List<Response>;
            }
            trace.Trace("Processing DeSerialization of json survey Payload - start");

            return survey;

        }

        public static string SerializeSurveyJson(SurveyReturnResponse response, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("trace is null;");
            if (response == null) throw new InvalidPluginExecutionException("SurveyReturnResponse is null;");
            trace.Trace("Processing Serialization of SurveyReturnResponse - start");
            using (var memoryStream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(SurveyReturnResponse));

                serializer.WriteObject(memoryStream, response);
                byte[] json = memoryStream.ToArray();
                trace.Trace("Processing Serialization of SurveyReturnResponse - end");
                return Encoding.UTF8.GetString(json, 0, json.Length);
            }
        }
    }
}
