using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.WebServiceClient;
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
            var req = new RetrieveMultipleRequest
            {
                Query = query
            };
            var response =
                (RetrieveMultipleResponse)
                this._client.CrmInterface.ExecuteCrmOrganizationRequest(req, "Requesting SSO Details");
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
        }

        public void GetOwrSsoServiceUrl()
        {
            var owrUrl = GetConfigValue(DataKey.OwrUrlConfigName);
            LogWriter.Log($"Retrieved {DataKey.OwrUrlConfigName} result: {owrUrl}",
                System.Diagnostics.TraceEventType.Verbose);
        }

        public void GetPrivateInfo()
        {
            var impersonationId = GetConfigValue(DataKey.ImpersonationIdConfigName);
            if (impersonationId != null)
            {
                LogWriter.Log($"Retrieved {DataKey.ImpersonationIdConfigName} result: {impersonationId}",
                    System.Diagnostics.TraceEventType.Verbose);
                var privateKey = GetPrivateInfo(DataKey.JwtPrivateKeyConfigName, impersonationId);
                if (privateKey != null)
                {
                    LogWriter.Log(
                        $"Retrieved {DataKey.JwtPrivateKeyConfigName} result {DataKey.JwtPrivateKeyConfigName} is not null",
                        System.Diagnostics.TraceEventType.Verbose);
                }
            }
        }

        private string GetConfigValue(string configName)
        {
            var query = new QueryByAttribute("tc_configuration")
            {
                ColumnSet = new ColumnSet("tc_value")
            };
            query.AddAttributeValue("tc_name", configName);

            var req = new RetrieveMultipleRequest
            {
                Query = query
            };
            try
            {
                var response =
                    (RetrieveMultipleResponse)
                    this._client.CrmInterface.ExecuteCrmOrganizationRequest(req,
                        $"Requesting {configName}");
                var configValue =
                    response.EntityCollection.Entities.FirstOrDefault()?.GetAttributeValue<string>("tc_value");
                return configValue;
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

        private string GetPrivateInfo(string configName, string impersonationId)
        {

            var query = new QueryByAttribute("tc_secureconfiguration")
            {
                ColumnSet = new ColumnSet("tc_value")
            };
            query.AddAttributeValue("tc_name", configName);
            try
            {
                var conn = this._client.CrmInterface;
                OrganizationWebProxyClient client = conn.OrganizationWebProxyClient;
                var systemUserId = GetSystemUserId(client, impersonationId);
                if (systemUserId != Guid.Empty)
                {
                    client.CallerId = systemUserId;
                }
                var privateInfo = client.RetrieveMultiple(query);
                LogWriter.Log($"GetPrivateInfo count entities {privateInfo.Entities.Count}");
                var config = privateInfo.Entities?.FirstOrDefault();
                if (config != null)
                {
                    var configValue = config.GetAttributeValue<string>("tc_value");
                    return configValue;
                }
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

        private Guid GetSystemUserId(OrganizationWebProxyClient client, string impersonationId)
        {
            QueryExpression query = new QueryExpression("systemuser");
            FilterExpression filter = new FilterExpression(LogicalOperator.And);

            ConditionExpression byEmail = new ConditionExpression("domainname", ConditionOperator.Equal, impersonationId);
            filter.Conditions.Add(byEmail);

            query.Criteria.Filters.Add(filter);
            query.ColumnSet = new ColumnSet(true);
            try
            {
                var activeUsers = client.RetrieveMultiple(query);
                return activeUsers?[0]?.Id ?? Guid.Empty;
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
            return Guid.Empty;
        }
    }
}  

