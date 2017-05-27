using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.Plugins.OptionSetValues
{
    public static class CacheRequest
    {
        public const int StatecodeInactive = 1;
        public const int StatuscodeSucceeded = 2;
        public const int StatuscodeFailed = 950000000;
    }

}
namespace Tc.Crm.Plugins.Attributes
{
    public static class CacheRequest
    {
        public const string Name = "tc_name";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
    }
    public static class Annotation
    {
        public const string Subject = "subject";
        public const string NoteText = "notetext";
        public const string ObjectId = "objectid";
    }
    public static class ConfigurationKeys
    {
        public const string Name = "tc_name";
        public const string Value = "tc_value";
        public const string LongValue = "tc_longvalue";
    }
    public static class Team
    {
        public const string Name = "name";
        public const string HotelTeam = "tc_hotelteam";
        public const string BusinessUnitId = "businessunitid";
        public const string TeamId = "teamid";
        public const string HotelTeamId = "tc_hotelteamid";
    }

    public static class Hotel
    {
        public const string Owner = "ownerid";
    }

    public static class Role
    {
        public const string Name = "name";
        public const string BusinessUnitId = "businessunitid";
        public const string RoleId = "roleid";
    }

    public static class BusinessUnit
    {
        public const string BusinessUnitId = "businessunitid";
        public const string Name = "name";
    }
}
namespace Tc.Crm.Plugins
{
    public static class CachingParameter
    {
        public const string SecretKey = "Tc.Caching.SecretKey";
        public const string ServiceUrl1 = "Tc.Caching.ServiceUrl.Server1";
        public const string ServiceUrl2 = "Tc.Caching.ServiceUrl.Server2";
        public const string Api = "Tc.Caching.Api";
        public const string IssuedAtTimeFromNow = "Tc.Caching.Payload.IatSecondsFromNow";
        public const string NotBeforeTimeFromNow = "Tc.Caching.Payload.NbfSecondsFromNow";
        public const string ExpirySecondsFromNow = "Tc.Caching.Payload.ExpirySecondsFromNow";
    }
    public static class General
    {
        public const string RoleTcIdBase = "Tc.Ids.Base";
        public const string RoleTcIdRep = "Tc.Ids.Rep";
    }

    public static class ValidationMessages{
        public const string ContextIsNull = "Context is null.";
        public const string TraceIsNull = "Trace is null.";
        public const string OrganizationServiceIsNull = "Organization service is null.";
        public const string ContextInvalidOrNameInvalid = "Either name provided in cache request is invalid or this plugin has fired out of context.";
        public const string CachingParameterIsNull = "Caching parametrer is null.";
        public const string ExpirySecondsFromNowNotSpecified = "Expiry seconds from now not specified in the configuration.";
        public const string ExpirySecondsFromNowIncorrectFormat = "Expiry seconds from now has incorrect format.";
        public const string IssuedAtTimeFromNowNotSpecified = "Issued at time has not been specified in the configuration.";
        public const string IssuedAtTimeFromNowIncorrectFormat = "Issued at time from now has an incorrect format.";
        public const string NotBeforeTimeFromNowNotSpecified = "Not before time from now has not been specified in the configuration.";
        public const string NotBeforeTimeFromNowIncorrectFormart = "Not before time from now has an incorrect format.";
        public const string ConfigurationValuesForCachingInCrmMissing = "Configuration values for caching is missing in CRM.";
        public const string ConfigurationHasNoValueForKey = "Configuration has no value for a specific key.";
        public const string CachingKeysMissingInCrm = "One or more caching keys are missing in the configuration entity in CRM.";
        public const string CachingSecretKeyIsNullOrEmpty = "Caching secret key is null or empty.";
        public const string TokenIsNull = "Token is null";
        public const string CachingErrorNoteSubject = "Caching Job has Failed.";
        public const string UnexpectedError = "An unexpected error has occurred.";
        public const string CachingServiceUrlIsNullOrEmpty = "Caching service url is null or empty.";
        public const string CachingApiIsNullOrEmpty = "Caching api is null or empty.";
        public const string RequestDataIsEmpty = "Request data is null or empty.";
    }

    public static class Messages
    {
        public const string Associate = "Associate";
        public const string Create = "Create";
        public const string Disassociate = "Disassociate";
    }

    public static class CacheBucket
    {
        public const string Brand = "BRAND";
        public const string Country = "COUNTRY";
        public const string Currency = "CURRENCY";
        public const string SourceMarket = "SOURCEMARKET";
        public const string Gateway = "GATEWAY";
        public const string TourOperator = "TOUROPERATOR";
        public const string Hotel = "HOTEL";
    }
    public static class InputParameters
    {
        public const string Target = "Target";
        public const string Relationship  = "Relationship";
        public const string RelatedEntities = "RelatedEntities";
    }

    public static class Entities
    {
        public const string Annotation = "annotation";
        public const string Team = "team";
        public const string BusinessUnit = "businessunit";
        public const string Role = "role";
        public const string User = "systemuser";
        public const string Hotel = "tc_hotel";
        public const string CacheRequest = "tc_cacherequest";
    }

    public static class Relationships
    {
        public const string TeamRolesAssociation = "teamroles_association";
        public const string TeamMembershipAssociation = "teammembership_association";
    }

    public static class JwtPayloadParameters
    {
        public const string IssuedAtTime = "iat";
        public const string NotBeforeTime = "nbf";
        public const string Expiry = "exp";
    }
    enum PluginStage
    {
        Prevalidation = 10,
        Preoperation = 20,
        Postoperation = 40
    }
}
