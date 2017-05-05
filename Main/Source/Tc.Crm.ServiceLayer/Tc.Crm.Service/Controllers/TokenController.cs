using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tc.Crm.Service.Models;
using Tc.Crm.Service.Services;

namespace Tc.Crm.Service.Controllers
{
    public class TokenController : ApiController
    {
        IJwtTokenService tokenService;
        public TokenController(IJwtTokenService tokenService)
        {
            this.tokenService = tokenService;
        }
        [Route("api/v1/token/Generate")]
        [Route("api/token/Generate")]
        [HttpPost]
        public HttpResponseMessage GenerateToken(Token token)
        {
            var jwtToken = tokenService.CreateJWTToken(token);
            return Request.CreateResponse(HttpStatusCode.OK, jwtToken);
        }
    }
}
