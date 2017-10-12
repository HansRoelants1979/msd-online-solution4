using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Common.IntegrationLayer.Model;
using Tc.Crm.Common.IntegrationLayer.Model.Schema;
using Tc.Crm.Common.IntegrationLayer.Service.Synchronisation;
using Attributes = Tc.Crm.Common.Constants.Attributes;

namespace Tc.Crm.UnitTests.Common.IntegrationLayer.Service.Syncronisation
{
	[TestClass]
	public class TestCustomerMapper
	{
		[TestMethod]
		public void TestCreateMap()
		{
			var model = new EntityModel
			{
				Fields = new List<Field>
				{
					new Field { Name = Attributes.Customer.SourceMarket, Type = FieldType.String, Value = "sourcemarket" },
					new Field { Name = Attributes.Customer.StatusCode, Type = FieldType.OptionSet, Value = new OptionSet { Value = 1} },
					new Field { Name = Attributes.Customer.Salutation, Type = FieldType.String, Value = "salutation" },
					new Field { Name = Attributes.Customer.AcademicTitle, Type = FieldType.String, Value = "academictitle" },
					new Field { Name = Attributes.Customer.FirstName, Type = FieldType.String, Value = "firstname" },
					new Field { Name = Attributes.Customer.LastName, Type = FieldType.String, Value = "lastname" },
					new Field { Name = Attributes.Customer.MiddleName, Type = FieldType.String, Value = "middlename" },
					new Field { Name = Attributes.Customer.Language, Type = FieldType.String, Value = "language" },
					new Field { Name = Attributes.Customer.Gender, Type = FieldType.OptionSet, Value = new OptionSet { Value = 950000000 } },
					new Field { Name = Attributes.Customer.Birthdate, Type = FieldType.DateTime, Value = DateTime.Now.Date },
					new Field { Name = Attributes.Customer.Name, Type = FieldType.String, Value = "customername" },
					new Field { Name = Attributes.Customer.Segment, Type = FieldType.String, Value = "segment" },
					new Field { Name = Attributes.Customer.DateOfDeath, Type = FieldType.DateTime, Value = DateTime.Now.Date },
					new Field { Name = Attributes.Customer.Address1AdditionaInformation, Type = FieldType.String, Value = "addressadditionalanformation1" },
					new Field { Name = Attributes.Customer.Address1FlatOrUnitNumber, Type = FieldType.String, Value = "addressflatnumberunit1" },
					new Field { Name = Attributes.Customer.Address1HouseNumberOrBuilding, Type = FieldType.String, Value = "addresshousenumberbuilding1" },
					new Field { Name = Attributes.Customer.Address1Street, Type = FieldType.String, Value = "addressstreet1" },
					new Field { Name = Attributes.Customer.Address1Town, Type = FieldType.String, Value = "addresstown1" },
					new Field { Name = Attributes.Customer.Address1CountryId, Type = FieldType.String, Value = "addresscountry1" },
					new Field { Name = Attributes.Customer.Address1County, Type = FieldType.String, Value = "addresscounty1" },
					new Field { Name = Attributes.Customer.Address1PostalCode, Type = FieldType.String, Value = "addresspostalcode1" },
					new Field { Name = Attributes.Customer.Address2AdditionaInformation, Type = FieldType.String, Value = "addressadditionalanformation2" },
					new Field { Name = Attributes.Customer.Address2FlatOrUnitNumber, Type = FieldType.String, Value = "addressflatnumberunit2" },
					new Field { Name = Attributes.Customer.Address2HouseNumberOrBuilding, Type = FieldType.String, Value = "addresshousenumberbuilding2" },
					new Field { Name = Attributes.Customer.Address2Street, Type = FieldType.String, Value = "addressstreet2" },
					new Field { Name = Attributes.Customer.Address2Town, Type = FieldType.String, Value = "addresstown2" },
					new Field { Name = Attributes.Customer.Address2CountryId, Type = FieldType.String, Value = "addresscountry2" },
					new Field { Name = Attributes.Customer.Address2County, Type = FieldType.String, Value = "addresscounty2" },
					new Field { Name = Attributes.Customer.Address2PostalCode, Type = FieldType.String, Value = "addresspostalcode2" },
					new Field { Name = Attributes.Customer.Telephone1Type, Type = FieldType.OptionSet, Value = new OptionSet { Value = 950000000 } },
					new Field { Name = Attributes.Customer.Telephone1, Type = FieldType.String, Value = "telephonenumber1" },
					new Field { Name = Attributes.Customer.Telephone2Type, Type = FieldType.OptionSet, Value = new OptionSet { Value = 950000003 } },
					new Field { Name = Attributes.Customer.Telephone2, Type = FieldType.String, Value = "telephonenumber2" },
					new Field { Name = Attributes.Customer.Telephone3Type, Type = FieldType.OptionSet, Value = new OptionSet { Value = 950000002 } },
					new Field { Name = Attributes.Customer.Telephone3, Type = FieldType.String, Value = "telephonenumber3" },
					new Field { Name = Attributes.Customer.EmailAddress1Type, Type = FieldType.OptionSet, Value = new OptionSet { Value = 950000000 } },
					new Field { Name = Attributes.Customer.EmailAddress1, Type = FieldType.String, Value = "emailaddress1" },
					new Field { Name = Attributes.Customer.EmailAddress2Type, Type = FieldType.OptionSet, Value = new OptionSet { Value = 950000001 } },
					new Field { Name = Attributes.Customer.EmailAddress2, Type = FieldType.String, Value = "emailaddress2" },
					new Field { Name = Attributes.Customer.EmailAddress3Type, Type = FieldType.OptionSet, Value = new OptionSet { Value = 950000002 } },
					new Field { Name = Attributes.Customer.EmailAddress3, Type = FieldType.String, Value = "emailaddress3" }
				}
			};
			var mapper = new CreateCustomerRequestMapper();
			var customer = mapper.Map(model) as Customer;
			Assert.IsNotNull(customer);
			Assert.IsNotNull(customer.CustomerIdentity);
			Assert.AreEqual("sourcemarket", customer.CustomerIdentifier.SourceMarket);
			Assert.AreEqual(CustomerStatus.Active, customer.CustomerGeneral.CustomerStatus);
			Assert.AreEqual("salutation", customer.CustomerIdentity.Salutation);
			Assert.AreEqual("academictitle", customer.CustomerIdentity.AcademicTitle);
			Assert.AreEqual("firstname", customer.CustomerIdentity.FirstName);
			Assert.AreEqual("lastname", customer.CustomerIdentity.LastName);
			Assert.AreEqual("middlename", customer.CustomerIdentity.MiddleName);
			Assert.AreEqual("language", customer.CustomerIdentity.Language);
			Assert.AreEqual(Gender.Male, customer.CustomerIdentity.Gender);
			Assert.AreEqual(DateTime.Now.Date.ToShortDateString(), customer.CustomerIdentity.Birthdate);
			Assert.AreEqual("customername", customer.Company.CompanyName);
			Assert.AreEqual("segment", customer.Additional.Segment);
			Assert.AreEqual(DateTime.Now.Date.ToShortDateString(), customer.Additional.DateOfDeath);
			Assert.AreEqual("addressadditionalanformation1", customer.Address[0].AdditionalAddressInfo);
			Assert.AreEqual("addressflatnumberunit1", customer.Address[0].FlatNumberUnit);
			Assert.AreEqual("addresshousenumberbuilding1", customer.Address[0].HouseNumberBuilding);
			Assert.AreEqual("addressstreet1", customer.Address[0].Street);
			Assert.AreEqual("addresstown1", customer.Address[0].Town);
			Assert.AreEqual("addresscountry1", customer.Address[0].Country);
			Assert.AreEqual("addresscounty1", customer.Address[0].County);
			Assert.AreEqual("addresspostalcode1", customer.Address[0].PostalCode);
			Assert.AreEqual("addressadditionalanformation2", customer.Address[1].AdditionalAddressInfo);
			Assert.AreEqual("addressflatnumberunit2", customer.Address[1].FlatNumberUnit);
			Assert.AreEqual("addresshousenumberbuilding2", customer.Address[1].HouseNumberBuilding);
			Assert.AreEqual("addressstreet2", customer.Address[1].Street);
			Assert.AreEqual("addresstown2", customer.Address[1].Town);
			Assert.AreEqual("addresscountry2", customer.Address[1].Country);
			Assert.AreEqual("addresscounty2", customer.Address[1].County);
			Assert.AreEqual("addresspostalcode2", customer.Address[1].PostalCode);
			Assert.AreEqual(PhoneType.Mobile, customer.Phone[0].PhoneType);
			Assert.AreEqual("telephonenumber1", customer.Phone[0].Number);
			Assert.AreEqual(PhoneType.Business, customer.Phone[1].PhoneType);
			Assert.AreEqual("telephonenumber2", customer.Phone[1].Number);
			Assert.AreEqual(PhoneType.NotSpecified, customer.Phone[2].PhoneType);
			Assert.AreEqual("telephonenumber3", customer.Phone[2].Number);
			Assert.AreEqual(EmailType.Primary, customer.Email[0].EmailType);
			Assert.AreEqual("emailaddress1", customer.Email[0].Address);
			Assert.AreEqual(EmailType.Promo, customer.Email[1].EmailType);
			Assert.AreEqual("emailaddress2", customer.Email[1].Address);
			Assert.AreEqual(EmailType.NotSpecified, customer.Email[2].EmailType);
			Assert.AreEqual("emailaddress3", customer.Email[2].Address);
		}

		[TestMethod]
		public void TestUpdateMap()
		{
			// Given
			var date = DateTime.Now.Date;
			var mapper = new UpdateCustomerRequestMapper();

			var model = new EntityModel
			{
				Fields = new List<Field>
				{
					new Field { Name = Attributes.Customer.SourceSystemId, Type = FieldType.String, Value = "sourceSystem" },
					new Field { Name = Attributes.Customer.Salutation, Type = FieldType.String, Value = "salutation" },
					new Field { Name = Attributes.Customer.FirstName, Type = FieldType.String, Value = "firstname" },
					new Field { Name = Attributes.Customer.LastName, Type = FieldType.String, Value = "lastname" },
					new Field { Name = Attributes.Customer.Language, Type = FieldType.String, Value = "language" },
					new Field { Name = Attributes.Customer.Birthdate, Type = FieldType.DateTime, Value = date },
							    
					new Field { Name = Attributes.Customer.Address1FlatOrUnitNumber, Type = FieldType.String, Value = "addressFlatNumberUnit1" },
					new Field { Name = Attributes.Customer.Address2FlatOrUnitNumber, Type = FieldType.String, Value = "addressFlatNumberUnit2" },
					new Field { Name = Attributes.Customer.Address1HouseNumberOrBuilding, Type = FieldType.String, Value = "addressHouseNumberBuilding1" },
					new Field { Name = Attributes.Customer.Address2HouseNumberOrBuilding, Type = FieldType.String, Value = "addressHouseNumberBuilding2" },
					new Field { Name = Attributes.Customer.Address1Street, Type = FieldType.String, Value = "addressStreet1" },
					new Field { Name = Attributes.Customer.Address2Street, Type = FieldType.String, Value = "addressStreet2" },
					new Field { Name = Attributes.Customer.Address1AdditionaInformation, Type = FieldType.String, Value = "addressAdditionalInformation1" },
					new Field { Name = Attributes.Customer.Address2AdditionaInformation, Type = FieldType.String, Value = "addressAdditionalInformation2" },
					new Field { Name = Attributes.Customer.Address1Town, Type = FieldType.String, Value = "addressTown1" },
					new Field { Name = Attributes.Customer.Address2Town, Type = FieldType.String, Value = "addressTown2" },
					new Field { Name = Attributes.Customer.Address1PostalCode, Type = FieldType.String, Value = "addressPostalCode1" },
					new Field { Name = Attributes.Customer.Address2PostalCode, Type = FieldType.String, Value = "addressPostalCode2" },
					new Field { Name = Attributes.Customer.Address1County, Type = FieldType.String, Value = "addressCounty1" },
					new Field { Name = Attributes.Customer.Address2County, Type = FieldType.String, Value = "addressCounty2" },
					new Field { Name = Attributes.Customer.Address1CountryId, Type = FieldType.String, Value = "addressCountry1" },
					new Field { Name = Attributes.Customer.Address2CountryId, Type = FieldType.String, Value = "addressCountry2" },
					new Field { Name = Attributes.Customer.EmailAddress1Type, Type = FieldType.OptionSet, Value = new OptionSet { Value = (int)Tc.Crm.Common.EmailType.Primary } },
					new Field { Name = Attributes.Customer.EmailAddress2Type, Type = FieldType.OptionSet, Value =new OptionSet { Value = (int) Tc.Crm.Common.EmailType.Promotion } },
					new Field { Name = Attributes.Customer.EmailAddress3Type, Type = FieldType.OptionSet, Value = new OptionSet { Value = (int)Tc.Crm.Common.EmailType.Unknown } },
					new Field { Name = Attributes.Customer.Telephone1Type, Type = FieldType.OptionSet, Value = new OptionSet { Value = (int)Tc.Crm.Common.PhoneType.Mobile } },
					new Field { Name = Attributes.Customer.Telephone2Type, Type = FieldType.OptionSet, Value = new OptionSet { Value = (int)Tc.Crm.Common.PhoneType.Business } },
					new Field { Name = Attributes.Customer.Telephone3Type, Type = FieldType.OptionSet, Value = new OptionSet { Value = (int)Tc.Crm.Common.PhoneType.Unknown } },
					new Field { Name = Attributes.Customer.Telephone1, Type = FieldType.String, Value = "telephoneNumber1" },
					new Field { Name = Attributes.Customer.Telephone2, Type = FieldType.String, Value = "telephoneNumber2" },
					new Field { Name = Attributes.Customer.Telephone3, Type = FieldType.String, Value = "telephoneNumber3" },
							    
					new Field { Name = Attributes.Customer.AcademicTitle, Type = FieldType.String, Value = "academicTitle" },
					new Field { Name = Attributes.Customer.MiddleName, Type = FieldType.String, Value = "middleName" },
					new Field { Name = Attributes.Customer.Segment, Type = FieldType.String, Value = "segment" },
					new Field { Name = Attributes.Customer.DateOfDeath, Type = FieldType.DateTime, Value = date.AddYears(100) },
					new Field { Name = Attributes.Customer.Name, Type = FieldType.String, Value = "companyName" },
					new Field { Name = Attributes.Customer.StatusCode, Type = FieldType.OptionSet, Value = new OptionSet { Value = (int)Tc.Crm.Common.CustomerStatusCode.Active } },
					new Field { Name = Attributes.Customer.Gender, Type = FieldType.OptionSet, Value = new OptionSet { Value = (int)Tc.Crm.Common.Gender.Male } },
					new Field { Name = Attributes.Customer.SourceMarket, Type = FieldType.String, Value = "sourceMarket" }
				}
			};

			// When
			var patchElements = mapper.Map(model) as List<PatchElement>;

			// Then
			Assert.IsNotNull(patchElements);
			Assert.AreEqual(patchElements.Count, 38);
			CollectionAssert.AllItemsAreUnique(patchElements.Select(element => element.Path).ToList());
			Assert.AreEqual("salutation", patchElements[0].Value);
			Assert.AreEqual("firstname", patchElements[1].Value);
			Assert.AreEqual("lastname", patchElements[2].Value);
			Assert.AreEqual("language", patchElements[3].Value);
			Assert.AreEqual(date.ToShortDateString(), patchElements[4].Value);

			Assert.AreEqual("addressFlatNumberUnit1", patchElements[5].Value);
			Assert.AreEqual("addressFlatNumberUnit2", patchElements[6].Value);
			Assert.AreEqual("addressHouseNumberBuilding1", patchElements[7].Value);
			Assert.AreEqual("addressHouseNumberBuilding2", patchElements[8].Value);
			Assert.AreEqual("addressStreet1", patchElements[9].Value);
			Assert.AreEqual("addressStreet2", patchElements[10].Value);
			Assert.AreEqual("addressAdditionalInformation1", patchElements[11].Value);
			Assert.AreEqual("addressAdditionalInformation2", patchElements[12].Value);
			Assert.AreEqual("addressTown1", patchElements[13].Value);
			Assert.AreEqual("addressTown2", patchElements[14].Value);
			Assert.AreEqual("addressPostalCode1", patchElements[15].Value);
			Assert.AreEqual("addressPostalCode2", patchElements[16].Value);
			Assert.AreEqual("addressCounty1", patchElements[17].Value);
			Assert.AreEqual("addressCounty2", patchElements[18].Value);
			Assert.AreEqual("addressCountry1", patchElements[19].Value);
			Assert.AreEqual("addressCountry2", patchElements[20].Value);

			Assert.AreEqual(EmailType.Primary.ToString(), patchElements[21].Value);
			Assert.AreEqual(EmailType.Promo.ToString(), patchElements[22].Value);
			Assert.AreEqual(EmailType.NotSpecified.ToString(), patchElements[23].Value);
			Assert.AreEqual(PhoneType.Mobile.ToString(), patchElements[24].Value);
			Assert.AreEqual(PhoneType.Business.ToString(), patchElements[25].Value);
			Assert.AreEqual(PhoneType.NotSpecified.ToString(), patchElements[26].Value);
			Assert.AreEqual("telephoneNumber1", patchElements[27].Value);
			Assert.AreEqual("telephoneNumber2", patchElements[28].Value);
			Assert.AreEqual("telephoneNumber3", patchElements[29].Value);

			Assert.AreEqual("academicTitle", patchElements[30].Value);
			Assert.AreEqual("middleName", patchElements[31].Value);
			Assert.AreEqual("segment", patchElements[32].Value);
			Assert.AreEqual(date.AddYears(100).ToShortDateString(), patchElements[33].Value);
			Assert.AreEqual("companyName", patchElements[34].Value);
			Assert.AreEqual(CustomerStatus.Active.ToString(), patchElements[35].Value);
			Assert.AreEqual(Gender.Male.ToString(), patchElements[36].Value);
			Assert.AreEqual("sourceMarket", patchElements[37].Value);
		}
	}
}