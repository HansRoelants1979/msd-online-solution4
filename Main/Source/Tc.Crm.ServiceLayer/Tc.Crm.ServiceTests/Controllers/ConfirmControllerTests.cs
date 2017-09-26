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


namespace Tc.Crm.ServiceTests.Controllers
{
    [TestClass()]
    public class ConfirmControllerTests
    {
        XrmFakedContext context;
        IConfirmationService confirmationService;
        ConfirmationController controller;
        ICrmService crmService;        

        [TestInitialize()]
        public void TestSetup()
        {
            context = new XrmFakedContext();
            crmService = new TestCrmService(context);
            confirmationService = new ConfirmationService(crmService);
            crmService = new TestCrmService(context);
            controller = new ConfirmationController(confirmationService);
            controller.Request = new System.Net.Http.HttpRequestMessage();
            controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());                   
        }

        private void LoadData(Guid entityCacheMessageId)
        {
            var entityCacheId = Guid.NewGuid();

            var entityCacheMessage = new Entity(EntityName.EntityCacheMessage, entityCacheMessageId);
            entityCacheMessage.Attributes[Attributes.EntityCacheMessage.EntityCacheId] = new EntityReference(EntityName.EntityCache, entityCacheId);
            entityCacheMessage.Attributes[Attributes.EntityCacheMessage.Notes] = "Default notes.........";
            entityCacheMessage.Attributes[Attributes.EntityCacheMessage.OutcomeId] = string.Empty;
            entityCacheMessage.Attributes[Attributes.EntityCacheMessage.State] = new OptionSetValue(0);
            entityCacheMessage.Attributes[Attributes.EntityCacheMessage.StatusReason] = new OptionSetValue(0);

            var entityCache = new Entity(EntityName.EntityCache, entityCacheId);
            entityCache.Attributes[Attributes.EntityCache.State] = new OptionSetValue(0);
            entityCache.Attributes[Attributes.EntityCache.StatusReason] = new OptionSetValue(0);

            context.Initialize(new List<Entity> { entityCacheMessage, entityCache });
        }

        [TestMethod()]
        public void ConfirmationIsNull()
        {
            var response = controller.Confirmations("123", null);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
            Assert.AreEqual(((System.Net.Http.ObjectContent)response.Content).Value, Service.Constants.Messages.ConfirmationDataPassedIsNullOrCouldNotBeParsed);
        }

        [TestMethod()]
        public void ConfirmationWithoutCorrelationId()
        {
            var response = controller.Confirmations("", new IntegrationLayerResponse { });
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
            Assert.AreEqual(((System.Net.Http.ObjectContent)response.Content).Value, Service.Constants.Messages.CorrelationIdWasMissing);
        }

        [TestMethod()]
        public void ConfirmationWithInvalidCorrelationId()
        {
           var id = Guid.NewGuid();
           var response = controller.Confirmations(id.ToString(), new IntegrationLayerResponse { CorrelationId = Guid.NewGuid().ToString() ,SourceSystemEntityID="123" });
           Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
           Assert.AreEqual(((System.Net.Http.ObjectContent)response.Content).Value, Service.Constants.Messages.CorrelationIdWasMissing);
        }

        [TestMethod()]
        public void ConfirmationWithWrongCorrelationId()
        {
            var id = Guid.NewGuid();
            LoadData(id);
            var response = controller.Confirmations(id.ToString(), new IntegrationLayerResponse { CorrelationId = Guid.NewGuid().ToString(), SourceSystemEntityID = "123" });
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
            Assert.AreEqual(((System.Net.Http.ObjectContent)response.Content).Value, Service.Constants.Messages.CorrelationIdWasMissing);
        }

        [TestMethod()]
        public void ConfirmationWithWrongFormatCorrelationId()
        {
            var id = Guid.NewGuid();
            LoadData(id);
            var response = controller.Confirmations("123", new IntegrationLayerResponse { CorrelationId = "123", SourceSystemEntityID = "123" });
            Assert.AreEqual(response.StatusCode, HttpStatusCode.GatewayTimeout);
            Assert.AreEqual(((System.Net.Http.ObjectContent)response.Content).Value, Service.Constants.Messages.FailedToUpdateEntityCacheMessage);
        }

        [TestMethod()]
        public void ConfirmationWithValidDetailsAndFailedStatus()
        {
            var id = Guid.NewGuid();
            LoadData(id);
            var response = controller.Confirmations(id.ToString(), new IntegrationLayerResponse { CorrelationId = id.ToString(), SourceSystemEntityID = "", SourceSystemStatusCode = HttpStatusCode.ExpectationFailed,SourceSystemRequest="request failed",SourceSystemResponse="response also got failed" });
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(((System.Net.Http.ObjectContent)response.Content).Value, string.Empty);
            var entityCacheMessage = (from c in context.CreateQuery(EntityName.EntityCacheMessage)
                                      where c.Id == id
                                      select c).ToList();
            Assert.IsTrue(entityCacheMessage.Count > 0);            
            Assert.AreEqual(((OptionSetValue)entityCacheMessage[0].Attributes[Attributes.EntityCacheMessage.StatusReason]).Value, (int)EntityCacheMessageStatusReason.Failed);
        }

        [TestMethod()]
        public void ConfirmationWithValidDetails()
        {
            var id = Guid.NewGuid();
            LoadData(id);
            var response = controller.Confirmations(id.ToString(), new IntegrationLayerResponse { CorrelationId = id.ToString(), SourceSystemEntityID = "123", SourceSystemStatusCode = HttpStatusCode.OK });
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(((System.Net.Http.ObjectContent)response.Content).Value, string.Empty);
            var entityCacheMessage = (from c in context.CreateQuery(EntityName.EntityCacheMessage)
                                      where c.Id == id
                                      select c).ToList();
            Assert.IsTrue(entityCacheMessage.Count > 0);
            Assert.AreEqual(entityCacheMessage[0].Attributes[Attributes.EntityCacheMessage.OutcomeId].ToString(),  "123");
            Assert.AreEqual(((OptionSetValue)entityCacheMessage[0].Attributes[Attributes.EntityCacheMessage.StatusReason]).Value, (int)EntityCacheMessageStatusReason.EndtoEndSuccess);

            var entityCache = (from c in context.CreateQuery(EntityName.EntityCache)
                               where c.Id != id
                               select c).ToList();
            Assert.IsTrue(entityCache.Count > 0);            
            Assert.AreEqual(((OptionSetValue)entityCache[0].Attributes[Attributes.EntityCache.StatusReason]).Value, (int)EntityCacheStatusReason.Succeeded);
        }
    }
}
