using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            bookingService = new BookingService();
            crmService = new TestCrmService(context);
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