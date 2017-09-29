using System.Collections.Generic;
using System.Net;

namespace Tc.Crm.Common.IntegrationLayer.Model
{
    public class ResponseEntity
    {
        public string Content { get; set; }

        public HttpStatusCode StatusCode { get; set; }
        public Dictionary<string, string> Cookies { get; set; }
    }
}