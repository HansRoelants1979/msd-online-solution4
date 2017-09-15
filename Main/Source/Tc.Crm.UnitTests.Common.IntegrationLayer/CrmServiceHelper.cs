using System;
using System.Collections.Generic;
using System.Linq;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;

namespace Tc.Crm.UnitTests.Common.IL
{
    internal class CrmServiceHelper
    {
        private static readonly Guid RecorId1 = Guid.NewGuid();
        private static readonly Guid RecorId2 = Guid.NewGuid();
        private static readonly Guid RecorId3 = Guid.NewGuid();
        private static readonly Guid RecorId4 = Guid.NewGuid();
        private static readonly Guid RecorId5 = Guid.NewGuid();

        private static readonly Guid EntityCacheId1 = Guid.NewGuid();
        private static readonly Guid EntityCacheId2 = Guid.NewGuid();
        private static readonly Guid EntityCacheId3 = Guid.NewGuid();
        private static readonly Guid EntityCacheId4 = Guid.NewGuid();
        private static readonly Guid EntityCacheId5 = Guid.NewGuid();
        private static readonly Guid EntityCacheId6 = Guid.NewGuid();
        private static readonly Guid EntityCacheId7 = Guid.NewGuid();
        private static readonly Guid EntityCacheId8 = Guid.NewGuid();
        private static readonly Guid EntityCacheId9 = Guid.NewGuid();
        private static readonly Guid EntityCacheId10 = Guid.NewGuid();


        public XrmFakedContext Context { get; set; }

        public CrmServiceHelper()
        {
            Context = new XrmFakedContext();
            PrepareData();
        }

        public EntityCollection GetCreatedCacheEntities()
        {
            var cacheEntities = Context.Data["tc_entitycache"].Values.ToList()
                .Where(entity =>
                    (string)entity.Attributes["tc_type"] == "contact" &&
                    (int)entity.Attributes["statuscode"] == 1 &&
                    ((OptionSetValue)entity.Attributes["tc_operation"]).Value == 950000000)
                .OrderBy(entity => (DateTime)entity.Attributes["createdon"]).ToList();

            return new EntityCollection(cacheEntities);
        }

        public EntityCollection GetUpdatedCacheEntities()
        {
            var cacheEntities = (from entityCache in Context.Data["tc_entitycache"].Values
                join entityCacheMessage in Context.Data["tc_entitycachemessage"].Values 
                on entityCache.Attributes["tc_entitycacheid"] equals entityCacheMessage.Attributes["tc_entitycacheid"]
                join contact in Context.Data["contact"].Values 
                on entityCache.Attributes["tc_recordid"] equals contact.Attributes["contactid"]
                where
                    (string) entityCache.Attributes["tc_type"] == "contact" &&
                    (int) entityCache.Attributes["statuscode"] == 1 &&
                    ((OptionSetValue) entityCache.Attributes["tc_operation"]).Value == 950000001 &&
                    contact.Attributes["tc_sourcesystemid"] != null &&
                    (int) entityCacheMessage.Attributes["statuscode"] != 950000001
                select entityCache).ToList();

            return new EntityCollection(cacheEntities);
        }

        public EntityCollection GetExpiry()
        {
            var cacheEntities = Context.Data["tc_configuration"].Values.ToList()
                .Where(entity =>
                    (string)entity.Attributes["tc_name"] == "Tc.OutboundSynchronisation.SsoTokenExpiredSeconds")
                .ToList();

            return new EntityCollection(cacheEntities);
        }

        public EntityCollection GetNotBeforeTime()
        {
            var cacheEntities = Context.Data["tc_configuration"].Values.ToList()
                .Where(entity =>
                    (string)entity.Attributes["tc_name"] == "Tc.OutboundSynchronisation.SsoTokenNotBeforeTimeSeconds")
                .ToList();

            return new EntityCollection(cacheEntities);
        }

        public EntityCollection GetServiceUrl()
        {
            var cacheEntities = Context.Data["tc_configuration"].Values.ToList()
                .Where(entity =>
                    (string)entity.Attributes["tc_name"] == "Tc.OutboundSynchronisation.SsoServiceUrl")
                .ToList();

            return new EntityCollection(cacheEntities);
        }

        public EntityCollection GetPrivateKey()
        {
            var cacheEntities = Context.Data["tc_secureconfiguration"].Values.ToList()
                .Where(entity =>
                    (string)entity.Attributes["tc_name"] == "Tc.OutboundSynchronisation.JwtPrivateKey")
                .ToList();

            return new EntityCollection(cacheEntities);
        }

        private void AddCacheEntities()
        {
            var collection = new Dictionary<Guid, Entity>();
            var entity1Id = Guid.NewGuid();
            var cacheEntity1 = new Entity("tc_entitycache", entity1Id)
            {
                ["tc_entitycacheid"] = EntityCacheId1,
                ["tc_name"] = "Dave Dave",
                ["createdon"] = DateTime.Now,
                ["statuscode"] = 1,
                ["tc_recordid"] = Guid.NewGuid(),
                ["tc_type"] = "contact",
                ["tc_operation"] = new OptionSetValue(950000000),
                ["tc_data"] = @"{'Fields':[
                                                {'Name':'tc_address1_county','Type':9,'Value':'Hampshire'},
                                                {'Name':'territorycode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'address2_freighttermscode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'address2_shippingmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'isprivate','Type':0,'Value':false},
                                                {'Name':'followemail','Type':0,'Value':true},
                                                {'Name':'donotbulkemail','Type':0,'Value':false},
                                                {'Name':'donotsendmm','Type':0,'Value':false},
                                                {'Name':'haschildrencode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'educationcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'customertypecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'tc_address1_flatorunitnumber','Type':9,'Value':'7 Mariners Close'},
                                                {'Name':'isautocreate','Type':0,'Value':false},
                                                {'Name':'tc_deceased','Type':0,'Value':false},
                                                {'Name':'ownerid','Type':5,'Value':{'__type':'Lookup','Id':'245bfe4d-a981-e711-8108-5065f38a9b51','LogicalName':'systemuser','Name':null}},
                                                {'Name':'isbackofficecustomer','Type':0,'Value':false},
                                                {'Name':'donotbulkpostalmail','Type':0,'Value':false},
                                                {'Name':'donotpostalmail','Type':0,'Value':false},
                                                {'Name':'tc_nolongerlivingataddress','Type':0,'Value':false},
                                                {'Name':'donotemail','Type':0,'Value':false},
                                                {'Name':'statecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':0}},
                                                {'Name':'tc_donotallowsms','Type':0,'Value':false},
                                                {'Name':'address2_addresstypecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'donotphone','Type':0,'Value':false},
                                                {'Name':'createdon','Type':1,'Value':'\/Date(1504173594000)\/'},
                                                {'Name':'transactioncurrencyid','Type':5,'Value':{'__type':'Lookup','Id':'2ecffb4d-33c7-e611-80fe-3863bb34da28','LogicalName':'transactioncurrency','Name':null}},
                                                {'Name':'contactid','Type':6,'Value':'2638b022-338e-e711-8104-5065f38b74a1'},
                                                {'Name':'tc_emailaddressavailable','Type':0,'Value':false},
                                                {'Name':'modifiedby','Type':5,'Value':{'__type':'Lookup','Id':'245bfe4d-a981-e711-8108-5065f38a9b51','LogicalName':'systemuser','Name':null}},
                                                {'Name':'leadsourcecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'statuscode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000002}},
                                                {'Name':'modifiedonbehalfby','Type':10,'Value':null},
                                                {'Name':'preferredcontactmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'lastname','Type':9,'Value':'Dave'},
                                                {'Name':'tc_address1_town','Type':9,'Value':'TADLEY'},
                                                {'Name':'firstname','Type':9,'Value':'Dave'},
                                                {'Name':'createdby','Type':5,'Value':{'__type':'Lookup','Id':'245bfe4d-a981-e711-8108-5065f38a9b51','LogicalName':'systemuser','Name':null}},
                                                {'Name':'yomifullname','Type':9,'Value':'Dave Dave'},
                                                {'Name':'donotfax','Type':0,'Value':false},
                                                {'Name':'merged','Type':0,'Value':false},
                                                {'Name':'fullname','Type':9,'Value':'Dave Dave'},
                                                {'Name':'customersizecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'marketingonly','Type':0,'Value':false},
                                                {'Name':'tc_vip','Type':0,'Value':false},
                                                {'Name':'owningbusinessunit','Type':5,'Value':{'__type':'Lookup','Id':'29876693-2ec7-e611-80fe-3863bb34da28','LogicalName':'businessunit','Name':null}},
                                                {'Name':'shippingmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'tc_marketing','Type':0,'Value':false},
                                                {'Name':'tc_salutation','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000000}},
                                                {'Name':'creditonhold','Type':0,'Value':false},
                                                {'Name':'tc_address1_countryid','Type':5,'Value':{'__type':'Lookup','Id':'fa255c25-8a14-e711-810c-3863bb34fa70','LogicalName':'tc_country','Name':null}},
                                                {'Name':'modifiedon','Type':1,'Value':'\/Date(1504173594000)\/'},
                                                {'Name':'participatesinworkflow','Type':0,'Value':false},
                                                {'Name':'preferredappointmenttimecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'tc_disabledindicator','Type':0,'Value':false},
                                                {'Name':'tc_address1_postalcode','Type':9,'Value':'RG26 3NP'},
                                                {'Name':'exchangerate','Type':2,'Value':1.0000000000}]}"
            };

            var entityId2 = Guid.NewGuid();
            var cacheEntity2 = new Entity("tc_entitycache", entityId2)
            {
                ["tc_entitycacheid"] = EntityCacheId2,
                ["tc_name"] = "Cache entity 2",
                ["createdon"] = DateTime.Now,
                ["statuscode"] = 1,
                ["tc_recordid"] = Guid.NewGuid(),
                ["tc_type"] = "contact",
                ["tc_operation"] = new OptionSetValue(950000000),
                ["tc_data"] = @"{'Fields':[
                                                {'Name':'territorycode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'address2_freighttermscode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'address2_shippingmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'isprivate','Type':0,'Value':false},
                                                {'Name':'followemail','Type':0,'Value':true},
                                                {'Name':'tc_colleague','Type':0,'Value':false},
                                                {'Name':'donotbulkemail','Type':0,'Value':false},
                                                {'Name':'donotsendmm','Type':0,'Value':false},
                                                {'Name':'emailaddress1','Type':9,'Value':'green.wood@mail.com'},
                                                {'Name':'educationcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'customertypecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'fullname','Type':9,'Value':'Green Wood'},
                                                {'Name':'isautocreate','Type':0,'Value':false},
                                                {'Name':'tc_emailaddress1type','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000000}},
                                                {'Name':'tc_deceased','Type':0,'Value':false},
                                                {'Name':'ownerid','Type':5,'Value':{'__type':'Lookup','Id':'e851057b-5465-e711-8105-e0071b65fe61','LogicalName':'systemuser','Name':null}},
                                                {'Name':'isbackofficecustomer','Type':0,'Value':false},
                                                {'Name':'donotbulkpostalmail','Type':0,'Value':false},
                                                {'Name':'donotpostalmail','Type':0,'Value':false},
                                                {'Name':'tc_nolongerlivingataddress','Type':0,'Value':false},
                                                {'Name':'donotemail','Type':0,'Value':false},
                                                {'Name':'statecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':0}},
                                                {'Name':'tc_donotallowsms','Type':0,'Value':false},
                                                {'Name':'tc_marketing','Type':0,'Value':false},
                                                {'Name':'donotphone','Type':0,'Value':false},
                                                {'Name':'createdon','Type':1,'Value':'\/Date(1504275764000)\/'},
                                                {'Name':'transactioncurrencyid','Type':5,'Value':{'__type':'Lookup','Id':'2ecffb4d-33c7-e611-80fe-3863bb34da28','LogicalName':'transactioncurrency','Name':null}},
                                                {'Name':'contactid','Type':6,'Value':'1b106c01-218f-e711-8105-5065f38bc531'},
                                                {'Name':'tc_emailaddressavailable','Type':0,'Value':true},
                                                {'Name':'address2_addresstypecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'haschildrencode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'modifiedby','Type':5,'Value':{'__type':'Lookup','Id':'e851057b-5465-e711-8105-e0071b65fe61','LogicalName':'systemuser','Name':null}},
                                                {'Name':'leadsourcecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'statuscode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000002}},
                                                {'Name':'modifiedonbehalfby','Type':10,'Value':null},
                                                {'Name':'preferredcontactmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'lastname','Type':9,'Value':'Wood'},
                                                {'Name':'firstname','Type':9,'Value':'Green'},
                                                {'Name':'createdby','Type':5,'Value':{'__type':'Lookup','Id':'e851057b-5465-e711-8105-e0071b65fe61','LogicalName':'systemuser','Name':null}},
                                                {'Name':'yomifullname','Type':9,'Value':'Green Wood'},
                                                {'Name':'donotfax','Type':0,'Value':false},
                                                {'Name':'merged','Type':0,'Value':false},
                                                {'Name':'customersizecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'marketingonly','Type':0,'Value':false},
                                                {'Name':'tc_vip','Type':0,'Value':false},
                                                {'Name':'owningbusinessunit','Type':5,'Value':{'__type':'Lookup','Id':'29876693-2ec7-e611-80fe-3863bb34da28','LogicalName':'businessunit','Name':null}},
                                                {'Name':'shippingmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'tc_emailaddress1confirmation','Type':9,'Value':'green.wood@mail.com'},
                                                {'Name':'tc_salutation','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000001}},
                                                {'Name':'creditonhold','Type':0,'Value':false},
                                                {'Name':'tc_address1_countryid','Type':5,'Value':{'__type':'Lookup','Id':'b4255c25-8a14-e711-810c-3863bb34fa70','LogicalName':'tc_country','Name':null}},
                                                {'Name':'modifiedon','Type':1,'Value':'\/Date(1504275764000)\/'},
                                                {'Name':'participatesinworkflow','Type':0,'Value':false},
                                                {'Name':'preferredappointmenttimecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'tc_disabledindicator','Type':0,'Value':false},
                                                {'Name':'tc_address1_postalcode','Type':9,'Value':'MK 3452'},
                                                {'Name':'exchangerate','Type':2,'Value':1.0000000000}]}"
            };

            var entityId3 = Guid.NewGuid();
            var cacheEntity3 = new Entity("tc_entitycache", entityId3)
            {
                ["tc_entitycacheid"] = EntityCacheId3,
                ["tc_name"] = "Cache entity 3",
                ["createdon"] = DateTime.Now,
                ["statuscode"] = 1,
                ["tc_recordid"] = Guid.NewGuid(),
                ["tc_type"] = "contact",
                ["tc_operation"] = new OptionSetValue(950000000),
                ["tc_data"] = @"{'Fields':[
                                                {'Name':'territorycode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'address2_freighttermscode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'address2_shippingmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'isprivate','Type':0,'Value':false},
                                                {'Name':'followemail','Type':0,'Value':true},
                                                {'Name':'tc_colleague','Type':0,'Value':false},
                                                {'Name':'donotbulkemail','Type':0,'Value':false},
                                                {'Name':'donotsendmm','Type':0,'Value':false},
                                                {'Name':'haschildrencode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'educationcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'customertypecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'fullname','Type':9,'Value':'John Jones'},
                                                {'Name':'isautocreate','Type':0,'Value':false},
                                                {'Name':'tc_deceased','Type':0,'Value':false},
                                                {'Name':'ownerid','Type':5,'Value':{'__type':'Lookup','Id':'a363ca44-4dd7-e611-8101-3863bb34fa70','LogicalName':'systemuser','Name':null}},
                                                {'Name':'isbackofficecustomer','Type':0,'Value':false},
                                                {'Name':'donotbulkpostalmail','Type':0,'Value':false},
                                                {'Name':'donotpostalmail','Type':0,'Value':false},
                                                {'Name':'tc_nolongerlivingataddress','Type':0,'Value':false},
                                                {'Name':'donotemail','Type':0,'Value':false},
                                                {'Name':'statecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':0}},
                                                {'Name':'tc_donotallowsms','Type':0,'Value':false},
                                                {'Name':'tc_marketing','Type':0,'Value':false},
                                                {'Name':'donotphone','Type':0,'Value':false},
                                                {'Name':'createdon','Type':1,'Value':'\/Date(1504199152000)\/'},
                                                {'Name':'transactioncurrencyid','Type':5,'Value':{'__type':'Lookup','Id':'2ecffb4d-33c7-e611-80fe-3863bb34da28','LogicalName':'transactioncurrency','Name':null}},
                                                {'Name':'contactid','Type':6,'Value':'8f9af6a1-6e8e-e711-8105-5065f38bc531'},
                                                {'Name':'tc_emailaddressavailable','Type':0,'Value':false},
                                                {'Name':'address2_addresstypecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'modifiedby','Type':5,'Value':{'__type':'Lookup','Id':'a363ca44-4dd7-e611-8101-3863bb34fa70','LogicalName':'systemuser','Name':null}},
                                                {'Name':'leadsourcecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'statuscode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000002}},
                                                {'Name':'modifiedonbehalfby','Type':10,'Value':null},
                                                {'Name':'preferredcontactmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'lastname','Type':9,'Value':'Jones'},
                                                {'Name':'firstname','Type':9,'Value':'John'},
                                                {'Name':'createdby','Type':5,'Value':{'__type':'Lookup','Id':'a363ca44-4dd7-e611-8101-3863bb34fa70','LogicalName':'systemuser','Name':null}},
                                                {'Name':'yomifullname','Type':9,'Value':'John Jones'},
                                                {'Name':'donotfax','Type':0,'Value':false},
                                                {'Name':'merged','Type':0,'Value':false},
                                                {'Name':'customersizecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'marketingonly','Type':0,'Value':false},
                                                {'Name':'tc_vip','Type':0,'Value':false},
                                                {'Name':'owningbusinessunit','Type':5,'Value':{'__type':'Lookup','Id':'29876693-2ec7-e611-80fe-3863bb34da28','LogicalName':'businessunit','Name':null}},
                                                {'Name':'shippingmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'tc_salutation','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000000}},
                                                {'Name':'creditonhold','Type':0,'Value':false},
                                                {'Name':'tc_address1_countryid','Type':5,'Value':{'__type':'Lookup','Id':'fa255c25-8a14-e711-810c-3863bb34fa70','LogicalName':'tc_country','Name':null}},
                                                {'Name':'modifiedon','Type':1,'Value':'\/Date(1504199152000)\/'},
                                                {'Name':'participatesinworkflow','Type':0,'Value':false},
                                                {'Name':'preferredappointmenttimecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'tc_disabledindicator','Type':0,'Value':false},
                                                {'Name':'tc_address1_postalcode','Type':9,'Value':'YO41 5SJ'},
                                                {'Name':'exchangerate','Type':2,'Value':1.0000000000}]}"
            };

            var entityId4 = Guid.NewGuid();
            var cacheEntity4 = new Entity("tc_entitycache", entityId4)
            {
                ["tc_entitycacheid"] = EntityCacheId4,
                ["tc_name"] = "Cache entity 4",
                ["createdon"] = DateTime.Now,
                ["statuscode"] = 1,
                ["tc_recordid"] = Guid.NewGuid(),
                ["tc_type"] = "contact",
                ["tc_operation"] = new OptionSetValue(950000000),
                ["tc_data"] = @"{'Fields':[
                                                {'Name':'territorycode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'address2_freighttermscode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'address2_shippingmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'isprivate','Type':0,'Value':false},
                                                {'Name':'followemail','Type':0,'Value':true},
                                                {'Name':'tc_colleague','Type':0,'Value':false},
                                                {'Name':'donotbulkemail','Type':0,'Value':false},
                                                {'Name':'donotsendmm','Type':0,'Value':false},
                                                {'Name':'haschildrencode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'educationcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'customertypecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'fullname','Type':9,'Value':'John Singer'},
                                                {'Name':'isautocreate','Type':0,'Value':false},
                                                {'Name':'tc_deceased','Type':0,'Value':false},
                                                {'Name':'ownerid','Type':5,'Value':{'__type':'Lookup','Id':'04601adf-d8d7-e611-80f9-3863bb349770','LogicalName':'systemuser','Name':null}},
                                                {'Name':'isbackofficecustomer','Type':0,'Value':false},
                                                {'Name':'donotbulkpostalmail','Type':0,'Value':false},
                                                {'Name':'donotpostalmail','Type':0,'Value':false},
                                                {'Name':'tc_nolongerlivingataddress','Type':0,'Value':false},
                                                {'Name':'donotemail','Type':0,'Value':false},
                                                {'Name':'statecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':0}},
                                                {'Name':'tc_donotallowsms','Type':0,'Value':false},
                                                {'Name':'tc_marketing','Type':0,'Value':false},
                                                {'Name':'donotphone','Type':0,'Value':false},
                                                {'Name':'createdon','Type':1,'Value':'\/Date(1504519480000)\/'},
                                                {'Name':'transactioncurrencyid','Type':5,'Value':{'__type':'Lookup','Id':'2ecffb4d-33c7-e611-80fe-3863bb34da28','LogicalName':'transactioncurrency','Name':null}},
                                                {'Name':'contactid','Type':6,'Value':'c7870677-5891-e711-8104-5065f38b74a1'},
                                                {'Name':'tc_emailaddressavailable','Type':0,'Value':false},
                                                {'Name':'address2_addresstypecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'modifiedby','Type':5,'Value':{'__type':'Lookup','Id':'04601adf-d8d7-e611-80f9-3863bb349770','LogicalName':'systemuser','Name':null}},
                                                {'Name':'leadsourcecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'statuscode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000002}},
                                                {'Name':'modifiedonbehalfby','Type':10,'Value':null},
                                                {'Name':'preferredcontactmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'lastname','Type':9,'Value':'Singer'},
                                                {'Name':'firstname','Type':9,'Value':'John'},
                                                {'Name':'createdby','Type':5,'Value':{'__type':'Lookup','Id':'04601adf-d8d7-e611-80f9-3863bb349770','LogicalName':'systemuser','Name':null}},
                                                {'Name':'yomifullname','Type':9,'Value':'John Singer'},
                                                {'Name':'donotfax','Type':0,'Value':false},
                                                {'Name':'merged','Type':0,'Value':false},
                                                {'Name':'customersizecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'marketingonly','Type':0,'Value':false},
                                                {'Name':'tc_vip','Type':0,'Value':false},
                                                {'Name':'owningbusinessunit','Type':5,'Value':{'__type':'Lookup','Id':'29876693-2ec7-e611-80fe-3863bb34da28','LogicalName':'businessunit','Name':null}},
                                                {'Name':'shippingmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'tc_salutation','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000000}},
                                                {'Name':'creditonhold','Type':0,'Value':false},
                                                {'Name':'tc_address1_countryid','Type':5,'Value':{'__type':'Lookup','Id':'fa255c25-8a14-e711-810c-3863bb34fa70','LogicalName':'tc_country','Name':null}},
                                                {'Name':'modifiedon','Type':1,'Value':'\/Date(1504519480000)\/'},
                                                {'Name':'participatesinworkflow','Type':0,'Value':false},
                                                {'Name':'preferredappointmenttimecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'tc_disabledindicator','Type':0,'Value':false},
                                                {'Name':'tc_address1_postalcode','Type':9,'Value':'kt4 7al'},
                                                {'Name':'exchangerate','Type':2,'Value':1.0000000000}]}"
            };

            var entityId5 = Guid.NewGuid();
            var cacheEntity5 = new Entity("tc_entitycache", entityId5)
            {
                ["tc_entitycacheid"] = EntityCacheId5,
                ["tc_name"] = "Cache entity 5",
                ["createdon"] = DateTime.Now,
                ["statuscode"] = 1,
                ["tc_recordid"] = Guid.NewGuid(),
                ["tc_type"] = "contact",
                ["tc_operation"] = new OptionSetValue(950000000),
                ["tc_data"] = @"{'Fields':[
                                                {'Name':'territorycode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'address2_freighttermscode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'tc_gender','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000001}},
                                                {'Name':'address2_shippingmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'isprivate','Type':0,'Value':false},
                                                {'Name':'followemail','Type':0,'Value':true},
                                                {'Name':'tc_colleague','Type':0,'Value':false},
                                                {'Name':'donotbulkemail','Type':0,'Value':false},{
                                                'Name':'donotsendmm','Type':0,'Value':false},
                                                {'Name':'emailaddress1','Type':9,'Value':'last.friday@mail.com'},
                                                {'Name':'educationcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'customertypecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'fullname','Type':9,'Value':'Last Friday'},
                                                {'Name':'isautocreate','Type':0,'Value':false},
                                                {'Name':'tc_emailaddress1type','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000000}},
                                                {'Name':'tc_deceased','Type':0,'Value':false},
                                                {'Name':'ownerid','Type':5,'Value':{'__type':'Lookup','Id':'e851057b-5465-e711-8105-e0071b65fe61','LogicalName':'systemuser','Name':null}},
                                                {'Name':'isbackofficecustomer','Type':0,'Value':false},
                                                {'Name':'donotbulkpostalmail','Type':0,'Value':false},
                                                {'Name':'donotpostalmail','Type':0,'Value':false},
                                                {'Name':'tc_nolongerlivingataddress','Type':0,'Value':false},
                                                {'Name':'donotemail','Type':0,'Value':false},
                                                {'Name':'statecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':0}},
                                                {'Name':'tc_donotallowsms','Type':0,'Value':false},
                                                {'Name':'tc_marketing','Type':0,'Value':false},
                                                {'Name':'donotphone','Type':0,'Value':false},
                                                {'Name':'createdon','Type':1,'Value':'\/Date(1504536853000)\/'},
                                                {'Name':'transactioncurrencyid','Type':5,'Value':{'__type':'Lookup','Id':'2ecffb4d-33c7-e611-80fe-3863bb34da28','LogicalName':'transactioncurrency','Name':null}},
                                                {'Name':'contactid','Type':6,'Value':'ce8dfbe9-8091-e711-8105-5065f38bb571'},
                                                {'Name':'tc_emailaddressavailable','Type':0,'Value':true},
                                                {'Name':'address2_addresstypecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'haschildrencode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'modifiedby','Type':5,'Value':{'__type':'Lookup','Id':'e851057b-5465-e711-8105-e0071b65fe61','LogicalName':'systemuser','Name':null}},
                                                {'Name':'leadsourcecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'statuscode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000002}},
                                                {'Name':'modifiedonbehalfby','Type':10,'Value':null},
                                                {'Name':'preferredcontactmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'lastname','Type':9,'Value':'Friday'},
                                                {'Name':'firstname','Type':9,'Value':'Last'},
                                                {'Name':'createdby','Type':5,'Value':{'__type':'Lookup','Id':'e851057b-5465-e711-8105-e0071b65fe61','LogicalName':'systemuser','Name':null}},
                                                {'Name':'yomifullname','Type':9,'Value':'Last Friday'},
                                                {'Name':'donotfax','Type':0,'Value':false},
                                                {'Name':'merged','Type':0,'Value':false},
                                                {'Name':'customersizecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'marketingonly','Type':0,'Value':false},
                                                {'Name':'tc_vip','Type':0,'Value':false},
                                                {'Name':'owningbusinessunit','Type':5,'Value':{'__type':'Lookup','Id':'29876693-2ec7-e611-80fe-3863bb34da28','LogicalName':'businessunit','Name':null}},
                                                {'Name':'shippingmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'tc_emailaddress1confirmation','Type':9,'Value':'last.friday@mail.com'},
                                                {'Name':'tc_salutation','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000000}},
                                                {'Name':'creditonhold','Type':0,'Value':false},
                                                {'Name':'tc_address1_countryid','Type':5,'Value':{'__type':'Lookup','Id':'48265c25-8a14-e711-810c-3863bb34fa70','LogicalName':'tc_country','Name':null}},
                                                {'Name':'modifiedon','Type':1,'Value':'\/Date(1504536853000)\/'},
                                                {'Name':'participatesinworkflow','Type':0,'Value':false},
                                                {'Name':'preferredappointmenttimecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'tc_disabledindicator','Type':0,'Value':false},
                                                {'Name':'tc_address1_postalcode','Type':9,'Value':'OB 45647'},
                                                {'Name':'exchangerate','Type':2,'Value':1.0000000000}]}"
            };

            var entity6Id = Guid.NewGuid();
            var cacheEntity6 = new Entity("tc_entitycache", entity6Id)
            {
                ["tc_entitycacheid"] = EntityCacheId6,
                ["tc_name"] = "User6",
                ["createdon"] = DateTime.Now,
                ["statuscode"] = 1,
                ["tc_recordid"] = RecorId1,
                ["tc_type"] = "contact",
                ["tc_operation"] = new OptionSetValue(950000001),
                ["tc_data"] = @"{'Fields':[
                                                {'Name':'tc_address1_county','Type':9,'Value':'Hampshire'},
                                                {'Name':'territorycode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'address2_freighttermscode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'address2_shippingmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'isprivate','Type':0,'Value':false},
                                                {'Name':'followemail','Type':0,'Value':true},
                                                {'Name':'donotbulkemail','Type':0,'Value':false},
                                                {'Name':'donotsendmm','Type':0,'Value':false},
                                                {'Name':'haschildrencode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'educationcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'customertypecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'tc_address1_flatorunitnumber','Type':9,'Value':'7 Mariners Close'},
                                                {'Name':'isautocreate','Type':0,'Value':false},
                                                {'Name':'tc_deceased','Type':0,'Value':false},
                                                {'Name':'ownerid','Type':5,'Value':{'__type':'Lookup','Id':'245bfe4d-a981-e711-8108-5065f38a9b51','LogicalName':'systemuser','Name':null}},
                                                {'Name':'isbackofficecustomer','Type':0,'Value':false},
                                                {'Name':'donotbulkpostalmail','Type':0,'Value':false},
                                                {'Name':'donotpostalmail','Type':0,'Value':false},
                                                {'Name':'tc_nolongerlivingataddress','Type':0,'Value':false},
                                                {'Name':'donotemail','Type':0,'Value':false},
                                                {'Name':'statecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':0}},
                                                {'Name':'tc_donotallowsms','Type':0,'Value':false},
                                                {'Name':'address2_addresstypecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'donotphone','Type':0,'Value':false},
                                                {'Name':'createdon','Type':1,'Value':'\/Date(1504173594000)\/'},
                                                {'Name':'transactioncurrencyid','Type':5,'Value':{'__type':'Lookup','Id':'2ecffb4d-33c7-e611-80fe-3863bb34da28','LogicalName':'transactioncurrency','Name':null}},
                                                {'Name':'contactid','Type':6,'Value':'2638b022-338e-e711-8104-5065f38b74a1'},
                                                {'Name':'tc_emailaddressavailable','Type':0,'Value':false},
                                                {'Name':'modifiedby','Type':5,'Value':{'__type':'Lookup','Id':'245bfe4d-a981-e711-8108-5065f38a9b51','LogicalName':'systemuser','Name':null}},
                                                {'Name':'leadsourcecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'statuscode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000002}},
                                                {'Name':'modifiedonbehalfby','Type':10,'Value':null},
                                                {'Name':'preferredcontactmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'lastname','Type':9,'Value':'Dave'},
                                                {'Name':'tc_address1_town','Type':9,'Value':'TADLEY'},
                                                {'Name':'firstname','Type':9,'Value':'Dave'},
                                                {'Name':'createdby','Type':5,'Value':{'__type':'Lookup','Id':'245bfe4d-a981-e711-8108-5065f38a9b51','LogicalName':'systemuser','Name':null}},
                                                {'Name':'yomifullname','Type':9,'Value':'Dave Dave'},
                                                {'Name':'donotfax','Type':0,'Value':false},
                                                {'Name':'merged','Type':0,'Value':false},
                                                {'Name':'fullname','Type':9,'Value':'Dave Dave'},
                                                {'Name':'customersizecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'marketingonly','Type':0,'Value':false},
                                                {'Name':'tc_vip','Type':0,'Value':false},
                                                {'Name':'owningbusinessunit','Type':5,'Value':{'__type':'Lookup','Id':'29876693-2ec7-e611-80fe-3863bb34da28','LogicalName':'businessunit','Name':null}},
                                                {'Name':'shippingmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'tc_marketing','Type':0,'Value':false},
                                                {'Name':'tc_salutation','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000000}},
                                                {'Name':'creditonhold','Type':0,'Value':false},
                                                {'Name':'tc_address1_countryid','Type':5,'Value':{'__type':'Lookup','Id':'fa255c25-8a14-e711-810c-3863bb34fa70','LogicalName':'tc_country','Name':null}},
                                                {'Name':'modifiedon','Type':1,'Value':'\/Date(1504173594000)\/'},
                                                {'Name':'participatesinworkflow','Type':0,'Value':false},
                                                {'Name':'preferredappointmenttimecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'tc_disabledindicator','Type':0,'Value':false},
                                                {'Name':'tc_address1_postalcode','Type':9,'Value':'RG26 3NP'},
                                                {'Name':'exchangerate','Type':2,'Value':1.0000000000}]}"
            };

            var entityId7 = Guid.NewGuid();
            var cacheEntity7 = new Entity("tc_entitycache", entityId7)
            {
                ["tc_entitycacheid"] = EntityCacheId7,
                ["tc_name"] = "User7",
                ["createdon"] = DateTime.Now,
                ["statuscode"] = 1,
                ["tc_recordid"] = RecorId2,
                ["tc_type"] = "contact",
                ["tc_operation"] = new OptionSetValue(950000001),
                ["tc_data"] = @"{'Fields':[
                                                {'Name':'territorycode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'address2_freighttermscode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'address2_shippingmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'isprivate','Type':0,'Value':false},
                                                {'Name':'followemail','Type':0,'Value':true},
                                                {'Name':'tc_colleague','Type':0,'Value':false},
                                                {'Name':'donotbulkemail','Type':0,'Value':false},
                                                {'Name':'donotsendmm','Type':0,'Value':false},
                                                {'Name':'emailaddress1','Type':9,'Value':'green.wood@mail.com'},
                                                {'Name':'educationcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'customertypecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'fullname','Type':9,'Value':'Green Wood'},
                                                {'Name':'isautocreate','Type':0,'Value':false},
                                                {'Name':'tc_emailaddress1type','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000000}},
                                                {'Name':'tc_deceased','Type':0,'Value':false},
                                                {'Name':'ownerid','Type':5,'Value':{'__type':'Lookup','Id':'e851057b-5465-e711-8105-e0071b65fe61','LogicalName':'systemuser','Name':null}},
                                                {'Name':'isbackofficecustomer','Type':0,'Value':false},
                                                {'Name':'donotbulkpostalmail','Type':0,'Value':false},
                                                {'Name':'donotpostalmail','Type':0,'Value':false},
                                                {'Name':'tc_nolongerlivingataddress','Type':0,'Value':false},
                                                {'Name':'donotemail','Type':0,'Value':false},
                                                {'Name':'statecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':0}},
                                                {'Name':'tc_donotallowsms','Type':0,'Value':false},
                                                {'Name':'tc_marketing','Type':0,'Value':false},
                                                {'Name':'donotphone','Type':0,'Value':false},
                                                {'Name':'createdon','Type':1,'Value':'\/Date(1504275764000)\/'},
                                                {'Name':'transactioncurrencyid','Type':5,'Value':{'__type':'Lookup','Id':'2ecffb4d-33c7-e611-80fe-3863bb34da28','LogicalName':'transactioncurrency','Name':null}},
                                                {'Name':'contactid','Type':6,'Value':'1b106c01-218f-e711-8105-5065f38bc531'},
                                                {'Name':'tc_emailaddressavailable','Type':0,'Value':true},
                                                {'Name':'address2_addresstypecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'haschildrencode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'modifiedby','Type':5,'Value':{'__type':'Lookup','Id':'e851057b-5465-e711-8105-e0071b65fe61','LogicalName':'systemuser','Name':null}},
                                                {'Name':'leadsourcecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'statuscode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000002}},
                                                {'Name':'modifiedonbehalfby','Type':10,'Value':null},
                                                {'Name':'preferredcontactmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'lastname','Type':9,'Value':'Wood'},
                                                {'Name':'firstname','Type':9,'Value':'Green'},
                                                {'Name':'createdby','Type':5,'Value':{'__type':'Lookup','Id':'e851057b-5465-e711-8105-e0071b65fe61','LogicalName':'systemuser','Name':null}},
                                                {'Name':'yomifullname','Type':9,'Value':'Green Wood'},
                                                {'Name':'donotfax','Type':0,'Value':false},
                                                {'Name':'merged','Type':0,'Value':false},
                                                {'Name':'customersizecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'marketingonly','Type':0,'Value':false},
                                                {'Name':'tc_vip','Type':0,'Value':false},
                                                {'Name':'owningbusinessunit','Type':5,'Value':{'__type':'Lookup','Id':'29876693-2ec7-e611-80fe-3863bb34da28','LogicalName':'businessunit','Name':null}},
                                                {'Name':'shippingmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'tc_emailaddress1confirmation','Type':9,'Value':'green.wood@mail.com'},
                                                {'Name':'tc_salutation','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000001}},
                                                {'Name':'creditonhold','Type':0,'Value':false},
                                                {'Name':'tc_address1_countryid','Type':5,'Value':{'__type':'Lookup','Id':'b4255c25-8a14-e711-810c-3863bb34fa70','LogicalName':'tc_country','Name':null}},
                                                {'Name':'modifiedon','Type':1,'Value':'\/Date(1504275764000)\/'},
                                                {'Name':'participatesinworkflow','Type':0,'Value':false},
                                                {'Name':'preferredappointmenttimecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'tc_disabledindicator','Type':0,'Value':false},
                                                {'Name':'tc_address1_postalcode','Type':9,'Value':'MK 3452'},
                                                {'Name':'exchangerate','Type':2,'Value':1.0000000000}]}"
            };

            var entityId8 = Guid.NewGuid();
            var cacheEntity8 = new Entity("tc_entitycache", entityId8)
            {
                ["tc_entitycacheid"] = EntityCacheId8,
                ["tc_name"] = "User8",
                ["createdon"] = DateTime.Now,
                ["statuscode"] = 1,
                ["tc_recordid"] = RecorId3,
                ["tc_type"] = "contact",
                ["tc_operation"] = new OptionSetValue(950000001),
                ["tc_data"] = @"{'Fields':[
                                                {'Name':'territorycode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'address2_freighttermscode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'address2_shippingmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'isprivate','Type':0,'Value':false},
                                                {'Name':'followemail','Type':0,'Value':true},
                                                {'Name':'tc_colleague','Type':0,'Value':false},
                                                {'Name':'donotbulkemail','Type':0,'Value':false},
                                                {'Name':'donotsendmm','Type':0,'Value':false},
                                                {'Name':'haschildrencode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'educationcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'customertypecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'fullname','Type':9,'Value':'John Jones'},
                                                {'Name':'isautocreate','Type':0,'Value':false},
                                                {'Name':'tc_deceased','Type':0,'Value':false},
                                                {'Name':'ownerid','Type':5,'Value':{'__type':'Lookup','Id':'a363ca44-4dd7-e611-8101-3863bb34fa70','LogicalName':'systemuser','Name':null}},
                                                {'Name':'isbackofficecustomer','Type':0,'Value':false},
                                                {'Name':'donotbulkpostalmail','Type':0,'Value':false},
                                                {'Name':'donotpostalmail','Type':0,'Value':false},
                                                {'Name':'tc_nolongerlivingataddress','Type':0,'Value':false},
                                                {'Name':'donotemail','Type':0,'Value':false},
                                                {'Name':'statecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':0}},
                                                {'Name':'tc_donotallowsms','Type':0,'Value':false},
                                                {'Name':'tc_marketing','Type':0,'Value':false},
                                                {'Name':'donotphone','Type':0,'Value':false},
                                                {'Name':'createdon','Type':1,'Value':'\/Date(1504199152000)\/'},
                                                {'Name':'transactioncurrencyid','Type':5,'Value':{'__type':'Lookup','Id':'2ecffb4d-33c7-e611-80fe-3863bb34da28','LogicalName':'transactioncurrency','Name':null}},
                                                {'Name':'contactid','Type':6,'Value':'8f9af6a1-6e8e-e711-8105-5065f38bc531'},
                                                {'Name':'tc_emailaddressavailable','Type':0,'Value':false},
                                                {'Name':'address2_addresstypecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'modifiedby','Type':5,'Value':{'__type':'Lookup','Id':'a363ca44-4dd7-e611-8101-3863bb34fa70','LogicalName':'systemuser','Name':null}},
                                                {'Name':'leadsourcecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'statuscode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000002}},
                                                {'Name':'modifiedonbehalfby','Type':10,'Value':null},
                                                {'Name':'preferredcontactmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'lastname','Type':9,'Value':'Jones'},
                                                {'Name':'firstname','Type':9,'Value':'John'},
                                                {'Name':'createdby','Type':5,'Value':{'__type':'Lookup','Id':'a363ca44-4dd7-e611-8101-3863bb34fa70','LogicalName':'systemuser','Name':null}},
                                                {'Name':'yomifullname','Type':9,'Value':'John Jones'},
                                                {'Name':'donotfax','Type':0,'Value':false},
                                                {'Name':'merged','Type':0,'Value':false},
                                                {'Name':'customersizecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'marketingonly','Type':0,'Value':false},
                                                {'Name':'tc_vip','Type':0,'Value':false},
                                                {'Name':'owningbusinessunit','Type':5,'Value':{'__type':'Lookup','Id':'29876693-2ec7-e611-80fe-3863bb34da28','LogicalName':'businessunit','Name':null}},
                                                {'Name':'shippingmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'tc_salutation','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000000}},
                                                {'Name':'creditonhold','Type':0,'Value':false},
                                                {'Name':'tc_address1_countryid','Type':5,'Value':{'__type':'Lookup','Id':'fa255c25-8a14-e711-810c-3863bb34fa70','LogicalName':'tc_country','Name':null}},
                                                {'Name':'modifiedon','Type':1,'Value':'\/Date(1504199152000)\/'},
                                                {'Name':'participatesinworkflow','Type':0,'Value':false},
                                                {'Name':'preferredappointmenttimecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'tc_disabledindicator','Type':0,'Value':false},
                                                {'Name':'tc_address1_postalcode','Type':9,'Value':'YO41 5SJ'},
                                                {'Name':'exchangerate','Type':2,'Value':1.0000000000}]}"
            };

            var entityId9 = Guid.NewGuid();
            var cacheEntity9 = new Entity("tc_entitycache", entityId9)
            {
                ["tc_entitycacheid"] = EntityCacheId9,
                ["tc_name"] = "User 9",
                ["createdon"] = DateTime.Now,
                ["statuscode"] = 1,
                ["tc_recordid"] = RecorId4,
                ["tc_type"] = "contact",
                ["tc_operation"] = new OptionSetValue(950000001),
                ["tc_data"] = @"{'Fields':[
                                                {'Name':'territorycode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'address2_freighttermscode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'address2_shippingmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'isprivate','Type':0,'Value':false},
                                                {'Name':'followemail','Type':0,'Value':true},
                                                {'Name':'tc_colleague','Type':0,'Value':false},
                                                {'Name':'donotbulkemail','Type':0,'Value':false},
                                                {'Name':'donotsendmm','Type':0,'Value':false},
                                                {'Name':'haschildrencode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'educationcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'customertypecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'fullname','Type':9,'Value':'John Singer'},
                                                {'Name':'isautocreate','Type':0,'Value':false},
                                                {'Name':'tc_deceased','Type':0,'Value':false},
                                                {'Name':'ownerid','Type':5,'Value':{'__type':'Lookup','Id':'04601adf-d8d7-e611-80f9-3863bb349770','LogicalName':'systemuser','Name':null}},
                                                {'Name':'isbackofficecustomer','Type':0,'Value':false},
                                                {'Name':'donotbulkpostalmail','Type':0,'Value':false},
                                                {'Name':'donotpostalmail','Type':0,'Value':false},
                                                {'Name':'tc_nolongerlivingataddress','Type':0,'Value':false},
                                                {'Name':'donotemail','Type':0,'Value':false},
                                                {'Name':'statecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':0}},
                                                {'Name':'tc_donotallowsms','Type':0,'Value':false},
                                                {'Name':'tc_marketing','Type':0,'Value':false},
                                                {'Name':'donotphone','Type':0,'Value':false},
                                                {'Name':'createdon','Type':1,'Value':'\/Date(1504519480000)\/'},
                                                {'Name':'transactioncurrencyid','Type':5,'Value':{'__type':'Lookup','Id':'2ecffb4d-33c7-e611-80fe-3863bb34da28','LogicalName':'transactioncurrency','Name':null}},
                                                {'Name':'contactid','Type':6,'Value':'c7870677-5891-e711-8104-5065f38b74a1'},
                                                {'Name':'tc_emailaddressavailable','Type':0,'Value':false},
                                                {'Name':'address2_addresstypecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'modifiedby','Type':5,'Value':{'__type':'Lookup','Id':'04601adf-d8d7-e611-80f9-3863bb349770','LogicalName':'systemuser','Name':null}},
                                                {'Name':'leadsourcecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'statuscode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000002}},
                                                {'Name':'modifiedonbehalfby','Type':10,'Value':null},
                                                {'Name':'preferredcontactmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'lastname','Type':9,'Value':'Singer'},
                                                {'Name':'firstname','Type':9,'Value':'John'},
                                                {'Name':'createdby','Type':5,'Value':{'__type':'Lookup','Id':'04601adf-d8d7-e611-80f9-3863bb349770','LogicalName':'systemuser','Name':null}},
                                                {'Name':'yomifullname','Type':9,'Value':'John Singer'},
                                                {'Name':'donotfax','Type':0,'Value':false},
                                                {'Name':'merged','Type':0,'Value':false},
                                                {'Name':'customersizecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'marketingonly','Type':0,'Value':false},
                                                {'Name':'tc_vip','Type':0,'Value':false},
                                                {'Name':'owningbusinessunit','Type':5,'Value':{'__type':'Lookup','Id':'29876693-2ec7-e611-80fe-3863bb34da28','LogicalName':'businessunit','Name':null}},
                                                {'Name':'shippingmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'tc_salutation','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000000}},
                                                {'Name':'creditonhold','Type':0,'Value':false},
                                                {'Name':'tc_address1_countryid','Type':5,'Value':{'__type':'Lookup','Id':'fa255c25-8a14-e711-810c-3863bb34fa70','LogicalName':'tc_country','Name':null}},
                                                {'Name':'modifiedon','Type':1,'Value':'\/Date(1504519480000)\/'},
                                                {'Name':'participatesinworkflow','Type':0,'Value':false},
                                                {'Name':'preferredappointmenttimecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'tc_disabledindicator','Type':0,'Value':false},
                                                {'Name':'tc_address1_postalcode','Type':9,'Value':'kt4 7al'},
                                                {'Name':'exchangerate','Type':2,'Value':1.0000000000}]}"
            };

            var entityId10 = Guid.NewGuid();
            var cacheEntity10 = new Entity("tc_entitycache", entityId10)
            {
                ["tc_entitycacheid"] = EntityCacheId10,
                ["tc_name"] = "User10",
                ["createdon"] = DateTime.Now,
                ["statuscode"] = 1,
                ["tc_recordid"] = RecorId5,
                ["tc_type"] = "contact",
                ["tc_operation"] = new OptionSetValue(950000001),
                ["tc_data"] = @"{'Fields':[
                                                {'Name':'territorycode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'address2_freighttermscode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'tc_gender','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000001}},
                                                {'Name':'address2_shippingmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'isprivate','Type':0,'Value':false},
                                                {'Name':'followemail','Type':0,'Value':true},
                                                {'Name':'tc_colleague','Type':0,'Value':false},
                                                {'Name':'donotbulkemail','Type':0,'Value':false},{
                                                'Name':'donotsendmm','Type':0,'Value':false},
                                                {'Name':'emailaddress1','Type':9,'Value':'last.friday@mail.com'},
                                                {'Name':'educationcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'customertypecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'fullname','Type':9,'Value':'Last Friday'},
                                                {'Name':'isautocreate','Type':0,'Value':false},
                                                {'Name':'tc_emailaddress1type','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000000}},
                                                {'Name':'tc_deceased','Type':0,'Value':false},
                                                {'Name':'ownerid','Type':5,'Value':{'__type':'Lookup','Id':'e851057b-5465-e711-8105-e0071b65fe61','LogicalName':'systemuser','Name':null}},
                                                {'Name':'isbackofficecustomer','Type':0,'Value':false},
                                                {'Name':'donotbulkpostalmail','Type':0,'Value':false},
                                                {'Name':'donotpostalmail','Type':0,'Value':false},
                                                {'Name':'tc_nolongerlivingataddress','Type':0,'Value':false},
                                                {'Name':'donotemail','Type':0,'Value':false},
                                                {'Name':'statecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':0}},
                                                {'Name':'tc_donotallowsms','Type':0,'Value':false},
                                                {'Name':'tc_marketing','Type':0,'Value':false},
                                                {'Name':'donotphone','Type':0,'Value':false},
                                                {'Name':'createdon','Type':1,'Value':'\/Date(1504536853000)\/'},
                                                {'Name':'transactioncurrencyid','Type':5,'Value':{'__type':'Lookup','Id':'2ecffb4d-33c7-e611-80fe-3863bb34da28','LogicalName':'transactioncurrency','Name':null}},
                                                {'Name':'contactid','Type':6,'Value':'ce8dfbe9-8091-e711-8105-5065f38bb571'},
                                                {'Name':'tc_emailaddressavailable','Type':0,'Value':true},
                                                {'Name':'address2_addresstypecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'haschildrencode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'modifiedby','Type':5,'Value':{'__type':'Lookup','Id':'e851057b-5465-e711-8105-e0071b65fe61','LogicalName':'systemuser','Name':null}},
                                                {'Name':'leadsourcecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'statuscode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000002}},
                                                {'Name':'modifiedonbehalfby','Type':10,'Value':null},
                                                {'Name':'preferredcontactmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'lastname','Type':9,'Value':'Friday'},
                                                {'Name':'firstname','Type':9,'Value':'Last'},
                                                {'Name':'createdby','Type':5,'Value':{'__type':'Lookup','Id':'e851057b-5465-e711-8105-e0071b65fe61','LogicalName':'systemuser','Name':null}},
                                                {'Name':'yomifullname','Type':9,'Value':'Last Friday'},
                                                {'Name':'donotfax','Type':0,'Value':false},
                                                {'Name':'merged','Type':0,'Value':false},
                                                {'Name':'customersizecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'marketingonly','Type':0,'Value':false},
                                                {'Name':'tc_vip','Type':0,'Value':false},
                                                {'Name':'owningbusinessunit','Type':5,'Value':{'__type':'Lookup','Id':'29876693-2ec7-e611-80fe-3863bb34da28','LogicalName':'businessunit','Name':null}},
                                                {'Name':'shippingmethodcode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'tc_emailaddress1confirmation','Type':9,'Value':'last.friday@mail.com'},
                                                {'Name':'tc_salutation','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':950000000}},
                                                {'Name':'creditonhold','Type':0,'Value':false},
                                                {'Name':'tc_address1_countryid','Type':5,'Value':{'__type':'Lookup','Id':'48265c25-8a14-e711-810c-3863bb34fa70','LogicalName':'tc_country','Name':null}},
                                                {'Name':'modifiedon','Type':1,'Value':'\/Date(1504536853000)\/'},
                                                {'Name':'participatesinworkflow','Type':0,'Value':false},
                                                {'Name':'preferredappointmenttimecode','Type':8,'Value':{'__type':'OptionSet','Name':null,'Value':1}},
                                                {'Name':'tc_disabledindicator','Type':0,'Value':false},
                                                {'Name':'tc_address1_postalcode','Type':9,'Value':'OB 45647'},
                                                {'Name':'exchangerate','Type':2,'Value':1.0000000000}]}"
            };

            collection.Add(entity1Id, cacheEntity1);
            collection.Add(entityId2, cacheEntity2);
            collection.Add(entityId3, cacheEntity3);
            collection.Add(entityId4, cacheEntity4);
            collection.Add(entityId5, cacheEntity5);
            collection.Add(entity6Id, cacheEntity6);
            collection.Add(entityId7, cacheEntity7);
            collection.Add(entityId8, cacheEntity8);
            collection.Add(entityId9, cacheEntity9);
            collection.Add(entityId10, cacheEntity10);

            Context.Data.Add("tc_entitycache", collection);
        }

        private void AddConfigurations()
        {
            var collection = new Dictionary<Guid, Entity>();
            var entity1Id = Guid.NewGuid();
            var configuration1 = new Entity("tc_configuration", entity1Id)
            {
                ["tc_name"] = "Tc.OutboundSynchronisation.SsoServiceUrl",
                ["tc_value"] = "http://localhost:8080/int/GetCacheEntity"
            };

            var entity2Id = Guid.NewGuid();
            var configuration2 = new Entity("tc_configuration", entity2Id)
            {
                ["tc_name"] = "Tc.OutboundSynchronisation.SsoTokenExpiredSeconds",
                ["tc_value"] = "100"
            };

            var entity3Id = Guid.NewGuid();
            var configuration3 = new Entity("tc_configuration", entity3Id)
            {
                ["tc_name"] = "Tc.OutboundSynchronisation.SsoTokenNotBeforeTimeSeconds",
                ["tc_value"] = "100"
            };

            collection.Add(entity1Id, configuration1);
            collection.Add(entity2Id, configuration2);
            collection.Add(entity3Id, configuration3);

            Context.Data.Add("tc_configuration", collection);
        }

        private void AddContacts()
        {
            var collection = new Dictionary<Guid, Entity>();
            var contact1 = new Entity("contact", RecorId1)
            {
                ["contactid"] = RecorId1,
                ["tc_sourcesystemid"] = "Sys1"
            };

            var contact2 = new Entity("contact", RecorId2)
            {
                ["contactid"] = RecorId2,
                ["tc_sourcesystemid"] = "Sys2"
            };

            var contact3 = new Entity("contact", RecorId3)
            {
                ["contactid"] = RecorId3,
                ["tc_sourcesystemid"] = "Sys3"
            };

            var contact4 = new Entity("contact", RecorId4)
            {
                ["contactid"] = RecorId4,
                ["tc_sourcesystemid"] = "Sys4"
            };

            var contact5 = new Entity("contact", RecorId5)
            {
                ["contactid"] = RecorId5,
                ["tc_sourcesystemid"] = "Sys5"
            };

            collection.Add(RecorId1, contact1);
            collection.Add(RecorId2, contact2);
            collection.Add(RecorId3, contact3);
            collection.Add(RecorId4, contact4);
            collection.Add(RecorId5, contact5);

            Context.Data.Add("contact", collection);
        }

        private void AddEntityCacheMessages()
        {
            var collection = new Dictionary<Guid, Entity>();
            var entitycachemessage1 = new Entity("tc_entitycachemessage", EntityCacheId6)
            {
                ["tc_entitycacheid"] = EntityCacheId6,
                ["statuscode"] = 1
            };

            var entitycachemessage2 = new Entity("tc_entitycachemessage", EntityCacheId7)
            {
                ["tc_entitycacheid"] = EntityCacheId7,
                ["statuscode"] = 1
            };

            var entitycachemessage3 = new Entity("tc_entitycachemessage", EntityCacheId8)
            {
                ["tc_entitycacheid"] = EntityCacheId8,
                ["statuscode"] = 1
            };

            var entitycachemessage4 = new Entity("tc_entitycachemessage", EntityCacheId9)
            {
                ["tc_entitycacheid"] = EntityCacheId9,
                ["statuscode"] = 1
            };

            var entitycachemessage5 = new Entity("tc_entitycachemessage", EntityCacheId10)
            {
                ["tc_entitycacheid"] = EntityCacheId10,
                ["statuscode"] = 1
            };

            collection.Add(RecorId1, entitycachemessage1);
            collection.Add(RecorId2, entitycachemessage2);
            collection.Add(RecorId3, entitycachemessage3);
            collection.Add(RecorId4, entitycachemessage4);
            collection.Add(RecorId5, entitycachemessage5);

            Context.Data.Add("tc_entitycachemessage", collection);
        }

        private void AddSecurityConfigurations()
        {
            var collection = new Dictionary<Guid, Entity>();
            var entity = Guid.NewGuid();
            var configuration = new Entity("tc_secureconfiguration", entity)
            {
                ["tc_name"] = "Tc.OutboundSynchronisation.JwtPrivateKey",
                ["tc_value"] = @"<RSAKeyValue>
  <Modulus>29xdU6ptfXHxO4anp6bLjjIbl+Fczr6B39sU9cxv9fKC6bjxQ4cX+RwsLs6PnGWRz0vw+pjazldtEit5Wwk8WNlcKOGNgs2BhWCMHYs9mgu+NlJAXdG8cPxZQKWzGJA+bmp2MnCj1XGN2wTDn+Ah3piZqAPHXlUUUe6AHlmyMwDBkR2tCUXVWVv+VCZuuuv9HVmDVSqUEx+YrKD7kiQgKk4AV25p3eV0b4Vze1sYK+1MiN4/QqBQPAiPPUlPYfm9VAES3t/U4ylbek7tVaS9d5BinGXkOnQXV6sbyRuKtaaDCX0XoNcWzOxYwiHrnO88D88a8SnhNqKIJokPTwLiLw==</Modulus>
  <Exponent>AQAB</Exponent>
  <P>/ON3B5UJcIsciTPnhJ2zAGOM4C/viAm64/JBYSMm8udVi4+TGrQRGwGa7H9zKx4wHr9EEsqdJpMz9AUzeSSnaKOXE+tHsqLn9LwW3lvHw7zFqEwyb30vFXEQWX1zDeiHxJc7Ukw+1RrFuP7ZuXf8flMR0YoTNgGgrowC3eDpEiM=</P>
  <Q>3pDe3ZMe5owxSUMFKDSxHqvIop3tGyDIEdvzkA2aXNeQ3aNaEySh+KI9k9rbpQ6cA9ZV43abfo7PSSiSwZWLzTmuLqx3W8abmkNAWcKWK0uLt61+aJVoYjO/AlboRHyr+J1mSzApGL5P4rYMb4w572+ivqw2GOTN7pcPak4iEoU=</Q>
  <DP>DlTNsA5QJKKdkWDxo+BT/pelqibNSkZS4wwdjGWzlVxqyqfuTDscJQ2oO/LVEgJ586QfNXlqAn+hGBkbW6gqHJH4w9Y3j/YPcx0dpqhI39zYzrrSuOK9QlfP92JWnNkqqIdxgy5y+Ry1S9CVgh88neQTRG6wvATHmFyy5OQUEUU=</DP>
  <DQ>oW/xyES7yDzuTxbG+dfmlbnDCXmGEARiOtoRPG8xhaBzGuEvJ+2Ncxyzj7jTU1Fah0oD+L8CoPUTlBxS/wnrYwwwtPgyh6ZzHZ0kYzdK19KvYKb+pvugwIKKTTceuPa5gtcg6O7hEGqS5X5pXMwZBf0yzh16C+qDGUoHS3OrMUU=</DQ>
  <InverseQ>+IP26tjXOHVO2MiWZU6ZXC7UB1c5hQzfYTsaZ43WfMoABkCZ9WvTVHnvrPAnA0ywaoAiFJb37a3XY4zzayb3ECKMPPC8nFulIxJex5CeSimVchKTPxc6wPpVJFfS4WYPTOXOmrXz7z6KdkwzrmXD5baD+Z6XOu9YHAM4cb1d10k=</InverseQ>
  <D>DkqQtnOJknHpoFjsZPVundub949qnPW9M3PmNOQJEw+ketTOufj6EfNG2/QJWb0wcS0aiO+OqYL9UAULamN7TLs0RmQC8tGw7Z6M/Q6j/nNs9dL65B5SBXYhxxX+QkZ+CGdbL4Qq4iDze50fqjDDgtighE9akkMtgvXh1hc2giIXix9hWry0DE8ngsXSWO8b/AU+k4z5b+VeoN6c/h+SNKkTJghQgnX33MVtOGS3+VmKhImXxUOb16Eo6GE44n8BJzKmIzl6LUYduJCJcwJjQM/zt+fU1nJ83v9xxtMH1hLf0SpDEkM3gkIfH9EWoR7CKjjjc6pNWHFHgX4jC2zrIQ==</D>
</RSAKeyValue>"
            };

            collection.Add(entity, configuration);

            Context.Data.Add("tc_secureconfiguration", collection);
        }

        private void PrepareData()
        { 
            Context.Data.Clear();
            AddCacheEntities();
            AddConfigurations();
            AddSecurityConfigurations();
            AddContacts();
            AddEntityCacheMessages();
        }
    }
}