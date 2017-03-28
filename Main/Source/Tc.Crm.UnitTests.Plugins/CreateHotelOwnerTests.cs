using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using FakeXrmEasy;

namespace Tc.Crm.UnitTests.Plugins
{
    [TestClass]
    public class CreateHotelOwnerTests
    {
        [TestMethod]
        public void CreateHotelNoName()
        {
            var context = new XrmFakedContext();
            var target = new Microsoft.Xrm.Sdk.Entity("tc_hotel") { Id = Guid.NewGuid() };
            var fakedPlugin = context.ExecutePluginWithTarget<Crm.Plugins.CreateHotelOwner>(target);
            var hotels = (from t in context.CreateQuery("tc_hotel")
                                     select t).ToList();
            Assert.IsTrue(hotels.Count == 0);
        }

        [TestMethod]
        public void CreateHotelNoRoles()
        {
            var role = new Entity("role", Guid.NewGuid());         

            var context = new XrmFakedContext();
            context.Initialize(new List<Entity> { role });
            var target = new Microsoft.Xrm.Sdk.Entity("tc_hotel") { Id = Guid.NewGuid() };
            target["tc_name"] = "Hotel XXX";
            Assert.ThrowsException<InvalidPluginExecutionException>(() => context.ExecutePluginWithTarget<Crm.Plugins.CreateHotelOwner>(target));
        }       
    }
}
