using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tc.Crm.CustomWorkflowSteps.ProcessSurvey.Models
{
    public class Survey
    {
        public List<Response> Responses { get; set; }
    }

    [DataContract(Name = "response")]
    public class Response
    {


        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "surveyGatewayId")]
        public long? SurveyGatewayId { get; set; }

        [DataMember(Name = "surveyGatewayName")]
        public string SurveyGatewayName { get; set; }

        [DataMember(Name = "surveyGatewayAlias")]
        public string SurveyGatewayAlias { get; set; }

        [DataMember(Name = "surveyId")]
        public long? SurveyId { get; set; }

        [DataMember(Name = "surveyName")]
        public string SurveyName { get; set; }

        [DataMember(Name = "beginTime")]
        public string BeginTime { get; set; }

        [DataMember(Name = "dateOfService")]
        public string DateOfService { get; set; }

        [DataMember(Name = "redemptionCode")]
        public string RedemptionCode { get; set; }

        [DataMember(Name = "minutes")]
        public double Minutes { get; set; }

        [DataMember(Name = "complete")]
        public bool Complete { get; set; }

        [DataMember(Name = "read")]
        public bool Read { get; set; }

        [DataMember(Name = "ipAddress")]
        public string IpAddress { get; set; }

        [DataMember(Name = "cookieUID")]
        public string CookieUID { get; set; }

        [DataMember(Name = "offerId")]
        public long? OfferId { get; set; }

        [DataMember(Name = "offerName")]
        public string OfferName { get; set; }

        [DataMember(Name = "offerCode")]
        public string OfferCode { get; set; }

        [DataMember(Name = "unitId")]
        public long? UnitId { get; set; }

        [DataMember(Name = "unitExternalId")]
        public string UnitExternalId { get; set; }

        [DataMember(Name = "unitName")]
        public string UnitName { get; set; }

        [DataMember(Name = "organizationId")]
        public long? OrganizationId { get; set; }

        [DataMember(Name = "organizationName")]
        public string OrganizationName { get; set; }

        [DataMember(Name = "exclusionReason")]
        public string ExclusionReason { get; set; }

        [DataMember(Name = "mode")]
        public string Mode { get; set; }

        [DataMember(Name = "surveyDescription")]
        public string SurveyDescription { get; set; }

        [DataMember(Name = "answers")]
        public List<Answer> Answers { get; set; }
    }

    [DataContract(Name = "answers")]
    public class Answer
    {
        [DataMember(Name = "id")]
        public object Id { get; set; }

        [DataMember(Name = "fieldId")]
        public int FieldId { get; set; }

        [DataMember(Name = "fieldName")]
        public string FieldName { get; set; }

        [DataMember(Name = "fieldText")]
        public string FieldText { get; set; }

        [DataMember(Name = "fieldLabel")]
        public string FieldLabel { get; set; }

        [DataMember(Name = "fieldType")]
        public FieldType FieldType { get; set; }

        [DataMember(Name = "literalValue")]
        public string LiteralValue { get; set; }

        [DataMember(Name = "option")]
        public Option Option { get; set; }

        [DataMember(Name = "commentId")]
        public int? CommentId { get; set; }

        [DataMember(Name = "commentType")]
        public string CommentType { get; set; }

        [DataMember(Name = "commentArchived")]
        public bool? CommentArchived { get; set; }

    }

    [DataContract(Name = "option")]
    public class Option
    {

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "labels")]
        public List<LocalizedString> Label { get; set; }

        [DataMember(Name = "label")]
        public string DefaultLabel { get; set; }
    }

    [DataContract(Name = "LocalizedString")]
    public class LocalizedString
    {

        [DataMember(Name = "Locale")]
        public string Locale { get; set; }

        [DataMember(Name = "Value")]
        public string Value { get; set; }
    }

    [DataContract(Name = "FieldType")]
    public enum FieldType
    {
        [EnumMember]
        UNKNOWN = 0,
        [EnumMember]
        TEXT = 1,
        [EnumMember]
        NUMERIC = 2,
        [EnumMember]
        DATE = 3,
        [EnumMember]
        TIME = 4,
        [EnumMember]
        RATING = 5,
        [EnumMember]
        MULTIPLE_CHOICE = 6,
        [EnumMember]
        COMMENT = 7,
        [EnumMember]
        BOOLEAN = 8,
        [EnumMember]
        DAY_OF_WEEK = 9,
        [EnumMember]
        MONTH = 10
    }
}
