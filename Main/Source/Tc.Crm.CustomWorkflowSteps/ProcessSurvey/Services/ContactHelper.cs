using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Tc.Crm.CustomWorkflowSteps.ProcessSurvey.Models;
using System.Text;
using System;

namespace Tc.Crm.CustomWorkflowSteps.ProcessSurvey.Services
{
    public static class ContactHelper
    {
        /// <summary>
        /// To find customer from survey payload
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        public static string GetLastName(Contact contact, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Trace service is null;");
            trace.Trace("Processing GetLastName - start");
            var lastName = string.Empty;
            if (contact != null && contact.Name != null)
            {
                if (!string.IsNullOrWhiteSpace(contact.Name.LastName))
                {
                    lastName = contact.Name.LastName;
                }
            }
            trace.Trace("Processing GetLastName - end");
            return lastName;
        }

        public static string GetEmail(Contact contact, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Trace service is null;");
            trace.Trace("Processing GetEmail - start");
            var email = string.Empty;
            if (contact != null && contact.Email != null)
            {
                if (!string.IsNullOrWhiteSpace(contact.Email))
                {
                    email = contact.Email;
                }
            }
            trace.Trace("Processing GetEmail - end");
            return email;
        }
    }
}
