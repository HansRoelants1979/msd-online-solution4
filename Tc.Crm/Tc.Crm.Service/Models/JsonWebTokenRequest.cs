using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tc.Crm.Service.Models
{
    public class JsonWebTokenRequest
    {
        public JsonWebTokenRequest()
        {
            Errors = new List<JsonWebTokenRequestError>();
        }

        public JsonWebTokenHeader Header { get; set; }

        public JsonWebTokenPayload Payload { get; set; }

        public string Token { get; set; }

        public bool HeaderAlgorithmValid { get; set; }

        public bool HeaderTypeValid { get; set; }

        public bool HeaderValid { get; set; }

        public bool IssuedAtTimeValid { get; set; }

        public bool NotBeforeTimeValid { get; set; }

        public bool SignaturValid { get; set; }

        public List<JsonWebTokenRequestError> Errors { get; set; }
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