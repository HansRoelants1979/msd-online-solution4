using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tc.Crm.Service
{
    public static class Constants
    {
        public const string TRUE_VALUE = "true";
        public static class JsonWebTokenContent
        {
            public const string ALGORITHM_HS256 = "HS256";
            public const string TYPE_JWT = "JWT";
        }
        public static class Configuration
        {
            public static class AppSettings
            {
                public const string JSON_WEB_TOKEN_SECRET = "jwtkey";
                public const string REDIRECT_TO_HTTPS = "redirectToHttps";
                public const string ISSUED_AT_TIME_EXPIRY_IN_SECONDS = "issuedAtTimeExpiryInSeconds";
            }
            public static class ConnectionStrings
            {
                public const string CRM = "Crm";
            }
        }
        public static class Delimiters
        {
            public const char DOT = '.';
        }
        public static class Messages
        {
            public const string RESPONSE_NULL = "Response is Null.";
            public const string SOURCE_KEY_NOT_PRESENT = "Source key is empty or null.";
            public const string JSON_WEB_TOKEN_PARSES_ERROR = "Error while parsing JSON Web Token.";
            public const string SIGNATURE_VALIDATION_UNHANDLED_ERROR = "Error while validating the signature.";
            public const string HEADER_VALIDATION_UNHANDLED_ERROR = "Error while validating the header.";
            public const string PAYLOAD_VALIDATION_UNHANDLED_ERROR = "Error while validating the payload.";
            public const string JSON_WEB_TOKEN_EXPIRED_NO_MATCH = "Token has either expired or signature doesn't match.";
            public const string CONNECTION_STRING_NULL ="Connection string is null.";
            public const string CRM_CONNECTION_IS_NULL = "Could not CRM connection from the connection string provided.";
            public const string CRM_CONNECTION_PARSE_ERROR = "Error while parsing connection string.";
            public const string HTTPS_REQUIRED = "HTTPS Required";
            public const string CLAIM_NOT_INT = "Claim value must be an integer.";
            public const string TOKEN_FORMAT_ERROR = "Token must consist from 3 delimited by dot parts";
            public const string EXPIRY_NOT_INT = "Expiry value in config is not an integer.";
        }
        public static class Parameters
        {
            public const string JSON_WEB_TOKEN_REQUEST = "jsonWebTokenRequest";
            public const string JSON_WEB_TOKEN_REQUEST_TOKEN = "jsonWebTokenRequest.Token";
            public const string REQUEST = "request";
            public const string REQUEST_HEADERS = "request.Headers";
            public const string REQUEST_HEADER = "request.Header";
            public const string REQUEST_PAYLOAD = "request.Payload";
            public const string REQUEST_HEADERS_AUTHORIZATION = "request.Headers.Authorization";
            public const string REQUEST_HEADER_ALGORITHM = "request.Header.Algorithm";
            public const string REQUEST_HEADER_TYPE = "request.Header.Type";
            public const string CUSTOMER = "customer";
            public const string BOOKING = "booking";
            public const string DATA_JSON = "dataJson";
            public const string TOKEN = "token";
        }
        public static class Crm
        {
            public static class Booking
            {
                public const string LOGICAL_NAME = "new_booking";
                public static class Fields
                {
                    public const string SOURCE_KEY = "new_sourcekey";
                    public const string CUSTOMER_ID = "new_customerid";
                    public const string TOTAL = "new_total";
                }
            }
            public static class Contact
            {
                public const string LOGICAL_NAME = "contact";
                public static class Fields
                {
                    public const string FIRST_NAME = "firstname";
                    public const string LAST_NAME = "lastname";
                    public const string EMAIL = "emailaddress1";
                    public const string SOURCE_KEY = "new_sourcekey";
                    public const string CONTACT_ID = "contactid";
                }
            }
        }
    }
}