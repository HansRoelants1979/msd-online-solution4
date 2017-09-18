using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using Tc.Crm.Plugins.MultipleEntities.BusinessLogic;

namespace Tc.Crm.Plugins.Customer.BusinessLogic
{
    public class CreateEntityCacheOnCustomerOperationService : CreateEntityCacheOnEntityOperationService
    {
       
        /// <summary>
        /// To set mapping attributes of entity cache from customer entity
        /// </summary>
        /// <param name="sourceEntity"></param>
        /// <param name="targetEntity"></param>
        public override void SetEntityParameters(Entity sourceEntity, Entity targetEntity)
        {
            trace.Trace("SetEntityParameters - Start");
            if (sourceEntity.Attributes.Contains(Attributes.Customer.FullName) && sourceEntity.Attributes[Attributes.Customer.FullName] != null)
            {
                targetEntity.Attributes[Attributes.EntityCache.Name] = sourceEntity.Attributes[Attributes.Customer.FullName];
            }
            else if(context.MessageName.Equals(Messages.Update, StringComparison.OrdinalIgnoreCase))
            {
                var entityImage = base.GetEntityImage();
                if(entityImage != null && entityImage.Attributes.Count > 0 && entityImage.Attributes.Contains(Attributes.Customer.FullName) && entityImage.Attributes[Attributes.Customer.FullName] != null)
                {
                    targetEntity.Attributes[Attributes.EntityCache.Name] = entityImage.Attributes[Attributes.Customer.FullName];
                }
            }
            if (sourceEntity.Attributes.Contains(Attributes.Customer.SourceMarketId) && sourceEntity.Attributes[Attributes.Customer.SourceMarketId] != null)
            {
                var iso2Code = GetSourceMarketISO2Code(((EntityReference)sourceEntity.Attributes[Attributes.Customer.SourceMarketId]).Id);
                targetEntity.Attributes[Attributes.EntityCache.SourceMarket] = iso2Code;
            }
            trace.Trace("SetEntityParameters - End");
        }

        /// <summary>
        /// To get address, email, telephone details from post image
        /// </summary>
        /// <returns></returns>
        public override Entity GetEntityImage()
        {
            trace.Trace("GetEntityImage - Start");
            Entity entityImage = new Entity();
            if (context.InputParameters.Contains(InputParameters.Target) && context.InputParameters[InputParameters.Target] is Entity)
            {   
                var primaryEntity = context.InputParameters[InputParameters.Target] as Entity;
                entityImage.Attributes.AddRange(GetAddressDetails(primaryEntity));
                entityImage.Attributes.AddRange(GetEmailDetails(primaryEntity));
                entityImage.Attributes.AddRange(GetTelephoneDetails(primaryEntity));
            }
            trace.Trace("GetEntityImage - End");
            return entityImage;
        }

        /// <summary>
        /// To get list of address details
        /// </summary>
        /// <param name="primaryEntity"></param>
        /// <returns></returns>
        private AttributeCollection GetAddressDetails(Entity primaryEntity)
        {
            trace.Trace("GetAddressDetails - Start");
            var listOfAddressFields = new List<string>(){ Attributes.Customer.Address1FlatorUnitNumber, Attributes.Customer.Address1HouseNumberoBuilding,
                                                          Attributes.Customer.Address1Street, Attributes.Customer.Address1AdditionalInformation,
                                                          Attributes.Customer.Address1Town, Attributes.Customer.Address1County,
                                                          Attributes.Customer.Address1CountryId, Attributes.Customer.Address1PostalCode,
                                                          Attributes.Customer.Address2FlatorUnitNumber, Attributes.Customer.Address2HouseNumberoBuilding,
                                                          Attributes.Customer.Address2Street, Attributes.Customer.Address2AdditionalInformation,
                                                          Attributes.Customer.Address2Town, Attributes.Customer.Address2County,
                                                          Attributes.Customer.Address2CountryId, Attributes.Customer.Address2PostalCode};
            trace.Trace("GetAddressDetails - End");
            return GetSelectedAttributesFromEntityImage(primaryEntity, listOfAddressFields);           
        }

        /// <summary>
        /// To get list of telephone details
        /// </summary>
        /// <param name="primaryEntity"></param>
        /// <returns></returns>
        private AttributeCollection GetTelephoneDetails(Entity primaryEntity)
        {
            trace.Trace("GetTelephoneDetails - Start");
            var listOfTelephoneFields = new List<string>(){ Attributes.Customer.Telephone1, Attributes.Customer.Telephone1Type,
                                                            Attributes.Customer.Telephone2, Attributes.Customer.Telephone2Type,
                                                            Attributes.Customer.Telephone3,Attributes.Customer.Telephone3Type};
            trace.Trace("GetTelephoneDetails - End");
            return GetSelectedAttributesFromEntityImage(primaryEntity, listOfTelephoneFields);
        }

        /// <summary>
        /// To get list of email details
        /// </summary>
        /// <param name="primaryEntity"></param>
        /// <returns></returns>
        private AttributeCollection GetEmailDetails(Entity primaryEntity)
        {
            trace.Trace("GetEmailDetails - Start");
            var listOfEmailFields = new List<string>() { Attributes.Customer.EmailAddress1, Attributes.Customer.EmailAddress1Type,
                                                         Attributes.Customer.EmailAddress2, Attributes.Customer.EmailAddress2Type,
                                                         Attributes.Customer.EmailAddress3, Attributes.Customer.EmailAddress3Type };
            trace.Trace("GetEmailDetails - End");
            return GetSelectedAttributesFromEntityImage(primaryEntity, listOfEmailFields);           
        }

        /// <summary>
        /// To get selected attributes from entity image when any attribute was updated
        /// </summary>
        /// <param name="primaryEntity"></param>
        /// <param name="listOfAttributes"></param>
        /// <returns></returns>
        private AttributeCollection GetSelectedAttributesFromEntityImage(Entity primaryEntity, List<string> listOfAttributes)
        {
            trace.Trace("GetSelectedAttributesFromEntityImage - Start");
            var attributes = new AttributeCollection();            
            if (primaryEntity == null || listOfAttributes == null || listOfAttributes.Count == 0) return attributes;            
            if (listOfAttributes.Any(a => primaryEntity.Attributes.Contains(a)))
            {                
                var postImage = base.GetEntityImage();
                attributes.AddRange(PrepareAttributesFromEntityImage(primaryEntity, postImage, listOfAttributes));                
            }
            trace.Trace("GetSelectedAttributesFromEntityImage - End");
            return attributes;
        }

        /// <summary>
        /// To prepare attributes from entity image by avoiding duplicates
        /// </summary>
        /// <param name="primaryEntity"></param>
        /// <param name="entityImage"></param>
        /// <param name="listOfAttributes"></param>
        /// <returns></returns>
        private List<KeyValuePair<string, object>> PrepareAttributesFromEntityImage(Entity primaryEntity, Entity entityImage, List<string> listOfAttributes)
        {
            trace.Trace("PrepareAttributesFromEntityImage - Start");
            var attributes = new List<KeyValuePair<string, object>>();
            if (primaryEntity == null || entityImage == null || listOfAttributes == null || listOfAttributes.Count == 0) return attributes;
            foreach (KeyValuePair<string, object> attribute in entityImage.Attributes)
            {
                if (listOfAttributes.Contains(attribute.Key) && !primaryEntity.Attributes.Contains(attribute.Key))
                {
                    attributes.Add(attribute);
                }
            }
            trace.Trace("PrepareAttributesFromEntityImage - End");
            return attributes;
        }
        
    }
}
