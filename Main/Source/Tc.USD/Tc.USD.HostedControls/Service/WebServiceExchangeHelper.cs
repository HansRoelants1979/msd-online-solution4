using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Tc.Usd.HostedControls.Models;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;

namespace Tc.Usd.HostedControls.Service
{
    public class WebServiceExchangeHelper
    {
        public static string GetCustomerTravelPlannerJson()
        {
            return
                "{\r\n  \"$schema\": \"http://json-schema.org/draft-04/schema#\",\r\n  \"type\": \"object\",\r\n  \"title\": \"OWRSearch\",\r\n  \"description\": \"One Web Retail Search as initiated from Dynamics 365\",\r\n  \"definitions\": {\r\n    \"address\": {\r\n      \"type\": \"object\",\r\n      \"properties\": {\r\n        \"flatNumberUnit\": {\r\n          \"type\": \"string\"\r\n        },\r\n        \"houseNumberBuilding\": {\r\n          \"type\": \"string\"\r\n        },\r\n        \"street\": {\r\n          \"type\": \"string\"\r\n        },\r\n        \"town\": {\r\n          \"type\": \"string\"\r\n        },\r\n        \"country\": {\r\n          \"type\": \"string\"\r\n        },\r\n        \"county\": {\r\n          \"type\": \"string\"\r\n        },\r\n        \"number\": {\r\n          \"type\": \"string\"\r\n        },\r\n        \"postalCode\": {\r\n          \"type\": \"string\"\r\n        }\r\n      }\r\n    },\r\n    \"customer\": {\r\n      \"type\": \"object\",\r\n      \"properties\": {\r\n        \"customerIdentifier\": {\r\n          \"type\": \"object\",\r\n          \"properties\": {\r\n            \"customerID\": {\r\n              \"type\": \"string\"\r\n            }\r\n          }\r\n        },\r\n        \"customerIdentity\": {\r\n          \"type\": \"object\",\r\n          \"properties\": {\r\n            \"salutation\": {\r\n              \"type\": \"string\"\r\n            },\r\n            \"firstName\": {\r\n              \"type\": \"string\"\r\n            },\r\n            \"middleName\": {\r\n              \"type\": \"string\"\r\n            },\r\n            \"lastName\": {\r\n              \"type\": \"string\"\r\n            },\r\n            \"birthDate\": {\r\n              \"type\": \"string\"\r\n            }\r\n          }\r\n        },\r\n        \"address\": {\r\n          \"type\": \"array\",\r\n          \"items\": {\r\n            \"$ref\": \"#/definitions/address\"\r\n          }\r\n        },\r\n        \"phone\": {\r\n          \"type\": \"array\",\r\n          \"additionalItems\": false,\r\n          \"items\": [\r\n            {\r\n              \"type\": \"object\",\r\n              \"properties\": {\r\n                \"type\": {\r\n                  \"type\": \"string\",\r\n                  \"enum\": [\r\n                    \"950000001\",\r\n                    \"950000000\",\r\n                    \"950000003\",\r\n                    \"950000002\"\r\n                  ]\r\n                },\r\n                \"number\": {\r\n                  \"type\": \"string\"\r\n                }\r\n              }\r\n            }\r\n          ]\r\n        },\r\n        \"email\": {\r\n          \"type\": \"array\",\r\n          \"additionalItems\": false,\r\n          \"items\": [\r\n            {\r\n              \"type\": \"object\",\r\n              \"properties\": {\r\n                \"type\": {\r\n                  \"type\": \"string\",\r\n                  \"enum\": [\r\n                    \"950000000\",\r\n                    \"950000001\",\r\n                    \"950000002\"\r\n                  ]\r\n                },\r\n                \"address\": {\r\n                  \"type\": \"string\"\r\n                }\r\n              }\r\n            }\r\n          ]\r\n        }\r\n      }\r\n    },        \r\n    \"room\": {\r\n      \"type\": \"object\",\r\n      \"properties\": {\r\n        \"numberOfAdults\": {\r\n          \"type\": \"number\"\r\n        },\r\n        \"numberOfChildren\": {\r\n          \"type\": \"number\"\r\n        },\r\n        \"childrensAges\": {\r\n          \"type\": \"array\",\r\n          \"items\": {\r\n            \"type\": \"string\",\r\n            \"enum\": [\r\n              \"950000000\",\r\n              \"950000001\",\r\n              \"950000002\",\r\n              \"950000003\",\r\n              \"950000004\",\r\n              \"950000005\",\r\n              \"950000006\",\r\n              \"950000007\",\r\n              \"950000008\",\r\n              \"950000009\",\r\n              \"950000010\",\r\n              \"950000011\",\r\n              \"950000012\",\r\n              \"950000013\",\r\n              \"950000014\",\r\n              \"950000015\",\r\n              \"950000016\",\r\n              \"950000017\",\r\n              \"950000018\",\r\n              \"950000019\"\r\n            ]\r\n          }\r\n        }\r\n      },\r\n      \"required\": [\r\n        \"numberOfAdults\"\r\n      ]\r\n    },\r\n    \"rooms\": {\r\n      \"type\": \"array\",\r\n      \"items\": {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n          \"location\": {\r\n            \"$ref\": \"#/definitions/room\"\r\n          }\r\n        }\r\n      }\r\n    },\r\n    \"travelPlanner\": {\r\n      \"travelPlannerId\": {\r\n        \"type\": \"string\"\r\n      },\r\n      \"consultationReference\": {\r\n        \"type\": \"string\"\r\n      },\r\n      \"departureDateFrom\": {\r\n        \"type\": \"string\"\r\n      },\r\n      \"departureDateTo\": {\r\n        \"type\": \"string\"\r\n      },\r\n      \"numberOfNights\": {\r\n        \"type\": \"number\",\r\n        \"enum\": [\r\n          \"950000000\",\r\n          \"950000021\",\r\n          \"950000020\",\r\n          \"950000001\",\r\n          \"950000012\",\r\n          \"950000005\",\r\n          \"950000006\",\r\n          \"950000007\",\r\n          \"950000008\",\r\n          \"950000009\",\r\n          \"950000010\",\r\n          \"950000002\",\r\n          \"950000011\",\r\n          \"950000013\",\r\n          \"950000014\",\r\n          \"950000015\",\r\n          \"950000016\",\r\n          \"950000017\",\r\n          \"950000003\",\r\n          \"950000004\",\r\n          \"950000022\",\r\n          \"950000023\",\r\n          \"950000024\",\r\n          \"950000025\",\r\n          \"950000026\",\r\n          \"950000018\",\r\n          \"950000027\",\r\n          \"950000028\",\r\n          \"950000029\",\r\n          \"950000030\",\r\n          \"950000031\",\r\n          \"950000032\",\r\n          \"950000033\",\r\n          \"950000034\",\r\n          \"950000035\",\r\n          \"950000036\",\r\n          \"950000019\",\r\n          \"950000037\",\r\n          \"950000038\",\r\n          \"950000039\",\r\n          \"950000040\",\r\n          \"950000041\",\r\n          \"950000042\",\r\n          \"950000043\",\r\n          \"950000044\",\r\n          \"950000045\",\r\n          \"950000046\",\r\n          \"950000047\",\r\n          \"950000048\",\r\n          \"950000049\"\r\n        ]\r\n      },\r\n      \"rooms\": {\r\n        \"$ref\": \"#/definitions/rooms\"\r\n      },\r\n      \"includedDestinations\": {\r\n        \"type\": \"array\",\r\n        \"items\": {\r\n          \"type\": \"string\"\r\n        }\r\n      },\r\n      \"excludedDestinations\": {\r\n        \"type\": \"array\",\r\n        \"items\": {\r\n          \"type\": \"string\"\r\n        }\r\n      },\r\n      \"departurePoints\": {\r\n        \"type\": \"array\",\r\n        \"items\": {\r\n          \"type\": \"string\"\r\n        }\r\n      }\r\n    },\r\n    \"properties\": {\r\n      \"owrRequest\": {\r\n        \"requestId\": {\r\n          \"type\": \"string\"\r\n        },\r\n        \"travelPlanner\": {\r\n          \"$ref\": \"#/definitions/travelPlanner\"\r\n        },\r\n        \"customer\": {\r\n          \"$ref\": \"#/definitions/customer\"\r\n        },\r\n        \"required\": [\r\n          \"requestId\",\r\n          \"customer\"\r\n        ]\r\n      }\r\n    }\r\n  }\r\n}\r\n\r\n";
        }

        public static Dictionary<string, string> ContentToEventParams(string content)
        {
            dynamic ssoResult = JsonConvert.DeserializeObject(content);
            var eventParams = new Dictionary<string, string>
            {
                {"ResponseCode", ssoResult.definitions.properties.owrRequest.responseCode.ToString()},
                {"ResponseMessage", ssoResult.definitions.properties.owrRequest.responseMessage.ToString()},
                {"LaunchUri", ssoResult.definitions.properties.owrRequest.launchUri.ToString()}
            };
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
            if (string.IsNullOrWhiteSpace(json)) return null;

            WebRioResponse response = new WebRioResponse();
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer deSerializer = new DataContractJsonSerializer(response.GetType());
                response = deSerializer.ReadObject(memoryStream) as WebRioResponse;
            }
            return response;
        }

        public static string SerializeOpenConsultationSsoRequestToJson(WebRioSsoRequest request)
        {
            if (request == null) return null;

            using (var memoryStream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(WebRioSsoRequest));
                serializer.WriteObject(memoryStream, request);
                byte[] json = memoryStream.ToArray();
                return Encoding.UTF8.GetString(json, 0, json.Length);
            }
        }

        public static Guid GetJti()
        {
            return Guid.NewGuid();
        }
    }
}