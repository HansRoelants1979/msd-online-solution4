using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Tc.Crm.Service.Models
{
    public class UpdateResponse
    {
        [DataMember]
        public bool Created { get; set; }
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public bool Success { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
    }
}