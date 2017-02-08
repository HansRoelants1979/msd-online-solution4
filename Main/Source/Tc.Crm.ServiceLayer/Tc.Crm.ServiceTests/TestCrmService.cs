using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Tc.Crm.Service.Services;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;

namespace Tc.Crm.ServiceTests
{
    public enum DataSwitch
    {
        Created,
        Updated,
        Response_NULL,
        Response_Failed,
        Return_NULL
    }
    public class TestCrmService : ICrmService
    {
        XrmFakedContext context;

        public DataSwitch Switch { get; set; }

        public TestCrmService(XrmFakedContext context)
        {
            this.context = context;
        }
        public Tc.Crm.Service.Models.UpdateResponse ExecuteActionForBookingUpdate(string data)
        {
            object Constants = null;
            if (Switch == DataSwitch.Created)
                return new Tc.Crm.Service.Models.UpdateResponse { Created = true, Id = Guid.NewGuid().ToString(), Success = true };

            else if (Switch == DataSwitch.Updated)
                return new Tc.Crm.Service.Models.UpdateResponse { Created = false, Id = Guid.NewGuid().ToString(), Success = true };

            else if (Switch == DataSwitch.Response_NULL)
                throw new InvalidOperationException(Tc.Crm.Service.Constants.Messages.ResponseFromCrmIsNull);

            else if (Switch == DataSwitch.Response_Failed)
                return new Tc.Crm.Service.Models.UpdateResponse { Created = false, Id = null, Success = false, ErrorMessage = "Unexpcted Error" };
            else if (Switch == DataSwitch.Return_NULL)
                return null;
            return null;
        }
    }
}
