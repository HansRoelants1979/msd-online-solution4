namespace Tc.Crm.Service.Constants
{
    public static class Path
    {
        public const string Customeridentifier_Sourcemarket = "customer/customerIdentifier/sourceMarket";
        public const string Customergeneral_Customerstatus = "customer/customerGeneral/customerStatus";
        public const string Customeridentity_Salutation = "customer/customerIdentity/salutation";
        public const string Customeridentity_Academictitle = "customer/customerIdentity/academicTitle";
        public const string Customeridentity_Firstname = "customer/customerIdentity/firstName";
        public const string Customeridentity_Middlename = "customer/customerIdentity/middleName";
        public const string Customeridentity_Lastname = "customer/customerIdentity/lastname";
        public const string Customeridentity_Language = "customer/customerIdentity/language";
        public const string Customeridentity_Gender = "customer/customerIdentity/gender";
        public const string Customeridentity_Birthdate = "customer/customerIdentity/birthDate";
        public const string Company_Companyname = "customer/company/companyName";
        public const string Additional_Segment = "customer/additional/segment";
        public const string Additional_DateOfDeath = "customer/additional/dateOfdeath";
        public const string Address1_Additionaladdressinfo = "customer/address1/additionalAddressInfo";
        public const string Address1_Flatnumberunit = "customer/address1/flatNumberUnit";
        public const string Address1_Housenumberbuilding = "customer/address1/houseNumberBuilding";
        public const string Address1_Street = "customer/address1/street";
        public const string Address1_Town = "customer/address1/town";
        public const string Address1_Country = "customer/address1/country";
        public const string Address1_County = "customer/address1/county";
        public const string Address1_Postalcode = "customer/address1/postalCode";
        public const string Address2_Additionaladdressinfo = "customer/address2/additionalAddressInfo";
        public const string Address2_Flatnumberunit = "customer/address2/flatNumberUnit";
        public const string Address2_Housenumberbuilding = "customer/address2/houseNumberBuilding";
        public const string Address2_Street = "customer/address2/street";
        public const string Address2_Town = "customer/address2/town";
        public const string Address2_Country = "customer/address2/country";
        public const string Address2_County = "customer/address2/county";
        public const string Address2_Postalcode = "customer/address2/postalCode";
        public const string Phone1_Type = "customer/phone1/type";
        public const string Phone1_Number = "customer/phone1/number";
        public const string Phone2_Type = "customer/phone2/type";
        public const string Phone2_Number = "customer/phone2/number";
        public const string Phone3_Type = "customer/phone3/type";
        public const string Phone3_Number = "customer/phone3/number";
        public const string Email1_Type = "customer/email1/type";
        public const string Email1_Email = "customer/email1/address";
        public const string Email2_Type = "customer/email2/type";
        public const string Email2_Email = "customer/email2/address";
        public const string Email3_Type = "customer/email3/type";
        public const string Email3_Email = "customer/email3/address";
		public const string Permissions_Allowmarketing = "customer/permissions/allowMarketing";
		public const string Permissions_Donotallowemail = "customer/permissions/doNotAllowEmail";
		public const string Permissions_Donotallowmail = "customer/permissions/doNotAllowMail";
		public const string Permissions_Donotallowphonecalls = "customer/permissions/doNotAllowPhoneCalls";
		public const string Permissions_Donotallowsms = "customer/permissions/doNotAllowSMS";
	}
    public static class Attributes
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
        public const string Address1AdditionalInformation = "tc_address1_additionalinformation";
        public const string Address1County = "tc_address1_county";
        public const string Address1FlatOrUnitNumber = "tc_address1_flatorunitnumber";
        public const string Address1HouseNumberOrBuilding = "tc_address1_housenumberorbuilding";
        public const string Address1Town = "tc_address1_town";
        public const string Address1CountryId = "tc_address1_countryid";
        public const string Address1Street = "tc_address1_street";
        public const string Address1PostalCode = "tc_address1_postalcode";
        public const string Address2AdditionalInformation = "tc_address2_additionalinformation";
        public const string Address2CountryId = "tc_address2_countryid";
        public const string Address2Street = "tc_address2_street";
        public const string Address2County = "tc_address2_county";
        public const string Address2FlatOrUnitNumber = "tc_address2_flatorunitnumber";
        public const string Address2HouseNumberOrBuilding = "tc_address2_housenumberorbuilding";
        public const string Address2PostalCode = "tc_address2_postalcode";
        public const string Address2Town = "tc_address2_town";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
        public const string DateOfDeath = "tc_dateofdeath";
        public const string EmailAddress1Type = "tc_emailaddress1type";
        public const string EmailAddress2Type = "tc_emailaddress2type";
        public const string EmailAddress3Type = "tc_emailaddress3type";
        public const string Segment = "tc_segment";
        public const string Telephone1 = "telephone1";
        public const string Telephone2 = "telephone2";
        public const string Telephone3 = "telephone3";
        public const string EmailAddress1 = "emailaddress1";
        public const string EmailAddress2 = "emailaddress2";
        public const string EmailAddress3 = "emailaddress3";
        public const string MiddleName = "middlename";
        public const string Birthdate = "birthdate";
        public const string SourceSystemId = "tc_sourcesystemid";
        public const string ThomasCookMarketingConsent = "tc_thomascookmarketingconsent";
        public const string SendMarketingByPost = "tc_sendmarketingbypost";
        public const string MarketingByPhone = "tc_marketingbyphone";
        public const string SendMarketingBySms = "tc_sendmarketingbysms";
        public const string SendMarketingByEmail = "tc_sendmarketingbyemail";
        public const string Name = "name";
    }
    public static class General
    {
        public const string BookingJsonSchema = "{ \"$schema\": \"http://json-schema.org/draft-04/schema#\", \"type\": \"object\", \"title\": \"ThomasCookIntegrationLayer\", \"description\": \"ThomasCookIntegrationLayer-draft\", \"definitions\": { \"address\": { \"type\": \"object\", \"properties\": { \"additionalAddressInfo\": { \"type\": \"string\" }, \"flatNumberUnit\": { \"type\": \"string\" }, \"houseNumberBuilding\": { \"type\": \"string\" }, \"box\": { \"type\": \"string\" }, \"town\": { \"type\": \"string\" }, \"country\": { \"type\": \"string\" }, \"county\": { \"type\": \"string\" }, \"number\": { \"type\": \"string\" }, \"postalCode\": { \"type\": \"string\" }, \"street\": { \"type\": \"string\" }, \"type\": { \"type\": \"string\", \"enum\": [ \"M\" ] } } }, \"booking\": { \"type\": \"object\", \"properties\": { \"bookingIdentifier\": { \"type\": \"object\", \"properties\": { \"sourceMarket\": { \"type\": \"string\" }, \"sourceSystem\": { \"type\": \"string\" }, \"bookingNumber\": { \"type\": \"string\" }, \"bookingVersionOnTour\": { \"type\": \"string\" }, \"bookingVersionTourOperator\": { \"type\": \"string\" }, \"bookingUpdateDateOnTour\": { \"type\": \"string\" }, \"bookingUpdateDateTourOperator\": { \"type\": \"string\" } } }, \"general\": { \"type\": \"object\", \"properties\": { \"bookingStatus\": { \"type\": \"string\", \"enum\": [ \"B\", \"C\" ] }, \"bookingDate\": { \"type\": \"string\" }, \"departureDate\": { \"type\": \"string\" }, \"returnDate\": { \"type\": \"string\" }, \"duration\": { \"type\": \"string\" }, \"destination\": { \"type\": \"string\" }, \"toCode\": { \"type\": \"string\" }, \"brand\": { \"type\": \"string\" }, \"brochureCode\": { \"type\": \"string\" }, \"isLateBooking\": { \"type\": \"boolean\" }, \"numberofParticipants\": { \"type\": \"integer\" }, \"numberOfAdults\": { \"type\": \"integer\" }, \"numberOfChildren\": { \"type\": \"integer\" }, \"numberOfInfants\": { \"type\": \"integer\" }, \"travelAmount\": { \"type\": \"number\" }, \"currency\": { \"type\": \"string\" }, \"hasComplaint\": { \"type\": \"boolean\" } } }, \"bookingIdentity\": { \"type\": \"object\", \"properties\": { \"booker\": { \"$ref\": \"#/definitions/booking_booker\" } } }, \"travelParticipant\": { \"type\": \"array\", \"items\": { \"type\": \"object\", \"properties\": { \"firstName\": { \"type\": \"string\" }, \"lastName\": { \"type\": \"string\" }, \"age\": { \"type\": \"integer\" }, \"gender\": { \"type\": \"string\", \"enum\": [ \"M\", \"F\", \"U\" ] }, \"relation\": { \"type\": \"string\", \"enum\": [ \"P\", \"C\", \"I\" ] }, \"travelParticipantIDOnTour\": { \"type\": \"string\" }, \"language\": { \"type\": \"string\" }, \"birthDate\": { \"type\": \"string\" }, \"Remark\": { \"type\": \"array\", \"items\": { \"$ref\": \"#/definitions/remark\" } } } } }, \"services\": { \"type\": \"object\", \"properties\": { \"accommodation\": { \"type\": \"array\", \"items\": { \"type\": \"object\", \"properties\": { \"accommodationCode\": { \"type\": \"string\" }, \"groupAccommodationCode\": { \"type\": \"string\" }, \"accommodationDescription\": { \"type\": \"string\" }, \"order\": { \"type\": \"integer\" }, \"startDate\": { \"type\": \"string\" }, \"endDate\": { \"type\": \"string\" }, \"roomType\": { \"type\": \"string\" }, \"boardType\": { \"type\": \"string\", \"enum\": [ \"AI\", \"HB\" ] }, \"status\": { \"type\": \"string\", \"enum\": [ \"OK\", \"R\", \"PR\" ] }, \"hasSharedRoom\": { \"type\": \"boolean\" }, \"numberOfParticipants\": { \"type\": \"integer\" }, \"numberOfRooms\": { \"type\": \"integer\" }, \"withTransfer\": { \"type\": \"boolean\" }, \"isExternalService\": { \"type\": \"boolean\" }, \"externalServiceCode\": { \"type\": \"string\" }, \"notificationRequired\": { \"type\": \"boolean\" }, \"needsTourGuideAssignment\": { \"type\": \"boolean\" }, \"isExternalTransfer\": { \"type\": \"boolean\" }, \"transferServiceLevel\": { \"type\": \"string\" }, \"travelParticipantAssignment\": { \"type\": \"array\", \"items\": { \"properties\": { \"travelParticipantID\": { \"type\": \"string\" } } } }, \"remark\": { \"type\": \"array\", \"items\": { \"$ref\": \"#/definitions/remark\" } }, \"tourguideAssignment\": { \"type\": \"object\", \"properties\": { \"tourguide\": { \"type\": \"object\", \"properties\": { \"tourguideID\": { \"type\": \"string\" }, \"tourguideName\": { \"type\": \"string\" }, \"brands\": { \"type\": \"array\", \"items\": { \"items\": [ { \"type\": \"object\", \"properties\": { \"brand\": { \"type\": \"string\" } } } ] } } } } } } } } }, \"transport\": { \"type\": \"array\", \"items\": { \"type\": \"object\", \"properties\": { \"transportCode\": { \"type\": \"string\" }, \"transportDescription\": { \"type\": \"string\" }, \"order\": { \"type\": \"integer\" }, \"startDate\": { \"type\": \"string\" }, \"endDate\": { \"type\": \"string\" }, \"transferType\": { \"type\": \"string\", \"enum\": [ \"I\", \"O\", \"TH\" ] }, \"departureAirport\": { \"type\": \"string\" }, \"arrivalAirport\": { \"type\": \"string\" }, \"carrierCode\": { \"type\": \"string\" }, \"flightNumber\": { \"type\": \"string\" }, \"flightIdentifier\": { \"type\": \"string\" }, \"numberOfParticipants\": { \"type\": \"integer\" }, \"travelParticipantAssignment\": { \"type\": \"array\", \"items\": { \"properties\": { \"travelParticipantID\": { \"type\": \"string\" } } } }, \"remark\": { \"type\": \"array\", \"items\": { \"$ref\": \"#/definitions/remark\" } } } } }, \"transfer\": { \"type\": \"array\", \"items\": { \"type\": \"object\", \"properties\": { \"transferCode\": { \"type\": \"string\" }, \"transferDescription\": { \"type\": \"string\" }, \"order\": { \"type\": \"integer\" }, \"startDate\": { \"type\": \"string\" }, \"category\": { \"type\": \"string\" }, \"endDate\": { \"type\": \"string\" }, \"transferType\": { \"type\": \"string\", \"enum\": [ \"I\", \"O\", \"TH\" ] }, \"departureAirport\": { \"type\": \"string\" }, \"arrivalAirport\": { \"type\": \"string\" }, \"travelParticipantAssignment\": { \"type\": \"array\", \"items\": { \"properties\": { \"travelParticipantID\": { \"type\": \"string\" } } } }, \"remark\": { \"type\": \"array\", \"items\": { \"$ref\": \"#/definitions/remark\" } } } } }, \"extraService\": { \"type\": \"array\", \"items\": { \"type\": \"object\", \"properties\": { \"extraServiceCode\": { \"type\": \"object\", \"properties\": {} }, \"extraServiceDescription\": { \"type\": \"object\", \"properties\": {} }, \"order\": { \"type\": \"integer\" }, \"startDate\": { \"type\": \"string\" }, \"endDate\": { \"type\": \"string\" }, \"travelParticipantAssignment\": { \"type\": \"array\", \"items\": { \"properties\": { \"travelParticipantID\": { \"type\": \"string\" } } } }, \"remark\": { \"type\": \"array\", \"items\": { \"$ref\": \"#/definitions/remark\" } } } } } } }, \"customer\": { \"$ref\": \"#/definitions/customer\" }, \"remark\": { \"type\": \"array\", \"items\": { \"$ref\": \"#/definitions/remark\" } } } }, \"customer\": { \"type\": \"object\", \"properties\": { \"customerIdentifier\": { \"type\": \"object\", \"properties\": { \"customerID\": { \"type\": \"string\" }, \"businessArea\": { \"type\": \"string\" }, \"sourceMarket\": { \"type\": \"string\" }, \"sourceSystem\": { \"type\": \"string\" } } }, \"general\": { \"type\": \"object\", \"properties\": { \"customerStatus\": { \"type\": \"string\", \"enum\": [ \"A\", \"D\", \"B\" ] }, \"customerType\": { \"type\": \"string\", \"enum\": [ \"P\", \"B\" ] } } }, \"customerIdentity\": { \"type\": \"object\", \"properties\": { \"salutation\": { \"type\": \"string\" }, \"academictitle\": { \"type\": \"string\" }, \"firstName\": { \"type\": \"string\" }, \"middleName\": { \"type\": \"string\" }, \"lastName\": { \"type\": \"string\" }, \"language\": { \"type\": \"string\" }, \"gender\": { \"type\": \"string\", \"enum\": [ \"M\", \"F\", \"U\" ] }, \"birthdate\": { \"type\": \"string\" } } }, \"company\": { \"type\": \"object\", \"properties\": { \"companyName\": { \"type\": \"string\" } } }, \"additional\": { \"type\": \"object\", \"properties\": { \"segment\": { \"type\": \"string\" }, \"dateOfdeath\": { \"type\": \"string\" } } }, \"address\": { \"type\": \"array\", \"items\": { \"$ref\": \"#/definitions/address\" } }, \"phone\": { \"type\": \"array\", \"additionalItems\": false, \"items\": [ { \"type\": \"object\", \"properties\": { \"type\": { \"type\": \"string\", \"enum\": [ \"H\", \"M\" ] }, \"number\": { \"type\": \"string\" } } } ] }, \"email\": { \"type\": \"array\", \"additionalItems\": false, \"items\": [ { \"type\": \"object\", \"properties\": { \"type\": { \"type\": \"string\", \"enum\": [ \"Pri\", \"Pro\" ] }, \"address\": { \"type\": \"string\" } } } ] }, \"social\": { \"type\": \"array\", \"additionalItems\": false, \"items\": [ { \"type\": \"object\", \"properties\": { \"type\": { \"type\": \"string\" }, \"value\": { \"type\": \"string\" } } } ] } } }, \"remark\": { \"type\": \"object\", \"properties\": { \"type\": { \"type\": \"string\", \"enum\": [ \"T\", \"A\" ] }, \"text\": { \"type\": \"string\" } } }, \"booking_booker\": { \"anyOf\": [ { \"$ref\": \"#/definitions/address\" }, { \"properties\": { \"email\": { \"type\": \"string\" }, \"phone\": { \"type\": \"string\" }, \"mobile\": { \"type\": \"string\" }, \"emergencyNumber\": { \"type\": \"string\" } } } ] } } }";
        public const string TrueValue = "true";
        public const string Error = "ERROR";
    }
    public static class JsonWebTokenContent
    {
        public const string AlgorithmRS256 = "RS256";
        public const string AlgorithmHS256 = "HS256";
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Jwt")]
        public const string TypeJwt = "JWT";
    }
    public static class Delimiters
    {
        public const char Dot = '.';
    }
    public static class SourceMarketIsoCode
    {
        public const string UK = "GB";
        public const string Germany = "DE";
        public const string France = "FR";
        public const string Belgium = "BE";
        public const string CzechRepublic = "CZ";
        public const string Poland = "PL";
        public const string Netherlands = "NL";
        public const string Hungary = "HU";

    }
    public static class CurrencyCode
    {
        public const string Euro = "EUR";
        public const string Pound = "GBP";
        public const string HungarianForint = "HUF";
        public const string CzechKoruna = "CZK";
        public const string PolishZłoty = "PLN";
    }
    public static class Messages
    {
        public const string PayloadReadError = "Payload could not be read.";
        public const string CustomerSourceMarketMissing = "Customer record doesn't have a source market or the source market provided could not be resolved.";
        public const string SourceMarketMissing = "Booking record doesn't have a source market or the source market provided could not be resolved.";
        public const string CurrencyResolutionError = "Currency could not be resolved.";
        public const string CustomerIdIsNull = "Customer does not have a customer id.";
        public const string BookingSystemIsUnknown = "Booking system provided is either null or unknown";
        public const string ResponseNull = "Response is Null.";
        public const string SourceKeyNotPresent = "Source system id of booking record is empty or null.";
        public const string bookingNumberNotPresent = "Booking number is missing";
        public const string BookingDatabaseNotPresent = "Booking database is unknown or not provided.";
        public const string ConsultationReferenceNotPresent = "The booking received was from TCV but no consultationReference was provided within the supplied Json";
        public const string CustomerGeneralNotPresent = "Customer general is empty or null";
        public const string JsonWebTokenParserError = "Error while parsing JSON Web Token.";
        public const string SignatureValidationUnhandledError = "Error while validating the signature.";
        public const string HeaderValidationUnhandledError = "Error while validating the header.";
        public const string PayloadValidationUnhandledError = "Error while validating the payload.";
        public const string JsonWebTokenExpiredOrNoMatch = "Token has either expired or signature doesn't match.";
        public const string ConnectionStringNull = "Connection string is null.";
        public const string CrmConnectionIsNull = "Could not CRM connection from the connection string provided.";
        public const string CrmConnectionParseError = "Error while parsing connection string.";
        public const string HttpsRequired = "HTTPS Required";
        public const string ClaimNotInteger = "Claim value must be an integer.";
        public const string TokenFormatError = "Token must consist from 3 delimited by dot parts";
        public const string ExpiryNotInteger = "Expiry value in config is not an integer.";
        public const string TokenIsNull = "Token is null.";
        public const string PayloadIsNull = "Payload is null.";
        public const string BookingObjectIsNull = "Booking object is null.";
        public const string CustomerObjectIsNull = "Customer object is null.";
        public const string ResponseFromCrmIsNull = "Response from CRM is NULL.";
        public const string BookingDataPassedIsNullOrCouldNotBeParsed = "Booking data passed is null or could not be parsed.";
        public const string SurveyDataPassedIsNullOrCouldNotBeParsed = "Survey data passed is null or could not be parsed.";
        public const string BookingDataDoesNotComplyToSchema = "Booking data passed does not comply to schema.";
        public const string FailedtoCreateSurvey = "The corresponding operations can not be completed on downstream applications (MSD) for some reason. This should be considered as a temporary issue and retried.";
        public const string CustomerCreationError = "Customer Already Exists in MSD against the provided customer Id";
        public const string CustomerUpdateError = "Customer does not exists in MSD against the provided customer Id";
        public const string CustomerDataPassedIsNullOrCouldNotBeParsed = "Customer data passed is null or could not be parsed.";
        public const string CustomerTypeNotPresent = "Customer type record is empty or null.";
        public const string CustomerIdentityNotPresent = "Customer Identity is null or empty";
        public const string FailedToUpdateEntityCacheMessage = FailedtoCreateSurvey;
        public const string ConfirmationDataPassedIsNullOrCouldNotBeParsed = "The payload could not be read.";
        public const string MissingMsdCorrelationId = "msdCorrelationId is missing.";
		public const string IncorrectMsdCorrelationId = "msdCorrelationId is not guid.";
		public const string MissingCorrelationId = "correlationId missing in request body.";
		public const string UnexpectedError = "An unexpected error has occurred.";
		public const string MsdCorrelationIdDoesNotExist = "Record with id {0} does not exist";
	}
    public static class Parameters
    {
        public const string JsonWebTokenRequest = "jsonWebTokenRequest";
        public const string JsonWebTokenRequestToken = "jsonWebTokenRequest.Token";
        public const string Request = "request";
        public const string RequestHeaders = "request.Headers";
        public const string RequestHeader = "request.Header";
        public const string RequestPayload = "request.Payload";
        public const string RequestHeadersAuthorization = "request.Headers.Authorization";
        public const string RequestHeaderAlgorithm = "request.Header.Algorithm";
        public const string RequestHeaderType = "request.Header.Type";
        public const string Customer = "customer";
        public const string Booking = "booking";
        public const string BookingData = "bookingData";
        public const string ProcessCustomer = "ProcessCustomer";
        public const string DataJson = "dataJson";
        public const string Token = "token";
        public const string ActionContext = "actionContext";
        public const string CrmService = "crmService";
        public const string CustomerId = "customerId";
        public const string Data = "data";
    }
}
namespace Tc.Crm.Service.Constants.Configuration
{
    public static class AppSettings
    {
        public const string JsonWebTokenSecret = "jwtkey";
        public const string RedirectToHttps = "redirectToHttps";
        public const string IssuedAtTimeExpiryInSeconds = "issuedAtTimeExpiryInSeconds";
        public const string BookingPublicKeyFileNames = "Booking_PublicKey_FileNames";
        public const string SurveyPublicKeyFileNames = "Survey_PublicKey_FileNames";
        public const string CustomerPublicKeyFileNames = "Customer_PublicKey_FileNames";
        public const string ConfirmationPublicKeyFileNames = "Confirmation_PublicKey_FileNames";
    }
    public static class ConnectionStrings
    {
        public const string Crm = "Crm";
    }
}
namespace Tc.Crm.Service.Constants.Crm
{
    public static class Actions
    {
        public const string ProcessBooking = "tc_ProcessBooking";
        public const string ProcessSurvey = "tc_ProcessSurvey";
        public const string ProcessCustomer = "tc_ProcessCustomer";
        public const string ParameterData = "BookingInfo";
        public const string CustomerData = "CustomerInfo";
        public const string ParameterSurveyData = "SurveyResponseInfo";
        public const string ProcessSurveyResponse = "Response";
        public const string ProcessBookingResponse = "Response";
        public const string ProcessCustomerResponse = "Response";
        public const string Patch = "Patch";
        public const string Post = "Post";
        public const string Operation = "Operation";

        public enum OperationType
        {
            Post,
            Patch
        }
    }
    public static class Booking
    {
        public const string LogicalName = "new_booking";
    }
    public static class Contact
    {
        public const string LogicalName = "contact";
    }
}
namespace Tc.Crm.Service.Constants.Crm.Fields
{
    public static class Booking
    {
        public const string SourceKey = "new_sourcekey";
        public const string CustomerId = "new_customerid";
        public const string Total = "new_total";
    }
    public static class Contact
    {
        public const string FirstName = "firstname";
        public const string LastName = "lastname";
        public const string Email = "emailaddress1";
        public const string SourceKey = "new_sourcekey";
        public const string ContactId = "contactid";
    }
}