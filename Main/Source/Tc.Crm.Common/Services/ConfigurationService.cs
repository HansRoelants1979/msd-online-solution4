using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.Common.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public string DefaultUserId
        {
            get { return ConfigurationManager.ConnectionStrings["DeallocationOwner"].ConnectionString; }
        }

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DepartureDateinNextXDays")]
        public int DepartureDateInNextXDays
        {
            get
            {
                var daysInString = ConfigurationManager.AppSettings["DepartureDateinNextXDays"];
                int days;
                if (Int32.TryParse(daysInString,out days))
                {
                    return days;
                }
                throw new FormatException("DepartureDateinNextXDays has not been provided in config or has been provided in an incorrect format.");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public int ExecuteMultipleBatchSize
        {
            get
            {
                var batchInString = ConfigurationManager.AppSettings["ExecuteMultipleBatchSize"];
                int batch;
                if (Int32.TryParse(batchInString, out batch))
                {
                    return batch;
                }
                throw new FormatException("Batch has not been provided in config or has been provided in an incorrect format.");
            }
        }

        public string DestinationGatewayIds
        {
            get
            {
                return ConfigurationManager.AppSettings["DestinationGatewayIds"].ToString();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
