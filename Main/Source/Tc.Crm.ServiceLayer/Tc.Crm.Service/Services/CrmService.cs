using System;
using Microsoft.Xrm.Sdk;
using System.Configuration;
using tcm = Tc.Crm.Service.Models;
using Newtonsoft.Json;
using Microsoft.Xrm.Tooling.Connector;

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
    }
}