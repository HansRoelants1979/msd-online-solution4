using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.BusinessServices.CRM
{
    interface ICustomerService
    {
        Customer GetCustomerBy(string sourceSystemId);
        void Update(Customer customer);
        Customer Create(Customer customer);
        Customer Upsert(Customer customer);
    }
}
