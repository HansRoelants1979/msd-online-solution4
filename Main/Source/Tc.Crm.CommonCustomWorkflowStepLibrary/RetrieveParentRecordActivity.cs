using System;
using System.Activities;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using System.Collections.Generic;

namespace Tc.Crm.CommonCustomWorkflowStepLibrary
{
    public class RetrieveParentRecordActivity : CodeActivity
    {

        protected override void Execute(CodeActivityContext executionContext)
        {
            //Create the tracing service
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            try
            {
                var sourceEntity = SourceEntity.Get<EntityReference>(executionContext);
                var targetEntity = TargetEntity.Get<string>(executionContext);
                var relationshipName = RelationshipName.Get<string>(executionContext);
                ParentRecord.Set(executionContext, RetrieveParentRecord(sourceEntity, targetEntity, relationshipName, service));

            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new InvalidPluginExecutionException(ex.ToString());
            }
            catch (TimeoutException ex)
            {
                throw new InvalidPluginExecutionException(ex.ToString());
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.ToString());
            }

        }

        static EntityReference RetrieveParentRecord(EntityReference SourceEntity, string TargetEntity, string RelationshipName, IOrganizationService service)
        {
            EntityReference parentEntity = null;
            //EntityCollection ParentEntityRecords = null;
            List<Entity> ParentEntityRecords = new List<Entity>();

            //the related entity we are going to retrieve
            QueryExpression query = new QueryExpression();
            query.EntityName = TargetEntity;
            query.ColumnSet = new ColumnSet();

            //the relationship that links the primary to the target
            Relationship relationship = new Relationship(RelationshipName);
            relationship.PrimaryEntityRole = EntityRole.Referenced; //important if the relationship is self-referencing

            //the query collection which forms the request
            RelationshipQueryCollection relatedEntity = new RelationshipQueryCollection();
            relatedEntity.Add(relationship, query);

            //the request to get the primary entity with the related records
            RetrieveRequest request = new RetrieveRequest();
            request.RelatedEntitiesQuery = relatedEntity;
            request.ColumnSet = new ColumnSet();
            request.Target = new EntityReference(SourceEntity.LogicalName, SourceEntity.Id);

            RetrieveResponse r = (RetrieveResponse)service.Execute(request);

            //query the returned collection for the target entity 
            ParentEntityRecords = r.Entity.RelatedEntities[relationship].Entities.Select(e => e).ToList();
            
            return parentEntity = new EntityReference(TargetEntity,ParentEntityRecords[0].Id );
        }
        [Input("EntityReference SourceEntity")]
        public InArgument<EntityReference> SourceEntity { get; set; }

        [Input("String TargetEntity")]
        public InArgument<string> TargetEntity { get; set; }

        [Input("String RelationshipName")]
        public InArgument<string> RelationshipName { get; set; }

        [Output("EntityReference ParentRecord")]
        public OutArgument<EntityReference> ParentRecord { get; set; }



    }
}
