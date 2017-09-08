using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.Common.IntegrationLayer.Model;
using Attributes = Tc.Crm.Common.Constants.Attributes;
using Tc.Crm.Common.IntegrationLayer.Service.Synchronisation;

namespace Tc.Crm.UnitTests.Common.IntegrationLayer.Service.Syncronisation
{
    [TestClass]
    public class TestCustomerPayloadCreator
    {
        [TestMethod]
        public void TestGetPayload()
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

            var creator = new CreateCustomerRequestPayloadCreator();
            var payload = creator.GetPayload(model);
            Assert.IsNotNull(payload);
        }
    }
}
