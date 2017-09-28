using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using Tc.Usd.SingleSignOnLogin.Models;
using Microsoft.Xrm.Sdk.Messages;

namespace Tc.Usd.SingleSignOnLogin.Services
{
    internal class BudgetCentreService
    {
        private readonly Guid _myGuid;
        private readonly CrmServiceClient _crmService;
        private readonly ColumnSet _storeColumnSet = new ColumnSet("tc_storeid", "tc_abta", "tc_clusterid", "tc_ukregionid", "tc_name", "tc_budgetcentre");

        internal BudgetCentreService(CrmServiceClient crmClient)
        {
            _crmService = crmClient;
            _myGuid = crmClient.GetMyCrmUserId();
        }

        public List<BudgetCentre> GetBudgetCentre()
        {
            var lst = new List<Entity>();
            var primaryStore = GetPrimaryStore();
            if (primaryStore != null)
            {
                lst.Add(primaryStore);
            }
            lst.AddRange(GetRelatedStores());
            return lst.Select(ConvertEntityToBudgetCentre).ToList();
        }

        private List<Entity> GetRelatedStores()
        {
            var query = new QueryExpression
            {
                EntityName = "tc_store",
                ColumnSet = _storeColumnSet
            };

            var linkEntStore = new LinkEntity("tc_store", "tc_store_systemuser", "tc_storeid", "tc_storeid", JoinOperator.Inner);
            var linkEntUser = new LinkEntity("tc_store_systemuser", "systemuser", "systemuserid", "systemuserid", JoinOperator.Inner);
            linkEntStore.LinkEntities.Add(linkEntUser);
            query.LinkEntities.Add(linkEntStore);
            linkEntUser.LinkCriteria = new FilterExpression();
            linkEntUser.LinkCriteria.AddCondition("systemuserid", ConditionOperator.Equal, _myGuid);

            var req = new RetrieveMultipleRequest
            {
                Query = query
            };

            return ((RetrieveMultipleResponse)_crmService.ExecuteCrmOrganizationRequest(req)).EntityCollection.Entities.ToList();
        }

        private Entity GetPrimaryStore()
        {
            var suReq = new RetrieveRequest()
            {
                ColumnSet = new ColumnSet("tc_primarystoreid"),
                Target = new EntityReference("systemuser", _myGuid)
            };
            var systemuser = ((RetrieveResponse)_crmService.ExecuteCrmOrganizationRequest(suReq)).Entity;
            if (!systemuser.Contains("tc_primarystoreid"))
                return null;

            var primaryStoreReq = new RetrieveRequest()
            {
                ColumnSet = _storeColumnSet,
                Target = new EntityReference("tc_store", ((EntityReference)systemuser["tc_primarystoreid"]).Id)
            };
            return ((RetrieveResponse)_crmService.ExecuteCrmOrganizationRequest(primaryStoreReq)).Entity;
        }

        public Guid UpsertExternalLogin(string userInitials, string abta, Guid bcId, string branchcode)
        {
            var externalLoginQuery = new QueryExpression("tc_externallogin")
            {
                ColumnSet = new ColumnSet("tc_initials", "tc_abtanumber", "tc_budgetcentreid", "tc_employeeid"),
                Criteria = new FilterExpression()
            };
            externalLoginQuery.Criteria.AddCondition("ownerid", ConditionOperator.Equal, _myGuid);

            var extLoginReq = new RetrieveMultipleRequest {Query = externalLoginQuery};
            
            var extLogin = ((RetrieveMultipleResponse)_crmService.ExecuteCrmOrganizationRequest(extLoginReq)).EntityCollection.Entities.FirstOrDefault();

            var extLoginId = extLogin == null ? CreateNewExternalLogin(userInitials, abta, bcId, branchcode) : UpdateExternalLogin(userInitials, abta, bcId, branchcode, extLogin);
            return extLoginId;
        }

        private Guid UpdateExternalLogin(string userInitials, string abta, Guid bcId, string branchcode,  Entity extLogin)

        {
            extLogin["tc_initials"] = userInitials;
            extLogin["tc_abtanumber"] = abta;
            extLogin["tc_budgetcentreid"] = new EntityReference("tc_store", bcId);
            extLogin["tc_branchcode"] = branchcode;

            var updateReq = new UpdateRequest() {Target = extLogin};
            _crmService.ExecuteCrmOrganizationRequest(updateReq);
            return extLogin.Id;
        }

        private Guid CreateNewExternalLogin(string userInitials, string abta, Guid bcId, string branchcode)
        {
            var extLoginEntity = new Entity("tc_externallogin");
            extLoginEntity.Attributes.Add("tc_initials", userInitials);
            extLoginEntity.Attributes.Add("tc_abtanumber", abta);
            extLoginEntity.Attributes.Add("tc_branchcode", branchcode);
            extLoginEntity.Attributes.Add("tc_budgetcentreid", new EntityReference("tc_store", bcId));
            extLoginEntity.Attributes.Add("ownerid", new EntityReference("systemuser", _myGuid));

            var createReq = new CreateRequest {Target = extLoginEntity};
            return ((CreateResponse)_crmService.ExecuteCrmOrganizationRequest(createReq)).id;
        }

        private static BudgetCentre ConvertEntityToBudgetCentre(Entity entity)
        {
            return new BudgetCentre
            {
                StoreId = (Guid)entity["tc_storeid"],
                Name = (string)entity["tc_name"],
                Abta = (string)entity["tc_abta"],
                BudgetCentreName = (string)entity["tc_budgetcentre"],
                Cluster = ((EntityReference)entity["tc_clusterid"]).Name,
                Region = ((EntityReference)entity["tc_ukregionid"]).Name
            };
        }
    }
}