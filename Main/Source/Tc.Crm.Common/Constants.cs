namespace Tc.Crm.Common.Constants
{
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
            public const string OwrAudWebRio = "webrio";
            public const string OwrOpportunityIdParamName = "opportunityId";

            public const string SsoCompleteEvent = "Tc.Event.OnSsoComplete";

            public const string OpenOwr = "Tc.Usd.SingleSignOnController.Custom.OpenOwr";
            public const string SsoLogin = "Tc.Event.SsoLogin";
            public const string SsoCancel = "Tc.Event.SsoCancel";
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
            public const string OwnerId = "ownerid";            
        }

        public static class CommonAttribute
        {
            public const string Owner = "ownerid";
        }

        public static class BusinessUnit
        {
            public const string BusinessUnitId = "businessunitid";
            public const string Name = "name";
        }

        public static class Gateway
        {
            public const string GatewayId = "tc_gatewayId";
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
    }
}