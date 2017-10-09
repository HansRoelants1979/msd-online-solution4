using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.CustomWorkflowSteps.ProcessCustomer.Services
{
    public class FieldService
    {
        private Entity entity;
        private List<string> patchList;

        public FieldService(Entity entity, List<string> patchList)
        {
            this.entity = entity;
            this.patchList = patchList;
        }
        public void PopulateField(string attributeName, string value)
        {
            if (patchList.Contains(attributeName))
                entity[attributeName] = value;
        }

        public void PopulateField(string attributeName, OptionSetValue value)
        {
            if (patchList.Contains(attributeName))
                entity[attributeName] = value;
        }

        public void PopulateField(string attributeName, EntityReference value)
        {
            if (patchList.Contains(attributeName))
                entity[attributeName] = value;
        }
        public void PopulateField(string attributeName, DateTime? value)
        {
            if (!patchList.Contains(attributeName)) return;
            if (value == null)
                entity[attributeName] = null;
            else
                entity[attributeName] = value.Value;
        }
       
    }
}
