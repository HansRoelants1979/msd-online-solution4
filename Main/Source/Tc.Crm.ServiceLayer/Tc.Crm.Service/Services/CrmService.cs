using System;
using Microsoft.Xrm.Sdk;
using System.Configuration;
using tcm = Tc.Crm.Service.Models;
using Newtonsoft.Json;
using Microsoft.Xrm.Tooling.Connector;
using System.Collections.ObjectModel;
using Microsoft.Xrm.Sdk.Query;
using System.Xml;
using System.IO;
using System.Text;
using Microsoft.Crm.Sdk.Messages;
using System.Collections.Generic;
using static Tc.Crm.Service.Constants.Crm.Actions;
using Tc.Crm.Common.Constants;
using Attributes = Tc.Crm.Common.Constants.Attributes;
using Tc.Crm.Common;
using Tc.Crm.Service.Constants;
using System.Linq;

namespace Tc.Crm.Service.Services
{

    public class CrmService : ICrmService, IDisposable
    {
        private IOrganizationService orgService;

        public CrmService()
        {
            orgService = CreateOrgService();
        }

        public tcm.UpdateResponse ExecuteActionForBookingUpdate(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentNullException(Constants.Parameters.Data);

            var request = new OrganizationRequest(Constants.Crm.Actions.ProcessBooking);
            request[Constants.Crm.Actions.ParameterData] = data;
            var response = orgService.Execute(request);
            if (response == null || response.Results == null ||
                !response.Results.ContainsKey(Constants.Crm.Actions.ProcessBookingResponse) ||
                response.Results[Constants.Crm.Actions.ProcessBookingResponse] == null)
                throw new InvalidOperationException(Constants.Messages.ResponseFromCrmIsNull);

            var actionResponse = response.Results[Constants.Crm.Actions.ProcessBookingResponse].ToString();

            var responseObject = JsonConvert.DeserializeObject<tcm.UpdateResponse>(actionResponse);

            return new tcm.UpdateResponse { Created = responseObject.Created, Id = responseObject.Id,ResponseCode=responseObject.ResponseCode,ResponseMessage = responseObject.ResponseMessage };
        }

        public tcm.CustomerResponse ExecuteActionOnCustomerEvent(string data, OperationType operation)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentNullException(Constants.Parameters.Data);

            var request = new OrganizationRequest(Constants.Crm.Actions.ProcessCustomer);
            request[Constants.Crm.Actions.CustomerData] = data;
            request[Constants.Crm.Actions.Operation] = Enum.GetName(typeof(OperationType), operation);
            var response = orgService.Execute(request);
            if (response == null || response.Results == null ||
                !response.Results.ContainsKey(Constants.Crm.Actions.ProcessCustomerResponse) ||
                response.Results[Constants.Crm.Actions.ProcessCustomerResponse] == null)
                throw new InvalidOperationException(Constants.Messages.ResponseFromCrmIsNull);

            var actionResponse = response.Results[Constants.Crm.Actions.ProcessCustomerResponse].ToString();

            var responseObject = JsonConvert.DeserializeObject<tcm.CustomerResponse>(actionResponse);

            return new tcm.CustomerResponse {
                Existing = responseObject.Existing,
                Create = responseObject.Create,
                Updated = responseObject.Updated,
                Id = responseObject.Id
            };
        }

        public tcm.SurveyReturnResponse ExecuteActionForSurveyCreate(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentNullException(Constants.Parameters.Data);

            var request = new OrganizationRequest(Constants.Crm.Actions.ProcessSurvey);
            request[Constants.Crm.Actions.ParameterSurveyData] = data;
            var response = orgService.Execute(request);
            if (response == null || response.Results == null ||
                !response.Results.ContainsKey(Constants.Crm.Actions.ProcessSurveyResponse) ||
                response.Results[Constants.Crm.Actions.ProcessSurveyResponse] == null)
                throw new InvalidOperationException(Constants.Messages.ResponseFromCrmIsNull);

            var actionResponse = response.Results[Constants.Crm.Actions.ProcessSurveyResponse].ToString();

            var responseObject = JsonConvert.DeserializeObject<tcm.SurveyReturnResponse>(actionResponse);

            return responseObject;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public IOrganizationService CreateOrgService()
        {
            if (orgService != null) return orgService;

            var connectionString = ConfigurationManager.ConnectionStrings[Constants.Configuration.ConnectionStrings.Crm];
            CrmServiceClient client = new CrmServiceClient(connectionString.ConnectionString);
            return (IOrganizationService)client;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                //dispose org service
                if (orgService != null) ((IDisposable)orgService).Dispose();
            }

            disposed = true;
        }

        public Collection<tcm.Brand> GetBrands()
        {
            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='tc_brand'>
                                <attribute name='tc_brandid' />
                                <attribute name='tc_brandcode' />
                              </entity>
                            </fetch>";
            FetchExpression exp = new FetchExpression(fetchXml);
            var records = orgService.RetrieveMultiple(exp);
            if (records == null || records.Entities.Count == 0) return null;

            var brands = new Collection<tcm.Brand>();
            foreach (var item in records.Entities)
            {
                brands.Add(new Models.Brand { Code = item["tc_brandcode"].ToString(), Id = item.Id.ToString() });
            };

            return brands;
        }

        public Collection<tcm.Country> GetCountries()
        {
            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='tc_country'>
                                <attribute name='tc_iso2code' />
                                <attribute name='tc_countryid' />
                              </entity>
                            </fetch>";
            FetchExpression exp = new FetchExpression(fetchXml);
            var records = orgService.RetrieveMultiple(exp);
            if (records == null || records.Entities.Count == 0) return null;

            var countries = new Collection<tcm.Country>();
            foreach (var item in records.Entities)
            {
                countries.Add(new Models.Country { Code = item["tc_iso2code"].ToString(), Id = item.Id.ToString() });
            };

            return countries;
        }

        public Collection<tcm.Currency> GetCurrencies()
        {
            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='transactioncurrency'>
                                <attribute name='transactioncurrencyid' />
                                <attribute name='isocurrencycode' />
                              </entity>
                            </fetch>";
            FetchExpression exp = new FetchExpression(fetchXml);
            var records = orgService.RetrieveMultiple(exp);
            if (records == null || records.Entities.Count == 0) return null;

            var currencies = new Collection<tcm.Currency>();
            foreach (var item in records.Entities)
            {
                currencies.Add(new Models.Currency { Code = item["isocurrencycode"].ToString(), Id = item.Id.ToString() });
            };

            return currencies;
        }

        private string GetGatewayType(int value)
        {
            string gatewayType = string.Empty;
            switch (value)
            {
                case 950000000:
                    gatewayType = Constants.GatewayType.Airport;
                    break;
                case 950000001:
                    gatewayType = Constants.GatewayType.Port;  
                    break;
                case 950000002:
                    gatewayType = Constants.GatewayType.TrainStation;
                    break;
                case 950000003:
                    gatewayType = Constants.GatewayType.Other;  
                    break;
                default:
                    gatewayType = Constants.GatewayType.Airport;
                    break;
            }
            return gatewayType;
        }       
        
        public Dictionary<string,string> GetGateways()
        {
            Dictionary<string, string> getWayItems = new Dictionary<string, string>();
            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='tc_gateway'>
                                <attribute name='tc_iata' />
                                <attribute name='tc_gatewaytype' /> 
                                <attribute name='tc_gatewayid' />
                              </entity>
                            </fetch>";
            FetchExpression exp = new FetchExpression(fetchXml);
            var records = orgService.RetrieveMultiple(exp);
            if (records == null || records.Entities.Count == 0) return null;
            foreach (var item in records.Entities){
                var iata = item[Attributes.Gateway.Iata].ToString();
                var typeValue = item.GetAttributeValue<OptionSetValue>(Attributes.Gateway.GatewayType).Value;
                var typeString = GetGatewayType((int)typeValue);
                var gateWay = new Models.Gateway { Code = $"{iata}_{typeString}", Id = item.Id.ToString()};
                getWayItems.Add(gateWay.Code, gateWay.Id);
            };
            return getWayItems;
        }
        public Collection<tcm.TourOperator> GetTourOperators()
        {
            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='tc_touroperator'>
                                <attribute name='tc_touroperatorcode' />
                                <attribute name='tc_touroperatorid' />
                              </entity>
                            </fetch>";
            FetchExpression exp = new FetchExpression(fetchXml);
            var records = orgService.RetrieveMultiple(exp);
            if (records == null || records.Entities.Count == 0) return null;

            var tourOperators = new Collection<tcm.TourOperator>();
            foreach (var item in records.Entities)
            {
                tourOperators.Add(new Models.TourOperator { Code = item["tc_touroperatorcode"].ToString(), Id = item.Id.ToString() });
            };

            return tourOperators;
        }

        public Collection<tcm.SourceMarket> GetSourceMarkets()
        {
            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='tc_country'>
                                <attribute name='tc_iso2code' />
                                <attribute name='tc_countryid' />
                                <attribute name='tc_sourcemarketbusinessunitid' />
                                <filter type='and'>
                                  <condition attribute='tc_sourcemarketbusinessunitid' operator='not-null' />
                                </filter>
                              </entity>
                            </fetch>";
            FetchExpression exp = new FetchExpression(fetchXml);
            var records = orgService.RetrieveMultiple(exp);
            if (records == null || records.Entities.Count == 0) return null;

            var sourceMarkets = new Collection<tcm.SourceMarket>();
            foreach (var item in records.Entities)
            {
                sourceMarkets.Add(new Models.SourceMarket
                {
                    Code = item["tc_iso2code"].ToString()
                                                            ,
                    Id = item.Id.ToString()
                                                             ,
                    BusinessUnitId = ((EntityReference)item["tc_sourcemarketbusinessunitid"]).Id.ToString()
                });
            };
            foreach (var item in sourceMarkets)
            {
                item.TeamId = GetDefaultOwner(item.BusinessUnitId);
            };


            return sourceMarkets;
        }

        private string GetDefaultOwner(string businessUnitId)
        {
            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='team'>
                                <attribute name='businessunitid' />
                                <attribute name='teamid' />
                                <attribute name='teamtype' />
                                <filter type='and'>
                                  <condition attribute='isdefault' operator='eq' value='1' />
                                  <condition attribute='businessunitid' operator='eq' value='{0}' />
                                </filter>
                              </entity>
                            </fetch>";
            fetchXml = string.Format(fetchXml, businessUnitId);
            FetchExpression exp = new FetchExpression(fetchXml);
            var records = orgService.RetrieveMultiple(exp);
            if (records == null || records.Entities.Count == 0) return null;
            return records.Entities[0].Id.ToString();

        }

        public Collection<tcm.Hotel> GetHotels()
        {
            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='tc_hotel'>
                                <attribute name='tc_masterhotelid' />
                                <attribute name='tc_hotelid' />
                                <order attribute='tc_masterhotelid' descending='false' />
                                <link-entity name='tc_location' from='tc_locationid' to='tc_locationid' visible='false' link-type='outer' alias='a'>
                                  <attribute name='tc_regionid' />
                                </link-entity>
                              </entity>
                            </fetch>";
           

            var hotels = new Collection<tcm.Hotel>();
            int pageNumber = 1;
            string pagingCookie = null;

            int fetchCount = 10000;
            while (true)
            {
                string xml = CreateXml(fetchXml, pagingCookie, pageNumber, fetchCount);

                FetchExpression exp = new FetchExpression(xml);
                var records = orgService.RetrieveMultiple(exp);
                if (records == null || records.Entities.Count == 0) return null;
                foreach (var item in records.Entities)
                {
                    var h = new Models.Hotel
                    {
                        Code = item["tc_masterhotelid"].ToString(),
                        Id = item.Id.ToString()
                    };
                    if (item.Contains("a.tc_regionid"))
                    {
                        var alias = item["a.tc_regionid"];
                        if (alias != null)
                        {
                            var aliasValue = ((AliasedValue)alias).Value;
                            if (aliasValue != null)
                                h.DestinationId = ((EntityReference)aliasValue).Id.ToString();
                        }
                    }
                    hotels.Add(h);
                };

                if (records.MoreRecords)
                {
                    pageNumber++;
                    pagingCookie = records.PagingCookie;
                }
                else
                {
                    break;
                }
            }
           
            return hotels;
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
        public Guid ProcessEntityCacheMessage(Guid entityCacheMessageId, string outComeId, Status status, EntityCacheMessageStatusReason statusReason, string notes = null)
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

		public string GetConfiguration(string name)
		{
			var query = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='{EntityName.Configuration}'>
                        <attribute name='{Attributes.Configuration.Name}' />
                        <attribute name='{Attributes.Configuration.Value}' />
                        <filter type='and'>
                          <condition attribute='{Attributes.Configuration.Name}' operator='eq' value='{name}' />
                        </filter>
                      </entity>
                    </fetch>";
			var response = orgService.RetrieveMultiple(new FetchExpression(query));
			var config = response?.Entities?.FirstOrDefault();
			var value = config?.GetAttributeValue<string>(Attributes.Configuration.Value);
			return value;
		}

		public int GetEntityCacheMessageCount(Guid entityCacheId)
		{
			var query = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'> 
								<entity name='{EntityName.EntityCacheMessage}'> 
									<attribute name='{Attributes.EntityCacheMessage.EntityCacheMessageId}' alias='totalCount' aggregate='count'/> 
									<filter type='and'>
									  <condition attribute='{Attributes.EntityCacheMessage.EntityCacheId}' operator='eq' value='{entityCacheId}' />
									</filter>
								</entity>
						</fetch>";
			EntityCollection result = orgService.RetrieveMultiple(new FetchExpression(query));
			var count = (int)((AliasedValue)result?.Entities?.FirstOrDefault()?["totalCount"]).Value;
			return count;
		}

		/// <summary>
		/// Activate earliest EntityCache with same recordId and in Pending status
		/// </summary>
		/// <param name="entityCacheId">Guid of processed EntityCache record</param>
		public void ActivateRelatedPendingEntityCache(Guid entityCacheId)
		{
			var fetchRecordIdQuery = $@"<fetch version='1.0' output-format='xml-platform' distinct='false' mapping='logical' >
									<entity name='tc_entitycache' >
										<attribute name='tc_recordid' />
										<filter type='and' >
											<condition attribute='tc_entitycacheid' operator='eq' value='{entityCacheId}' />
										</filter>
									</entity>
								</fetch>";
			EntityCollection requestResult = orgService.RetrieveMultiple(new FetchExpression(fetchRecordIdQuery));			
			var recordId = (string)requestResult.Entities[0]["tc_recordid"];
			var pendingRecordQuery = $@"<fetch top='1' version='1.0' output-format='xml-platform' distinct='false' mapping='logical'>
										<entity name='tc_entitycache' >
											<attribute name='tc_entitycacheid' />
											<filter type='and' >
												<condition attribute='statuscode' operator='eq' value='{(int)EntityCacheStatusReason.Pending}' />
												<condition attribute='tc_recordid' operator='eq' value='{recordId}' />
											</filter>
											<order attribute='createdon' />
										</entity>
									</fetch>";
			requestResult = orgService.RetrieveMultiple(new FetchExpression(pendingRecordQuery));
			if (requestResult.Entities.Count == 1)
			{
				var pendingRecordId = (Guid)requestResult.Entities[0]["tc_entitycacheid"];
				ProcessEntityCache(pendingRecordId, Status.Active, EntityCacheStatusReason.Active);
			}
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
		/// <param name="wasLastOperationSuccessful"></param>
		public void ProcessEntityCache(Guid entityCacheId, Status status, EntityCacheStatusReason statusReason, bool wasLastOperationSuccessful = false, DateTime? eligibleRetryTime = null)
        {
            var entityCache = new Entity(EntityName.EntityCache, entityCacheId);
			entityCache.Attributes[Attributes.EntityCache.StatusReason] = new OptionSetValue((int)statusReason);
            entityCache.Attributes[Attributes.EntityCache.State] = new OptionSetValue((int)status);
			entityCache.Attributes[Attributes.EntityCache.WasLastOperationSuccessful] = wasLastOperationSuccessful;
			if (eligibleRetryTime.HasValue)
			{
				entityCache.Attributes[Attributes.EntityCache.EligibleRetryTime] = eligibleRetryTime.Value;
			}

			orgService.Update(entityCache);
        }
       
        public string CreateXml(string xml, string cookie, int page, int count)
        {
            StringReader stringReader = new StringReader(xml);
            XmlTextReader reader = new XmlTextReader(stringReader);

            // Load document
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            return CreateXml(doc, cookie, page, count);
        }

        public string CreateXml(XmlDocument doc, string cookie, int page, int count)
        {
            XmlAttributeCollection attrs = doc.DocumentElement.Attributes;

            if (cookie != null)
            {
                XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
                pagingAttr.Value = cookie;
                attrs.Append(pagingAttr);
            }

            XmlAttribute pageAttr = doc.CreateAttribute("page");
            pageAttr.Value = System.Convert.ToString(page);
            attrs.Append(pageAttr);

            XmlAttribute countAttr = doc.CreateAttribute("count");
            countAttr.Value = System.Convert.ToString(count);
            attrs.Append(countAttr);

            StringBuilder sb = new StringBuilder(1024);
            StringWriter stringWriter = new StringWriter(sb);

            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            doc.WriteTo(writer);
            writer.Close();

            return sb.ToString();
        }

        public bool PingCRM()
        {
            try
            {
                WhoAmIRequest who = new WhoAmIRequest();
                var response = (WhoAmIResponse)orgService.Execute(who);
                var orgId = response.OrganizationId;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}