using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.Common.IntegrationLayer.Jti.Models;
using Tc.Crm.Common.IntegrationLayer.Jti.Service;
using Tc.Crm.Common.Services;
using Tc.Crm.OutboundSynchronisation.Customer.Services;
using System.Configuration;
using Tc.Crm.Common.IntegrationLayer.Model;
using Tc.Crm.Common.IntegrationLayer.Service.Synchronisation;
using Attributes = Tc.Crm.Common.Constants.Attributes;

namespace Tc.Crm.OutboundSynchronisation.CustomerTests.Services
{
    [TestClass]
    public class OutboundSynchronisationServiceTests
    {
        private ILogger logger;
        private IConfigurationService configurationService;
        private IOutboundSynchronisationDataService outboundSynchronisationService;
        private TestCrmService crmService;
        private IJwtService jwtService;
        private OutboundJsonWebTokenPayload _payload;

        private readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        [TestInitialize]
        public void Setpup()
        {
            _payload = new OutboundJsonWebTokenPayload
            {
                NotBefore = GetNotBeforeTime("100").ToString(),
                Expiry = GetExpiry("100").ToString()
            };

            this.logger = new TestLogger();
            this.configurationService = new TestConfigurationService();
            this.crmService = new TestCrmService();
            this.jwtService = new JwtService(logger);
            this.outboundSynchronisationService = new OutboundSynchronisationDataService(this.logger, this.crmService, jwtService);
        }

        /// <summary>
        /// No cacheenitity records were returned from service layer
        /// Expected Result: No records will be updated or processed. A null is returned
        /// No exceptions will be thrown.
        /// </summary>
        [TestMethod]
        public void RunTest_CacheEntityCollectionIsNulld()
        {
            crmService.Switch = DataSwitch.NoRecordsReturned;
            var service = new OutboundSynchronisationService(logger, outboundSynchronisationService, configurationService);
            service.Run();
            Assert.IsTrue(true);
        }

        /// <summary>
        /// No cacheenitity records were returned from service layer
        /// Expected Result: No records will be updated or processed. An empty EntityCollection instance is returned.
        /// No exceptions will be thrown.
        /// </summary>
        [TestMethod]
        public void RunTest_CacheEntityCollectionIsEmpty()
        {
            crmService.Switch = DataSwitch.CollectionWithNoRecordsReturned;
            var service = new OutboundSynchronisationService(logger, outboundSynchronisationService, configurationService);
            service.Run();
            Assert.IsTrue(true);
        }

        /// <summary>
        /// All needed records are retrieved
        /// Expected Result: Not null value is returned and collection is not empty
        /// </summary>
        [TestMethod()]
        public void RunTest_CacheEntityCollectionContainsAllNecessaryRecords()
        {
            crmService.Switch = DataSwitch.ReturnsData;
            crmService.PrepareData();

            var context = crmService.Context;
            var cacheEntities = context.Data["tc_entitycache"];
            var expectedCacheEntities = cacheEntities;

            var service = new OutboundSynchronisationService(logger, outboundSynchronisationService, configurationService);
            service.Run();

            Assert.AreEqual(expectedCacheEntities, cacheEntities);
        }

        /// <summary>
        /// All active records are retrieved
        /// Expected Result: Not null value is returned and collection is not empty
        /// </summary>
        [TestMethod]
        public void RunTest_CacheEntityCollectionReturnOnlyActiveRecords()
        {
            crmService.Switch = DataSwitch.ReturnsData;
            crmService.PrepareData();

            var context = crmService.Context;
            var cacheEntities = context.Data["tc_entitycache"];
            var cacheEntity = cacheEntities.Values.ToList()[0];
            crmService.UpdateStatus(cacheEntity, 2);

            var expectedCacheEntities = outboundSynchronisationService.RetrieveEntityCaches("contact", 10000);

            var service = new OutboundSynchronisationService(logger, outboundSynchronisationService, configurationService);
            service.Run();

            Assert.AreNotSame(expectedCacheEntities, cacheEntities);
        }

        /// <summary>
        /// JWt token is created
        /// Expected Result: Not null value is returned and token is not empty
        /// </summary>
        [TestMethod]
        public void CreateJwtTokenTest()
        {
            var fileName = ConfigurationManager.AppSettings["privateKeyFileName"];
            var privateKey = File.ReadAllText(fileName);
            var token = outboundSynchronisationService.CreateJwtToken(privateKey, _payload);
            Assert.IsNotNull(token);
        }

        /// <summary>
        /// JWt token is created
        /// Expected Result: Not null value is returned and content is not empty
        /// </summary>
        [TestMethod]
        public void SendPostHttpRequest()
        {
            var fileName = ConfigurationManager.AppSettings["privateKeyFileName"];
            var privateKey = File.ReadAllText(fileName);
            var token = outboundSynchronisationService.CreateJwtToken(privateKey, _payload);
            Assert.IsNotNull(token);

            var guid = Guid.NewGuid();
            var model = new EntityModel
            {
                Fields = new List<Field> {
                    new Field { Name = "tc_sourcesystemid", Type = FieldType.Guid, Value = guid },
                    new Field { Name = Attributes.Customer.Salutation, Type = FieldType.String, Value = "salutation" },
                    new Field { Name = Attributes.Customer.FirstName, Type = FieldType.String, Value = "firstname" },
                    new Field { Name = Attributes.Customer.LastName, Type = FieldType.String, Value = "lastname" },
                    new Field { Name = Attributes.Customer.Language, Type = FieldType.String, Value = "language" },
                    new Field { Name = Attributes.Customer.Birthdate, Type = FieldType.DateTime, Value = DateTime.Now.Date }
                }
            };

            var creator = new CreateCustomerRequestPayloadCreator();
            var payload = creator.GetPayload(model);
            var serviceUrl = ConfigurationManager.AppSettings["endPoint"]; 
            Assert.IsNotNull(payload);

            var result = outboundSynchronisationService.SendHttpPostRequest(serviceUrl, token, payload);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Content);
        }

        /// <summary>
        /// SendHttpPostRequest throws an exeption
        /// Expected Result: Exception ArgumentNullException is thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SendPostHttpRequest1()
        {
            outboundSynchronisationService.SendHttpPostRequest(string.Empty, string.Empty, string.Empty);
        }

        #region Private Methods

        private double GetExpiry(string expiredSeconds)
        {
            int sec;
            int.TryParse(expiredSeconds, out sec);
            return Math.Round((DateTime.UtcNow - unixEpoch).TotalSeconds + sec);
        }

        private double GetNotBeforeTime(string notBeforeSeconds)
        {
            int sec;
            int.TryParse(notBeforeSeconds, out sec);
            return Math.Round((DateTime.UtcNow - unixEpoch).TotalSeconds + sec);
        }

        #endregion
    }
}