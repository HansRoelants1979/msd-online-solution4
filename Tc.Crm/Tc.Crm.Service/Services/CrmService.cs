using System;
using Microsoft.Xrm.Sdk;
using System.Configuration;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk.Messages;

namespace Tc.Crm.Service.Services
{

    public class CrmService
    {
        private static IOrganizationService orgService;

        public static UpsertResponse Upsert(Entity entity)
        {
            var service = GetOrgService();
            UpsertRequest request = new UpsertRequest
            {
                Target = entity
            };
            return service.Execute<UpsertResponse>(request);
        }

        public static IOrganizationService GetOrgService()
        {

            if (orgService != null) return orgService;

            var connectionString = ConfigurationManager.ConnectionStrings[Constants.Configuration.ConnectionStrings.CRM];
            if (connectionString == null)
            {
                //todo:logging
                throw new InvalidOperationException(Constants.Messages.CONNECTION_STRING_NULL);
            }
            //todo:logging
            CrmConnection crmConnection = null;
            try
            {
                crmConnection = CrmConnection.Parse(connectionString.ConnectionString);
                if (crmConnection == null)
                {
                    //todo:logging
                    throw new InvalidOperationException(Constants.Messages.CRM_CONNECTION_IS_NULL);
                }
            }
            catch (Exception)
            {
                throw new InvalidOperationException(Constants.Messages.CRM_CONNECTION_PARSE_ERROR);
            }
            orgService = new OrganizationService(crmConnection);

            //TODO:logging
            return orgService;


        }


    }
}