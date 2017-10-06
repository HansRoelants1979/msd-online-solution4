using System;
using Microsoft.Uii.Csr;
using Tc.Crm.Common.Constants.UsdConstants;
using System.Collections.Generic;

namespace Tc.Usd.HostedControls
{
    public partial class SingleSignOnController
    {
        public void CloseApplication(RequestActionEventArgs args)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error has occurred while closing the applications.{ex.StackTrace.ToString()}");
                var eventParams = new Dictionary<string, string>
                {
                    {"text", "Unexpected error has occurred while closing the applications."},
                    {"caption", "Closing App - Error"},
                };
                FireRequestAction(new Microsoft.Uii.Csr.RequestActionEventArgs(UsdHostedControl.CrmGlobalManager, UsdAction.DisplayMessage, null));
            }
        }
    }
}