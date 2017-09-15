using System.Configuration;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.Common.IntegrationLayer.Service.Synchronisation.Outbound;
using Tc.Crm.Common.Services;

namespace Tc.Crm.UnitTests.Common.IL.Service.Syncronisation.Outbound
{
    [TestClass]
    public class TestOutboundSynchronisationDataService
    {
        private IOutboundSynchronisationDataService outboundSynchronisationDataService;
        private ILogger logger;
        private ICrmService crmService;
        private CrmServiceHelper crmServiceHelper;

        [TestInitialize]
        public void Setup()
        {
            logger = new TestLogger();
            crmServiceHelper = new CrmServiceHelper();
        }

        /// <summary>
        /// Entity caches collecction is retrieved 
        /// Expected Result: Not null value is returned and collections contains records
        /// </summary>
        [TestMethod]
        public void GetCreatedEntityCacheToProcessTest()
        {
            var expectedResult = 5;
            crmService = new TestCrmService(crmServiceHelper.GetCreatedCacheEntities);
            outboundSynchronisationDataService = new OutboundSynchronisationDataService(this.logger, this.crmService);
            var actualResult = outboundSynchronisationDataService.GetCreatedEntityCacheToProcess("contact", expectedResult);
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(expectedResult, actualResult.Count);
        }

        /// <summary>
        /// Entity caches collecction is retrieved 
        /// Expected Result: Not null value is returned and collections contains records
        /// </summary>
        [TestMethod]
        public void GetUpdatedEntityCacheToProcessTest()
        {
            var expectedResult = 5;
            crmService = new TestCrmService(crmServiceHelper.GetUpdatedCacheEntities);
            outboundSynchronisationDataService = new OutboundSynchronisationDataService(this.logger, this.crmService);
            var actualResult = outboundSynchronisationDataService.GetUpdatedEntityCacheToProcess("contact", expectedResult);
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(expectedResult, actualResult.Count);
        }

        /// <summary>
        /// Expiration value is retrieved
        /// Expected Result: Not null value is returned
        /// </summary>
        [TestMethod]
        public void GetExpiryTest()
        {
            var expectedResult = "100";
            crmService = new TestCrmService(crmServiceHelper.GetExpiry);
            outboundSynchronisationDataService = new OutboundSynchronisationDataService(this.logger, this.crmService);
            var actualResult = outboundSynchronisationDataService.GetExpiry();

            Assert.IsNotNull(actualResult);
            Assert.AreEqual(expectedResult, actualResult);
        }

        /// <summary>
        /// NotBeforeTime value is retrieved
        /// Expected Result: Not null value is returned
        /// </summary>
        [TestMethod]
        public void GetNotBeforeTimeTest()
        {
            var expectedResult = "100";
            crmService = new TestCrmService(crmServiceHelper.GetNotBeforeTime);
            outboundSynchronisationDataService = new OutboundSynchronisationDataService(this.logger, this.crmService);
            var actualResult = outboundSynchronisationDataService.GetNotBeforeTime();

            Assert.IsNotNull(actualResult);
            Assert.AreEqual(expectedResult, actualResult);
        }

        /// <summary>
        /// Integration Layer URL is retrieved
        /// Expected Result: Not null value is returned
        /// </summary>
        [TestMethod]
        public void GetServiceUrlTest()
        {
            var expectedResult = ConfigurationManager.AppSettings["endPoint"];
            crmService = new TestCrmService(crmServiceHelper.GetServiceUrl);
            outboundSynchronisationDataService = new OutboundSynchronisationDataService(this.logger, this.crmService);
            var actualResult = outboundSynchronisationDataService.GetServiceUrl();

            Assert.IsNotNull(actualResult);
            Assert.AreEqual(expectedResult, actualResult);
        }

        /// <summary>
        /// Secret key for JWT token is retrieved
        /// Expected Result: Not null value is returned
        /// </summary>
        [TestMethod]
        public void GetSecretKeyTest()
        {
            var config = ConfigurationManager.AppSettings["privateKeyFileName"];
            Assert.IsNotNull(config);

            var expectedResult = File.ReadAllText(config);
            Assert.IsNotNull(expectedResult);

            crmService = new TestCrmService(crmServiceHelper.GetPrivateKey);
            outboundSynchronisationDataService = new OutboundSynchronisationDataService(this.logger, this.crmService);
            var actualResult = outboundSynchronisationDataService.GetSecretKey();

            Assert.IsNotNull(actualResult);
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}