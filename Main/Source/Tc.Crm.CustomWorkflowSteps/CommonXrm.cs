using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System.IO;
using System.Text;
using System.Globalization;
using System.Xml;
using System;

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
        public static void DeleteRecords(string entityName, string[] columns, string[] filterKeys, string[] filterValues,IOrganizationService service)
        {
            EntityCollection entityCollection = CommonXrm.RetrieveMultipleRecords(entityName, columns, filterKeys, filterValues, service);
            if (entityCollection != null && entityCollection.Entities.Count > 0)
            {
                EntityReferenceCollection entityReferenceCollection = new EntityReferenceCollection();
                foreach (Entity entity in entityCollection.Entities)
                {
                    entityReferenceCollection.Add(new EntityReference(entity.LogicalName, entity.Id));
                }
                CommonXrm.BulkDelete(entityReferenceCollection, service);
            }
        }

        /// <summary>
        /// To get OptionSetValue by text and entityName
        /// </summary>
        /// <param name="text"></param>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public static OptionSetValue GetOptionSetValue(string text, string optionsetName)
        {
            int value = -1;
            switch (optionsetName)
            {
                case "tc_language":
                    {
                        switch (text)
                        {
                            case "English":
                                value = 950000000;
                                break;
                            case "German":
                                value = 950000001;
                                break;
                            case "Dutch":
                                value = 950000002;
                                break;
                            case "French":
                                value = 950000003;
                                break;
                            case "Spanish":
                                value = 950000004;
                                break;
                            case "Danish":
                                value = 950000005;
                                break;

                        }
                    }
                    break;

                case "tc_gender":
                    {
                        switch (text)
                        {
                            case "Male":
                                value = 950000000;
                                break;
                            case "Female":
                                value = 950000001;
                                break;
                            case "Unknown":
                                value = 950000002;
                                break;
                        }
                    }
                    break;
                case "tc_segment":
                    {
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

                        }
                    }
                    break;
                case "tc_salutation":
                    {
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
                        }
                    }
                    break;
                case "tc_type":
                    {
                        switch (text)
                        {
                            case "T":
                                value = 950000000;
                                break;
                            case "A":
                                value = 950000001;
                                break;
                        }
                    }
                    break;
                case "tc_transfertype":
                    {
                        switch (text)
                        {
                            case "I":
                                value = 950000000;
                                break;
                            case "IN":
                                value = 950000000;
                                break;
                            case "O":
                                value = 950000001;
                                break;
                            case "TH":
                                value = 950000002;
                                break;
                        }
                    }
                    break;
                case "tc_transferservicelevel":
                    {
                        switch (text)
                        {
                            case "Service Level 1":
                                value = 950000000;
                                break;
                            case "Service Level 2":
                                value = 950000001;
                                break;
                            case "Service Level 3":
                                value = 950000002;
                                break;
                        }
                    }
                    break;
                case "tc_externalservicecode":
                    {
                        switch (text)
                        {
                            case "Service Code A":
                                value = 950000000;
                                break;
                            case "Service Code B":
                                value = 950000001;
                                break;
                            case "Service Code C":
                                value = 950000002;
                                break;
                        }
                    }
                    break;
                case "tc_boardtype":
                    {
                        switch (text)
                        {
                            case "AI":
                                value = 950000000;
                                break;
                            case "HB":
                                value = 950000002;
                                break;
                            case "FB":
                                value = 950000001;
                                break;
                        }
                    }
                    break;
                case "tc_emailaddress1type":
                case "tc_emailaddress2type":
                case "tc_emailaddress3type":
                case "tc_emailaddress1_type":
                case "tc_emailaddress2_type":
                case "tc_emailaddress3_type":
                    {
                        switch (text)
                        {
                            case "Pri":
                                value = 950000000;
                                break;
                            case "Pro":
                                value = 950000001;
                                break;

                        }
                    }
                    break;
                case "tc_telephone1type":
                case "tc_telephone2type":
                case "tc_telephone3type":
                case "tc_telephone1_type":
                case "tc_telephone2_type":
                case "tc_telephone3_type":   
                    {
                        switch (text)
                        {
                            case "H":
                                value = 950000001;
                                break;
                            case "M":
                                value = 950000000;
                                break;

                        }
                    }
                    break;
                case "statuscode":
                    {
                        switch (text)
                        {
                            case "A":
                                value = 1;
                                break;
                            case "B":
                                value = 950000001;
                                break;
                            case "C":
                                value = 950000000;
                                break;
                            case "D":
                                value = 950000000;
                                break;
                            case "Inactive":
                                value = 2;
                                break;
                            case "OK":
                                value = 2;
                                break;
                            case "PR":
                                value = 950000001;
                                break;
                            case "RQ":
                                value = 950000000;
                                break;
                            case "Booked":
                                value = 950000001;
                                break;
                            case "Cancelled":
                                value = 950000000;
                                break;

                        }
                    }
                    break;
                case "community":
                    {
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


                        }
                    }
                    break;
            }
            return (value != -1) ? new OptionSetValue(value) : null;
        }

        /// <summary>
        /// Call this method to create or update record
        /// </summary>
        /// <param name="entityRecord"></param>
        /// <param name="service"></param>
        /// <returns></returns>
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
                    var response =  (CreateResponse)service.Execute(createRequest);
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
