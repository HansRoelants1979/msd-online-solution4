using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessCustomer.Services
{
    public static class ContactHelper
    {
        public static Entity GetContactEntityForCustomerPayload(Customer customer,ITracingService trace )
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Contact populate fields - start");
            if (customer == null) throw new InvalidPluginExecutionException("Customer payload is null.");
            if (customer.CustomerIdentifier == null)
                throw new InvalidPluginExecutionException("Customer Identifier could not be retrieved from payload.");
            if (string.IsNullOrWhiteSpace(customer.CustomerIdentifier.CustomerId))
                throw new InvalidPluginExecutionException("Customer Identifier could not be retrieved from payload.");

            Entity contact = new Entity(EntityName.Contact);
            var sourceSystemID = (!string.IsNullOrWhiteSpace(customer.CustomerIdentifier.CustomerId)) ?
                                    customer.CustomerIdentifier.CustomerId : string.Empty;
            contact[Attributes.Contact.SourceSystemId] = sourceSystemID;
            contact[Attributes.Contact.DuplicateSourceSystemId] = sourceSystemID;
            var sourceMarket = (!string.IsNullOrWhiteSpace(customer.CustomerIdentifier.SourceMarket))
                               ? new EntityReference(EntityName.Country,
                               new Guid(customer.CustomerIdentifier.SourceMarket)) : null;
            contact[Attributes.Contact.SourceMarketId] = sourceMarket;
            PopulateIdentityInformation(contact, customer.CustomerIdentity, trace);
            PopulateAddress(contact, customer.Address, trace);
            PopulateEmail(contact, customer.Email, trace);
            PopulatePhone(contact, customer.Phone, trace);
            PopulatePermission(contact, customer.Permissions, trace);
            if (customer.CustomerGeneral != null){
                contact[Attributes.Contact.StatusCode] = CommonXrm.GetCustomerStatus(customer.CustomerGeneral.CustomerStatus);
            }
            if (customer.Additional != null){
                contact[Attributes.Contact.Segment] = CommonXrm.GetSegment(customer.Additional.Segment);
                DateTime? dateOfDeath = null;
                if (!string.IsNullOrWhiteSpace(customer.Additional.DateOfDeath))
                    dateOfDeath = Convert.ToDateTime(customer.Additional.DateOfDeath);
                contact[Attributes.Contact.DateOfDeath] = dateOfDeath;
            }
            trace.Trace("Contact populate fields - end");
            return contact;
        }
       
        private static void PopulateIdentityInformation(Entity contact, CustomerIdentity identity,ITracingService trace)
        {
            trace.Trace("Contact populate identity - start");            
            if (identity == null) return;
            contact[Attributes.Contact.FirstName] = (!string.IsNullOrWhiteSpace(identity.FirstName)) ? identity.FirstName : string.Empty;
            contact[Attributes.Contact.MiddleName] = (!string.IsNullOrWhiteSpace(identity.MiddleName)) ? identity.MiddleName : string.Empty;
            contact[Attributes.Contact.LastName] = (!string.IsNullOrWhiteSpace(identity.LastName)) ? identity.LastName : string.Empty;
            contact[Attributes.Contact.AcademicTitle] = (!string.IsNullOrWhiteSpace(identity.AcademicTitle)) ? identity.AcademicTitle : string.Empty;
            contact[Attributes.Contact.Language] = CommonXrm.GetLanguage(identity.Language);
            contact[Attributes.Contact.Gender] = CommonXrm.GetGender(identity.Gender);
            if (!string.IsNullOrWhiteSpace(identity.Salutation)){
                contact[Attributes.Contact.Salutation] = CommonXrm.GetSalutation(identity.Salutation);
            }
            DateTime? dateOfBirth = null;
            if (!string.IsNullOrWhiteSpace(identity.Birthdate))
                dateOfBirth = Convert.ToDateTime(identity.Birthdate);
            contact[Attributes.Contact.Birthdate] = dateOfBirth;
            trace.Trace("Contact populate identity - end");
        }

        private static void PopulateAddress(Entity contact, Address[] addresses, ITracingService trace)
        {
            if (addresses == null || addresses.Length <= 0) return;
            trace.Trace("Contact populate address - start");                       
            var address1 = addresses[0];
            var address2 = addresses.Length > 1 ? addresses[1] : null;

            if (address1 != null)
            {
                contact[Attributes.Contact.Address1AdditionalInformation] =(!string.IsNullOrWhiteSpace(address1.AdditionalAddressInfo)) ?
                    address1.AdditionalAddressInfo : string.Empty;
                contact[Attributes.Contact.Address1FlatOrUnitNumber] = (!string.IsNullOrWhiteSpace(address1.FlatNumberUnit)) ?
                    address1.FlatNumberUnit : string.Empty;
                contact[Attributes.Contact.Address1HouseNumberOrBuilding] = (!string.IsNullOrWhiteSpace(address1.HouseNumberBuilding)) ?
                    address1.HouseNumberBuilding : string.Empty;
                contact[Attributes.Contact.Address1Town] = (!string.IsNullOrWhiteSpace(address1.Town)) ?
                    address1.Town : string.Empty;
                contact[Attributes.Contact.Address1Street] = (!string.IsNullOrWhiteSpace(address1.Street)) ?
                    address1.Street : string.Empty;
                contact[Attributes.Contact.Address1PostalCode] = (!string.IsNullOrWhiteSpace(address1.PostalCode)) ?
                    address1.PostalCode : string.Empty;
                contact[Attributes.Contact.Address1County] = (!string.IsNullOrWhiteSpace(address1.County)) ?
                    address1.County : string.Empty;
                var country = !string.IsNullOrWhiteSpace(address1.Country) ? new EntityReference(EntityName.Country,
                                                                   new Guid(address1.Country)) : null;
                contact[Attributes.Contact.Address1CountryId] = country;
            }
            if (address2 != null)
            {
                contact[Attributes.Contact.Address2AdditionalInformation] = (!string.IsNullOrWhiteSpace(address2.AdditionalAddressInfo)) ?
                    address2.AdditionalAddressInfo : string.Empty;
                contact[Attributes.Contact.Address2FlatOrUnitNumber] = (!string.IsNullOrWhiteSpace(address2.FlatNumberUnit)) ?
                    address2.FlatNumberUnit : string.Empty;
                contact[Attributes.Contact.Address2HouseNumberOrBuilding] = (!string.IsNullOrWhiteSpace(address2.HouseNumberBuilding)) ?
                    address2.HouseNumberBuilding : string.Empty;
                contact[Attributes.Contact.Address2Town] = (!string.IsNullOrWhiteSpace(address2.Town)) ?
                    address2.Town : string.Empty;
                contact[Attributes.Contact.Address2Street] = (!string.IsNullOrWhiteSpace(address2.Street)) ?
                    address2.Street : string.Empty;
                contact[Attributes.Contact.Address2PostalCode] = (!string.IsNullOrWhiteSpace(address2.PostalCode)) ?
                    address2.PostalCode : string.Empty;
                contact[Attributes.Contact.Address2County] = (!string.IsNullOrWhiteSpace(address2.County)) ?
                    address2.County : string.Empty;
                var country = !string.IsNullOrWhiteSpace(address2.Country) ? new EntityReference(EntityName.Country,
                                                                   new Guid(address2.Country)) : null;
                contact[Attributes.Contact.Address2CountryId] = country;
            }
            trace.Trace("Contact populate address - end");
        }      

        private static void PopulateEmail(Entity contact, Email[] emails, ITracingService trace)
        {            
            if (emails == null || emails.Length <= 0) return;
            trace.Trace("Contact populate email - start");
            var email1 = emails[0];
            var email2 = emails.Length > 1 ? emails[1] : null;
            var email3 = emails.Length > 2 ? emails[2] : null;
           if (email1 != null){
                contact[Attributes.Contact.EmailAddress1Type] = CommonXrm.GetEmailType(email1.EmailType);
                contact[Attributes.Contact.EmailAddress1] = (!string.IsNullOrWhiteSpace(email1.Address)) ? email1.Address : string.Empty; 
            }
            if (email2 != null){
                contact[Attributes.Contact.EmailAddress2Type] = CommonXrm.GetEmailType(email2.EmailType);
                contact[Attributes.Contact.EmailAddress2] = (!string.IsNullOrWhiteSpace(email2.Address)) ? email2.Address : string.Empty; 
            }
            if (email3 != null){
                contact[Attributes.Contact.EmailAddress3Type] = CommonXrm.GetEmailType(email3.EmailType);
                contact[Attributes.Contact.EmailAddress3] = (!string.IsNullOrWhiteSpace(email3.Address)) ? email3.Address : string.Empty;
            }
            trace.Trace("Contact populate email - end");
        }      

        private static void PopulatePhone(Entity contact, Phone[] phoneNumbers, ITracingService trace){
            if (phoneNumbers == null || phoneNumbers.Length <= 0) return;
            trace.Trace("Contact populate phone - start");
            var phone1 = phoneNumbers[0];
            var phone2 = phoneNumbers.Length > 1 ? phoneNumbers[1] : null;
            var phone3 = phoneNumbers.Length > 2 ? phoneNumbers[2] : null;
            if (phone1 != null){
                contact[Attributes.Contact.Telephone1Type] = CommonXrm.GetPhoneType(phone1.PhoneType);
                contact[Attributes.Contact.Telephone1] = (!string.IsNullOrWhiteSpace(phone1.Number)) ? phone1.Number : string.Empty;  
            }
            if (phone2 != null){
                contact[Attributes.Contact.Telephone2Type] = CommonXrm.GetPhoneType(phone2.PhoneType);
                contact[Attributes.Contact.Telephone2] = (!string.IsNullOrWhiteSpace(phone2.Number)) ? phone2.Number : string.Empty;
            }
            if (phone3 != null) {
                contact[Attributes.Contact.Telephone3Type] = CommonXrm.GetPhoneType(phone3.PhoneType);
                contact[Attributes.Contact.Telephone3] = (!string.IsNullOrWhiteSpace(phone3.Number)) ? phone3.Number : string.Empty;
            }
            trace.Trace("Contact populate phone - end");
        }      

        private static void PopulatePermission(Entity contact, Permissions permissions, ITracingService trace){
            if (permissions == null) return;
            trace.Trace("Contact populate permission - start");
            contact[Attributes.Contact.SendMarketingByPost] = CommonXrm.GetMarketingByPost(permissions.DoNotAllowMail);
            contact[Attributes.Contact.MarketingByPhone] = CommonXrm.GetMarketingByPhone(permissions.DoNotAllowPhoneCalls);
            contact[Attributes.Contact.SendMarketingBySms] = CommonXrm.GetMarketingBySms(permissions.DoNotAllowSms);
            contact[Attributes.Contact.SendMarketingByEmail] = CommonXrm.GetMarketingByEmail(permissions.DoNotAllowMail);
            contact[Attributes.Contact.ThomasCookMarketingConsent] = CommonXrm.GetMarketingConsent(permissions.AllowMarketing); 
            trace.Trace("Contact populate permission - end");
        }      
    }
}


