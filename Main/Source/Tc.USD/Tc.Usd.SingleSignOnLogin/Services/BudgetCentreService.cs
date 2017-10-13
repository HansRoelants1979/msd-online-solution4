using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using Tc.Usd.SingleSignOnLogin.Models;
using Microsoft.Xrm.Sdk.Messages;
using Tc.Crm.Common.Constants;
using Attributes = Tc.Crm.Common.Constants.Attributes;

namespace Tc.Usd.SingleSignOnLogin.Services
{
    internal class BudgetCentreService
    {
        private readonly Guid _myGuid;
        private readonly CrmServiceClient _crmService;
        private readonly ColumnSet _storeColumnSet = new ColumnSet(Attributes.Store.StoreId, Attributes.Store.Abta, Attributes.Store.ClusterId, Attributes.Store.UkRegionId, Attributes.Store.Name, Attributes.Store.BudgetCentre, Attributes.Store.StoreClosed);

        internal BudgetCentreService(CrmServiceClient crmClient)
        {
            _crmService = crmClient;
            _myGuid = crmClient.GetMyCrmUserId();
        }

        public List<BudgetCentre> GetBudgetCentre(bool allBudgetCentreAccess)
        {
            var result = new List<BudgetCentre>();
            var primaryStore = GetPrimaryStore();
            if (primaryStore != null && primaryStore.Attributes.Contains(Attributes.Store.StoreClosed) && !(bool)primaryStore.Attributes[Attributes.Store.StoreClosed])
            {
				result.Add(ConvertEntityToBudgetCentre(primaryStore));
				result.Add(new BudgetCentre { IsPrimary = true });
			}
			var stores = GetStores(allBudgetCentreAccess).Where(b => primaryStore == null || (Guid)b[Attributes.Store.StoreId] != (Guid)primaryStore[Attributes.Store.StoreId]);
			bool hasRegular = false;
			bool? closed = null;
			foreach (var store in stores)
			{
				hasRegular = hasRegular || store.Attributes.Contains(Attributes.Store.StoreClosed) && !(bool)store.Attributes[Attributes.Store.StoreClosed];
				if (hasRegular && !closed.HasValue && store.Attributes.Contains(Attributes.Store.StoreClosed) && (bool)store.Attributes[Attributes.Store.StoreClosed])
				{
					closed = true;
					result.Add(new BudgetCentre { IsClosed = true });
				}
				result.Add(ConvertEntityToBudgetCentre(store));
			}
			
            return result;
        }

        private List<Entity> GetStores(bool loadAllBudgetStores)
        {
			QueryBase query;
            if (!loadAllBudgetStores)
            {
				var request = $@"<fetch distinct='true' >
									<entity name='{EntityName.Store}' >
										<attribute name='{Attributes.Store.StoreId}' />
										<attribute name='{Attributes.Store.Abta}' />
										<attribute name='{Attributes.Store.ClusterId}' />
										<attribute name='{Attributes.Store.UkRegionId}' />
										<attribute name='{Attributes.Store.Name}' />
										<attribute name='{Attributes.Store.BudgetCentre}' />
										<attribute name='{Attributes.Store.StoreClosed}' />
										<filter type='or' >
											<filter type='and' >
												<condition attribute='{Attributes.Store.StoreClosed}' operator='eq' value='0' />
												<condition entityname='assigned' attribute='{Attributes.User.UserId}' operator='eq' value='{_myGuid}' />
											</filter>
											<filter type='and' >
												<condition attribute='{Attributes.Store.StoreClosed}' operator='eq' value='1' />
												<condition entityname='admin_regular_user' attribute='{Attributes.User.UserId}' operator='eq' value='{_myGuid}' />
											</filter>
											<filter type='and' >
												<condition attribute='{Attributes.Store.StoreClosed}' operator='eq' value='1' />
												<condition entityname='admin_primary_user' attribute='{Attributes.User.UserId}' operator='eq' value='{_myGuid}' />
											</filter>
										</filter>
										<link-entity name='{EntityRelationName.StoreUser}' from='{Attributes.Store.StoreId}' to='{Attributes.Store.StoreId}' link-type='outer' alias='assigned' intersect='true' />
										<link-entity name='{EntityName.Store}' from='{Attributes.Store.StoreId}' to='{Attributes.Store.AdminHostBudgetCenter}' link-type='outer' alias='admin_regular' >
											<link-entity name='{EntityRelationName.StoreUser}' from='{Attributes.Store.StoreId}' to='{Attributes.Store.StoreId}' link-type='outer' alias='admin_regular_user' intersect='true' />
										</link-entity>
										<link-entity name='{EntityName.Store}' from='{Attributes.Store.StoreId}' to='{Attributes.Store.AdminHostBudgetCenter}' link-type='outer' alias='admin_primary' >
											<link-entity name='{EntityName.User}' from='{Attributes.User.PrimaryStoreId}' to='{Attributes.Store.StoreId}' link-type='outer' alias='admin_primary_user' />
										</link-entity>
										<order attribute='{Attributes.Store.StoreClosed}' />
										<order attribute='{Attributes.Store.Name}' />
									</entity>
								</fetch>";
				query = new FetchExpression(request);
			}
			else
			{
				query = new QueryExpression
				{
					EntityName = EntityName.Store,
					ColumnSet = _storeColumnSet
				};
			}
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
                ColumnSet = new ColumnSet(Attributes.User.PrimaryStoreId),
                Target = new EntityReference(EntityName.User, _myGuid)
            };
            var systemuser = ((RetrieveResponse)_crmService.ExecuteCrmOrganizationRequest(suReq)).Entity;
            if (!systemuser.Contains(Attributes.User.PrimaryStoreId))
                return null;

            var primaryStoreReq = new RetrieveRequest()
            {
                ColumnSet = _storeColumnSet,
                Target = new EntityReference(EntityName.Store, ((EntityReference)systemuser[Attributes.User.PrimaryStoreId]).Id)
            };
            return ((RetrieveResponse)_crmService.ExecuteCrmOrganizationRequest(primaryStoreReq)).Entity;
        }

        public Guid UpsertExternalLogin(string userInitials, string abta, Guid bcId, string branchcode, string employeeId, string name)
        {
            var externalLoginQuery = new QueryExpression(EntityName.ExternalLogin)
            {
                ColumnSet = new ColumnSet(Attributes.ExternalLogin.Initials, Attributes.ExternalLogin.AbtaNumber, Attributes.ExternalLogin.BudgetCentreId, Attributes.ExternalLogin.EmployeeId, Attributes.ExternalLogin.Name),
                Criteria = new FilterExpression()
            };
            externalLoginQuery.Criteria.AddCondition("ownerid", ConditionOperator.Equal, _myGuid);

            var extLoginReq = new RetrieveMultipleRequest {Query = externalLoginQuery};
            
            var extLogin = ((RetrieveMultipleResponse)_crmService.ExecuteCrmOrganizationRequest(extLoginReq)).EntityCollection.Entities.FirstOrDefault();

            var extLoginId = extLogin == null ? CreateNewExternalLogin(userInitials, abta, bcId, branchcode, employeeId, name) : UpdateExternalLogin(userInitials, abta, bcId, branchcode, employeeId, name, extLogin);
            return extLoginId;
        }

        private Guid UpdateExternalLogin(string userInitials, string abta, Guid bcId, string branchcode, string employeeId, string name,  Entity extLogin)

        {
            extLogin[Attributes.ExternalLogin.Initials] = userInitials;
            extLogin[Attributes.ExternalLogin.AbtaNumber] = abta;
            extLogin[Attributes.ExternalLogin.BudgetCentreId] = new EntityReference(EntityName.Store, bcId);
            extLogin[Attributes.ExternalLogin.BranchCode] = branchcode;
            extLogin[Attributes.ExternalLogin.EmployeeId] = employeeId;
            extLogin[Attributes.ExternalLogin.Name] = name;

            var updateReq = new UpdateRequest() {Target = extLogin};
            _crmService.ExecuteCrmOrganizationRequest(updateReq);
            return extLogin.Id;
        }

        private Guid CreateNewExternalLogin(string userInitials, string abta, Guid bcId, string branchcode, string employeeId, string name)
        {
            var extLoginEntity = new Entity(EntityName.ExternalLogin);
            extLoginEntity.Attributes.Add(Attributes.ExternalLogin.Initials, userInitials);
            extLoginEntity.Attributes.Add(Attributes.ExternalLogin.AbtaNumber, abta);
            extLoginEntity.Attributes.Add(Attributes.ExternalLogin.BranchCode, branchcode);
            extLoginEntity.Attributes.Add(Attributes.ExternalLogin.BudgetCentreId, new EntityReference(EntityName.Store, bcId));
            extLoginEntity.Attributes.Add(Attributes.ExternalLogin.OwnerId, new EntityReference(EntityName.User, _myGuid));
            extLoginEntity.Attributes.Add(Attributes.ExternalLogin.EmployeeId, employeeId);
            extLoginEntity.Attributes.Add(Attributes.ExternalLogin.Name, name);

            var createReq = new CreateRequest {Target = extLoginEntity};
            return ((CreateResponse)_crmService.ExecuteCrmOrganizationRequest(createReq)).id;
        }

        private static BudgetCentre ConvertEntityToBudgetCentre(Entity entity)
        {
			return new BudgetCentre
			{
				StoreId = (Guid)entity[Attributes.Store.StoreId],
				Name = $"{(string)entity[Attributes.Store.BudgetCentre]}({(string)entity[Attributes.Store.Name]})",
				Abta = (string)entity[Attributes.Store.Abta],
				BudgetCentreName = (string)entity[Attributes.Store.BudgetCentre],
				Cluster = entity.Contains(Attributes.Store.ClusterId) ? ((EntityReference)entity[Attributes.Store.ClusterId]).Name : string.Empty,
				Region = ((EntityReference)entity[Attributes.Store.UkRegionId]).Name
			};
        }
    }
}