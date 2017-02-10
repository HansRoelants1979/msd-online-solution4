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
            bookingService = new BookingService();
            crmService = new TestCrmService(context);
            controller = new BookingController(bookingService, crmService);
            controller.Request = new System.Net.Http.HttpRequestMessage();
            controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            bookingWithNmber = new Booking
            {
                BookingIdentifier = new BookingIdentifier
                {
                    BookingNumber = "1234"
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
            Booking booking = new Booking();
            var response = controller.Update(booking);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
            Assert.AreEqual(((System.Net.Http.ObjectContent)response.Content).Value, Constants.Messages.SourceKeyNotPresent);
        }

        [TestMethod()]
        public void BookingNumberIsNull()
        {
            Booking booking = new Booking();
            booking.BookingIdentifier = new BookingIdentifier();
            var response = controller.Update(booking);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
            Assert.AreEqual(((System.Net.Http.ObjectContent)response.Content).Value, Constants.Messages.SourceKeyNotPresent);
        }

        [TestMethod()]
        public void BookingCreated()
        {
            TestCrmService service = new TestCrmService(context);
            service.Switch = DataSwitch.Created;
            controller = new BookingController(bookingService, service);
            controller.Request = new System.Net.Http.HttpRequestMessage();
            controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var response = controller.Update(bookingWithNmber);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
        }

        [TestMethod()]
        public void BookingUpdated()
        {
            TestCrmService service = new TestCrmService(context);
            service.Switch = DataSwitch.Updated;
            controller = new BookingController(bookingService, service);
            controller.Request = new System.Net.Http.HttpRequestMessage();
            controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var response = controller.Update(bookingWithNmber);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NoContent);
        }

        [TestMethod()]
        public void ActionResponseIsNull()
        {
            TestCrmService service = new TestCrmService(context);
            service.Switch = DataSwitch.Response_NULL;
            controller = new BookingController(bookingService, service);
            controller.Request = new System.Net.Http.HttpRequestMessage();
            controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var response = controller.Update(bookingWithNmber);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError);
        }

        [TestMethod()]
        public void ActionResponseIsFailure()
        {
            TestCrmService service = new TestCrmService(context);
            service.Switch = DataSwitch.Response_Failed;
            controller = new BookingController(bookingService, service);
            controller.Request = new System.Net.Http.HttpRequestMessage();
            controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var response = controller.Update(bookingWithNmber);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError);
        }

        [TestMethod()]
        public void ActionThrowsException()
        {
            TestCrmService service = new TestCrmService(context);
            service.Switch = DataSwitch.ActionThrowsError;
            controller = new BookingController(bookingService, service);
            controller.Request = new System.Net.Http.HttpRequestMessage();
            controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var response = controller.Update(bookingWithNmber);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError);
        }


    }
}