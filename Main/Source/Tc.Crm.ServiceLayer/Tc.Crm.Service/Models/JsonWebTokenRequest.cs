using System.Collections.ObjectModel;

namespace Tc.Crm.Service.Models
{
    public class JsonWebTokenRequest
    {
        public JsonWebTokenRequest()
        {
            Errors = new Collection<JsonWebTokenRequestError>();
        }

        public JsonWebTokenHeader Header { get; set; }

        public JsonWebTokenPayload Payload { get; set; }

        public string Token { get; set; }

        public bool HeaderAlgorithmValid { get; set; }

        public bool HeaderTypeValid { get; set; }

        public bool IssuedAtTimeValid { get; set; }

        public bool NotBeforetimeValid { get; set; }

        public bool ExpiryValid { get; set; }

        public bool SignatureValid { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public Collection<JsonWebTokenRequestError> Errors { get; set; }
    }

    public class JsonWebTokenRequestError
    {
        public JsonWebTokenRequestError()
        { }
        public JsonWebTokenRequestError(string message)
        {
            this.Message = message;
        }
        public JsonWebTokenRequestError(string message,string trace)
        {
            this.Message = message;
            this.StackTrace = trace;
        }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}