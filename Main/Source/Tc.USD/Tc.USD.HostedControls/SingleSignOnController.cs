using System;
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Csr;
using Tc.Crm.Common.IntegrationLayer.Jti.Service;
using Tc.Crm.Common.Services;
using EntityRecords = Tc.Crm.Common.Constants.EntityRecords;
using Tc.Usd.HostedControls.Models;
using Tc.Crm.Common.Constants.UsdConstants;

namespace Tc.Usd.HostedControls
{
    public partial class SingleSignOnController : DynamicsBaseHostedControl
    {
        private readonly ILogger _logger;
        private readonly IJwtService _jtiService;

        public SingleSignOnController(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {
            _logger = new UsdLogger(new TraceLogger(EntityRecords.Configuration.OwrDiagnosticSource));
            _jtiService = new JwtService(_logger);
        }

        protected override void DoAction(RequestActionEventArgs args)
        {
            _logger.LogInformation($"{ApplicationName} -- DoAction called for action: {args.Action}");
            if (args.Action.Equals(EntityRecords.Configuration.OpenOwr, StringComparison.OrdinalIgnoreCase))
                DoActionsOnOpenOwr(args);
            else if (args.Action.Equals(UsdAction.OpenWebRioGlobal, StringComparison.OrdinalIgnoreCase))
                DoActionsOnOpenWebRio(args, true);
            else if (args.Action.Equals(UsdAction.CloseApp, StringComparison.OrdinalIgnoreCase))
                DoActionsOnCloseApp(args);
            else
                return;
        }

        private void DoActionsOnCloseApp(RequestActionEventArgs args)
        {
            Dispatcher.InvokeAsync(() => { CloseApplication(args); });
        }
        private void DoActionsOnOpenWebRio(RequestActionEventArgs args,bool global)
        {
            Dispatcher.InvokeAsync(() => { OpenWebRio(args,global); });
        }

        public void DoActionsOnOpenOwr(RequestActionEventArgs args)
        {
            Dispatcher.InvokeAsync(()=> { CallSsoService(args); });
        }
    }
}