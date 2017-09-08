using System;
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Csr;
using Tc.Crm.Common.IntegrationLayer.Jti.Service;
using Tc.Crm.Common.Services;
using Tc.Usd.HostedControls.Constants;
using Tc.Usd.HostedControls.Models;

namespace Tc.Usd.HostedControls
{
    public partial class SingleSignOnController : DynamicsBaseHostedControl
    {
        private readonly ILogger _logger;
        private readonly IJwtService _jtiService;

        public SingleSignOnController(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {
            _logger = new UsdLogger(new TraceLogger(DataKey.DiagnosticSource));
            _jtiService = new JwtService(_logger);
        }

        protected override void DoAction(RequestActionEventArgs args)
        {
            _logger.LogInformation($"{ApplicationName} -- DoAction called for action: {args.Action}");
            if (args.Action.Equals(ActionName.OpenOwr, StringComparison.OrdinalIgnoreCase))
                DoActionsOnOpenOwr(args);
        }

        public void DoActionsOnOpenOwr(RequestActionEventArgs args)
        {
            Dispatcher.InvokeAsync(()=> { CallSsoService(args); });
        }
    }
}