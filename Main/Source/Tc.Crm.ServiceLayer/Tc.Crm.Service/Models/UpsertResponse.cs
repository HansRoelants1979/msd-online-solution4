using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tc.Crm.Service.Models
{
    public class UpsertResponse
    {
        public bool RecordCreated { get; set; }
        public EntityReference Target { get; set; }
    }
}