namespace Tc.Crm.Service.Constants
{

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
        public const string CustomerSourceMarketMissing = "Customer record doesn't have a source market or the source market provided could not be resolved.";
        public const string SourceMarketMissing = "Booking record doesn't have a source market or the source market provided could not be resolved.";
        public const string CurrencyResolutionError = "Currency could not be resolved.";
        public const string CustomerIdIsNull = "Customer does not have a customer id.";
        public const string BookingSystemIsUnknown = "Booking system provided is either null or unknown";
        public const string ResponseNull = "Response is Null.";
        public const string SourceKeyNotPresent = "Source system id of booking record is empty or null.";
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
        public const string DataJson = "dataJson";
        public const string Token = "token";
        public const string ActionContext = "actionContext";
        public const string CrmService = "crmService";
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
        public const string ParameterData = "BookingInfo";
        public const string ParameterSurveyData = "SurveyResponseInfo";
        public const string ProcessSurveyResponse = "Response";
        public const string ProcessBookingResponse = "Response";
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