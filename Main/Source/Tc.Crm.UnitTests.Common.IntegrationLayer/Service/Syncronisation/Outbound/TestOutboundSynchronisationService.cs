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

namespace Tc.Crm.UnitTests.Common.IL.Service.Syncronisation.Outbound
{
	[TestClass]
	public class TestOutboundSynchronisationService
	{
		private const int batchSize = 5;
		private const string entityName = "customer";
		private const string secretKey = "secretKey";
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

		[TestInitialize]
		public void TestInitialize()
		{
			logger = A.Fake<ILogger>();
			jwtService = A.Fake<IJwtService>();
			outboundSynchDataService = A.Fake<IOutboundSynchronisationDataService>();
			createRequestPayloadCreator = A.Fake<IRequestPayloadCreator>();
			updateRequestPayloadCreator = A.Fake<IRequestPayloadCreator>();
			outboundSyncConfigurationService = A.Fake<IOutboundSyncConfigurationService>();
			outboundSynchService = new OutboundSynchronisationService(
				logger,
				outboundSynchDataService,
				jwtService,
				createRequestPayloadCreator,
				updateRequestPayloadCreator,
				outboundSyncConfigurationService);

			A.CallTo(() => outboundSyncConfigurationService.BatchSize).Returns(batchSize);
			A.CallTo(() => outboundSyncConfigurationService.EntityName).Returns(entityName);
			A.CallTo(() => outboundSynchDataService.GetSecretKey()).Returns(secretKey);
			A.CallTo(() => outboundSyncConfigurationService.CreateServiceUrl).Returns(createServiceUrl);
			A.CallTo(() => outboundSyncConfigurationService.UpdateServiceUrl).Returns(updateServiceUrl);
		}

		[TestMethod]
		public void TestIfNoEntitiesActive()
		{
			// Given
			A.CallTo(() => outboundSynchDataService.GetCreatedEntityCacheToProcess(entityName, batchSize)).Returns(new List<EntityCache>());
			A.CallTo(() => outboundSynchDataService.GetUpdatedEntityCacheToProcess(entityName, batchSize)).Returns(new List<EntityCache>());

			// When
			outboundSynchService.Run();

			// Then
			A.CallTo(() => logger.LogInformation($"Configuration create record: {entityName}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"The number of records: {batchSize}")).MustHaveHappened();

			A.CallTo(() => logger.LogInformation($"Configuration update record: {entityName}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"The number of records: {batchSize}")).MustHaveHappened();
		}

		[TestMethod]
		public void TestCreateEntitiesActiveSuccessFlow()
		{
			// Given
			var dateTime = DateTime.Now;
			var payload = "payload";
			var entityCache = new List<EntityCache>()
			{
				new EntityCache()
				{
					SourceMarket = "sm1",
					Data = "{'Fields':[{'Name':'territorycode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}}]}",
					Id = Guid.NewGuid(),
					Name = "name1",
					Operation = null,
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
			outboundSynchService.Run();

			// Then
			A.CallTo(() => logger.LogInformation($"Configuration create record: {entityName}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"The number of records: {batchSize}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"The create endpoint: {createServiceUrl}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"EntityCache/EntityCacheMessage : {entityCache[0].Name}/{entityCacheMessage.Name}")).MustHaveHappened();
			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheStatus(entityCache[0].Id, (int)Status.Active, (int)EntityCacheStatusReason.InProgress)).MustHaveHappened();
			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheMessageStatus(messageGuid, (int)Status.Inactive, (int)EntityCacheMessageStatusReason.SuccessfullySentToIL, A<string>._)).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"EntityCacheMessage Status Reason: {Enum.GetName(typeof(EntityCacheMessageStatusReason), EntityCacheMessageStatusReason.SuccessfullySentToIL)}")).MustHaveHappened();
		}

		[TestMethod]
		public void TestCreateEntitiesActiveFailFlow()
		{
			// Given
			var dateTime = DateTime.Now;
			var payload = "payload";
			var entityCache = new List<EntityCache>()
			{
				new EntityCache()
				{
					SourceMarket = "sm1",
					Data = "{'Fields':[{'Name':'territorycode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}}]}",
					Id = Guid.NewGuid(),
					Name = "name1",
					Operation = null,
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
			outboundSynchService.Run();

			// Then
			A.CallTo(() => logger.LogInformation($"Configuration create record: {entityName}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"The number of records: {batchSize}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"The create endpoint: {createServiceUrl}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"EntityCache/EntityCacheMessage : {entityCache[0].Name}/{entityCacheMessage.Name}")).MustHaveHappened();
			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheStatus(entityCache[0].Id, (int)Status.Active, (int)EntityCacheStatusReason.InProgress)).MustHaveHappened();

			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheMessageStatus(messageGuid, (int)Status.Inactive, (int)EntityCacheMessageStatusReason.Failed, A<string>._)).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"EntityCacheMessage Status Reason: {Enum.GetName(typeof(EntityCacheMessageStatusReason), EntityCacheMessageStatusReason.Failed)}")).MustHaveHappened();
		}

		[TestMethod]
		public void TestCreateEntitiesActiveExceptionFlow()
		{
			// Given
			var dateTime = DateTime.Now;
			var payload = "payload";
			var entityCache = new List<EntityCache>()
			{
				new EntityCache()
				{
					SourceMarket = "sm1",
					Data = "{'Fields':[{'Name':'territorycode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}}]}",
					Id = Guid.NewGuid(),
					Name = "name1",
					Operation = null,
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
			outboundSynchService.Run();

			// Then
			A.CallTo(() => logger.LogInformation($"Configuration create record: {entityName}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"The number of records: {batchSize}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"The create endpoint: {createServiceUrl}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"EntityCache/EntityCacheMessage : {entityCache[0].Name}/{entityCacheMessage.Name}")).MustHaveHappened();
			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheStatus(entityCache[0].Id, (int)Status.Active, (int)EntityCacheStatusReason.InProgress)).MustHaveHappened();

			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheMessageStatus(messageGuid, (int)Status.Inactive, (int)EntityCacheMessageStatusReason.Failed, A<string>._)).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"EntityCacheMessage Status Reason: {Enum.GetName(typeof(EntityCacheMessageStatusReason), EntityCacheMessageStatusReason.Failed)}")).MustHaveHappened();
		}

		[TestMethod]
		public void TestUpdateEntitiesActiveSuccessFlow()
		{
			// Given
			var dateTime = DateTime.Now;
			var payload = "payload";
			var entityCache = new List<EntityCache>()
			{
				new EntityCache()
				{
					SourceMarket = "sm1",
					Data = "{'Fields':[{'Name':'territorycode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}}]}",
					Id = Guid.NewGuid(),
					Name = "name1",
					Operation = null,
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
			outboundSynchService.Run();

			// Then
			A.CallTo(() => logger.LogInformation($"Configuration update record: {entityName}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"The number of records: {batchSize}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"The update endpoint: {updateServiceUrl}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"EntityCache/EntityCacheMessage : {entityCache[0].Name}/{entityCacheMessage.Name}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"EntityCache Status Reason: {Enum.GetName(typeof(EntityCacheStatusReason), EntityCacheStatusReason.InProgress)}")).MustHaveHappened();

			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheStatus(entityCache[0].Id, (int)Status.Active, (int)EntityCacheStatusReason.InProgress)).MustHaveHappened();
			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheMessageStatus(messageGuid, (int)Status.Inactive, (int)EntityCacheMessageStatusReason.SuccessfullySentToIL, A<string>._)).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"EntityCacheMessage Status Reason: {Enum.GetName(typeof(EntityCacheMessageStatusReason), EntityCacheMessageStatusReason.SuccessfullySentToIL)}")).MustHaveHappened();
		}

		[TestMethod]
		public void TestUpdateEntitiesActiveFailFlow()
		{
			// Given
			var dateTime = DateTime.Now;
			var payload = "payload";
			var entityCache = new List<EntityCache>()
			{
				new EntityCache()
				{
					SourceMarket = "sm1",
					Data = "{'Fields':[{'Name':'territorycode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}}]}",
					Id = Guid.NewGuid(),
					Name = "name1",
					Operation = null,
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
			outboundSynchService.Run();

			// Then
			A.CallTo(() => logger.LogInformation($"Configuration update record: {entityName}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"The number of records: {batchSize}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"The update endpoint: {updateServiceUrl}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"EntityCache/EntityCacheMessage : {entityCache[0].Name}/{entityCacheMessage.Name}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"EntityCache Status Reason: {Enum.GetName(typeof(EntityCacheStatusReason), EntityCacheStatusReason.InProgress)}")).MustHaveHappened();

			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheStatus(entityCache[0].Id, (int)Status.Active, (int)EntityCacheStatusReason.InProgress)).MustHaveHappened();

			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheMessageStatus(messageGuid, (int)Status.Inactive, (int)EntityCacheMessageStatusReason.Failed, A<string>._)).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"EntityCacheMessage Status Reason: {Enum.GetName(typeof(EntityCacheMessageStatusReason), EntityCacheMessageStatusReason.Failed)}")).MustHaveHappened();
		}

		[TestMethod]
		public void TestUpdateEntitiesActiveExceptionFlow()
		{
			// Given
			var dateTime = DateTime.Now;
			var payload = "payload";
			var entityCache = new List<EntityCache>()
			{
				new EntityCache()
				{
					SourceMarket = "sm1",
					Data = "{'Fields':[{'Name':'territorycode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}}]}",
					Id = Guid.NewGuid(),
					Name = "name1",
					Operation = null,
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
			outboundSynchService.Run();

			// Then
			A.CallTo(() => logger.LogInformation($"Configuration update record: {entityName}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"The number of records: {batchSize}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"The update endpoint: {updateServiceUrl}")).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"EntityCache/EntityCacheMessage : {entityCache[0].Name}/{entityCacheMessage.Name}")).MustHaveHappened();
			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheStatus(entityCache[0].Id, (int)Status.Active, (int)EntityCacheStatusReason.InProgress)).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"EntityCache Status Reason: {Enum.GetName(typeof(EntityCacheStatusReason), EntityCacheStatusReason.InProgress)}")).MustHaveHappened();

			A.CallTo(() => outboundSynchDataService.UpdateEntityCacheMessageStatus(messageGuid, (int)Status.Inactive, (int)EntityCacheMessageStatusReason.Failed, A<string>._)).MustHaveHappened();
			A.CallTo(() => logger.LogInformation($"EntityCacheMessage Status Reason: {Enum.GetName(typeof(EntityCacheMessageStatusReason), EntityCacheMessageStatusReason.Failed)}")).MustHaveHappened();
		}
	}
}
