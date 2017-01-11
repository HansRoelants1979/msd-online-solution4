using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using Tc.Crm.Service.Services;

namespace Tc.Crm.Service.Filters
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class JsonWebTokenAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {

            var request = JsonWebTokenHelper.GetRequestObject(actionContext.Request);
            //presence of errors indicate bad request
            if (request.Errors != null && request.Errors.Count > 0)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = Constants.Messages.JSON_WEB_TOKEN_PARSES_ERROR
                };
                //todo: logging
                return;
            }
            //check token validation flags
            if (!request.HeaderAlgorithmValid || !request.HeaderTypeValid || !request.IssuedAtTimeValid || !request.NotBeforeTimeValid || !request.SignaturValid)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                {
                    ReasonPhrase = Constants.Messages.JSON_WEB_TOKEN_EXPIRED_NO_MATCH
                };
                //todo: logging
                return;
            }
            //todo: logging
        }
    }
}