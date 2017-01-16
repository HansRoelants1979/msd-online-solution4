using System;
using Microsoft.Xrm.Sdk;
using System.Configuration;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk.Messages;

namespace Tc.Crm.Service.Services
{

    public static class CrmService
    {
        private static IOrganizationService orgService;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Upsert")]
        public static UpsertResponse Upsert(Entity entity)
        {
            var service = CreateOrgService();
            UpsertRequest request = new UpsertRequest
            {
                Target = entity
            };
            return service.Execute<UpsertResponse>(request);
        }

        public static IOrganizationService CreateOrgService()
        {

            if (orgService != null) return orgService;

            var connectionString = ConfigurationManager.ConnectionStrings[Constants.Configuration.ConnectionStrings.Crm];
            if (connectionString == null)
            {
                //todo:logging
                throw new InvalidOperationException(Constants.Messages.ConnectionStringNull);
            }
            //todo:logging
            CrmConnection crmConnection = null;
            try
            {
                crmConnection = CrmConnection.Parse(connectionString.ConnectionString);
                if (crmConnection == null)
                {
                    //todo:logging
                    throw new InvalidOperationException(Constants.Messages.CrmConnectionIsNull);
                }
            }
            catch (Exception)
            {
                throw new InvalidOperationException(Constants.Messages.CrmConnectionParseError);
            }
            orgService = new OrganizationService(crmConnection);

            //TODO:logging
            return orgService;


        }


    }
}