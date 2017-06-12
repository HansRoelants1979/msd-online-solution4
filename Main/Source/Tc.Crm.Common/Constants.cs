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
    }

    namespace Attributes
    {
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
        }

        public static class Contact
        {
            public const string ContactId = "contactid";
            public const string Owner = "ownerid";
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
        }

        public static class SourceMarket
        {
            public const string BusinessUnitId = "tc_sourcemarketbusinessunitid";
        }
    }

    
}


