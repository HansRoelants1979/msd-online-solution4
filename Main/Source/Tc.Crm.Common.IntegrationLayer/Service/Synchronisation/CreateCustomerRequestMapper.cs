using System;
using Tc.Crm.Common.IntegrationLayer.Helper;
using Tc.Crm.Common.IntegrationLayer.Model;
using Tc.Crm.Common.IntegrationLayer.Model.Schema;
using Attributes = Tc.Crm.Common.Constants.Attributes;

namespace Tc.Crm.Common.IntegrationLayer.Service.Synchronisation
{
    /// <summary>
    /// Mapper class to create payload of syncronisation request for customer creation
    /// </summary>
    public class CreateCustomerRequestMapper : IEntityCacheMapper
    {

        /// <summary>
        /// Creates Tc.Crm.Common.IntegrationLayer.Model.Schema.Customer class and fills properties required to create request
        /// </summary>
        /// <param name="recordId">Customer record guid</param>
        /// <param name="model">Customer record entity model</param>
        /// <returns>mapped Tc.Crm.Common.IntegrationLayer.Model.Schema.Customer</returns>
        public object Map(string recordId, EntityModel model)
        {
            const int fieldsToMapCount = 5;
            var mappedFieldsCount = 0;
            var customer = new Customer();
            customer.CustomerIdentifier = new CustomerIdentifier();
            customer.CustomerIdentity = new CustomerIdentity();
            customer.CustomerIdentifier.CustomerId = recordId;
            foreach (var field in model.Fields)
            {
                var mapped = 
                    FieldMapHelper.TryMapField(Attributes.Customer.Salutation, field, value => customer.CustomerIdentity.Salutation = value) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.FirstName, field, value => customer.CustomerIdentity.FirstName = value) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.LastName, field, value => customer.CustomerIdentity.LastName = value) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.Language, field, value => customer.CustomerIdentity.Language = value) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.Birthdate, field, value => customer.CustomerIdentity.Birthdate = value);
                if (mapped && ++mappedFieldsCount == fieldsToMapCount) break;
            }
            return customer;
        }
    }
}
