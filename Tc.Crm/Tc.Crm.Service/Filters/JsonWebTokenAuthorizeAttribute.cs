using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using Tc.Crm.Service.Services;

namespace Tc.Crm.Service.Filters
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class JsonWebTokenAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            //guard clause
            if (actionContext == null) throw new ArgumentNullException(Constants.Parameters.ActionContext);
            var request = JsonWebTokenHelper.GetRequestObject(actionContext.Request);
            //presence of errors indicate bad request
            if (request.Errors != null && request.Errors.Count > 0)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = Constants.Messages.JsonWebTokenParserError
                };
                //todo: logging
                return;
            }
            //check token validation flags
            if (!request.HeaderAlgorithmValid || !request.HeaderTypeValid || !request.IssuedAtTimeValid || !request.NotBeforetimeValid || !request.SignatureValid)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                {
                    ReasonPhrase = Constants.Messages.JsonWebTokenExpiredOrNoMatch
                };
                //todo: logging
                return;
            }
            //todo: logging
        }
    }
}