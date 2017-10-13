using System;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Tc.Crm.Plugins;
using Tc.Crm.Plugins.Merge.BusinessLogic;
using Attributes = Tc.Crm.Plugins.Attributes;


namespace Tc.Crm.UnitTests.Plugins
{
    [TestClass]
    public class EntityCacheMessageOutcomeServiceTests
    {
        private ITracingService trace;
        private IOrganizationService service;
        private IPluginExecutionContext pluginExecutionContext;
        private IEntityMergeFactory entityMergeFactory;
        private EntityCacheMessageOutcomeService entityCacheMessageOutcomeService;
        private EntityMerge entityMerge;

        [TestInitialize]
        public void Initialize()
        {
            trace = A.Fake<ITracingService>();
            service = A.Fake<IOrganizationService>();
            entityMergeFactory = A.Fake<IEntityMergeFactory>();
            pluginExecutionContext = A.Fake<IPluginExecutionContext>();
            entityCacheMessageOutcomeService = new EntityCacheMessageOutcomeService(trace, service, entityMergeFactory);
            entityMerge = A.Fake<EntityMerge>();
        }

        [TestMethod]
        public void TestOutcomeIdDoesNotExist()
        {
            // Given
            const string outcomeId = "123";
            const string cacheMessageName = "entity cache message 1";
            const string entityCacheName = "entity cache name 1";
            var customerRecordId = Guid.NewGuid();
            var entityCacheMessageId = Guid.NewGuid();
            var entityCacheId = Guid.NewGuid();

            var target = new Entity(Entities.EntityCacheMessage) { Id = entityCacheMessageId };
            var inputParameters = new ParameterCollection();
            inputParameters.Add(InputParameters.Target, target);

            A.CallTo(() => pluginExecutionContext.InputParameters).Returns(inputParameters);

            var entityCacheMessage = new Entity(Entities.EntityCacheMessage, entityCacheMessageId)
            {
                [Attributes.EntityCacheMessage.Name] = cacheMessageName,
                [Attributes.EntityCacheMessage.EntityCacheId] = new EntityReference(Entities.EntityCache, entityCacheId),
                [Attributes.EntityCacheMessage.OutcomeId] = outcomeId
            };

            A.CallTo(() => service.Retrieve(Entities.EntityCacheMessage, entityCacheMessageId,
                    A<ColumnSet>.That.Matches(column => column.Columns[0] == Attributes.EntityCacheMessage.Name &&
                                                        column.Columns[1] == Attributes.EntityCacheMessage.EntityCacheId &&
                                                        column.Columns[2] == Attributes.EntityCacheMessage.OutcomeId)))
                .Returns(entityCacheMessage);

            var entityCache = new Entity(Entities.EntityCache, entityCacheId)
            {
                [Attributes.EntityCache.EntityCacheId] = entityCacheId,
                [Attributes.EntityCache.Name] = entityCacheName,
                [Attributes.EntityCache.RecordId] = customerRecordId,
                [Attributes.EntityCache.Type] = "contact"
            };

            A.CallTo(() => service.Retrieve(Entities.EntityCache, entityCacheId,
                    A<ColumnSet>.That.Matches(column => column.Columns[0] == Attributes.EntityCache.Name &&
                                                        column.Columns[1] == Attributes.EntityCache.EntityCacheId &&
                                                        column.Columns[2] == Attributes.EntityCache.RecordId &&
                                                        column.Columns[3] == Attributes.EntityCache.Type)))
                .Returns(entityCache);

            A.CallTo(() => entityMergeFactory.GetEntityMerge(EntityCacheType.Customer, service)).Returns(entityMerge);

            A.CallTo(() => entityMerge.GetExistingEntity(outcomeId)).Returns(Guid.Empty);

            // When
            entityCacheMessageOutcomeService.Run(pluginExecutionContext);

            // Then
            A.CallTo(() => trace.Trace($"EntityCacheMessage was retrieved: {cacheMessageName}")).MustHaveHappened();
            A.CallTo(() => trace.Trace($"EntityCache was retrieved: {entityCacheName}")).MustHaveHappened();
            A.CallTo(() => entityMerge.UpdateSourceSystemId(customerRecordId, outcomeId)).MustHaveHappened();
            A.CallTo(() => trace.Trace($"Source System Id for Record Id from EntityCache: {customerRecordId} - was updated")).MustHaveHappened();
        }

        [TestMethod]
        public void TestOutcomeIdExistsForSameCustomerIdAndEntityCacheRecordId()
        {
            // Given
            const string outcomeId = "123";
            const string cacheMessageName = "entity cache message 1";
            const string entityCacheName = "entity cache name 1";
            var customerRecordId = Guid.NewGuid();
            var entityCacheMessageId = Guid.NewGuid();
            var entityCacheRecordId = customerRecordId;
            var entityCacheId = Guid.NewGuid();

            var target = new Entity(Entities.EntityCacheMessage) { Id = entityCacheMessageId };
            var inputParameters = new ParameterCollection();
            inputParameters.Add(InputParameters.Target, target);

            A.CallTo(() => pluginExecutionContext.InputParameters).Returns(inputParameters);

            var entityCacheMessage = new Entity(Entities.EntityCacheMessage, entityCacheMessageId)
            {
                [Attributes.EntityCacheMessage.Name] = cacheMessageName,
                [Attributes.EntityCacheMessage.EntityCacheId] = new EntityReference(Entities.EntityCache, entityCacheId),
                [Attributes.EntityCacheMessage.OutcomeId] = outcomeId
            };

            A.CallTo(() => service.Retrieve(Entities.EntityCacheMessage, entityCacheMessageId,
                    A<ColumnSet>.That.Matches(column => column.Columns[0] == Attributes.EntityCacheMessage.Name &&
                                                        column.Columns[1] == Attributes.EntityCacheMessage.EntityCacheId &&
                                                        column.Columns[2] == Attributes.EntityCacheMessage.OutcomeId)))
                .Returns(entityCacheMessage);

            var entityCache = new Entity(Entities.EntityCache, entityCacheId)
            {
                [Attributes.EntityCache.EntityCacheId] = entityCacheId,
                [Attributes.EntityCache.Name] = entityCacheName,
                [Attributes.EntityCache.RecordId] = entityCacheRecordId,
                [Attributes.EntityCache.Type] = "contact"
            };

            A.CallTo(() => service.Retrieve(Entities.EntityCache, entityCacheId,
                    A<ColumnSet>.That.Matches(column => column.Columns[0] == Attributes.EntityCache.Name &&
                                                        column.Columns[1] == Attributes.EntityCache.EntityCacheId &&
                                                        column.Columns[2] == Attributes.EntityCache.RecordId &&
                                                        column.Columns[3] == Attributes.EntityCache.Type)))
                .Returns(entityCache);

            A.CallTo(() => entityMergeFactory.GetEntityMerge(EntityCacheType.Customer, service)).Returns(entityMerge);

            A.CallTo(() => entityMerge.GetExistingEntity(outcomeId)).Returns(customerRecordId);

            // When
            entityCacheMessageOutcomeService.Run(pluginExecutionContext);

            // Then
            A.CallTo(() => trace.Trace($"EntityCacheMessage was retrieved: {cacheMessageName}")).MustHaveHappened();
            A.CallTo(() => trace.Trace($"EntityCache was retrieved: {entityCacheName}")).MustHaveHappened();
            A.CallTo(() => trace.Trace($"Contact was retrieved: {customerRecordId}")).MustHaveHappened();
            A.CallTo(() => entityMerge.UpdateSourceSystemId(entityCacheRecordId, outcomeId)).MustHaveHappened();
            A.CallTo(() => trace.Trace($"Record Id from EntityCache and existing record Id are the same. Source System Id for: {entityCacheRecordId} - was updated")).MustHaveHappened();
        }

        [TestMethod]
        public void TestOutcomeIdExistsForDifferentCustomerIdAndEntityCacheRecordId()
        {
            // Given
            const string outcomeId = "123";
            const string cacheMessageName = "entity cache message 1";
            const string entityCacheName = "entity cache name 1";
            var customerRecordId = Guid.NewGuid();
            var entityCacheMessageId = Guid.NewGuid();
            var entityCacheId = Guid.NewGuid();
            var entityCacheRecordId = Guid.NewGuid();

            var target = new Entity(Entities.EntityCacheMessage) { Id = entityCacheMessageId };
            var inputParameters = new ParameterCollection();
            inputParameters.Add(InputParameters.Target, target);

            A.CallTo(() => pluginExecutionContext.InputParameters).Returns(inputParameters);

            var entityCacheMessage = new Entity(Entities.EntityCacheMessage, entityCacheMessageId)
            {
                [Attributes.EntityCacheMessage.Name] = cacheMessageName,
                [Attributes.EntityCacheMessage.EntityCacheId] = new EntityReference(Entities.EntityCache, entityCacheId),
                [Attributes.EntityCacheMessage.OutcomeId] = outcomeId
            };

            A.CallTo(() => service.Retrieve(Entities.EntityCacheMessage, entityCacheMessageId,
                    A<ColumnSet>.That.Matches(column => column.Columns[0] == Attributes.EntityCacheMessage.Name &&
                                                        column.Columns[1] == Attributes.EntityCacheMessage.EntityCacheId &&
                                                        column.Columns[2] == Attributes.EntityCacheMessage.OutcomeId)))
                .Returns(entityCacheMessage);

            var entityCache = new Entity(Entities.EntityCache, entityCacheId)
            {
                [Attributes.EntityCache.EntityCacheId] = entityCacheId,
                [Attributes.EntityCache.Name] = entityCacheName,
                [Attributes.EntityCache.RecordId] = entityCacheRecordId,
                [Attributes.EntityCache.Type] = "contact"
            };

            A.CallTo(() => service.Retrieve(Entities.EntityCache, entityCacheId,
                    A<ColumnSet>.That.Matches(column => column.Columns[0] == Attributes.EntityCache.Name &&
                                                        column.Columns[1] == Attributes.EntityCache.EntityCacheId &&
                                                        column.Columns[2] == Attributes.EntityCache.RecordId &&
                                                        column.Columns[3] == Attributes.EntityCache.Type)))
                .Returns(entityCache);

            A.CallTo(() => entityMergeFactory.GetEntityMerge(EntityCacheType.Customer, service)).Returns(entityMerge);

            A.CallTo(() => entityMerge.GetExistingEntity(outcomeId)).Returns(customerRecordId);

            // When
            entityCacheMessageOutcomeService.Run(pluginExecutionContext);

            // Then
            A.CallTo(() => trace.Trace($"EntityCacheMessage was retrieved: {cacheMessageName}")).MustHaveHappened();
            A.CallTo(() => trace.Trace($"EntityCache was retrieved: {entityCacheName}")).MustHaveHappened();
            A.CallTo(() => trace.Trace($"Contact was retrieved: {customerRecordId}")).MustHaveHappened();
            A.CallTo(() => entityMerge.UpdateDuplicateSourceSystemId(entityCacheRecordId, outcomeId)).MustHaveHappened();
            A.CallTo(() => trace.Trace($"Source System Id for Record Id from EntityCache: {entityCacheRecordId} - was updated")).MustHaveHappened();
            A.CallTo(() => entityMerge.UpdateDuplicateSourceSystemId(customerRecordId, outcomeId)).MustHaveHappened();
            A.CallTo(() => trace.Trace($"Source System Id for existing entity: {customerRecordId} - was updated")).MustHaveHappened();
            A.CallTo(() => entityMerge.CreateEntityMerge(cacheMessageName, customerRecordId, entityCacheRecordId)).MustHaveHappened();
            A.CallTo(() => trace.Trace($"Entity Merge: {cacheMessageName} - was created")).MustHaveHappened();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestPluginExecutionContextIsNull()
        {
            entityCacheMessageOutcomeService.Run(null);
            A.CallTo(() => trace.Trace("Plugin execution context is null")).MustHaveHappened();
        }

        [TestMethod]
        public void TestEntityCacheDoesNotExistForEntityCacheMessage()
        {
            // Given
            const string outcomeId = "123";
            const string cacheMessageName = "entity cache message 1";
            var entityCacheMessageId = Guid.NewGuid();
            var entityCacheId = Guid.NewGuid();

            var target = new Entity(Entities.EntityCacheMessage) { Id = entityCacheMessageId };
            var inputParameters = new ParameterCollection();
            inputParameters.Add(InputParameters.Target, target);

            A.CallTo(() => pluginExecutionContext.InputParameters).Returns(inputParameters);

            var entityCacheMessage = new Entity(Entities.EntityCacheMessage, entityCacheMessageId)
            {
                [Attributes.EntityCacheMessage.Name] = cacheMessageName,
                [Attributes.EntityCacheMessage.EntityCacheId] = new EntityReference(Entities.EntityCache, entityCacheId),
                [Attributes.EntityCacheMessage.OutcomeId] = outcomeId
            };

            A.CallTo(() => service.Retrieve(Entities.EntityCacheMessage, entityCacheMessageId,
                    A<ColumnSet>.That.Matches(column => column.Columns[0] == Attributes.EntityCacheMessage.Name &&
                                                        column.Columns[1] == Attributes.EntityCacheMessage.EntityCacheId &&
                                                        column.Columns[2] == Attributes.EntityCacheMessage.OutcomeId)))
                .Returns(entityCacheMessage);

            A.CallTo(() => service.Retrieve(Entities.EntityCache, entityCacheId,
                    A<ColumnSet>.That.Matches(column => column.Columns[0] == Attributes.EntityCache.Name &&
                                                        column.Columns[1] == Attributes.EntityCache.EntityCacheId &&
                                                        column.Columns[2] == Attributes.EntityCache.RecordId &&
                                                        column.Columns[3] == Attributes.EntityCache.Type)))
                .Returns(null);

            // When
            entityCacheMessageOutcomeService.Run(pluginExecutionContext);

            //Then
            A.CallTo(() => trace.Trace($"EntityCacheMessage was retrieved: {cacheMessageName}")).MustHaveHappened();
            A.CallTo(() => trace.Trace($"EntityCache for EntityCacheMessage: {cacheMessageName} does not exist")).MustHaveHappened();
        }
    }
}