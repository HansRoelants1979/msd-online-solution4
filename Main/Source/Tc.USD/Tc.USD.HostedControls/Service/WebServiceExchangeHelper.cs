using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Tc.Crm.Common.Services;
using Tc.Usd.HostedControls.Models;

namespace Tc.Usd.HostedControls.Service
{
    public class WebServiceExchangeHelper
    {
        public static Dictionary<string, string> ContentToEventParams(OwrResponse response, ILogger logger)
        {
            var eventParams = new Dictionary<string, string>();
            eventParams.Add("ResponseCode", response.Definitions.OwrRequest.ResponseCode.ToString());
            eventParams.Add("ResponseMessage", response.Definitions.OwrRequest.ResponseMessage);
            eventParams.Add("LaunchUri", response.Definitions.OwrRequest.LaunchUri);

            return eventParams;
        }


        public static Dictionary<string, string> ContentToEventParamsForWebRio(WebRioResponse ssoResult)
        {
            var eventParams = new Dictionary<string, string>();
            eventParams.Add("responseCode", ssoResult.ResponseCode);
            eventParams.Add("responseMessage", ssoResult.ResponseMessage);
            eventParams.Add("webRioUrl", ssoResult.WebRioUrl);

            return eventParams;
        }

        public static WebRioResponse DeserializeWebRioSsoResponseJson(string json)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json)) return null;

                var response = new WebRioResponse();
                using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    var deSerializer = new DataContractJsonSerializer(response.GetType());
                    response = deSerializer.ReadObject(memoryStream) as WebRioResponse;
                }

                return response;
            }
            catch
            {
                return null;
            }
        }

        public static string SerializeOpenConsultationSsoRequestToJson(WebRioSsoRequest request)
        {
            if (request == null) return null;

            using (var memoryStream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(WebRioSsoRequest));
                serializer.WriteObject(memoryStream, request);
                var json = memoryStream.ToArray();
                return Encoding.UTF8.GetString(json, 0, json.Length);
            }
        }

        public static Guid GetJti()
        {
            return Guid.NewGuid();
        }

        public static OwrResponse DeserializeOwrResponseJson(string json, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            try
            {
                var response = new OwrResponse();
                using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    var deSerializer = new DataContractJsonSerializer(response.GetType());
                    response = deSerializer.ReadObject(memoryStream) as OwrResponse;
                }
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed trying to deserialize response from the server. Exception:{ex.ToString()}");
                return null;
            }
        }
    }
}