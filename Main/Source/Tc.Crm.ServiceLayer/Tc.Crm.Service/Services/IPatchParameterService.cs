using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.Service.Services
{
    public interface IPatchParameterService
    {
        Dictionary<string, string> Map { get; set; }
    }
}
