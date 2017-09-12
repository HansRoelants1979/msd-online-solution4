using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tc.Crm.Common.IntegrationLayer.Jti.Models
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

        public override Dictionary<string, object> GeneratePayload()
        {
            var payload = base.GeneratePayload();

            payload.Add("jti", Jti);
            payload.Add("aud", Aud);
            payload.Add("bra", BranchCode);
            payload.Add("abt", AbtaNumber);
            payload.Add("emp", EmployeeId);
            payload.Add("ini", Initials);
            payload.Add("crt", CreatedBy);

            return payload;
        }
    }
}