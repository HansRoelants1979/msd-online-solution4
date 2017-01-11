using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Tc.Crm.Service.Filters
{
    public class RequireHttpsAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var setting = ConfigurationManager.AppSettings[Constants.Configuration.AppSettings.REDIRECT_TO_HTTPS];
            if (!setting.Equals(Constants.TRUE_VALUE, StringComparison.OrdinalIgnoreCase))
            {
                base.OnAuthorization(actionContext);
                return;
            }

            if (actionContext.Request.RequestUri.Scheme != Uri.UriSchemeHttps)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                {
                    ReasonPhrase = Constants.Messages.HTTPS_REQUIRED
                };
            }
            else
            {
                base.OnAuthorization(actionContext);
            }
        }
    }
}