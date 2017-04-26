using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.CustomWorkflowSteps.QueueIdentifier.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Tc.Crm.UnitTests.CustomWorkFlowSteps;
using FakeXrmEasy;
using System.Reflection;
using Microsoft.Xrm.Sdk.Metadata;

namespace Tc.Crm.CustomWorkflowSteps.QueueIdentifier.Service.Tests
{
    [TestClass()]
    public class QueueIdentifierServiceTests
    {
        XrmFakedContext fakedContext;
        Guid q1Id = Guid.NewGuid();
        Guid q2Id = Guid.NewGuid();
        Guid q3Id = Guid.NewGuid();
        Guid u1Id = Guid.NewGuid();
        Guid u2Id = Guid.NewGuid();
        Guid sysAdminId = Guid.NewGuid();
        Guid customerRelationsBaseId = Guid.NewGuid();
        Guid idsRepId = Guid.NewGuid();
        Guid case1Id = Guid.NewGuid();
        Guid booking1Id = Guid.NewGuid();
        Guid case2Id = Guid.NewGuid();
        Guid u3Id = Guid.NewGuid();
        Guid case3Id = Guid.NewGuid();
        Guid case4Id = Guid.NewGuid();
        Guid case5Id = Guid.NewGuid();
        Guid acc1Id = Guid.NewGuid();
        Guid con1Id = Guid.NewGuid();

        [TestInitialize()]
        public void Setup()
        {
            fakedContext = new XrmFakedContext();
            fakedContext.Data = new Dictionary<string, Dictionary<Guid, Entity>>();

            #region Source Markets
            var sourceMarkets = new Dictionary<Guid, Entity>();
            var uk = new Entity("tc_country");
            uk["tc_iso_code"] = "GB";
            uk["tc_countryname"] = "United Kingdom";
            var ukId = Guid.NewGuid();
            uk.Id = ukId;
            uk["tc_countryid"] = ukId;
            sourceMarkets.Add(ukId, uk);

            var de = new Entity("tc_country");
            de["tc_iso_code"] = "DE";
            de["tc_countryname"] = "Germany";
            var deId = Guid.NewGuid();
            de["tc_countryid"] = deId;
            de.Id = deId;
            sourceMarkets.Add(deId, de);

            var fr = new Entity("tc_country");
            fr["tc_iso_code"] = "DE";
            fr["tc_countryname"] = "Germany";
            var frId = Guid.NewGuid();
            fr["tc_countryid"] = deId;
            fr.Id = frId;
            sourceMarkets.Add(frId, fr);

            #endregion Source Markets

            #region Queues
            var queues = new Dictionary<Guid, Entity>();
            var q1 = new Entity("queue");
            q1["name"] = "Customer Relations - UK";
            q1["tc_sourcemarket"] = new EntityReference("tc_country", ukId);
            q1["tc_department"] = new OptionSetValue(Department.CustomerRelations);
            q1.Id = q1Id;
            q1["queueid"] = q1Id;
            queues.Add(q1Id,q1);

            var q2 = new Entity("queue");
            q2["name"] = "Customer Relations - DE";
            q2["tc_sourcemarket"] = new EntityReference("tc_country", deId);
            q2["tc_department"] = new OptionSetValue(Department.CustomerRelations);
            q2.Id = q2Id;
            q2["queueid"] = q2Id;
            queues.Add(q2Id, q2);

            var q3 = new Entity("queue");
            q3["name"] = "Customer Relations - DE";
            q3["tc_sourcemarket"] = new EntityReference("tc_country", frId);
            q3["tc_department"] = new OptionSetValue(Department.InDestinationRep);
            q3.Id = q3Id;
            q3["queueid"] = q3Id;
            queues.Add(q3Id, q3);
            #endregion Queues

            #region Roles
            var roles = new Dictionary<Guid, Entity>();
            var sysAdmin = new Entity("role");
            sysAdmin["roleid"] = sysAdminId;
            sysAdmin["name"] = "System Administrator";
            sysAdmin.Id = sysAdminId;
            roles.Add(sysAdminId, sysAdmin);

            var customerRelationsBase = new Entity("role");
            customerRelationsBase["roleid"] = customerRelationsBaseId;
            customerRelationsBase["name"] = QueueName.TcCustomerRelationsBase;
            customerRelationsBase.Id = customerRelationsBaseId;
            roles.Add(customerRelationsBaseId, customerRelationsBase);

            var iDsRep = new Entity("role");
            iDsRep["roleid"] = idsRepId;
            iDsRep["name"] = QueueName.TcIdsBase;
            iDsRep.Id = idsRepId;
            roles.Add(idsRepId, iDsRep);
            #endregion roles

            #region Users
            var users = new Dictionary<Guid, Entity>();

            var u1 = new Entity("systemuser");
            u1["systemuserid"] = u1Id;
            users.Add(u1Id, u1);

            var u2 = new Entity("systemuser");
            u2["systemuserid"] = u2Id;
            users.Add(u2Id, u2);

            var u3 = new Entity("systemuser");
            u3["systemuserid"] = u3Id;
            users.Add(u3Id, u3);
            #endregion Users

            #region  system user roles
            var systemUserRoles = new Dictionary<Guid, Entity>();

            #region user 1
            var u1SysAdmin = new Entity("systemuserroles");
            u1SysAdmin["systemuserid"] = new EntityReference("systemuser", u1Id);
            u1SysAdmin["roleid"] = new EntityReference("role", sysAdminId);
            var u1SysAdminId = Guid.NewGuid();
            u1SysAdmin["systemuserroleid"] = u1SysAdminId;
            u1SysAdmin.Id= u1SysAdminId;
            systemUserRoles.Add(u1SysAdminId, u1SysAdmin);

            var u1CustomerRelations = new Entity("systemuserroles");
            u1CustomerRelations["systemuserid"] = new EntityReference("systemuser", u1Id);
            u1CustomerRelations["roleid"] = new EntityReference("role", customerRelationsBaseId);
            var u1CustomerRelationsId = Guid.NewGuid();
            u1CustomerRelations["systemuserroleid"] = u1CustomerRelationsId;
            u1CustomerRelations.Id = u1CustomerRelationsId;
            systemUserRoles.Add(u1CustomerRelationsId, u1CustomerRelations);
            #endregion user 1

            #region user 2
            var u2SysAdmin = new Entity("systemuserroles");
            u2SysAdmin["systemuserid"] = new EntityReference("systemuser", u2Id);
            u2SysAdmin["roleid"] = new EntityReference("role", sysAdminId);
            var u2SysAdminId = Guid.NewGuid();
            u1SysAdmin["systemuserroleid"] = u2SysAdminId;
            systemUserRoles.Add(u2SysAdminId, u2SysAdmin);

            #endregion user 2

            #region user 3
            var u3iDSRep = new Entity("systemuserroles");
            u3iDSRep["systemuserid"] = new EntityReference("systemuser", u3Id);
            u3iDSRep["roleid"] = new EntityReference("role", idsRepId);
            var u3iDSRepId = Guid.NewGuid();
            u1SysAdmin["systemuserroleid"] = u3iDSRepId;
            systemUserRoles.Add(u3iDSRepId, u3iDSRep);

            #endregion user 3

            #endregion system user roles

            #region bookings
            var bookings = new Dictionary<Guid, Entity>();
            var booking1 = new Entity("tc_booking");
            booking1["tc_bookingid"] = booking1Id;
            booking1["tc_sourcemarketid"] = new EntityReference("tc_country", deId);
            bookings.Add(booking1Id, booking1);
            #endregion bookings

            #region contacts
            var contacts = new Dictionary<Guid, Entity>();
            var con1 = new Entity("contact");
            con1["contactid"] = con1Id;
            con1["tc_sourcemarketid"] = new EntityReference("tc_country",frId);
            contacts.Add(con1Id,con1);
            #endregion contacts

            #region accounts
            var accounts = new Dictionary<Guid, Entity>();
            var acc1 = new Entity("account");
            acc1["accountid"] = acc1Id;
            acc1["tc_sourcemarketid"] = new EntityReference("tc_country", frId);
            accounts.Add(acc1Id, acc1);
            #endregion accounts

            #region Cases
            var cases = new Dictionary<Guid, Entity>();
            var case1 = new Entity("incident");
            case1["incidentid"] = case1Id;
            case1["tc_sourcemarketid"] = new EntityReference("tc_country", ukId);
            case1["ownerid"] = new EntityReference("systemuser", u1Id);
            cases.Add(case1Id, case1);

            var case2 = new Entity("incident");
            case2["incidentid"] = case2Id;
            case2["tc_sourcemarketid"] = new EntityReference("tc_country", ukId);
            case2["tc_bookingid"] = new EntityReference("tc_booking", booking1Id);
            case2["ownerid"] = new EntityReference("systemuser", u1Id);
            cases.Add(case2Id, case2);

            var case3 = new Entity("incident");
            case3["incidentid"] = case3Id;
            case3["tc_sourcemarketid"] = new EntityReference("tc_country", ukId);
            case3["tc_bookingid"] = new EntityReference("tc_booking", booking1Id);
            case3["ownerid"] = new EntityReference("systemuser", u3Id);
            case3["customerid"] = new EntityReference("contact", con1Id);
            cases.Add(case3Id, case3);

            var case4 = new Entity("incident");
            case4["incidentid"] = case4Id;
            case4["tc_sourcemarketid"] = new EntityReference("tc_country", ukId);
            case4["tc_bookingid"] = new EntityReference("tc_booking", booking1Id);
            case4["ownerid"] = new EntityReference("systemuser", u2Id);
            cases.Add(case4Id, case4);

            var case5 = new Entity("incident");
            case5["incidentid"] = case5Id;
            case5["tc_sourcemarketid"] = new EntityReference("tc_country", ukId);
            case5["tc_bookingid"] = new EntityReference("tc_booking", booking1Id);
            case5["ownerid"] = new EntityReference("systemuser", u3Id);
            case5["customerid"] = new EntityReference("account", acc1Id);
            cases.Add(case5Id, case5);
            #endregion Cases

            fakedContext.Data.Add("role", roles);
            fakedContext.Data.Add("systemuser", users);
            fakedContext.Data.Add("systemuserroles", systemUserRoles);
            fakedContext.Data.Add("tc_country", sourceMarkets);
            fakedContext.Data.Add("queue", queues);
            fakedContext.Data.Add("incident", cases);
            fakedContext.Data.Add("tc_booking", bookings);
            fakedContext.Data.Add("contact", contacts);
            fakedContext.Data.Add("account", accounts);

            fakedContext.OptionSetValuesMetadata = new Dictionary<string, OptionSetMetadata>();
            fakedContext.OptionSetValuesMetadata.Add("tc_department", new OptionSetMetadata
            {
                IsGlobal = true
                                            ,
                Name = "tc_department"
                                            ,
                OptionSetType = OptionSetType.Picklist
            });
            fakedContext.OptionSetValuesMetadata["tc_department"].Options.Add(new OptionMetadata { Value = Department.CustomerRelations });

        }

        [TestMethod()]
        public void GetQueueByQueueNameAndQueueNameIsNull()
        {
            var inputs = new Dictionary<string, object>() {
                { "QueueName", null },
                { "Case", null }
                };


            var result = fakedContext.ExecuteCodeActivity<QueueIdentifierActivity>(inputs);
            Assert.IsNull(result["Queue"]);
        }

        [TestMethod()]
        public void GetQueueByQueueName()
        {
            var inputs = new Dictionary<string, object>() {
                { "QueueName", "Customer Relations - UK" },
                { "Case", null }
                };
            var expected = q1Id;
            var result = fakedContext.ExecuteCodeActivity<QueueIdentifierActivity>(inputs);
            var actual = ((EntityReference)result["Queue"]).Id;
            Assert.AreEqual(expected,actual);
        }

        [TestMethod()]
        public void GetQueueByQueueNameAndQueNameDoesntExist()
        {
            var inputs = new Dictionary<string, object>() {
                { "QueueName", "Customer Relations - FR" },
                { "Case", null }
                };
            var result = fakedContext.ExecuteCodeActivity<QueueIdentifierActivity>(inputs);
            Assert.IsNull(result["Queue"]);
        }
        [TestMethod()]
        public void GetQueueByCaseId_CaseSourceMarket_CustomerRelations()
        {
            fakedContext.ProxyTypesAssembly = Assembly.GetAssembly(typeof(CrmEarlyBound.Queue));
            var inputs = new Dictionary<string, object>() {
                { "QueueName", null},
                { "Case", new EntityReference("incident",case1Id) }
                };
            var result = fakedContext.ExecuteCodeActivity<QueueIdentifierActivity>(inputs);
            var expected = q1Id;
            Assert.AreEqual(expected,((EntityReference)result["Queue"]).Id);
        }
        [TestMethod()]
        public void GetQueueByCaseId_BookingSourceMarket_CustomerRelations()
        {
            fakedContext.ProxyTypesAssembly = Assembly.GetAssembly(typeof(CrmEarlyBound.Queue));
            var inputs = new Dictionary<string, object>() {
                { "QueueName", null},
                { "Case", new EntityReference("incident",case2Id) }
                };
            var result = fakedContext.ExecuteCodeActivity<QueueIdentifierActivity>(inputs);
            var expected = q2Id;
            Assert.AreEqual(expected, ((EntityReference)result["Queue"]).Id);
        }
        [TestMethod()]
        public void GetQueueByCaseId_ContactSourceMarket_iDSRep()
        {
            fakedContext.ProxyTypesAssembly = Assembly.GetAssembly(typeof(CrmEarlyBound.Queue));
            var inputs = new Dictionary<string, object>() {
                { "QueueName", null},
                { "Case", new EntityReference("incident",case3Id) }
                };
            var result = fakedContext.ExecuteCodeActivity<QueueIdentifierActivity>(inputs);
            var expected = q3Id;
            Assert.AreEqual(expected, ((EntityReference)result["Queue"]).Id);
        }
        [TestMethod()]
        public void GetQueueByCaseId_OwnerIsNotIdsRepOrCustomerRelations()
        {
            fakedContext.ProxyTypesAssembly = Assembly.GetAssembly(typeof(CrmEarlyBound.Queue));
            var inputs = new Dictionary<string, object>() {
                { "QueueName", null},
                { "Case", new EntityReference("incident",case4Id) }
                };
            var result = fakedContext.ExecuteCodeActivity<QueueIdentifierActivity>(inputs);
            Assert.IsNull(result["Queue"]);
        }

        [TestMethod()]
        public void GetQueueByCaseId_AccountSourceMarket_iDSRep()
        {
            fakedContext.ProxyTypesAssembly = Assembly.GetAssembly(typeof(CrmEarlyBound.Queue));
            var inputs = new Dictionary<string, object>() {
                { "QueueName", null},
                { "Case", new EntityReference("incident",case5Id) }
                };
            var result = fakedContext.ExecuteCodeActivity<QueueIdentifierActivity>(inputs);
            var expected = q3Id;
            Assert.AreEqual(expected, ((EntityReference)result["Queue"]).Id);
        }
    }

    
}