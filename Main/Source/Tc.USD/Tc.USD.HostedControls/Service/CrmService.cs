using Microsoft.Xrm.Sdk.Query;
using System;
using tcm = Tc.Usd.HostedControls.Models;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Messages;
using Er = Tc.Crm.Common.Constants.EntityRecords;
using Tc.Crm.Common.Constants.Attributes;
using Microsoft.Xrm.Sdk;

namespace Tc.Usd.HostedControls.Service
{
    public class CrmService
    {
        public static void GetConsultationReferenceFromBookingSummary(CrmServiceClient client, tcm.WebRioSsoConfig configuration)
        {
            if (client == null) return;
            if (configuration == null) return;

            RetrieveRequest request = new RetrieveRequest
            {
                ColumnSet = new ColumnSet("tc_name"),
                Target = new Microsoft.Xrm.Sdk.EntityReference("tc_bookingsummary", new Guid(configuration.BookingSummaryId))
            };
            var response = (RetrieveResponse)client.ExecuteCrmOrganizationRequest(request, "Fetching booking summary record for Web Rio");
            if (response == null) return;
            var bookingSummary = response.Entity;
            if (bookingSummary == null || !bookingSummary.Contains("tc_name") || bookingSummary["tc_name"] == null)
                return;
            configuration.ConsultationReference = bookingSummary["tc_name"].ToString();


        }

        public static void GetWebRioSsoConfiguration(CrmServiceClient client, tcm.WebRioSsoConfig configuration)
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

            var response = (RetrieveMultipleResponse)client.ExecuteCrmOrganizationRequest(new RetrieveMultipleRequest { Query = new FetchExpression(query) }, "Fetching cnfiguration records for Web Rio");
            if (response == null) return;
            var configRecords = response.EntityCollection;
            if (configRecords == null || configRecords.Entities == null || configRecords.Entities.Count == 0)
                return;

            foreach (var item in configRecords.Entities)
            {
                if (!item.Contains(Configuration.Name)) continue;
                if (!item.Contains(Configuration.Value)) continue;

                var key = item[Configuration.Name] != null ? item[Configuration.Name].ToString() : null;
                var value = item[Configuration.Value] != null ? item[Configuration.Value].ToString() : null;

                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value)) return;

                if (key.Equals(Er.Configuration.WebRioAdminApi, StringComparison.OrdinalIgnoreCase))
                    configuration.AdminApi = value;
                else if (key.Equals(Er.Configuration.WebRioOpenConsultationApi, StringComparison.OrdinalIgnoreCase))
                    configuration.OpenConsultationApi = value;
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

        public static tcm.SsoLogin GetSsoLoginDetails(CrmServiceClient client, Guid userId)
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
            query = string.Format(query, userId.ToString());
            var response = (RetrieveMultipleResponse)client.ExecuteCrmOrganizationRequest(new RetrieveMultipleRequest { Query = new FetchExpression(query) }, "Fetching secure configuration records for Web Rio");
            if (response == null) return null;
            var loginRecords = response.EntityCollection;
            if (loginRecords == null || loginRecords.Entities == null || loginRecords.Entities.Count == 0)
                return null;

            var loginRecord = loginRecords[0];
            var login = new tcm.SsoLogin();

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

        public static tcm.Customer GetCustomerDataForWebRioNewConsultation(CrmServiceClient client, string customerId)
        {
            var query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='contact'>
                                <attribute name='telephone1' />
                                <attribute name='lastname' />
                                <attribute name='firstname' />
                                <attribute name='tc_emailaddress1type' />
                                <attribute name='emailaddress1' />
                                <attribute name='tc_telephone1type' />
                                <attribute name='tc_address1_town' />
                                <attribute name='tc_address1_street' />
                                <attribute name='tc_address1_postalcode' />
                                <attribute name='tc_address1_housenumberorbuilding' />
                                <attribute name='tc_address1_flatorunitnumber' />
                                <attribute name='tc_address1_county' />
                                <attribute name='tc_address1_countryid' />
                                <attribute name='tc_address1_additionalinformation' />
                                <attribute name='contactid' />
                                <attribute name='telephone3' />
                                <attribute name='telephone2' />
                                <attribute name='tc_telephone3type' />
                                <attribute name='tc_telephone2type' />
                                <attribute name='tc_sourcesystemid' />
                                <attribute name='tc_salutation' />
                                <attribute name='middlename' />
                                <attribute name='birthdate' />
                                <attribute name='tc_emailaddress3type' />
                                <attribute name='emailaddress3' />
                                <attribute name='tc_emailaddress2type' />
                                <attribute name='emailaddress2' />
                                <filter type='and'>
                                  <condition attribute='contactid' operator='eq' value='{0}' />
                                </filter>
                                <link-entity name='tc_country' from='tc_countryid' to='tc_address1_countryid' visible='false' link-type='outer' alias='a1'>
                                  <attribute name='tc_iso_code' />
                                </link-entity>
                                <link-entity name='tc_country' from='tc_countryid' to='tc_address2_countryid' visible='false' link-type='outer' alias='a2'>
                                  <attribute name='tc_iso_code' />
                                </link-entity>
                              </entity>
                            </fetch>";
            query = string.Format(query, customerId);
            var response = (RetrieveMultipleResponse)client.ExecuteCrmOrganizationRequest(new RetrieveMultipleRequest { Query = new FetchExpression(query) }, "Fetching customer record for Web Rio");
            if (response == null) return null;
            var customerRecords = response.EntityCollection;
            if (customerRecords == null || customerRecords.Entities == null || customerRecords.Entities.Count == 0)
                return null;
            var customerRecord = customerRecords.Entities[0];

            var customer = new tcm.Customer();
            customer.CustomerIdentifier = new tcm.CustomerIdentifier
            {
                CustomerId = GetStringValue(customerRecord, "tc_sourcesystemid")
            };
            customer.CustomerIdentity = new Models.CustomerIdentity
            {
                Birthdate = GetStringValue(customerRecord, "birthdate"),
                FirstName = GetStringValue(customerRecord, "firstname"),
                LastName = GetStringValue(customerRecord, "lastname"),
                MiddleName = GetStringValue(customerRecord, "middlename"),
                Salutation = GetStringValue(customerRecord, "tc_salutation"),
            };
            customer.Address = new tcm.Address[]
            {
                new tcm.Address {
                        AdditionalAddressInfo =  GetStringValue(customerRecord,"tc_address1_additionalinformation"),
                        County = GetStringValue(customerRecord,"tc_address1_county"),
                        FlatNumberUnit = GetStringValue(customerRecord,"tc_address1_flatorunitnumber"),
                        HouseNumberBuilding = GetStringValue(customerRecord,"tc_address1_housenumberorbuilding"),
                        PostalCode = GetStringValue(customerRecord,"tc_address1_postalcode"),
                        Street = GetStringValue(customerRecord,"tc_address1_street"),
                        Town = GetStringValue(customerRecord,"tc_address1_town"),
                        Country = GetStringValue(customerRecord,"tc_iso_code","a1")
                    },
                new tcm.Address {
                        AdditionalAddressInfo =  GetStringValue(customerRecord,"tc_address2_additionalinformation"),
                        County = GetStringValue(customerRecord,"tc_address2_county"),
                        FlatNumberUnit = GetStringValue(customerRecord,"tc_address2_flatorunitnumber"),
                        HouseNumberBuilding = GetStringValue(customerRecord,"tc_address2_housenumberorbuilding"),
                        PostalCode = GetStringValue(customerRecord,"tc_address2_postalcode"),
                        Street = GetStringValue(customerRecord,"tc_address2_street"),
                        Town = GetStringValue(customerRecord,"tc_address2_town"),
                        Country = GetStringValue(customerRecord,"tc_iso_code","a2")
                    }
            };

            customer.Email = new tcm.Email[]
            {
                new tcm.Email {Address = GetStringValue(customerRecord,"emailaddress1")
                               , EmailType= GetStringValue(customerRecord,"tc_emailaddress1type")},
                new tcm.Email {Address = GetStringValue(customerRecord,"emailaddress2")
                               , EmailType= GetStringValue(customerRecord,"tc_emailaddress2type")},
                new tcm.Email {Address = GetStringValue(customerRecord,"emailaddress3")
                               , EmailType= GetStringValue(customerRecord,"tc_emailaddress3type")}
            };

            customer.Phone = new tcm.Phone[]
            {
                new tcm.Phone {Number = GetStringValue(customerRecord,"telephone1")
                               , PhoneType = GetStringValue(customerRecord,"tc_telephone1type") },
                new tcm.Phone {Number = GetStringValue(customerRecord,"telephone2")
                               , PhoneType = GetStringValue(customerRecord,"tc_telephone2type") },
                new tcm.Phone {Number = GetStringValue(customerRecord,"telephone3")
                               , PhoneType = GetStringValue(customerRecord,"tc_telephone3type") }
            };

            return customer;
        }

        private static string GetStringValue(Entity e, string fieldName)
        {
            if (!e.Contains(fieldName) || e[fieldName] == null) return null;
            if (e[fieldName] is string)
                return e[fieldName].ToString();
            if (e[fieldName] is DateTime)
                return ((DateTime)e[fieldName]).ToString();
            if (e[fieldName] is OptionSetValue)
                return ((OptionSetValue)e[fieldName]).Value.ToString();
            return null;
        }

        private static string GetStringValue(Entity e, string fieldName,string alias)
        {
            if (!e.Contains($"{alias}.{fieldName}") || e[$"{alias}.{fieldName}"] == null)
                return null;
            var value = ((AliasedValue)e[$"{alias}.{fieldName}"]).Value;
            if (value is string)
                return value.ToString();
            if (value is DateTime)
                return ((DateTime)value).ToString();
            if (value is OptionSetValue)
                return ((OptionSetValue)value).Value.ToString();
            return null;
        }


    }
}
