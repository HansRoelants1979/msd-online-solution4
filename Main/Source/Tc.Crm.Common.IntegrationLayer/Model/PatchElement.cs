using Newtonsoft.Json;

namespace Tc.Crm.Common.IntegrationLayer.Model
{
    public class PatchElement
    {
        [JsonProperty(PropertyName = "op")]
        public string Operator { get; set; }

        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }
}