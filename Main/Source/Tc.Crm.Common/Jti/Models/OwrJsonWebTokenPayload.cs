using System.Runtime.Serialization;

namespace Tc.Crm.Common.Jti.Models
{
    [DataContract]
    public class OwrJsonWebTokenPayload: JsonWebTokenPayloadBase
    {
        [DataMember(Name = "jti")]
        public string Jti { get; set; }

        [DataMember(Name = "aud")]
        public string Aud { get; set; }

        [DataMember(Name = "bra")]
        public string BranchCode { get; set; }

        [DataMember(Name = "abt")]
        public string AbtaNumber { get; set; }

        [DataMember(Name = "emp")]
        public string EmployeeId { get; set; }

        [DataMember(Name = "ini")]
        public string Initials { get; set; }

        [DataMember(Name = "crt")]
        public string CreatedBy { get; set; }
    }
}