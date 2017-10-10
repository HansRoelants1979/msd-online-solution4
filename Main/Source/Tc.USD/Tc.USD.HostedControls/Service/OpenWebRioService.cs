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
using System.Net;
using System.Runtime.Serialization.Json;
using System.IO;
using Microsoft.Xrm.Tooling.Connector;

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
                
                var configuration = GetWebRioSsoConfiguration(args);
                
                if (configuration.Errors != null && configuration.Errors.Count > 0)
                {
                    HandleConfigurationErrors(configuration, global);
                    return;
                }
                
                SetRequestData(configuration);
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

                ResponseEntity response;
                
                response = SendRequest(url, token, configuration.JSessionId,configuration.Data);
                var content = GetResponseContent(response);
                var ssoResponse = GetWebRioSsoResponse(content);
                if (configuration.RequestType== RequestType.TravelPlanner
                    && ssoResponse.ResponseCode.Equals(HttpCode.NotFound,StringComparison.OrdinalIgnoreCase))
                {
                    ssoResponse = OpenNewConsultation(_client.CrmInterface, configuration,token);
                }

                if (ssoResponse == null)
                {
                    FireEventOnError("SSO Response is null or could not be parsed.", global);
                    return;
                }
                if(ssoResponse.ResponseCode != HttpCode.Ok)
                {
                    if(string.IsNullOrWhiteSpace(ssoResponse.ResponseMessage))
                    {
                        FireEventOnError("The SSO response did not return a success. No details have been provided in the response.", global);
                        return;
                    }
                    else
                    {
                        FireEventOnError($"The SSO response did not return a success. {ssoResponse.ResponseMessage}", global);
                        return;
                    }
                }

                if (ssoResponse.ResponseCode == HttpCode.Ok && string.IsNullOrWhiteSpace(ssoResponse.WebRioUrl))
                {
                    FireEventOnError("Response doesn't contain Web Rio Url.", global);
                    return;
                }

                var eventParameters = GetEventParameters(ssoResponse,response);

               
                if (eventParameters == null)
                {
                    FireEventOnError("Returned content could not be parsed.", global);
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

        private WebRioResponse OpenNewConsultation(CrmServiceClient crmInterface, WebRioSsoConfig configuration,string token)
        {
            Customer cust = CrmService.GetCustomerDataForWebRioNewConsultation(_client.CrmInterface, configuration.CustomerId);
            configuration.Data = SerializeNewConsultationSsoRequestToJson(cust);
            
            var url = $"{configuration.ServiceUrl}/{configuration.NewConsultationApi}";
            var response = SendRequest(url, token, configuration.JSessionId, configuration.Data);
            var content = GetResponseContent(response);
            return GetWebRioSsoResponse(content);
        }

        private static string SerializeNewConsultationSsoRequestToJson(Customer customer)
        {
            var memoryStream = new MemoryStream();

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Customer));
            serializer.WriteObject(memoryStream, customer);
            byte[] json = memoryStream.ToArray();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        private void SetRequestData(WebRioSsoConfig configuration)
        {
            if (configuration.RequestType == RequestType.Admin)
                configuration.Data = string.Empty;
            else if(configuration.RequestType == RequestType.Booking || configuration.RequestType == RequestType.TravelPlanner)
            {
                configuration.Data = WebServiceExchangeHelper.SerializeOpenConsultationSsoRequestToJson(new WebRioSsoRequest { Consultation = configuration.ConsultationReference });
            }
        }

        
        private string GetResponseContent(ResponseEntity response)
        {
            if (response == null) return null;
            return response.Content;
        }
        private WebRioResponse GetWebRioSsoResponse(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return null;
            try
            {
                return WebServiceExchangeHelper.DeserializeWebRioSsoResponseJson(content);
            }
            catch (Exception)
            {

                return null;
            }
        }
        private Dictionary<string, string> GetEventParameters(WebRioResponse webRioResponse, ResponseEntity response)
        {
            if (response == null) throw new ArgumentNullException("response");
            var eventParameters = WebServiceExchangeHelper.ContentToEventParamsForWebRio(webRioResponse);
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
            eventParameters.Add(UsdParameter.ApplicationType,UsdParameter.Application_WebRio);
            if (global)
                FireEvent(UsdEvent.GlobalSsoCompleteEvent, eventParameters);
            else
                FireEvent(UsdEvent.SsoCompleteEvent, eventParameters);
            
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
            
            if (configuration.RequestType == RequestType.Booking)
                GetBookingDetails(args,configuration);
            else if (configuration.RequestType == RequestType.TravelPlanner)
                GetTravelPlannerDetails(args, configuration);
            CrmService.GetWebRioSsoConfiguration(_client.CrmInterface, configuration);
            
            configuration.Login = CrmService.GetSsoLoginDetails(_client.CrmInterface, _client.CrmInterface.GetMyCrmUserId());
            configuration.PrivateKey = CrmService.GetWebRioPrivateKey(_client.CrmInterface);
            ValidateConfiguration(configuration);
            return configuration;
        }

        public void GetBookingDetails(RequestActionEventArgs args, WebRioSsoConfig configuration)
        {
            if (configuration.RequestType != RequestType.Booking) return;
            var url = GetParamValue(args, UsdParameter.Url);
            var decodedUrl = WebUtility.UrlDecode(url);

            decodedUrl = decodedUrl.Replace("%25", "");
            decodedUrl = decodedUrl.Replace("%3d", "=");

            var etc = decodedUrl.Substring(decodedUrl.IndexOf("etc="));
            configuration.ObjectTypeCode = etc.Substring(4, etc.IndexOf('&') - 4);


            var id = decodedUrl.Substring(decodedUrl.IndexOf("id="));
            id = id.Substring(3, id.IndexOf("7d") - 3);
            configuration.BookingSummaryId = id.Replace("7b", "");

            CrmService.GetConsultationReferenceFromBookingSummary(_client.CrmInterface,configuration);
        }

        public void GetTravelPlannerDetails(RequestActionEventArgs args, WebRioSsoConfig configuration)
        {
            if (configuration.RequestType != RequestType.TravelPlanner) return;
            configuration.ConsultationReference= GetParamValue(args, UsdParameter.WebRioConsultationNo);
            configuration.CustomerId= GetParamValue(args, UsdParameter.CustomerId);
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
            if ((configuration.RequestType == RequestType.Booking) || (configuration.RequestType == RequestType.TravelPlanner))
                return $"{configuration.ServiceUrl}/{configuration.OpenConsultationApi}";

            return null;
        }

        private ResponseEntity SendRequest(string url, string token, string jSessionId,string data)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException("url");
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException("token");

            if (string.IsNullOrWhiteSpace(jSessionId))
            {
                return _jtiService.SendHttpRequestWithCookie(HttpMethod.Post, url, token, data, null, null);
            }
            else
            {
                var cookie = new Dictionary<string, string>();
                cookie.Add("jsessionid", jSessionId);
                return _jtiService.SendHttpRequestWithCookie(HttpMethod.Post, url, token, data, null, cookie);
            }
        }

        private void FireEventOnError(string message, bool global)
        {
            _logger.LogError(message);
            var eventParameters = new Dictionary<string, string> { { UsdParameter.ResponseCode, HttpCode.InternalError }, { UsdParameter.ResponseMessage, message } };
            eventParameters.Add(UsdParameter.ApplicationType, UsdParameter.Application_WebRio);
            if (global)
                FireEvent(UsdEvent.GlobalSsoCompleteEvent, eventParameters);
            else
                FireEvent(EntityRecords.Configuration.SsoCompleteEvent, eventParameters);
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