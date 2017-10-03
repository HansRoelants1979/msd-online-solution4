using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Csr;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Tc.Crm.Common;
using Tc.Crm.Common.IntegrationLayer.Jti.Models;
using EntityRecords = Tc.Crm.Common.Constants.EntityRecords;
using Tc.Usd.HostedControls.Service;
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