using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.Common.Services
{
    public interface ILogger
    {
        void LogError(string message);
        void LogWarning(string message);
        void LogInformation(string message);
    }
}
