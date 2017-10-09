using System.Runtime.Serialization;

namespace Tc.Crm.Service.Models
{

    [DataContract]
    public class PatchParameter
    {
        [DataMember]
        public string Parent { get; set; }
        [DataMember]
        public string Name { get; set; }
    }

}
