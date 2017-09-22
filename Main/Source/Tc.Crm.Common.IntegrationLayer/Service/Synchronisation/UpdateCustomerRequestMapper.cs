using System;
using System.Collections.Generic;
using Tc.Crm.Common.IntegrationLayer.Helper;
using Tc.Crm.Common.IntegrationLayer.Model;
using Attributes = Tc.Crm.Common.Constants.Attributes;

namespace Tc.Crm.Common.IntegrationLayer.Service.Synchronisation
{
    /// <summary>
    /// Mapper class to update payload of syncronisation request for customer update
    /// </summary>
    public class UpdateCustomerRequestMapper : IEntityCacheMapper
    {
        private const string Replace = "replace";
        private const string Customeridentity = "customer/customerIdentity";

        /// <inheritdoc />
        /// <summary>
        /// Creates Tc.Crm.Common.IntegrationLayer.Model.Schema.Customer class and fills properties required to update request
        /// </summary>
        /// <param name="model">Customer record entity model</param>
        /// <returns>mapped Tc.Crm.Common.IntegrationLayer.Model.Schema.Customer</returns>
        public object Map(EntityModel model)
        {
            List<PatchElement> elements = null;
            foreach (var field in model.Fields)
            {
                var element = new PatchElement();
                
                var mapped =
                    FieldMapHelper.TryMapField(Attributes.Customer.Salutation, field, $"{Replace}", $"{Customeridentity}/salutation", UpdateElement(element)) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.FirstName, field, $"{Replace}", $"{Customeridentity}/firstName", UpdateElement(element)) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.LastName, field, $"{Replace}", $"{Customeridentity}/lastName", UpdateElement(element)) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.Language, field, $"{Replace}", $"{Customeridentity}/language", UpdateElement(element)) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.Birthdate, field, $"{Replace}", $"{Customeridentity}/birthdate", UpdateElement(element));
                if (mapped)
                {
                    if (elements == null)
                        elements = new List<PatchElement> {element};
                    else
                        elements.Add(element);
                }
            }
            return elements;
        }

        private static Action<string, string, string> UpdateElement(PatchElement element)
        {
            return (op, path, value) =>
            {
                element.Operator = op;
                element.Path = path;
                element.Value = value;
            };
        }
    }
}
