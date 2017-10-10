using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessCustomer.Services
{
    public static class AccountPatchHelper
    {
        public static Entity GetAccountEntityForCustomerPayload(Customer customer, ITracingService trace)
        {
            
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null.");
            trace.Trace("Account populate fields - start");
            if (customer == null) throw new InvalidPluginExecutionException("Customer payload is null.");
            if (customer.CustomerIdentifier == null)
                throw new InvalidPluginExecutionException("Customer Identifier could not be retrieved from payload.");
            if (string.IsNullOrWhiteSpace(customer.CustomerIdentifier.CustomerId))
                throw new InvalidPluginExecutionException("Customer Identifier could not be retrieved from payload.");
            if (customer.PatchParameters == null || customer.PatchParameters.Count <= 0)
                throw new InvalidPluginExecutionException("Customer Patch parameter is null.");
            Entity account = new Entity(EntityName.Account);
            var fieldService = new FieldService(account, customer.PatchParameters);
            
            
            fieldService.PopulateField(Attributes.Account.Name, customer.Company.CompanyName);
            var sourceMarket = (!string.IsNullOrWhiteSpace(customer.CustomerIdentifier.SourceMarket))
                               ? new EntityReference(EntityName.Country,
                               new Guid(customer.CustomerIdentifier.SourceMarket)) : null;
            fieldService.PopulateField(Attributes.Account.SourceMarketId, sourceMarket);
            if ((customer.Email != null) || (customer.Email != null & customer.Email.Length > 0))
                PopulateEmail(customer.Email, trace, fieldService);
            if ((customer.Phone != null) || (customer.Phone != null & customer.Phone.Length > 0))
                PopulatePhone( customer.Phone, trace, fieldService);
            if ((customer.Address != null) || (customer.Address != null & customer.Address.Length > 0))
                PopulateAddress(customer.Address, trace, fieldService);
            trace.Trace("Account populate fields - end");
            return account;
        }
       
        #region Email
        private static void PopulateEmail(Email[] emailList, ITracingService trace,FieldService fieldService){
            if (fieldService == null) return;
            trace.Trace("Account populate email - start");
            var email1 = emailList[0];
            var email2 = emailList.Length > 1 ? emailList[1] : null;
            var email3 = emailList.Length > 2 ? emailList[2] : null;
            if (email1 != null){
                trace.Trace(email1.EmailType.ToString());
                trace.Trace(CommonXrm.GetEmailType(email1.EmailType).Value.ToString());
                fieldService.PopulateField(Attributes.Account.EmailAddress1Type, CommonXrm.GetEmailType(email1.EmailType));
                fieldService.PopulateField(Attributes.Account.EmailAddress1, email1.Address);
            }
            if (email2 != null){
                trace.Trace(email2.EmailType.ToString());
                trace.Trace(CommonXrm.GetEmailType(email2.EmailType).Value.ToString());
                fieldService.PopulateField(Attributes.Account.EmailAddress2Type, CommonXrm.GetEmailType(email2.EmailType));
                fieldService.PopulateField(Attributes.Account.EmailAddress2, email2.Address);
            }
            if (email3 != null){
                trace.Trace(email3.EmailType.ToString());
                trace.Trace(CommonXrm.GetEmailType(email3.EmailType).Value.ToString());
                fieldService.PopulateField(Attributes.Account.EmailAddress3Type, CommonXrm.GetEmailType(email3.EmailType));
                fieldService.PopulateField(Attributes.Account.EmailAddress3, email3.Address);
            }
            trace.Trace("Account populate email - end");
        }
        #endregion

        #region Phone
        private static void PopulatePhone(Phone[] phoneList,ITracingService trace,FieldService fieldService){
            if (phoneList == null || phoneList.Length <= 0) return;
            if (fieldService == null) return;
            trace.Trace("Account populate phone - start");
            var phone1 = phoneList[0];
            var phone2 = phoneList.Length > 1 ? phoneList[1] : null;
            var phone3 = phoneList.Length > 2 ? phoneList[2] : null;
            if (phone1 != null){
                fieldService.PopulateField(Attributes.Account.Telephone1Type, CommonXrm.GetPhoneType(phone1.PhoneType));
                fieldService.PopulateField(Attributes.Account.Telephone1, phone1.Number);
            }
            if (phone2 != null){
                fieldService.PopulateField(Attributes.Account.Telephone2Type, CommonXrm.GetPhoneType(phone2.PhoneType));
                fieldService.PopulateField(Attributes.Account.Telephone2, phone2.Number);
            }
            if (phone3 != null){
                fieldService.PopulateField(Attributes.Account.Telephone3Type, CommonXrm.GetPhoneType(phone3.PhoneType));
                fieldService.PopulateField(Attributes.Account.Telephone3, phone3.Number);
            }
            trace.Trace("Account populate phone - end");
        }
        #endregion

        #region Address
        private static void PopulateAddress(Address[] addresses,ITracingService trace, FieldService fieldService){
            if (addresses == null || addresses.Length <= 0) return;
            if (fieldService == null) return;
            trace.Trace("Account populate address - start");
            if (addresses[0] == null) return;
            var address = addresses[0];
            fieldService.PopulateField(Attributes.Account.Address1AdditionalInformation, address.AdditionalAddressInfo);
            fieldService.PopulateField(Attributes.Account.Address1FlatOrUnitNumber, address.FlatNumberUnit);
            fieldService.PopulateField(Attributes.Account.Address1HouseNumberOrBuilding, address.HouseNumberBuilding);
            fieldService.PopulateField(Attributes.Account.Address1Town, address.Town);
            fieldService.PopulateField(Attributes.Account.Address1Street, address.Street);
            fieldService.PopulateField(Attributes.Account.Address1PostalCode, address.PostalCode);
            fieldService.PopulateField(Attributes.Account.Address1County, address.County);
            var country = (!string.IsNullOrWhiteSpace(address.Country))
                           ? new EntityReference(EntityName.Country,new Guid(address.Country)) : null;
            fieldService.PopulateField(Attributes.Account.Address1CountryId, country);
            trace.Trace("Account populate address - end");
        }
        #endregion
    }
}
