using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Tc.Crm.Service.Services;

namespace Tc.Crm.Service.Filters
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class JwtAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(HttpActionContext ctx)
        {

            var r = JwtHelper.GetRequestObject(ctx.Request);
            //presence of errors indicate bad request
            if (r.Errors != null && r.Errors.Count > 0)
            {
                ctx.Response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = "Error has occurred while parsing the token."
                };

                return;
            }
            //check token validation flags
            if (!r.AlgOk || !r.TypeOk || !r.IatOk || !r.NbfOk || !r.SignatureOk)
            {
                ctx.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                {
                    ReasonPhrase = "Token has either expired or signature doesn't match."
                };
            }

        }
    }
}