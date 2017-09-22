using System.Runtime.Serialization;

namespace Tc.Crm.Service.Models
{
    public class CustomerResponse
    {
        [DataMember]
        public bool Existing { get; set; }
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public bool Updated { get; set; }
        [DataMember]
        public bool Create { get; set; }
        [DataMember]
        public string EntityName { get; set; }
        [DataMember]
        public string Details { get; set; }
        [DataMember]
        public string Key { get; set; }
    }
}