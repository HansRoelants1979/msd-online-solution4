using Tc.Crm.Common.IntegrationLayer.Helper;
using Tc.Crm.Common.IntegrationLayer.Model;
using Tc.Crm.Common.IntegrationLayer.Model.Schema;
using Attributes = Tc.Crm.Common.Constants.Attributes;

namespace Tc.Crm.Common.IntegrationLayer.Service.Synchronisation
{
	using CustomerType = Model.Schema.CustomerType;
	using EmailType = Model.Schema.EmailType;
	using Gender = Model.Schema.Gender;
	using PhoneType = Model.Schema.PhoneType;

	/// <summary>
	/// Mapper class to create payload of syncronisation request for customer creation
	/// </summary>
	public class CreateCustomerRequestMapper : IEntityCacheMapper
    {
        /// <summary>
        /// Creates Tc.Crm.Common.IntegrationLayer.Model.Schema.Customer class and fills properties required to create request
        /// </summary>
        /// <param name="model">Customer record entity model</param>
        /// <returns>mapped Tc.Crm.Common.IntegrationLayer.Model.Schema.Customer</returns>
        public object Map(EntityModel model)
        {
            var customer = new Customer
            {
                CustomerIdentifier = new CustomerIdentifier(),
                CustomerGeneral = new CustomerGeneral(),
                CustomerIdentity = new CustomerIdentity(),
                Company = new Company(),
		            Additional = new Additional(),
		            Address = new[] { new Address(), new Address() },
		            Phone = new[] { new Phone(), new Phone(), new Phone() },
		            Email = new[] { new Email(), new Email(), new Email() },
					Social = new Social[1]
            };
            foreach (var field in model.Fields)
            {
                ////////////////////////////////////////CustomerIdentifier////////////////////////////////////////////////
                FieldMapHelper.TryMapField(Attributes.Customer.SourceMarket, field, value => customer.CustomerIdentifier.SourceMarket = value);
				////////////////////////////////////////CustomerGeneral///////////////////////////////////////////////////
				FieldMapHelper.TryMapField(Attributes.Customer.StatusCode, field, value => customer.CustomerGeneral.CustomerStatus = (CustomerStatus)EnumHelper.MapOptionSet(value, typeof(CustomerStatus)));
				customer.CustomerGeneral.CustomerType = CustomerType.Person;
				////////////////////////////////////////CustomerIdentity//////////////////////////////////////////////////
				FieldMapHelper.TryMapField(Attributes.Customer.Salutation, field, value => customer.CustomerIdentity.Salutation = value);
                FieldMapHelper.TryMapField(Attributes.Customer.AcademicTitle, field, value => customer.CustomerIdentity.AcademicTitle = value);
                FieldMapHelper.TryMapField(Attributes.Customer.FirstName, field, value => customer.CustomerIdentity.FirstName = value);
                FieldMapHelper.TryMapField(Attributes.Customer.LastName, field, value => customer.CustomerIdentity.LastName = value);
                FieldMapHelper.TryMapField(Attributes.Customer.MiddleName, field, value => customer.CustomerIdentity.MiddleName = value);
                FieldMapHelper.TryMapField(Attributes.Customer.Language, field, value => customer.CustomerIdentity.Language = value);
				FieldMapHelper.TryMapField(Attributes.Customer.Gender, field, value => customer.CustomerIdentity.Gender = (Gender)EnumHelper.MapOptionSet(value, typeof(Gender)) /*GetGender(value)*/);
				FieldMapHelper.TryMapField(Attributes.Customer.Birthdate, field, value => customer.CustomerIdentity.Birthdate = value);
                ////////////////////////////////////////Company//////////////////////////////////////////////////
                FieldMapHelper.TryMapField(Attributes.Customer.Name, field, value => customer.Company.CompanyName = value);
                ////////////////////////////////////////Additional///////////////////////////////////////////////
                FieldMapHelper.TryMapField(Attributes.Customer.Segment, field, value => customer.Additional.Segment = value);
                FieldMapHelper.TryMapField(Attributes.Customer.DateOfDeath, field, value => customer.Additional.DateOfDeath = value);
                ////////////////////////////////////////Address1/////////////////////////////////////////////////
                FieldMapHelper.TryMapField(Attributes.Customer.Address1AdditionaInformation, field, value => customer.Address[0].AdditionalAddressInfo = value);
                FieldMapHelper.TryMapField(Attributes.Customer.Address1FlatOrUnitNumber, field, value => customer.Address[0].FlatNumberUnit = value);
                FieldMapHelper.TryMapField(Attributes.Customer.Address1HouseNumberOrBuilding, field, value => customer.Address[0].HouseNumberBuilding = value);
                FieldMapHelper.TryMapField(Attributes.Customer.Address1Street, field, value => customer.Address[0].Street = value);
                FieldMapHelper.TryMapField(Attributes.Customer.Address1Town, field, value => customer.Address[0].Town = value);
                FieldMapHelper.TryMapField(Attributes.Customer.Address1CountryId, field, value => customer.Address[0].Country = value);
                FieldMapHelper.TryMapField(Attributes.Customer.Address1County, field, value => customer.Address[0].County = value);
                FieldMapHelper.TryMapField(Attributes.Customer.Address1PostalCode, field, value => customer.Address[0].PostalCode = value);
				customer.Address[0].AddressType = AddressType.Main;
				////////////////////////////////////////Address2/////////////////////////////////////////////////
				FieldMapHelper.TryMapField(Attributes.Customer.Address2AdditionaInformation, field, value => customer.Address[1].AdditionalAddressInfo = value);
                FieldMapHelper.TryMapField(Attributes.Customer.Address2FlatOrUnitNumber, field, value => customer.Address[1].FlatNumberUnit = value);
                FieldMapHelper.TryMapField(Attributes.Customer.Address2HouseNumberOrBuilding, field, value => customer.Address[1].HouseNumberBuilding = value);
                FieldMapHelper.TryMapField(Attributes.Customer.Address2Street, field, value => customer.Address[1].Street = value);
                FieldMapHelper.TryMapField(Attributes.Customer.Address2Town, field, value => customer.Address[1].Town = value);
                FieldMapHelper.TryMapField(Attributes.Customer.Address2CountryId, field, value => customer.Address[1].Country = value);
                FieldMapHelper.TryMapField(Attributes.Customer.Address2County, field, value => customer.Address[1].County = value);
                FieldMapHelper.TryMapField(Attributes.Customer.Address2PostalCode, field, value => customer.Address[1].PostalCode = value);
				customer.Address[1].AddressType = AddressType.NotSpecified;
				//////////////////////////////////////////Phone////////////////////////////////////////////////////
				FieldMapHelper.TryMapField(Attributes.Customer.Telephone1Type, field, value => customer.Phone[0].PhoneType = (PhoneType)EnumHelper.MapOptionSet(value, typeof(PhoneType)));
				FieldMapHelper.TryMapField(Attributes.Customer.Telephone1, field, value => customer.Phone[0].Number = value);
				FieldMapHelper.TryMapField(Attributes.Customer.Telephone2Type, field, value => customer.Phone[1].PhoneType = (PhoneType)EnumHelper.MapOptionSet(value, typeof(PhoneType)));
				FieldMapHelper.TryMapField(Attributes.Customer.Telephone2, field, value => customer.Phone[1].Number = value);
				FieldMapHelper.TryMapField(Attributes.Customer.Telephone3Type, field, value => customer.Phone[2].PhoneType = (PhoneType)EnumHelper.MapOptionSet(value, typeof(PhoneType)));
				FieldMapHelper.TryMapField(Attributes.Customer.Telephone3, field, value => customer.Phone[2].Number = value);
				////////////////////////////////////////Email////////////////////////////////////////////////////
				FieldMapHelper.TryMapField(Attributes.Customer.EmailAddress1Type, field, value => customer.Email[0].EmailType = (EmailType)EnumHelper.MapOptionSet(value, typeof(EmailType)));
				FieldMapHelper.TryMapField(Attributes.Customer.EmailAddress1, field, value => customer.Email[0].Address = value);
				FieldMapHelper.TryMapField(Attributes.Customer.EmailAddress2Type, field, value => customer.Email[1].EmailType = (EmailType)EnumHelper.MapOptionSet(value, typeof(EmailType)));
				FieldMapHelper.TryMapField(Attributes.Customer.EmailAddress2, field, value => customer.Email[1].Address = value);
				FieldMapHelper.TryMapField(Attributes.Customer.EmailAddress3Type, field, value => customer.Email[2].EmailType = (EmailType)EnumHelper.MapOptionSet(value, typeof(EmailType)));
				FieldMapHelper.TryMapField(Attributes.Customer.EmailAddress3, field, value => customer.Email[2].Address = value);
			}
            return customer;
        }

	    
	}
}