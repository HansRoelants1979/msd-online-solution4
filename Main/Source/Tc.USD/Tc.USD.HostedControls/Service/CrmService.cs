using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using tcm = Tc.Usd.HostedControls.Models;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Messages;
using Tc.Crm.Common.Constants;
using Er = Tc.Crm.Common.Constants.EntityRecords;
using Tc.Crm.Common.Constants.Attributes;
using Tc.Crm.Common.Services;

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
                else if (key.Equals(Er.Configuration.WebRioNewConsultationApi, StringComparison.OrdinalIgnoreCase))
                    configuration.NewConsultationApi = value;
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
        public static Entity GetOpportunity(CrmServiceClient client, ILogger logger, string opportunityId)
        {
            if (string.IsNullOrEmpty(opportunityId))
                return null;
            var query = new QueryExpression(EntityName.Opportunity)
            {
                ColumnSet =
                    new ColumnSet(Opportunity.CustomerId, Opportunity.Initials, Opportunity.Name,
                        Opportunity.EarliestDepartureDate, Opportunity.LatestDepartureDate, Opportunity.Duration,
                        Opportunity.DeparturePoint1, Opportunity.DeparturePoint2, Opportunity.DeparturePoint3,
                        Opportunity.DestinationCountry1, Opportunity.Region1, Opportunity.DestinationAirport1,
                        Opportunity.Hotel1, Opportunity.DestinationCountry2, Opportunity.Region2,
                        Opportunity.DestinationAirport2, Opportunity.Hotel2, Opportunity.DestinationCountry3,
                        Opportunity.Region3, Opportunity.Hotel3, Opportunity.DestinationAirport3, Opportunity.Exclude1,
                        Opportunity.Exclude2, Opportunity.Exclude3, Opportunity.HowDoYouWantToSearch)
            };
            query.LinkEntities.Add(new LinkEntity(EntityName.Opportunity, EntityName.Contact, Opportunity.CustomerId,
                Contact.ContactId,
                JoinOperator.Inner));
            query.LinkEntities[0].Columns.AddColumns(Contact.ContactId, Crm.Common.Constants.Attributes.Customer.SourceSystemId, Crm.Common.Constants.Attributes.Customer.Salutation,
                Crm.Common.Constants.Attributes.Customer.FirstName,
                Crm.Common.Constants.Attributes.Customer.MiddleName,
                Crm.Common.Constants.Attributes.Customer.LastName, Crm.Common.Constants.Attributes.Customer.Birthdate, Crm.Common.Constants.Attributes.Customer.Address1FlatorUnitNumber,
                Crm.Common.Constants.Attributes.Customer.Address1HouseNumberoBuilding,
                Crm.Common.Constants.Attributes.Customer.Address1Town, Crm.Common.Constants.Attributes.Customer.Address1CountryId, Crm.Common.Constants.Attributes.Customer.Address1County, Crm.Common.Constants.Attributes.Customer.Address1PostalCode,
                Crm.Common.Constants.Attributes.Customer.Address2FlatorUnitNumber, Crm.Common.Constants.Attributes.Customer.Address2HouseNumberoBuilding, Crm.Common.Constants.Attributes.Customer.Address2Town,
                Crm.Common.Constants.Attributes.Customer.Address2CountryId, Crm.Common.Constants.Attributes.Customer.Address2County, Crm.Common.Constants.Attributes.Customer.Address2PostalCode,
                Crm.Common.Constants.Attributes.Customer.Telephone1Type,
                Crm.Common.Constants.Attributes.Customer.Telephone1, Crm.Common.Constants.Attributes.Customer.Telephone2Type, Crm.Common.Constants.Attributes.Customer.Telephone2, Crm.Common.Constants.Attributes.Customer.Telephone3Type,
                Crm.Common.Constants.Attributes.Customer.Telephone3, Crm.Common.Constants.Attributes.Customer.EmailAddress1Type,
                Crm.Common.Constants.Attributes.Customer.EmailAddress1, Crm.Common.Constants.Attributes.Customer.EmailAddress2Type, Crm.Common.Constants.Attributes.Customer.EmailAddress2, Crm.Common.Constants.Attributes.Customer.EmailAddress3Type,
                Crm.Common.Constants.Attributes.Customer.EmailAddress3);
            query.LinkEntities[0].EntityAlias = AliasName.ContactQueryAliasName;
            query.Criteria.Conditions.Add(new ConditionExpression(Opportunity.OpportunityId, ConditionOperator.Equal,
                opportunityId));
            var req = new RetrieveMultipleRequest
            {
                Query = query
            };
                var response =
                    (RetrieveMultipleResponse)
                    client.ExecuteCrmOrganizationRequest(req,
                        $"Requesting {query.EntityName}");
                var opportunity = response?.EntityCollection?.Entities?.FirstOrDefault();
                if (opportunity != null)
                    logger.LogInformation("Opportunity retrieved");
                return opportunity;
        }

        public static Entity GetSsoDetails(Guid userId, ILogger logger, CrmServiceClient client)
        {
            var query = new QueryByAttribute(EntityName.ExternalLogin)
            {
                ColumnSet =
                    new ColumnSet(ExternalLogin.AbtaNumber, ExternalLogin.BranchCode, ExternalLogin.EmployeeId,
                        ExternalLogin.ExternalLoginId, ExternalLogin.Initials)
            };
            query.AddAttributeValue(ExternalLogin.OwnerId, userId);
            var login = ExecuteQuery(query, client)?.EntityCollection?.Entities?.FirstOrDefault();
            if (login == null)
                return null;
            var loginInfo =
                $"Abtanumber is {login.GetAttributeValue<string>(ExternalLogin.AbtaNumber)}, BranchCode is {login.GetAttributeValue<string>(ExternalLogin.BranchCode)}, Employee Id is {login.GetAttributeValue<string>(ExternalLogin.EmployeeId)}, Initials are {login.GetAttributeValue<string>(ExternalLogin.Initials)}";

            logger.LogInformation($"Requesting SSO Details result: {loginInfo}");
            return login;
        }

        public static string GetPrivateInfo(ILogger logger, CrmServiceClient client)
        {
            var query = new QueryByAttribute(EntityName.SecurityConfiguration)
            {
                ColumnSet = new ColumnSet(SecureConfiguration.Value)
            };
            query.AddAttributeValue(SecureConfiguration.Name, Tc.Crm.Common.Constants.EntityRecords.Configuration.OwrJwtPrivateKeyConfigName);
            var config = ExecuteQuery(query, client).EntityCollection?.Entities?.FirstOrDefault();
            var privateKey = config?.GetAttributeValue<string>(SecureConfiguration.Value);
            if (privateKey != null)
                logger.LogInformation(
                    $"Retrieved {Tc.Crm.Common.Constants.EntityRecords.Configuration.OwrJwtPrivateKeyConfigName} result {Tc.Crm.Common.Constants.EntityRecords.Configuration.OwrJwtPrivateKeyConfigName} is not null");
            return privateKey;
        }

        public static string GetConfig(string name, ILogger logger, CrmServiceClient client)
        {
            var query = new QueryByAttribute(EntityName.Configuration)
            {
                ColumnSet = new ColumnSet(Crm.Common.Constants.Attributes.Configuration.Value)
            };
            query.AddAttributeValue(Crm.Common.Constants.Attributes.Configuration.Name, name);
            var config = ExecuteQuery(query, client).EntityCollection?.Entities?.FirstOrDefault();

            var value = config?.GetAttributeValue<string>(Crm.Common.Constants.Attributes.Configuration.Value);
            logger.LogInformation($"Retrieved {name} result: {value}");
            return value;
        }

        public static DataCollection<Entity> GetTravelPlannerRooms(string opportunityId, ILogger logger, CrmServiceClient client)
        {
            if (string.IsNullOrEmpty(opportunityId))
                return null;
            var query = new QueryByAttribute(EntityName.Room)
            {
                ColumnSet =
                    new ColumnSet(Room.NoOfAdults, Room.NumberOfChildren, Room.Child1, Room.Child2, Room.Child3,
                        Room.Child4, Room.Child5, Room.Child6, Room.Name)
            };
            query.AddAttributeValue(Room.OpportunityId, opportunityId);
            var records = ExecuteQuery(query, client)?.EntityCollection?.Entities;
            return records;
        }

        public static string GetIso2Code(CrmServiceClient client, Guid? countryId)
        {
            if (countryId != null)
            {
                var query = new QueryByAttribute(EntityName.Country)
                {
                    ColumnSet =
                        new ColumnSet(Country.Iso2Code)
                };
                query.AddAttributeValue(Country.CountryId, countryId);
                var country = ExecuteQuery(query, client)?.EntityCollection?.Entities?.FirstOrDefault();
                if (country != null && country.Contains(Country.Iso2Code))
                {
                    return country.GetAttributeValue<string>(Country.Iso2Code);
                }
            }
            return "";
        }

        public static string GetAirportIata(CrmServiceClient client, Guid? airportId)
        {
            if (airportId != null)
            {
                var query = new QueryByAttribute(EntityName.Gateway)
                {
                    ColumnSet =
                        new ColumnSet(Gateway.Iata)
                };
                query.AddAttributeValue(Gateway.GatewayId, airportId);
                var airport = ExecuteQuery(query, client)?.EntityCollection?.Entities?.FirstOrDefault();
                if (airport != null && airport.Contains(Gateway.Iata))
                {
                    return airport.GetAttributeValue<string>(Gateway.Iata);
                }
            }
            return "";
        }

        public static string GetRegionCode(CrmServiceClient client, Guid? regionId)
        {
            if (regionId != null)
            {
                var query = new QueryByAttribute(EntityName.Region)
                {
                    ColumnSet = new ColumnSet(Region.RegionCode)
                };
                query.AddAttributeValue(Region.RegionId, regionId);
                var region = ExecuteQuery(query, client)?.EntityCollection?.Entities?.FirstOrDefault();
                if (region != null && region.Contains(Region.RegionCode))
                {
                    return region.GetAttributeValue<string>(Region.RegionCode);
                }
            }
            return "";
        }

        public static string GetHotelCode(CrmServiceClient client, Guid? hotelId)
        {
            if (hotelId != null)
            {
                var query = new QueryByAttribute(EntityName.Hotel)
                {
                    ColumnSet = new ColumnSet(Hotel.MasterHotelId)
                };
                query.AddAttributeValue(Hotel.HotelId, hotelId);
                var hotel = ExecuteQuery(query, client)?.EntityCollection?.Entities?.FirstOrDefault();
                if (hotel != null && hotel.Contains(Hotel.MasterHotelId))
                {
                    return hotel.GetAttributeValue<string>(Hotel.MasterHotelId);
                }
            }
            return "";
        }

        private static RetrieveMultipleResponse ExecuteQuery(QueryByAttribute query, CrmServiceClient client)
        {
            var req = new RetrieveMultipleRequest
            {
                Query = query
            };
                var response =
                    (RetrieveMultipleResponse)
                    client.ExecuteCrmOrganizationRequest(req,
                        $"Requesting {query.EntityName}");
                return response;
        }
    }
}
