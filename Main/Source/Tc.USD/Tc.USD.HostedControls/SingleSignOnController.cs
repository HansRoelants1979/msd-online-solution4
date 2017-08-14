using System;
using System.Diagnostics;
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Csr;
using Tc.Usd.HostedControls.Constants;

namespace Tc.Usd.HostedControls
{
    public partial class SingleSignOnController : DynamicsBaseHostedControl
    {
        public TraceLogger LogWriter;

        public SingleSignOnController(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {
            LogWriter = new TraceLogger(DataKey.DiagnosticSource);
        }

        protected override void DoAction(RequestActionEventArgs args)
        {
            // Log process.
            LogWriter.Log($"{ApplicationName} -- DoAction called for action: {args.Action}", TraceEventType.Information);
            if (args.Action.Equals(ActionName.OpenOwr, StringComparison.OrdinalIgnoreCase))
                DoActionsOnOpenOwr(args);
        }

        public void DoActionsOnOpenOwr(RequestActionEventArgs args)
        {
            Dispatcher.InvokeAsync(CallSsoService);
        }
    }
}