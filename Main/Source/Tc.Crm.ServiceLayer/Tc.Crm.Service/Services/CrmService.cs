using System;
using Microsoft.Xrm.Sdk;
using System.Configuration;
using tcm = Tc.Crm.Service.Models;
using Newtonsoft.Json;
using Microsoft.Xrm.Tooling.Connector;
using System.Collections.ObjectModel;
using Microsoft.Xrm.Sdk.Query;

namespace Tc.Crm.Service.Services
{

    public class CrmService : ICrmService,IDisposable
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

            return new tcm.UpdateResponse { Created= responseObject.Created, Id=responseObject.Id};
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

            var connectionString =  ConfigurationManager.ConnectionStrings[Constants.Configuration.ConnectionStrings.Crm];
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
                if (orgService != null ) ((IDisposable)orgService).Dispose();
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
            var records =orgService.RetrieveMultiple(exp);
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

        public Collection<tcm.Gateway> GetGateways()
        {
            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='tc_gateway'>
                                <attribute name='tc_iata' />
                                <attribute name='tc_gatewayid' />
                              </entity>
                            </fetch>";
            FetchExpression exp = new FetchExpression(fetchXml);
            var records = orgService.RetrieveMultiple(exp);
            if (records == null || records.Entities.Count == 0) return null;

            var gateways = new Collection<tcm.Gateway>();
            foreach (var item in records.Entities)
            {
                gateways.Add(new Models.Gateway { Code = item["tc_iata"].ToString(), Id = item.Id.ToString() });
            };

            return gateways;
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
                sourceMarkets.Add(new Models.SourceMarket { Code = item["tc_iso2code"].ToString()
                                                            , Id = item.Id.ToString()
                                                             , BusinessUnitId = ((EntityReference)item["tc_sourcemarketbusinessunitid"]).Id.ToString()
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
    }
}