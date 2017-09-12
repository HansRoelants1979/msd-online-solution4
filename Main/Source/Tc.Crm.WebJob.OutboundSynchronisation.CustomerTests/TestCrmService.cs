using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Common.Models;
using Tc.Crm.Common.Services;

namespace Tc.Crm.OutboundSynchronisation.CustomerTests
{
    public enum DataSwitch
    {
        NoRecordsReturned = 0,
        CollectionWithNoRecordsReturned = 1,
        ReturnsData = 2,
    }
    public class TestCrmService : ICrmService
    {
        public DataSwitch Switch { get; set; }
        public XrmFakedContext Context { get; set; }

        public TestCrmService()
        {
            Context = new XrmFakedContext();
            PrepareData();
        }

        public void AddCacheEntities()
        {
            var cacheEntityCollection = new Dictionary<Guid, Entity>();
            var entity1Id = Guid.NewGuid();
            var cacheEntity1 = new Entity("tc_entitycache", entity1Id)
            {
                ["tc_name"] = "Dave Dave",
                ["createdon"] = DateTime.Now,
                ["statuscode"] = 1,
                ["tc_type"] = "contact",
                ["tc_operation"] = 950000000,
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
                ["tc_name"] = "Cache entity 2",
                ["createdon"] = DateTime.Now,
                ["statuscode"] = 1,
                ["tc_type"] = "contact",
                ["tc_operation"] = 950000000,
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
                ["tc_name"] = "Cache entity 3",
                ["createdon"] = DateTime.Now,
                ["statuscode"] = 1,
                ["tc_type"] = "contact",
                ["tc_operation"] = 950000000,
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
                ["tc_name"] = "Cache entity 4",
                ["createdon"] = DateTime.Now,
                ["statuscode"] = 1,
                ["tc_type"] = "contact",
                ["tc_operation"] = 950000000,
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
                ["tc_name"] = "Cache entity 5",
                ["createdon"] = DateTime.Now,
                ["statuscode"] = 1,
                ["tc_type"] = "contact",
                ["tc_operation"] = 950000000,
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

            cacheEntityCollection.Add(entity1Id, cacheEntity1);
            cacheEntityCollection.Add(entityId2, cacheEntity2);
            cacheEntityCollection.Add(entityId3, cacheEntity3);
            cacheEntityCollection.Add(entityId4, cacheEntity4);
            cacheEntityCollection.Add(entityId5, cacheEntity5);

            Context.Data.Add("tc_entitycache", cacheEntityCollection);
        }

        public void PrepareData()
        {
            Context.Data.Clear();
            AddCacheEntities();
        }

        public void UpdateStatus(Entity cacheEntity, int status)
        {
            cacheEntity["statuscode"] = status;
        }

        public EntityCollection RetrieveMultipleRecordsFetchXml(string query, int numberOfElements)
        {
            return RetrieveMultipleRecordsFetchXml(query);
        }

        public void BulkAssign(Collection<AssignInformation> assignRequests)
        {
            throw new NotImplementedException();
        }

        public void BulkUpdate(IEnumerable<Entity> entities)
        {
            throw new NotImplementedException();
        }

        public EntityCollection RetrieveMultipleRecordsFetchXml(string query)
        {
            if (Switch == DataSwitch.NoRecordsReturned)
                return null;
            if (Switch == DataSwitch.CollectionWithNoRecordsReturned)
                return new EntityCollection();

            if (Switch == DataSwitch.ReturnsData)
            {
                return GetCacheEntities(Context.Data["tc_entitycache"].Values.ToList());
            }

            return null;
        }

        private EntityCollection GetCacheEntities(List<Entity> entityCacheCollection)
        {
            var cacheEntities = entityCacheCollection
                .Where(entity =>
                    (string) entity.Attributes["tc_type"] == "contact" &&
                    (int) entity.Attributes["statuscode"] == 1 &&
                    (int) entity.Attributes["tc_operation"] == 950000000)
                .OrderBy(entity => (DateTime) entity.Attributes["createdon"]).ToList();

            return new EntityCollection(cacheEntities);
        }

        public Guid Create(Entity entity)
        {
           return Guid.NewGuid();
        }

        public void Update(Entity entity)
        {
        }
    }
}