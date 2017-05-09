using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.Plugins.CacheRequest.Model;

namespace Tc.Crm.Plugins.CacheRequest.BusinessLogic
{
    public class CachingService
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        private bool IsContextValidForExecute(IPluginExecutionContext context, ITracingService trace)
        {
            if (context == null) throw new InvalidPluginExecutionException("context is null.");
            if (trace == null) throw new InvalidPluginExecutionException("trace is null.");

            if (!context.MessageName.Equals(Messages.Create, StringComparison.OrdinalIgnoreCase)) return false;
            if (!context.PrimaryEntityName.Equals(Entities.CacheRequest, StringComparison.OrdinalIgnoreCase)) return false;
            if (!context.InputParameters.Contains(InputParameters.Target)) return false;
            if (!(context.InputParameters[InputParameters.Target] is Entity)) return false;

            var target = context.InputParameters[InputParameters.Target] as Entity;
            if (!target.Contains(Attributes.CacheRequest.Name) || target[Attributes.CacheRequest.Name] == null) return false;
            var name = target[Attributes.CacheRequest.Name].ToString();

            var allowedListOfNames = new Collection<string>();
            allowedListOfNames.Add(CacheBucket.Brand);
            allowedListOfNames.Add(CacheBucket.Country);
            allowedListOfNames.Add(CacheBucket.Currency);
            allowedListOfNames.Add(CacheBucket.Gateway);
            allowedListOfNames.Add(CacheBucket.SourceMarket);
            allowedListOfNames.Add(CacheBucket.TourOperator);
            allowedListOfNames.Add(CacheBucket.Hotel);

            if (!allowedListOfNames.Contains(name)) return false;

            trace.Trace("Context is valid for execute");
            return true;
        }

        public void SendCacheRequest(IPluginExecutionContext context, ITracingService trace, IOrganizationService service)
        {
            if (service == null) throw new InvalidPluginExecutionException(ValidationMessages.OrganizationServiceIsNull);
            if (context == null) throw new InvalidPluginExecutionException(ValidationMessages.ContextIsNull);
            if (trace == null) throw new InvalidPluginExecutionException(ValidationMessages.TraceIsNull);

            trace.Trace("Start - SendCacheRequest");
            if (!IsContextValidForExecute(context, trace)) throw new InvalidPluginExecutionException("Either name provided in cache request is invalid or this plugin has fired out of context.");
            var cachingServiceParameters = GetCachingServiceParameters(trace, service);
            var token = CreateJWTToken(cachingServiceParameters, trace, service);
            var requestData = GetRequestDataFrom(token, context, trace);
            SendRequest(requestData, cachingServiceParameters, trace, context, service);
            trace.Trace("End - SendCacheRequest");
        }

        private void SendRequest(string requestData
                                , Dictionary<string, string> cachingServiceParameters
                                , ITracingService trace
                                , IPluginExecutionContext context
                                , IOrganizationService service)
        {
            if (service == null) throw new InvalidPluginExecutionException(ValidationMessages.OrganizationServiceIsNull);
            if (context == null) throw new InvalidPluginExecutionException(ValidationMessages.ContextIsNull);
            if (trace == null) throw new InvalidPluginExecutionException(ValidationMessages.TraceIsNull);
            if (cachingServiceParameters == null) throw new InvalidPluginExecutionException(ValidationMessages.CachingParameterIsNull);

            if (string.IsNullOrWhiteSpace(requestData)) throw new InvalidPluginExecutionException(ValidationMessages.RequestDataIsEmpty);

            if (!cachingServiceParameters.ContainsKey(CachingParameter.ServiceUrl) || cachingServiceParameters[CachingParameter.ServiceUrl] == null)
                throw new InvalidPluginExecutionException(ValidationMessages.CachingServiceUrlIsNullOrEmpty);

            if (!cachingServiceParameters.ContainsKey(CachingParameter.Api) || cachingServiceParameters[CachingParameter.Api] == null)
                throw new InvalidPluginExecutionException(ValidationMessages.CachingApiIsNullOrEmpty);

            trace.Trace("Start - SendRequest");
            try
            {
                var url = cachingServiceParameters[CachingParameter.ServiceUrl];
                var api = cachingServiceParameters[CachingParameter.Api];
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                Task<HttpResponseMessage> t = client.PostAsync(api, new StringContent(requestData, Encoding.UTF8, "application/json"));

                var response = t.Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    trace.Trace("Success");
                    UpdateStatus(true, context, service);
                    return;
                }
                else
                {
                    trace.Trace(response.StatusCode.ToString());
                    HandleError(response, context, service);
                    UpdateStatus(false, context, service);
                    throw new InvalidPluginExecutionException($"Fallied to refresh cache. Status code is {response.StatusCode.ToString()}");
                }
            }
            catch (Exception ex)
            {
                trace.Trace("Failure: unexpected error");
                HandleError(ex, context, service);
                UpdateStatus(false, context, service);
                throw;
            }
        }

        public void UpdateStatus(bool success, IPluginExecutionContext context, IOrganizationService service)
        {
            if (service == null) throw new InvalidPluginExecutionException(ValidationMessages.OrganizationServiceIsNull);
            if (context == null) throw new InvalidPluginExecutionException(ValidationMessages.ContextIsNull);

            var cacheRequest = new Entity(Entities.CacheRequest);
            cacheRequest.Id = context.PrimaryEntityId;
            cacheRequest[Attributes.CacheRequest.StateCode] = new OptionSetValue(OptionSetValues.CacheRequest.StatecodeInactive);
            if (success)
                cacheRequest[Attributes.CacheRequest.StatusCode] = new OptionSetValue(OptionSetValues.CacheRequest.StatuscodeSucceeded);
            else
                cacheRequest[Attributes.CacheRequest.StatusCode] = new OptionSetValue(OptionSetValues.CacheRequest.StatuscodeFailed);
            service.Update(cacheRequest);
        }

        private void HandleError(HttpResponseMessage response, IPluginExecutionContext context, IOrganizationService service)
        {
            if (context == null) throw new InvalidPluginExecutionException("context is null.");
            if (service == null) throw new InvalidPluginExecutionException("organization service is null.");

            var content = string.Empty;
            if (response == null || response.Content == null) content = "No Content";
            else
            {
                Task<string> task = response.Content.ReadAsStringAsync();
                content = task.Result;
            }
            var errorNote = new Entity(Entities.Annotation);
            errorNote[Attributes.Annotation.Subject] = "Cache Job Failed";
            errorNote[Attributes.Annotation.NoteText] = $"Status Code:{response.StatusCode.ToString()}:: Content:{content}";
            errorNote[Attributes.Annotation.ObjectId] = new EntityReference(Entities.CacheRequest, context.PrimaryEntityId);
            service.Create(errorNote);
        }

        public void HandleError(Exception exception, IPluginExecutionContext context, IOrganizationService service)
        {
            if (service == null) throw new InvalidPluginExecutionException(ValidationMessages.OrganizationServiceIsNull);
            if (context == null) throw new InvalidPluginExecutionException(ValidationMessages.ContextIsNull);

            var errorNote = new Entity(Entities.Annotation);
            errorNote[Attributes.Annotation.Subject] = ValidationMessages.CachingErrorNoteSubject;
            if (exception == null)
                errorNote[Attributes.Annotation.NoteText] = ValidationMessages.UnexpectedError;
            else
                errorNote[Attributes.Annotation.NoteText] = $"{ exception.Message} :: Stack Trace :{exception.StackTrace.ToString()}";
            errorNote[Attributes.Annotation.ObjectId] = new EntityReference(Entities.CacheRequest, context.PrimaryEntityId);
            service.Create(errorNote);
        }

        private string GetRequestDataFrom(string token, IPluginExecutionContext context, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException(ValidationMessages.TraceIsNull);
            if (context == null) throw new InvalidPluginExecutionException(ValidationMessages.ContextIsNull);
            if (string.IsNullOrWhiteSpace(token)) throw new InvalidPluginExecutionException(ValidationMessages.TokenIsNull);

            trace.Trace("Start - GetRequestDataFrom");
            var target = context.InputParameters[InputParameters.Target] as Entity;
            var name = target[Attributes.CacheRequest.Name].ToString();
            trace.Trace("End - GetRequestDataFrom");

            var payload = new Payload
            {
                Bucket = name.ToUpper(),
                JWTToken = token
            };

            return JsonHelper<Payload>.SerializeJson(payload);
        }

        private string CreateJWTToken(Dictionary<string, string> cachingServiceParameters, ITracingService trace, IOrganizationService service)
        {
            if (trace == null) throw new InvalidPluginExecutionException(ValidationMessages.TraceIsNull);
            if (cachingServiceParameters == null) throw new InvalidPluginExecutionException(ValidationMessages.CachingParameterIsNull);
            if (service == null) throw new InvalidPluginExecutionException(ValidationMessages.OrganizationServiceIsNull);

            trace.Trace("Start - CreateJWTToken");

            var secretKey = cachingServiceParameters[CachingParameter.SecretKey];
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new InvalidPluginExecutionException(ValidationMessages.CachingSecretKeyIsNullOrEmpty);

            var payload = new Dictionary<string, object>()
            {
                {JwtPayloadParameters.IssuedAtTime, GetIssuedAtTime(cachingServiceParameters,trace).ToString()},
                {JwtPayloadParameters.NotBeforeTime, GetNotBeforeTime(cachingServiceParameters,trace).ToString()},
                {JwtPayloadParameters.Expiry, GetExpiry(cachingServiceParameters,trace).ToString()}
            };

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            return encoder.Encode(payload, secretKey);
        }

        private double GetExpiry(Dictionary<string, string> cachingServiceParameters, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException(ValidationMessages.TraceIsNull);
            if (cachingServiceParameters == null) throw new InvalidPluginExecutionException(ValidationMessages.CachingParameterIsNull);

            if (!cachingServiceParameters.ContainsKey(CachingParameter.ExpirySecondsFromNow))
                throw new InvalidPluginExecutionException(ValidationMessages.ExpirySecondsFromNowNotSpecified);

            try
            {
                var sec = Int32.Parse(cachingServiceParameters[CachingParameter.ExpirySecondsFromNow]);
                return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds + sec);
            }
            catch (FormatException ex)
            {
                trace.Trace($"{ex.Message}::{ex.StackTrace.ToString()}");
                throw new InvalidPluginExecutionException(ValidationMessages.ExpirySecondsFromNowIncorrectFormat);
            }
        }

        private double GetIssuedAtTime(Dictionary<string, string> cachingServiceParameters, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException(ValidationMessages.TraceIsNull);
            if (cachingServiceParameters == null) throw new InvalidPluginExecutionException(ValidationMessages.CachingParameterIsNull);

            if (!cachingServiceParameters.ContainsKey(CachingParameter.IssuedAtTimeFromNow))
                throw new InvalidPluginExecutionException(ValidationMessages.IssuedAtTimeFromNowNotSpecified);

            try
            {
                var sec = Int32.Parse(cachingServiceParameters[CachingParameter.IssuedAtTimeFromNow]);
                return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds + sec);
            }
            catch (FormatException ex)
            {
                trace.Trace($"{ex.Message}::{ex.StackTrace.ToString()}");
                throw new InvalidPluginExecutionException(ValidationMessages.IssuedAtTimeFromNowIncorrectFormat);
            }
        }

        private double GetNotBeforeTime(Dictionary<string, string> cachingServiceParameters, ITracingService trace)
        {
            if (cachingServiceParameters == null) throw new InvalidPluginExecutionException(ValidationMessages.CachingParameterIsNull);
            if (trace == null) throw new InvalidPluginExecutionException(ValidationMessages.TraceIsNull);

            if (!cachingServiceParameters.ContainsKey(CachingParameter.NotBeforeTimeFromNow))
                throw new InvalidPluginExecutionException(ValidationMessages.IssuedAtTimeFromNowNotSpecified);

            try
            {
                var sec = Int32.Parse(cachingServiceParameters[CachingParameter.NotBeforeTimeFromNow]);
                return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds + sec);
            }
            catch (FormatException ex)
            {
                trace.Trace($"{ex.Message}::{ex.StackTrace.ToString()}");
                throw new InvalidPluginExecutionException(ValidationMessages.NotBeforeTimeFromNowIncorrectFormart);
            }
        }

        private Dictionary<string, string> GetCachingServiceParameters(ITracingService trace, IOrganizationService service)
        {
            if (service == null) throw new InvalidPluginExecutionException(ValidationMessages.OrganizationServiceIsNull);
            if (trace == null) throw new InvalidPluginExecutionException(ValidationMessages.TraceIsNull);

            trace.Trace("Start - GetCachingServiceParameters");
            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='tc_configuration'>
                                <attribute name='tc_configurationid' />
                                <attribute name='tc_name' />
                                <attribute name='tc_value' />
                                <attribute name='tc_longvalue' />
                                <filter type='and'>
                                  <condition attribute='tc_group' operator='eq' value='Caching' />
                                </filter>
                              </entity>
                            </fetch>";
            var query = new FetchExpression(fetchXml);
            var response = service.RetrieveMultiple(query);

            if (response == null) throw new InvalidPluginExecutionException(ValidationMessages.ConfigurationValuesForCachingInCrmMissing);
            if (response.Entities == null || response.Entities.Count == 0) throw new InvalidPluginExecutionException(ValidationMessages.ConfigurationValuesForCachingInCrmMissing);

            var parameters = new Dictionary<string, string>();
            foreach (var e in response.Entities)
            {
                var key = e.Contains(Attributes.ConfigurationKeys.Name) ? e[Attributes.ConfigurationKeys.Name].ToString() : null;
                var value = e.Contains(Attributes.ConfigurationKeys.Value) ? e[Attributes.ConfigurationKeys.Value].ToString() : null;
                var longValue = e.Contains(Attributes.ConfigurationKeys.LongValue) ? e[Attributes.ConfigurationKeys.LongValue].ToString() : null;
                trace.Trace($"{key}:{value}:{longValue}");

                if (string.IsNullOrWhiteSpace(key) || (string.IsNullOrWhiteSpace(value) && string.IsNullOrWhiteSpace(longValue)))
                    throw new InvalidPluginExecutionException(ValidationMessages.ConfigurationHasNoValueForKey);

                if (!string.IsNullOrWhiteSpace(value))
                    parameters.Add(key, value);

                if (!string.IsNullOrWhiteSpace(longValue))
                    parameters.Add(key, longValue);
            }

            if (!parameters.ContainsKey(CachingParameter.ServiceUrl) ||
                !parameters.ContainsKey(CachingParameter.Api) ||
                !parameters.ContainsKey(CachingParameter.SecretKey) ||
                !parameters.ContainsKey(CachingParameter.IssuedAtTimeFromNow) ||
                !parameters.ContainsKey(CachingParameter.ExpirySecondsFromNow) ||
                !parameters.ContainsKey(CachingParameter.NotBeforeTimeFromNow))
                throw new InvalidPluginExecutionException(ValidationMessages.CachingKeysMissingInCrm);

            trace.Trace("End - GetCachingServiceParameters");
            return parameters;
        }
    }
}
