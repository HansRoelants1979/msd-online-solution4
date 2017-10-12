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
        public const string Name = "tc_name";
        public const string MasterHotelId = "tc_masterhotelid";
        public const string OwningTeam = "owningteam";
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

    public static class SurveyResponse
    {
        public const string Regarding = "regardingobjectid";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
    }

    public static class Case
    {
        public const string SurveyId = "tc_surveyid";
    }

    public static class Configuration
    {
        public const string Name = "tc_name";
        public const string Configurationid = "tc_configurationid";
        public const string Value = "tc_value";
    }

    public static class EntityCache
    {
        public const string Name = "tc_name";
        public const string Type = "tc_type";
        public const string Data = "tc_data";
        public const string Operation = "tc_operation";
        public const string SourceMarket = "tc_sourcemarket";
        public const string RecordId = "tc_recordid";
		public const string EligibleRetryTime = "tc_eligibleretrytime";
		public const string WasLastOperationSuccessful = "tc_waslastoperationsuccessful";
		public const string StatusCode = "statuscode";
		public const string Status = "status";
		public const string EntityCacheId = "tc_entitycacheid";
	}

    public static class Customer
    {
        public const string SourceMarketId = "tc_sourcemarketid";
        public const string FullName = "fullname";
        public const string Address1FlatorUnitNumber = "tc_address1_flatorunitnumber";
        public const string Address1HouseNumberoBuilding = "tc_address1_housenumberorbuilding";
        public const string Address1Street = "tc_address1_street";
        public const string Address1AdditionalInformation = "tc_address1_additionalinformation";
        public const string Address1Town = "tc_address1_town";
        public const string Address1County = "tc_address1_county";
        public const string Address1CountryId = "tc_address1_countryid";
        public const string Address1PostalCode = "tc_address1_postalcode";
        public const string Address2FlatorUnitNumber = "tc_address2_flatorunitnumber";
        public const string Address2HouseNumberoBuilding = "tc_address2_housenumberorbuilding";
        public const string Address2Street = "tc_address2_street";
        public const string Address2AdditionalInformation = "tc_address2_additionalinformation";
        public const string Address2Town = "tc_address2_town";
        public const string Address2County = "tc_address2_county";
        public const string Address2CountryId = "tc_address2_countryid";
        public const string Address2PostalCode = "tc_address2_postalcode";
        public const string Telephone1 = "telephone1";
        public const string Telephone1Type = "tc_telephone1type";
        public const string Telephone2 = "telephone2";
        public const string Telephone2Type = "tc_telephone2type";
        public const string Telephone3 = "telephone3";
        public const string Telephone3Type = "tc_telephone3type";
        public const string EmailAddress1 = "emailaddress1";
        public const string EmailAddress1Type = "tc_emailaddress1type";
        public const string EmailAddress2 = "emailaddress2";
        public const string EmailAddress2Type = "tc_emailaddress2type";
        public const string EmailAddress3 = "emailaddress3";
        public const string EmailAddress3Type = "tc_emailaddress3type";
    }

    public static class Country
    {
        public const string ISO2Code = "tc_iso2code";
    }

    public static class ActivityParty
    {
        public const string PartyId = "partyid";
    }
    public static class FollowUp
    {
        public const string RescheduleCheck = "tc_reschedulecheck";
        public const string RescheduleReason = "tc_reschedulereason";
        public const string DueDate = "scheduledend";
        public const string ContactTime = "tc_contacttime";
        
    }

    public static class UserSettings
    {
        public const string TimeZoneCode = "timezonecode";
        public const string SystemUserId = "systemuserid";        
    }

    public static class Task
    {
        public const string Subject = "subject";
        public const string Description = "description";
    }

    public static class PhoneCall
    {
        public const string Subject = "subject";
        public const string Description = "description";
    }

    public static class AssistanceRequest
    {
        public const string Description = "description";
        public const string ExchangeItemId = "exchangeitemid";
    }

    public static class Appointment
    {
        public const string Subject = "subject";
        public const string Description = "description";
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
    public static class Configurationkeys
    {
        public const string CreditCardPattern = "Tc.Validation.GDPR.CreditCardPattern";
    }

    public static class SpecialCharacters
    {
        public const string OpenBrace = "{";
        public const string ClosedBrace = "}";
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
        public const string DataYouEnteredInNotesContainPotentionalCreditCardNumber = "The data you entered contains a barred sequence (16 consecutive numbers).";
    }

    public static class Messages
    {
        public const string Associate = "Associate";
        public const string Create = "Create";
        public const string Disassociate = "Disassociate";
        public const string Update = "Update";
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
        public const string SurveyResponse = "tc_surveyresponse";
        public const string Case = "incident";
        public const string Contact = "contact";
        public const string Booking = "tc_booking";
        public const string Phonecall = "phonecall";
        public const string Opportunity = "opportunity";
        public const string Configuration = "tc_configuration";       
        public const string EntityCache = "tc_entitycache";
        public const string Country = "tc_country";
        public const string FollowUp = "tc_followup";
        public const string UserSettings = "usersettings";
        public const string Task = "task";
        public const string Email = "email";
        public const string Appointment = "appointment";
        public const string AssistanceRequest = "tc_assistancerequest";               
    }

    public static class Relationships
    {
        public const string TeamRolesAssociation = "teamroles_association";
        public const string TeamMembershipAssociation = "teammembership_association";
        public const string UserHotels = "tc_systemuser_tc_hotel";
    }

    public static class JwtPayloadParameters
    {
        public const string IssuedAtTime = "iat";
        public const string NotBeforeTime = "nbf";
        public const string Expiry = "exp";
    }
    public enum PluginStage
    {
        Prevalidation = 10,
        Preoperation = 20,
        Postoperation = 40
    }
    public enum PluginMode
    {
        Synchronous = 0,
        Asynchronous = 1
    }
    
}

namespace Tc.Crm.Plugins.OptionSetValues
{
    public class Operation
    {
        public const int Create = 950000000;
        public const int Update = 950000001;
    }

	public enum EntityCacheStatusReason
	{
		Active = 1,
		InProgress = 950000000,
		Failed = 950000001,
		Pending = 950000002,
		Succeeded = 2
	}
}
