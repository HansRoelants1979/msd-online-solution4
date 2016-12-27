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
        Task<bool> Update(Customer customer);
        Task<Guid> Create(Customer customer);
        Task<Customer> Upsert(Customer customer);
    }
}
