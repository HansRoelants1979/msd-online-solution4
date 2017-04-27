using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.UnitTests.CustomWorkFlowSteps;
using Microsoft.Xrm.Sdk;
using Tc.Crm.CustomWorkflowSteps.ExecutingUserInDepartment.Service;
using Tc.Crm.CustomWorkflowSteps.ExecutingUserInDepartment;
using FakeXrmEasy;
using System.Reflection;

namespace Tc.Crm.UnitTests.CustomWorkFlowSteps.ExecutingUserInDepartment.Services
{

    [TestClass()]
    public class RetrieveSecurityRolesTests
    {
        XrmFakedContext fakedContext;

        Guid u2Id = Guid.NewGuid();
        Guid u1Id = Guid.NewGuid();
        Guid sysAdminId = Guid.NewGuid();
        Guid sysCustomizerId = Guid.NewGuid();


        TestTracingService trace;
        IOrganizationService service;
        [TestInitialize()]
        public void Setp()
        {
            fakedContext = new XrmFakedContext();
            trace = new TestTracingService();
            service = fakedContext.GetOrganizationService();
            fakedContext.Data = new Dictionary<string, Dictionary<Guid, Entity>>();

            #region Roles
            var roles = new Dictionary<Guid, Entity>();
            var sysAdmin = new Entity("role");
            sysAdmin["roleid"] = sysAdminId;
            sysAdmin["name"] = "System Administrator";
            sysAdmin.Id = sysAdminId;
            roles.Add(sysAdminId, sysAdmin);

            var sysCustomizer = new Entity("role");
            sysCustomizer["roleid"] = sysAdminId;
            sysCustomizer["name"] = "System Customizer";
            sysCustomizer.Id = sysCustomizerId;
            roles.Add(sysCustomizerId, sysCustomizer);

            #endregion roles

            #region Users
            var users = new Dictionary<Guid, Entity>();
            
            var u2 = new Entity("systemuser");
            u2["systemuserid"] = u2Id;
            users.Add(u2Id, u2);

            var u1 = new Entity("systemuser");
            u1["systemuserid"] = u1Id;
            users.Add(u1Id, u1);

            #endregion Users

            #region  system user roles
            var systemUserRoles = new Dictionary<Guid, Entity>();

            #region user 2
            var u2SysAdmin = new Entity("systemuserroles");
            u2SysAdmin["systemuserid"] = new EntityReference("systemuser", u2Id);
            u2SysAdmin["roleid"] = new EntityReference("role", sysAdminId);
            var u2SysAdminId = Guid.NewGuid();
            u2SysAdmin["systemuserroleid"] = u2SysAdminId;
            systemUserRoles.Add(u2SysAdminId, u2SysAdmin);

            var u2SysCustomizer = new Entity("systemuserroles");
            u2SysCustomizer["systemuserid"] = new EntityReference("systemuser", u1Id);
            u2SysCustomizer["roleid"] = new EntityReference("role", sysCustomizerId);
            var u2SysCustomizerId = Guid.NewGuid();
            u2SysCustomizer["systemuserroleid"] = u2SysCustomizerId;
            systemUserRoles.Add(u2SysCustomizerId, u2SysCustomizer);

            #endregion user 2

            #endregion system user roles

            fakedContext.Data.Add("role", roles);
            fakedContext.Data.Add("systemuser", users);
            fakedContext.Data.Add("systemuserroles", systemUserRoles);
        }
        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException), "service is null")]
        public void RetrieveSecurityRole_ServiceIsNull()
        {
            string securityRoleName = "system administrator";
            Guid userId = Guid.NewGuid();
            RetrieveSecurityRoles.GetSecurityRoles(securityRoleName,userId,null, trace);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException), "trace is null")]
        public void RetrieveSecurityRole_TraceIsNull()
        {
            string securityRoleName = "system administrator";
            Guid userId = Guid.NewGuid();
            RetrieveSecurityRoles.GetSecurityRoles(securityRoleName, userId, service, null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException), "securityRoleName is null")]
        public void RetrieveSecurityRole_SecurityRoleNameIsNull()
        {
            string securityRoleName = string.Empty;
            Guid userId = Guid.NewGuid();
            RetrieveSecurityRoles.GetSecurityRoles(null, userId, service, trace);
        }
        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException), "UserId is null")]
        public void RetrieveSecurityRole_UserIdIsNull()
        {
            string securityRoleName = "system administrator";
            Guid userId = Guid.Empty;
            RetrieveSecurityRoles.GetSecurityRoles(securityRoleName, Guid.Empty , service, trace);
        }

        [TestMethod()]
        public void RetrieveSecurityRole_IslnRole()
        {
            fakedContext.ProxyTypesAssembly = Assembly.GetAssembly(typeof(CrmEarlyBound.Queue));
            var inputs = new Dictionary<string, object>() {
                { "DeptBaseSecurityRoleName", "System Administrator"}
                };
            fakedContext.CallerId = new EntityReference("systemuser",u2Id);
            var result = fakedContext.ExecuteCodeActivity<ExecutingUserInDepartmentActivity>(inputs);
            var expected = true;
            Assert.AreEqual(expected, (bool)result["IsInRole"]);
        }

        [TestMethod()]
        public void retrievesecurityrole_isnotlnrole()
        {
            fakedContext.ProxyTypesAssembly = Assembly.GetAssembly(typeof(CrmEarlyBound.Queue));
            var inputs = new Dictionary<string, object>() {
                { "DeptBaseSecurityRoleName", "System Administrator"}
                };
            fakedContext.CallerId = new EntityReference("systemuser", u1Id);
            var result = fakedContext.ExecuteCodeActivity<ExecutingUserInDepartmentActivity>(inputs);
            var expected = false;
            Assert.AreEqual(expected, (bool)result["IsInRole"]);
        }


    }
}
