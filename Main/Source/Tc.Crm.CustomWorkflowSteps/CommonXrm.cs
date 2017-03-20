using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System.IO;
using System.Text;
using System.Globalization;
using System.Xml;
using System;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps
{
    public class CommonXrm
    {


        /// <summary>
        /// To delete records by filtering the data
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="columns"></param>
        /// <param name="filterKeys"></param>
        /// <param name="filterValues"></param>
        /// <param name="service"></param>
        public static void MarkEntityRecordsAsPendingDelete(string entityName, string[] columns, string[] filterKeys, string[] filterValues, IOrganizationService service)
        {
            EntityCollection entityCollection = CommonXrm.RetrieveMultipleRecords(entityName, columns, filterKeys, filterValues, service);
            foreach (var item in entityCollection.Entities)
            {
                var entityToMarkAsPendingDelete = new Entity(entityName);
                entityToMarkAsPendingDelete.Id = item.Id;
                entityToMarkAsPendingDelete[Attributes.Booking.StateCode] = new OptionSetValue((int)Statecode.InActive);
                if (entityName == EntityName.BookingAccommodation)
                    entityToMarkAsPendingDelete[Attributes.Booking.StatusCode] = new OptionSetValue(950000007);
                else if (entityName == EntityName.BookingTransfer)
                    entityToMarkAsPendingDelete[Attributes.Booking.StatusCode] = new OptionSetValue(950000000);
                else if (entityName == EntityName.BookingTransport)
                    entityToMarkAsPendingDelete[Attributes.Booking.StatusCode] = new OptionSetValue(950000000);
                else if (entityName == EntityName.BookingExtraService)
                    entityToMarkAsPendingDelete[Attributes.Booking.StatusCode] = new OptionSetValue(950000001);
                else if (entityName == EntityName.CustomerBookingRole)
                    entityToMarkAsPendingDelete[Attributes.Booking.StatusCode] = new OptionSetValue(950000000);

                service.Update(entityToMarkAsPendingDelete);
            }
        }

        #region Option set mappings
        public static OptionSetValue GetCommunity(string text)
        {
            int value = -1;
            switch (text)
            {
                case "Facebook":
                    value = 1;
                    break;
                case "Google":
                    value = 0;
                    break;
                case "Twitter":
                    value = 2;
                    break;
                case "Other":
                    value = 0;
                    break;
                case "":
                case null:
                    value = 0;
                    break;
                default:
                    value = 0;
                    break;
            }

            return new OptionSetValue(value);
        }
        public static OptionSetValue GetExternalServiceCode(ExternalServiceCode externalServiceCode)
        {
            int value = -1;
            switch (externalServiceCode)
            {
                case ExternalServiceCode.NotSpecified:
                    value = 950000000;
                    break;
                case ExternalServiceCode.BedBank:
                    value = 950000001;
                    break;
                case ExternalServiceCode.GTA:
                    value = 950000002;
                    break;
                case ExternalServiceCode.HBSI:
                    value = 950000003;
                    break;
                case ExternalServiceCode.Hotel4You:
                    value = 950000004;
                    break;
                case ExternalServiceCode.IberostarBedBank:
                    value = 950000005;
                    break;
                case ExternalServiceCode.Juniper:
                    value = 950000007;
                    break;
                case ExternalServiceCode.SunHotels:
                    value = 950000008;
                    break;
                case ExternalServiceCode.Unknown:
                    value = 950000000;
                    break;
                default:
                    value = 950000000;
                    break;
            }
            return new OptionSetValue(value);
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public static OptionSetValue GetBoardType(BoardType boardType)
        {
            int value = -1;
            switch (boardType)
            {
                case BoardType.NotSpecified:
                    value = 950000003;
                    break;
                case BoardType.AllInclusive:
                    value = 950000000;
                    break;
                case BoardType.AllInclusivePlus:
                    value = 950000028;
                    break;
                case BoardType.AmericanBreakfast:
                    value = 950000004;
                    break;
                case BoardType.BedAndBreakfast:
                    value = 950000005;
                    break;
                case BoardType.BedEnglishBfast:
                    value = 950000006;
                    break;
                case BoardType.BoardAccordingToDescription:
                    value = 950000007;
                    break;
                case BoardType.Breakfast:
                    value = 950000008;
                    break;
                case BoardType.CateredChalet:
                    value = 950000009;
                    break;
                case BoardType.ClubBoard:
                    value = 950000010;
                    break;
                case BoardType.ContinentalBfast:
                    value = 950000011;
                    break;
                case BoardType.CruiseBoard:
                    value = 950000012;
                    break;
                case BoardType.DeluxeHalfBoard:
                    value = 950000013;
                    break;
                case BoardType.DineOut:
                    value = 950000014;
                    break;
                case BoardType.DrinksInclusive:
                    value = 950000015;
                    break;
                case BoardType.EveningMeal:
                    value = 950000016;
                    break;
                case BoardType.FlyDrive:
                    value = 950000017;
                    break;
                case BoardType.FullBoard:
                    value = 950000001;
                    break;
                case BoardType.FullBoardPlus:
                    value = 950000018;
                    break;
                case BoardType.HalfBoard:
                    value = 950000002;
                    break;
                case BoardType.HalfBoardUpgrade:
                    value = 950000020;
                    break;
                case BoardType.MealPlan:
                    value = 950000021;
                    break;
                case BoardType.NotApplicable:
                    value = 950000022;
                    break;
                case BoardType.RoomOnly:
                    value = 950000023;
                    break;
                case BoardType.Unknown:
                    value = 950000003;
                    break;
                case BoardType.ValueDiningPlan:
                    value = 950000025;
                    break;
                case BoardType.VariableBoard:
                    value = 950000026;
                    break;
                case BoardType.WithoutAny:
                    value = 950000027;
                    break;
                default:
                    value = 950000003;
                    break;
            }
            return new OptionSetValue(value);
        }
        public static OptionSetValue GetTransferServiceLevel(TransferServiceLevel transferServiceLevel)
        {
            int value = -1;
            switch (transferServiceLevel)
            {
                case TransferServiceLevel.NotSpecified:
                    value = 950000000;
                    break;
                case TransferServiceLevel.Differentiated:
                    value = 950000001;
                    break;
                case TransferServiceLevel.RegularComplementary:
                    value = 950000002;
                    break;
                default:
                    value = 950000000;
                    break;
            }
            return new OptionSetValue(value);
        }
        public static OptionSetValue GetAccommodationStatus(AccommodationStatus accommodationStatus)
        {
            int value = -1;
            switch (accommodationStatus)
            {
                case AccommodationStatus.NotSpecified:
                    value = 1;
                    break;
                case AccommodationStatus.OK:
                    value = 950000002;
                    break;
                case AccommodationStatus.Request:
                    value = 950000003;
                    break;
                case AccommodationStatus.PartialRequest:
                    value = 950000004;
                    break;
                default:
                    value = 1;
                    break;

            }

            return new OptionSetValue(value);
        }
        public static OptionSetValue GetCustomerStatus(CustomerStatus customerStatus)
        {
            int value = -1;
            switch (customerStatus)
            {
                case CustomerStatus.NotSpecified:
                    value = 950000002;
                    break;
                case CustomerStatus.Active:
                    value = 1;
                    break;
                case CustomerStatus.Blacklisted:
                    value = 950000003;
                    break;
                case CustomerStatus.Deceased:
                    value = 950000004;
                    break;
                case CustomerStatus.Inactive:
                    value = 950000005;
                    break;
                default:
                    value = 950000002;
                    break;
            }
            return new OptionSetValue(value);
        }
        public static OptionSetValue GetGender(Gender gender)
        {
            int value = -1;
            switch (gender)
            {
                case Gender.NotSpecified:
                    value = 950000002;
                    break;
                case Gender.Male:
                    value = 950000000;
                    break;
                case Gender.Female:
                    value = 950000001;
                    break;
                default:
                    value = 950000002;
                    break;
            }
            return new OptionSetValue(value);
        }
        public static OptionSetValue GetEmailType(EmailType emailType)
        {
            int value = -1;
            switch (emailType)
            {
                case EmailType.NotSpecified:
                    value = 950000002;
                    break;
                    
                case EmailType.Pri:
                    value = 950000000;
                    
                    break;
                case EmailType.Pro:
                    value = 950000001;
                    break;
                default:
                    value = 950000002;
                    break;
            }
            return new OptionSetValue(value);
        }
        public static OptionSetValue GetBookingStatus(BookingStatus bookingStatus)
        {
            int value = -1;
            switch (bookingStatus)
            {
                case BookingStatus.NotSpecified:
                    value = 1;
                    break;
                case BookingStatus.Booked:
                    value = 950000002;
                    break;
                case BookingStatus.Cancelled:
                    value = 950000003;
                    break;
                default:
                    value = 1;
                    break;
            }
            
            return new OptionSetValue(value);
        }
        public static OptionSetValue GetPhoneType(PhoneType phoneType)
        {
            int value = -1;
            switch (phoneType)
            {
                case PhoneType.NotSpecified:
                    value = 950000002;
                    break;
                case PhoneType.H:
                    value = 950000001;
                    break;
                case PhoneType.M:
                    value = 950000000;
                    break;
                default:
                    value = 950000002;
                    break;

            }
            return new OptionSetValue(value);
        }
        public static OptionSetValue GetTransferType(TransferType transferType)
        {
            int value = -1;
            switch (transferType)
            {
                case TransferType.NotSpecified:
                    value = 950000003;
                    break;
                case TransferType.Inbound:
                    value = 950000000;
                    break;
                case TransferType.Outbound:
                    value = 950000001;
                    break;
                case TransferType.TransferBetweenHotels:
                    value = 950000002;
                    break;
                default:
                    value = 950000003;
                    break;
            }
           
            return new OptionSetValue(value);
        }
        public static OptionSetValue GetSegment(string text)
        {
            int value = -1;
            switch (text)
            {
                case "1":
                    value = 950000000;
                    break;
                case "2":
                    value = 950000001;
                    break;
                case "3":
                    value = 950000002;
                    break;
                case "4":
                    value = 950000003;
                    break;
                case "5":
                    value = 950000004;
                    break;
                case "":
                case null:
                    value = 950000005;
                    break;
                default:
                    value = 950000005;
                    break;
            }

            return new OptionSetValue(value);
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public static OptionSetValue GetSalutation(string text)
        {
            int value = -1;
            switch (text)
            {
                case "Mr":
                    value = 950000000;
                    break;
                case "Mrs":
                    value = 950000001;
                    break;
                case "Ms":
                    value = 950000002;
                    break;
                case "Miss":
                    value = 950000003;
                    break;
                case "Dr":
                    value = 950000004;
                    break;
                case "Sir":
                    value = 950000005;
                    break;
                case "Prof.":
                    value = 950000006;
                    break;
                case "Lord":
                    value = 950000007;
                    break;
                case "Lady":
                    value = 950000008;
                    break;
                case "":
                case null:
                    value = 950000009;
                    break;
                default:
                    value = 950000009;
                    break;
            }

            return new OptionSetValue(value);
        }
        public static OptionSetValue GetLanguage(string text)
        {
            int value = -1;
            switch (text)
            {
                case "EN":
                    value = 950000000;
                    break;
                case "DE":
                    value = 950000001;
                    break;
                case "NL":
                    value = 950000002;
                    break;
                case "FR":
                    value = 950000003;
                    break;
                case "ES":
                    value = 950000004;
                    break;
                case "DA":
                    value = 950000005;
                    break;
                case "":
                case null:
                    value = 950000006;
                    break;
                default:
                    value = 950000006;
                    break;
            }

            return new OptionSetValue(value);
        }
        #endregion Option set mappings




        /// <summary>
        /// Call this method to create or update record
        /// </summary>
        /// <param name="entityRecord"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Upsert")]
        public static XrmResponse UpsertEntity(Entity entityRecord, IOrganizationService service)
        {

            XrmResponse xrmResponse = null;
            if (service != null)
            {
                UpsertRequest request = new UpsertRequest()
                {
                    Target = entityRecord
                };



                // Execute UpsertRequest and obtain UpsertResponse. 
                UpsertResponse response = (UpsertResponse)service.Execute(request);
                if (response.RecordCreated)
                    xrmResponse = new XrmResponse
                    {
                        Id = response.Target.Id.ToString(),
                        EntityName = entityRecord.LogicalName,
                        Create = true

                    };
                else
                    xrmResponse = new XrmResponse()
                    {
                        Id = response.Target.Id.ToString(),
                        EntityName = entityRecord.LogicalName,
                        Create = false
                    };


            }


            return xrmResponse;
        }


        /// <summary>
        /// Call this method for bulk create
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public static List<XrmResponse> BulkCreate(EntityCollection entities, IOrganizationService service)
        {
            List<XrmResponse> xrmResponseList = new List<XrmResponse>();
            if (service != null && entities != null && entities.Entities.Count > 0)
            {

                // Add a CreateRequest for each entity to the request collection.
                foreach (var entity in entities.Entities)
                {
                    var createRequest = new CreateRequest { Target = entity };
                    var response = (CreateResponse)service.Execute(createRequest);
                    var xrmResponse = new XrmResponse
                    {
                        Id = response.id.ToString(),
                        EntityName = response.ResponseName

                    };
                    xrmResponseList.Add(xrmResponse);
                }

            }

            return xrmResponseList;
        }


        /// <summary>
        /// Call this method for bulk delete
        /// </summary>
        /// <param name="entityReferences"></param>
        /// <param name="service"></param>
        public static void BulkDelete(DataCollection<EntityReference> entityReferences, IOrganizationService service)
        {
            if (service != null && entityReferences != null && entityReferences.Count > 0)
            {

                // Add a DeleteRequest for each entity to the request collection.
                foreach (var entityRef in entityReferences)
                {
                    var deleteRequest = new DeleteRequest { Target = entityRef };
                    service.Execute(deleteRequest);
                }

            }
        }

        /// <summary>
        /// To get records by using filter keys and values
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="columns"></param>
        /// <param name="filterKeys"></param>
        /// <param name="filterValues"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public static EntityCollection RetrieveMultipleRecords(string entityName, string[] columns, string[] filterKeys, string[] filterValues, IOrganizationService service)
        {
            var query = new QueryExpression(entityName);
            query.ColumnSet = new ColumnSet(columns);

            for (int i = 0; i < filterKeys.Length; i++)
            {
                if (filterValues[i] == null) continue;
                var condExpr = new ConditionExpression();
                condExpr.AttributeName = filterKeys[i];
                condExpr.Operator = ConditionOperator.Equal;
                condExpr.Values.Add(filterValues[i]);

                var fltrExpr = new FilterExpression(LogicalOperator.And);
                fltrExpr.AddCondition(condExpr);

                query.Criteria.AddFilter(fltrExpr);
            }
            return GetRecordsUsingQuery(query, service);

        }

        /// <summary>
        /// To get records using QueryExpression
        /// </summary>
        /// <param name="queryExpr"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        static EntityCollection GetRecordsUsingQuery(QueryExpression queryExpr, IOrganizationService service)
        {
            EntityCollection entityCollection = null;
            entityCollection = service.RetrieveMultiple(queryExpr);
            return entityCollection;
        }

        public static EntityCollection RetrieveMultipleRecordsFetchXml(string query, IOrganizationService service)
        {

            EntityCollection entityCollection = new EntityCollection();

            int fetchCount = 10000;
            int pageNumber = 1;
            string pagingCookie = null;

            while (true)
            {
                string xml = CreateXml(query, pagingCookie, pageNumber, fetchCount);
                FetchExpression fetch = new FetchExpression(xml);
                EntityCollection returnCollection = service.RetrieveMultiple(fetch);
                entityCollection.Entities.AddRange(returnCollection.Entities);
                if (returnCollection.MoreRecords)
                {
                    pageNumber++;
                }
                else
                {
                    break;
                }
            }

            return entityCollection;

        }


        public static string CreateXml(string xml, string cookie, int page, int count)
        {

            StringReader stringReader = new StringReader(xml);
            XmlTextReader reader = new XmlTextReader(stringReader);

            // Load document
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            return CreateXml(doc, cookie, page, count);
        }


        public static string CreateXml(XmlDocument doc, string cookie, int page, int count)
        {

            if (doc == null) throw new ArgumentNullException("doc");
            XmlAttributeCollection attrs = doc.DocumentElement.Attributes;

            if (cookie != null)
            {
                XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
                pagingAttr.Value = cookie;
                attrs.Append(pagingAttr);
            }

            XmlAttribute pageAttr = doc.CreateAttribute("page");
            pageAttr.Value = System.Convert.ToString(page, CultureInfo.CurrentCulture);
            attrs.Append(pageAttr);

            XmlAttribute countAttr = doc.CreateAttribute("count");
            countAttr.Value = System.Convert.ToString(count, CultureInfo.CurrentCulture);
            attrs.Append(countAttr);

            StringBuilder sb = new StringBuilder(1024);
            StringWriter stringWriter = new StringWriter(sb, CultureInfo.CurrentCulture);

            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            doc.WriteTo(writer);
            writer.Close();

            return sb.ToString();
        }




    }

    public class XrmResponse
    {
        public bool Create { get; set; }
        public string EntityName { get; set; }
        public string Id { get; set; }
        public string Details { get; set; }
        public string Key { get; set; }

    }

}
