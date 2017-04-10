using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Configuration;
using System.IO;
using Tc.Crm.ServiceTests;

namespace Tc.Crm.Service.Services.Tests
{
    [TestClass()]
    public class JsonWebTokenHelperTests
    {
        TestConfigurationService configurationService;
        bool issuedAtTimeInvalid = false;
        bool nbfInvalid = false;
        bool expInvalid = false;
        JsonWebTokenHelper helper;
        [TestInitialize()]
        public void TestSetup()
        {
            configurationService = new TestConfigurationService();
            helper = new JsonWebTokenHelper(configurationService);
        }

        [TestMethod()]
        public void RequestObjectIsNull()
        {

            var tokenRequest = helper.GetRequestObject(null);
            Assert.IsNotNull(tokenRequest.Errors);
            Assert.AreEqual(2, tokenRequest.Errors.Count);
            Assert.AreEqual(Constants.Messages.JsonWebTokenParserError, tokenRequest.Errors[1].Message);
        }

        [TestMethod()]
        public void TokenNotSeparatedBy3Parts()
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Headers.Add("Authorization", "Bearer abc");
            var tokenRequest = helper.GetRequestObject(request);
            Assert.IsNotNull(tokenRequest.Errors);
            Assert.AreEqual(2, tokenRequest.Errors.Count);
            Assert.AreEqual(Constants.Messages.JsonWebTokenParserError, tokenRequest.Errors[1].Message);
            Assert.AreEqual(Constants.Messages.TokenFormatError, tokenRequest.Errors[0].Message);
        }

        [TestMethod()]
        public void TokenIsNull()
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Headers.Add("Authorization", "Bearer ");
            var tokenRequest = helper.GetRequestObject(request);
            Assert.IsNotNull(tokenRequest.Errors);
            Assert.AreEqual(2, tokenRequest.Errors.Count);
            Assert.AreEqual(Constants.Messages.JsonWebTokenParserError, tokenRequest.Errors[1].Message);
            Assert.AreEqual("Value cannot be null.\r\nParameter name: token", tokenRequest.Errors[0].Message);
        }

        [TestMethod()]
        public void TokenHasInvalidHeader()
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Headers.Add("Authorization", "Bearer a.b.c");
            var tokenRequest = helper.GetRequestObject(request);
            Assert.IsNotNull(tokenRequest.Errors);
            Assert.AreEqual(2, tokenRequest.Errors.Count);
            Assert.AreEqual(Constants.Messages.JsonWebTokenParserError, tokenRequest.Errors[1].Message);
            Assert.AreEqual("Illegal base64url string!", tokenRequest.Errors[0].Message);
        }

        [TestMethod()]
        public void PositiveScenario()
        {
            configurationService.CorrectSignaure = true;
            HttpRequestMessage request = new HttpRequestMessage();
            var token = CreateJWTToken();
            request.Headers.Add("Authorization", "Bearer " + token);
            var tokenRequest = helper.GetRequestObject(request);
            Assert.AreEqual(0, tokenRequest.Errors.Count);
            Assert.AreEqual(true, tokenRequest.HeaderAlgorithmValid);
            Assert.AreEqual(true, tokenRequest.HeaderTypeValid);
            Assert.AreEqual(true, tokenRequest.IssuedAtTimeValid);
            Assert.AreEqual(true, tokenRequest.NotBeforetimeValid);
            Assert.AreEqual(true, tokenRequest.SignatureValid);
            Assert.AreEqual(true, tokenRequest.ExpiryValid);
        }

        [TestMethod()]
        public void SignatureInvalid()
        {
            configurationService.CorrectSignaure = false;
            HttpRequestMessage request = new HttpRequestMessage();
            var token = CreateJWTToken();
            request.Headers.Add("Authorization", "Bearer " + token);
            var tokenRequest = helper.GetRequestObject(request);
            Assert.AreEqual(false, tokenRequest.SignatureValid);
        }

        [TestMethod()]
        public void IssuedAtTimeInvalid()
        {
            issuedAtTimeInvalid = true;
            configurationService.CorrectSignaure = false;
            HttpRequestMessage request = new HttpRequestMessage();
            var token = CreateJWTToken();
            request.Headers.Add("Authorization", "Bearer " + token);
            var tokenRequest = helper.GetRequestObject(request);
            Assert.AreEqual(false, tokenRequest.IssuedAtTimeValid);
            issuedAtTimeInvalid = false;
        }

        [TestMethod()]
        public void NBFInvalid()
        {
            nbfInvalid = true;
            configurationService.CorrectSignaure = false;
            HttpRequestMessage request = new HttpRequestMessage();
            var token = CreateJWTToken();
            request.Headers.Add("Authorization", "Bearer " + token);
            var tokenRequest = helper.GetRequestObject(request);
            Assert.AreEqual(false, tokenRequest.NotBeforetimeValid);
            nbfInvalid = false;
        }

        [TestMethod()]
        public void ExpiryInvalid()
        {
            expInvalid = true;
            configurationService.CorrectSignaure = false;
            HttpRequestMessage request = new HttpRequestMessage();
            var token = CreateJWTToken();
            request.Headers.Add("Authorization", "Bearer " + token);
            var tokenRequest = helper.GetRequestObject(request);
            Assert.AreEqual(false, tokenRequest.ExpiryValid);
            expInvalid = false;
        }


        #region Helper Functions
        private string CreateJWTToken()
        {
            var payload = new Dictionary<string, object>()
            {
                {"iss", "TC"},
                {"aud", "CRM"},
                {"sub", "anonymous"},
                {"iat", GetIssuedAtTime().ToString()},
                {"nbf", GetNotBeforeTime().ToString()},
                {"exp", GetExpiryTime().ToString()},
            };

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            var fileName = ConfigurationManager.AppSettings["privateKeyFileName"];
            rsa.FromXmlString(File.ReadAllText(fileName));
            return Jose.JWT.Encode(payload, rsa, Jose.JwsAlgorithm.RS256);

        }
        private readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        private double GetIssuedAtTime()
        {
            if (issuedAtTimeInvalid)
                return (Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds) + 1805);
            else
                return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds);
        }
        private double GetNotBeforeTime()
        {
            if(nbfInvalid)
                return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds + 10);
            else
            return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds);
        }

        private double GetExpiryTime()
        {
            if (expInvalid)
                return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds - 10);
            else
                return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds + 50);
        }
        #endregion
    }
}