using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.CustomWorkflowSteps
{
    public static class EntityName
    {
        public const string Account = "account";
        public const string Contact = "contact";
        public const string Booking = "tc_booking";
        public const string BookingAccommodation = "tc_bookingaccommodation";
        public const string BookingTransport = "tc_bookingtransport";
        public const string Brand = "tc_brand";
        public const string Case = "incident";
        public const string CaseCategory = "tc_casecategory";
        public const string CaseType = "tc_casetype";
        public const string Country = "tc_country";
        public const string Gateway = "tc_gateway";
        public const string CustomerBookingRole = "tc_customerbookingrole";
        public const string Hotel = "tc_hotel";
        public const string Team = "team";
        public const string HotelPromises = "tc_hotelpromises";
        public const string Location = "tc_location";
        public const string Region = "tc_region";
        public const string Remark = "tc_remark";
        public const string LocationOffice = "tc_locationoffice";
        public const string SocialProfile = "socialprofile";
        public const string TourOperator = "tc_touroperator";
        public const string User = "systemuser";
    }

    public static class Attributes
    {
        public class Contact
        {
            public const string LastName = "lastname";
            public const string FirstName = "firstname";
            public const string Gender = "tc_gender";
            public const string Language = "tc_language";

        }

        public class Account
        {
            public const string Name = "name";
        }
        public class Booking
        {
            public const string AgentFullName = "tc_agentfullname";
            public const string AgentPersonalNumber = "tc_agentpersonalnumber";
            public const string AgentShortName = "tc_agentshortname";
            public const string AgentTeam = "tc_agentteam";
            public const string BookerEmail = "tc_bookeremail";
            public const string BookerEmergencyPhone = "tc_bookeremergencyphone";
            public const string BookerPhone1 = "tc_bookerphone1";
            public const string BookerPhone2 = "tc_bookerphone2";
            public const string BookingDate = "tc_bookingdate";
            public const string BookingId = "tc_bookingid";//added ID
            public const string Brand = "tc_brand ";//brand
            public const string BrochureCode = "tc_brochurecode";
            public const string DepartureDate = "tc_departuredate";
            public const string DestinationGateway = "tc_destinationgatewayid";
            public const string Destination = "tc_destinationid";
            public const string Duration = "tc_duration";
            public const string ExtraService = "tc_extraservice";
            public const string ExtraServiceRemarks = "tc_extraserviceremarks";
            public const string HasSourceMarketComplaint = "tc_hassourcemarketcomplaint";
            public const string IsLateBooking = "tc_islatebooking";
            public const string SourceSystemBookingId = "tc_name";
            public const string NumberofAdults = "tc_numberofadults";
            public const string NumberofChildren = "tc_numberofchildren";
            public const string NumberofInfants = "tc_numberofinfants";
            public const string NumberofParticipants = "tc_numberofparticipants";
            public const string OnTourUpdatedDate = "tc_ontourupdateddate";
            public const string OnTourVersion = "tc_ontourversion";
            public const string ParticipantRemarks = "tc_participantremarks";
            public const string Participants = "tc_participants";
            public const string ReturnDate = "tc_returndate";
            public const string ShopChannel = "tc_shopchannel ";
            public const string ShopCode = "tc_shopcode";
            public const string ShopCompany = "tc_shopcompany";
            public const string ShopName = "tc_shopname";
            public const string SourceMarket = "tc_sourcemarketid";
            public const string TourOperator = "tc_touroperatorid";
            public const string TourOperatorUpdatedDate = "tc_touroperatorupdateddate";
            public const string TourOperatorVersion = " tc_touroperatorversion ";
            public const string Transfer = "tc_transfer";
            public const string TransferRemarks = "tc_transferremarks";
            public const string TravelAmount = "tc_travelamount";
        }
        public class BookingAccommodation
        {
            public const string BoardType = "tc_boardtype";
            public const string BookingAccommodationid = "tc_bookingaccommodationid";
            public const string BookingId = "tc_bookingid";//added ID
            public const string EndDateandTime = "tc_enddateandtime";
            public const string ExternalServiceCode = "tc_externalservicecode";
            public const string ExternalTransfer = "tc_externaltransfer";
            public const string HasSharedRoom = "tc_hassharedroom";
            public const string Hotel = "tc_hotelid";
            public const string IsExternalService = "tc_isexternalservice";
            public const string Name = "tc_name";
            public const string NeedTourGuideAssignment = "tc_needtourguideassignment";
            public const string NotificationRequired = "tc_notificationrequired";
            public const string NumberofParticipants = "tc_numberofparticipants";
            public const string NumberofRooms = "tc_numberofrooms";
            public const string Order = "tc_order";
            public const string Participants = "tc_participants";
            public const string RoomType = "tc_roomtype";
            public const string ServiceType = "tc_servicetype";
            public const string SourceMarketHotelCode = "tc_sourcemarkethotelcode";
            public const string StartDateandTime = "tc_startdateandtime ";
            public const string TransferServiceLevel = "tc_transferservicelevel";
            public const string WithTransfer = "tc_withtransfer";
        }
        public class BookingTransport
        {
            public const string ArrivalGateway = "tc_arrivalgatewayid";
            public const string Booking = "tc_bookingid"; //
            public const string BookingTransportId = "tc_bookingtransportid";
            public const string CarrierCode = "tc_carriercode";
            public const string DepartureGateway = "tc_departuregatewayid";
            public const string Description = "tc_description";
            public const string EndDateandTime = "tc_enddateandtime";
            public const string FlightIdentifier = "tc_flightidentifier";
            public const string FlightNumber = "tc_flightnumber";
            public const string Name = "tc_name";
            public const string NumberofParticipants = "tc_numberofparticipants";
            public const string Order = "tc_order";
            public const string StartDateandTime = "tc_startdateandtime";
            public const string TransferType = "tc_transfertype";
            public const string TransportCode = "tc_transportcode";
        }
        public class Brand
        {
            public const string BrandCode = "tc_brandcode";
            public const string BrandId = "tc_brandid";
            public const string SourceMarket = "tc_countryid";
            public const string Name = "tc_name";
        }
        public class Country
        {
            public const string CountryId = "tc_countryid";//added ID
            //public const string Country = "tc_iso_code";
        }
        public class CustomerBookingRole
        {
            public const string Booking = "tc_bookingid";
            public const string Customer = "tc_customer";
            public const string Role = "tc_customerbookingrole";
            public const string CustomerBookingRoleId = "tc_customerbookingroleid";
            public const string Name = "tc_name";
        }
        public class Gateway
        {
            public const string Country = "tc_countryid";
            public const string GatewayName = "tc_gateway";//added Name
            public const string GatewayId = "tc_gatewayid";
            public const string IATA = "tc_iata";
        }
        public class Hotel
        {
            public const string AdditionalInformation = "tc_address1_additionalinformation";
            public const string Country = "tc_address1_countryid";
            public const string County = "tc_address1_county";
            public const string FlatorUnitNumber = "tc_address1_flatorunitnumber";
            public const string HouseNumberorBuilding = "tc_address1_housenumberorbuilding";
            public const string PostalCode = "tc_address1_postalcode";
            public const string Street = "tc_address1_street";
            public const string Town = "tc_address1_town ";
            public const string Category = "tc_category";
            public const string CCEmailAddress = "tc_ccemailaddress";
            public const string FaxNumber = "tc_faxnumber";
            public const string HotelId = "tc_hotelid";
            public const string Location = "tc_locationid";
            public const string MasterHotelID = "tc_masterhotelid";
            public const string Name = "tc_name";
            public const string PrimaryEmailAddress = "tc_primaryemailaddress";
            public const string SourceMarketHotelID = "tc_sourcemarkethotelid";
            public const string ResortTeam = "tc_teamid";
            public const string TelephoneNumber = "tc_telephonenumber";
        }
        public class HotelPromises
        {
            public const string Brand = "tc_brandid";
            public const string Hotel = "tc_hotelid";
            public const string HotelPromisesId = "tc_hotelpromisesid";
            public const string Name = "tc_name";
            public const string PromiseType = "tc_promisetype";
        }
        public class Location
        {
            public const string LocationCode = "tc_locationcode";
            public const string LocationId = "tc_locationid";
            public const string LocationOffice = "tc_locationofficeid";
            public const string Name = "tc_name";
            public const string PrimaryLocation = "tc_primarylocationid";
            public const string Region = "tc_regionid";
            public const string Type = "tc_type";
        }
        public class LocationOffice
        {
            public const string CityTownLocality = "tc_citytownlocality";
            public const string HouseBuildingNumber = "tc_housebuildingnumber";
            public const string Location = "tc_locationid";
            public const string LocationOfficeId = "tc_locationofficeid";
            public const string Name = "tc_name";
            public const string PostalCode = "tc_postalcode";
            public const string PrimaryTelephoneNumber = "tc_primarytelephonenumber";
            public const string SecondaryTelephoneNumber = "tc_secondarytelephonenumber";
            public const string StreetLine1 = "tc_streetline1";
            public const string StreetLine2 = "tc_streetline2";
            public const string StreetLine3 = "tc_streetline3";
        }
        public class Region
        {
            public const string Country = "tc_countryid";
            public const string Name = "tc_name";
            public const string RegionCode = "tc_regioncode";
            public const string RegionId = "tc_regionid";
        }
        public class Remark
        {
            public const string BookingAccommodationId = "tc_bookingaccommodationid";
            public const string BookingTransport = "tc_bookingtransportid";
            public const string Name = "tc_name";
            public const string RemarkName = "tc_remark";//added Name
            public const string RemarkId = "tc_remarkid";
            public const string SourceMarketID = "tc_sourcemarketid";
            public const string SourceSystemID = "tc_sourcesystemid";
            public const string Type = "tc_type";
        }
        public class TourOperator
        {
            public const string SourceMarket = "tc_countryid";
            public const string Name = "tc_name";
            public const string TourOperatorCode = "tc_touroperatorcode";
            public const string TourOperatorId = "tc_touroperatorid";
        }


    }
}
