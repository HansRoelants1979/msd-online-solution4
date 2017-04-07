using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.Service.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeXrmEasy;
using Tc.Crm.Service.Services;
using Tc.Crm.ServiceTests;
using System.Net;
using System.Web.Http.Hosting;
using System.Web.Http;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Controllers.Tests
{
    [TestClass()]
    public class BookingControllerTests
    {
        XrmFakedContext context;
        IBookingService bookingService;
        BookingController controller;
        ICrmService crmService;
        Booking bookingWithNmber;

        [TestInitialize()]
        public void TestSetup()
        {
            context = new XrmFakedContext();
            
            crmService = new TestCrmService(context);
            bookingService = new BookingService(new CacheBuckets.BrandBucket(crmService)
                                                , new CacheBuckets.CountryBucket(crmService)
                                                , new CacheBuckets.CurrencyBucket(crmService)
                                                , new CacheBuckets.GatewayBucket(crmService)
                                                , new CacheBuckets.SourceMarketBucket(crmService)
                                                , new CacheBuckets.TourOperatorBucket(crmService)
                                                , new CacheBuckets.HotelBucket(crmService));
            controller = new BookingController(bookingService, crmService);
            controller.Request = new System.Net.Http.HttpRequestMessage();
            controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            bookingWithNmber = new Booking
            {
                BookingIdentifier = new BookingIdentifier
                {
                    BookingNumber = "1234",
                    SourceMarket = "DE"
                },
                BookingGeneral = new BookingGeneral
                {
                    ToCode = "TO001",
                    Destination = "DE"
                },
                Services = new BookingServices
                {
                    Accommodation = new Accommodation[]
                    {
                        new Accommodation {GroupAccommodationCode = "hot001" }
                    },
                    Transfer = new Transfer[]
                    {
                        new Transfer {ArrivalAirport="HGR",DepartureAirport="SGP" }
                    },
                    Transport = new Transport[]
                    {
                        new Transport { ArrivalAirport="HGR",DepartureAirport="SGP" }
                    }
                }
            };
        }

        [TestMethod()]
        public void BookingIsNull()
        {
            var response = controller.Update(null);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
            Assert.AreEqual(((System.Net.Http.ObjectContent)response.Content).Value, Constants.Messages.BookingDataPassedIsNullOrCouldNotBeParsed);
        }

        [TestMethod()]
        public void BookingIdentifierIsNull()
        {
            BookingInformation bookingInfo = new BookingInformation();
            Booking booking = new Booking();
            bookingInfo.Booking = booking;
            var response = controller.Update(bookingInfo);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
            Assert.AreEqual(((System.Net.Http.ObjectContent)response.Content).Value, Constants.Messages.SourceKeyNotPresent);
        }

        [TestMethod()]
        public void BookingNumberIsNull()
        {
            BookingInformation bookingInfo = new BookingInformation();
            Booking booking = new Booking();
            bookingInfo.Booking = booking;
            booking.BookingIdentifier = new BookingIdentifier();
            var response = controller.Update(bookingInfo);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
            Assert.AreEqual(((System.Net.Http.ObjectContent)response.Content).Value, Constants.Messages.SourceKeyNotPresent);
        }

        [TestMethod()]
        public void BookingCreated()
        {
            BookingInformation bookingInfo = new BookingInformation();
            bookingInfo.Booking = bookingWithNmber;
            TestCrmService service = new TestCrmService(context);
            service.Switch = DataSwitch.Created;
            controller = new BookingController(bookingService, service);
            controller.Request = new System.Net.Http.HttpRequestMessage();
            controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var response = controller.Update(bookingInfo);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
        }

        [TestMethod()]
        public void BookingUpdated()
        {
            BookingInformation bookingInfo = new BookingInformation();
            bookingInfo.Booking = bookingWithNmber;
            TestCrmService service = new TestCrmService(context);
            service.Switch = DataSwitch.Updated;
            controller = new BookingController(bookingService, service);
            controller.Request = new System.Net.Http.HttpRequestMessage();
            controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var response = controller.Update(bookingInfo);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NoContent);
        }

        [TestMethod()]
        public void ActionResponseIsNull()
        {
            BookingInformation bookingInfo = new BookingInformation();
            bookingInfo.Booking = bookingWithNmber;
            TestCrmService service = new TestCrmService(context);
            service.Switch = DataSwitch.Response_NULL;
            controller = new BookingController(bookingService, service);
            controller.Request = new System.Net.Http.HttpRequestMessage();
            controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var response = controller.Update(bookingInfo);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError);
        }

        

        [TestMethod()]
        public void ActionThrowsException()
        {
             BookingInformation bookingInfo = new BookingInformation();
            bookingInfo.Booking = bookingWithNmber;
            TestCrmService service = new TestCrmService(context);
            service.Switch = DataSwitch.ActionThrowsError;
            controller = new BookingController(bookingService, service);
            controller.Request = new System.Net.Http.HttpRequestMessage();
            controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var response = controller.Update(bookingInfo);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError);
        }


    }
}