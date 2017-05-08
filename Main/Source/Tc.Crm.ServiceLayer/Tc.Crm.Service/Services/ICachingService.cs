using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tc.Crm.Service.Services
{
    public interface ICachingService
    {
        void Cache(string bucket);
       
    }
}