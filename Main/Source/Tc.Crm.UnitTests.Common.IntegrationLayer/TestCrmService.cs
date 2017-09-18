using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Common.Models;
using Tc.Crm.Common.Services;

namespace Tc.Crm.UnitTests.Common.IL
{
    public enum DataSwitch
    {
        NoRecordsReturned = 0,
        CollectionWithNoRecordsReturned = 1,
        ReturnsData = 2,
    }
    public class TestCrmService : ICrmService
    {
        private readonly Func<EntityCollection> executor;

        public TestCrmService(Func<EntityCollection> executor)
        {
            this.executor = executor;
        }

        public EntityCollection RetrieveMultipleRecordsFetchXml(string query, int numberOfElements)
        {
            return executor.Invoke();
        }

        public void BulkAssign(Collection<AssignInformation> assignRequests, int batchSize)
        {
            throw new NotImplementedException();
        }

        public void BulkUpdate(IEnumerable<Entity> entities, int batchSize)
        {
            throw new NotImplementedException();
        }

        public EntityCollection RetrieveMultipleRecordsFetchXml(string query)
        {
            return executor.Invoke();
        }

        public Guid Create(Entity entity)
        {
           return Guid.NewGuid();
        }

        public void Update(Entity entity)
        {
        }
    }
}