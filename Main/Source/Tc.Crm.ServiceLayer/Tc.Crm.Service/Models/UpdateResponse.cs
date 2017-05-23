using System.Runtime.Serialization;

namespace Tc.Crm.Service.Models
{
    public class UpdateResponse
    {
        [DataMember]
        public bool Created { get; set; }
        [DataMember]
        public string Id { get; set; }
    }
}