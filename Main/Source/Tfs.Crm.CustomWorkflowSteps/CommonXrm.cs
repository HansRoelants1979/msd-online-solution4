using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System.ServiceModel;

namespace Tc.Crm.CustomWorkflowSteps
{
    public class CommonXrm
    {
        public Guid UpsertEntity(Entity EntityRecord, OrganizationServiceProxy ServiceProxy)
        {
            Guid RecordId = Guid.Empty;

            UpsertRequest request = new UpsertRequest()
            {
                Target = EntityRecord
            };

            try
            {
                // Execute UpsertRequest and obtain UpsertResponse. 
                UpsertResponse response = (UpsertResponse)ServiceProxy.Execute(request);
                if (response.RecordCreated)
                    RecordId = response.Target.Id;
                else
                    RecordId = response.Target.Id;

               
            }

            // Catch any service fault exceptions that Microsoft Dynamics CRM throws.
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>)
            {
                throw;
            }


            return RecordId;
        }
    }
}
