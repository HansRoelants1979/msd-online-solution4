using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.CustomWorkflowSteps
{
    public static class General
    {
        public const string AccountType = "B";

        public const string ContactType = "P";

        public const string Seperator = ",";

        public const string NextLine = "\r\n";

        public const string Booked = "B";

        public const string Cancelled = "C";

        public const string Deceased = "D";

        public const string Blacklisted = "B";

        public const string Concatenator = " - ";

        public const string Space = " ";
    }
    public static class EntityName
    {
        public const string Account = "account";
        public const string Annotation = "annotation";
        public const string Contact = "contact";
        public const string Booking = "tc_booking";
        public const string BookingAccommodation = "tc_bookingaccommodation";
        public const string BookingTransport = "tc_bookingtransport";
        public const string Brand = "tc_brand";
        public const string Case = "incident";
        public const string CaseCategory = "tc_casecategory";
        public const string CaseType = "tc_casetype";
        public const string Country = "tc_country";
        public const string Currency = "transactioncurrency";
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
        public const string BookingTransfer = "tc_bookingtransfer";
        public const string BookingExtraService = "tc_bookingextraservice";
        public const string SurveyResponse = "tc_surveyresponse";
        public const string SurveyResponseFeedback = "tc_surveyresponsefeedback";
        public const string ActivityParty = "activityparty";

    }


    public static class Attributes
    {
        public class Contact
        {
            public const string LastName = "lastname";
            public const string FirstName = "firstname";
            public const string Gender = "tc_gender";
            public const string Language = "tc_language";
            public const string SourceMarketId = "tc_sourcemarketid";
            public const string Salutation = "tc_salutation";
            public const string Telephone1Type = "tc_telephone1type";
            public const string Telephone2Type = "tc_telephone2type";
            public const string Telephone3Type = "tc_telephone3type";
            public const string AcademicTitle = "tc_academictitle";
            public const string Address1_AdditionalInformation = "tc_address1_additionalinformation";
            public const string Address1_County = "tc_address1_county";
            public const string Address1_FlatOrUnitNumber = "tc_address1_flatorunitnumber";
            public const string Address1_HouseNumberOrBuilding = "tc_address1_housenumberorbuilding";
            public const string Address1_Town = "tc_address1_town";
            public const string Address1_CountryId = "tc_address1_countryid";
            public const string Address1_PostalCode = "tc_address1_postalcode";
            public const string Address2_AdditionalInformation = "tc_address2_additionalinformation";
            public const string Address2_CountryId = "tc_address2_countryid";
            public const string Address2_County = "tc_address2_county";
            public const string Address2_FlatOrUnitNumber = "tc_address2_flatorunitnumber";
            public const string Address2_HouseNumberorBuilding = "tc_address2_housenumberorbuilding";
            public const string Address2_PostalCode = "tc_address2_postalcode";
            public const string Address2_Town = "tc_address2_town";
            public const string StateCode = "statecode";
            public const string StatusCode = "statuscode";
            public const string DateofDeath = "tc_dateofdeath";
            public const string EmailAddress1Type = "tc_emailaddress1type";
            public const string EmailAddress2Type = "tc_emailaddress2type";
            public const string EmailAddress3Type = "tc_emailaddress3type";
            public const string Segment = "tc_segment";
            public const string Telephone1 = "telephone1";
            public const string Telephone2 = "telephone2";
            public const string Telephone3 = "telephone3";
            public const string EMailAddress1 = "emailaddress1";
            public const string EMailAddress2 = "emailaddress2";
            public const string EMailAddress3 = "emailaddress3";
            public const string MiddleName = "middlename";
            public const string BirthDate = "birthdate";
            public const string SourceSystemID = "tc_sourcesystemid";
            public const string FullName = "fullname";
            public const string ContactId = "contactid";


        }

        public class Account
        {
            public const string Name = "name";
            public const string SourceMarketId = "tc_sourcemarketid";
            public const string Address1_AdditionalInformation = "tc_address1_additionalinformation";
            public const string Address1_FlatOrUnitNumber = "tc_address1_flatorunitnumber";
            public const string Address1_HouseNumberOrBuilding = "tc_address1_housenumberorbuilding";
            public const string Address1_Town = "tc_address1_town";
            public const string Address1_CountryId = "tc_address1_countryid";
            public const string Address1_County = "tc_address1_county";
            public const string Address1_PostalCode = "tc_address1_postalcode";
            public const string Telephone1_Type = "tc_telephone1_type";
            public const string Telephone2_Type = "tc_telephone2_type";
            public const string Telephone3_Type = "tc_telephone3_type";
            public const string EmailAddress1_Type = "tc_emailaddress1_type";
            public const string EmailAddress2_Type = "tc_emailaddress2_type";
            public const string EmailAddress3_Type = "tc_emailaddress3_type";
            public const string Telephone1 = "telephone1";
            public const string Telephone2 = "telephone2";
            public const string Telephone3 = "telephone3";
            public const string EMailAddress1 = "emailaddress1";
            public const string EMailAddress2 = "emailaddress2";
            public const string EMailAddress3 = "emailaddress3";
            public const string SourceSystemID = "tc_sourcesystemid";
        }

        public class Annotation
        {
            public const string AnnotationId = "annotationid";
            public const string NoteText = "notetext";
            public const string Subject = "subject";
            public const string Regarding = "objectid";

        }
        public class Booking
        {
            public static string Remarks = "tc_remark";
            public const string AgentFullName = "tc_agentfullname";
            public const string AgentPersonalNumber = "tc_agentpersonalnumber";
            public const string AgentShortName = "tc_agentshortname";
            public const string AgentTeam = "tc_agentteam";
            public const string BookerEmail = "tc_bookeremail";
            public const string BookerEmergencyPhone = "tc_bookeremergencyphone";
            public const string BookerPhone1 = "tc_bookerphone1";
            public const string BookerPhone2 = "tc_bookerphone2";
            public const string BookingDate = "tc_bookingdate";
            public const string BookingId = "tc_bookingid";
            public const string BrandId = "tc_brandid";
            public const string BrochureCode = "tc_brochurecode";
            public const string DepartureDate = "tc_departuredate";
            public const string DestinationGatewayId = "tc_destinationgatewayid";
            public const string DestinationId = "tc_destinationid";
            public const string Duration = "tc_duration";
            public const string ExtraService = "tc_extraservice";
            public const string ExtraServiceRemarks = "tc_extraserviceremarks";
            public const string HasSourceMarketComplaint = "tc_hassourcemarketcomplaint";
            public const string IsLateBooking = "tc_islatebooking";
            public const string Name = "tc_name";
            public const string NumberofAdults = "tc_numberofadults";
            public const string NumberofChildren = "tc_numberofchildren";
            public const string NumberofInfants = "tc_numberofinfants";
            public const string NumberofParticipants = "tc_numberofparticipants";
            public const string OnTourUpdatedDate = "tc_ontourupdateddate";
            public const string OnTourVersion = "tc_ontourversion";
            public const string Owner = "ownerid";
            public const string ParticipantRemarks = "tc_participantremarks";
            public const string Participants = "tc_participants";
            public const string ReturnDate = "tc_returndate";
            public const string StateCode = "statecode";
            public const string StatusCode = "statuscode";
            public const string ShopChannel = "tc_shopchannel";
            public const string ShopCode = "tc_shopcode";
            public const string ShopCompany = "tc_shopcompany";
            public const string ShopName = "tc_shopname";
            public const string SourceMarketId = "tc_sourcemarketid";
            public const string TourOperatorId = "tc_touroperatorid";
            public const string TourOperatorUpdatedDate = "tc_touroperatorupdateddate";
            public const string TourOperatorVersion = "tc_touroperatorversion";
            public const string Transfer = "tc_transfer";
            public const string TransferRemarks = "tc_transferremarks";
            public const string TravelAmount = "tc_travelamount";
            public const string TransactionCurrencyId = "transactioncurrencyid";
        }
        public class BookingAccommodation
        {
            public const string BoardType = "tc_boardtype";
            public const string BookingAccommodationid = "tc_bookingaccommodationid";
            public const string BookingId = "tc_bookingid";
            public const string EndDateandTime = "tc_enddateandtime";
            public const string ExternalServiceCode = "tc_externalservicecodetype";
            public const string ExternalTransfer = "tc_externaltransfer";
            public const string HasSharedRoom = "tc_hassharedroom";
            public const string HotelId = "tc_hotelid";
            public const string IsExternalService = "tc_isexternalservice";
            public const string Name = "tc_name";
            public const string NeedTourGuideAssignment = "tc_needtourguideassignment";
            public const string NotificationRequired = "tc_notificationrequired";
            public const string NumberofParticipants = "tc_numberofparticipants";
            public const string NumberofRooms = "tc_numberofrooms";
            public const string Order = "tc_order";
            public const string Participants = "tc_participants";
            public const string RoomType = "tc_roomtype";
            public const string StatusCode = "statuscode";
            public const string ServiceType = "tc_servicetype";
            public const string SourceMarketHotelCode = "tc_sourcemarkethotelcode";
            public const string SourceMarketHotelName = "tc_sourcemarkethotelname";
            public const string StartDateandTime = "tc_startdateandtime";
            public const string TransferServiceLevel = "tc_transferserviceleveltype";
            public const string WithTransfer = "tc_withtransfer";
        }
        public class BookingTransport
        {
            public const string ArrivalGatewayId = "tc_arrivalgatewayid";
            public const string BookingId = "tc_bookingid";
            public const string BookingTransportId = "tc_bookingtransportid";
            public const string CarrierCode = "tc_carriercode";
            public const string DepartureGatewayId = "tc_departuregatewayid";
            public const string Description = "tc_description";
            public const string EndDateandTime = "tc_enddateandtime";
            public const string FlightIdentifier = "tc_flightidentifier";
            public const string FlightNumber = "tc_flightnumber";
            public const string Name = "tc_name";
            public const string NumberofParticipants = "tc_numberofparticipants";
            public const string Order = "tc_order";
            public const string Participants = "tc_participants";
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
            public const string CountryId = "tc_countryid";
            //public const string Country = "tc_iso_code";
            public const string ISO2Code = "tc_iso2code";
        }

        public class Currency
        {
            public const string Name = "currencyname";
        }

        public class CustomerBookingRole
        {
            public const string BookingId = "tc_bookingid";
            public const string Customer = "tc_customer";
            public const string Role = "tc_customerbookingrole";
            public const string CustomerBookingRoleId = "tc_customerbookingroleid";
            public const string Name = "tc_name";
        }
        public class Gateway
        {
            public const string CountryId = "tc_countryid";
            public const string GatewayName = "tc_gateway";
            public const string GatewayId = "tc_gatewayid";
            public const string IATA = "tc_iata";
        }
        public class Hotel
        {
            public const string AdditionalInformation = "tc_address1_additionalinformation";
            public const string CountryId = "tc_address1_countryid";
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
            public const string LocationId = "tc_locationid";
            public const string MasterHotelID = "tc_masterhotelid";
            public const string Name = "tc_name";
            public const string PrimaryEmailAddress = "tc_primaryemailaddress";
            public const string SourceMarketHotelID = "tc_sourcemarkethotelid";
            public const string ResortTeam = "tc_teamid";
            public const string TelephoneNumber = "tc_telephonenumber";
        }
        public class HotelPromises
        {
            public const string BrandId = "tc_brandid";
            public const string HotelId = "tc_hotelid";
            public const string HotelPromisesId = "tc_hotelpromisesid";
            public const string Name = "tc_name";
            public const string PromiseType = "tc_promisetype";
        }
        public class Location
        {
            public const string LocationCode = "tc_locationcode";
            public const string LocationId = "tc_locationid";
            public const string LocationOfficeId = "tc_locationofficeid";
            public const string Name = "tc_name";
            public const string PrimaryLocationId = "tc_primarylocationid";
            public const string RegionId = "tc_regionid";
            public const string Type = "tc_type";
        }
        public class LocationOffice
        {
            public const string CityTownLocality = "tc_citytownlocality";
            public const string HouseBuildingNumber = "tc_housebuildingnumber";
            public const string LocationId = "tc_locationid";
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
            public const string CountryId = "tc_countryid";
            public const string Name = "tc_name";
            public const string RegionCode = "tc_regioncode";
            public const string RegionId = "tc_regionid";
        }
        public class Remark
        {
            public const string BookingAccommodationId = "tc_bookingaccommodationid";
            public const string BookingId = "tc_bookingid";
            public const string BookingTransportId = "tc_bookingtransportid";
            public const string Name = "tc_name";
            public const string RemarkName = "tc_remark";
            public const string RemarkId = "tc_remarkid";
            public const string SourceMarketID = "tc_sourcemarketid";
            public const string SourceSystemID = "tc_sourcesystemid";
            public const string Type = "tc_type";
        }

        public class SocialProfile
        {
            public const string ProfileName = "profilename";
            public const string Customer = "customerid";
            public const string SocialChannel = "community";
            public const string UniqueProfileID = "uniqueprofileid";

        }

        public class BookingTransfer
        {
            public const string ArrivalGatewayId = "tc_arrivalgatewayid";
            public const string BookingId = "tc_bookingid";
            public const string BookingTransferId = "tc_bookingid";
            public const string Category = "tc_category";
            public const string DepartureGatewayId = "tc_departuregatewayid";
            public const string EndDateTime = "tc_enddateandtime";
            public const string Name = "tc_name";
            public const string Order = "tc_order";
            public const string Participants = "tc_participants";
            public const string StartDateandTime = "tc_startdateandtime";
            public const string TransferCode = "tc_transfercode";
            public const string TransferType = "tc_transfertype";

        }

        public class BookingExtraService
        {
            public const string BookingId = "tc_bookingid";
            public const string BookingExtraServiceId = "tc_bookingextraserviceid";
            public const string ExtraServiceCode = "tc_extraservicecode";
            public const string EndDateTime = "tc_enddateandtime";
            public const string Name = "tc_name";
            public const string Order = "tc_order";
            public const string Participants = "tc_participants";
            public const string StartDateandTime = "tc_startdateandtime";

        }



        public class TourOperator
        {
            public const string SourceMarketId = "tc_countryid";
            public const string Name = "tc_name";
            public const string TourOperatorCode = "tc_touroperatorcode";
            public const string TourOperatorId = "tc_touroperatorid";
        }

        public class SurveyResponse
        {
            public const string ResponseId = "tc_response_id";
            public const string SurveyId = "tc_survey_id";
            public const string Subject = "subject";
            public const string SurveyDescription = "tc_survey_description";
            public const string Mode = "tc_mode";
            public const string BeginTime = "tc_begintime";
            public const string ActivityAdditionalParams = "activityadditionalparams";
            public const string BookingId = "tc_bookingid";
            public const string CustomerId = "customers";
            public const string Regarding = "regardingobjectid";
        }

        public class SurveyResponseFeedback
        {
            public const string Name = "tc_name";
            public const string QuestionId = "tc_question_id";
            public const string QuestionFieldId = "tc_question_field_id";
            public const string QuestionName = "tc_question_name";
            public const string QuestionFieldLabel = "tc_question_fieldlabel";
            public const string QuestionFieldType = "tc_question_fieldtype";
            public const string QuestionResponse = "tc_question_response";
            public const string SurveyFeedbackId = "tc_surveyfeedbackid";
        }


        public class ActivityParty
        {
            public const string PartyId = "partyid";
        }


    }

    public class Relationships
    {
        public const string SurveyResponseFeedback = "tc_surveyresponse_tc_surveyresponsefeedback";
    }

    public class AliasName
    {
        public const string Booking = "booking.";
        public const string Contact = "contact.";
    }

    public class PayloadSurveyFieldMapping
    {
        public const int BookingNumber = 251884;
        public const int SourceMarket = 251727;        
        public const int ForeName = 251886;
        public const int SurName = 251887;
    }

}
