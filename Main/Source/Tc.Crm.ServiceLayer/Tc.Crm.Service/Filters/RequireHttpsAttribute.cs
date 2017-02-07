using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Tc.Crm.Service.Filters
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class RequireHttpsAttribute : AuthorizationFilterAttribute
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            //guard clause
            if (actionContext == null) throw new ArgumentNullException(Constants.Parameters.ActionContext);

            var setting = ConfigurationManager.AppSettings[Constants.Configuration.AppSettings.RedirectToHttps];
            if (!setting.Equals(Constants.General.TrueValue, StringComparison.OrdinalIgnoreCase))
            {
                base.OnAuthorization(actionContext);
                return;
            }

            if (actionContext.Request.RequestUri.Scheme != Uri.UriSchemeHttps)
            {
                Trace.TraceWarning("Request uri scheme is not https.");
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                {
                    ReasonPhrase = Constants.Messages.HttpsRequired
                };
            }
            else
            {
                base.OnAuthorization(actionContext);
            }
        }
    }
}