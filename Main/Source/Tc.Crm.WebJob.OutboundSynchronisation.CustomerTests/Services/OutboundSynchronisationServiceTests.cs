using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.Common.IntegrationLayer.Jti.Service;
using Tc.Crm.Common.Services;
using Tc.Crm.Common.IntegrationLayer.Service.Synchronisation;
using Tc.Crm.Common.IntegrationLayer.Service.Synchronisation.Outbound;

namespace Tc.Crm.OutboundSynchronisation.CustomerTests.Services
{
    [TestClass]
    public class OutboundSynchronisationServiceTests
    {
        private ILogger logger;
        private IOutboundSyncConfigurationService configurationService;
        private IOutboundSynchronisationDataService outboundSynchronisationService;
        private TestCrmService crmService;
        private IJwtService jwtService;
        private IRequestPayloadCreator requestPayloadCreator;

        private readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        [TestInitialize]
        public void Setup()
        {
            this.logger = new TestLogger();
            this.configurationService = new TestOutboundSyncConfigurationService();
            this.crmService = new TestCrmService();
            this.jwtService = new TestJwtService();
            this.requestPayloadCreator = new TestRequestPayloadCreator();
            this.outboundSynchronisationService = new OutboundSynchronisationDataService(this.logger, this.crmService);
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
            var service = new OutboundSynchronisationService(logger, outboundSynchronisationService, jwtService, requestPayloadCreator, configurationService);
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
            var service = new OutboundSynchronisationService(logger, outboundSynchronisationService, jwtService, requestPayloadCreator, configurationService);
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

            var service = new OutboundSynchronisationService(logger, outboundSynchronisationService, jwtService, requestPayloadCreator, configurationService);
            service.Run();

            Assert.AreEqual(expectedCacheEntities, cacheEntities);
        }
    }
}