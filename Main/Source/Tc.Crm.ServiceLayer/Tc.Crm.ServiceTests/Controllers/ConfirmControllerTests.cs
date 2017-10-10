using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.Service.Controllers;
using System.Collections.Generic;
using FakeXrmEasy;
using Tc.Crm.Service.Services;
using System.Net;
using System.Web.Http.Hosting;
using System.Web.Http;
using Tc.Crm.Service.Models;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Common.Constants;
using Attributes = Tc.Crm.Common.Constants.Attributes;
using System.Linq;
using Tc.Crm.Common;
using FakeItEasy;
using Tc.Crm.Service.Constants;

namespace Tc.Crm.ServiceTests.Controllers
{
    [TestClass()]
    public class ConfirmControllerTests
    {
        ConfirmationController controller;

        [TestInitialize()]
        public void TestSetup()
        {
            controller = new ConfirmationController(null);
            controller.Request = new System.Net.Http.HttpRequestMessage();
            controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
        }

        [TestMethod()]
        public void TestPayloadIsNull()
        {
            var response = controller.Confirmations(Guid.NewGuid().ToString(), null);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
            Assert.AreEqual(((System.Net.Http.ObjectContent)response.Content).Value, Messages.ConfirmationDataPassedIsNullOrCouldNotBeParsed);
        }

        [TestMethod()]
        public void TestCorrelationIdIsEmpty()
        {
            var response = controller.Confirmations("", new IntegrationLayerResponse { });
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
            Assert.AreEqual(((System.Net.Http.ObjectContent)response.Content).Value, Messages.MissingMsdCorrelationId);
        }

		[TestMethod()]
		public void TestCorrelationIdIsNotGuid()
		{
			var response = controller.Confirmations("not-guid", new IntegrationLayerResponse { });
			Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
			Assert.AreEqual(((System.Net.Http.ObjectContent)response.Content).Value, Messages.IncorrectMsdCorrelationId);
		}

		[TestMethod()]
		public void TestConfirmationServiceReturnsOkResponse()
		{
			var guid = Guid.NewGuid();
			var requestBody = new IntegrationLayerResponse
			{
				CorrelationId = guid.ToString(),
				SourceSystemEntityID = "entity id",
				SourceSystemStatusCode = HttpStatusCode.OK
			};
			var confirmationService = A.Fake<IConfirmationService>();
			A.CallTo(() => confirmationService.ProcessResponse(guid, requestBody)).Returns(new ConfirmationResponse() { StatusCode = HttpStatusCode.OK, Message = null });

			controller = new ConfirmationController(confirmationService);
			controller.Request = new System.Net.Http.HttpRequestMessage();
			controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

			var response = controller.Confirmations(guid.ToString(), requestBody);

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual(null, ((System.Net.Http.ObjectContent)response.Content).Value);
		}

		[TestMethod()]
		public void TestConfirmationServiceReturnsNotOkResponse()
		{
			var guid = Guid.NewGuid();
			var requestBody = new IntegrationLayerResponse
			{
				CorrelationId = guid.ToString(),
				SourceSystemEntityID = "entity id",
				SourceSystemStatusCode = HttpStatusCode.OK
			};
			var confirmationService = A.Fake<IConfirmationService>();
			A.CallTo(() => confirmationService.ProcessResponse(guid, requestBody)).Returns(new ConfirmationResponse() { StatusCode = HttpStatusCode.BadRequest, Message = "message" });

			controller = new ConfirmationController(confirmationService);
			controller.Request = new System.Net.Http.HttpRequestMessage();
			controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

			var response = controller.Confirmations(guid.ToString(), requestBody);

			Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.AreEqual("message", ((System.Net.Http.ObjectContent)response.Content).Value);
		}

		[TestMethod()]
		public void TestConfirmationServiceThrowsException()
		{
			var guid = Guid.NewGuid();
			var requestBody = new IntegrationLayerResponse
			{
				CorrelationId = guid.ToString(),
				SourceSystemEntityID = "entity id",
				SourceSystemStatusCode = HttpStatusCode.OK
			};
			var confirmationService = A.Fake<IConfirmationService>();
			A.CallTo(() => confirmationService.ProcessResponse(guid, requestBody)).Throws(new Exception("message"));

			controller = new ConfirmationController(confirmationService);
			controller.Request = new System.Net.Http.HttpRequestMessage();
			controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

			var response = controller.Confirmations(guid.ToString(), requestBody);

			Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
			Assert.AreEqual(Messages.UnexpectedError, ((System.Net.Http.ObjectContent)response.Content).Value);
		}
	}
}
