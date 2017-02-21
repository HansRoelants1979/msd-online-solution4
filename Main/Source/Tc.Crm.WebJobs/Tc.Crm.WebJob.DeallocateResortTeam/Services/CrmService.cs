using System;
using System.Text;
using System.Xml;
using System.IO;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;

namespace Tc.Crm.WebJob.DeallocateResortTeam.Services
{
    public class CrmService : ICrmService
    {
        IOrganizationService organizationService;
        IConfigurationService configurationService;
        ILogger logger;

        public CrmService(IConfigurationService configurationService, ILogger logger)
        {
            this.configurationService = configurationService;
            this.organizationService = GetOrganizationService();
            this.logger = logger;
        }



        public EntityCollection RetrieveMultipleRecords(string entityName, string[] columns, string[] filterKeys, string[] filterValues)
        {
            var query = new QueryExpression(entityName);
            query.ColumnSet = new ColumnSet(columns);


            for (int i = 0; i < filterKeys.Length; i++)
            {
                var condExpr = new ConditionExpression();
                condExpr.AttributeName = filterKeys[i];
                condExpr.Operator = ConditionOperator.Equal;
                condExpr.Values.Add(filterValues[i]);

                var fltrExpr = new FilterExpression(LogicalOperator.And);
                fltrExpr.AddCondition(condExpr);

                query.Criteria.AddFilter(fltrExpr);
            }
            return GetRecordsUsingQuery(query);

        }



        public EntityCollection RetrieveMultipleRecordsFetchXml(string query)
        {
            EntityCollection entityCollection = new EntityCollection();
            //paging has to be implimented

            //<snippetFetchPagingWithCookie1>
            // Define the fetch attributes.
            // Set the number of records per page to retrieve.
            int fetchCount = 4;
            // Initialize the page number.
            int pageNumber = 1;
            // Specify the current paging cookie. For retrieving the first page, 
            // pagingCookie should be null.
            string pagingCookie = null;

            while (true)
            {
                // Build fetchXml string with the placeholders.
                string xml = CreateXml(query, pagingCookie, pageNumber, fetchCount);
                FetchExpression fetch = new FetchExpression(xml);
                EntityCollection returnCollection = organizationService.RetrieveMultiple(fetch);
                entityCollection.Entities.AddRange(returnCollection.Entities);
                // Check for morerecords, if it returns 1.
                if (returnCollection.MoreRecords)
                {
                    // Increment the page number to retrieve the next page.
                    pageNumber++;

                    // Set the paging cookie to the paging cookie returned from current results.
                    //Commented as we are getting incorrect cookie value                            
                    //pagingCookie = returnCollection.PagingCookie;
                }
                else
                {
                    // If no more records in the result nodes, exit the loop.
                    break;
                }
            }
            return entityCollection;

        }

        public string CreateXml(string xml, string cookie, int page, int count)
        {
            StringReader stringReader = new StringReader(xml);
            XmlTextReader reader = new XmlTextReader(stringReader);

            // Load document
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            return CreateXml(doc, cookie, page, count);
        }

        public string CreateXml(XmlDocument doc, string cookie, int page, int count)
        {
            XmlAttributeCollection attrs = doc.DocumentElement.Attributes;

            if (cookie != null)
            {
                XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
                pagingAttr.Value = cookie;
                attrs.Append(pagingAttr);
            }

            XmlAttribute pageAttr = doc.CreateAttribute("page");
            pageAttr.Value = System.Convert.ToString(page);
            attrs.Append(pageAttr);

            XmlAttribute countAttr = doc.CreateAttribute("count");
            countAttr.Value = System.Convert.ToString(count);
            attrs.Append(countAttr);

            StringBuilder sb = new StringBuilder(1024);
            StringWriter stringWriter = new StringWriter(sb);

            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            doc.WriteTo(writer);
            writer.Close();

            return sb.ToString();
        }

        public EntityCollection GetRecordsUsingQuery(QueryExpression queryExpr)
        {
            int pageNumber = 1;
            int recordCount = 1;
            queryExpr.PageInfo = new PagingInfo();
            queryExpr.PageInfo.PageNumber = pageNumber;
            queryExpr.PageInfo.Count = recordCount;
            queryExpr.PageInfo.PagingCookie = null;
            EntityCollection entityCollection = null;
            while (true)
            {
                entityCollection = organizationService.RetrieveMultiple(queryExpr);

                // Check for more records, if it returns true.
                if (entityCollection.MoreRecords)
                {
                    // Increment the page number to retrieve the next page.
                    queryExpr.PageInfo.PageNumber++;

                    // Set the paging cookie to the paging cookie returned from current results.
                    queryExpr.PageInfo.PagingCookie = entityCollection.PagingCookie;
                }
                else
                {
                    // If no more records are in the result nodes, exit the loop.
                    break;
                }

            }

            return entityCollection;
        }


        public ExecuteMultipleResponse BulkUpdate(EntityCollection entityCollection)
        {
            ExecuteMultipleRequest request = new ExecuteMultipleRequest()
            {
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };

            for (int i = 0; i < entityCollection.Entities.Count; i++)
        {
                request.Requests.Add(new UpdateRequest() { Target = entityCollection[i] });
            }

            return (ExecuteMultipleResponse)organizationService.Execute(request);
        }



        public IOrganizationService GetOrganizationService()
        {
            if (organizationService != null) return organizationService;

            var connectionString = configurationService.ConnectionString;
            CrmServiceClient client = new CrmServiceClient(connectionString);
            return (IOrganizationService)client;
        }




        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                DisposeObject(organizationService);
                DisposeObject(configurationService);
                DisposeObject(logger);
            }

            disposed = true;
        }

        void DisposeObject(Object obj)
        {
            if (obj != null)
            {
                if (obj is IDisposable)
                    ((IDisposable)obj).Dispose();
                else
                    obj = null;
            }

        }
    }

    public static class EntityName
    {

        public const string Booking = "tc_booking";
        public const string BookingAccommodation = "tc_bookingaccommodation";
        public const string CustomerBookingRole = "tc_customerbookingrole";
        public const string Hotel = "tc_hotel";
        public const string Team = "team";
        public const string User = "systemuser";

    }

    public static class Attributes
    {
        public class Booking
        {
            public const string BookingId = "tc_bookingid";
            public const string Owner = "ownerid";
            public const string Name = "tc_name";
            public const string DepartureDate = "tc_departuredate";
            public const string DestinationId = "tc_destinationid";
            public const string ReturnDate = "tc_returndate";
        }

        public class BookingAccommodation
        {
            public const string BookingAccommodationid = "tc_bookingaccommodationid";
            public const string BookingId = "tc_bookingid";
            public const string EndDateandTime = "tc_enddateandtime";
            public const string HotelId = "tc_hotelid";
            public const string NumberofParticipants = "tc_numberofparticipants";
            public const string NumberofRooms = "tc_numberofrooms";
            public const string SourceMarketHotelCode = "tc_sourcemarkethotelcode";
            public const string SourceMarketHotelName = "tc_sourcemarkethotelname";
            public const string StartDateandTime = "tc_startdateandtime";
            public const string Owner = "ownerid";

        }

        public class CustomerBookingRole
        {
            public const string BookingId = "tc_bookingid";
            public const string Customer = "tc_customer";
            public const string Role = "tc_customerbookingrole";
            public const string CustomerBookingRoleId = "tc_customerbookingroleid";
            public const string Name = "tc_name";
        }

        }
    }

    public static class EntityName
    {
        public const string Booking = "tc_booking";
        public const string BookingAccommodation = "tc_bookingaccommodation";
        public const string CustomerBookingRole = "tc_customerbookingrole";
        public const string Hotel = "tc_hotel";
        public const string Team = "team";
        public const string User = "systemuser";
        public const string Account = "account";
        public const string Contact = "contact";
    }

    public static class Attributes
    {
        public class Booking
        {
            public const string BookingId = "tc_bookingid";
            public const string Owner = "ownerid";
            public const string Name = "tc_name";
            public const string DepartureDate = "tc_departuredate";
            public const string DestinationId = "tc_destinationid";
            public const string ReturnDate = "tc_returndate";
        }

        public class BookingAccommodation
        {
            public const string BookingAccommodationid = "tc_bookingaccommodationid";
            public const string BookingId = "tc_bookingid";
            public const string EndDateandTime = "tc_enddateandtime";
            public const string HotelId = "tc_hotelid";
            public const string StartDateandTime = "tc_startdateandtime";
            public const string Owner = "ownerid";

        }

        public class CustomerBookingRole
        {
            public const string BookingId = "tc_bookingid";
            public const string Customer = "tc_customer";
            public const string Role = "tc_customerbookingrole";
            public const string CustomerBookingRoleId = "tc_customerbookingroleid";
            public const string Name = "tc_name";
        }

        public class Hotel
        {

            public const string HotelId = "tc_hotelid";
            public const string LocationId = "tc_locationid";
            public const string MasterHotelID = "tc_masterhotelid";
            public const string Name = "tc_name";
            public const string SourceMarketHotelID = "tc_sourcemarkethotelid";
            public const string ResortTeam = "tc_teamid";
            public const string Owner = "ownerid";

        }
        public class Team
        {
            public const string TeamId = "teamid";
            public const string Name = "name";
        }

        public class Customer
        {
            public const string Owner = "ownerid";
        }
    }
}
