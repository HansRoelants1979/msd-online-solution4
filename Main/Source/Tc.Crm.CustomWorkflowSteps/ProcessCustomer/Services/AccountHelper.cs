using Microsoft.Xrm.Sdk;
using System;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessCustomer.Services
{
    public static class AccountHelper
    {
        public static Entity GetAccountEntityForCustomerPayload(Customer customer, ITracingService trace,
            OperationType operationType = OperationType.POST)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null.");
            trace.Trace("Account populate fields - start");
            if (customer == null) throw new InvalidPluginExecutionException("Customer payload is null.");
            if (customer.CustomerIdentifier == null)
                throw new InvalidPluginExecutionException("Customer Identifier could not be retrieved from payload.");
            Entity account = new Entity(EntityName.Account);
            if (customer.Company != null && !string.IsNullOrWhiteSpace(customer.Company.CompanyName))
                account[Attributes.Account.Name] = customer.Company.CompanyName;
            if ((!string.IsNullOrWhiteSpace(customer.CustomerIdentifier.SourceMarket))){
                account[Attributes.Account.SourceMarketId] =
                    new EntityReference(EntityName.Country, new Guid(customer.CustomerIdentifier.SourceMarket));
            }
            if (operationType == OperationType.POST){
                account[Attributes.Account.SourceSystemId] = 
                        (!string.IsNullOrWhiteSpace(customer.CustomerIdentifier.CustomerId)) ?
                        customer.CustomerIdentifier.CustomerId : string.Empty;
                account[Attributes.Account.DuplicateSourceSystemId] = 
                        (!string.IsNullOrWhiteSpace(customer.CustomerIdentifier.CustomerId)) ?
                        customer.CustomerIdentifier.CustomerId : string.Empty;
            }
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
            if (email1 != null)
            {
                if (email1.EmailType != EmailType.NotSpecified)
                    account[Attributes.Account.EmailAddress1Type] = CommonXrm.GetEmailType(email1.EmailType);
                if (!string.IsNullOrWhiteSpace(email1.Address))
                    account[Attributes.Account.EmailAddress1] = email1.Address;
            }
            if (email2 != null)
            {
                if (email2.EmailType != EmailType.NotSpecified)
                    account[Attributes.Account.EmailAddress2Type] = CommonXrm.GetEmailType(email2.EmailType);
                if (!string.IsNullOrWhiteSpace(email2.Address))
                    account[Attributes.Account.EmailAddress2] = email2.Address;
            }
            if (email3 != null)
            {
                if (email3.EmailType != EmailType.NotSpecified)
                    account[Attributes.Account.EmailAddress3Type] = CommonXrm.GetEmailType(email3.EmailType);
                if (!string.IsNullOrWhiteSpace(email3.Address))
                    account[Attributes.Account.EmailAddress3] = email3.Address;
            }
            trace.Trace("Account populate email - end");
        }
        #endregion

        #region Phone
        private static void PopulatePhone(Entity account, Phone[] phoneList, ITracingService trace)
        {
            if (phoneList == null || phoneList.Length <= 0) return;
            trace.Trace("Account populate phone - start");
            var phone1 = phoneList[0];
            var phone2 = phoneList.Length > 1 ? phoneList[1] : null;
            var phone3 = phoneList.Length > 2 ? phoneList[2] : null;
            if (phone1 != null)
            {
                if (phone1.PhoneType != PhoneType.NotSpecified)
                    account[Attributes.Account.Telephone1Type] = CommonXrm.GetPhoneType(phone1.PhoneType);
                if (!string.IsNullOrWhiteSpace(phone1.Number))
                    account[Attributes.Account.Telephone1] = phone1.Number;
            }
            if (phone2 != null)
            {
                if (phone2.PhoneType != PhoneType.NotSpecified)
                    account[Attributes.Account.Telephone2Type] = CommonXrm.GetPhoneType(phone2.PhoneType);
                if (!string.IsNullOrWhiteSpace(phone2.Number))
                    account[Attributes.Account.Telephone2] = phone2.Number;
            }
            if (phone3 != null)
            {
                if (phone3.PhoneType != PhoneType.NotSpecified)
                    account[Attributes.Account.Telephone3Type] = CommonXrm.GetPhoneType(phone3.PhoneType);
                if (!string.IsNullOrWhiteSpace(phone3.Number))
                    account[Attributes.Account.Telephone3] = phone3.Number;
            }
            trace.Trace("Account populate phone - end");
        }
        #endregion

        #region Address
        private static void PopulateAddress(Entity account, Address[] addresses, ITracingService trace)
        {
            if (addresses == null || addresses.Length <= 0) return;
            trace.Trace("Account populate address - start");
            if (addresses[0] == null) return;
            var address = addresses[0];
            if (!string.IsNullOrWhiteSpace(address.AdditionalAddressInfo))
                account[Attributes.Account.Address1AdditionalInformation] = address.AdditionalAddressInfo;
            if (!string.IsNullOrWhiteSpace(address.FlatNumberUnit))
                account[Attributes.Account.Address1FlatOrUnitNumber] = address.FlatNumberUnit;
            if (!string.IsNullOrWhiteSpace(address.HouseNumberBuilding))
                account[Attributes.Account.Address1HouseNumberOrBuilding] = address.HouseNumberBuilding;
            if (!string.IsNullOrWhiteSpace(address.Town))
                account[Attributes.Account.Address1Town] = address.Town;
            if (!string.IsNullOrWhiteSpace(address.Street))
                account[Attributes.Account.Address1Street] = address.Street;
            if (!string.IsNullOrWhiteSpace(address.PostalCode))
                account[Attributes.Account.Address1PostalCode] = address.PostalCode;
            if (!string.IsNullOrWhiteSpace(address.County))
            {
                account[Attributes.Account.Address1County] = address.County;
                account[Attributes.Account.Address1CountryId] = new EntityReference(EntityName.Country,
                                                                new Guid(address.Country));
            }
            trace.Trace("Account populate address - end");
        }
        #endregion
    }
}
