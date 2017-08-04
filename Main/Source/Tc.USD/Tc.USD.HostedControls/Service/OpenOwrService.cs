using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using Tc.Usd.HostedControls.Constants;

namespace Tc.Usd.HostedControls
{
    public partial class SingleSignOnController
    {
        public void GetSsoDetails()
        {
            var query = new QueryByAttribute("tc_externallogin")
            {
                ColumnSet =
                    new ColumnSet("tc_abtanumber", "tc_branchcode", "tc_employeeid", "tc_externalloginid", "tc_initials")
            };
            query.AddAttributeValue("ownerid", _client.CrmInterface.GetMyCrmUserId());
            var login = ExecuteQuery(query);
            if (login == null) return;
            var loginInfo =
                $"Abtanumber is {login.GetAttributeValue<string>("tc_abtanumber")}, BranchCode is {login.GetAttributeValue<string>("tc_branchcode")}, Employee Id is {login.GetAttributeValue<string>("tc_employeeid")}, Initials are {login.GetAttributeValue<string>("tc_initials")}";

            LogWriter.Log($"Requesting SSO Details result: {loginInfo}", System.Diagnostics.TraceEventType.Verbose);
        }

        public void GetOwrSsoServiceUrl()
        {
            var query = new QueryByAttribute("tc_configuration")
            {
                ColumnSet = new ColumnSet("tc_value")
            };
            query.AddAttributeValue("tc_name", DataKey.OwrUrlConfigName);
            var config = ExecuteQuery(query);
            var owrUrl = config?.GetAttributeValue<string>("tc_value");
            LogWriter.Log($"Retrieved {DataKey.OwrUrlConfigName} result: {owrUrl}",
                System.Diagnostics.TraceEventType.Verbose);
        }

        public void GetPrivateInfo()
        {
            var query = new QueryByAttribute("tc_secureconfiguration")
            {
                ColumnSet = new ColumnSet("tc_value")
            };
            query.AddAttributeValue("tc_name", DataKey.JwtPrivateKeyConfigName);
            var config = ExecuteQuery(query);
            var privateKey = config?.GetAttributeValue<string>("tc_value");
            if (privateKey != null)
            {
                LogWriter.Log(
                    $"Retrieved {DataKey.JwtPrivateKeyConfigName} result {DataKey.JwtPrivateKeyConfigName} is not null",
                    System.Diagnostics.TraceEventType.Verbose);
            }
        }
       

        private Entity ExecuteQuery(QueryByAttribute query)
        {
            var req = new RetrieveMultipleRequest
            {
                Query = query
            };
            try
            {
                var response =
                    (RetrieveMultipleResponse)
                    this._client.CrmInterface.ExecuteCrmOrganizationRequest(req,
                        $"Requesting {query.EntityName}");
                var record = response.EntityCollection?.Entities?.FirstOrDefault();
                return record;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                LogWriter.Log($"{this.ApplicationName} application terminated with an error::{ex.ToString()}",
                    System.Diagnostics.TraceEventType.Error);
            }
            catch (TimeoutException ex)
            {
                LogWriter.Log($"{this.ApplicationName} application terminated with an error::{ex.ToString()}",
                    System.Diagnostics.TraceEventType.Error);
            }
            catch (Exception ex)
            {
                LogWriter.Log($"{this.ApplicationName} application terminated with an error::{ex.ToString()}",
                    System.Diagnostics.TraceEventType.Error);
            }
            return null;
        }
    }
} 

