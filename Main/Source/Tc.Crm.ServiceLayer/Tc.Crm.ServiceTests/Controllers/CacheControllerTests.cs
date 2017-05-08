using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.Service.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeXrmEasy;
using Tc.Crm.ServiceTests;
using Tc.Crm.Service.Services;

namespace Tc.Crm.Service.Controllers.Tests
{

//    [TestClass()]
//    public class CacheControllerTests
//    {
//        XrmFakedContext context;
//        CacheController controller;
//        ICrmService crmService;
//        CacheBuckets.BrandBucket brandBucket;
//        CacheBuckets.CountryBucket countryBucket;
//        CacheBuckets.CurrencyBucket currencyBucket;
//        CacheBuckets.GatewayBucket gatewayBucket;
//        CacheBuckets.SourceMarketBucket sourceMarketBucket;
//        CacheBuckets.TourOperatorBucket tourOperatorBucket;
//        CacheBuckets.HotelBucket hotelBucket;

//        [TestInitialize()]
//        public void TestSetup()
//        {
//            context = new XrmFakedContext();

//            crmService = new TestCrmService(context);
//            brandBucket = new CacheBuckets.BrandBucket(crmService);
//            countryBucket = new CacheBuckets.CountryBucket(crmService);
//            currencyBucket = new CacheBuckets.CurrencyBucket(crmService);
//            gatewayBucket = new CacheBuckets.GatewayBucket(crmService);
//            sourceMarketBucket = new CacheBuckets.SourceMarketBucket(crmService);
//            tourOperatorBucket = new CacheBuckets.TourOperatorBucket(crmService);
//            hotelBucket = new CacheBuckets.HotelBucket(crmService);

//            brandBucket.Items = null;
//            currencyBucket.Items = null;
//            countryBucket.Items = null;
//            gatewayBucket.Items = null;
//            sourceMarketBucket.Items = null;
//            tourOperatorBucket.Items = null;
//            hotelBucket.Items = null;

//            controller = new CacheController(brandBucket
//                                                , countryBucket
//                                                , currencyBucket
//                                                , gatewayBucket
//                                                , sourceMarketBucket
//                                                , tourOperatorBucket
//                                                , hotelBucket);

//        }

//        [TestMethod()]
//        public void InputHasKeys()
//        {
//            controller.Refresh(new Models.Payload { Bucket = "BRAND",JWTToken=""});
//            Assert.IsNotNull(brandBucket.Items);
//            Assert.IsNotNull(currencyBucket.Items);
//            Assert.IsNotNull(countryBucket.Items);
//            Assert.IsNotNull(gatewayBucket.Items);
//            Assert.IsNotNull(sourceMarketBucket.Items);
//            Assert.IsNotNull(tourOperatorBucket.Items);
//            Assert.IsNotNull(hotelBucket.Items);
//        }

//        [TestMethod()]
//        public void InputHasEmptyStringArray()
//        {
//            controller.Refresh(new string[] { });
//            Assert.IsNull(brandBucket.Items);
//            Assert.IsNull(currencyBucket.Items);
//            Assert.IsNull(countryBucket.Items);
//            Assert.IsNull(gatewayBucket.Items);
//            Assert.IsNull(sourceMarketBucket.Items);
//            Assert.IsNull(tourOperatorBucket.Items);
//            Assert.IsNull(hotelBucket.Items);
//        }

//        [TestMethod()]
//        public void InputIsNull()
//        {
//            controller.Refresh(new string[] { });
//            Assert.IsNull(brandBucket.Items);
//            Assert.IsNull(currencyBucket.Items);
//            Assert.IsNull(countryBucket.Items);
//            Assert.IsNull(gatewayBucket.Items);
//            Assert.IsNull(sourceMarketBucket.Items);
//            Assert.IsNull(tourOperatorBucket.Items);
//            Assert.IsNull(hotelBucket.Items);
//        }


//    }
}