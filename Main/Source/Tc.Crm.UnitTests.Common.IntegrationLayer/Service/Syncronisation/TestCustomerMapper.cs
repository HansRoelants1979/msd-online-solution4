using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.Common.IntegrationLayer.Service.Synchronisation;
using Tc.Crm.Common.IntegrationLayer.Model;
using Tc.Crm.Common.IntegrationLayer.Model.Schema;
using Attributes = Tc.Crm.Common.Constants.Attributes;

namespace Tc.Crm.UnitTests.Common.IntegrationLayer.Service.Syncronisation
{
    [TestClass]
    public class TestCustomerMapper
    {
        [TestMethod]
        public void TestMap()
        {
            var guid = Guid.NewGuid();
            var model = new EntityModel
            {
                Fields = new List<Field> {
                    new Field { Name = "tc_sourcesystemid", Type = FieldType.Guid, Value = guid },
                    new Field { Name = Attributes.Customer.Salutation, Type = FieldType.String, Value = "salutation" },
                    new Field { Name = Attributes.Customer.FirstName, Type = FieldType.String, Value = "firstname" },
                    new Field { Name = Attributes.Customer.LastName, Type = FieldType.String, Value = "lastname" },
                    new Field { Name = Attributes.Customer.Language, Type = FieldType.String, Value = "language" },
                    new Field { Name = Attributes.Customer.Birthdate, Type = FieldType.DateTime, Value = DateTime.Now.Date }
                }
            };
            var mapper = new CreateCustomerRequestMapper();
            var customer  = mapper.Map(model) as Customer;
            Assert.IsNotNull(customer);
            Assert.IsNotNull(customer.CustomerIdentifier);
            Assert.AreEqual(guid.ToString(), customer.CustomerIdentifier.CustomerId);            
            Assert.IsNotNull(customer.CustomerIdentity);
            Assert.AreEqual("salutation", customer.CustomerIdentity.Salutation);
            Assert.AreEqual("firstname", customer.CustomerIdentity.FirstName);
            Assert.AreEqual("lastname", customer.CustomerIdentity.LastName);
            Assert.AreEqual("language", customer.CustomerIdentity.Language);
            Assert.AreEqual(DateTime.Now.Date.ToShortDateString(), customer.CustomerIdentity.Birthdate);
        }
    }
}
