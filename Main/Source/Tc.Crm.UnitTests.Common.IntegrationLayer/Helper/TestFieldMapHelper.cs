using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.Common.IntegrationLayer.Helper;
using Tc.Crm.Common.IntegrationLayer.Model;

namespace Tc.Crm.UnitTests.Common.IntegrationLayer.Helper
{
    [TestClass]
    public class TestFieldMapHelper
    {
        [TestMethod]
        public void TestMapFieldTypeString()
        {
            string property = null;
            var field = new Field
            {
                Name = "fieldName",
                Type = FieldType.String,
                Value = "fieldValue"
            };
            var mapped = FieldMapHelper.TryMapField("fieldName", field, value => property = value);
            Assert.AreEqual(true, mapped);
            Assert.AreEqual("fieldValue", property);
        }

        [TestMethod]
        public void TestMapFieldMatchFieldName()
        {
            string property = null;
            var field = new Field
            {
                Name = "fieldName", Type = FieldType.String, Value = "fieldValue"
            };
            var mapped = FieldMapHelper.TryMapField("fieldName", field, value => property = value);
            Assert.AreEqual(true, mapped);
            Assert.AreEqual("fieldValue", property);
            mapped = FieldMapHelper.TryMapField("fieldName1", field, value => property = value);
            Assert.AreEqual(false, mapped);
            Assert.AreEqual("fieldValue", property);
            property = null;
            mapped = FieldMapHelper.TryMapField("fieldName1", field, value => property = value);            
            Assert.AreEqual(false, mapped);
            Assert.IsNull(property);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestMapFieldTypeAmount()
        {
            string property = null;
            var field = new Field
            {
                Type = FieldType.Amount,
                Name = "fieldName",                
                Value = "fieldValue"
            };
            var mapped = FieldMapHelper.TryMapField("fieldName", field, value => property = value);
            //Assert.AreEqual(true, mapped);
            //Assert.AreEqual("fieldValue", property);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestMapFieldTypeBoolean()
        {
            string property = null;
            var field = new Field
            {
                Type = FieldType.Boolean,
                Name = "fieldName",
                Value = "fieldValue"
            };
            var mapped = FieldMapHelper.TryMapField("fieldName", field, value => property = value);
            //Assert.AreEqual(true, mapped);
            //Assert.AreEqual("fieldValue", property);
        }

        [TestMethod]
        public void TestMapFieldTypeDateTime()
        {
            string property = null;
            var field = new Field
            {
                Type = FieldType.DateTime,
                Name = "fieldName",
                Value = DateTime.Now.Date
            };
            var mapped = FieldMapHelper.TryMapField("fieldName", field, value => property = value);
            Assert.AreEqual(true, mapped);
            Assert.AreEqual(DateTime.Now.Date.ToShortDateString(), property);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestMapFieldTypeDecimal()
        {
            string property = null;
            var field = new Field
            {
                Type = FieldType.Decimal,
                Name = "fieldName",
                Value = "fieldValue"
            };
            var mapped = FieldMapHelper.TryMapField("fieldName", field, value => property = value);
            //Assert.AreEqual(true, mapped);
            //Assert.AreEqual("fieldValue", property);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestMapFieldTypeDouble()
        {
            string property = null;
            var field = new Field
            {
                Type = FieldType.Double,
                Name = "fieldName",
                Value = "fieldValue"
            };
            var mapped = FieldMapHelper.TryMapField("fieldName", field, value => property = value);
            //Assert.AreEqual(true, mapped);
            //Assert.AreEqual("fieldValue", property);
        }

        [TestMethod]
        public void TestMapFieldTypeGuid()
        {
            var guid = Guid.NewGuid();
            string property = null;
            var field = new Field
            {
                Name = "fieldName",
                Type = FieldType.Guid,
                Value = guid
            };
            var mapped = FieldMapHelper.TryMapField("fieldName", field, value => property = value);
            Assert.AreEqual(true, mapped);
            Assert.AreEqual(guid.ToString(), property);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestMapFieldTypeInt32()
        {
            string property = null;
            var field = new Field
            {
                Type = FieldType.Int32,
                Name = "fieldName",
                Value = "fieldValue"
            };
            var mapped = FieldMapHelper.TryMapField("fieldName", field, value => property = value);
            //Assert.AreEqual(true, mapped);
            //Assert.AreEqual("fieldValue", property);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestMapFieldTypeLookup()
        {
            string property = null;
            var field = new Field
            {
                Type = FieldType.Lookup,
                Name = "fieldName",
                Value = "fieldValue"
            };
            var mapped = FieldMapHelper.TryMapField("fieldName", field, value => property = value);
            //Assert.AreEqual(true, mapped);
            //Assert.AreEqual("fieldValue", property);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestMapFieldTypeNull()
        {
            string property = null;
            var field = new Field
            {
                Type = FieldType.Null,
                Name = "fieldName",
                Value = "fieldValue"
            };
            var mapped = FieldMapHelper.TryMapField("fieldName", field, value => property = value);
            //Assert.AreEqual(true, mapped);
            //Assert.AreEqual("fieldValue", property);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestMapFieldTypeOptionSet()
        {
            string property = null;
            var field = new Field
            {
                Type = FieldType.OptionSet,
                Name = "fieldName",
                Value = "fieldValue"
            };
            var mapped = FieldMapHelper.TryMapField("fieldName", field, value => property = value);
            //Assert.AreEqual(true, mapped);
            //Assert.AreEqual("fieldValue", property);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestMapFieldTypeOptionRecordCollection()
        {
            string property = null;
            var field = new Field
            {
                Type = FieldType.RecordCollection,
                Name = "fieldName",
                Value = "fieldValue"
            };
            var mapped = FieldMapHelper.TryMapField("fieldName", field, value => property = value);
            //Assert.AreEqual(true, mapped);
            //Assert.AreEqual("fieldValue", property);
        }
    }
}
