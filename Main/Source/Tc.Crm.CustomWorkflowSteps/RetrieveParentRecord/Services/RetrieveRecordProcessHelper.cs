using System;
using System.Activities;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using System.Collections.Generic;


namespace Tc.Crm.CustomWorkflowSteps.RetrieveParentRecord.Services
{
    class RetrieveRecordProcessHelper
    {
        public static EntityReference RetrieveParentRecord(string expression, IOrganizationService service, IWorkflowContext context)
        {
            if (expression == null)
                throw new InvalidPluginExecutionException("Expression is null");
            if (service == null)
                throw new InvalidPluginExecutionException("service is null");
            if (context == null)
                throw new InvalidPluginExecutionException("context is null");

            string SourceId = string.Empty;
            EntityReference ReturnValue = null;
            
            #region ParseString

            string s = expression;
            string[] separators = { "||" };
            string[] words = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            #endregion ParseString
            #region RetrievParentRecord
            for (var i = 0; i < words.Length - 1; i++)
            {

                List<Entity> ParentEntityRecords = new List<Entity>();

                //the related entity we are going to retrieve
                QueryExpression query = new QueryExpression();

                query.EntityName = words[i + 1].Substring(words[i + 1].IndexOf(";") + 1);

                query.ColumnSet = new ColumnSet();

                //check relationship Exist or not in the CRM system
                RelationshipExistOrNot(words[i + 1].Substring(0, words[i + 1].IndexOf(";")), service);
                //the relationship that links the primary to the target
                Relationship relationship = new Relationship(words[i + 1].Substring(0, words[i + 1].IndexOf(";")));



                relationship.PrimaryEntityRole = EntityRole.Referenced; //important if the relationship is self-referencing

                //the query collection which forms the request
                RelationshipQueryCollection relatedEntity = new RelationshipQueryCollection();
                relatedEntity.Add(relationship, query);

                //the request to get the primary entity with the related records
                RetrieveRequest request = new RetrieveRequest();
                request.RelatedEntitiesQuery = relatedEntity;
                request.ColumnSet = new ColumnSet();
                if (i == 0)
                {
                    if (context.PrimaryEntityName != words[i].Substring(words[i].IndexOf(";") + 1))
                        throw new InvalidPluginExecutionException("First Entity name should be the name of workflow execution entity ");

                    request.Target = new EntityReference(context.PrimaryEntityName, context.PrimaryEntityId);
                }
                else
                {
                    request.Target = new EntityReference(words[i].Substring(words[i].IndexOf(";") + 1), new Guid(SourceId));
                }

                RetrieveResponse r = (RetrieveResponse)service.Execute(request);

                //query the returned collection for the target entity ids
                ParentEntityRecords = r.Entity.RelatedEntities[relationship].Entities.Select(e => e).ToList();
                if (ParentEntityRecords.Count > 0)
                {
                    SourceId = ParentEntityRecords[0].Id.ToString();


                    if (i == (words.Length - 2))
                    {

                        ReturnValue = new EntityReference(words[i + 1].Substring(words[i + 1].IndexOf(";") + 1), ParentEntityRecords[0].Id);
                    }


                }

            }


            return ReturnValue;
            #endregion RetrievParentRecord
        }
        static void RelationshipExistOrNot(string relationshipname, IOrganizationService service)
        {
            try
            {

                RetrieveRelationshipRequest retrieveManyToOneRequest = new RetrieveRelationshipRequest { Name = relationshipname };
                service.Execute(retrieveManyToOneRequest);

            }
            catch (Exception)
            {
                throw new InvalidPluginExecutionException("" + relationshipname + " relationship is not present in the CRM system");
            }
        }
    }
}
