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
        private const string Identity = "customer/customerIdentity";
        private const string Identifier = "customer/customerIdentifier";
        private const string General = "customer/customerGeneral";
        private const string Additional = "customer/additional";
        private const string Address1 = "customer/address1";
        private const string Address2 = "customer/address2";
        private const string Phone1 = "customer/phone1";
        private const string Phone2 = "customer/phone2";
        private const string Phone3 = "customer/phone3";
		private const string Email1 = "customer/email1";
        private const string Email2 = "customer/email2";
        private const string Email3 = "customer/email3";
        private const string Company = "customer/company";

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
					////////////////////////////////////////CustomerIdentity//////////////////////////////////////////////////
					FieldMapHelper.TryMapField(Attributes.Customer.Salutation, field, $"{Replace}", $"{Identity}/salutation", UpdateElement(element)) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.FirstName, field, $"{Replace}", $"{Identity}/firstName", UpdateElement(element)) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.LastName, field, $"{Replace}", $"{Identity}/lastName", UpdateElement(element)) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.Language, field, $"{Replace}", $"{Identity}/language", UpdateElement(element)) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.Birthdate, field, $"{Replace}", $"{Identity}/birthdate", UpdateElement(element)) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.AcademicTitle, field, $"{Replace}", $"{Identity}/academicTitle", UpdateElement(element)) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.MiddleName, field, $"{Replace}", $"{Identity}/middleName", UpdateElement(element)) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.Gender, field, $"{Replace}", $"{Identity}/gender", UpdateElement(element, typeof(Model.Schema.Gender))) ||
					////////////////////////////////////////CustomerIdentifier///////////////////////////////////////////////////
					FieldMapHelper.TryMapField(Attributes.Customer.SourceMarket, field, $"{Replace}", $"{Identifier}/sourceMarket", UpdateElement(element)) ||
					////////////////////////////////////////CustomerGeneral///////////////////////////////////////////////////
					FieldMapHelper.TryMapField(Attributes.Customer.StatusCode, field, $"{Replace}", $"{General}/customerStatus", UpdateElement(element, typeof(Model.Schema.CustomerStatus))) ||
					////////////////////////////////////////Company//////////////////////////////////////////////////
					FieldMapHelper.TryMapField(Attributes.Customer.Name, field, $"{Replace}", $"{Company}/companyName", UpdateElement(element)) ||
					////////////////////////////////////////Additional///////////////////////////////////////////////
					FieldMapHelper.TryMapField(Attributes.Customer.Segment, field, $"{Replace}", $"{Company}/segmentadditional", UpdateElement(element)) ||
					FieldMapHelper.TryMapField(Attributes.Customer.DateOfDeath, field, $"{Replace}", $"{Additional}/dateOfDeath", UpdateElement(element)) ||
					////////////////////////////////////////Address1/////////////////////////////////////////////////
					FieldMapHelper.TryMapField(Attributes.Customer.Address1AdditionaInformation, field, $"{Replace}", $"{Address1}/additionalAddressInfo", UpdateElement(element)) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.Address1FlatOrUnitNumber, field, $"{Replace}", $"{Address1}/flatNumberUnit", UpdateElement(element)) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.Address1Street, field, $"{Replace}", $"{Address1}/street", UpdateElement(element)) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.Address1Town, field, $"{Replace}", $"{Address1}/town", UpdateElement(element)) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.Address1County, field, $"{Replace}", $"{Address1}/county", UpdateElement(element)) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.Address1CountryId, field, $"{Replace}", $"{Address1}/country", UpdateElement(element)) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.Address1PostalCode, field, $"{Replace}", $"{Address1}/postalCode", UpdateElement(element)) ||
					FieldMapHelper.TryMapField(Attributes.Customer.Address1HouseNumberOrBuilding, field, $"{Replace}", $"{Address1}/houseNumberBuilding", UpdateElement(element)) ||
					////////////////////////////////////////Address2/////////////////////////////////////////////////
					FieldMapHelper.TryMapField(Attributes.Customer.Address2AdditionaInformation, field, $"{Replace}", $"{Address2}/additionalAddressInfo", UpdateElement(element)) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.Address2FlatOrUnitNumber, field, $"{Replace}", $"{Address2}/flatNumberUnit", UpdateElement(element)) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.Address2Street, field, $"{Replace}", $"{Address2}/street", UpdateElement(element)) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.Address2Town, field, $"{Replace}", $"{Address2}/town", UpdateElement(element)) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.Address2County, field, $"{Replace}", $"{Address2}/county", UpdateElement(element)) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.Address2CountryId, field, $"{Replace}", $"{Address2}/country", UpdateElement(element)) ||
                    FieldMapHelper.TryMapField(Attributes.Customer.Address2PostalCode, field, $"{Replace}", $"{Address2}/postalCode", UpdateElement(element)) ||
					FieldMapHelper.TryMapField(Attributes.Customer.Address2HouseNumberOrBuilding, field, $"{Replace}", $"{Address2}/houseNumberBuilding", UpdateElement(element)) ||
					//////////////////////////////////////////Phone////////////////////////////////////////////////////
					FieldMapHelper.TryMapField(Attributes.Customer.Telephone1, field, $"{Replace}", $"{Phone1}/number", UpdateElement(element)) ||
					FieldMapHelper.TryMapField(Attributes.Customer.Telephone1Type, field, $"{Replace}", $"{Phone1}/type", UpdateElement(element, typeof(Model.Schema.PhoneType))) ||
					FieldMapHelper.TryMapField(Attributes.Customer.Telephone2, field, $"{Replace}", $"{Phone2}/number", UpdateElement(element)) ||
					FieldMapHelper.TryMapField(Attributes.Customer.Telephone2Type, field, $"{Replace}", $"{Phone2}/type", UpdateElement(element, typeof(Model.Schema.PhoneType))) ||					
					FieldMapHelper.TryMapField(Attributes.Customer.Telephone3, field, $"{Replace}", $"{Phone3}/number", UpdateElement(element)) ||
					FieldMapHelper.TryMapField(Attributes.Customer.Telephone3Type, field, $"{Replace}", $"{Phone3}/type", UpdateElement(element, typeof(Model.Schema.PhoneType))) ||
					////////////////////////////////////////Email////////////////////////////////////////////////////
					FieldMapHelper.TryMapField(Attributes.Customer.EmailAddress1, field, $"{Replace}", $"{Email1}/number", UpdateElement(element)) ||
					FieldMapHelper.TryMapField(Attributes.Customer.EmailAddress1Type, field, $"{Replace}", $"{Email1}/type", UpdateElement(element, typeof(Model.Schema.EmailType))) ||					
					FieldMapHelper.TryMapField(Attributes.Customer.EmailAddress2, field, $"{Replace}", $"{Email2}/number", UpdateElement(element)) ||
					FieldMapHelper.TryMapField(Attributes.Customer.EmailAddress2Type, field, $"{Replace}", $"{Email2}/type", UpdateElement(element, typeof(Model.Schema.EmailType))) ||				
					FieldMapHelper.TryMapField(Attributes.Customer.EmailAddress3, field, $"{Replace}", $"{Email3}/number", UpdateElement(element)) ||
					FieldMapHelper.TryMapField(Attributes.Customer.EmailAddress3Type, field, $"{Replace}", $"{Email3}/type", UpdateElement(element, typeof(Model.Schema.EmailType)));
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

		private static Action<string, string, string> UpdateElement(PatchElement element, System.Type type)
        {
            return (op, path, value) =>
            {
                element.Operator = op;
                element.Path = path;
                element.Value = EnumHelper.MapOptionSet(value, type).ToString();
            };
        }
    }
}
