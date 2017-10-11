using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessCustomer.Services
{
    public static class AccountHelper
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
            Entity account = new Entity(EntityName.Account);
            if (customer.Company != null && !string.IsNullOrWhiteSpace(customer.Company.CompanyName))
                account[Attributes.Account.Name] = customer.Company.CompanyName;
            var sourceMarket = (!string.IsNullOrWhiteSpace(customer.CustomerIdentifier.SourceMarket))
                               ? new EntityReference(EntityName.Country,
                               new Guid(customer.CustomerIdentifier.SourceMarket)) : null;
            account[Attributes.Account.SourceMarketId] = sourceMarket;
            var sourceSystemID = (!string.IsNullOrWhiteSpace(customer.CustomerIdentifier.CustomerId))? 
                                    customer.CustomerIdentifier.CustomerId : string.Empty;
            account[Attributes.Account.SourceSystemId] = sourceSystemID; 
            account[Attributes.Account.DuplicateSourceSystemId] = sourceSystemID; 
            if ((customer.Email != null) || (customer.Email != null & customer.Email.Length > 0))
                PopulateEmail(account, customer.Email, trace);
            if ((customer.Phone != null) || (customer.Phone != null & customer.Phone.Length > 0))
                PopulatePhone(account, customer.Phone, trace);
            if ((customer.Address != null) || (customer.Address != null & customer.Address.Length > 0))
                PopulateAddress(account, customer.Address, trace);
            trace.Trace("Account populate fields - end");
            return account;
        }

        #region Email
        private static void PopulateEmail(Entity account, Email[] emailList, ITracingService trace)
        {
            if (emailList == null || emailList.Length <= 0) return;
            trace.Trace("Account populate email - start");
            var email1 = emailList[0];
            var email2 = emailList.Length > 1 ? emailList[1] : null;
            var email3 = emailList.Length > 2 ? emailList[2] : null;
            if (email1 != null){
                account[Attributes.Account.EmailAddress1Type] = CommonXrm.GetEmailType(email1.EmailType);
                account[Attributes.Account.EmailAddress1] = (!string.IsNullOrWhiteSpace(email1.Address)) ? email1.Address : string.Empty; 
            }
            if (email2 != null){
                account[Attributes.Account.EmailAddress2Type] = CommonXrm.GetEmailType(email2.EmailType);
                account[Attributes.Account.EmailAddress2] = (!string.IsNullOrWhiteSpace(email2.Address)) ? email2.Address : string.Empty; 
            }
            if (email3 != null){
                account[Attributes.Account.EmailAddress3Type] = CommonXrm.GetEmailType(email3.EmailType);
                account[Attributes.Account.EmailAddress3] = (!string.IsNullOrWhiteSpace(email3.Address)) ? email3.Address : string.Empty;
            }
             
            trace.Trace("Account populate email - end");
        }
        #endregion

        #region Phone
        private static void PopulatePhone(Entity account, Phone[] phoneList, ITracingService trace){
            if (phoneList == null || phoneList.Length <= 0) return;
            trace.Trace("Account populate phone - start");
            var phone1 = phoneList[0];
            var phone2 = phoneList.Length > 1 ? phoneList[1] : null;
            var phone3 = phoneList.Length > 2 ? phoneList[2] : null; 
            if (phone1 != null){
                account[Attributes.Account.Telephone1Type] = CommonXrm.GetPhoneType(phone1.PhoneType);
                account[Attributes.Account.Telephone1] = (!string.IsNullOrWhiteSpace(phone1.Number)) ? phone1.Number : string.Empty;  
            }
            if (phone2 != null){
                account[Attributes.Account.Telephone2Type] = CommonXrm.GetPhoneType(phone2.PhoneType);
                account[Attributes.Account.Telephone2] = (!string.IsNullOrWhiteSpace(phone2.Number)) ? phone2.Number : string.Empty;
            }
            if (phone3 != null) {
                account[Attributes.Account.Telephone3Type] = CommonXrm.GetPhoneType(phone3.PhoneType);
                account[Attributes.Account.Telephone3] = (!string.IsNullOrWhiteSpace(phone3.Number)) ? phone3.Number : string.Empty;
            }
            trace.Trace("Account populate phone - end");
        }
        #endregion

        #region Address
        private static void PopulateAddress(Entity account, Address[] addresses, ITracingService trace){
            if (addresses == null || addresses.Length <= 0) return;
            trace.Trace("Account populate address - start");
            if (addresses[0] == null) return;
            var address = addresses[0];
            account[Attributes.Account.Address1AdditionalInformation] = 
                (!string.IsNullOrWhiteSpace(address.AdditionalAddressInfo)) ? 
                address.AdditionalAddressInfo:string.Empty;
            account[Attributes.Account.Address1FlatOrUnitNumber] = (!string.IsNullOrWhiteSpace(address.FlatNumberUnit)) ?
                address.FlatNumberUnit : string.Empty; 
            account[Attributes.Account.Address1HouseNumberOrBuilding] = (!string.IsNullOrWhiteSpace(address.HouseNumberBuilding)) ?
                address.HouseNumberBuilding : string.Empty;
            account[Attributes.Account.Address1Town] = (!string.IsNullOrWhiteSpace(address.Town)) ?
                address.Town : string.Empty;  
            account[Attributes.Account.Address1Street] = (!string.IsNullOrWhiteSpace(address.Street)) ?
                address.Street : string.Empty;  
            account[Attributes.Account.Address1PostalCode] = (!string.IsNullOrWhiteSpace(address.PostalCode)) ?
                address.PostalCode : string.Empty;  
            account[Attributes.Account.Address1County] = (!string.IsNullOrWhiteSpace(address.County)) ?
                address.County : string.Empty;  
            var country = !string.IsNullOrWhiteSpace(address.Country) ? new EntityReference(EntityName.Country,
                                                               new Guid(address.Country)) : null;
            account[Attributes.Account.Address1CountryId] = country;
            trace.Trace("Account populate address - end");
        }
        #endregion
    }
}
