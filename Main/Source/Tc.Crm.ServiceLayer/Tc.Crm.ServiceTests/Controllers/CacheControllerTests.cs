using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeXrmEasy;
using Tc.Crm.ServiceTests;
using Tc.Crm.Service.Services;
using Jose;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System.Net;
using System;
using System.Collections.Generic;

namespace Tc.Crm.Service.Controllers.Tests
{

    [TestClass()]
    public class CacheControllerTests
    {
        XrmFakedContext context;
        CacheController controller;
        ICrmService crmService;
        ICachingService cachingService;
        TestConfigurationService configurationService;
        CacheBuckets.BrandBucket brandBucket;
        CacheBuckets.CountryBucket countryBucket;
        CacheBuckets.CurrencyBucket currencyBucket;
        CacheBuckets.GatewayBucket gatewayBucket;
        CacheBuckets.SourceMarketBucket sourceMarketBucket;
        CacheBuckets.TourOperatorBucket tourOperatorBucket;
        CacheBuckets.HotelBucket hotelBucket;

        [TestInitialize()]
        public void TestSetup()
        {
            context = new XrmFakedContext();
            configurationService = new TestConfigurationService();
            crmService = new TestCrmService(context);
            
            brandBucket = new CacheBuckets.BrandBucket(crmService);
            countryBucket = new CacheBuckets.CountryBucket(crmService);
            currencyBucket = new CacheBuckets.CurrencyBucket(crmService);
            gatewayBucket = new CacheBuckets.GatewayBucket(crmService);
            sourceMarketBucket = new CacheBuckets.SourceMarketBucket(crmService);
            tourOperatorBucket = new CacheBuckets.TourOperatorBucket(crmService);
            hotelBucket = new CacheBuckets.HotelBucket(crmService);

            cachingService = new CachingService(brandBucket
                                                , countryBucket
                                                , currencyBucket
                                                , gatewayBucket
                                                , sourceMarketBucket
                                                , tourOperatorBucket
                                                , hotelBucket);

            controller = new CacheController(cachingService, configurationService);

        }

        [TestMethod()]
        public void CacheBrand()
        {
            ClearBuckets();
            var response = controller.Refresh(new Models.Payload { Bucket = "BRAND", JWTToken = CreateJWTToken() });
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(brandBucket.Items);
            Assert.AreEqual(1,brandBucket.Items.Count);
        }

        [TestMethod()]
        public void CacheGateway()
        {
            ClearBuckets();
            var response = controller.Refresh(new Models.Payload { Bucket = "GATEWAY", JWTToken = CreateJWTToken() });
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(gatewayBucket.Items);
            Assert.AreEqual(1, gatewayBucket.Items.Count);
        }
        [TestMethod()]
        public void CacheCurrency()
        {
            ClearBuckets();
            var response = controller.Refresh(new Models.Payload { Bucket = "CURRENCY", JWTToken = CreateJWTToken() });
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(currencyBucket.Items);
            Assert.AreEqual(1, currencyBucket.Items.Count);
        }
        [TestMethod()]
        public void CacheCountry()
        {
            ClearBuckets();
            var response = controller.Refresh(new Models.Payload { Bucket = "COUNTRY", JWTToken = CreateJWTToken() });
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(countryBucket.Items);
            Assert.AreEqual(1, countryBucket.Items.Count);
        }
        [TestMethod()]
        public void CacheSourceMarket()
        {
            ClearBuckets();
            var response = controller.Refresh(new Models.Payload { Bucket = "SOURCEMARKET", JWTToken = CreateJWTToken() });
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(sourceMarketBucket.Items);
            Assert.AreEqual(1, sourceMarketBucket.Items.Count);
        }
        [TestMethod()]
        public void CacheTourOperator()
        {
            ClearBuckets();
            var response = controller.Refresh(new Models.Payload { Bucket = "TOUROPERATOR", JWTToken = CreateJWTToken() });
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(tourOperatorBucket.Items);
            Assert.AreEqual(1, tourOperatorBucket.Items.Count);
        }

        [TestMethod()]
        public void CacheHotel()
        {
            ClearBuckets();
            var response = controller.Refresh(new Models.Payload { Bucket = "HOTEL", JWTToken = CreateJWTToken() });
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(hotelBucket.Items);
            Assert.AreEqual(1, hotelBucket.Items.Count);
        }

        private string CreateJWTToken()
        {
            var secretKey = configurationService.GetSecretKey();
            var payload = new Dictionary<string, object>()
            {
                {"iat", GetIssuedAtTime().ToString()},
                {"nbf", GetNotBeforeTime().ToString()},
                {"exp", GetExpiry().ToString()}
            };

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            return encoder.Encode(payload, secretKey);
        }

        private double GetExpiry()
        {

            var sec = 100;
                return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds + sec);
           
        }

        private double GetIssuedAtTime()
        {

            var sec = 100;
            return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds + sec);
           
        }
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        private double GetNotBeforeTime()
        {

            var sec = -1;
                return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds + sec);
           
        }


        private void ClearBuckets()
        {
            brandBucket.Items = null;
            currencyBucket.Items = null;
            countryBucket.Items = null;
            gatewayBucket.Items = null;
            sourceMarketBucket.Items = null;
            tourOperatorBucket.Items = null;
            hotelBucket.Items = null;
        }

        [TestMethod()]
        public void PayloadHasInvalidCacheBucket()
        {
            ClearBuckets();
            controller.Refresh(new Models.Payload { Bucket = "BRAND1", JWTToken = CreateJWTToken() });
            Assert.IsNull(brandBucket.Items);
            Assert.IsNull(currencyBucket.Items);
            Assert.IsNull(countryBucket.Items);
            Assert.IsNull(gatewayBucket.Items);
            Assert.IsNull(sourceMarketBucket.Items);
            Assert.IsNull(tourOperatorBucket.Items);
            Assert.IsNull(hotelBucket.Items);
        }

        [TestMethod()]
        public void InputIsNull()
        {
            var response = controller.Refresh(null);
            Assert.AreEqual(HttpStatusCode.BadRequest,response.StatusCode);
        }


    }
}