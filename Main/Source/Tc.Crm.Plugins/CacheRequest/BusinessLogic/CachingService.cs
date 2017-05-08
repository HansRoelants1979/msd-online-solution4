using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
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
            if (context == null) throw new InvalidPluginExecutionException("context is null.");
            if (trace == null) throw new InvalidPluginExecutionException("trace is null.");
            if (service == null) throw new InvalidPluginExecutionException("organization service is null.");

            trace.Trace("Start - SendCacheRequest");
            if (!IsContextValidForExecute(context, trace)) return;
            var cachingServiceParameters = GetCachingServiceParameters(trace, service);
            var token = CreateJWTToken(cachingServiceParameters, context, trace, service);
            var requestData = GetRequestDataFrom(token, context, trace);
            SendRequest(requestData, token, cachingServiceParameters, trace, context, service);
            trace.Trace("End - SendCacheRequest");
        }

        private void SendRequest(string requestData
                                , string token
                                , Dictionary<string, string> cachingServiceParameters
                                , ITracingService trace
                                , IPluginExecutionContext context
                                , IOrganizationService service)
        {
            if (cachingServiceParameters == null) throw new InvalidPluginExecutionException("caching parameters is null.");
            if (context == null) throw new InvalidPluginExecutionException("context is null.");
            if (trace == null) throw new InvalidPluginExecutionException("trace is null.");
            if (service == null) throw new InvalidPluginExecutionException("organization service is null.");
            if (string.IsNullOrWhiteSpace(token)) throw new InvalidPluginExecutionException("token is null or empty");
            if (string.IsNullOrWhiteSpace(requestData)) throw new InvalidPluginExecutionException("request data is null or empty");
            if (!cachingServiceParameters.ContainsKey(CachingParameter.ServiceUrl))
                throw new InvalidPluginExecutionException("Service Url has not been specified in configuration.");
            if (!cachingServiceParameters.ContainsKey(CachingParameter.Api))
                throw new InvalidPluginExecutionException("API has not been specified in configuration.");
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
                throw new InvalidPluginExecutionException("Fallied to refresh cache. Unexpected Error.");
            }
        }

        public void UpdateStatus(bool success, IPluginExecutionContext context, IOrganizationService service)
        {
            if (context == null) throw new InvalidPluginExecutionException("context is null.");
            if (service == null) throw new InvalidPluginExecutionException("organization service is null.");

            var cacheRequest = new Entity(Entities.CacheRequest);
            cacheRequest.Id = context.PrimaryEntityId;
            cacheRequest[Attributes.CacheRequest.StateCode] = new OptionSetValue(1);
            if (success)
                cacheRequest[Attributes.CacheRequest.StatusCode] = new OptionSetValue(2);
            else
                cacheRequest[Attributes.CacheRequest.StatusCode] = new OptionSetValue(950000000);
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

        public void HandleError(Exception ex, IPluginExecutionContext context, IOrganizationService service)
        {
            if (context == null) throw new InvalidPluginExecutionException("context is null.");
            if (service == null) throw new InvalidPluginExecutionException("organization service is null.");

            var errorNote = new Entity(Entities.Annotation);
            errorNote[Attributes.Annotation.Subject] = $"Caching Job Failed";
            if (ex == null)
                errorNote[Attributes.Annotation.NoteText] = "Unexpected error has occurred.";
            else
                errorNote[Attributes.Annotation.NoteText] = $"{ ex.Message} :: Stack Trace :{ex.StackTrace.ToString()}";
            errorNote[Attributes.Annotation.ObjectId] = new EntityReference(Entities.CacheRequest, context.PrimaryEntityId);
            service.Create(errorNote);
        }

        private string GetRequestDataFrom(string token, IPluginExecutionContext context, ITracingService trace)
        {
            if (context == null) throw new InvalidPluginExecutionException("context is null.");
            if (trace == null) throw new InvalidPluginExecutionException("trace is null.");

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

        private string CreateJWTToken(Dictionary<string, string> cachingServiceParameters, IPluginExecutionContext context, ITracingService trace, IOrganizationService service)
        {

            if (cachingServiceParameters == null) throw new InvalidPluginExecutionException("caching parameters is null.");
            if (trace == null) throw new InvalidPluginExecutionException("trace is null.");
            trace.Trace("Start - CreateJWTToken");

            Token token = new Model.Token();
            token.IssuedAtTime = GetIssuedAtTime(cachingServiceParameters, trace).ToString();
            token.NotBeforeTime = GetNotBeforeTime(cachingServiceParameters, trace).ToString();
            token.Expiry = GetExpiry(cachingServiceParameters, trace).ToString();
            token.PrivateKey = cachingServiceParameters[CachingParameter.PrivateKey];

            var requestData = JsonHelper<Token>.SerializeJson(token);

            var url = cachingServiceParameters[CachingParameter.ServiceUrl];
            var api = cachingServiceParameters[CachingParameter.TokenApi]; ;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            Task<HttpResponseMessage> t = client.PostAsync(api, new StringContent(requestData, Encoding.UTF8, "application/json"));

            try
            {

                var response = t.Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    trace.Trace("Success: Token Api");
                    Task<string> task = response.Content.ReadAsStringAsync();
                    var content = task.Result;
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        HandleError(response, context, service);
                        throw new InvalidPluginExecutionException("token retrieved is null.");
                    }
                    return content.Trim('"');
                }
                throw new InvalidPluginExecutionException("Unable to retrieve token");
            }
            catch (Exception ex)
            {
                trace.Trace($"{ex.Message}::{ex.StackTrace.ToString()}");
                throw new InvalidPluginExecutionException("Unable to retrieve token");
            }
        }

        private double GetExpiry(Dictionary<string, string> cachingServiceParameters, ITracingService trace)
        {
            if (cachingServiceParameters == null) throw new InvalidPluginExecutionException("caching parameters is null.");

            if (!cachingServiceParameters.ContainsKey(CachingParameter.ExpirySecondsFromNow))
                throw new InvalidPluginExecutionException("ExpirySecondsFromNow has not been specified in configuration.");

            try
            {
                var sec = Int32.Parse(cachingServiceParameters[CachingParameter.ExpirySecondsFromNow]);
                return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds + sec);
            }
            catch (FormatException ex)
            {
                trace.Trace($"{ex.Message}::{ex.StackTrace.ToString()}");
                throw new InvalidPluginExecutionException("ExpirySecondsFromNow has an incorrect format.");
            }
        }

        private double GetIssuedAtTime(Dictionary<string, string> cachingServiceParameters, ITracingService trace)
        {
            if (cachingServiceParameters == null) throw new InvalidPluginExecutionException("caching parameters is null.");

            if (!cachingServiceParameters.ContainsKey(CachingParameter.IssuedAtTimeFromNow))
                throw new InvalidPluginExecutionException("IssuedAtTimeFromNow has not been specified in configuration.");

            try
            {
                var sec = Int32.Parse(cachingServiceParameters[CachingParameter.IssuedAtTimeFromNow]);
                return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds + sec);
            }
            catch (FormatException ex)
            {
                trace.Trace($"{ex.Message}::{ex.StackTrace.ToString()}");
                throw new InvalidPluginExecutionException("IssuedAtTimeFromNow has an incorrect format.");
            }
        }

        private double GetNotBeforeTime(Dictionary<string, string> cachingServiceParameters, ITracingService trace)
        {
            if (cachingServiceParameters == null) throw new InvalidPluginExecutionException("caching parameters is null.");
            if (!cachingServiceParameters.ContainsKey(CachingParameter.NotBeforeTimeFromNow))
                throw new InvalidPluginExecutionException("NotBeforeTimeFromNow has not been specified in configuration.");

            try
            {
                var sec = Int32.Parse(cachingServiceParameters[CachingParameter.NotBeforeTimeFromNow]);
                return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds + sec);
            }
            catch (FormatException ex)
            {
                trace.Trace($"{ex.Message}::{ex.StackTrace.ToString()}");
                throw new InvalidPluginExecutionException("NotBeforeTimeFromNow has an incorrect format.");
            }
        }

        private Dictionary<string, string> GetCachingServiceParameters(ITracingService trace, IOrganizationService service)
        {
            if (service == null) throw new InvalidPluginExecutionException("organization service is null.");
            if (trace == null) throw new InvalidPluginExecutionException("trace is null.");

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

            if (response == null) throw new InvalidPluginExecutionException("Configuration values for caching is missing.");
            if (response.Entities == null || response.Entities.Count < 5) throw new InvalidPluginExecutionException("Configuration values for caching is missing.");

            var parameters = new Dictionary<string, string>();
            foreach (var e in response.Entities)
            {
                var key = e.Contains(Attributes.ConfigurationKeys.Name) ? e[Attributes.ConfigurationKeys.Name].ToString() : null;
                var value = e.Contains(Attributes.ConfigurationKeys.Value) ? e[Attributes.ConfigurationKeys.Value].ToString() : null;
                var longValue = e.Contains(Attributes.ConfigurationKeys.LongValue) ? e[Attributes.ConfigurationKeys.LongValue].ToString() : null;
                trace.Trace($"{key}:{value}:{longValue}");
                if (string.IsNullOrWhiteSpace(key) || (string.IsNullOrWhiteSpace(value) && string.IsNullOrWhiteSpace(longValue)))
                    throw new InvalidPluginExecutionException("Issue while retrieving configuration records.");

                if (!string.IsNullOrWhiteSpace(value))
                    parameters.Add(key, value);

                if (!string.IsNullOrWhiteSpace(longValue))
                    parameters.Add(key, longValue);
            }

            if (!parameters.ContainsKey(CachingParameter.ServiceUrl) ||
                !parameters.ContainsKey(CachingParameter.Api) ||
                !parameters.ContainsKey(CachingParameter.PrivateKey) ||
                !parameters.ContainsKey(CachingParameter.IssuedAtTimeFromNow) ||
                !parameters.ContainsKey(CachingParameter.ExpirySecondsFromNow) ||
                !parameters.ContainsKey(CachingParameter.NotBeforeTimeFromNow))
                throw new InvalidPluginExecutionException("Issue while retrieving configuration records.");

            trace.Trace("End - GetCachingServiceParameters");
            return parameters;
        }
    }
}
