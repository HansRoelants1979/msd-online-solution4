namespace Tc.Crm.Common.Constants
{
    public static class ResponseAttribute
    {
        public const string WebRioResponseCookie_JSessionId = "jsessionid";
    }
    public static class EntityName
    {
        public const string Booking = "tc_booking";
        public const string BookingAccommodation = "tc_bookingaccommodation";
        public const string CustomerBookingRole = "tc_customerbookingrole";
        public const string Hotel = "tc_hotel";
        public const string Team = "team";
        public const string User = "systemuser";
        public const string Account = "account";
        public const string Contact = "contact";
        public const string Case = "incident";
        public const string BusinessUnit = "businessunit";
        public const string Country = "tc_country";
        public const string Gateway = "tc_gateway";
        public const string EntityCache = "tc_entitycache";
        public const string EntityCacheMessage = "tc_entitycachemessage";
        public const string Configuration = "tc_configuration";
        public const string SecurityConfiguration = "tc_secureconfiguration";
        public const string ExternalLogin = "tc_externallogin";
        public const string Store = "tc_store";
        public const string Opportunity = "opportunity";
        public const string Room = "tc_room";
        public const string Region = "tc_region";
    }
    public static class AliasName
    {
        public const string AccommodationAliasName = "accommodation.";
        public const string HotelAliasName = "hotel.";
        public const string RoleAliasName = "role.";
        public const string AccountAliasName = "account.";
        public const string ContactAliasName = "contact.";
        public const string SourceMarketAliasName = "sourcemarket.";     
        public const string TeamAliasName = "team.";
        public const string UserAliasName = "systemuser.";
        public const string ContactCaseAliasName = "contactincident.";
        public const string AccountCaseAliasName = "accountincident.";
        public const string BusinessUnitAliasName = "businessunit.";
        public const string ContactQueryAliasName = "contact";
    }
   
    namespace EntityRecords
    {
        public static class BusinessUnit
        {
            public const string GB = "GB";
        }

        public static class Configuration
        {
            public const string OutboundSynchronisationJwtPrivateKeyConfigName = "Tc.OutboundSynchronisation.JwtPrivateKey";
            public const string OutboundSynchronisationSsoTokenExpired = "Tc.OutboundSynchronisation.SsoTokenExpiredSeconds";
            public const string OutboundSynchronisationSsoTokenNotBefore = "Tc.OutboundSynchronisation.SsoTokenNotBeforeTimeSeconds";

            public const string OwrUrlConfigName = "Tc.Owr.SsoServiceUrl";
            public const string OwrDiagnosticSource = "Tc.Usd.SessionCustomActions";
            public const string OwrJwtPrivateKeyConfigName = "Tc.Owr.JwtPrivateKey";
            public const string OwrSsoTokenExpired = "Tc.Owr.SsoTokenExpiredSeconds";
            public const string OwrSsoTokenNotBefore = "Tc.Owr.SsoTokenNotBeforeTimeSeconds";
            public const string OwrAudOneWebRetail = "onewebretail";
            public const string OwrOpportunityIdParamName = "opportunityId";

            public const string WebRioJwtPrivateKeyConfigName = "Tc.WebRio.JwtPrivateKey";
            public const string WebRioAudWebRio = "webrio";
            public const string WebRioAdminApi = "Tc.Wr.Api.Admin";
            public const string WebRioOpenConsultationApi = "Tc.Wr.Api.OpenConsultation";
            public const string WebRioNewConsultationApi = "Tc.Wr.Api.NewConsultation";
            public const string WebRioServiceUrl = "Tc.Wr.SSOServiceUrl";
            public const string WebRioExpirySecondsFromNow = "Tc.Wr.SsoToken.ExpiredSecondsFromNow";
            public const string WebRioNotBeforeTimeSecondsFromNow = "Tc.Wr.SsoToken.NotBeforeTimeSecondsFromNow";
            public const string SsoCompleteEvent = "Tc.Event.OnSsoComplete";

            public const string OpenOwr = "Tc.Usd.SingleSignOnController.Custom.OpenOwr";

            public const string SsoLogin = "Tc.Event.SsoLogin";
            public const string SsoCancel = "Tc.Event.SsoCancel";
        }

    }

    namespace UsdConstants
    {
        public static class HttpCode
        {
            public const string InternalError = "501";
            public const string Ok = "200";
            public const string NotFound = "404";
        }
        public static class UsdParameter
        {
            public const string WebRioJSessionId = "JSessionId";
            public const string WebRioRequestType = "Type";
            public const string ApplicationType = "applicationType";
            public const string Application_WebRio = "WebRio";
            public const string ApplicationOwr = "OWR";
            public const string ResponseCode = "responseCode";
            public const string ResponseMessage = "responseMessage";
            public const string Application = "AppName";
            public const string Url = "Url";
            public const string WebRioConsultationNo = "ConsultationNo";
            public const string CustomerId = "CustomerId";
            public const string EntityId = "EntityId";
        }

        public static class UsdHostedControl
        {
            public const string WebRioAdmin = "Tc.Wr.Admin";
            public const string WebRioConsultation = "Tc.Wr.Consultation";
            public const string CrmGlobalManager = "CRM Global Manager";
        }

        public static class UsdActionCall
        {
            
        }

        public static class UsdAction
        {
            public const string OpenWebRioGlobal = "Tc.Usd.Global.SingleSignOnController.OpenWR";
            public const string CloseApp = "Tc.Usd.Global.SingleSignOnController.CloseApp";
            public const string Close = "Close";
            public const string OpenWebRio = "Tc.Usd.SingleSignOnController.OpenWebRio";
            public const string DisplayMessage = "DisplayMessage";
        }

        public static class UsdEvent
        {
            public const string GlobalSsoCompleteEvent = "Tc.Event.OnGlobalSsoComplete";
            public const string SsoCompleteEvent = "Tc.Event.OnSsoComplete";
        }
    }

    namespace Attributes
    {
        public static class ExternalLogin
        {
            public const string AbtaNumber = "tc_abtanumber";
            public const string BranchCode = "tc_branchcode";
            public const string BudgetCentreId = "tc_budgetcentreid";
            public const string EmployeeId = "tc_employeeid";
            public const string Initials = "tc_initials";
            public const string Name = "tc_name";
            public const string Id = "Id";
            public const string ExternalLoginId = "tc_externalloginid";
            public const string OwnerId = "ownerid";            
        }

        public static class CommonAttribute
        {
            public const string Owner = "ownerid";
        }
        public static class SecureConfiguration
        {
            public const string Name = "tc_name";
            public const string Value = "tc_value";
        }
       
        
        public static class BusinessUnit
        {
            public const string BusinessUnitId = "businessunitid";
            public const string Name = "name";
        }

        public static class Gateway
        {
            public const string GatewayId = "tc_gatewayid";
            public const string Iata = "tc_iata";
            public const string Name = "tc_gateway";
        }

        public static class Booking
        {
            public const string BookingId = "tc_bookingid";
            public const string Owner = "ownerid";
            public const string Name = "tc_name";
            public const string DepartureDate = "tc_departuredate";
            public const string DestinationGatewayId = "tc_destinationgatewayid";
            public const string ReturnDate = "tc_returndate";
            public const string OwningTeam = "owningteam";
            public const string SourceMarketId = "tc_sourcemarketid";
        }

        public static class Team
        {
            public const string TeamId = "teamid";
            public const string Name = "name";
            public const string IsHotelTeam = "tc_hotelteam";
            public const string IsDefaultTeam = "isdefault";
            public const string BusinessUnitId = "businessunitid";
            public const string ParentTeamId = "tc_hotelteamid";
        }

        public static class Country
        {
            public const string CountryId = "tc_countryid";
            public const string BusinessUnitId = "tc_sourcemarketbusinessunitid";
            public const string Iso2Code = "tc_iso2code";
        }

        public static class BookingAccommodation
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Accommodationid")]
            public const string BookingAccommodationid = "tc_bookingaccommodationid";
            public const string BookingId = "tc_bookingid";
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dateand")]
            public const string EndDateandTime = "tc_enddateandtime";
            public const string HotelId = "tc_hotelid";
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dateand")]
            public const string StartDateandTime = "tc_startdateandtime";
            public const string Owner = "ownerid";

        }

        public static class CustomerBookingRole
        {
            public const string BookingId = "tc_bookingid";
            public const string Customer = "tc_customer";
            public const string Role = "tc_bookingrole";
            public const string CustomerBookingRoleId = "tc_customerbookingroleid";
            public const string Name = "tc_name";
        }

        public static class Hotel
        {

            public const string HotelId = "tc_hotelid";
            public const string LocationId = "tc_locationid";
            public const string MasterHotelId = "tc_masterhotelid";
            public const string Name = "tc_name";
            public const string SourceMarketHotelId = "tc_sourcemarkethotelid";
            public const string ResortTeam = "tc_teamid";
            public const string Owner = "ownerid";

        }

        public static class Customer
        {
            public const string TeamId = "teamid";
            public const string Name = "name";
            public const string BusinessUnitId = "businessunitid";
            public const string ParentTeamId = "tc_hotelteamid";
            public const string Owner = "ownerid";
            public const string Salutation = "tc_salutation";
            public const string Language = "tc_language";
            public const string FirstName = "firstname";
            public const string LastName = "lastname";
            public const string Birthdate = "birthdate";
            public const string SourceSystemId = "tc_sourcesystemid";
            public const string MiddleName = "middlename";
            public const string Address1FlatorUnitNumber = "tc_address1_flatorunitnumber";
            public const string Address1HouseNumberoBuilding = "tc_address1_housenumberorbuilding";
            public const string Address1Street = "tc_address1_street";
            public const string Address1Town = "tc_address1_town";
            public const string Address1County = "tc_address1_county";
            public const string Address1CountryId = "tc_address1_countryid";
            public const string Address1PostalCode = "tc_address1_postalcode";
            public const string Address2FlatorUnitNumber = "tc_address2_flatorunitnumber";
            public const string Address2HouseNumberoBuilding = "tc_address2_housenumberorbuilding";
            public const string Address2Street = "tc_address2_street";
            public const string Address2Town = "tc_address2_town";
            public const string Address2County = "tc_address2_county";
            public const string Address2CountryId = "tc_address2_countryid";
            public const string Address2PostalCode = "tc_address2_postalcode";
            public const string Telephone1 = "telephone1";
            public const string Telephone1Type = "tc_telephone1type";
            public const string Telephone2 = "telephone2";
            public const string Telephone2Type = "tc_telephone2type";
            public const string Telephone3 = "telephone3";
            public const string Telephone3Type = "tc_telephone3type";
            public const string EmailAddress1 = "emailaddress1";
            public const string EmailAddress1Type = "tc_emailaddress1type";
            public const string EmailAddress2 = "emailaddress2";
            public const string EmailAddress2Type = "tc_emailaddress2type";
            public const string EmailAddress3 = "emailaddress3";
            public const string EmailAddress3Type = "tc_emailaddress3type";
        }

        public static class Contact
        {
            public const string ContactId = "contactid";
            public const string Owner = "ownerid";
            public const string SourceSystemId = "tc_sourcesystemid";
        }

        public static class Account
        {
            public const string AccountId = "accountid";
            public const string Owner = "ownerid";
        }

        public static class Case
        {
            public const string CaseId = "incidentid";
            public const string CustomerId = "customerid";
            public const string Owner = "ownerid";
            public const string StatusReason = "statuscode";
            public const string State = "statecode";
        }

        public static class User
        {
            public const string UserId = "systemuserid";
            public const string PayrollNumber = "tc_payrollnumbertext";
            public const string FullName = "fullname";
            public const string AllBudgetCentreAccess = "tc_allbudgetcentreaccess";
            public const string PrimaryStoreId = "tc_primarystoreid";
        }

        public static class Store
        {
            public const string StoreId = "tc_storeid";
            public const string Name = "tc_name";
            public const string Abta = "tc_abta";
            public const string BudgetCentre = "tc_budgetcentre";
            public const string ClusterId = "tc_clusterid";
            public const string UkRegionId = "tc_ukregionid";
        }

        public static class SourceMarket
        {
            public const string BusinessUnitId = "tc_sourcemarketbusinessunitid";
        }

        public static class EntityCache
        {
            public const string Name = "tc_name";
            public const string Type = "tc_type";
            public const string Data = "tc_data";
            public const string Operation = "tc_operation";
            public const string SourceMarket = "tc_sourcemarket";
            public const string RecordId = "tc_recordid";
            public const string StatusReason = "statuscode";
            public const string State = "statecode";
            public const string EntityCacheId = "tc_entitycacheid";
            public const string CreatedOn = "createdon";
			public const string WasLastOperationSuccessful = "tc_waslastoperationsuccessful";
			public const string EligibleRetryTime = "tc_eligibleretrytime";
		}

        public static class EntityCacheMessage
        {
            public const string Name = "tc_name";
            public const string EntityCacheId = "tc_entitycacheid";
            public const string OutcomeId = "tc_outcomeid";
            public const string Notes = "tc_notes";
            public const string StatusReason = "statuscode";
            public const string State = "statecode";
            public const string EntityCacheMessageId = "tc_entitycachemessageid";
        }

        public static class Configuration
        {
            public const string Name = "tc_name";
            public const string Value = "tc_value";
        }

        public static class SecurityConfiguration
        {
            public const string Name = "tc_name";
            public const string Value = "tc_value";
        }

        public static class Opportunity
        {
            public const string Name = "name";
            public const string OpportunityId = "opportunityid";
            public const string CustomerId = "customerid";
            public const string Initials = "tc_initials";
            public const string EarliestDepartureDate = "tc_earliestdeparturedate";
            public const string LatestDepartureDate = "tc_latestdeparturedate";
            public const string Duration = "tc_duration";
            public const string DeparturePoint1 = "tc_departurepoint1";
            public const string DeparturePoint2 = "tc_departurepoint2";
            public const string DeparturePoint3 = "tc_departurepoint3";
            public const string DestinationCountry1 = "tc_destinationcountry1";
            public const string Region1 = "tc_region1";
            public const string DestinationAirport1 = "tc_destinationairport1";
            public const string Hotel1 = "tc_hotel1";
            public const string DestinationCountry2 = "tc_destinationcountry2";
            public const string Region2 = "tc_region2";
            public const string DestinationAirport2 = "tc_destinationairport2";
            public const string Hotel2 = "tc_hotel2";
            public const string DestinationCountry3 = "tc_destinationcountry3";
            public const string Region3 = "tc_region3";
            public const string Hotel3 = "tc_hotel3";
            public const string DestinationAirport3 = "tc_destinationairport3";
            public const string Exclude1 = "tc_exclude1";
            public const string Exclude2 = "tc_exclude2";
            public const string Exclude3 = "tc_exclude3";
            public const string HowDoYouWantToSearch = "tc_howdoyouwanttosearch";
        }
        public static class Room
        {
            public const string OpportunityId = "tc_relatedtravelplannerid";
            public const string NoOfAdults = "tc_noofadults";
            public const string NumberOfChildren = "tc_numberofchildren";
            public const string Child1 = "tc_child1";
            public const string Child2 = "tc_child2";
            public const string Child3 = "tc_child3";
            public const string Child4 = "tc_child4";
            public const string Child5 = "tc_child5";
            public const string Child6 = "tc_child6";
            public const string Name = "tc_name";
        }

        public static class Region
        {
            public const string RegionCode = "tc_regioncode";
            public const string RegionId = "tc_regionid";
        }
    }
}