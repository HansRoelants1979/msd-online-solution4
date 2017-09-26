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

namespace Tc.Crm.ServiceTests.Services
{
    [TestClass()]
    public class ConfirmationServiceTests
    {
        XrmFakedContext context;
        IConfirmationService confirmationService;
        TestCrmService crmService;        

        [TestInitialize()]
        public void TestSetup()
        {  
            context = new XrmFakedContext();            
            crmService = new TestCrmService(context);
            confirmationService = new ConfirmationService(crmService);          
        }

        private void LoadData(Guid entityCacheMessageId)
        {
            var entityCacheId = Guid.NewGuid();

            var entityCacheMessage = new Entity(EntityName.EntityCacheMessage, entityCacheMessageId);
            entityCacheMessage.Attributes[Attributes.EntityCacheMessage.EntityCacheId] = new EntityReference(EntityName.EntityCache, entityCacheId);
            entityCacheMessage.Attributes[Attributes.EntityCacheMessage.Notes] = string.Empty;
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
            var response = confirmationService.ProcessResponse(null);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.GatewayTimeout);
            Assert.IsTrue(response.Message == Messages.FailedToUpdateEntityCacheMessage);
        }

        [TestMethod()]       
        public void ConfirmationIsEmpty()
        {
            var response = confirmationService.ProcessResponse(new IntegrationLayerResponse() { });
            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
            Assert.IsTrue(response.Message == Messages.CorrelationIdWasMissing);
        }

        [TestMethod()]        
        public void CrmServiceIsNull()
        {
            var response = confirmationService.ProcessResponse(new IntegrationLayerResponse() { CorrelationId="123" });
            Assert.IsTrue(response.StatusCode == HttpStatusCode.GatewayTimeout);
            Assert.IsTrue(response.Message == Messages.FailedToUpdateEntityCacheMessage);
        }

       

        [TestMethod()]
        public void WithoutCorrelationId()
        {
            var response = confirmationService.ProcessResponse(new IntegrationLayerResponse() { SourceSystemEntityID="3445",SourceSystemRequest="request" });
            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
            Assert.IsTrue(response.Message == Messages.CorrelationIdWasMissing);
        }

        [TestMethod()]
        public void FailedConfirmation()
        {
            var id = Guid.NewGuid();
            LoadData(id);
            var resp = confirmationService.ProcessResponse(new IntegrationLayerResponse() { CorrelationId= id.ToString(),SourceSystemEntityID="" });
            Assert.AreEqual(resp.StatusCode,System.Net.HttpStatusCode.OK);
            var entityCacheMessage = (from c in context.CreateQuery(EntityName.EntityCacheMessage)
                                      where c.Id == id
                                      select c).ToList();
            Assert.IsTrue(entityCacheMessage.Count > 0);            
            Assert.AreEqual(((OptionSetValue)entityCacheMessage[0].Attributes[Attributes.EntityCacheMessage.StatusReason]).Value, (int)EntityCacheMessageStatusReason.Failed);
        }

        [TestMethod()]
        public void OnSuccess()
        {
            var id = Guid.NewGuid();
            LoadData(id);
            var resp = confirmationService.ProcessResponse(new IntegrationLayerResponse() { CorrelationId = id.ToString(), SourceSystemEntityID = "123",SourceSystemStatusCode=HttpStatusCode.OK });
            Assert.AreEqual(resp.StatusCode, System.Net.HttpStatusCode.OK);
            var entityCacheMessage = (from c in context.CreateQuery(EntityName.EntityCacheMessage)
                                      where c.Id == id
                                      select c).ToList();
            Assert.IsTrue(entityCacheMessage.Count > 0);
            Assert.AreEqual(entityCacheMessage[0].Attributes[Attributes.EntityCacheMessage.OutcomeId].ToString(), "123");
            Assert.AreEqual(((OptionSetValue)entityCacheMessage[0].Attributes[Attributes.EntityCacheMessage.StatusReason]).Value, (int)EntityCacheMessageStatusReason.EndtoEndSuccess);

            var entityCache = (from c in context.CreateQuery(EntityName.EntityCache)
                               where c.Id != id
                               select c).ToList();
            Assert.IsTrue(entityCache.Count > 0);
            Assert.AreEqual(((OptionSetValue)entityCache[0].Attributes[Attributes.EntityCache.StatusReason]).Value, (int)EntityCacheStatusReason.Succeeded);
        }
    }
}
