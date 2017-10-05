using Microsoft.Xrm.Sdk;
using System;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessCustomer.Services
{
    public static class ContactHelper
    {
        public static Entity GetContactEntityForCustomerPayload(Customer customer, 
            ITracingService trace,OperationType operationType = OperationType.POST)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Contact populate fields - start");
            if (customer == null) throw new InvalidPluginExecutionException("Customer payload is null.");
            if (customer.CustomerIdentifier == null)
                throw new InvalidPluginExecutionException("Customer Identifier could not be retrieved from payload.");
            if (string.IsNullOrWhiteSpace(customer.CustomerIdentifier.CustomerId))
                throw new InvalidPluginExecutionException("Customer Identifier could not be retrieved from payload.");

            Entity contact = new Entity(EntityName.Contact);
            string[] patchList = null;
            if (!string.IsNullOrEmpty(customer.PatchParameters))
                patchList = customer.PatchParameters.Split(',');
            if (operationType == OperationType.POST){
                contact[Attributes.Contact.SourceSystemId] = customer.CustomerIdentifier.CustomerId;
                contact[Attributes.Contact.DuplicateSourceSystemId] = customer.CustomerIdentifier.CustomerId;
            }
            if(!string.IsNullOrWhiteSpace(customer.CustomerIdentifier.SourceMarket)){
                contact[Attributes.Contact.SourceMarketId] = new EntityReference(EntityName.Country, 
                    new Guid(customer.CustomerIdentifier.SourceMarket));                                                                  
            }
            PopulateIdentityInformation(contact, customer.CustomerIdentity, trace,operationType, patchList);
            PopulateAddress(contact, customer.Address, trace);
            PopulateEmail(contact, customer.Email, trace, operationType, patchList);
            PopulatePhone(contact, customer.Phone, trace, operationType, patchList);
            PopulatePermission(contact, customer.Permissions, trace, operationType, patchList);
            if (customer.CustomerGeneral != null)
            {
                contact[Attributes.Contact.StatusCode] = CommonXrm.GetCustomerStatus(customer.CustomerGeneral.CustomerStatus);
            }
            if (customer.Additional != null)
            {
                trace.Trace("Contact populate Additional details - start");
                contact[Attributes.Contact.Segment] = CommonXrm.GetSegment(customer.Additional.Segment);

                if (!string.IsNullOrWhiteSpace(customer.Additional.DateOfDeath))
                    contact[Attributes.Contact.DateOfDeath] = Convert.ToDateTime(customer.Additional.DateOfDeath);
                else
                    contact[Attributes.Contact.DateOfDeath] = null;
                trace.Trace("Contact populate Additional details - end");
            }
            trace.Trace("Contact populate fields - end");
            return contact;
        }
        public static Entity GetContactEntityForCustomerPayload(Entity existingContact, 
            Customer customer, ITracingService trace, OperationType operationType = OperationType.POST)
        {
            trace.Trace("Contact populate id - start");
            var contact = GetContactEntityForCustomerPayload(customer, trace, operationType);
            contact["contactid"] = existingContact.GetAttributeValue<Guid>("contactid");
            trace.Trace("Contact populate id - end");
            return contact;
        }
        private static void PopulateIdentityInformation(Entity contact, CustomerIdentity identity, 
            ITracingService trace, OperationType operationType = OperationType.POST,string[] patchList=null)
        {
            trace.Trace("Contact populate identity - start");            
            if (identity == null) return;
            if (!string.IsNullOrWhiteSpace(identity.FirstName))
                contact[Attributes.Contact.FirstName] = identity.FirstName;
            if (!string.IsNullOrWhiteSpace(identity.LastName))
                contact[Attributes.Contact.LastName] = identity.LastName;
            if(operationType == OperationType.PATCH){
                if(patchList!=null && patchList.Length>0){
                    if(Array.Exists(patchList, x => x == Attributes.Contact.Language)){
                        if (!string.IsNullOrWhiteSpace(identity.Language)) {
                            contact[Attributes.Contact.Language] = CommonXrm.GetLanguage(identity.Language);
                        } 
                    }
                    if(Array.Exists(patchList, x => x == Attributes.Contact.Salutation)){
                        if (!string.IsNullOrWhiteSpace(identity.Salutation)){
                            contact[Attributes.Contact.Salutation] = CommonXrm.GetSalutation(identity.Salutation);
                        } 
                    }
                    if(Array.Exists(patchList, x => x == Attributes.Contact.Gender)){
                        contact[Attributes.Contact.Gender] = CommonXrm.GetGender(identity.Gender);
                    }
                } 
            }
            else{
                if (!string.IsNullOrWhiteSpace(identity.Language)) {
                    contact[Attributes.Contact.Language] = CommonXrm.GetLanguage(identity.Language);
                }
                if (!string.IsNullOrWhiteSpace(identity.Salutation)){
                    contact[Attributes.Contact.Salutation] = CommonXrm.GetSalutation(identity.Salutation);
                }
                contact[Attributes.Contact.Gender] = CommonXrm.GetGender(identity.Gender);
            }
            if (!string.IsNullOrWhiteSpace(identity.Birthdate))
                contact[Attributes.Contact.Birthdate] = Convert.ToDateTime(identity.Birthdate);
            if (!string.IsNullOrWhiteSpace(identity.MiddleName))
                contact[Attributes.Contact.MiddleName] = identity.MiddleName;
            
            if (!string.IsNullOrWhiteSpace(identity.AcademicTitle))
                contact[Attributes.Contact.AcademicTitle] = identity.AcademicTitle;
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
                if (!string.IsNullOrWhiteSpace(address1.Country))
                    contact[Attributes.Contact.Address1CountryId] = new EntityReference(EntityName.Country, new Guid(address1.Country));
                if (!string.IsNullOrWhiteSpace(address1.AdditionalAddressInfo))
                    contact[Attributes.Contact.Address1AdditionalInformation] = address1.AdditionalAddressInfo;
                if (!string.IsNullOrWhiteSpace(address1.FlatNumberUnit))
                    contact[Attributes.Contact.Address1FlatOrUnitNumber] = address1.FlatNumberUnit;
                if (!string.IsNullOrWhiteSpace(address1.HouseNumberBuilding))
                    contact[Attributes.Contact.Address1HouseNumberOrBuilding] = address1.HouseNumberBuilding;
                if (!string.IsNullOrWhiteSpace(address1.Street))
                    contact[Attributes.Contact.Address1Street] = address1.Street;
                if (!string.IsNullOrWhiteSpace(address1.Town))
                    contact[Attributes.Contact.Address1Town] = address1.Town;
                if (!string.IsNullOrWhiteSpace(address1.County))
                    contact[Attributes.Contact.Address1County] = address1.County;
                if (!string.IsNullOrWhiteSpace(address1.PostalCode))
                    contact[Attributes.Contact.Address1PostalCode] = address1.PostalCode;
            }
            if (address2 != null)
            {
                if (!string.IsNullOrWhiteSpace(address2.Country))
                    contact[Attributes.Contact.Address2CountryId] = new EntityReference(EntityName.Country, new Guid(address2.Country));
                if (!string.IsNullOrWhiteSpace(address2.AdditionalAddressInfo))
                    contact[Attributes.Contact.Address2AdditionalInformation] = address2.AdditionalAddressInfo;
                if (!string.IsNullOrWhiteSpace(address2.FlatNumberUnit))
                    contact[Attributes.Contact.Address2FlatOrUnitNumber] = address2.FlatNumberUnit;
                if (!string.IsNullOrWhiteSpace(address2.HouseNumberBuilding))
                    contact[Attributes.Contact.Address2HouseNumberOrBuilding] = address2.HouseNumberBuilding;
                if (!string.IsNullOrWhiteSpace(address2.Street))
                    contact[Attributes.Contact.Address2Street] = address2.Street;
                if (!string.IsNullOrWhiteSpace(address2.Town))
                    contact[Attributes.Contact.Address2Town] = address2.Town;
                if (!string.IsNullOrWhiteSpace(address2.County))
                    contact[Attributes.Contact.Address2County] = address2.County;
                if (!string.IsNullOrWhiteSpace(address2.PostalCode))
                    contact[Attributes.Contact.Address2PostalCode] = address2.PostalCode;
            }
            trace.Trace("Contact populate address - end");
        }      

        private static void PopulateEmail(Entity contact, Email[] emails, ITracingService trace,
            OperationType operationType = OperationType.POST, string[] patchList = null)
        {            
            if (emails == null || emails.Length <= 0) return;
            trace.Trace("Contact populate email - start");
            var email1 = emails[0];
            var email2 = emails.Length > 1 ? emails[1] : null;
            var email3 = emails.Length > 2 ? emails[2] : null;
            if (operationType == OperationType.PATCH){
                if (patchList != null && patchList.Length > 0){
                    if (email1 != null){
                        if (Array.Exists(patchList, x => x == Attributes.Contact.EmailAddress1Type)){
                            contact[Attributes.Contact.EmailAddress1Type] = CommonXrm.GetEmailType(email1.EmailType);
                        }
                        if (!string.IsNullOrWhiteSpace(email1.Address))
                            contact[Attributes.Contact.EmailAddress1] = email1.Address;
                    }
                    if (email2 != null){
                        if (Array.Exists(patchList, x => x == Attributes.Account.EmailAddress2Type)){
                            contact[Attributes.Contact.EmailAddress2Type] = CommonXrm.GetEmailType(email2.EmailType);
                        }
                        if (!string.IsNullOrWhiteSpace(email2.Address))
                            contact[Attributes.Contact.EmailAddress2] = email2.Address;
                    }
                    if (email3 != null){
                        if (Array.Exists(patchList, x => x == Attributes.Account.EmailAddress3Type)){
                            contact[Attributes.Contact.EmailAddress3Type] = CommonXrm.GetEmailType(email3.EmailType);
                        }
                        if (!string.IsNullOrWhiteSpace(email3.Address))
                            contact[Attributes.Contact.EmailAddress3] = email3.Address;
                    }
                }
            }
            else{
                if (email1 != null){
                    contact[Attributes.Contact.EmailAddress1Type] = CommonXrm.GetEmailType(email1.EmailType);
                    if (!string.IsNullOrWhiteSpace(email1.Address))
                        contact[Attributes.Contact.EmailAddress1] = email1.Address;
                }
                if (email2 != null){
                    contact[Attributes.Contact.EmailAddress2Type] = CommonXrm.GetEmailType(email2.EmailType);
                    if (!string.IsNullOrWhiteSpace(email2.Address))
                        contact[Attributes.Account.EmailAddress2] = email2.Address;
                }
                if (email3 != null){
                    contact[Attributes.Contact.EmailAddress3Type] = CommonXrm.GetEmailType(email3.EmailType);
                    if (!string.IsNullOrWhiteSpace(email3.Address))
                        contact[Attributes.Contact.EmailAddress3] = email3.Address;
                }
            }
            trace.Trace("Contact populate email - end");
        }      

        private static void PopulatePhone(Entity contact, Phone[] phoneNumbers, ITracingService trace,
            OperationType operationType = OperationType.POST, string[] patchList = null)
        {
            if (phoneNumbers == null || phoneNumbers.Length <= 0) return;
            trace.Trace("Contact populate phone - start");
            var phone1 = phoneNumbers[0];
            var phone2 = phoneNumbers.Length > 1 ? phoneNumbers[1] : null;
            var phone3 = phoneNumbers.Length > 2 ? phoneNumbers[2] : null;
            if (operationType == OperationType.PATCH){
                if (patchList != null && patchList.Length > 0){
                    if (phone1 != null){
                        if (Array.Exists(patchList, x => x == Attributes.Contact.Telephone1Type)){
                            contact[Attributes.Contact.Telephone1Type] = CommonXrm.GetPhoneType(phone1.PhoneType);
                        }
                        if (!string.IsNullOrWhiteSpace(phone1.Number))
                            contact[Attributes.Contact.Telephone1] = phone1.Number;
                    }
                    if (phone2 != null){
                        if (Array.Exists(patchList, x => x == Attributes.Contact.Telephone2Type)){
                            contact[Attributes.Contact.Telephone2Type] = CommonXrm.GetPhoneType(phone2.PhoneType);
                        }
                        if (!string.IsNullOrWhiteSpace(phone2.Number))
                            contact[Attributes.Contact.Telephone2] = phone2.Number;
                    }
                    if (phone3 != null){
                        if (Array.Exists(patchList, x => x == Attributes.Contact.Telephone2Type)){
                            contact[Attributes.Contact.Telephone2Type] = CommonXrm.GetPhoneType(phone2.PhoneType);
                        }
                        if (!string.IsNullOrWhiteSpace(phone3.Number))
                            contact[Attributes.Contact.Telephone3] = phone3.Number;
                    }
                }
            }
            else{
                if (phone1 != null){
                    contact[Attributes.Contact.Telephone1Type] = CommonXrm.GetPhoneType(phone1.PhoneType);
                    if (!string.IsNullOrWhiteSpace(phone1.Number))
                        contact[Attributes.Contact.Telephone1] = phone1.Number;
                }
                if (phone2 != null){
                    contact[Attributes.Contact.Telephone2Type] = CommonXrm.GetPhoneType(phone2.PhoneType);
                    if (!string.IsNullOrWhiteSpace(phone2.Number))
                        contact[Attributes.Contact.Telephone2] = phone2.Number;
                }
                if (phone3 != null){
                    contact[Attributes.Contact.Telephone3Type] = CommonXrm.GetPhoneType(phone3.PhoneType);
                    if (!string.IsNullOrWhiteSpace(phone3.Number))
                        contact[Attributes.Contact.Telephone3] = phone3.Number;
                }
            }
            trace.Trace("Contact populate phone - end");
        }      

        private static void PopulatePermission(Entity contact, Permission permission, ITracingService trace,
            OperationType operationType = OperationType.POST, string[] patchList = null)
        {
            if (permission == null) return;
            trace.Trace("Contact populate permission - start");    
            if(operationType == OperationType.PATCH)
            {
                if (patchList != null)
                {
                    if (Array.Exists(patchList, x => x == Attributes.Contact.SendMarketingByPost)){
                        if (!string.IsNullOrWhiteSpace(permission.MailAllowedInd))
                            contact[Attributes.Contact.SendMarketingByPost] = CommonXrm.GetMarketingByPost(permission.MailAllowedInd);
                    }
                    if (Array.Exists(patchList, x => x == Attributes.Contact.MarketingByPhone)){
                        if (!string.IsNullOrWhiteSpace(permission.PhoneAllowedInd))
                            contact[Attributes.Contact.MarketingByPhone] = CommonXrm.GetMarketingByPhone(permission.PhoneAllowedInd);
                    }
                    if (Array.Exists(patchList, x => x == Attributes.Contact.SendMarketingBySms)){
                        if (!string.IsNullOrWhiteSpace(permission.SmsAllowedInd))
                            contact[Attributes.Contact.SendMarketingBySms] = CommonXrm.GetMarketingBySms(permission.SmsAllowedInd);
                    }
                    if (Array.Exists(patchList, x => x == Attributes.Contact.SendMarketingByEmail)){
                        if (!string.IsNullOrWhiteSpace(permission.EmailAllowedInd))
                            contact[Attributes.Contact.SendMarketingByEmail] = CommonXrm.GetMarketingByEmail(permission.EmailAllowedInd);
                    }
                    if (Array.Exists(patchList, x => x == Attributes.Contact.ThomasCookMarketingConsent)){
                        if (!string.IsNullOrWhiteSpace(permission.DoNotContactInd))
                            contact[Attributes.Contact.ThomasCookMarketingConsent] = CommonXrm.GetMarketingConsent(permission.DoNotContactInd);
                    }
                    if (Array.Exists(patchList, x => x == Attributes.Contact.PreferredContactMethodCode)){
                        if (!string.IsNullOrWhiteSpace(permission.PreferredContactMethod))
                            contact[Attributes.Contact.PreferredContactMethodCode] = CommonXrm.GetPreferredContactMethodCode(permission.PreferredContactMethod);
                    }
                }
            }
            else {  
                if(!string.IsNullOrWhiteSpace(permission.MailAllowedInd))
                    contact[Attributes.Contact.SendMarketingByPost] = CommonXrm.GetMarketingByPost(permission.MailAllowedInd);
                if (!string.IsNullOrWhiteSpace(permission.PhoneAllowedInd))
                    contact[Attributes.Contact.MarketingByPhone] = CommonXrm.GetMarketingByPhone(permission.PhoneAllowedInd);
                if (!string.IsNullOrWhiteSpace(permission.SmsAllowedInd))
                    contact[Attributes.Contact.SendMarketingBySms] = CommonXrm.GetMarketingBySms(permission.SmsAllowedInd);
                if (!string.IsNullOrWhiteSpace(permission.EmailAllowedInd))
                    contact[Attributes.Contact.SendMarketingByEmail] = CommonXrm.GetMarketingByEmail(permission.EmailAllowedInd);            
                if (!string.IsNullOrWhiteSpace(permission.DoNotContactInd))
                    contact[Attributes.Contact.ThomasCookMarketingConsent] = CommonXrm.GetMarketingConsent(permission.DoNotContactInd);
                if (!string.IsNullOrWhiteSpace(permission.PreferredContactMethod))
                    contact[Attributes.Contact.PreferredContactMethodCode] = CommonXrm.GetPreferredContactMethodCode(permission.PreferredContactMethod);
            }
            trace.Trace("Contact populate permission - end");
        }      
    }
}


