using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessCustomer.Services
{
    public static class ContactPatchHelper
    {
        public static Entity GetContactEntityForCustomerPayload(Customer customer,ITracingService trace){
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Contact populate fields - start");
            if (customer == null) throw new InvalidPluginExecutionException("Customer payload is null.");
            if (customer.CustomerIdentifier == null)
                throw new InvalidPluginExecutionException("Customer Identifier could not be retrieved from payload.");
            if (string.IsNullOrWhiteSpace(customer.CustomerIdentifier.CustomerId))
                throw new InvalidPluginExecutionException("Customer Identifier could not be retrieved from payload.");
            Entity contact = new Entity(EntityName.Contact);
            var fieldService = new FieldService(contact, customer.PatchParameters);
            var sourceMarket = (!string.IsNullOrWhiteSpace(customer.CustomerIdentifier.SourceMarket))
                                              ? new EntityReference(EntityName.Country,
                                              new Guid(customer.CustomerIdentifier.SourceMarket)) : null;

            fieldService.PopulateField(Attributes.Contact.SourceMarketId, sourceMarket);
            PopulateIdentityInformation(customer.CustomerIdentity, trace,fieldService);
            PopulateAddress(customer.Address, trace, fieldService);
            PopulateEmail(customer.Email, trace, fieldService);
            PopulatePhone(customer.Phone, trace, fieldService);
            PopulatePermission(customer.Permissions, trace, fieldService);
            if (customer.CustomerGeneral != null){
                contact[Attributes.Contact.StatusCode] = CommonXrm.GetCustomerStatus(customer.CustomerGeneral.CustomerStatus);
            }
            if (customer.Additional != null){
                trace.Trace("Contact populate Additional details - start");
                fieldService.PopulateField(Attributes.Contact.Segment, CommonXrm.GetSegment(customer.Additional.Segment));
                DateTime? dateOfDeath = null;
                if (!string.IsNullOrWhiteSpace(customer.Additional.DateOfDeath))
                    dateOfDeath = Convert.ToDateTime(customer.Additional.DateOfDeath);
                fieldService.PopulateField(Attributes.Contact.DateOfDeath,dateOfDeath);
                trace.Trace("Contact populate Additional details - end");
            }
            trace.Trace("Contact populate fields - end");
            return contact;
        }

        #region Identity Information
        private static void PopulateIdentityInformation(CustomerIdentity identity,
            ITracingService trace, FieldService fieldService){
            trace.Trace("Contact populate identity - start");
            if (fieldService == null) return;
            if (identity == null) return;
            fieldService.PopulateField(Attributes.Contact.FirstName, identity.FirstName);
            fieldService.PopulateField(Attributes.Contact.LastName, identity.LastName);
            fieldService.PopulateField(Attributes.Contact.MiddleName, identity.MiddleName);
            fieldService.PopulateField(Attributes.Contact.AcademicTitle, identity.AcademicTitle);
            fieldService.PopulateField(Attributes.Contact.Language, CommonXrm.GetLanguage(identity.Language));
            var salutation = CommonXrm.GetSalutation(identity.Salutation);
            if(salutation!=null)
                fieldService.PopulateField(Attributes.Contact.Salutation, CommonXrm.GetSalutation(identity.Salutation));
            fieldService.PopulateField(Attributes.Contact.Gender, CommonXrm.GetGender(identity.Gender));
            DateTime? dateOfBirth = null;
            if (!string.IsNullOrWhiteSpace(identity.Birthdate))
                dateOfBirth = Convert.ToDateTime(identity.Birthdate);
            fieldService.PopulateField(Attributes.Contact.Birthdate, dateOfBirth);
            trace.Trace("Contact populate identity - end");
        }
        #endregion

        #region Address
        private static void PopulateAddress(Address[] addresses, 
            ITracingService trace, FieldService fieldService){
            trace.Trace("Contact populate address - start");
            if (addresses == null || addresses.Length <= 0) return;
            if (fieldService == null) return;
            var address1 = addresses[0];
            var address2 = addresses.Length > 1 ? addresses[1] : null;
            if (address1 != null){
                fieldService.PopulateField(Attributes.Contact.Address1AdditionalInformation, address1.AdditionalAddressInfo);
                fieldService.PopulateField(Attributes.Contact.Address1FlatOrUnitNumber, address1.FlatNumberUnit);
                fieldService.PopulateField(Attributes.Contact.Address1HouseNumberOrBuilding, address1.HouseNumberBuilding);
                fieldService.PopulateField(Attributes.Contact.Address1Town, address1.Town);
                fieldService.PopulateField(Attributes.Contact.Address1Street, address1.Street);
                fieldService.PopulateField(Attributes.Contact.Address1PostalCode, address1.PostalCode);
                fieldService.PopulateField(Attributes.Contact.Address1County, address1.County);
                var country = (!string.IsNullOrWhiteSpace(address1.Country))
                                              ? new EntityReference(EntityName.Country,
                                              new Guid(address1.Country)) : null;
                fieldService.PopulateField(Attributes.Contact.Address1CountryId, country);
            }
            if (address2 != null){
                fieldService.PopulateField(Attributes.Contact.Address2AdditionalInformation, address2.AdditionalAddressInfo);
                fieldService.PopulateField(Attributes.Contact.Address2FlatOrUnitNumber, address2.FlatNumberUnit);
                fieldService.PopulateField(Attributes.Contact.Address2HouseNumberOrBuilding, address2.HouseNumberBuilding);
                fieldService.PopulateField(Attributes.Contact.Address2Town, address2.Town);
                fieldService.PopulateField(Attributes.Contact.Address2Street, address2.Street);
                fieldService.PopulateField(Attributes.Contact.Address2PostalCode, address2.PostalCode);
                fieldService.PopulateField(Attributes.Contact.Address2County, address2.County);
                fieldService.PopulateField(Attributes.Contact.Address2CountryId,
                   (!string.IsNullOrWhiteSpace(address2.Country))
                                             ? new EntityReference(EntityName.Country,
                                             new Guid(address2.Country)) : null);
            }
            trace.Trace("Contact populate address - end");
        }
        #endregion

        #region Email
        private static void PopulateEmail(Email[] emails, ITracingService trace,
            FieldService fieldService){            
            if (emails == null || emails.Length <= 0) return;
            if (fieldService == null) return;
            trace.Trace("Contact populate email - start");
            var email1 = emails[0];
            var email2 = emails.Length > 1 ? emails[1] : null;
            var email3 = emails.Length > 2 ? emails[2] : null;

            if (email1 != null){
                fieldService.PopulateField(Attributes.Contact.EmailAddress1Type, CommonXrm.GetEmailType(email1.EmailType));
                fieldService.PopulateField(Attributes.Contact.EmailAddress1, email1.Address);
            }
            if (email2 != null){
                fieldService.PopulateField(Attributes.Contact.EmailAddress2Type, CommonXrm.GetEmailType(email2.EmailType));
                fieldService.PopulateField(Attributes.Contact.EmailAddress2, email2.Address);
            }
            if (email3 != null){
                fieldService.PopulateField(Attributes.Contact.EmailAddress3Type, CommonXrm.GetEmailType(email3.EmailType));
                fieldService.PopulateField(Attributes.Contact.EmailAddress3, email3.Address);
            } 
            trace.Trace("Contact populate email - end");
        }
        #endregion

        #region Phone
        private static void PopulatePhone(Phone[] phoneList, ITracingService trace, FieldService fieldService)
        {
            if (phoneList == null || phoneList.Length <= 0) return;
            if (fieldService == null) return;
            trace.Trace("Contact populate phone - start");
            var phone1 = phoneList[0];
            var phone2 = phoneList.Length > 1 ? phoneList[1] : null;
            var phone3 = phoneList.Length > 2 ? phoneList[2] : null;
            if (phone1 != null){
                fieldService.PopulateField(Attributes.Contact.Telephone1Type, CommonXrm.GetPhoneType(phone1.PhoneType));
                fieldService.PopulateField(Attributes.Contact.Telephone1, phone1.Number);
            }
            if (phone2 != null){
                fieldService.PopulateField(Attributes.Contact.Telephone2Type, CommonXrm.GetPhoneType(phone2.PhoneType));
                fieldService.PopulateField(Attributes.Contact.Telephone2, phone2.Number);
            }
            if (phone3 != null){
                fieldService.PopulateField(Attributes.Contact.Telephone3Type, CommonXrm.GetPhoneType(phone3.PhoneType));
                fieldService.PopulateField(Attributes.Contact.Telephone3, phone3.Number);
            }
            trace.Trace("Contact populate phone - end");
        }
        #endregion

        #region Permissions
        private static void PopulatePermission(Permissions permissions, 
            ITracingService trace,FieldService fieldService){
            trace.Trace("Contact populate permission - start");
            if (permissions == null) return;
            if (fieldService == null) return;        

            fieldService.PopulateField(Attributes.Contact.SendMarketingByPost, 
                CommonXrm.GetMarketingByPost(permissions.DoNotAllowMail));
            fieldService.PopulateField(Attributes.Contact.MarketingByPhone,
                CommonXrm.GetMarketingByPhone(permissions.DoNotAllowPhoneCalls));
            fieldService.PopulateField(Attributes.Contact.SendMarketingBySms, 
                CommonXrm.GetMarketingBySms(permissions.DoNotAllowSms));
            fieldService.PopulateField(Attributes.Contact.SendMarketingByEmail, 
                CommonXrm.GetMarketingByEmail(permissions.DoNotAllowEmail));
            fieldService.PopulateField(Attributes.Contact.ThomasCookMarketingConsent, 
                CommonXrm.GetMarketingConsent(permissions.AllowMarketing));
            trace.Trace("Contact populate permission - end");
        }

        #endregion

    }
}


