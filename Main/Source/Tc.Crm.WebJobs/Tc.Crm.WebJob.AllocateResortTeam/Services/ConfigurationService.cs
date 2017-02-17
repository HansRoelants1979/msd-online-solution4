using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.WebJob.AllocateResortTeam.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Crm"].ConnectionString;
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
