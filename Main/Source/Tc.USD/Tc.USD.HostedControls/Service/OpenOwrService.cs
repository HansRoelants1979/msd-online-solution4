using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Tc.Usd.HostedControls.Constants;
using Tc.Usd.HostedControls.Models;
using Tc.Usd.HostedControls.Service;

namespace Tc.Usd.HostedControls
{
    public partial class SingleSignOnController
    {
        public void CallSsoService()
        {
            var login = GetSsoDetails();
            var privateKey = GetPrivateInfo();
            var expiredSeconds = GetConfig(DataKey.SsoTokenExpired);
            var notBeforeSeconds = GetConfig(DataKey.SsoTokenNotBefore);
            var payload = GetPayload(login, expiredSeconds, notBeforeSeconds);
            var token = WebServiceExchangeHelper.CreateJwtToken(privateKey, payload);
            var data = WebServiceExchangeHelper.GetCustomerTravelPlannerJson();
            var serviceUrl = GetConfig(DataKey.OwrUrlConfigName);
            var content = WebServiceExchangeHelper.SendHttpRequest(serviceUrl, token, data);
            var eventParams = WebServiceExchangeHelper.ContentToEventParams(content);

            FireEvent(EventName.SsoCompleteEvent, eventParams);
        }

        private Entity GetSsoDetails()
        {
            var query = new QueryByAttribute("tc_externallogin")
            {
                ColumnSet =
                    new ColumnSet("tc_abtanumber", "tc_branchcode", "tc_employeeid", "tc_externalloginid", "tc_initials",
                        "createdby")
            };
            query.AddAttributeValue("ownerid", _client.CrmInterface.GetMyCrmUserId());
            var login = ExecuteQuery(query);
            if (login == null) return null;
            var loginInfo =
                $"Abtanumber is {login.GetAttributeValue<string>("tc_abtanumber")}, BranchCode is {login.GetAttributeValue<string>("tc_branchcode")}, Employee Id is {login.GetAttributeValue<string>("tc_employeeid")}, Initials are {login.GetAttributeValue<string>("tc_initials")}";

            LogWriter.Log($"Requesting SSO Details result: {loginInfo}", TraceEventType.Verbose);
            return login;
        }


        private string GetPrivateInfo()
        {
            var query = new QueryByAttribute("tc_secureconfiguration")
            {
                ColumnSet = new ColumnSet("tc_value")
            };
            query.AddAttributeValue("tc_name", DataKey.JwtPrivateKeyConfigName);
            var config = ExecuteQuery(query);
            var privateKey = config?.GetAttributeValue<string>("tc_value");
            if (privateKey != null)
                LogWriter.Log(
                    $"Retrieved {DataKey.JwtPrivateKeyConfigName} result {DataKey.JwtPrivateKeyConfigName} is not null",
                    TraceEventType.Verbose);
            return privateKey;
        }

        private string GetConfig(string name)
        {
            var query = new QueryByAttribute("tc_configuration")
            {
                ColumnSet = new ColumnSet("tc_value")
            };
            query.AddAttributeValue("tc_name", name);
            var config = ExecuteQuery(query);
            var value = config?.GetAttributeValue<string>("tc_value");
            LogWriter.Log($"Retrieved {name} result: {value}",
                TraceEventType.Verbose);
            return value;
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
                    _client.CrmInterface.ExecuteCrmOrganizationRequest(req,
                        $"Requesting {query.EntityName}");
                var record = response.EntityCollection?.Entities?.FirstOrDefault();
                return record;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                LogWriter.Log($"{ApplicationName} application terminated with an error::{ex}",
                    TraceEventType.Error);
            }
            catch (TimeoutException ex)
            {
                LogWriter.Log($"{ApplicationName} application terminated with an error::{ex}",
                    TraceEventType.Error);
            }
            catch (Exception ex)
            {
                LogWriter.Log($"{ApplicationName} application terminated with an error::{ex}",
                    TraceEventType.Error);
            }

            return null;
        }

        private JsonWebTokenPayload GetPayload(Entity login, string expiredSeconds, string notBeforeSeconds)
        {
            var payload = new JsonWebTokenPayload
            {
                IssuedAtTime = WebServiceExchangeHelper.GetIssuedAtTime().ToString(),
                NotBefore = WebServiceExchangeHelper.GetNotBeforeTime(notBeforeSeconds).ToString(),
                Expiry = WebServiceExchangeHelper.GetExpiry(expiredSeconds).ToString(),
                Jti = WebServiceExchangeHelper.GetJti().ToString(),
                BranchCode = login.GetAttributeValue<string>("tc_branchcode"),
                AbtaNumber = login.GetAttributeValue<string>("tc_abtanumber"),
                EmployeeId = login.GetAttributeValue<string>("tc_employeeid"),
                Initials = login.GetAttributeValue<string>("tc_initials"),
                CreatedBy = login.GetAttributeValue<EntityReference>("createdby").Name,
                Aud = DataKey.AudOneWebRetail
            };
            return payload;
        }
    }
}