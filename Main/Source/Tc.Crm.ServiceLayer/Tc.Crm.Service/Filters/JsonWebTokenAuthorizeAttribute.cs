using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using Tc.Crm.Service.Services;

namespace Tc.Crm.Service.Filters
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class JsonWebTokenAuthorizeAttribute : AuthorizeAttribute
    {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            //guard clause
            if (actionContext == null) throw new ArgumentNullException(Constants.Parameters.ActionContext);
            JsonWebTokenHelper helper = new JsonWebTokenHelper(new ConfigurationService());
            var request = helper.GetRequestObject(actionContext.Request);
            //presence of errors indicate bad request
            if (request.Errors != null && request.Errors.Count > 0)
            {
                Trace.TraceWarning("Bad Request: Error while parsing the request object");
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = Constants.Messages.JsonWebTokenParserError
                };
                return;
            }
            //check token validation flags
            if (!request.HeaderAlgorithmValid 
                || !request.HeaderTypeValid 
                || !request.IssuedAtTimeValid 
                || !request.NotBeforetimeValid 
                || !request.SignatureValid 
                || !request.ExpiryValid)
            {
                Trace.TraceWarning("Bad Request: One or more information is missing in the token or signature didn't match.");
                Trace.TraceWarning("Type:{0},Algorithm:{1},IatValid:{2},NbfValid:{3},SignValid:{4},Expiry:{5}"
                                     ,request.HeaderTypeValid
                                     , request.HeaderAlgorithmValid
                                     ,request.IssuedAtTimeValid
                                     ,request.NotBeforetimeValid
                                     ,request.SignatureValid
                                     ,request.ExpiryValid);
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                {
                    ReasonPhrase = Constants.Messages.JsonWebTokenExpiredOrNoMatch
                };
                return;
            }
        }
    }
}