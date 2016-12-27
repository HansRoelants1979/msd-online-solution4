using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.Service.BusinessServices.CRM
{
    public interface IDataService
    {
        Task<Guid> Create(string uri,JObject obj);
        Task<bool> Update(string uri,JObject obj);
        Task Fetch(string uri,string queryOptions);
    }
}
