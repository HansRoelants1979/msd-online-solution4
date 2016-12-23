using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using Tc.Crm.Service.BusinessServices;
using Tc.Crm.Service.BusinessServices.Custom;

namespace Tc.Crm.Service.Filters
{
    public class ApiAuthenticationFilter:GenericAuthenticationFilter
    {
        protected override bool OnAuthorizeUser(string username, string password, HttpActionContext actionContext)
        {
            IUserService provider = new CustomUserService();
            if (provider != null)
            {
                var userId = provider.Authenticate(username, password);
                if (userId>0)
                {
                    var basicAuthenticationIdentity = Thread.CurrentPrincipal.Identity as BasicAuthenticationIdentity;
                    if (basicAuthenticationIdentity != null)
                        basicAuthenticationIdentity.UserId = userId;
                    return true;
                }
            }
            return false;
        }
    }
}