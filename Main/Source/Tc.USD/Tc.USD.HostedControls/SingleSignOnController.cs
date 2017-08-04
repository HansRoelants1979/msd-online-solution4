using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Csr;
using System;
using Tc.Usd.HostedControls.Constants;

namespace Tc.Usd.HostedControls
{
    public partial class SingleSignOnController : DynamicsBaseHostedControl
    {
        public TraceLogger LogWriter = null;
        public SingleSignOnController(Guid appID, string appName, string initString)
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
            Dispatcher.InvokeAsync(this.GetSsoDetails);
            Dispatcher.InvokeAsync(this.GetOwrSsoServiceUrl);
            Dispatcher.InvokeAsync(this.GetPrivateInfo);
        }
    }
}

