using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeXrmEasy;
using Tc.Crm.Plugins;
using Attributes = Tc.Crm.Plugins.Attributes;
using Tc.Crm.Plugins.OptionSetValues;

namespace Tc.Crm.UnitTests.Plugins
{
    [TestClass]
    public class CreateEntityCacheOnCustomerOperationTests
    {
        XrmFakedContext context = null;
        Guid sourceMarketId = Guid.NewGuid();
        Guid initiatingUserId = Guid.NewGuid();
        string iso2Code = "AO";
        string customerName = "Srinivas";
        [TestInitialize]
        public void Initialise()
        {
            var country = new Entity(Entities.Country, sourceMarketId);
            country.Attributes.Add(Attributes.Country.ISO2Code, iso2Code);
            context = new XrmFakedContext();
            context.Initialize(new List<Entity>() { country });
        }

        private XrmFakedPluginExecutionContext GetPluginContext(Guid countryId, string messageName, int stage, int mode)
        {
            var customer = new Entity(Entities.Contact, Guid.NewGuid());
            customer.Attributes.Add(Attributes.Customer.FullName, customerName);
            customer.Attributes.Add(Attributes.Customer.SourceMarketId, new EntityReference(Entities.Country, countryId));
            var cntxt = new XrmFakedPluginExecutionContext();
            cntxt.InputParameters = new ParameterCollection();
            cntxt.InputParameters.Add(InputParameters.Target, customer);
            cntxt.InitiatingUserId = initiatingUserId;
            cntxt.MessageName = messageName;
            cntxt.Stage = stage;
            cntxt.Mode = mode;
            return cntxt;
        }

        [TestMethod]        
        public void CheckForInvalidMessage()
        {
            var cntxt = GetPluginContext(sourceMarketId,Messages.Update,(int)PluginStage.Postoperation, (int)PluginMode.Asynchronous);
            context.ExecutePluginWithConfigurations<Tc.Crm.Plugins.Customer.CreateEntityCacheOnCustomerOperation>(cntxt, null, null);
            Assert.IsTrue(context.Data.Count == 1);
        }

        [TestMethod]
        public void CheckForInvalidStage()
        {
            var cntxt = GetPluginContext(sourceMarketId, Messages.Create, (int)PluginStage.Preoperation, (int)PluginMode.Asynchronous);
            context.ExecutePluginWithConfigurations<Tc.Crm.Plugins.Customer.CreateEntityCacheOnCustomerOperation>(cntxt, null, null);
            Assert.IsTrue(context.Data.Count == 1);
        }

        [TestMethod]
        public void CheckForInvalidMode()
        {
            var cntxt = GetPluginContext(sourceMarketId, Messages.Create, (int)PluginStage.Postoperation, (int)PluginMode.Synchronous);
            context.ExecutePluginWithConfigurations<Tc.Crm.Plugins.Customer.CreateEntityCacheOnCustomerOperation>(cntxt, null, null);
            Assert.IsTrue(context.Data.Count == 1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException))]
        public void CheckWithInvalidSourceMarket()
        {
            var cntxt = GetPluginContext(Guid.NewGuid(), Messages.Create, (int)PluginStage.Postoperation, (int)PluginMode.Asynchronous);
            context.ExecutePluginWithConfigurations<Tc.Crm.Plugins.Customer.CreateEntityCacheOnCustomerOperation>(cntxt, null, null);
        }

        [TestMethod]
        public void CheckWithServiceAccountToIgnore()
        {
            var cntxt = GetPluginContext(sourceMarketId, Messages.Create, (int)PluginStage.Postoperation, (int)PluginMode.Asynchronous);
            context.ExecutePluginWithConfigurations<Tc.Crm.Plugins.Customer.CreateEntityCacheOnCustomerOperation>(cntxt, initiatingUserId.ToString() , null);
            Assert.IsTrue(context.Data.Count == 1);
        }


        [TestMethod]        
        public void CheckWithValidData()
        {
            var cntxt = GetPluginContext(sourceMarketId, Messages.Create, (int)PluginStage.Postoperation, (int)PluginMode.Asynchronous);
            context.ExecutePluginWithConfigurations<Tc.Crm.Plugins.Customer.CreateEntityCacheOnCustomerOperation>(cntxt, null, null);
            Assert.IsTrue(context.Data.Count == 2);
            var cache = (from c in context.CreateQuery(Entities.EntityCache)
                         where c.Id != Guid.Empty
                         select c).ToList();
            Assert.IsTrue(cache.Count == 1);
            Assert.IsNotNull(cache[0].Attributes[Attributes.EntityCache.Name]);
            Assert.IsNotNull(cache[0].Attributes[Attributes.EntityCache.Type]);
            Assert.IsNotNull(cache[0].Attributes[Attributes.EntityCache.Data]);
            Assert.IsNotNull(cache[0].Attributes[Attributes.EntityCache.Operation]);
            Assert.IsNotNull(cache[0].Attributes[Attributes.EntityCache.SourceMarket]);
            Assert.IsTrue(cache[0].Attributes[Attributes.EntityCache.Type].ToString() == Entities.Contact);
            Assert.IsTrue(((OptionSetValue)cache[0].Attributes[Attributes.EntityCache.Operation]).Value == Operation.Create);
            Assert.IsTrue(cache[0].Attributes[Attributes.EntityCache.SourceMarket].ToString() == iso2Code);
            Assert.IsTrue(cache[0].Attributes[Attributes.EntityCache.Name].ToString() == customerName);
            Assert.IsTrue(cache[0].Attributes[Attributes.EntityCache.Data].ToString().Length > 0);
        }
    }
}
