using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace Tc.Crm.CustomWorkflowSteps.DistributeByRelated.Service
{
    public class DistributeByRelatedService
    {
        public static IList<Guid> GatherOneToManyKeys(IOrganizationService service, OneToManyRelationshipMetadata relationship, Guid currentRecordId)
        {
            var query = new QueryByAttribute()
            {
                EntityName = relationship.ReferencingEntity
            };
            query.Attributes.Add(relationship.ReferencingAttribute);
            query.Values.Add(currentRecordId.ToString());

            var retrieveRequest = new RetrieveMultipleRequest()
            {
                Query = query
            };

            // Foreach object just get the primary key
            var retrieveResponse = (RetrieveMultipleResponse)service.Execute(retrieveRequest);
            var keyList = new List<Guid>();
            foreach (var entity in retrieveResponse.EntityCollection.Entities)
            {
                keyList.Add(entity.Id);
            }

            return keyList;
        }

        /// <summary>
        /// Returns list of keys from many to many relationship
        /// </summary>
        /// <param name="service"></param>
        /// <param name="relationship"></param>
        /// <param name="currentRecordId"></param>
        /// <param name="currentEntityName"></param>
        /// <returns></returns>
        public static IList<Guid> GatherManyToManyKeys(IOrganizationService service, ManyToManyRelationshipMetadata relationship, Guid currentRecordId, string currentEntityName)
        {
            string intersection = relationship.IntersectEntityName;
            if (relationship.Entity1LogicalName == currentEntityName && relationship.Entity2LogicalName == currentEntityName)
            {
                // N:N on the same entity
                var list1 = GatherKeysInternal(service,
                    relationship.Entity1IntersectAttribute,
                    relationship.Entity2LogicalName,
                    relationship.Entity2IntersectAttribute,
                    intersection,
                    currentEntityName, currentRecordId);
                var list2 = GatherKeysInternal(service,
                    relationship.Entity2IntersectAttribute,
                    relationship.Entity1LogicalName,
                    relationship.Entity1IntersectAttribute,
                    intersection,
                    currentEntityName, currentRecordId);

                var list = new HashSet<Guid>();
                foreach (var key in list1)
                {
                    if (object.Equals(key, currentRecordId))
                    {
                        // exclude this own entity
                    }
                    else
                    {
                        list.Add(key);
                    }
                }
                foreach (var key in list2)
                {
                    if (object.Equals(key, currentRecordId))
                    {
                        // exclude this own entity
                    }
                    else if (list.Contains(key))
                    {
                        // already there (?)
                    }
                    else
                    {
                        list.Add(key);
                    }
                }
                return list.ToList<Guid>();
            }
            else if (relationship.Entity1LogicalName == currentEntityName)
            {
                // entity1 is primary
                return GatherKeysInternal(service,
                    relationship.Entity1IntersectAttribute,
                    relationship.Entity2LogicalName,
                    relationship.Entity2IntersectAttribute,
                    intersection, currentEntityName, currentRecordId);
            }
            else
            {
                // entity2 is primary
                return GatherKeysInternal(service,
                    relationship.Entity2IntersectAttribute,
                    relationship.Entity1LogicalName,
                    relationship.Entity1IntersectAttribute,
                    intersection, currentEntityName, currentRecordId);
            }
        }

        /// <summary>
        /// Internal many to many relationship key gathering helper
        /// </summary>
        /// <param name="service"></param>
        /// <param name="primaryAttribute"></param>
        /// <param name="secondaryEntity"></param>
        /// <param name="secondaryAttribute"></param>
        /// <param name="intersection"></param>
        /// <param name="currentEntityName"></param>
        /// <param name="currentRecordId"></param>
        /// <returns></returns>
        private static IList<Guid> GatherKeysInternal(IOrganizationService service, string primaryAttribute, string secondaryEntity, string secondaryAttribute, string intersection, string currentEntityName, Guid currentRecordId)
        {
            var query = new QueryExpression();
            var secondaryToIntersection = new LinkEntity();
            var intersectionToPrimary = new LinkEntity();
            var primaryCondition = new ConditionExpression();

            // Chain all links
            query.EntityName = secondaryEntity;
            query.LinkEntities.Add(secondaryToIntersection);
            secondaryToIntersection.LinkEntities.Add(intersectionToPrimary);
            intersectionToPrimary.LinkCriteria.Conditions.Add(primaryCondition);

            // First link
            secondaryToIntersection.LinkToEntityName = intersection;
            secondaryToIntersection.LinkFromAttributeName =
            secondaryToIntersection.LinkToAttributeName = secondaryAttribute;

            // Second link
            intersectionToPrimary.LinkToEntityName = currentEntityName;
            intersectionToPrimary.LinkFromAttributeName =
            intersectionToPrimary.LinkToAttributeName = primaryAttribute;

            // Condition
            primaryCondition.AttributeName = primaryAttribute;
            primaryCondition.Operator = ConditionOperator.Equal;
            primaryCondition.Values.Add(currentRecordId.ToString());

            var retrieveRequest = new RetrieveMultipleRequest()
            {
                Query = query
            };

            var list = new List<Guid>();
            var retrieveResponse = (RetrieveMultipleResponse)service.Execute(retrieveRequest);
            foreach (var entity in retrieveResponse.EntityCollection.Entities)
            {
                list.Add(entity.Id);
            }

            return list;
        }

        /// <summary>
        /// Returns OneToManyRelationshipMetadata by relationship name
        /// </summary>
        /// <param name="service"></param>
        /// <param name="relationshipName"></param>
        /// <returns></returns>
        public static OneToManyRelationshipMetadata GetOneToManyRelationship(IOrganizationService service, string relationshipName)
        {
            var relationshipRequest = new RetrieveRelationshipRequest()
            {
                Name = relationshipName,
                RetrieveAsIfPublished = false
            };

            var relationshipResponse = (RetrieveRelationshipResponse)service.Execute(relationshipRequest);
            if (!(relationshipResponse.RelationshipMetadata is OneToManyRelationshipMetadata))
            {
                throw new InvalidWorkflowException("Relationship is not One to Many");
            }

            return (OneToManyRelationshipMetadata)relationshipResponse.RelationshipMetadata;
        }

        /// <summary>
        /// Returns ManyToManyRelationshipMetadata by relationship name
        /// </summary>
        /// <param name="service"></param>
        /// <param name="relationshipName"></param>
        /// <returns></returns>
        public static ManyToManyRelationshipMetadata GetManyToManyRelationship(IOrganizationService service, string relationshipName)
        {
            var relationshipRequest = new RetrieveRelationshipRequest()
            {
                Name = relationshipName,
                RetrieveAsIfPublished = false
            };

            var relationshipResponse = (RetrieveRelationshipResponse)service.Execute(relationshipRequest);
            if (!(relationshipResponse.RelationshipMetadata is ManyToManyRelationshipMetadata))
            {
                throw new InvalidWorkflowException("Relationship is not Many to Many");
            }
            return (ManyToManyRelationshipMetadata)relationshipResponse.RelationshipMetadata;
        }
    }
}
