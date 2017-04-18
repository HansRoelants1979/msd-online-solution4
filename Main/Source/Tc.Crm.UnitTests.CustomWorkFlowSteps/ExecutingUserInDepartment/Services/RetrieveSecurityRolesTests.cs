using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.UnitTests.CustomWorkFlowSteps;
using Microsoft.Xrm.Sdk;
using Tc.Crm.CustomWorkflowSteps.ExecutingUserInDepartment.Service;

namespace Tc.Crm.UnitTests.CustomWorkFlowSteps.ExecutingUserInDepartment.Services
{

    [TestClass()]
    public class RetrieveSecurityRolesTests
    {
        TestTracingService trace;
        TestOrganizationService service;
        [TestInitialize()]
        public void Setp()
        {
            trace = new TestTracingService();
            service = new TestOrganizationService();
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

        //[TestMethod()]
        //public void RetrieveSecurityRole_IslnRole()
        //{
        //    string securityRoleName = "system administrator";
        //    Guid userId = new Guid("64826A8D-5DE9-E611-8100-3863BB351D00");
        //    var result = RetrieveSecurityRoles.GetSecurityRoles(securityRoleName, userId, service, trace);
        //    //Assert.That(result, Is.True);
        //    Assert.IsTrue(result = true);
        //}

        //[TestMethod()]
        //public void RetrieveSecurityRole_IsNotlnRole()
        //{
        //    string securityRoleName = "XX";
        //    Guid userId = new Guid("64826A8D-5DE9-E611-8100-3863BB351D00"); 
        //    var result = RetrieveSecurityRoles.GetSecurityRoles(securityRoleName, userId, service, trace);
        //    //Assert.That(result, Is.True);
        //    Assert.IsTrue(result = false);
        //}


    }
}
