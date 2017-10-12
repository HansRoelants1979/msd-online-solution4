using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.Common;
using Tc.Crm.Common.IntegrationLayer.Jti.Models;
using Tc.Crm.Common.IntegrationLayer.Jti.Service;
using Tc.Crm.Common.IntegrationLayer.Service.Synchronisation;
using Tc.Crm.Common.IntegrationLayer.Service.Synchronisation.Outbound;
using Tc.Crm.Common.Models;
using Tc.Crm.Common.Services;
using Tc.Crm.Common.IntegrationLayer.Helper;

namespace Tc.Crm.UnitTests.Common.IL.Service.Syncronisation.Outbound
{
	[TestClass]
	public class TestOutboundSynchronisationService
	{
		private const int batchSize = 5;
		private const string entityName = "customer";
		private const string secretKey = "secretKey";
		private static readonly int[] retries = { 5, 10, 15 };
		private const double issuedAtTime = 100;
		private const string expiry = "exp";
		private const string notBeforeTime = "nbf";
		private const string jwtToken = "jwtToken";
		private const string createServiceUrl = "createServiceUrl";
		private const string updateServiceUrl = "updateServiceUrl";
		private const string entityCacheMessageName = "RecordId: {0}, EntityCacheId: {1}";

		private ILogger logger;
		private IOutboundSynchronisationDataService outboundSynchDataService;
		private IJwtService jwtService;
		private OutboundSynchronisationService outboundSynchService;
		private IRequestPayloadCreator createRequestPayloadCreator;
		private IRequestPayloadCreator updateRequestPayloadCreator;
		private IOutboundSyncConfigurationService outboundSyncConfigurationService;
		private IEntityModelDeserializer entityModelDeserializer; 

		[TestInitialize]
		public void TestInitialize()
		{
			logger = A.Fake<ILogger>();
			jwtService = A.Fake<IJwtService>();
			outboundSynchDataService = A.Fake<IOutboundSynchronisationDataService>();
			createRequestPayloadCreator = A.Fake<IRequestPayloadCreator>();
			updateRequestPayloadCreator = A.Fake<IRequestPayloadCreator>();
			outboundSyncConfigurationService = A.Fake<IOutboundSyncConfigurationService>();
			entityModelDeserializer = A.Fake<IEntityModelDeserializer>();

			A.CallTo(() => outboundSyncConfigurationService.BatchSize).Returns(batchSize);
			A.CallTo(() => outboundSyncConfigurationService.EntityName).Returns(entityName);
			A.CallTo(() => outboundSynchDataService.GetSecretKey()).Returns(secretKey);
			A.CallTo(() => outboundSynchDataService.GetRetries()).Returns(retries);
			A.CallTo(() => outboundSyncConfigurationService.CreateServiceUrl).Returns(createServiceUrl);
			A.CallTo(() => outboundSyncConfigurationService.UpdateServiceUrl).Returns(updateServiceUrl);
			A.CallTo(() => entityModelDeserializer.Deserialize(A<string>._)).Returns(
				new Crm.Common.IntegrationLayer.Model.EntityModel
				{
					Fields = new List<Crm.Common.IntegrationLayer.Model.Field>
					{
						new Crm.Common.IntegrationLayer.Model.Field
						{
							Name = Tc.Crm.Common.Constants.Attributes.Customer.FirstName,
							Type = Crm.Common.IntegrationLayer.Model.FieldType.String,
							Value = "Name"
						}
					}
				});

			outboundSynchService = new OutboundSynchronisationService(
				logger,
				outboundSynchDataService,
				jwtService,
				createRequestPayloadCreator,
				updateRequestPayloadCreator,
				outboundSyncConfigurationService,
				entityModelDeserializer);
		}

		[TestMethod]
		public void TestCreateNoRecordsToProcess()
		{
			// Given
			A.CallTo(() => outboundSynchDataService.GetCreatedEntityCacheToProcess(entityName, batchSize)).Returns(new List<EntityCache>());
			
			// When
			outboundSynchService.ProcessEntityCacheOperation(Operation.Create);
			
			// Then
			A.CallTo(() => logger.LogInformation($"Processing {Enum.GetName(typeof(EntityCacheMessageStatusReason), Operation.Create)} EntityCache for entity: {entityName}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"Integration layer endpoint: {createServiceUrl}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"Retrieving entity cache records to process with maximum batch size: {batchSize}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"Found 0 records to be processed")).MustHaveHappened();
		}

		[TestMethod]
		public void TestUpdateNoRecordsToProcess()
		{
			// Given
			A.CallTo(() => outboundSynchDataService.GetUpdatedEntityCacheToProcess(entityName, batchSize)).Returns(new List<EntityCache>());
			
			// When
			outboundSynchService.ProcessEntityCacheOperation(Operation.Create);

			// Then
			A.CallTo(() => logger.LogInformation($"Processing {Enum.GetName(typeof(EntityCacheMessageStatusReason), Operation.Create)} EntityCache for entity: {entityName}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"Integration layer endpoint: {createServiceUrl}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"Retrieving entity cache records to process with maximum batch size: {batchSize}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"Found 0 records to be processed")).MustHaveHappened();
		}

		[TestMethod]
		public void TestCreateSuccessResponse()
		{
			// Given
			var payload = "payload";
			var entityCache = new List<EntityCache>()
			{
				new EntityCache()
				{
					SourceMarket = "sm1",
					Data = "{'Fields':[{'Name':'territorycode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}}]}",
					Id = Guid.NewGuid(),
					Name = "name1",
					Operation = EntityCacheOperation.Create,
					RecordId = "id1",
					SourceSystemId = "ssId1",
					Type = "type1",
					RequestsCount = 1
				}
			};
			var entityCacheMessage = new EntityCacheMessage()
			{
				EntityCacheId = entityCache[0].Id,
				Name = string.Format(entityCacheMessageName, entityCache[0].RecordId, entityCache[0].Id)
			};
			var messageGuid = Guid.NewGuid();
			var responseEntity = new Crm.Common.IntegrationLayer.Model.ResponseEntity()
			{
				StatusCode = HttpStatusCode.Created,
				Content = "content",
				Cookies = new Dictionary<string, string>()
			};

			A.CallTo(() => outboundSynchDataService.GetCreatedEntityCacheToProcess(entityName, batchSize)).Returns(entityCache);
			A.CallTo(() => outboundSynchDataService.GetSecretKey()).Returns(secretKey);
			A.CallTo(() => jwtService.GetIssuedAtTime()).Returns(issuedAtTime);
			A.CallTo(() => outboundSynchDataService.GetExpiry()).Returns(expiry);
			A.CallTo(() => outboundSynchDataService.GetNotBeforeTime()).Returns(notBeforeTime);

			A.CallTo(() => jwtService.CreateJwtToken(
				secretKey,
				A<OutboundJsonWebTokenPayload>.That.Matches(el => el.Issuer == "msd" && el.Expiry == expiry && el.IssuedAtTime == issuedAtTime.ToString(CultureInfo.InvariantCulture) && el.NotBefore == notBeforeTime)))
				.Returns(jwtToken);

			A.CallTo(() => outboundSynchDataService.CreateEntityCacheMessage(A<EntityCacheMessage>.That.Matches(el => el.Name == entityCacheMessage.Name))).Returns(messageGuid);
			A.CallTo(() => createRequestPayloadCreator.GetPayload(A<Crm.Common.IntegrationLayer.Model.EntityModel>._)).Returns(payload);
			A.CallTo(() => jwtService.SendHttpRequest(HttpMethod.Post, createServiceUrl, jwtToken, payload, messageGuid.ToString())).Returns(responseEntity);

			// When
			outboundSynchService.ProcessEntityCacheOperation(Operation.Create);

			// Then
			A.CallTo(() => logger.LogInformation($"Processing {Enum.GetName(typeof(EntityCacheMessageStatusReason), Operation.Create)} EntityCache for entity: {entityName}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"Integration layer endpoint: {createServiceUrl}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"Retrieving entity cache records to process with maximum batch size: {batchSize}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation("Found 1 records to be processed")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"Executed call to integration layer.  EntityCacheMessage Status Reason: {Enum.GetName(typeof(EntityCacheMessageStatusReason), EntityCacheMessageStatusReason.SuccessfullySentToIL)}")).MustHaveHappened();

			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheStatus(entityCache[0].Id, Status.Active, EntityCacheStatusReason.InProgress)).MustHaveHappened();
			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheMessageStatus(messageGuid, Status.Inactive, EntityCacheMessageStatusReason.SuccessfullySentToIL, A<string>._)).MustHaveHappened();
			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheSendToIntegrationLayerStatus(entityCache[0].Id, true, null)).MustHaveHappened();
		}

		[TestMethod]
		public void TestCreateErrorResponseDoNextRetry()
		{
			// Given
			var payload = "payload";
			var entityCache = new List<EntityCache>()
			{
				new EntityCache()
				{
					SourceMarket = "sm1",
					Data = "{'Fields':[{'Name':'territorycode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}}]}",
					Id = Guid.NewGuid(),
					Name = "name1",
					Operation = EntityCacheOperation.Create,
					RecordId = "id1",
					SourceSystemId = "ssId1",
					Type = "type1",
					RequestsCount = 1
				}
			};
			var entityCacheMessage = new EntityCacheMessage()
			{
				EntityCacheId = entityCache[0].Id,
				Name = string.Format(entityCacheMessageName, entityCache[0].RecordId, entityCache[0].Id)
			};
			var messageGuid = Guid.NewGuid();
			var responseEntity = new Crm.Common.IntegrationLayer.Model.ResponseEntity()
			{
				StatusCode = HttpStatusCode.Unauthorized,
				Content = "content",
				Cookies = new Dictionary<string, string>()
			};

			A.CallTo(() => outboundSynchDataService.GetCreatedEntityCacheToProcess(entityName, batchSize)).Returns(entityCache);
			A.CallTo(() => outboundSynchDataService.GetSecretKey()).Returns(secretKey);
			A.CallTo(() => jwtService.GetIssuedAtTime()).Returns(issuedAtTime);
			A.CallTo(() => outboundSynchDataService.GetExpiry()).Returns(expiry);
			A.CallTo(() => outboundSynchDataService.GetNotBeforeTime()).Returns(notBeforeTime);

			A.CallTo(() => jwtService.CreateJwtToken(
				secretKey,
				A<OutboundJsonWebTokenPayload>.That.Matches(el => el.Issuer == "msd" && el.Expiry == expiry && el.IssuedAtTime == issuedAtTime.ToString(CultureInfo.InvariantCulture) && el.NotBefore == notBeforeTime)))
				.Returns(jwtToken);

			A.CallTo(() => outboundSynchDataService.CreateEntityCacheMessage(A<EntityCacheMessage>.That.Matches(el => el.Name == entityCacheMessage.Name))).Returns(messageGuid);
			A.CallTo(() => createRequestPayloadCreator.GetPayload(A<Crm.Common.IntegrationLayer.Model.EntityModel>._)).Returns(payload);
			A.CallTo(() => jwtService.SendHttpRequest(HttpMethod.Post, createServiceUrl, jwtToken, payload, messageGuid.ToString())).Returns(responseEntity);

			// When
			outboundSynchService.ProcessEntityCacheOperation(Operation.Create);

			// Then
			A.CallTo(() => logger.LogInformation($"Processing {Enum.GetName(typeof(EntityCacheMessageStatusReason), Operation.Create)} EntityCache for entity: {entityName}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"Integration layer endpoint: {createServiceUrl}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"Retrieving entity cache records to process with maximum batch size: {batchSize}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation("Found 1 records to be processed")).MustHaveHappened();

			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheStatus(entityCache[0].Id, Status.Active, EntityCacheStatusReason.InProgress)).MustHaveHappened();
			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheMessageStatus(messageGuid, Status.Inactive, EntityCacheMessageStatusReason.Failed, A<string>._)).MustHaveHappened();

			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheSendToIntegrationLayerStatus(entityCache[0].Id, false, A<DateTime>.That.Matches(d => d > DateTime.UtcNow.AddMinutes(5) && d < DateTime.UtcNow.AddMinutes(10)))).MustHaveHappened();

			A.CallTo(() => logger.LogInformation($"Executed call to integration layer.  EntityCacheMessage Status Reason: {Enum.GetName(typeof(EntityCacheMessageStatusReason), EntityCacheMessageStatusReason.Failed)}")).MustHaveHappened();
		}

		[TestMethod]
		public void TestCreateMaxRetries()
		{
			// Given
			var entityCache = new List<EntityCache>()
			{
				new EntityCache()
				{
					SourceMarket = "sm1",
					Data = "{'Fields':[{'Name':'territorycode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}}]}",
					Id = Guid.NewGuid(),
					Name = "name1",
					Operation = EntityCacheOperation.Create,
					RecordId = "id1",
					SourceSystemId = "ssId1",
					Type = "type1",
					RequestsCount = 4,
					StatusReason = EntityCacheStatusReason.InProgress
				}
			};
			var entityCacheMessage = new EntityCacheMessage()
			{
				EntityCacheId = entityCache[0].Id,
				Name = string.Format(entityCacheMessageName, entityCache[0].RecordId, entityCache[0].Id)
			};
			var messageGuid = Guid.NewGuid();
			var responseEntity = new Crm.Common.IntegrationLayer.Model.ResponseEntity()
			{
				StatusCode = HttpStatusCode.Unauthorized,
				Content = "content",
				Cookies = new Dictionary<string, string>()
			};

			A.CallTo(() => outboundSynchDataService.GetCreatedEntityCacheToProcess(entityName, batchSize)).Returns(entityCache);
			A.CallTo(() => outboundSynchDataService.GetSecretKey()).Returns(secretKey);
			A.CallTo(() => jwtService.GetIssuedAtTime()).Returns(issuedAtTime);
			A.CallTo(() => outboundSynchDataService.GetExpiry()).Returns(expiry);
			A.CallTo(() => outboundSynchDataService.GetNotBeforeTime()).Returns(notBeforeTime);

			A.CallTo(() => jwtService.CreateJwtToken(
				secretKey,
				A<OutboundJsonWebTokenPayload>.That.Matches(el => el.Issuer == "msd" && el.Expiry == expiry && el.IssuedAtTime == issuedAtTime.ToString(CultureInfo.InvariantCulture) && el.NotBefore == notBeforeTime)))
				.Returns(jwtToken);

			// When
			outboundSynchService.ProcessEntityCacheOperation(Operation.Create);

			// Then
			A.CallTo(() => logger.LogInformation($"Processing {Enum.GetName(typeof(EntityCacheMessageStatusReason), Operation.Create)} EntityCache for entity: {entityName}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"Integration layer endpoint: {createServiceUrl}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"Retrieving entity cache records to process with maximum batch size: {batchSize}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation("Found 1 records to be processed")).MustHaveHappened();

			A.CallTo(() => logger.LogInformation($"EntityCache record: name1 reached maximum retries 3 of calls to integration layer and will be failed")).MustHaveHappened();
			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheStatus(entityCache[0].Id, Status.Inactive, EntityCacheStatusReason.Failed)).MustHaveHappened();
			A.CallTo(() => jwtService.SendHttpRequest(A<HttpMethod>._, A<string>._, A<string>._, A<string>._, A<string>._)).MustNotHaveHappened();
		}

		[TestMethod]
		public void TestCreateExceptionThrownDoNextRetry()
		{
			// Given
			var payload = "payload";
			var entityCache = new List<EntityCache>()
			{
				new EntityCache()
				{
					SourceMarket = "sm1",
					Data = "{'Fields':[{'Name':'territorycode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}}]}",
					Id = Guid.NewGuid(),
					Name = "name1",
					Operation = EntityCacheOperation.Create,
					RecordId = "id1",
					SourceSystemId = "ssId1",
					Type = "type1",
					RequestsCount = 1
				}
			};
			var entityCacheMessage = new EntityCacheMessage()
			{
				EntityCacheId = entityCache[0].Id,
				Name = string.Format(entityCacheMessageName, entityCache[0].RecordId, entityCache[0].Id)
			};
			var messageGuid = Guid.NewGuid();

			A.CallTo(() => outboundSynchDataService.GetCreatedEntityCacheToProcess(entityName, batchSize)).Returns(entityCache);
			A.CallTo(() => outboundSynchDataService.GetSecretKey()).Returns(secretKey);
			A.CallTo(() => jwtService.GetIssuedAtTime()).Returns(issuedAtTime);
			A.CallTo(() => outboundSynchDataService.GetExpiry()).Returns(expiry);
			A.CallTo(() => outboundSynchDataService.GetNotBeforeTime()).Returns(notBeforeTime);

			A.CallTo(() => jwtService.CreateJwtToken(
				secretKey,
				A<OutboundJsonWebTokenPayload>.That.Matches(el => el.Issuer == "msd" && el.Expiry == expiry && el.IssuedAtTime == issuedAtTime.ToString(CultureInfo.InvariantCulture) && el.NotBefore == notBeforeTime)))
				.Returns(jwtToken);

			A.CallTo(() => outboundSynchDataService.CreateEntityCacheMessage(A<EntityCacheMessage>.That.Matches(el => el.Name == entityCacheMessage.Name))).Returns(messageGuid);
			A.CallTo(() => createRequestPayloadCreator.GetPayload(A<Crm.Common.IntegrationLayer.Model.EntityModel>._)).Returns(payload);
			A.CallTo(() => jwtService.SendHttpRequest(HttpMethod.Post, createServiceUrl, jwtToken, payload, messageGuid.ToString())).Throws(new Exception("error"));

			// When
			outboundSynchService.ProcessEntityCacheOperation(Operation.Create);

			// Then
			A.CallTo(() => logger.LogInformation($"Processing {Enum.GetName(typeof(EntityCacheMessageStatusReason), Operation.Create)} EntityCache for entity: {entityName}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"Integration layer endpoint: {createServiceUrl}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"Retrieving entity cache records to process with maximum batch size: {batchSize}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation("Found 1 records to be processed")).MustHaveHappened();

			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheStatus(entityCache[0].Id, Status.Active, EntityCacheStatusReason.InProgress)).MustHaveHappened();
			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheMessageStatus(messageGuid, Status.Inactive, EntityCacheMessageStatusReason.Failed, A<string>._)).MustHaveHappened();
			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheSendToIntegrationLayerStatus(entityCache[0].Id, false, A<DateTime>.That.Matches(d => d > DateTime.UtcNow.AddMinutes(5) && d < DateTime.UtcNow.AddMinutes(10)))).MustHaveHappened();

			A.CallTo(() => logger.LogInformation($"Exception thrown while executing call to service layer. EntityCacheMessage Status Reason: {Enum.GetName(typeof(EntityCacheMessageStatusReason), EntityCacheMessageStatusReason.Failed)}")).MustHaveHappened();
			A.CallTo(() => logger.LogError(A<string>._)).MustHaveHappened();
		}

		[TestMethod]
		public void TestUpdateSuccessResponse()
		{
			// Given
			var payload = "payload";
			var entityCache = new List<EntityCache>()
			{
				new EntityCache()
				{
					SourceMarket = "sm1",
					Data = "{'Fields':[{'Name':'territorycode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}}]}",
					Id = Guid.NewGuid(),
					Name = "name1",
					Operation = EntityCacheOperation.Update,
					RecordId = "id1",
					SourceSystemId = "ssId1",
					Type = "type1"
				}
			};
			var entityCacheMessage = new EntityCacheMessage()
			{
				EntityCacheId = entityCache[0].Id,
				Name = string.Format(entityCacheMessageName, entityCache[0].RecordId, entityCache[0].Id)
			};
			var messageGuid = Guid.NewGuid();
			var responseEntity = new Crm.Common.IntegrationLayer.Model.ResponseEntity()
			{
				StatusCode = HttpStatusCode.Created,
				Content = "content",
				Cookies = new Dictionary<string, string>()
			};

			A.CallTo(() => outboundSynchDataService.GetUpdatedEntityCacheToProcess(entityName, batchSize)).Returns(entityCache);
			A.CallTo(() => outboundSynchDataService.GetSecretKey()).Returns(secretKey);
			A.CallTo(() => jwtService.GetIssuedAtTime()).Returns(issuedAtTime);
			A.CallTo(() => outboundSynchDataService.GetExpiry()).Returns(expiry);
			A.CallTo(() => outboundSynchDataService.GetNotBeforeTime()).Returns(notBeforeTime);

			A.CallTo(() => jwtService.CreateJwtToken(
				secretKey,
				A<OutboundJsonWebTokenPayload>.That.Matches(el => el.Issuer == "msd" && el.Expiry == expiry && el.IssuedAtTime == issuedAtTime.ToString(CultureInfo.InvariantCulture) && el.NotBefore == notBeforeTime)))
				.Returns(jwtToken);

			A.CallTo(() => outboundSynchDataService.CreateEntityCacheMessage(A<EntityCacheMessage>.That.Matches(el => el.Name == entityCacheMessage.Name))).Returns(messageGuid);
			A.CallTo(() => updateRequestPayloadCreator.GetPayload(A<Crm.Common.IntegrationLayer.Model.EntityModel>._)).Returns(payload);
			A.CallTo(() => jwtService.SendHttpRequest(HttpMethod.Patch, updateServiceUrl + "/" + entityCache[0].SourceSystemId, jwtToken, payload, messageGuid.ToString())).Returns(responseEntity);

			// When
			outboundSynchService.ProcessEntityCacheOperation(Operation.Update);

			// Then
			A.CallTo(() => logger.LogInformation($"Processing {Enum.GetName(typeof(EntityCacheMessageStatusReason), Operation.Update)} EntityCache for entity: {entityName}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"Integration layer endpoint: {updateServiceUrl}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"Retrieving entity cache records to process with maximum batch size: {batchSize}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation("Found 1 records to be processed")).MustHaveHappened();

			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheStatus(entityCache[0].Id, Status.Active, EntityCacheStatusReason.InProgress)).MustHaveHappened();
			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheMessageStatus(messageGuid, Status.Inactive, EntityCacheMessageStatusReason.SuccessfullySentToIL, A<string>._)).MustHaveHappened();
			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheSendToIntegrationLayerStatus(entityCache[0].Id, true, null)).MustHaveHappened();

			A.CallTo(() => logger.LogInformation($"Executed call to integration layer.  EntityCacheMessage Status Reason: {Enum.GetName(typeof(EntityCacheMessageStatusReason), EntityCacheMessageStatusReason.SuccessfullySentToIL)}")).MustHaveHappened();
		}

		[TestMethod]
		public void TestUpdateErrorResponseDoNextRetry()
		{
			// Given
			var payload = "payload";
			var entityCache = new List<EntityCache>()
			{
				new EntityCache()
				{
					SourceMarket = "sm1",
					Data = "{'Fields':[{'Name':'territorycode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}}]}",
					Id = Guid.NewGuid(),
					Name = "name1",
					Operation = EntityCacheOperation.Update,
					RecordId = "id1",
					SourceSystemId = "ssId1",
					Type = "type1",
					RequestsCount = 1,
					StatusReason = EntityCacheStatusReason.InProgress
				}
			};
			var entityCacheMessage = new EntityCacheMessage()
			{
				EntityCacheId = entityCache[0].Id,
				Name = string.Format(entityCacheMessageName, entityCache[0].RecordId, entityCache[0].Id)
			};
			var messageGuid = Guid.NewGuid();
			var responseEntity = new Crm.Common.IntegrationLayer.Model.ResponseEntity()
			{
				StatusCode = HttpStatusCode.Unauthorized,
				Content = "content",
				Cookies = new Dictionary<string, string>()
			};

			A.CallTo(() => outboundSynchDataService.GetUpdatedEntityCacheToProcess(entityName, batchSize)).Returns(entityCache);
			A.CallTo(() => outboundSynchDataService.GetSecretKey()).Returns(secretKey);
			A.CallTo(() => jwtService.GetIssuedAtTime()).Returns(issuedAtTime);
			A.CallTo(() => outboundSynchDataService.GetExpiry()).Returns(expiry);
			A.CallTo(() => outboundSynchDataService.GetNotBeforeTime()).Returns(notBeforeTime);

			A.CallTo(() => jwtService.CreateJwtToken(
				secretKey,
				A<OutboundJsonWebTokenPayload>.That.Matches(el => el.Issuer == "msd" && el.Expiry == expiry && el.IssuedAtTime == issuedAtTime.ToString(CultureInfo.InvariantCulture) && el.NotBefore == notBeforeTime)))
				.Returns(jwtToken);

			A.CallTo(() => outboundSynchDataService.CreateEntityCacheMessage(A<EntityCacheMessage>.That.Matches(el => el.Name == entityCacheMessage.Name))).Returns(messageGuid);
			A.CallTo(() => updateRequestPayloadCreator.GetPayload(A<Crm.Common.IntegrationLayer.Model.EntityModel>._)).Returns(payload);
			A.CallTo(() => jwtService.SendHttpRequest(HttpMethod.Patch, updateServiceUrl + "/" + entityCache[0].SourceSystemId, jwtToken, payload, messageGuid.ToString())).Returns(responseEntity);

			// When
			outboundSynchService.ProcessEntityCacheOperation(Operation.Update);

			// Then
			A.CallTo(() => logger.LogInformation($"Processing {Enum.GetName(typeof(EntityCacheMessageStatusReason), Operation.Update)} EntityCache for entity: {entityName}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"Integration layer endpoint: {updateServiceUrl}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"Retrieving entity cache records to process with maximum batch size: {batchSize}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation("Found 1 records to be processed")).MustHaveHappened();

			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheStatus(entityCache[0].Id, Status.Active, EntityCacheStatusReason.InProgress)).MustHaveHappened();
			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheMessageStatus(messageGuid, Status.Inactive, EntityCacheMessageStatusReason.Failed, A<string>._)).MustHaveHappened();
			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheSendToIntegrationLayerStatus(entityCache[0].Id, false, A<DateTime>.That.Matches(d => d > DateTime.UtcNow.AddMinutes(5) && d < DateTime.UtcNow.AddMinutes(10)))).MustHaveHappened();

			A.CallTo(() => logger.LogInformation($"Executed call to integration layer.  EntityCacheMessage Status Reason: {Enum.GetName(typeof(EntityCacheMessageStatusReason), EntityCacheMessageStatusReason.Failed)}")).MustHaveHappened();
		}

		[TestMethod]
		public void TestUpdateMaxRetries()
		{
			// Given
			var entityCache = new List<EntityCache>()
			{
				new EntityCache()
				{
					SourceMarket = "sm1",
					Data = "{'Fields':[{'Name':'territorycode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}}]}",
					Id = Guid.NewGuid(),
					Name = "name1",
					Operation = EntityCacheOperation.Update,
					RecordId = "id1",
					SourceSystemId = "ssId1",
					Type = "type1",
					RequestsCount = 4,
					StatusReason = EntityCacheStatusReason.InProgress
				}
			};
			var entityCacheMessage = new EntityCacheMessage()
			{
				EntityCacheId = entityCache[0].Id,
				Name = string.Format(entityCacheMessageName, entityCache[0].RecordId, entityCache[0].Id)
			};
			var messageGuid = Guid.NewGuid();
			var responseEntity = new Crm.Common.IntegrationLayer.Model.ResponseEntity()
			{
				StatusCode = HttpStatusCode.Unauthorized,
				Content = "content",
				Cookies = new Dictionary<string, string>()
			};

			A.CallTo(() => outboundSynchDataService.GetUpdatedEntityCacheToProcess(entityName, batchSize)).Returns(entityCache);
			A.CallTo(() => outboundSynchDataService.GetSecretKey()).Returns(secretKey);
			A.CallTo(() => jwtService.GetIssuedAtTime()).Returns(issuedAtTime);
			A.CallTo(() => outboundSynchDataService.GetExpiry()).Returns(expiry);
			A.CallTo(() => outboundSynchDataService.GetNotBeforeTime()).Returns(notBeforeTime);

			A.CallTo(() => jwtService.CreateJwtToken(
				secretKey,
				A<OutboundJsonWebTokenPayload>.That.Matches(el => el.Issuer == "msd" && el.Expiry == expiry && el.IssuedAtTime == issuedAtTime.ToString(CultureInfo.InvariantCulture) && el.NotBefore == notBeforeTime)))
				.Returns(jwtToken);

			// When
			outboundSynchService.ProcessEntityCacheOperation(Operation.Update);

			// Then
			A.CallTo(() => logger.LogInformation($"Processing {Enum.GetName(typeof(EntityCacheMessageStatusReason), Operation.Update)} EntityCache for entity: {entityName}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"Integration layer endpoint: {updateServiceUrl}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"Retrieving entity cache records to process with maximum batch size: {batchSize}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation("Found 1 records to be processed")).MustHaveHappened();

			A.CallTo(() => logger.LogInformation($"EntityCache record: name1 reached maximum retries 3 of calls to integration layer and will be failed")).MustHaveHappened();
			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheStatus(entityCache[0].Id, Status.Inactive, EntityCacheStatusReason.Failed)).MustHaveHappened();
			A.CallTo(() => jwtService.SendHttpRequest(A<HttpMethod>._, A<string>._, A<string>._, A<string>._, A<string>._)).MustNotHaveHappened();
		}

		[TestMethod]
		public void TestUpdateExceptionThrownDoNextRetry()
		{
			// Given
			var payload = "payload";
			var entityCache = new List<EntityCache>()
			{
				new EntityCache()
				{
					SourceMarket = "sm1",
					Data = "{'Fields':[{'Name':'territorycode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}}]}",
					Id = Guid.NewGuid(),
					Name = "name1",
					Operation = EntityCacheOperation.Update,
					RecordId = "id1",
					SourceSystemId = "ssId1",
					Type = "type1",
					RequestsCount = 1
				}
			};
			var entityCacheMessage = new EntityCacheMessage()
			{
				EntityCacheId = entityCache[0].Id,
				Name = string.Format(entityCacheMessageName, entityCache[0].RecordId, entityCache[0].Id)
			};
			var messageGuid = Guid.NewGuid();

			A.CallTo(() => outboundSynchDataService.GetUpdatedEntityCacheToProcess(entityName, batchSize)).Returns(entityCache);
			A.CallTo(() => outboundSynchDataService.GetSecretKey()).Returns(secretKey);
			A.CallTo(() => jwtService.GetIssuedAtTime()).Returns(issuedAtTime);
			A.CallTo(() => outboundSynchDataService.GetExpiry()).Returns(expiry);
			A.CallTo(() => outboundSynchDataService.GetNotBeforeTime()).Returns(notBeforeTime);

			A.CallTo(() => jwtService.CreateJwtToken(
				secretKey,
				A<OutboundJsonWebTokenPayload>.That.Matches(el => el.Issuer == "msd" && el.Expiry == expiry && el.IssuedAtTime == issuedAtTime.ToString(CultureInfo.InvariantCulture) && el.NotBefore == notBeforeTime)))
				.Returns(jwtToken);

			A.CallTo(() => outboundSynchDataService.CreateEntityCacheMessage(A<EntityCacheMessage>.That.Matches(el => el.Name == entityCacheMessage.Name))).Returns(messageGuid);
			A.CallTo(() => updateRequestPayloadCreator.GetPayload(A<Crm.Common.IntegrationLayer.Model.EntityModel>._)).Returns(payload);
			A.CallTo(() => jwtService.SendHttpRequest(HttpMethod.Patch, updateServiceUrl + "/" + entityCache[0].SourceSystemId, jwtToken, payload, messageGuid.ToString())).Throws(new Exception("error"));

			// When
			outboundSynchService.ProcessEntityCacheOperation(Operation.Update);

			// Then
			A.CallTo(() => logger.LogInformation($"Processing {Enum.GetName(typeof(EntityCacheMessageStatusReason), Operation.Update)} EntityCache for entity: {entityName}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"Integration layer endpoint: {updateServiceUrl}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"Retrieving entity cache records to process with maximum batch size: {batchSize}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation("Found 1 records to be processed")).MustHaveHappened();

			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheStatus(entityCache[0].Id, Status.Active, EntityCacheStatusReason.InProgress)).MustHaveHappened();
			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheMessageStatus(messageGuid, Status.Inactive, EntityCacheMessageStatusReason.Failed, A<string>._)).MustHaveHappened();
			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheSendToIntegrationLayerStatus(entityCache[0].Id, false, A<DateTime>.That.Matches(d => d > DateTime.UtcNow.AddMinutes(5) && d < DateTime.UtcNow.AddMinutes(10)))).MustHaveHappened();

			A.CallTo(() => logger.LogInformation($"Exception thrown while executing call to service layer. EntityCacheMessage Status Reason: {Enum.GetName(typeof(EntityCacheMessageStatusReason), EntityCacheMessageStatusReason.Failed)}")).MustHaveHappened();
			A.CallTo(() => logger.LogError(A<string>._)).MustHaveHappened();
		}
	}
}
