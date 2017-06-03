using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FakeXrmEasy;
using Tc.Crm.ServiceTests;

namespace Tc.Crm.Service.Services.Tests
{
    [TestClass()]
    public class BookingServiceTests
    {
        XrmFakedContext context;
        IBookingService bookingService;
        TestCrmService crmService;

        [TestInitialize()]
        public void TestSetup()
        {
            context = new XrmFakedContext();
            crmService = new TestCrmService(context);
            bookingService = new BookingService(
                                                new CacheBuckets.BrandBucket(crmService)
                                                , new CacheBuckets.CountryBucket(crmService)
                                                , new CacheBuckets.CurrencyBucket(crmService)
                                                , new CacheBuckets.GatewayBucket(crmService)
                                                , new CacheBuckets.SourceMarketBucket(crmService)
                                                , new CacheBuckets.TourOperatorBucket(crmService)
                                                , new CacheBuckets.HotelBucket(crmService));
            
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException),Constants.Parameters.BookingData)]
        public void BookingIsNull()
        {
            bookingService.Update(null,crmService);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException), Constants.Parameters.BookingData)]
        public void BookingIsEmpty()
        {
            bookingService.Update(string.Empty, crmService);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException), Constants.Parameters.CrmService)]
        public void CrmServiceIsNull()
        {
            bookingService.Update("some data", null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException), Constants.Messages.ResponseNull)]
        public void ResponseIsNull()
        {
            crmService.Switch = DataSwitch.Return_NULL;
            bookingService.Update("some data", crmService);
        }

    }
}