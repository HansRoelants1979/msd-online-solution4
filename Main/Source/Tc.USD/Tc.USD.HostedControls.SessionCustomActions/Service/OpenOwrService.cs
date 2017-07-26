using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using Tc.USD.HostedControls.SessionCustomActions.Constants;

namespace Tc.USD.HostedControls.SessionCustomActions
{
    public partial class CustomAction
    {
        public void GetSsoDetails()
        {
            var query = new QueryByAttribute("tc_externallogin")
            {
                ColumnSet =
                    new ColumnSet("tc_abtanumber", "tc_branchcode", "tc_employeeid", "tc_externalloginid", "tc_initials")
            };
            query.AddAttributeValue("ownerid", _client.CrmInterface.GetMyCrmUserId());
            var req = new RetrieveMultipleRequest
            {
                Query = query
            };
            var response = (RetrieveMultipleResponse)this._client.CrmInterface.ExecuteCrmOrganizationRequest(req, "Requesting SSO Details");
            try
            {
                var login = response.EntityCollection.Entities.FirstOrDefault();
                if (login == null) return;
                var configInfo =
                    $"Abtanumber is {login.GetAttributeValue<string>("tc_abtanumber")}, BranchCode is {login.GetAttributeValue<string>("tc_branchcode")}, Employee Id is {login.GetAttributeValue<string>("tc_employeeid")}, Initials are {login.GetAttributeValue<string>("tc_initials")}";

                LogWriter.Log($"Requesting SSO Details result: {configInfo}", System.Diagnostics.TraceEventType.Verbose);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                LogWriter.Log($"{this.ApplicationName} application terminated with an error::Timestamp:{ex.Detail.Timestamp}, Code: {ex.Detail.ErrorCode}::Message: {ex.Detail.Message}", System.Diagnostics.TraceEventType.Error);
            }
            catch (TimeoutException ex)
            {
                LogWriter.Log($"{this.ApplicationName} application terminated with an error::Message: {ex.Message}::Stack Trace: {ex.StackTrace}", System.Diagnostics.TraceEventType.Error);
            }
            catch (Exception ex)
            {
                LogWriter.Log($"{this.ApplicationName} application terminated with an error:: Message:{ex.Message}::Stack Trace:{ex.StackTrace}", System.Diagnostics.TraceEventType.Error);
            }
        }

        public void GetOwrSsoServiceUrl()
        {
            var query = new QueryByAttribute("tc_configuration")
            {
                ColumnSet = new ColumnSet("tc_value")
            };
            query.AddAttributeValue("tc_name", DataKey.OwrUrlConfigName);
            var req = new RetrieveMultipleRequest
            {
                Query = query
            };
            try
            { 
            var response = (RetrieveMultipleResponse)this._client.CrmInterface.ExecuteCrmOrganizationRequest(req, "Requesting SSO Service URL");
            var url = response.EntityCollection.Entities.FirstOrDefault()?.GetAttributeValue<string>("tc_value");
            LogWriter.Log($"Requesting SSO Service URL result: {url}", System.Diagnostics.TraceEventType.Verbose);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                LogWriter.Log($"{this.ApplicationName} application terminated with an error::Timestamp:{ex.Detail.Timestamp}, Code: {ex.Detail.ErrorCode}::Message: {ex.Detail.Message}", System.Diagnostics.TraceEventType.Error);
            }
            catch (TimeoutException ex)
            {
                LogWriter.Log($"{this.ApplicationName} application terminated with an error::Message: {ex.Message}::Stack Trace: {ex.StackTrace}", System.Diagnostics.TraceEventType.Error);
            }
            catch (Exception ex)
            {
                LogWriter.Log($"{this.ApplicationName} application terminated with an error:: Message:{ex.Message}::Stack Trace:{ex.StackTrace}", System.Diagnostics.TraceEventType.Error);
            }
        }
    }
}
