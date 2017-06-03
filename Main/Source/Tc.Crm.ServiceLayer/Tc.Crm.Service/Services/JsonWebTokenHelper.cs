using System;
using System.Net.Http;
using Tc.Crm.Service.Models;
using System.Diagnostics;

namespace Tc.Crm.Service.Services
{
    public class JsonWebTokenHelper
    {
        private readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        private IConfigurationService configurationService;
        private JsonWebTokenServiceBase jsonWebTokenService;

        public JsonWebTokenHelper(IConfigurationService configurationService, Api contextApi)
        {
            this.configurationService = configurationService;
            if (contextApi == Api.Booking)
                jsonWebTokenService = new BookingJsonWebTokenService(configurationService);
            else if(contextApi == Api.Survey)
                jsonWebTokenService = new SurveyJsonWebTokenService(configurationService);
            else if (contextApi == Api.Caching)
                jsonWebTokenService = new CacheJsonWebTokenService(configurationService);
            else
                throw new InvalidOperationException("Invalid API has been called.");

        }
        /// <summary>
        /// Parses the request to an object
        /// Validates the Header pay load and signature
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Tc.Crm.Service.Models.JsonWebTokenRequestError.#ctor(System.String)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public JsonWebTokenRequest GetRequestObject(HttpRequestMessage request)
        {
            var jsonWebTokenRequest = new JsonWebTokenRequest();
            try
            {
                if (request == null)
                    throw new ArgumentNullException("request");

                Trace.TraceInformation("requst is not null");
                //parse json web token parts
                jsonWebTokenRequest.Token = jsonWebTokenService.GetToken(request);
                Trace.TraceInformation("token: {0}", jsonWebTokenRequest.Token);
                jsonWebTokenRequest.Header = jsonWebTokenService.DecodeHeaderToObject<JsonWebTokenHeader>(jsonWebTokenRequest.Token);
                Trace.TraceInformation("algo: {0},type:{1}", jsonWebTokenRequest.Header.Algorithm, jsonWebTokenRequest.Header.TokenType);
                jsonWebTokenRequest.Payload = jsonWebTokenService.DecodePayloadToObject<JsonWebTokenPayload>(jsonWebTokenRequest.Token);
                Trace.TraceInformation("iat: {0},nbf:{1},exp:{2}", jsonWebTokenRequest.Payload.IssuedAtTime, jsonWebTokenRequest.Payload.NotBefore, jsonWebTokenRequest.Payload.Expiry);


                //validate the parts
                jsonWebTokenService.ValidateHeader(jsonWebTokenRequest);
                jsonWebTokenService.ValidatePayload(jsonWebTokenRequest);
                jsonWebTokenService.ValidateSignature(jsonWebTokenRequest);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Unexpected error in GetRequestObject::Message:{0}||Trace:{1}", ex.Message, ex.StackTrace);
                jsonWebTokenRequest.Errors.Add(new JsonWebTokenRequestError(ex.Message, ex.StackTrace));
                jsonWebTokenRequest.Errors.Add(new JsonWebTokenRequestError(Constants.Messages.JsonWebTokenParserError));
            }

            return jsonWebTokenRequest;
        }

        public JsonWebTokenRequest GetRequestObject(Payload payload)
        {
            var jsonWebTokenRequest = new JsonWebTokenRequest();
            try
            {
                if (payload == null || string.IsNullOrWhiteSpace(payload.JWTToken))
                    throw new ArgumentNullException("request");

                Trace.TraceInformation("requst is not null");
                //parse json web token parts
                jsonWebTokenRequest.Token = payload.JWTToken;
                Trace.TraceInformation("token: {0}", jsonWebTokenRequest.Token);
                jsonWebTokenRequest.Header = jsonWebTokenService.DecodeHeaderToObject<JsonWebTokenHeader>(jsonWebTokenRequest.Token);
                Trace.TraceInformation("algo: {0},type:{1}", jsonWebTokenRequest.Header.Algorithm, jsonWebTokenRequest.Header.TokenType);
                jsonWebTokenRequest.Payload = jsonWebTokenService.DecodePayloadToObject<JsonWebTokenPayload>(jsonWebTokenRequest.Token);
                Trace.TraceInformation("iat: {0},nbf:{1},exp:{2}", jsonWebTokenRequest.Payload.IssuedAtTime, jsonWebTokenRequest.Payload.NotBefore, jsonWebTokenRequest.Payload.Expiry);


                //validate the parts
                jsonWebTokenService.ValidateHeader(jsonWebTokenRequest);
                jsonWebTokenService.ValidatePayload(jsonWebTokenRequest);
                jsonWebTokenService.ValidateSignature(jsonWebTokenRequest);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Unexpected error in GetRequestObject::Message:{0}||Trace:{1}", ex.Message, ex.StackTrace);
                jsonWebTokenRequest.Errors.Add(new JsonWebTokenRequestError(ex.Message, ex.StackTrace));
                jsonWebTokenRequest.Errors.Add(new JsonWebTokenRequestError(Constants.Messages.JsonWebTokenParserError));
            }

            return jsonWebTokenRequest;
        }

    }
}