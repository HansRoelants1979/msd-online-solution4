using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Jose;
using Newtonsoft.Json;
using Tc.Usd.HostedControls.Models;

namespace Tc.Usd.HostedControls.Service
{
    public class WebServiceExchangeHelper
    {
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static string CreateJwtToken(string privateKey, JsonWebTokenPayload payloadObj)
        {
            var payload = new Dictionary<string, object>
            {
                {"iat", payloadObj.IssuedAtTime},
                {"nbf", payloadObj.NotBefore},
                {"exp", payloadObj.Expiry},
                {"jti", payloadObj.Jti},
                {"aud", payloadObj.Aud},
                {"bra", payloadObj.BranchCode},
                {"abt", payloadObj.AbtaNumber},
                {"emp", payloadObj.EmployeeId},
                {"ini", payloadObj.Initials},
                {"crt", payloadObj.CreatedBy}
            };

            var header = new Dictionary<string, object>
            {
                {"alg", "RS256"},
                {"typ", "JWT"}
            };

            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);
            return JWT.Encode(payload, rsa, JwsAlgorithm.RS256, header);
        }

        public static string GetCustomerTravelPlannerJson()
        {
            return
                "{\r\n  \"$schema\": \"http://json-schema.org/draft-04/schema#\",\r\n  \"type\": \"object\",\r\n  \"title\": \"OWRSearch\",\r\n  \"description\": \"One Web Retail Search as initiated from Dynamics 365\",\r\n  \"definitions\": {\r\n    \"address\": {\r\n      \"type\": \"object\",\r\n      \"properties\": {\r\n        \"flatNumberUnit\": {\r\n          \"type\": \"string\"\r\n        },\r\n        \"houseNumberBuilding\": {\r\n          \"type\": \"string\"\r\n        },\r\n        \"street\": {\r\n          \"type\": \"string\"\r\n        },\r\n        \"town\": {\r\n          \"type\": \"string\"\r\n        },\r\n        \"country\": {\r\n          \"type\": \"string\"\r\n        },\r\n        \"county\": {\r\n          \"type\": \"string\"\r\n        },\r\n        \"number\": {\r\n          \"type\": \"string\"\r\n        },\r\n        \"postalCode\": {\r\n          \"type\": \"string\"\r\n        }\r\n      }\r\n    },\r\n    \"customer\": {\r\n      \"type\": \"object\",\r\n      \"properties\": {\r\n        \"customerIdentifier\": {\r\n          \"type\": \"object\",\r\n          \"properties\": {\r\n            \"customerID\": {\r\n              \"type\": \"string\"\r\n            }\r\n          }\r\n        },\r\n        \"customerIdentity\": {\r\n          \"type\": \"object\",\r\n          \"properties\": {\r\n            \"salutation\": {\r\n              \"type\": \"string\"\r\n            },\r\n            \"firstName\": {\r\n              \"type\": \"string\"\r\n            },\r\n            \"middleName\": {\r\n              \"type\": \"string\"\r\n            },\r\n            \"lastName\": {\r\n              \"type\": \"string\"\r\n            },\r\n            \"birthDate\": {\r\n              \"type\": \"string\"\r\n            }\r\n          }\r\n        },\r\n        \"address\": {\r\n          \"type\": \"array\",\r\n          \"items\": {\r\n            \"$ref\": \"#/definitions/address\"\r\n          }\r\n        },\r\n        \"phone\": {\r\n          \"type\": \"array\",\r\n          \"additionalItems\": false,\r\n          \"items\": [\r\n            {\r\n              \"type\": \"object\",\r\n              \"properties\": {\r\n                \"type\": {\r\n                  \"type\": \"string\",\r\n                  \"enum\": [\r\n                    \"950000001\",\r\n                    \"950000000\",\r\n                    \"950000003\",\r\n                    \"950000002\"\r\n                  ]\r\n                },\r\n                \"number\": {\r\n                  \"type\": \"string\"\r\n                }\r\n              }\r\n            }\r\n          ]\r\n        },\r\n        \"email\": {\r\n          \"type\": \"array\",\r\n          \"additionalItems\": false,\r\n          \"items\": [\r\n            {\r\n              \"type\": \"object\",\r\n              \"properties\": {\r\n                \"type\": {\r\n                  \"type\": \"string\",\r\n                  \"enum\": [\r\n                    \"950000000\",\r\n                    \"950000001\",\r\n                    \"950000002\"\r\n                  ]\r\n                },\r\n                \"address\": {\r\n                  \"type\": \"string\"\r\n                }\r\n              }\r\n            }\r\n          ]\r\n        }\r\n      }\r\n    },        \r\n    \"room\": {\r\n      \"type\": \"object\",\r\n      \"properties\": {\r\n        \"numberOfAdults\": {\r\n          \"type\": \"number\"\r\n        },\r\n        \"numberOfChildren\": {\r\n          \"type\": \"number\"\r\n        },\r\n        \"childrensAges\": {\r\n          \"type\": \"array\",\r\n          \"items\": {\r\n            \"type\": \"string\",\r\n            \"enum\": [\r\n              \"950000000\",\r\n              \"950000001\",\r\n              \"950000002\",\r\n              \"950000003\",\r\n              \"950000004\",\r\n              \"950000005\",\r\n              \"950000006\",\r\n              \"950000007\",\r\n              \"950000008\",\r\n              \"950000009\",\r\n              \"950000010\",\r\n              \"950000011\",\r\n              \"950000012\",\r\n              \"950000013\",\r\n              \"950000014\",\r\n              \"950000015\",\r\n              \"950000016\",\r\n              \"950000017\",\r\n              \"950000018\",\r\n              \"950000019\"\r\n            ]\r\n          }\r\n        }\r\n      },\r\n      \"required\": [\r\n        \"numberOfAdults\"\r\n      ]\r\n    },\r\n    \"rooms\": {\r\n      \"type\": \"array\",\r\n      \"items\": {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n          \"location\": {\r\n            \"$ref\": \"#/definitions/room\"\r\n          }\r\n        }\r\n      }\r\n    },\r\n    \"travelPlanner\": {\r\n      \"travelPlannerId\": {\r\n        \"type\": \"string\"\r\n      },\r\n      \"consultationReference\": {\r\n        \"type\": \"string\"\r\n      },\r\n      \"departureDateFrom\": {\r\n        \"type\": \"string\"\r\n      },\r\n      \"departureDateTo\": {\r\n        \"type\": \"string\"\r\n      },\r\n      \"numberOfNights\": {\r\n        \"type\": \"number\",\r\n        \"enum\": [\r\n          \"950000000\",\r\n          \"950000021\",\r\n          \"950000020\",\r\n          \"950000001\",\r\n          \"950000012\",\r\n          \"950000005\",\r\n          \"950000006\",\r\n          \"950000007\",\r\n          \"950000008\",\r\n          \"950000009\",\r\n          \"950000010\",\r\n          \"950000002\",\r\n          \"950000011\",\r\n          \"950000013\",\r\n          \"950000014\",\r\n          \"950000015\",\r\n          \"950000016\",\r\n          \"950000017\",\r\n          \"950000003\",\r\n          \"950000004\",\r\n          \"950000022\",\r\n          \"950000023\",\r\n          \"950000024\",\r\n          \"950000025\",\r\n          \"950000026\",\r\n          \"950000018\",\r\n          \"950000027\",\r\n          \"950000028\",\r\n          \"950000029\",\r\n          \"950000030\",\r\n          \"950000031\",\r\n          \"950000032\",\r\n          \"950000033\",\r\n          \"950000034\",\r\n          \"950000035\",\r\n          \"950000036\",\r\n          \"950000019\",\r\n          \"950000037\",\r\n          \"950000038\",\r\n          \"950000039\",\r\n          \"950000040\",\r\n          \"950000041\",\r\n          \"950000042\",\r\n          \"950000043\",\r\n          \"950000044\",\r\n          \"950000045\",\r\n          \"950000046\",\r\n          \"950000047\",\r\n          \"950000048\",\r\n          \"950000049\"\r\n        ]\r\n      },\r\n      \"rooms\": {\r\n        \"$ref\": \"#/definitions/rooms\"\r\n      },\r\n      \"includedDestinations\": {\r\n        \"type\": \"array\",\r\n        \"items\": {\r\n          \"type\": \"string\"\r\n        }\r\n      },\r\n      \"excludedDestinations\": {\r\n        \"type\": \"array\",\r\n        \"items\": {\r\n          \"type\": \"string\"\r\n        }\r\n      },\r\n      \"departurePoints\": {\r\n        \"type\": \"array\",\r\n        \"items\": {\r\n          \"type\": \"string\"\r\n        }\r\n      }\r\n    },\r\n    \"properties\": {\r\n      \"owrRequest\": {\r\n        \"requestId\": {\r\n          \"type\": \"string\"\r\n        },\r\n        \"travelPlanner\": {\r\n          \"$ref\": \"#/definitions/travelPlanner\"\r\n        },\r\n        \"customer\": {\r\n          \"$ref\": \"#/definitions/customer\"\r\n        },\r\n        \"required\": [\r\n          \"requestId\",\r\n          \"customer\"\r\n        ]\r\n      }\r\n    }\r\n  }\r\n}\r\n\r\n";
        }

        public static string SendHttpRequest(string serviceUrl, string token, string data)
        {
            var request = new HttpClient();
            request.DefaultRequestHeaders.Accept.Clear();
            request.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var t = request.PostAsync(serviceUrl, new StringContent(data, Encoding.UTF8, "application/json"));
            var response = t.Result;
            var task = response.Content.ReadAsStringAsync();
            var content = task.Result;
            return content;
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

        public static Guid GetJti()
        {
            return Guid.NewGuid();
        }

        public static double GetExpiry(string expiredSeconds)
        {
            int sec;
            int.TryParse(expiredSeconds, out sec);
            return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds + sec);
        }

        public static double GetIssuedAtTime()
        {
            return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds);
        }

        public static double GetNotBeforeTime(string notBeforeSeconds)
        {
            int sec;
            int.TryParse(notBeforeSeconds, out sec);
            return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds + sec);
        }
    }
}