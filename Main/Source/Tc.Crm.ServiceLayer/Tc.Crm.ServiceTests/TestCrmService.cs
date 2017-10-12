using System;
using System.Collections.Generic;
using Tc.Crm.Service.Services;
using FakeXrmEasy;
using Tc.Crm.Service.Models;
using System.Collections.ObjectModel;
using Tc.Crm.Service.Constants.Crm;
using Tc.Crm.Common;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Common.Constants;
using Attributes = Tc.Crm.Common.Constants.Attributes;
using Microsoft.Xrm.Sdk.Query;

namespace Tc.Crm.ServiceTests
{
    public enum DataSwitch
    {
        Created,
        Updated,
        Response_NULL,
        Response_Failed,
        Return_NULL,
        ActionThrowsError
    }
    public class TestCrmService : ICrmService
    {
        XrmFakedContext context;
        IOrganizationService orgService;

        public DataSwitch Switch { get; set; }

        public TestCrmService(XrmFakedContext context)
        {
            this.context = context;
            orgService = context.GetFakedOrganizationService();
        }
        public Tc.Crm.Service.Models.UpdateResponse ExecuteActionForBookingUpdate(string data)
        {
            object Constants = null;
            if (Switch == DataSwitch.Created)
                return new Tc.Crm.Service.Models.UpdateResponse { Created = true, Id = Guid.NewGuid().ToString() };

            else if (Switch == DataSwitch.Updated)
                return new Tc.Crm.Service.Models.UpdateResponse { Created = false, Id = Guid.NewGuid().ToString() };

            else if (Switch == DataSwitch.Response_NULL)
                throw new InvalidOperationException(Tc.Crm.Service.Constants.Messages.ResponseFromCrmIsNull);

            else if (Switch == DataSwitch.Response_Failed)
                return new Tc.Crm.Service.Models.UpdateResponse { Created = false, Id = null};
            else if (Switch == DataSwitch.Return_NULL)
                return null;
            else if (Switch == DataSwitch.ActionThrowsError)
                throw new Exception("Action faulted");
            return null;
        }

		public string GetConfiguration(string name) => "configuration";

		public int GetEntityCacheMessageCount(Guid entityCacheId) => 5;

		public SurveyReturnResponse ExecuteActionForSurveyCreate(string data)
        {
            if (Switch == DataSwitch.Created)
                return new Tc.Crm.Service.Models.SurveyReturnResponse { FailedSurveys=new List<FailedSurvey>() };
        
            else if (Switch == DataSwitch.Response_NULL)
                throw new InvalidOperationException(Tc.Crm.Service.Constants.Messages.ResponseFromCrmIsNull);

            else if (Switch == DataSwitch.Response_Failed)
                throw new InvalidOperationException(Tc.Crm.Service.Constants.Messages.ResponseFromCrmIsNull);
            else if (Switch == DataSwitch.Return_NULL)
                return null;
            else if (Switch == DataSwitch.ActionThrowsError)
                throw new Exception("Action faulted");
            return null;
        }

        public Collection<Brand> GetBrands()
        {
            Collection<Brand> brands = new Collection<Brand>();
            brands.Add(new Brand { Code = "BUC", Id = "E1A4B6EE-1282-4913-B460-F80EA939529A" });
            return brands;
        }

        public Collection<Country> GetCountries()
        {
            Collection<Country> countries = new Collection<Country>();
            countries.Add(new Country { Code = "DE", Id = "9EC77C9B-A5D8-41BF-A363-F2C018E10305" });
            return countries;
        }

        public Collection<Currency> GetCurrencies()
        {
            Collection<Currency> currencies = new Collection<Currency>();
            currencies.Add(new Currency { Code = "EUR", Id = "B4AB639E-A230-4479-95EA-604401E092CB" });
            return currencies;
        }

        public Dictionary<string,string> GetGateways()
        {
            Dictionary<string, string> gatewaysDictionary = new Dictionary<string, string>();
            gatewaysDictionary.Add("HGR_Airport","54E0C5FE-D522-4148-9192-9C885BA3CED6");
            return gatewaysDictionary;
        }

        public Collection<TourOperator> GetTourOperators()
        {
            Collection<TourOperator> tos = new Collection<TourOperator>();
            tos.Add(new TourOperator { Code = "TO001", Id = "A228B648-418A-4BCE-8680-AD33740AAB79" });
            return tos;
        }

        public Collection<SourceMarket> GetSourceMarkets()
        {
            Collection<SourceMarket> countries = new Collection<SourceMarket>();
            countries.Add(new SourceMarket { Code = "DE", Id = "9EC77C9B-A5D8-41BF-A363-F2C018E10305", BusinessUnitId = "A3C4C04F-42F4-42EF-8AC6-E24746E1CAA2", TeamId= "99B4323E-5607-4FAE-88AB-5D2D82EB5078" });
            return countries;
        }

        public Collection<Hotel> GetHotels()
        {
            Collection<Hotel> countries = new Collection<Hotel>();
            countries.Add(new Hotel { Code = "hot001", Id = "273ABA3A-6B88-40B7-AF4E-E8215BB009FB", DestinationId = "7A93B49A-48B4-44C3-A0FA-A04F2872D811" });
            return countries;
        }

        public bool PingCRM()
        {
            if (Switch == DataSwitch.Response_Failed)
                return false;
            else
                return true;
        }

        /// <summary>
        /// To process entitycachemessage record without notes
        /// </summary>
        /// <param name="entityCacheMessageId"></param>
        /// <param name="outComeId"></param>
        /// <param name="status"></param>
        /// <param name="statusReason"></param>
        /// <returns></returns>
        public Guid ProcessEntityCacheMessage(Guid entityCacheMessageId, string outComeId, Status status, EntityCacheMessageStatusReason statusReason)
        {
            return ProcessEntityCacheMessage(entityCacheMessageId, outComeId, status, statusReason, null);
        }

        /// <summary>
        /// To process entitycachemessage record with notes
        /// </summary>
        /// <param name="entityCacheMessageId"></param>
        /// <param name="outComeId"></param>
        /// <param name="status"></param>
        /// <param name="statusReason"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public Guid ProcessEntityCacheMessage(Guid entityCacheMessageId, string outComeId, Status status, EntityCacheMessageStatusReason statusReason, string notes)
        {
            var entityCacheId = Guid.Empty;
            var entityCacheMessages = GetEntityCacheMessages(entityCacheMessageId);
            if (entityCacheMessages == null || entityCacheMessages.Entities.Count == 0) return entityCacheId;
            var entityCacheMessage = entityCacheMessages[0];
            if (!string.IsNullOrWhiteSpace(outComeId))
                entityCacheMessage.Attributes[Attributes.EntityCacheMessage.OutcomeId] = outComeId;
            entityCacheMessage.Attributes[Attributes.EntityCacheMessage.State] = new OptionSetValue((int)status);
            entityCacheMessage.Attributes[Attributes.EntityCacheMessage.StatusReason] = new OptionSetValue((int)statusReason);
            if (!string.IsNullOrWhiteSpace(notes))
            {
                entityCacheMessage.Attributes[Attributes.EntityCacheMessage.Notes] = GetAppendedNotes(entityCacheMessage, notes);
            }
            orgService.Update(entityCacheMessage);
            if (entityCacheMessage.Attributes.Contains(Attributes.EntityCacheMessage.EntityCacheId) && entityCacheMessage.Attributes[Attributes.EntityCacheMessage.EntityCacheId] != null)
            {
                entityCacheId = ((EntityReference)(entityCacheMessage.Attributes[Attributes.EntityCacheMessage.EntityCacheId])).Id;
            }
            return entityCacheId;
        }

        /// <summary>
        /// To get entitycachemessage based on id
        /// </summary>
        /// <param name="entityCacheMessageId"></param>
        /// <returns></returns>
        private EntityCollection GetEntityCacheMessages(Guid entityCacheMessageId)
        {
            var entityCacheMessages = orgService.RetrieveMultiple(new QueryExpression
            {
                EntityName = EntityName.EntityCacheMessage,
                Criteria = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                               {
                                new ConditionExpression
                                    {
                                        AttributeName = Attributes.EntityCacheMessage.EntityCacheMessageId,
                                        Operator = ConditionOperator.Equal,
                                        Values = { entityCacheMessageId }
                                    }
                                }
                },
                ColumnSet = new ColumnSet(new string[] { Attributes.EntityCacheMessage.EntityCacheId, Attributes.EntityCacheMessage.Notes })
            });
            return entityCacheMessages;
        }

        /// <summary>
        /// To get tc_notes of entitycachemessage
        /// </summary>
        /// <param name="entityCacheMessage"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        private string GetAppendedNotes(Entity entityCacheMessage, string notes)
        {
            if (entityCacheMessage != null && entityCacheMessage.Attributes.Contains(Attributes.EntityCacheMessage.Notes) && entityCacheMessage.Attributes[Attributes.EntityCacheMessage.Notes] != null)
                if (!string.IsNullOrWhiteSpace(entityCacheMessage.Attributes[Attributes.EntityCacheMessage.Notes].ToString()))
                    notes += entityCacheMessage.Attributes[Attributes.EntityCacheMessage.Notes].ToString();
            return notes;
        }

        /// <summary>
        /// To update entitycache status
        /// </summary>
        /// <param name="entityCacheId"></param>
        /// <param name="status"></param>
        /// <param name="statusReason"></param>
        public void ProcessEntityCache(Guid entityCacheId, Status status, EntityCacheStatusReason statusReason, bool WasLastOperationSuccessful = false, DateTime? time = null)
        {
            var entityCache = new Entity(EntityName.EntityCache, entityCacheId);
            entityCache.Attributes[Attributes.EntityCache.StatusReason] = new OptionSetValue((int)statusReason);
            entityCache.Attributes[Attributes.EntityCache.State] = new OptionSetValue((int)status);
            orgService.Update(entityCache);
        }

		public void ActivateRelatedPendingEntityCache(Guid entityCacheId)
		{

		}


		public CustomerResponse ExecuteActionOnCustomerEvent(string data, Actions.OperationType operation)
        {
            if (Switch == DataSwitch.Created)
                return new CustomerResponse { Create = true, Existing = false, Id = Guid.NewGuid().ToString() };

            if (Switch == DataSwitch.Updated)
                return new CustomerResponse { Updated = true, Existing = false, Id = Guid.NewGuid().ToString() };

            if (Switch == DataSwitch.Response_NULL)
                throw new InvalidOperationException(Tc.Crm.Service.Constants.Messages.ResponseFromCrmIsNull);

            if (Switch == DataSwitch.Response_Failed)
                return new CustomerResponse { Existing = false, Id = null };
            if (Switch == DataSwitch.Return_NULL)
                return null;
            if (Switch == DataSwitch.ActionThrowsError)
                throw new Exception("Action faulted");
            return null;
        }
    }
}
