using System;
using Microsoft.Xrm.Sdk;
using System.Configuration;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk.Messages;
using tcm = Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{

    public  class CrmService:ICrmService
    {
        private  IOrganizationService orgService;

        public CrmService()
        {
            orgService = CreateOrgService();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Upsert")]
        public  tcm.UpsertResponse Upsert(Entity entity)
        {
            UpsertRequest request = new UpsertRequest
            {
                Target = entity
            };
            var response =  orgService.Execute<UpsertResponse>(request);
            if (response == null) return null;
            return new tcm.UpsertResponse
            {
                RecordCreated = response.RecordCreated,
                Target = response.Target
            };
        }

        public  IOrganizationService CreateOrgService()
        {

            if (orgService != null) return orgService;

            var connectionString = ConfigurationManager.ConnectionStrings[Constants.Configuration.ConnectionStrings.Crm];
            if (connectionString == null)
            {
                throw new InvalidOperationException(Constants.Messages.ConnectionStringNull);
            }

            CrmConnection crmConnection = null;
            try
            {
                crmConnection = CrmConnection.Parse(connectionString.ConnectionString);
                if (crmConnection == null)
                {
                    throw new InvalidOperationException(Constants.Messages.CrmConnectionIsNull);
                }
            }
            catch (Exception)
            {
                throw new InvalidOperationException(Constants.Messages.CrmConnectionParseError);
            }
            orgService = new OrganizationService(crmConnection);

            return orgService;


        }


    }
}