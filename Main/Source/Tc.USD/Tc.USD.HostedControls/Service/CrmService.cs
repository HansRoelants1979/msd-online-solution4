using Microsoft.Xrm.Sdk.Query;
using System;
using Tc.Usd.HostedControls.Models;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Messages;
using Er = Tc.Crm.Common.Constants.EntityRecords;
using Tc.Crm.Common.Constants.Attributes;

namespace Tc.Usd.HostedControls.Service
{
    public class CrmService
    {
        public static void GetWebRioSsoConfiguration(CrmServiceClient client,WebRioSsoConfig configuration)
        {
            if (client == null) return;
            if (configuration == null) return;

            var query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                            <entity name = 'tc_configuration' >
                                <attribute name = 'tc_configurationid' />
                                <attribute name = 'tc_name' />
                                <attribute name = 'tc_value' />
                                <attribute name = 'tc_longvalue' />
                                <attribute name = 'tc_group' />
                                <order attribute = 'tc_name' descending = 'false' />
                                <filter type = 'and' >
                                    <condition attribute = 'tc_group' operator= 'eq' value = 'WebRio' />
                                    <condition attribute = 'statecode' operator= 'eq' value = '0' />
                                </filter >
                            </entity >
                        </fetch > ";

            var response = (RetrieveMultipleResponse)client.ExecuteCrmOrganizationRequest(new RetrieveMultipleRequest { Query=new FetchExpression(query)},"Fetching cnfiguration records for Web Rio");
            if (response == null) return;
            var configRecords = response.EntityCollection;
            if (configRecords == null || configRecords.Entities == null || configRecords.Entities.Count == 0)
                return;
            
            foreach (var item in configRecords.Entities)
            {
                if (!item.Contains(Configuration.Name)) continue;
                if (!item.Contains(Configuration.Value)) continue;

                var key = item[Configuration.Name] != null ? item[Configuration.Name].ToString():null;
                var value = item[Configuration.Value] != null ? item[Configuration.Value].ToString() : null;

                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value)) return;

                if (key.Equals(Er.Configuration.WebRioAdminApi, StringComparison.OrdinalIgnoreCase))
                    configuration.AdminApi = value;
                else if (key.Equals(Er.Configuration.WebRioServiceUrl, StringComparison.OrdinalIgnoreCase))
                    configuration.ServiceUrl = value;
                else if (key.Equals(Er.Configuration.WebRioExpirySecondsFromNow, StringComparison.OrdinalIgnoreCase))
                    configuration.ExpirySeconds = value;
                else if (key.Equals(Er.Configuration.WebRioNotBeforeTimeSecondsFromNow, StringComparison.OrdinalIgnoreCase))
                    configuration.NotBeforeTime = value;
                else
                    continue;
            }
        }

        public static string GetWebRioPrivateKey(CrmServiceClient client)
        {
            if (client == null) return null;

            var query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='tc_secureconfiguration'>
                                <attribute name='tc_secureconfigurationid' />
                                <attribute name='tc_name' />
                                <attribute name='tc_value' />
                                <order attribute='tc_name' descending='false' />
                                <filter type='and'>
                                  <condition attribute='tc_name' operator='eq' value='Tc.Wr.JwtPrivateKey' />
                                </filter>
                              </entity>
                            </fetch>";

            var response = (RetrieveMultipleResponse)client.ExecuteCrmOrganizationRequest(new RetrieveMultipleRequest { Query = new FetchExpression(query) }, "Fetching secure configuration records for Web Rio");
            if (response == null) return null;
            var configRecords = response.EntityCollection;
            if (configRecords == null || configRecords.Entities == null || configRecords.Entities.Count == 0)
                return null;
            return configRecords.Entities[0][SecureConfiguration.Value].ToString();
        }

        public static SsoLogin GetSsoLoginDetails(CrmServiceClient client, Guid userId)
        {
            if (userId == Guid.Empty) return null;
            if (client == null) return null;

            var query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='tc_externallogin'>
                                <attribute name='tc_externalloginid' />
                                <attribute name='tc_initials' />
                                <attribute name='tc_employeeid' />
                                <attribute name='tc_branchcode' />
                                <attribute name='tc_abtanumber' />
                                <filter type='and'>
                                  <condition attribute='ownerid' operator='eq' value='{0}' />
                                </filter>
                              </entity>
                            </fetch>";
            query = string.Format(query,userId.ToString());
            var response = (RetrieveMultipleResponse)client.ExecuteCrmOrganizationRequest(new RetrieveMultipleRequest { Query = new FetchExpression(query) }, "Fetching secure configuration records for Web Rio");
            if (response == null) return null;
            var loginRecords = response.EntityCollection;
            if (loginRecords == null || loginRecords.Entities == null || loginRecords.Entities.Count == 0)
                return null;

            var loginRecord = loginRecords[0];
            var login = new SsoLogin();

            if (loginRecord.Contains(ExternalLogin.AbtaNumber) && loginRecord[ExternalLogin.AbtaNumber] != null)
                login.AbtaNumber = loginRecord[ExternalLogin.AbtaNumber].ToString();
            if (loginRecord.Contains(ExternalLogin.BranchCode) && loginRecord[ExternalLogin.BranchCode] != null)
                login.BranchCode = loginRecord[ExternalLogin.BranchCode].ToString();
            if (loginRecord.Contains(ExternalLogin.EmployeeId) && loginRecord[ExternalLogin.EmployeeId] != null)
                login.EmployeeId = loginRecord[ExternalLogin.EmployeeId].ToString();
            if (loginRecord.Contains(ExternalLogin.Initials) && loginRecord[ExternalLogin.Initials] != null)
                login.Initials = loginRecord[ExternalLogin.Initials].ToString();

            return login;

        }

      
    }
}
