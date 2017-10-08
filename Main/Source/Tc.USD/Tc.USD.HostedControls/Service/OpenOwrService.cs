using System;
using System.Collections.Generic;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Csr;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Common;
using Tc.Crm.Common.Constants.Attributes;
using Tc.Crm.Common.Constants.UsdConstants;
using Tc.Crm.Common.IntegrationLayer.Jti.Models;
using Tc.Usd.HostedControls.Service;
using Configuration = Tc.Crm.Common.Constants.EntityRecords.Configuration;

namespace Tc.Usd.HostedControls
{
    public partial class SingleSignOnController
    {
        public void CallSsoService(RequestActionEventArgs args)
        {
            var opportunityId = GetParamValue(args, Configuration.OwrOpportunityIdParamName);
            var opportunity = CrmService.GetOpportunity(_client.CrmInterface, _logger, opportunityId);
            if (opportunity == null)
            {
                FireEventOnOwrError("There is no opportunity in context.");
                return;
            }

            var rooms = CrmService.GetTravelPlannerRooms(opportunityId, _logger, _client.CrmInterface);
            var createdByInitials = opportunity.GetAttributeValue<string>(Opportunity.Initials);
            var login = CrmService.GetSsoDetails(_client.CrmInterface.GetMyCrmUserId(), _logger, _client.CrmInterface);
            if (login == null)
            {
                FireEventOnOwrError("Login details are missing for the logged-in user.");
                return;
            }

            var privateKey = CrmService.GetPrivateInfo(_logger, _client.CrmInterface);
            if (privateKey == null)
            {
                FireEventOnOwrError("Private Key is missing in the system");
                return;
            }
            var expiredSeconds = CrmService.GetConfig(Configuration.OwrSsoTokenExpired, _logger, _client.CrmInterface);
            var notBeforeSeconds = CrmService.GetConfig(Configuration.OwrSsoTokenNotBefore, _logger, _client.CrmInterface);
            if (expiredSeconds == null || notBeforeSeconds==null)
            {
                FireEventOnOwrError("Missing payload configuration");
                return;
            }
            var payload = GetPayload(login, expiredSeconds, notBeforeSeconds, createdByInitials);
            var token = _jtiService.CreateJwtToken(privateKey, payload);
            if (token == null)
            {
                FireEventOnOwrError("JWT token is null");
                return;
            }
            var owrJsonHelper = new OwrJsonHelper(_client.CrmInterface, opportunity);
            var data = owrJsonHelper.GetCustomerTravelPlannerJson(rooms);
            var serviceUrl = CrmService.GetConfig(Configuration.OwrUrlConfigName, _logger, _client.CrmInterface);
            if (serviceUrl == null)
            {
                FireEventOnOwrError("Service Url is null");
                return;
            }
            var content = _jtiService.SendHttpRequest(HttpMethod.Post, serviceUrl, token, data).Content;
            if (content == null)
            {
                FireEventOnOwrError("Owr response content is null");
                return;
            }
            var eventParams = WebServiceExchangeHelper.ContentToEventParams(content, _logger);
            if (eventParams == null)
            {
                FireEventOnOwrError("Failed to parse OWR response");
                return;
            }
            FireEventOnOwrSuccess(eventParams);
        }
        private OwrJsonWebTokenPayload GetPayload(Entity login, string expiredSeconds, string notBeforeSeconds,
            string createdByInitials)
        {
            var payload = new OwrJsonWebTokenPayload
            {
                IssuedAtTime = _jtiService.GetIssuedAtTime().ToString(),
                NotBefore = _jtiService.GetNotBeforeTime(notBeforeSeconds).ToString(),
                Expiry = _jtiService.GetExpiry(expiredSeconds).ToString(),
                Jti = WebServiceExchangeHelper.GetJti().ToString(),
                BranchCode = login.GetAttributeValue<string>(ExternalLogin.BranchCode),
                AbtaNumber = login.GetAttributeValue<string>(ExternalLogin.AbtaNumber),
                EmployeeId = login.GetAttributeValue<string>(ExternalLogin.EmployeeId),
                Initials = login.GetAttributeValue<string>(ExternalLogin.Initials),
                CreatedBy = createdByInitials,
                Aud = Configuration.OwrAudOneWebRetail
            };
            return payload;
        }

        private string GetParamValue(RequestActionEventArgs args, string paramName)
        {
            var actionDataList = Utility.SplitLines(args.Data, CurrentContext,
                localSession);
            var paramValue = Utility.GetAndRemoveParameter(actionDataList, paramName);
            _logger.LogInformation($"Parameter {paramName} is {paramValue}");
            return paramValue;
        }

        private void FireEventOnOwrError(string message)
        {
            _logger.LogError(message);
            var eventParameters = new Dictionary<string, string> { { UsdParameter.ResponseCode, HttpCode.InternalError }, { UsdParameter.ResponseMessage, message } };
            eventParameters.Add(UsdParameter.ApplicationType, UsdParameter.ApplicationOwr);
            FireEvent(Crm.Common.Constants.EntityRecords.Configuration.SsoCompleteEvent, eventParameters);
        }

        private void FireEventOnOwrSuccess(Dictionary<string, string> eventParameters)
        {
            if (eventParameters == null) throw new ArgumentNullException("eventParameters");
            eventParameters.Add(UsdParameter.ApplicationType, UsdParameter.ApplicationOwr);
            FireEvent(UsdEvent.SsoCompleteEvent, eventParameters);
        }



    }
}