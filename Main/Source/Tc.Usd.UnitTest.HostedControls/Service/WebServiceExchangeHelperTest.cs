using System;
using System.Net.Http;
using System.Text;
using JWT;
using Tc.Usd.HostedControls.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using Tc.Crm.Common.IntegrationLayer.Jti.Models;
using Tc.Crm.Common.IntegrationLayer.Jti.Service;
using Tc.Crm.Common.Services;
using Tc.Crm.Service.Constants;
using HttpMethod = Tc.Crm.Common.HttpMethod;

namespace Tc.Usd.UnitTest.HostedControls.Service
{
    [TestClass()]
    public class WebServiceExchangeHelperTest
    {
        private JwtService _jtiService;
        private OwrJsonWebTokenPayload _payload;

        [TestInitialize]
        public void Setup()
        {
            _jtiService = new JwtService(new Logger());
            _payload =  new OwrJsonWebTokenPayload
            {
                IssuedAtTime = _jtiService.GetIssuedAtTime().ToString(),
                NotBefore = _jtiService.GetNotBeforeTime("100").ToString(),
                Expiry = _jtiService.GetExpiry("100").ToString(),
                Jti = WebServiceExchangeHelper.GetJti().ToString(),
                Aud = "CRM",
                BranchCode = "001",
                AbtaNumber = "002",
                EmployeeId = "1010",
                Initials = "IMS",
                CreatedBy = "IMS"
            };
        }

        [TestMethod]
        public void CreateJwtTokenTest()
        {
            var fileName = ConfigurationManager.AppSettings["privateKeyFileName"];
            var privateKey = File.ReadAllText(fileName);
            var token = _jtiService.CreateJwtToken(privateKey, _payload);
            Assert.IsNotNull(token);
        }

        [TestMethod]
        public void SendHttpRequestTest()
        {
            var fileName = ConfigurationManager.AppSettings["privateKeyFileName"];
            var privateKey = File.ReadAllText(fileName);
            
            var token = _jtiService.CreateJwtToken(privateKey, _payload);
            Assert.IsNotNull(token);
            var data = GetCustomerTravelPlannerJson();
            Assert.IsNotNull(data);
            var serviceUrl = ConfigurationManager.AppSettings["serviceUrl"];
            var content = _jtiService.SendHttpRequest(HttpMethod.Post, serviceUrl, token, data);
        }
        
        [TestMethod]
        public void ValidateDecodedRequest()
        {
            
            var request = CreateRequest(_payload);
            var token = request.Headers.Authorization.Parameter;
            var parts = token.Split(Delimiters.Dot);
            Assert.AreEqual(3, parts.Length);
            var header = DecodeHeader(parts);
            var resultPayload = DecodePayload(token);

            Assert.AreEqual("RS256", header.alg.ToString());
            Assert.AreEqual("JWT", header.typ.ToString());

            Assert.AreEqual(_payload.Aud, resultPayload.Aud);
            Assert.AreEqual(_payload.AbtaNumber, resultPayload.AbtaNumber);
            Assert.AreEqual(_payload.BranchCode, resultPayload.BranchCode);
            Assert.AreEqual(_payload.CreatedBy, resultPayload.CreatedBy);
            Assert.AreEqual(_payload.EmployeeId, resultPayload.EmployeeId);
            Assert.AreEqual(_payload.Initials, resultPayload.Initials);

            var now = Math.Round((DateTime.UtcNow - _jtiService.UnixEpoch).TotalSeconds);
            Assert.IsFalse(Double.Parse(resultPayload.Expiry) < now);
            Assert.IsFalse(Double.Parse(resultPayload.NotBefore) <= now);
            Assert.IsFalse(Double.Parse(resultPayload.IssuedAtTime) >= Math.Round((DateTime.UtcNow - _jtiService.UnixEpoch).TotalSeconds + 3));
          
        }

        #region Private
        private dynamic DecodeHeader(dynamic parts)
        {
            var header = parts[0];
            var headerJson = Encoding.UTF8.GetString(JsonWebToken.Base64UrlDecode(header));
            dynamic headerObj = JsonConvert.DeserializeObject(headerJson);
            return headerObj;
        }

        private OwrJsonWebTokenPayload DecodePayload(string token)
        {
            if (String.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(Parameters.Token);

            var parts = token.Split(Delimiters.Dot);
            if (parts.Length != 3)
            {
                throw new FormatException("Token must consist from 3 delimited by dot parts");
            }

            var payLoad = parts[1];
            var payLoadJson = Encoding.UTF8.GetString(JsonWebToken.Base64UrlDecode(payLoad));
            OwrJsonWebTokenPayload payload = JsonConvert.DeserializeObject<OwrJsonWebTokenPayload>(payLoadJson);
            return payload;
        }

        private HttpRequestMessage CreateRequest(OwrJsonWebTokenPayload payload)
        {
            var fileName = ConfigurationManager.AppSettings["privateKeyFileName"];
            var privateKey = File.ReadAllText(fileName);
            
            var token = _jtiService.CreateJwtToken(privateKey, payload);
            HttpRequestMessage request = new HttpRequestMessage();
            request.Headers.Add("Authorization", "Bearer " + token);
            return request;
        }

        private static string GetCustomerTravelPlannerJson()
        {
            return
                "{\r\n  \"owrRequest\": {\r\n    \"requestId\": \"0c22ab95-e510-4fc7-8b9d-0c008c6ed354\",\r\n    \"travelPlanner\": {\r\n      \"travelPlannerId\": \"0c22ab95-e510-4fc7-8b9d-0c008c6ed352\",\r\n      \"consultationReference\": \"29134000000001\",\r\n      \"departureDateFrom\": \"2017-10-28\",\r\n      \"departureDateTo\": \"2017-11-15\",\r\n      \"numberOfNights\": \"950000001\",\r\n      \"rooms\": [\r\n        {\r\n          \"numberOfAdults\": \"2\",\r\n          \"numberOfChildren\": \"1\",\r\n          \"childrensAges\": [\r\n            \"950000005\"\r\n          ]\r\n        },\r\n        {\r\n          \"numberOfAdults\": \"2\",\r\n          \"numberOfChildren\": \"2\",\r\n          \"childrensAges\": [\r\n            \"950000005\",\r\n            \"950000007\"\r\n          ]\r\n        }\r\n      ],\r\n      \"includedDestinations\": [\r\n        \"TUR0428899\",\r\n        \"950000006\"\r\n      ],\r\n      \"excludedDestinations\": [\r\n        \"CPV2588899\",\r\n        \"QM\"\r\n      ]\r\n    },\r\n    \"customer\": {\r\n      \"customerIdentifier\": {\r\n        \"customerId\": \"100684534\",\r\n        \"businessArea\": \"0\"\r\n      },      \r\n      \"customerIdentity\": {\r\n        \"salutation\": \"Mr\",\r\n        \"firstName\": \"John\",\r\n        \"middleName\": \"\",\r\n        \"lastName\": \"Faltin\",\r\n        \"birthDate\": \"1981-05-11\"\r\n\r\n      },\r\n      \"address\": [\r\n        {\r\n          \"flatNumberUnit\": \"\",\r\n          \"houseNumberBuilding\": \"Herrenbergstr.\",\r\n          \"street\": \"\",\r\n          \"town\": \"\",\r\n          \"country\": \"GB\",\r\n          \"county\": \"Rotenburg\",\r\n          \"number\": \"2a\",\r\n          \"postalCode\": \"\"\r\n        }\r\n      ],\r\n      \"phone\": [\r\n        {\r\n          \"number\": \"01642281491\",\r\n          \"type\": \"950000001\"\r\n        },\r\n        {\r\n          \"number\": \"07747611545\",\r\n          \"type\": \"950000000\"\r\n        }\r\n      ],\r\n      \"email\": [\r\n        {\r\n          \"address\": \"jonesj@hcl.com\",\r\n          \"type\": \"950000001\"\r\n        },\r\n        {\r\n          \"address\": \"john.jones@thomascook.com\",\r\n          \"type\": \"950000000\"\r\n        }\r\n      ]\r\n    }\r\n  }\r\n}\r\n";
        }

        #endregion
    }
}

