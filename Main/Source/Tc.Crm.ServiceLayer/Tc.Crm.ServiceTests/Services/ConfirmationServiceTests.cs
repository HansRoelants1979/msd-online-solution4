using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.Service.Services;
using System;
using FakeXrmEasy;
using Tc.Crm.Service.Models;
using Tc.Crm.Service.Constants;
using System.Net;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Common.Constants;
using Attributes = Tc.Crm.Common.Constants.Attributes;
using System.Linq;
using Tc.Crm.Common;
using FakeItEasy;
using System.Text;
using Tc.Crm.Common.Constants.EntityRecords;

namespace Tc.Crm.ServiceTests.Services
{
    [TestClass()]
    public class ConfirmationServiceTests
    {
		[TestMethod()]
		public void TestEntityCacheMessageIdEmpty()
		{
			var confirmationService = new ConfirmationService(null);

			var response = confirmationService.ProcessResponse(Guid.Empty, null);
			Assert.IsTrue(response.StatusCode == HttpStatusCode.GatewayTimeout);
			Assert.IsTrue(response.Message == Messages.FailedToUpdateEntityCacheMessage);
		}

		[TestMethod()]        
        public void TestIntegrationLayerResponseIsNull()
        {
			var confirmationService = new ConfirmationService(null);

			var response = confirmationService.ProcessResponse(Guid.NewGuid(), null);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.GatewayTimeout);
            Assert.IsTrue(response.Message == Messages.FailedToUpdateEntityCacheMessage);
        }

        [TestMethod()]        
        public void TestCrmServiceIsNull()
        {
			var confirmationService = new ConfirmationService(null);

			var response = confirmationService.ProcessResponse(Guid.NewGuid(), new IntegrationLayerResponse());
            Assert.IsTrue(response.StatusCode == HttpStatusCode.GatewayTimeout);
            Assert.IsTrue(response.Message == Messages.FailedToUpdateEntityCacheMessage);
        }       

        [TestMethod()]
        public void TestIntegrationResponseStatusSuccess()
        {
			TestIntegrationResponseStatus(HttpStatusCode.OK);
			TestIntegrationResponseStatus(HttpStatusCode.Created);
			TestIntegrationResponseStatus(HttpStatusCode.Accepted);
			TestIntegrationResponseStatus(HttpStatusCode.NonAuthoritativeInformation);
			TestIntegrationResponseStatus(HttpStatusCode.NoContent);
			TestIntegrationResponseStatus(HttpStatusCode.ResetContent);
			TestIntegrationResponseStatus(HttpStatusCode.PartialContent);
		}

		[TestMethod()]
		public void TestIntegrationResponseStatusOkEntityCacheMessageDoesNotExist()
		{
			var entityCacheMessageId = Guid.NewGuid();
			var sourceSystemId = "source system id";
			var serviceResponse = new IntegrationLayerResponse
			{
				SourceSystemEntityID = sourceSystemId,
				SourceSystemStatusCode = HttpStatusCode.OK
			};

			var crmService = A.Fake<ICrmService>();
			A.CallTo(() => crmService.ProcessEntityCacheMessage(entityCacheMessageId, sourceSystemId, Status.Inactive, EntityCacheMessageStatusReason.EndtoEndSuccess, null)).Returns(Guid.Empty);
			A.CallTo(() => crmService.ActivateRelatedPendingEntityCache(Guid.Empty)).DoesNothing();
			A.CallTo(() => crmService.ProcessEntityCache(Guid.Empty, Status.Inactive, EntityCacheStatusReason.Succeeded, true, null)).DoesNothing();

			var confirmationService = new ConfirmationService(crmService);
			var response = confirmationService.ProcessResponse(entityCacheMessageId, serviceResponse);

			A.CallTo(() => crmService.ProcessEntityCacheMessage(entityCacheMessageId, sourceSystemId, Status.Inactive, EntityCacheMessageStatusReason.EndtoEndSuccess, null)).MustHaveHappened();
			A.CallTo(() => crmService.ActivateRelatedPendingEntityCache(Guid.Empty)).MustNotHaveHappened();
			A.CallTo(() => crmService.ProcessEntityCache(Guid.Empty, Status.Inactive, EntityCacheStatusReason.Succeeded, true, null)).MustNotHaveHappened();

			Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.AreEqual(string.Format(Messages.MsdCorrelationIdDoesNotExist, entityCacheMessageId), response.Message);
		}

		[TestMethod()]
		public void TestIntegrationResponseStatusNotOkRetryNeeded()
		{
			var entityCacheMessageId = Guid.NewGuid();
			var entityCacheId = Guid.NewGuid();
			var sourceSystemId = "source system id";
			var serviceResponse = new IntegrationLayerResponse
			{
				SourceSystemEntityID = sourceSystemId,
				SourceSystemStatusCode = HttpStatusCode.InternalServerError,
				SourceSystemRequest = "SourceSystemRequest",
				SourceSystemResponse = "SourceSystemResponse"
			};

			var builder = new StringBuilder();
			builder.AppendLine("SourceSystemStatusCode: " + serviceResponse.SourceSystemStatusCode);
			if (!string.IsNullOrWhiteSpace(serviceResponse.SourceSystemRequest)) builder.AppendLine("SourceSystemRequest: " + serviceResponse.SourceSystemRequest);
			if (!string.IsNullOrWhiteSpace(serviceResponse.SourceSystemResponse)) builder.AppendLine("SourceSystemResponse: " + serviceResponse.SourceSystemResponse);
			var notes = builder.ToString();

			var crmService = A.Fake<ICrmService>();
			A.CallTo(() => crmService.ProcessEntityCacheMessage(entityCacheMessageId, sourceSystemId, Status.Inactive, EntityCacheMessageStatusReason.Failed, notes)).Returns(entityCacheId);
			A.CallTo(() => crmService.ActivateRelatedPendingEntityCache(entityCacheId)).DoesNothing();
			A.CallTo(() => crmService.ProcessEntityCache(entityCacheId, Status.Active, EntityCacheStatusReason.InProgress, false, null)).DoesNothing();
			A.CallTo(() => crmService.GetEntityCacheMessageCount(entityCacheId)).Returns(1);
			A.CallTo(() => crmService.GetConfiguration(Configuration.OutboundSynchronisationMaxRetries)).Returns("5,10");

			var confirmationService = new ConfirmationService(crmService);
			var response = confirmationService.ProcessResponse(entityCacheMessageId, serviceResponse);

			A.CallTo(() => crmService.ProcessEntityCacheMessage(entityCacheMessageId, sourceSystemId, Status.Inactive, EntityCacheMessageStatusReason.Failed, notes)).MustHaveHappened();
			A.CallTo(() => crmService.ActivateRelatedPendingEntityCache(entityCacheId)).MustNotHaveHappened();
			A.CallTo(() => crmService.ProcessEntityCache(entityCacheId, Status.Active, EntityCacheStatusReason.InProgress, false, A<DateTime>.That.Matches(d => d > DateTime.UtcNow && d < DateTime.UtcNow.AddMinutes(5)))).MustHaveHappened();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual(string.Empty, response.Message);
		}

		public void TestIntegrationResponseStatusNotOkNoRetryAvailable()
		{
			var entityCacheMessageId = Guid.NewGuid();
			var entityCacheId = Guid.NewGuid();
			var sourceSystemId = "source system id";
			var serviceResponse = new IntegrationLayerResponse
			{
				SourceSystemEntityID = sourceSystemId,
				SourceSystemStatusCode = HttpStatusCode.InternalServerError,
				SourceSystemRequest = "SourceSystemRequest",
				SourceSystemResponse = "SourceSystemResponse"
			};

			var builder = new StringBuilder();
			builder.AppendLine("SourceSystemStatusCode: " + serviceResponse.SourceSystemStatusCode);
			if (!string.IsNullOrWhiteSpace(serviceResponse.SourceSystemRequest)) builder.AppendLine("SourceSystemRequest: " + serviceResponse.SourceSystemRequest);
			if (!string.IsNullOrWhiteSpace(serviceResponse.SourceSystemResponse)) builder.AppendLine("SourceSystemResponse: " + serviceResponse.SourceSystemResponse);
			var notes = builder.ToString();

			var crmService = A.Fake<ICrmService>();
			A.CallTo(() => crmService.ProcessEntityCacheMessage(entityCacheMessageId, sourceSystemId, Status.Inactive, EntityCacheMessageStatusReason.Failed, notes)).Returns(entityCacheId);
			A.CallTo(() => crmService.ActivateRelatedPendingEntityCache(entityCacheId)).DoesNothing();
			A.CallTo(() => crmService.ProcessEntityCache(entityCacheId, Status.Active, EntityCacheStatusReason.InProgress, false, null)).DoesNothing();

			var confirmationService = new ConfirmationService(crmService);
			var response = confirmationService.ProcessResponse(entityCacheMessageId, serviceResponse);

			A.CallTo(() => crmService.ProcessEntityCacheMessage(entityCacheMessageId, sourceSystemId, Status.Inactive, EntityCacheMessageStatusReason.Failed, notes)).MustHaveHappened();
			A.CallTo(() => crmService.ActivateRelatedPendingEntityCache(entityCacheId)).MustNotHaveHappened();
			A.CallTo(() => crmService.ProcessEntityCache(entityCacheId, Status.Inactive, EntityCacheStatusReason.Failed, false, null)).MustHaveHappened();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual(string.Empty, response.Message);
		}


		[TestMethod()]
		public void TestCrmServiceThrowsError()
		{
			var entityCacheMessageId = Guid.NewGuid();
			var entityCacheId = Guid.NewGuid();
			var sourceSystemId = "source system id";
			var serviceResponse = new IntegrationLayerResponse
			{
				SourceSystemEntityID = sourceSystemId,
				SourceSystemStatusCode = HttpStatusCode.OK,
				SourceSystemRequest = "SourceSystemRequest",
				SourceSystemResponse = "SourceSystemResponse"
			};

			var crmService = A.Fake<ICrmService>();
			A.CallTo(() => crmService.ProcessEntityCacheMessage(entityCacheMessageId, sourceSystemId, Status.Inactive, EntityCacheMessageStatusReason.EndtoEndSuccess, null)).Throws(new Exception("exception"));

			var confirmationService = new ConfirmationService(crmService);
			var response = confirmationService.ProcessResponse(entityCacheMessageId, serviceResponse);

			A.CallTo(() => crmService.ProcessEntityCacheMessage(entityCacheMessageId, sourceSystemId, Status.Inactive, EntityCacheMessageStatusReason.EndtoEndSuccess, null)).MustHaveHappened();

			Assert.AreEqual(HttpStatusCode.GatewayTimeout, response.StatusCode);
			Assert.AreEqual(Messages.FailedToUpdateEntityCacheMessage, response.Message);
		}

		private void TestIntegrationResponseStatus(HttpStatusCode status)
		{
			var entityCacheMessageId = Guid.NewGuid();
			var entityCacheId = Guid.NewGuid();
			var sourceSystemId = "source system id";
			var serviceResponse = new IntegrationLayerResponse
			{
				SourceSystemEntityID = sourceSystemId,
				SourceSystemStatusCode = HttpStatusCode.OK
			};

			var crmService = A.Fake<ICrmService>();
			A.CallTo(() => crmService.ProcessEntityCacheMessage(entityCacheMessageId, sourceSystemId, Status.Inactive, EntityCacheMessageStatusReason.EndtoEndSuccess, null)).Returns(entityCacheId);
			A.CallTo(() => crmService.ActivateRelatedPendingEntityCache(entityCacheId)).DoesNothing();
			A.CallTo(() => crmService.ProcessEntityCache(entityCacheId, Status.Inactive, EntityCacheStatusReason.Succeeded, true, null)).DoesNothing();

			var confirmationService = new ConfirmationService(crmService);
			var response = confirmationService.ProcessResponse(entityCacheMessageId, serviceResponse);

			A.CallTo(() => crmService.ProcessEntityCacheMessage(entityCacheMessageId, sourceSystemId, Status.Inactive, EntityCacheMessageStatusReason.EndtoEndSuccess, null)).MustHaveHappened();
			A.CallTo(() => crmService.ActivateRelatedPendingEntityCache(entityCacheId)).MustHaveHappened();
			A.CallTo(() => crmService.ProcessEntityCache(entityCacheId, Status.Inactive, EntityCacheStatusReason.Succeeded, true, null)).MustHaveHappened();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual(string.Empty, response.Message);
		}
	}
}
