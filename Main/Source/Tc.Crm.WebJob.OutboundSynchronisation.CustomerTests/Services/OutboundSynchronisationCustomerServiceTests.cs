using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.Common.Services;
using Tc.Crm.OutboundSynchronisation.Customer.Services;

namespace Tc.Crm.OutboundSynchronisation.CustomerTests.Services
{
    [TestClass]
    public class OutboundSynchronisationCustomerServiceTests
    {
        private ILogger logger;
        private IConfigurationService configurationService;
        private IOutboundSynchronisationDataService outboundSynchronisationService;
        private TestCrmService crmService;


        [TestInitialize]
        public void Setpup()
        {
            this.logger = new TestLogger();
            this.configurationService = new TestConfigurationService();
            this.crmService = new TestCrmService();
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

            var expectedCacheEntities = outboundSynchronisationService.RetrieveEntityCaches("contact", 10000).Entities;

            var service = new OutboundSynchronisationService(logger, outboundSynchronisationService, configurationService);
            service.Run();

            Assert.AreNotSame(expectedCacheEntities, cacheEntities);
        }
    }
}