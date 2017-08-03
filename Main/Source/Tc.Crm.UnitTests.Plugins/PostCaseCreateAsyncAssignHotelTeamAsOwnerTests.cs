using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeXrmEasy;


namespace Tc.Crm.UnitTests.Plugins
{
    [TestClass]
    public class PostCaseCreateAsyncAssignHotelTeamAsOwnerTests
    {
       
        private XrmFakedPluginExecutionContext GetPluginContext(Guid initiatingUserId, Guid incidentId, Guid? customerId = null, Guid? bookingId = null)
        {
            var entCase = new Entity("incident", incidentId);
            if(customerId.HasValue)
            entCase.Attributes["customerid"] = new EntityReference("contact", customerId.Value);
            entCase.Attributes["ownerid"] = new EntityReference("systemuser", initiatingUserId);
            if (bookingId.HasValue)
                entCase.Attributes["tc_bookingid"] = new EntityReference("tc_booking", bookingId.Value);
            var cntxt = new XrmFakedPluginExecutionContext();
            cntxt.InputParameters = new ParameterCollection();
            cntxt.InputParameters.Add("Target", entCase);
            cntxt.MessageName = "Create";
            cntxt.Stage = 40;
            cntxt.InitiatingUserId = initiatingUserId;
            return cntxt;
        }

        private XrmFakedPluginExecutionContext GetInvalidPluginContext(Guid initiatingUserId, Guid incidentId, Guid? customerId = null, Guid? bookingId = null)
        {
            var entCase = new Entity("incident", incidentId);
            if (customerId.HasValue)
                entCase.Attributes["customerid"] = new EntityReference("contact", customerId.Value);
            entCase.Attributes["ownerid"] = new EntityReference("systemuser", initiatingUserId);
            var cntxt = new XrmFakedPluginExecutionContext();
            cntxt.InputParameters = new ParameterCollection();
            cntxt.InputParameters.Add("Target", entCase);
            cntxt.MessageName = "Create";
            cntxt.Stage = 20;
            cntxt.InitiatingUserId = initiatingUserId;
            return cntxt;
        }

        [TestMethod]
        public void CheckInvalidContext()
        {
            AssignHotelTeamAsOwnerContext contextObj = new AssignHotelTeamAsOwnerContext();
            var incidentId = Guid.NewGuid();
            var userId = Guid.NewGuid();            
            var cntxt = GetInvalidPluginContext(Guid.Empty, Guid.Empty);
            XrmFakedContext context = contextObj.InitialiseContext(incidentId, userId);
            context.ExecutePluginWith<Tc.Crm.Plugins.Case.PostCaseCreateAsyncAssignHotelTeamAsOwner>(cntxt);
            var incidentUpdated = (from c in context.CreateQuery("incident")
                                   where c.Id == incidentId
                                   select c).ToList();
            Assert.IsTrue(((EntityReference)incidentUpdated[0].Attributes["ownerid"]).Id == userId);
        }

        [TestMethod]
        public void CheckWithoutRepRole()
        {
            AssignHotelTeamAsOwnerContext contextObj = new AssignHotelTeamAsOwnerContext();
            var incidentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            XrmFakedContext context = contextObj.InitialiseContext(incidentId, userId);
            var cntxt = GetPluginContext(userId, incidentId);
            context.ExecutePluginWith<Tc.Crm.Plugins.Case.PostCaseCreateAsyncAssignHotelTeamAsOwner>(cntxt);
            var incidentUpdated = (from c in context.CreateQuery("incident")
                                   where c.Id == incidentId
                                   select c).ToList();
            Assert.IsTrue(((EntityReference)incidentUpdated[0].Attributes["ownerid"]).Id == userId);          
        }

        [TestMethod]
        public void CheckWithRepRole()
        {
            AssignHotelTeamAsOwnerContext contextObj = new AssignHotelTeamAsOwnerContext();
            var incidentId = Guid.NewGuid();
            var userId = Guid.NewGuid();                      
            var cntxt = GetPluginContext(userId, incidentId);
            XrmFakedContext context = contextObj.InitialiseContext(incidentId, userId, Guid.NewGuid());
            context.ExecutePluginWith<Tc.Crm.Plugins.Case.PostCaseCreateAsyncAssignHotelTeamAsOwner>(cntxt);
            var incidentUpdated = (from c in context.CreateQuery("incident")
                                   where c.Id == incidentId
                                   select c).ToList();
            Assert.IsTrue(((EntityReference)incidentUpdated[0].Attributes["ownerid"]).Id == userId);
        }

        [TestMethod]
        public void CheckWithRepRoleAndCustomer()
        {
            AssignHotelTeamAsOwnerContext contextObj = new AssignHotelTeamAsOwnerContext();
            var incidentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var businessUnitId = Guid.NewGuid();
            var bookingId = Guid.NewGuid();
            var cntxt = GetPluginContext(userId, incidentId, customerId);
            XrmFakedContext context = contextObj.InitialiseContext(incidentId, userId, Guid.NewGuid(), Guid.Empty);
            context.ExecutePluginWith<Tc.Crm.Plugins.Case.PostCaseCreateAsyncAssignHotelTeamAsOwner>(cntxt);
            var incidentUpdated = (from c in context.CreateQuery("incident")
                                   where c.Id == incidentId
                                   select c).ToList();            
            Assert.IsTrue(((EntityReference)incidentUpdated[0].Attributes["ownerid"]).Id == userId);            
        }

        [TestMethod]
        public void CheckWithRepRoleCustomerAndBooking()
        {
            AssignHotelTeamAsOwnerContext contextObj = new AssignHotelTeamAsOwnerContext();
            var incidentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var businessUnitId = Guid.NewGuid();
            var bookingId = Guid.NewGuid();
            var cntxt = GetPluginContext(userId, incidentId, customerId,  bookingId);
            XrmFakedContext context = contextObj.InitialiseContext(incidentId,userId,Guid.NewGuid(), customerId, businessUnitId, bookingId);
            context.ExecutePluginWith<Tc.Crm.Plugins.Case.PostCaseCreateAsyncAssignHotelTeamAsOwner>(cntxt);            
            var incidentUpdated = (from c in context.CreateQuery("incident")
                                   where c.Id == incidentId
                                   select c).ToList();
            var customerUpdated = (from c in context.CreateQuery("contact")
                                   where c.Id == customerId
                                   select c).ToList();
            Assert.IsTrue(((EntityReference)incidentUpdated[0].Attributes["ownerid"]).Id != userId);
            Assert.IsTrue(((EntityReference)customerUpdated[0].Attributes["ownerid"]).Id != userId);
        }

        [TestMethod]
        public void CheckWithRepRoleCustomerAndUserAssociatedToHotel()
        {
            AssignHotelTeamAsOwnerContext contextObj = new AssignHotelTeamAsOwnerContext();
            var incidentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var businessUnitId = Guid.NewGuid();
            var bookingId = Guid.NewGuid();
            var cntxt = GetPluginContext(userId, incidentId, customerId);
            XrmFakedContext context = contextObj.InitialiseContext(incidentId, userId, Guid.NewGuid(), customerId, businessUnitId);
            context.ExecutePluginWith<Tc.Crm.Plugins.Case.PostCaseCreateAsyncAssignHotelTeamAsOwner>(cntxt);
            var incidentUpdated = (from c in context.CreateQuery("incident")
                                   where c.Id == incidentId
                                   select c).ToList();
            var customerUpdated = (from c in context.CreateQuery("contact")
                                   where c.Id == customerId
                                   select c).ToList();
            Assert.IsTrue(((EntityReference)incidentUpdated[0].Attributes["ownerid"]).Id != userId);
            Assert.IsTrue(((EntityReference)customerUpdated[0].Attributes["ownerid"]).Id != userId);
        }
    }
}
