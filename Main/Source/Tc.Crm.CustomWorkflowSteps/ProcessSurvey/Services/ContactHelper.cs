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
        /// To get customer first name
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        public static string GetFirstName(Contact contact, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Trace service is null;");
            trace.Trace("Processing GetFirstName - start");
            var firstName = string.Empty;
            if (contact != null && contact.Name != null)
            {
                if (!string.IsNullOrWhiteSpace(contact.Name.FirstName))
                {
                    firstName = contact.Name.FirstName;
                }
            }
            trace.Trace("Processing GetFirstName - end");
            return firstName;
        }

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

        /// <summary>
        /// To get emailof customer
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
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

        /// <summary>
        /// To get phone number of the customer
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        public static string GetPhone(Contact contact, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Trace service is null;");
            trace.Trace("Processing GetPhone - start");
            var phone = string.Empty;
            if (contact != null && contact.Phone != null)
            {
                if (!string.IsNullOrWhiteSpace(contact.Phone))
                {
                    phone = contact.Phone;
                }
            }
            trace.Trace("Processing GetPhone - end");
            return phone;
        }
    }
}
