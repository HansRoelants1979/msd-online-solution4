using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Tc.Crm.Plugins.MultipleEntities.Model;
using Tc.Crm.Plugins.MultipleEntities.Helper;
using Tc.Crm.Plugins.OptionSetValues;


namespace Tc.Crm.Plugins.MultipleEntities.BusinessLogic
{
    public abstract class CreateEntityCacheOnEntityOperationService
    {
        public IPluginExecutionContext context = null;
        public IOrganizationService service = null;
        public ITracingService trace = null;
        public string[] serviceAccountsToIgnore;

       
        /// <summary>
        /// To assign plugin parameters of context, service and trace
        /// </summary>
        /// <param name="context"></param>
        /// <param name="trace"></param>
        /// <param name="service"></param>
        private void AssignPluginParameters(IPluginExecutionContext context, ITracingService trace, IOrganizationService service)
        {
            this.context = context;
            this.service = service;
            this.trace = trace;
        }
                   

        public abstract void SetEntityParameters(Entity sourceEntity, Entity targetEntity);  
        
        /// <summary>
        /// To get post entity image from context
        /// </summary>
        /// <returns></returns>
        public virtual Entity GetEntityImage()
        {
            trace.Trace("GetEntityImage - Start");
            Entity postEntityImage = null;
            if (context.PostEntityImages == null || context.PostEntityImages.Count == 0 || !context.PostEntityImages.Contains(PostImageName) || context.PostEntityImages[PostImageName] == null)
                return postEntityImage;            
             postEntityImage = (Entity)context.PostEntityImages[PostImageName];
            trace.Trace("GetEntityImage - End");
            return postEntityImage;
        }
       
        /// <summary>
        /// To execute logic while creating or updating a customer record
        /// </summary>
        public void DoActionsOnEntityOperation(IPluginExecutionContext context, ITracingService trace, IOrganizationService service)
        {
            trace.Trace("DoActionsOnEntityOperation - Start");
            AssignPluginParameters(context, trace, service);
            if (context.InputParameters.Contains(InputParameters.Target) && context.InputParameters[InputParameters.Target] is Entity)
            {
                trace.Trace("Contains Input Parameters 'Target' as Entity");
                var entity = context.InputParameters[InputParameters.Target] as Entity;
                if (context.MessageName.Equals(Messages.Create, StringComparison.OrdinalIgnoreCase))
                    DoActionsOnEntityCreate(entity);
                else if (context.MessageName.Equals(Messages.Update, StringComparison.OrdinalIgnoreCase))
                    DoActionsOnEntityUpdate(entity);                               
            }
            trace.Trace("DoActionsOnEntityOperation - End");
        }

        /// <summary>
        /// To execute logic while creating a record
        /// </summary>
        /// <param name="primaryEntity"></param>
        private void DoActionsOnEntityCreate(Entity primaryEntity)
        {
            if (primaryEntity == null) return;
            trace.Trace("DoActionsOnEntityCreate - Start");
            var entityCache = PrepareEntityCache(primaryEntity);
            SetEntityParameters(primaryEntity, entityCache);
            entityCache.Attributes[Attributes.EntityCache.Data] = JsonHelper.SerializeJson(GetData(primaryEntity));
            service.Create(entityCache);
            trace.Trace("DoActionsOnEntityCreate - End");
        }

        /// <summary>
        /// To execute logic while updating a record
        /// </summary>
        /// <param name="primaryEntity"></param>
        private void DoActionsOnEntityUpdate(Entity primaryEntity)
        {
            if (primaryEntity == null) return;
            trace.Trace("DoActionsOnEntityUpdate - Start");
            var entityCache = PrepareEntityCache(primaryEntity);
            SetEntityParameters(primaryEntity, entityCache);
            var entityModel = GetData(primaryEntity);
            var entityImage = GetEntityImage();
            if (entityImage != null && entityImage.Attributes.Count > 0)
                entityModel.Fields.AddRange(GetData(entityImage).Fields);
            entityCache.Attributes[Attributes.EntityCache.Data] = JsonHelper.SerializeJson(entityModel);
			// set Pending status reason if needed
			CheckPendingStatus(entityCache, primaryEntity);
            service.Create(entityCache);
            trace.Trace("DoActionsOnEntityUpdate - End");
        }

		private void CheckPendingStatus(Entity entityCache, Entity primaryEntity)
		{
			var query = $@"<fetch version='1.0' output-format='xml-platform' distinct='false' mapping='logical' aggregate='true'>
							<entity name = 'tc_entitycache'>
								<attribute name = 'tc_entitycacheid' alias = 'totalCount' aggregate = 'count' />
								<filter type = 'and'>
									<condition attribute = 'statuscode' operator= 'neq' value = '{(int)EntityCacheStatusReason.Succeeded}' />
									<condition attribute = 'tc_recordid' operator= 'eq' value = '{primaryEntity.Id}' />
								</filter>
							</entity>
						</fetch>";
			var fetch = new FetchExpression(query);
			EntityCollection countResult = service.RetrieveMultiple(new FetchExpression(query));
			var count = (Int32)((AliasedValue)countResult.Entities[0]["totalCount"]).Value;
			trace.Trace($"Existing non-successfull EntityCache related records count:{count}.");
			if (count > 0)
			{
				entityCache[Attributes.EntityCache.StatusCode] = new OptionSetValue((int)EntityCacheStatusReason.Pending);
				trace.Trace("EntityCache record will be created with Pending status.");
			}
		}

		/// <summary>
		/// To prepare entitycache record
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		private Entity PrepareEntityCache(Entity entity)
        {
            trace.Trace("PrepareEntityCache - Start");
            var entityCache = new Entity(Entities.EntityCache);
            entityCache.Attributes[Attributes.EntityCache.RecordId] = entity.Id.ToString();
            entityCache.Attributes[Attributes.EntityCache.Type] = entity.LogicalName;
            entityCache.Attributes[Attributes.EntityCache.Operation] = GetOperationValue(context.MessageName);
            trace.Trace("PrepareEntityCache - End");
            return entityCache;
        }

        /// <summary>
        /// To get OptionSetValue of Operation picklist
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private OptionSetValue GetOperationValue(string message)
        {
            trace.Trace("GetOperationValue - Start");
            int value = -1;
            switch (message)
            {
                case Messages.Create:
                    value = Operation.Create;
                    break;
                case Messages.Update:
                    value = Operation.Update;
                    break;               
                default:
                    return null;                   
            }
            trace.Trace("GetOperationValue - End");
            return new OptionSetValue(value);
        }

        /// <summary>
        /// To get ISO2 Code of source market
        /// </summary>
        /// <param name="sourceMarketId"></param>
        /// <returns></returns>
        protected string GetSourceMarketISO2Code(Guid sourceMarketId)
        {
            trace.Trace("GetSourceMarketISO2Code - Start");
            var sourceMarket = string.Empty;
            var columns = new ColumnSet();
            columns.AddColumn(Attributes.Country.ISO2Code);
            var country = service.Retrieve(Entities.Country, sourceMarketId, columns);
            if(country != null && country.Attributes.Count > 0)
            {
                if(country.Attributes.Contains(Attributes.Country.ISO2Code) && country.Attributes[Attributes.Country.ISO2Code] != null)
                {
                    sourceMarket = country.Attributes[Attributes.Country.ISO2Code].ToString();
                }
            }
            trace.Trace("GetSourceMarketISO2Code - End");
            return sourceMarket;
        }

        /// <summary>
        /// To get data of customer in json format
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private EntityModel GetData(Entity entity)
        {
            trace.Trace("GetData - Start");
            var entityModel = new EntityModel()
            {
                Fields = GetFields(entity)
            };            
            trace.Trace("GetData - End");
            return entityModel;
        }

        /// <summary>
        /// To get list of fields based on data type
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private List<Field> GetFields(Entity entity)
        {
            trace.Trace("GetFields - Start");
            var fields = new List<Field>();
            foreach (KeyValuePair<String, Object> attribute in entity.Attributes)
            {
                var attributeType = GetFieldType(attribute.Key, entity);
                switch (attributeType)
                {
                    case FieldType.Boolean:
                    case FieldType.DateTime:
                    case FieldType.Decimal:
                    case FieldType.Double:
                    case FieldType.Guid:
                    case FieldType.Int32:
                    case FieldType.String:
                    case FieldType.Null:
                        fields.Add(new Field { Name = attribute.Key, Type = attributeType, Value = attribute.Value });
                        break;
                    case FieldType.Amount:
                        fields.Add(new Field { Name = attribute.Key, Type = attributeType, Value = GetMoney((Money)attribute.Value) });
                        break;
                    case FieldType.OptionSet:
                        fields.Add(new Field { Name = attribute.Key, Type = attributeType, Value = GetOptionSet(entity, attribute.Key) });
                        break;
                    case FieldType.Lookup:
                        fields.Add(new Field { Name = attribute.Key, Type = attributeType, Value = GetLookup(entity, attribute.Key) });
                        break;
                    case FieldType.RecordCollection:
                        fields.Add(new Field { Name = attribute.Key, Type = attributeType, Value = GetRecordCollection((EntityCollection)attribute.Value) });
                        break;
                }
            }
            trace.Trace("GetFields - End");
            return fields;
        }

        /// <summary>
        /// To get field type of crm attribute
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private FieldType GetFieldType(string attributeName, Entity entity)
        {           
            var fieldType = FieldType.Null;
            if (entity.Attributes[attributeName] == null || entity.Attributes[attributeName].GetType() == null || string.IsNullOrWhiteSpace(entity.Attributes[attributeName].GetType().Name)) return fieldType;
            trace.Trace("GetFieldType - Start");
            var attributeType = entity.Attributes[attributeName].GetType().Name;
            switch (attributeType.ToLower())
            {
                case "string":
                    fieldType = FieldType.String;
                    break;
                case "boolean":
                    fieldType = FieldType.Boolean;
                    break;
                case "guid":
                    fieldType = FieldType.Guid;
                    break;
                case "decimal":
                    fieldType = FieldType.Decimal;
                    break;
                case "datetime":
                    fieldType = FieldType.DateTime;
                    break;
                case "int32":
                    fieldType = FieldType.Int32;
                    break;
                case "double":
                    fieldType = FieldType.Double;
                    break;
                case "optionsetvalue":
                    fieldType = FieldType.OptionSet;
                    break;
                case "entityreference":
                    fieldType = FieldType.Lookup;
                    break;
                case "entitycollection":
                    fieldType = FieldType.RecordCollection;
                    break;
                case "money":
                    fieldType = FieldType.Amount;
                    break;
                default:
                    fieldType = FieldType.Null;
                    break;
            }            
            trace.Trace("GetFieldType - End");
            return fieldType;
        }

        /// <summary>
        /// To get money value
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        private decimal GetMoney(Money money)
        {
            decimal amount = 0;
            trace.Trace("GetMoney - Start");
            amount = (money == null) ? 0 : money.Value;
            trace.Trace("GetMoney - End");
            return amount;
        }

        /// <summary>
        /// To prepare lookup object
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private Lookup GetLookup(Entity entity, string attribute)
        {
            if (entity == null || !entity.Attributes.Contains(attribute) || entity.Attributes[attribute] == null) return null;
            var entityReference = (EntityReference)entity.Attributes[attribute];           
            trace.Trace("GetLookup - Start");
            var lookup = new Lookup()
            {
                Id = entityReference.Id,
                Name = (entity.FormattedValues.Contains(attribute)) ? entity.FormattedValues[attribute] : entityReference.Name,
                LogicalName = entityReference.LogicalName
            };      
            trace.Trace("GetLookup - End");
            return lookup;
        }

        /// <summary>
        /// To prepare optionset object
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private OptionSet GetOptionSet(Entity entity, string attribute)
        {
            if (entity == null || !entity.Attributes.Contains(attribute) || entity.Attributes[attribute] == null) return null;
            var option = (OptionSetValue)entity.Attributes[attribute];
            trace.Trace("GetOptionSet - Start");
            var optionSet = new OptionSet()
            {
                Value = option.Value,
                Name = (entity.FormattedValues.Contains(attribute)) ? entity.FormattedValues[attribute] : null
            };            
            trace.Trace("GetOptionSet - End");
            return optionSet;
        }

        /// <summary>
        /// To prepare recordcollection object using entitycollection
        /// </summary>
        /// <param name="entityCollection"></param>
        /// <returns></returns>
        private RecordCollection GetRecordCollection(EntityCollection entityCollection)
        {
            if (entityCollection == null) return null;
            trace.Trace("GetRecordCollection - Start");           
            var recordCollection = new RecordCollection()
            {
                ActivityParty = GetEntityRecordCollection(entityCollection)
            };
            trace.Trace("GetRecordCollection - End");
            return recordCollection;
        }

        /// <summary>
        /// To prepare entityrecord collection using entitycollection
        /// </summary>
        /// <param name="entityCollection"></param>
        /// <returns></returns>
        private List<EntityRecord> GetEntityRecordCollection(EntityCollection entityCollection)
        {  
            trace.Trace("GetEntityRecordCollection - Start");
            var entityRecordList = new List<EntityRecord>();
            foreach (Entity e in entityCollection.Entities)
            {
                var entityReference = (EntityReference)e.Attributes[Attributes.ActivityParty.PartyId];
                entityRecordList.Add(new EntityRecord { PartyId = new Lookup { Id = entityReference.Id, Name = entityReference.Name, LogicalName = entityReference.LogicalName } });
            }
            trace.Trace("GetEntityRecordCollection - End");
            return entityRecordList;
        }

        /// <summary>
        /// To get post entity image name
        /// </summary>
        private string PostImageName
        {
            get
            {
                return "PostImage";
            }
        }
        
    }
}
