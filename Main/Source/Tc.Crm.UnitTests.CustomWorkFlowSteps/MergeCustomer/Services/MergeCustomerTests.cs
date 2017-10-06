using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Xrm.Sdk;
using Tc.Crm.CustomWorkflowSteps;
using FakeXrmEasy;
using System.Collections.Generic;

using Attributes = Tc.Crm.CustomWorkflowSteps.Attributes;

namespace Tc.Crm.UnitTests.CustomWorkFlowSteps.MergeCustomer
{

    [TestClass()]
    public class MergerCustomerTests
    {
        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException))]
        public void CheckEntityMergeWithInvalidEntityMerge()
        {
            Guid entityMergeId = Guid.NewGuid();
            var context = new XrmFakedContext();
            var entityMerge = new Entity("tc_entitymerge", entityMergeId);
            entityMerge.Attributes.Add(Attributes.EntityMerge.Master, null);
            entityMerge.Attributes.Add(Attributes.EntityMerge.Subordinate, null);
            var inputs = new Dictionary<string, object>()
            {
                { "EntityMerge", entityMerge.ToEntityReference() }
            };
            var entityMerges = new Dictionary<Guid, Entity>();
            entityMerges.Add(entityMergeId, entityMerge);
            context.Data.Add("tc_entitymerge", entityMerges);
            var result = context.ExecuteCodeActivity<MergeCustomerActivity>(inputs);           
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException))]
        public void CheckEntityMergeWithValidEntityMerge()
        {
            Guid entityMergeId = Guid.NewGuid();
            var context = new XrmFakedContext();

            // create setup contact records
            var contact1Guid = Guid.NewGuid();
            var contact1 = new Entity("contact", contact1Guid);
            var contact2Guid = Guid.NewGuid();
            var contact2 = new Entity("contact", contact2Guid);

            // create setup entity merge records
            var entityMerge = new Entity("tc_entitymerge", entityMergeId);
            entityMerge.Attributes.Add(Attributes.EntityMerge.Master, contact1.ToEntityReference());
            entityMerge.Attributes.Add(Attributes.EntityMerge.Subordinate, contact2.ToEntityReference());

            var inputs = new Dictionary<string, object>()
            {
                { "EntityMerge", entityMerge.ToEntityReference() }
            };            

            var entityMerges = new Dictionary<Guid, Entity>();
            entityMerges.Add(entityMergeId, entityMerge);
            context.Data.Add("tc_entitymerge", entityMerges);

            var contacts = new Dictionary<Guid, Entity>();
            contacts.Add(contact1Guid, contact1);
            contacts.Add(contact2Guid, contact2);
            context.Data.Add("contact", contacts);

            var result = context.ExecuteCodeActivity<MergeCustomerActivity>(inputs);
        }
    }
}
