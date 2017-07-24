using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Csr;
using Microsoft.Uii.Desktop.SessionManager;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tc.USD.HostedControls.SessionCustomActions
{
    public partial class CustomAction: DynamicsBaseHostedControl
    {
        public TraceLogger LogWriter = null;
        public CustomAction(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {
            LogWriter = new TraceLogger();

        }
        protected override void DoAction(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            // Log process.
            LogWriter.Log(string.Format(CultureInfo.CurrentCulture, "{0} -- DoAction called for action: {1}", this.ApplicationName, args.Action), System.Diagnostics.TraceEventType.Information);
            //if (args.Action.Equals(Constants.ActionName.CloseAllApp, StringComparison.OrdinalIgnoreCase))
            //{
            //    DoActionsOnCloseAllApp(args);
            //}
        }

        
    }
}
