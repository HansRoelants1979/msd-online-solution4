using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Csr;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Tc.Crm.Common;
using Tc.Crm.Common.IntegrationLayer.Jti.Models;
using Tc.Usd.HostedControls.Constants;
using Tc.Usd.HostedControls.Service;

namespace Tc.Usd.HostedControls
{
    public partial class SingleSignOnController
    {
        public void CallSsoService(RequestActionEventArgs args)
        {
            var opportunityId = GetParamValue(args, DataKey.OpportunityIdParamName);
            var createdByInitials = GetCreatorsInitials(opportunityId);
            var login = GetSsoDetails(_client.CrmInterface.GetMyCrmUserId());
            var privateKey = GetPrivateInfo();
            var expiredSeconds = GetConfig(DataKey.SsoTokenExpired);
            var notBeforeSeconds = GetConfig(DataKey.SsoTokenNotBefore);
            var payload = GetPayload(login, expiredSeconds, notBeforeSeconds, createdByInitials);
            var token = _jtiService.CreateJwtToken(privateKey, payload);
            var data = WebServiceExchangeHelper.GetCustomerTravelPlannerJson();
            var serviceUrl = GetConfig(DataKey.OwrUrlConfigName);
            var content = _jtiService.SendHttpRequest(HttpMethod.Post, serviceUrl, token, data).Content;
            var eventParams = WebServiceExchangeHelper.ContentToEventParams(content);

            FireEvent(EventName.SsoCompleteEvent, eventParams);
        }

        private string GetCreatorsInitials(string opportunityId)
        {
            if (string.IsNullOrEmpty(opportunityId))
            {
                return null;
            }
            var query = new QueryByAttribute("opportunity")
            {
                ColumnSet = new ColumnSet("createdby")
            };
            query.AddAttributeValue("opportunityid", opportunityId);
            var record = ExecuteQuery(query);
            if (record == null) return null;
            var createdById = record.GetAttributeValue<EntityReference>("createdby").Id;
            var login = GetSsoDetails(createdById);
            var initials = login?.GetAttributeValue<string>("tc_initials");
            _logger.LogInformation($"crt (created by initials) are {initials}");
            return initials;
        }
        private Entity GetSsoDetails(Guid userId)
        {
            var query = new QueryByAttribute("tc_externallogin")
            {
                ColumnSet =
                     new ColumnSet("tc_abtanumber", "tc_branchcode", "tc_employeeid", "tc_externalloginid", "tc_initials",
                         "createdby")
            };
            query.AddAttributeValue("ownerid", userId);
            var login = ExecuteQuery(query);
            if (login == null) return null;
            var loginInfo =
                $"Abtanumber is {login.GetAttributeValue<string>("tc_abtanumber")}, BranchCode is {login.GetAttributeValue<string>("tc_branchcode")}, Employee Id is {login.GetAttributeValue<string>("tc_employeeid")}, Initials are {login.GetAttributeValue<string>("tc_initials")}";

            _logger.LogInformation($"Requesting SSO Details result: {loginInfo}");
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
                _logger.LogInformation(
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
            _logger.LogInformation($"Retrieved {name} result: {value}");
            return value;
        }

        

        private OwrJsonWebTokenPayload GetPayload(Entity login, string expiredSeconds, string notBeforeSeconds, string createdByInitials)
        {
            var payload = new OwrJsonWebTokenPayload
            {
                IssuedAtTime = _jtiService.GetIssuedAtTime().ToString(),
                NotBefore = _jtiService.GetNotBeforeTime(notBeforeSeconds).ToString(),
                Expiry = _jtiService.GetExpiry(expiredSeconds).ToString(),
                Jti = WebServiceExchangeHelper.GetJti().ToString(),
                BranchCode = login.GetAttributeValue<string>("tc_branchcode"),
                AbtaNumber = login.GetAttributeValue<string>("tc_abtanumber"),
                EmployeeId = login.GetAttributeValue<string>("tc_employeeid"),
                Initials = login.GetAttributeValue<string>("tc_initials"),
                CreatedBy = createdByInitials,
                Aud = DataKey.AudOneWebRetail
            };
            return payload;
        }
        private string GetParamValue(RequestActionEventArgs args, string paramName)
        {
            List<KeyValuePair<string, string>> actionDataList = Utility.SplitLines(args.Data, CurrentContext,
                localSession);
            var paramValue = Utility.GetAndRemoveParameter(actionDataList, paramName);
            _logger.LogInformation($"Parameter {paramName} is {paramValue}");
            return paramValue;
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
                _logger.LogError($"{ApplicationName} application terminated with an error::{ex}");
            }
            catch (TimeoutException ex)
            {
                _logger.LogError($"{ApplicationName} application terminated with an error::{ex}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ApplicationName} application terminated with an error::{ex}");
            }

            return null;
        }
    }
}