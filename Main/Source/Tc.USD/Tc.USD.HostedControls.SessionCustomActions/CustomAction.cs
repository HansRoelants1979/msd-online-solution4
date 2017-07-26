using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Csr;
using System;
using Tc.USD.HostedControls.SessionCustomActions.Constants;


namespace Tc.USD.HostedControls.SessionCustomActions
{
    public partial class CustomAction: DynamicsBaseHostedControl
    {
        public TraceLogger LogWriter = null;
        public CustomAction(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {
            LogWriter = new TraceLogger(DataKey.DiagnosticSource);

        }
        protected override void DoAction(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            // Log process.
            LogWriter.Log($"{this.ApplicationName} -- DoAction called for action: {args.Action}", System.Diagnostics.TraceEventType.Information);
            if (args.Action.Equals(ActionName.OpenOwr, StringComparison.OrdinalIgnoreCase))
            {
                DoActionsOnOpenOwr(args);
            }
          
        }

        public void DoActionsOnOpenOwr(RequestActionEventArgs args)
        {
            Dispatcher.Invoke(this.GetSsoDetails);
            Dispatcher.Invoke(this.GetOwrSsoServiceUrl);
        }
    }
}
