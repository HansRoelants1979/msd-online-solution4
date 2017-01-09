using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tc.Crm.Service.Models
{
    public class JwtRequest
    {
        public JwtRequest()
        {
            Errors = new List<JwtRequestError>();
        }
        public JwtHeader Header { get; set; }

        public JwtPayload Payload { get; set; }

        public string Token { get; set; }

        public bool AlgOk { get; set; }

        public bool TypeOk { get; set; }

        public bool HeaderOk { get; set; }

        public bool IatOk { get; set; }

        public bool NbfOk { get; set; }

        public bool SignatureOk { get; set; }

        public List<JwtRequestError> Errors { get; set; }
    }

    public class JwtRequestError
    {
        public JwtRequestError()
        { }
        public JwtRequestError(string message)
        {
            this.Message = message;
        }
        public JwtRequestError(string message,string trace)
        {
            this.StackTrace = trace;
        }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}