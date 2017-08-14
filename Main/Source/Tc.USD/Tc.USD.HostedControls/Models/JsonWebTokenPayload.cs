using System.Runtime.Serialization;

namespace Tc.Usd.HostedControls.Models
{
    [DataContract]
    public class JsonWebTokenPayload
    {
        [DataMember(Name = "nbf")]
        public string NotBefore { get; set; }

        [DataMember(Name = "iat")]
        public string IssuedAtTime { get; set; }

        [DataMember(Name = "exp")]
        public string Expiry { get; set; }

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