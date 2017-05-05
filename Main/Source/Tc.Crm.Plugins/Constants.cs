using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public const string PrivateKey = "Tc.Caching.PrivateKey";
        public const string ServiceUrl = "Tc.Caching.ServiceUrl";
        public const string Api = "Tc.Caching.Api";
        public const string IssuedAtTimeFromNow = "Tc.Caching.Payload.IatSecondsFromNow";
        public const string NotBeforeTimeFromNow = "Tc.Caching.Payload.NbfSecondsFromNow";
        public const string ExpirySecondsFromNow = "Tc.Caching.Payload.ExpirySecondsFromNow";
        public const string TokenApi = "Tc.Caching.Token.Api";
    }
    public static class General
    {
        public const string RoleTcIdBase = "Tc.Ids.Base";
        public const string RoleTcIdRep = "Tc.Ids.Rep";
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
