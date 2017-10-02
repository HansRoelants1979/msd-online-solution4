using System;
using System.Collections.Generic;
using Microsoft.Uii.Csr;
using Tc.Crm.Common;
using Tc.Crm.Common.IntegrationLayer.Jti.Models;
using EntityRecords = Tc.Crm.Common.Constants.EntityRecords;
using Tc.Usd.HostedControls.Service;
using Tc.Usd.HostedControls.Models;
using Tc.Crm.Common.IntegrationLayer.Model;
using System.Text;
using Tc.Crm.Common.Constants;
using Tc.Crm.Common.Constants.UsdConstants;

namespace Tc.Usd.HostedControls
{
    public enum RequestType
    {
        Other,
        Admin,
        Booking,
        TravelPlanner
    }
    public partial class SingleSignOnController
    {
        public void OpenWebRio(RequestActionEventArgs args, bool global)
        {
            try
            {
                if (CheckIfWebRioIsOpen())
                {
                    FireEventOnError("Multiple instances of Web Rio cannot be openned - close existing ones first.", global);
                    return;
                }
                var configuration = GetWebRioSsoConfiguration(args);
                if (configuration.Errors != null && configuration.Errors.Count > 0)
                {
                    HandleConfigurationErrors(configuration, global);
                    return;
                }

                var payload = GetWebRioSsoTokenPayload(configuration);
                if (payload == null)
                {
                    FireEventOnError("Payload of JWT is null", global);
                    return;
                }
                var token = _jtiService.CreateJwtToken(configuration.PrivateKey, payload);
                if (token == null)
                {
                    FireEventOnError("JWT token is null", global);
                    return;
                }

                var url = GetUrl(configuration);

                ResponseEntity response = SendRequest(url, token, configuration.JSessionId);
                var eventParameters = GetEventParameters(response);

               
                if (eventParameters == null)
                {
                    FireEventOnError("Returned content could not be parsed.", global);
                    return;
                }
                if (!eventParameters.ContainsKey("webRioUrl") 
                    || (eventParameters.ContainsKey("webRioUrl") && string.IsNullOrWhiteSpace(eventParameters["webRioUrl"])))
                {
                    FireEventOnError("Response doesn't contain Web Rio Url.", global);
                    return;
                }

                FireOnSuccess(eventParameters, global);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace.ToString());
                FireEventOnError("Either the service is down or an internal unexpected error has occurred. Please contact administrator.", global);
            }
        }

        private bool CheckIfWebRioIsOpen()
        {
            return CheckIfApplicationIsOpen(UsdHostedControl.WebRioAdmin
                                            , UsdHostedControl.WebRioConsultation);
        }

        private bool CheckIfApplicationIsOpen(params string[] appNames)
        {
            foreach (Session session in localSessionManager)
            {
                foreach (IHostedApplication app in session)
                {
                    for (int i = 0; i < appNames.Length; i++)
                    {
                        if (app.ApplicationName.Equals(appNames[i], StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private Dictionary<string, string> GetEventParameters(ResponseEntity response)
        {
            if (response == null) throw new ArgumentNullException("response");
            var content = response.Content;

            var eventParameters = WebServiceExchangeHelper.ContentToEventParamsForWebRio(content);
            if (eventParameters == null || eventParameters.Count == 0)
                return null;
           
            string jsessionId = string.Empty;
            if (response.Cookies != null && response.Cookies.TryGetValue(ResponseAttribute.WebRioResponseCookie_JSessionId, out jsessionId))
            {
                eventParameters.Add(ResponseAttribute.WebRioResponseCookie_JSessionId, jsessionId);
            }
            return eventParameters;
        }

        private void FireOnSuccess(Dictionary<string, string> eventParameters, bool global)
        {
            if (eventParameters == null) throw new ArgumentNullException("eventParameters");
            if (global)
                FireEvent(UsdEvent.GlobalSsoCompleteEvent, eventParameters);
        }

        private void HandleConfigurationErrors(WebRioSsoConfig configuration, bool global)
        {
            if (configuration == null) return;
            if (configuration.Errors == null || configuration.Errors.Count == 0) return;
            var message = new StringBuilder();
            foreach (var item in configuration.Errors)
            {
                message.AppendLine(item);
            }
            message.Append("Please contact administrator for further details.");
            FireEventOnError(message.ToString(), global);
        }

        private WebRioSsoConfig GetWebRioSsoConfiguration(RequestActionEventArgs args)
        {
            var configuration = new WebRioSsoConfig();
            configuration.JSessionId = GetParamValue(args, UsdParameter.WebRioJSessionId);
            configuration.RequestType = GetRequestType(args);
            CrmService.GetWebRioSsoConfiguration(_client.CrmInterface, configuration);
            configuration.Login = CrmService.GetSsoLoginDetails(_client.CrmInterface, _client.CrmInterface.GetMyCrmUserId());
            configuration.PrivateKey = CrmService.GetWebRioPrivateKey(_client.CrmInterface);
            ValidateConfiguration(configuration);
            return configuration;
        }

        private RequestType GetRequestType(RequestActionEventArgs args)
        {
            var requestType = GetParamValue(args, UsdParameter.WebRioRequestType);
            if (string.IsNullOrWhiteSpace(requestType))
                return RequestType.Other;
            else if (requestType.Equals(RequestType.Admin.ToString(),StringComparison.OrdinalIgnoreCase)
                                        || requestType.Equals(RequestType.Booking.ToString(), StringComparison.OrdinalIgnoreCase)
                                        || requestType.Equals(RequestType.TravelPlanner.ToString(),StringComparison.OrdinalIgnoreCase))
                return (RequestType)Enum.Parse(typeof(RequestType), requestType, true);
            else
                return RequestType.Other;
        }

        private void ValidateConfiguration(WebRioSsoConfig configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            var errors = new List<string>();

            if (configuration.RequestType == RequestType.Other)
                errors.Add("Action call parameter [Type] is missing or not valid.");

            if (configuration.Login == null
                    || string.IsNullOrWhiteSpace(configuration.Login.AbtaNumber)
                    || string.IsNullOrWhiteSpace(configuration.Login.BranchCode)
                    || string.IsNullOrWhiteSpace(configuration.Login.Initials)
                    || string.IsNullOrWhiteSpace(configuration.Login.EmployeeId))
                errors.Add("Login details are missing for the logged-in user.");

            int expirySeconds = -1;
            if (string.IsNullOrWhiteSpace(configuration.ExpirySeconds) || !Int32.TryParse(configuration.ExpirySeconds,out expirySeconds))
            {
                errors.Add("Expiry seconds have not been specified or the one specified does not have the correct format.");
            }

            int notBeforeTime = -1;
            if (string.IsNullOrWhiteSpace(configuration.NotBeforeTime) || !Int32.TryParse(configuration.NotBeforeTime, out notBeforeTime))
            {
                errors.Add("Not before time seconds have not been specified or the one specified does not have correct format.");
            }

            if (string.IsNullOrWhiteSpace(configuration.PrivateKey))
                errors.Add("Private key has not been provided");

            if (string.IsNullOrWhiteSpace(configuration.ServiceUrl))
            {
                errors.Add("SSO Url has not been configured properly.");
            }
            else
            {
                if (configuration.RequestType == RequestType.Admin)
                    if (string.IsNullOrWhiteSpace(configuration.AdminApi))
                        errors.Add("Admin Api has not been configured properly.");
            }

            configuration.Errors = errors;
        }

        private string GetUrl(WebRioSsoConfig configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (configuration.RequestType == RequestType.Admin)
                return $"{configuration.ServiceUrl}/{configuration.AdminApi}";
            return null;
        }

        private ResponseEntity SendRequest(string url, string token, string jSessionId)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException("url");
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException("token");

            if (string.IsNullOrWhiteSpace(jSessionId))
            {
                return _jtiService.SendHttpRequestWithCookie(HttpMethod.Post, url, token, string.Empty, null, null);
            }
            else
            {
                var cookie = new Dictionary<string, string>();
                cookie.Add("jsessionid", jSessionId);
                return _jtiService.SendHttpRequestWithCookie(HttpMethod.Post, url, token, string.Empty, null, cookie);
            }
        }

        private void FireEventOnError(string message, bool global)
        {
            _logger.LogError(message);
            var eventParams = new Dictionary<string, string> { { "responseCode", "501" }, { "responseMessage", message } };
            if (global)
                FireEvent(UsdEvent.GlobalSsoCompleteEvent, eventParams);
            else
                FireEvent(EntityRecords.Configuration.SsoCompleteEvent, eventParams);
        }

        private WebRioJsonWebTokenPayload GetWebRioSsoTokenPayload(WebRioSsoConfig configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            var payload = new WebRioJsonWebTokenPayload
            {
                IssuedAtTime = _jtiService.GetIssuedAtTime().ToString(),
                NotBefore = _jtiService.GetNotBeforeTime(configuration.NotBeforeTime).ToString(),
                Expiry = _jtiService.GetExpiry(configuration.ExpirySeconds).ToString(),
                Jti = WebServiceExchangeHelper.GetJti().ToString(),
                BranchCode = configuration.Login.BranchCode,
                AbtaNumber = configuration.Login.AbtaNumber,
                EmployeeId = configuration.Login.EmployeeId,
                Initials = configuration.Login.Initials,
                Aud = EntityRecords.Configuration.WebRioAudWebRio
            };
            return payload;
        }
    }
}