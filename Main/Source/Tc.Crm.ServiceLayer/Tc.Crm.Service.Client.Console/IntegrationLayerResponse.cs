using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Tc.Crm.Service.Client.Console
{
    public class IntegrationLayerResponse
    {
        [JsonProperty(PropertyName = "correlationId")]
        public string CorrelationId { get; set; }

        [JsonProperty(PropertyName = "sourceSystemEntityID")]
        public string SourceSystemEntityId { get; set; }

        [JsonProperty(PropertyName = "sourceSystemStatusCode")]
        public string SourceSystemStatusCode { get; set; }

        [JsonProperty(PropertyName = "sourceSystemRequest")]
        public string SourceSystemRequest { get; set; }

        [DataMember(Name = "sourceSystemResponse")]
        public string SourceSystemResponse { get; set; }
    }
}