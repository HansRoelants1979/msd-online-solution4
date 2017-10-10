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

			var entityCache = new Entity(Entities.EntityCache, Guid.NewGuid());
			entityCache.Attributes.Add(Attributes.EntityCache.RecordId, Guid.NewGuid());
			entityCache.Attributes.Add(Attributes.EntityCache.EntityCacheId, Guid.NewGuid());
			entityCache.Attributes.Add(Attributes.EntityCache.StatusCode, new OptionSetValue((int)EntityCacheStatusReason.Succeeded));
			

			context = new XrmFakedContext();
            context.Initialize(new List<Entity>() { country });
        }

        private XrmFakedPluginExecutionContext GetPluginContext(Guid countryId, string messageName, int stage, int mode)
        {
            return GetPluginContext(countryId, messageName, stage, mode, false, false, false, false);
        }

        private XrmFakedPluginExecutionContext GetPluginContext(Guid countryId, string messageName, int stage, int mode, bool updateAddress,bool updateEmail,bool updatePhone, bool getPostImage)
        {
            var customer = new Entity(Entities.Contact, Guid.NewGuid());
            customer.Attributes.Add(Attributes.Customer.FullName, customerName);
            customer.Attributes.Add(Attributes.Customer.SourceMarketId, new EntityReference(Entities.Country, countryId));
            if(updateAddress)
				customer.Attributes.Add(Attributes.Customer.Address1FlatorUnitNumber, "143");
            if(updateEmail)
				customer.Attributes.Add(Attributes.Customer.EmailAddress1, "srini@gmail.com");
            if(updatePhone)
				customer.Attributes.Add(Attributes.Customer.Telephone1, "1234545");
            var cntxt = new XrmFakedPluginExecutionContext();
            cntxt.InputParameters = new ParameterCollection();
            cntxt.InputParameters.Add(InputParameters.Target, customer);
            cntxt.InitiatingUserId = initiatingUserId;
            cntxt.MessageName = messageName;
            cntxt.Stage = stage;
            cntxt.Mode = mode;
            if (getPostImage)
            {
                var entPostImage = new Entity();
                entPostImage.Attributes.Add(Attributes.Customer.Address2FlatorUnitNumber, "stre");
                entPostImage.Attributes.Add(Attributes.Customer.EmailAddress1Type, new OptionSetValue(0));
                entPostImage.Attributes.Add(Attributes.Customer.Telephone1Type, new OptionSetValue(0));
                cntxt.PostEntityImages = new EntityImageCollection();
                cntxt.PostEntityImages.Add("PostImage", entPostImage);
            }
            return cntxt;
        }

        [TestMethod]        
        public void CheckForInvalidMessage()
        {
            var cntxt = GetPluginContext(sourceMarketId,Messages.Associate,(int)PluginStage.Postoperation, (int)PluginMode.Asynchronous);
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

        //[TestMethod]
        public void CheckUpdateOperation()
        {
            var cntxt = GetPluginContext(sourceMarketId, Messages.Update, (int)PluginStage.Postoperation, (int)PluginMode.Asynchronous);
            context.ExecutePluginWithConfigurations<Tc.Crm.Plugins.Customer.CreateEntityCacheOnCustomerOperation>(cntxt, null, null);
            Assert.IsTrue(context.Data.Count == 2);
            var cache = (from c in context.CreateQuery(Entities.EntityCache)
                         where c.Id != Guid.Empty
                         select c).ToList();
            CheckUpdateCoditions(cache, false, false, false);
        }

        //[TestMethod]
        public void CheckUpdateOperationWithPostImage()
        {
            var cntxt = GetPluginContext(sourceMarketId, Messages.Update, (int)PluginStage.Postoperation, (int)PluginMode.Asynchronous,false,false,false,true);
            context.ExecutePluginWithConfigurations<Tc.Crm.Plugins.Customer.CreateEntityCacheOnCustomerOperation>(cntxt, null, null);
            Assert.IsTrue(context.Data.Count == 2);
            var cache = (from c in context.CreateQuery(Entities.EntityCache)
                         where c.Id != Guid.Empty
                         select c).ToList();
            CheckUpdateCoditions(cache, false, false, false);
        }

        //[TestMethod]
        public void CheckUpdateAddressOperationWithPostImage()
        {
            var cntxt = GetPluginContext(sourceMarketId, Messages.Update, (int)PluginStage.Postoperation, (int)PluginMode.Asynchronous, true, false, false, true);
            context.ExecutePluginWithConfigurations<Tc.Crm.Plugins.Customer.CreateEntityCacheOnCustomerOperation>(cntxt, null, null);
            Assert.IsTrue(context.Data.Count == 2);
            var cache = (from c in context.CreateQuery(Entities.EntityCache)
                         where c.Id != Guid.Empty
                         select c).ToList();
            CheckUpdateCoditions(cache, true, false, false);
        }

        //[TestMethod]
        public void CheckUpdateEmailOperationWithPostImage()
        {
            var cntxt = GetPluginContext(sourceMarketId, Messages.Update, (int)PluginStage.Postoperation, (int)PluginMode.Asynchronous, false, true, false, true);
            context.ExecutePluginWithConfigurations<Tc.Crm.Plugins.Customer.CreateEntityCacheOnCustomerOperation>(cntxt, null, null);
            Assert.IsTrue(context.Data.Count == 2);
            var cache = (from c in context.CreateQuery(Entities.EntityCache)
                         where c.Id != Guid.Empty
                         select c).ToList();
            CheckUpdateCoditions(cache, false, false, true);
        }

        //[TestMethod]
        public void CheckUpdateTelephoneOperationWithPostImage()
        {
            var cntxt = GetPluginContext(sourceMarketId, Messages.Update, (int)PluginStage.Postoperation, (int)PluginMode.Asynchronous, false, false, true, true);
            context.ExecutePluginWithConfigurations<Tc.Crm.Plugins.Customer.CreateEntityCacheOnCustomerOperation>(cntxt, null, null);
            Assert.IsTrue(context.Data.Count == 2);
            var cache = (from c in context.CreateQuery(Entities.EntityCache)
                         where c.Id != Guid.Empty
                         select c).ToList();
            CheckUpdateCoditions(cache, false, true, false);
        }

        //[TestMethod]
        public void CheckUpdateAddressTelephoneEmailOperationWithPostImage()
        {
            var cntxt = GetPluginContext(sourceMarketId, Messages.Update, (int)PluginStage.Postoperation, (int)PluginMode.Asynchronous, true, true, true, true);
            context.ExecutePluginWithConfigurations<Tc.Crm.Plugins.Customer.CreateEntityCacheOnCustomerOperation>(cntxt, null, null);
            Assert.IsTrue(context.Data.Count == 2);
            var cache = (from c in context.CreateQuery(Entities.EntityCache)
                         where c.Id != Guid.Empty
                         select c).ToList();
            CheckUpdateCoditions(cache, true, true, true);
        }

        //[TestMethod]
        public void CheckUpdateAddressTelephoneEmailOperationWithoutPostImage()
        {
            var cntxt = GetPluginContext(sourceMarketId, Messages.Update, (int)PluginStage.Postoperation, (int)PluginMode.Asynchronous, true, true, true, false);
            context.ExecutePluginWithConfigurations<Tc.Crm.Plugins.Customer.CreateEntityCacheOnCustomerOperation>(cntxt, null, null);
            Assert.IsTrue(context.Data.Count == 2);
            var cache = (from c in context.CreateQuery(Entities.EntityCache)
                         where c.Id != Guid.Empty
                         select c).ToList();
            CheckUpdateCoditions(cache,false,false,false);            
        }

        private void CheckUpdateCoditions(List<Entity> cache, bool containsAddress, bool containsTelephone, bool containsEmail )
        {
            Assert.IsTrue(cache.Count == 1);
            Assert.IsNotNull(cache[0].Attributes[Attributes.EntityCache.Name]);
            Assert.IsNotNull(cache[0].Attributes[Attributes.EntityCache.Type]);
            Assert.IsNotNull(cache[0].Attributes[Attributes.EntityCache.Data]);
            Assert.IsNotNull(cache[0].Attributes[Attributes.EntityCache.Operation]);
            Assert.IsNotNull(cache[0].Attributes[Attributes.EntityCache.SourceMarket]);
            Assert.IsTrue(cache[0].Attributes[Attributes.EntityCache.Type].ToString() == Entities.Contact);
            Assert.IsTrue(((OptionSetValue)cache[0].Attributes[Attributes.EntityCache.Operation]).Value == Operation.Update);
            Assert.IsTrue(cache[0].Attributes[Attributes.EntityCache.SourceMarket].ToString() == iso2Code);
            Assert.IsTrue(cache[0].Attributes[Attributes.EntityCache.Name].ToString() == customerName);
            Assert.IsTrue(cache[0].Attributes[Attributes.EntityCache.Data].ToString().Length > 0);
            Assert.IsTrue((containsTelephone) ? cache[0].Attributes[Attributes.EntityCache.Data].ToString().Contains(Attributes.Customer.Telephone1Type) : !cache[0].Attributes[Attributes.EntityCache.Data].ToString().Contains(Attributes.Customer.Telephone1Type));
            Assert.IsTrue((containsAddress) ? cache[0].Attributes[Attributes.EntityCache.Data].ToString().Contains(Attributes.Customer.Address2FlatorUnitNumber) : !cache[0].Attributes[Attributes.EntityCache.Data].ToString().Contains(Attributes.Customer.Address2FlatorUnitNumber));
            Assert.IsTrue((containsEmail) ? cache[0].Attributes[Attributes.EntityCache.Data].ToString().Contains(Attributes.Customer.EmailAddress1Type) : !cache[0].Attributes[Attributes.EntityCache.Data].ToString().Contains(Attributes.Customer.EmailAddress1Type));
        }
    }
}
