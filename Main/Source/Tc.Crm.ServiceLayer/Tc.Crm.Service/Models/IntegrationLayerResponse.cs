using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;

namespace Tc.Crm.Service.Models
{
    public class IntegrationLayerResponse
    {
        [DataMember(Name = "correlationId")]
        public string CorrelationId { get; set; }

        [DataMember(Name = "sourceSystemEntityID")]
        public string SourceSystemEntityID { get; set; }

        [DataMember(Name = "sourceSystemStatusCode")]
        public HttpStatusCode SourceSystemStatusCode { get; set; }

        [DataMember(Name = "sourceSystemRequest")]
        public string SourceSystemRequest { get; set; }

        [DataMember(Name = "sourceSystemResponse")]
        public string SourceSystemResponse { get; set; }

    }
        
}