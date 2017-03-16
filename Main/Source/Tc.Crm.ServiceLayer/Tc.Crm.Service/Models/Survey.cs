using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using InMoment;


namespace Tc.Crm.Service.Models
{

    public class Survey
    {
        public IList<Responses> Responses { get; set; }
    }

    public class Responses
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "surveyGatewayId")]
        public int SurveyGatewayId { get; set; }

        [DataMember(Name = "surveyGatewayName")]
        public string SurveyGatewayName { get; set; }

        [DataMember(Name = "surveyGatewayAlias")]
        public string SurveyGatewayAlias { get; set; }

        [DataMember(Name = "surveyId")]
        public int SurveyId { get; set; }

        [DataMember(Name = "surveyName")]
        public string SurveyName { get; set; }

        [DataMember(Name = "beginTime")]
        public DateTime BeginTime { get; set; }

        [DataMember(Name = "dateOfService")]
        public DateTime DateOfService { get; set; }

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
        public int OfferId { get; set; }

        [DataMember(Name = "offerName")]
        public string OfferName { get; set; }

        [DataMember(Name = "offerCode")]
        public string OfferCode { get; set; }

        [DataMember(Name = "unitId")]
        public int UnitId { get; set; }

        [DataMember(Name = "unitExternalId")]
        public string UnitExternalId { get; set; }

        [DataMember(Name = "unitName")]
        public string UnitName { get; set; }

        [DataMember(Name = "organizationId")]
        public int OrganizationId { get; set; }

        [DataMember(Name = "organizationName")]
        public string OrganizationName { get; set; }

        [DataMember(Name = "exclusionReason")]
        public string ExclusionReason { get; set; }

        [DataMember(Name = "mode")]
        public string Mode { get; set; }

        [DataMember(Name = "answers")]
        public IList<Answers> Answers { get; set; }
    }

    
    public class Answers
    {
        public object id { get; set; }
        public int fieldId { get; set; }
        public string fieldName { get; set; }
        public string fieldText { get; set; }
        public string fieldLabel { get; set; }
        public string fieldType { get; set; }
        public string literalValue { get; set; }
        public Option option { get; set; }
        public int? commentId { get; set; }
        public string commentType { get; set; }


    }

    public class Option
    {
        public int id { get; set; }
        public string name { get; set; }
        public string label { get; set; }
    }



}