using System;
using System.Collections.Generic;
using System.Linq;
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
        public void TestCreateMap()
        {
            var model = new EntityModel
            {
                Fields = new List<Field> {
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
            Assert.IsNotNull(customer.CustomerIdentity);
            Assert.AreEqual("salutation", customer.CustomerIdentity.Salutation);
            Assert.AreEqual("firstname", customer.CustomerIdentity.FirstName);
            Assert.AreEqual("lastname", customer.CustomerIdentity.LastName);
            Assert.AreEqual("language", customer.CustomerIdentity.Language);
            Assert.AreEqual(DateTime.Now.Date.ToShortDateString(), customer.CustomerIdentity.Birthdate);
        }

        [TestMethod]
        public void TestUpdateMap()
        {
            var model = new EntityModel
            {
                Fields = new List<Field>
                {
                    new Field {Name = Attributes.Customer.SourceSystemId, Type = FieldType.String, Value = "sourceSystem"},
                    new Field {Name = Attributes.Customer.Salutation, Type = FieldType.String, Value = "salutation"},
                    new Field {Name = Attributes.Customer.FirstName, Type = FieldType.String, Value = "firstname"},
                    new Field {Name = Attributes.Customer.LastName, Type = FieldType.String, Value = "lastname"},
                    new Field {Name = Attributes.Customer.Language, Type = FieldType.String, Value = "language"},
                    new Field
                    {
                        Name = Attributes.Customer.Birthdate,
                        Type = FieldType.DateTime,
                        Value = DateTime.Now.Date
                    }
                }
            };
            var mapper = new UpdateCustomerRequestMapper();
            var patchElements = mapper.Map(model) as List<PatchElement>;
            Assert.IsNotNull(patchElements);
            Assert.AreEqual(patchElements.Count, 5);
            CollectionAssert.AllItemsAreUnique(patchElements.Select(element => element.Path).ToList());
            CollectionAssert.AllItemsAreUnique(patchElements.Select(element => element.Value).ToList());
            Assert.AreEqual("salutation", patchElements[0].Value);
            Assert.AreEqual("firstname", patchElements[1].Value);
            Assert.AreEqual("lastname", patchElements[2].Value);
            Assert.AreEqual("language", patchElements[3].Value);
            Assert.AreEqual(DateTime.Now.Date.ToShortDateString(), patchElements[4].Value);
        }
    }
}