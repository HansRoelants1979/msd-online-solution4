using System;
using System.Net.Http;
using System.Text;
using JWT;
using Tc.Usd.HostedControls.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Configuration;
using System.IO;
using Tc.Crm.Common.Jti.Models;
using Tc.Crm.Common.Jti.Service;
using Tc.Crm.Common.Services;

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
            var data = WebServiceExchangeHelper.GetCustomerTravelPlannerJson();
            Assert.IsNotNull(data);
            var serviceUrl = ConfigurationManager.AppSettings["serviceUrl"];
            var content = _jtiService.SendHttpRequest(serviceUrl, token, data);
        }

        [TestMethod]
        public void ValidateDecodedRequest()
        {
            
            var request = CreateRequest(_payload);
            var token = request.Headers.Authorization.Parameter;
            var parts = token.Split(Crm.Service.Constants.Delimiters.Dot);
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

        private OwrJsonWebTokenPayload DecodePayload(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(Crm.Service.Constants.Parameters.Token);

            var parts = token.Split(Crm.Service.Constants.Delimiters.Dot);
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
        #endregion
    }
}

