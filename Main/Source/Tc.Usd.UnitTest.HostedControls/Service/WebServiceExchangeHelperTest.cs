using System;
using System.Net.Http;
using System.Text;
using JWT;
using Tc.Usd.HostedControls.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Configuration;
using System.IO;
using Tc.Usd.HostedControls.Models;

namespace Tc.Usd.UnitTest.HostedControls.Service
{
    [TestClass()]
    public class WebServiceExchangeHelperTest
    {
        [TestMethod]
        public void CreateJwtTokenTest()
        {
            var payload = GetPayload();
            var fileName = ConfigurationManager.AppSettings["privateKeyFileName"];
            var privateKey = File.ReadAllText(fileName);
            var token = WebServiceExchangeHelper.CreateJwtToken(privateKey, payload);
            Assert.IsNotNull(token);
        }

        [TestMethod]
        public void SendHttpRequestTest()
        {
            var fileName = ConfigurationManager.AppSettings["privateKeyFileName"];
            var privateKey = File.ReadAllText(fileName);
            var payload = GetPayload();
            var token = WebServiceExchangeHelper.CreateJwtToken(privateKey,payload);
            Assert.IsNotNull(token);
            var data = WebServiceExchangeHelper.GetCustomerTravelPlannerJson();
            Assert.IsNotNull(data);
            var serviceUrl = ConfigurationManager.AppSettings["serviceUrl"];
            var content = WebServiceExchangeHelper.SendHttpRequest(serviceUrl, token, data);
        }

        [TestMethod]
        public void ValidateDecodedRequest()
        {
            var primaryPayload = GetPayload();
            var request = CreateRequest(primaryPayload);
            var token = request.Headers.Authorization.Parameter;
            var parts = token.Split(Crm.Service.Constants.Delimiters.Dot);
            Assert.AreEqual(3, parts.Length);
            var header = DecodeHeader(parts);
            var resultPayload = DecodePayload(token);

            Assert.AreEqual("RS256", header.alg.ToString());
            Assert.AreEqual("JWT", header.typ.ToString());

            Assert.AreEqual(primaryPayload.Aud, resultPayload.Aud);
            Assert.AreEqual(primaryPayload.AbtaNumber, resultPayload.AbtaNumber);
            Assert.AreEqual(primaryPayload.BranchCode, resultPayload.BranchCode);
            Assert.AreEqual(primaryPayload.CreatedBy, resultPayload.CreatedBy);
            Assert.AreEqual(primaryPayload.EmployeeId, resultPayload.EmployeeId);
            Assert.AreEqual(primaryPayload.Initials, resultPayload.Initials);

            var now = Math.Round((DateTime.UtcNow - WebServiceExchangeHelper.UnixEpoch).TotalSeconds);
            Assert.IsFalse(double.Parse(resultPayload.Expiry) < now);
            Assert.IsFalse(double.Parse(resultPayload.NotBefore) <= now);
            Assert.IsFalse(double.Parse(resultPayload.IssuedAtTime) >= Math.Round((DateTime.UtcNow - WebServiceExchangeHelper.UnixEpoch).TotalSeconds + 3));
          
        }

        #region Private
        private dynamic DecodeHeader(dynamic parts)
        {
            var header = parts[0];
            var headerJson = Encoding.UTF8.GetString(JsonWebToken.Base64UrlDecode(header));
            dynamic headerObj = JsonConvert.DeserializeObject(headerJson);
            return headerObj;
        }

        private JsonWebTokenPayload DecodePayload(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(Crm.Service.Constants.Parameters.Token);

            var parts = token.Split(Crm.Service.Constants.Delimiters.Dot);
            if (parts.Length != 3)
            {
                throw new FormatException("Token must consist from 3 delimited by dot parts");
            }

            var payLoad = parts[1];
            var payLoadJson = Encoding.UTF8.GetString(JsonWebToken.Base64UrlDecode(payLoad));
            JsonWebTokenPayload payload = JsonConvert.DeserializeObject<JsonWebTokenPayload>(payLoadJson);
            return payload;
        }

        private HttpRequestMessage CreateRequest(JsonWebTokenPayload payload)
        {
            var fileName = ConfigurationManager.AppSettings["privateKeyFileName"];
            var privateKey = File.ReadAllText(fileName);
            
            var token = WebServiceExchangeHelper.CreateJwtToken(privateKey, payload);
            HttpRequestMessage request = new HttpRequestMessage();
            request.Headers.Add("Authorization", "Bearer " + token);
            return request;
        }

        private JsonWebTokenPayload GetPayload()
        {
            var payload = new JsonWebTokenPayload
            {
                IssuedAtTime = WebServiceExchangeHelper.GetIssuedAtTime().ToString(),
                NotBefore = WebServiceExchangeHelper.GetNotBeforeTime("100").ToString(),
                Expiry = WebServiceExchangeHelper.GetExpiry("100").ToString(),
                Jti = WebServiceExchangeHelper.GetJti().ToString(),
                Aud = "CRM",
                BranchCode = "001",
                AbtaNumber = "002",
                EmployeeId = "1010",
                Initials = "IMS",
                CreatedBy = "IMS"
            };
            return payload;
        }
        #endregion
    }
}

