using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tc.Crm.Service.Constants
{

    public static class General
    {
        public const string TrueValue= "true";
    }
    public static class JsonWebTokenContent
    {
        public const string AlgorithmHS256= "HS256";
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Jwt")]
        public const string TypeJwt = "JWT";
    }
    public static class Delimiters
    {
        public const char Dot = '.';
    }
    public static class Messages
    {
        public const string ResponseNull = "Response is Null.";
        public const string SourceKeyNotPresent = "Source key is empty or null.";
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
        public const string DataJson = "dataJson";
        public const string Token = "token";
        public const string ActionContext = "actionContext";
    }
}
namespace Tc.Crm.Service.Constants.Configuration
{
    public static class AppSettings
    {
        public const string JsonWebTokenSecret = "jwtkey";
        public const string RedirectToHttps = "redirectToHttps";
        public const string IssuedAtTimeExpiryInSeconds = "issuedAtTimeExpiryInSeconds";
    }
    public static class ConnectionStrings
    {
        public const string Crm = "Crm";
    }
}
namespace Tc.Crm.Service.Constants.Crm
{
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