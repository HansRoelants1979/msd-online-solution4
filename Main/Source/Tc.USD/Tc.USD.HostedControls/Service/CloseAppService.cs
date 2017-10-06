using System;
using Microsoft.Uii.Csr;
using Tc.Crm.Common.Constants.UsdConstants;

namespace Tc.Usd.HostedControls
{
    public partial class SingleSignOnController
    {
        public void CloseApplication(RequestActionEventArgs args)
        {
            var application = GetParamValue(args, UsdParameter.Application);
            foreach (Session session in localSessionManager)
            {
                if (!session.Global)
                    localSessionManager.SetActiveSession(session.SessionId);
                foreach (IHostedApplication app in session)
                {
                    if (app.ApplicationName.Equals(application, StringComparison.OrdinalIgnoreCase))
                    {
                        FireRequestAction(new Microsoft.Uii.Csr.RequestActionEventArgs(application, UsdAction.Close, null));
                    }
                }
            }
        }
    }
}