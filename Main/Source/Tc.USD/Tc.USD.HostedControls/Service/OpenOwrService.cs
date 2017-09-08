using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using Microsoft.Uii.Csr;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Tc.Crm.Common.Jti.Models;
using Tc.Crm.Common.Jti.Service;
using Tc.Usd.HostedControls.Constants;
using Tc.Usd.HostedControls.Models;
using Tc.Usd.HostedControls.Service;

namespace Tc.Usd.HostedControls
{
    public partial class SingleSignOnController
    {
        public void CallSsoService(RequestActionEventArgs args)
        {
            var login = GetSsoDetails();
            var privateKey = GetPrivateInfo();
            var expiredSeconds = GetConfig(DataKey.SsoTokenExpired);
            var notBeforeSeconds = GetConfig(DataKey.SsoTokenNotBefore);
            var payload = GetPayload(login, expiredSeconds, notBeforeSeconds);
            var token = JtiService.CreateJwtToken(privateKey, payload);
            var data = WebServiceExchangeHelper.GetCustomerTravelPlannerJson();
            var serviceUrl = GetConfig(DataKey.OwrUrlConfigName);
            var content = JtiService.SendHttpRequest(serviceUrl, token, data);
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

            LogWriter.LogInformation($"Requesting SSO Details result: {loginInfo}");
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
                LogWriter.LogInformation(
                    $"Retrieved {DataKey.JwtPrivateKeyConfigName} result {DataKey.JwtPrivateKeyConfigName} is not null");
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
            LogWriter.LogInformation($"Retrieved {name} result: {value}");
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
                LogWriter.LogError($"{ApplicationName} application terminated with an error::{ex}");
            }
            catch (TimeoutException ex)
            {
                LogWriter.LogError($"{ApplicationName} application terminated with an error::{ex}");
            }
            catch (Exception ex)
            {
                LogWriter.LogError($"{ApplicationName} application terminated with an error::{ex}");
            }

            return null;
        }

        private OwrJsonWebTokenPayload GetPayload(Entity login, string expiredSeconds, string notBeforeSeconds)
        {
            var payload = new OwrJsonWebTokenPayload
            {
                IssuedAtTime = JtiService.GetIssuedAtTime().ToString(),
                NotBefore = JtiService.GetNotBeforeTime(notBeforeSeconds).ToString(),
                Expiry = JtiService.GetExpiry(expiredSeconds).ToString(),
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