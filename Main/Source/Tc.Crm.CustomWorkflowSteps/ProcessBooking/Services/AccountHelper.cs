using Microsoft.Xrm.Sdk;
using System;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services
{
    public static class AccountHelper
    {
        public static void PopulateEmail(Entity account, Email[] emailList, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Account populate email - start");
            if (account == null) throw new InvalidPluginExecutionException("Account entity is null;");

            if (emailList == null || emailList.Length == 0) { ClearEmailList(account); return; }

            var email1 = emailList[0];
            var email2 = emailList.Length > 1 ? emailList[1] : ClearEmail2(account);
            var email3 = emailList.Length > 2 ? emailList[2] : ClearEmail3(account);

            trace.Trace("email 1");
            if (email1 == null) return;

            account[Attributes.Account.EmailAddress1] = (!string.IsNullOrWhiteSpace(email1.Address)) ? email1.Address : string.Empty;
            account[Attributes.Account.EmailAddress1Type] = CommonXrm.GetEmailType(email1.EmailType);

            trace.Trace("email 2");
            if (email2 == null) return;

            account[Attributes.Account.EmailAddress2] = (!string.IsNullOrWhiteSpace(email2.Address)) ? email2.Address : string.Empty;
            account[Attributes.Account.EmailAddress2Type] = CommonXrm.GetEmailType(email2.EmailType);

            trace.Trace("email 3");
            if (email3 == null) return;

            account[Attributes.Account.EmailAddress3] = (!string.IsNullOrWhiteSpace(email3.Address)) ? email3.Address : string.Empty;
            account[Attributes.Account.EmailAddress3Type] = CommonXrm.GetEmailType(email3.EmailType);

            trace.Trace("Account populate email - end");

        }

        private static Email ClearEmailList(Entity account)
        {
            ClearEmail1(account);
            ClearEmail2(account);
            ClearEmail3(account);
            return null;
        }

        private static Email ClearEmail1(Entity account)
        {
            account[Attributes.Account.EmailAddress1] = string.Empty;
            account[Attributes.Account.EmailAddress1Type] = null;
            return null;
        }

        private static Email ClearEmail2(Entity account)
        {
            account[Attributes.Account.EmailAddress2] = string.Empty;
            account[Attributes.Account.EmailAddress2Type] = null;
            return null;
        }

        private static Email ClearEmail3(Entity account)
        {
            account[Attributes.Account.EmailAddress3] = string.Empty;
            account[Attributes.Account.EmailAddress3Type] = null;
            return null;
        }

        public static void PopulatePhone(Entity account, Phone[] phoneList, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Account populate phone - start");
            if (account == null) throw new InvalidPluginExecutionException("Account entity is null;");

            if (phoneList == null || phoneList.Length == 0)
            {
                ClearPhoneList(account);
                return;
            }

            var phone1 = phoneList[0];
            var phone2 = phoneList.Length > 1 ? phoneList[1] : ClearPhone2(account);
            var phone3 = phoneList.Length > 2 ? phoneList[2] : ClearPhone3(account);

            trace.Trace("phone 1");
            if (phone1 == null) return;

            account[Attributes.Account.Telephone1Type] = CommonXrm.GetPhoneType(phone1.PhoneType);
            account[Attributes.Account.Telephone1] = (!string.IsNullOrWhiteSpace(phone1.Number)) ? phone1.Number : string.Empty;

            trace.Trace("phone 2");
            if (phone2 == null) return;

            account[Attributes.Account.Telephone2Type] = CommonXrm.GetPhoneType(phone2.PhoneType);
            account[Attributes.Account.Telephone2] = (!string.IsNullOrWhiteSpace(phone2.Number)) ? phone2.Number : string.Empty;

            trace.Trace("phone 3");
            if (phone3 == null) return;

            account[Attributes.Account.Telephone3Type] = CommonXrm.GetPhoneType(phone2.PhoneType);
            account[Attributes.Account.Telephone3] = (!string.IsNullOrWhiteSpace(phone3.Number)) ? phone3.Number : string.Empty;

            trace.Trace("Account populate phone - end");
        }

        private static void ClearPhoneList(Entity account)
        {
            ClearPhone1(account);
            ClearPhone2(account);
            ClearPhone3(account);
        }

        private static Phone ClearPhone1(Entity account)
        {
            account[Attributes.Account.Telephone1Type] = null;
            account[Attributes.Account.Telephone1] = string.Empty;
            return null;
        }

        private static Phone ClearPhone2(Entity account)
        {
            account[Attributes.Account.Telephone2Type] = null;
            account[Attributes.Account.Telephone2] = string.Empty;
            return null;
        }

        private static Phone ClearPhone3(Entity account)
        {
            account[Attributes.Account.Telephone3Type] = null;
            account[Attributes.Account.Telephone3] = string.Empty;
            return null;
        }

        public static void PopulateAddress(Entity account, Address[] addresses, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null.");
            trace.Trace("Account populate address - start");
            if (account == null) throw new InvalidPluginExecutionException("Account entity is null.");

            var address = (addresses != null && addresses.Length > 0) ? addresses[0] : ClearAddress(account);

            if (address == null) return;
            account[Attributes.Account.Address1AdditionalInformation] = (!string.IsNullOrWhiteSpace(address.AdditionalAddressInfo)) ? address.AdditionalAddressInfo : string.Empty;
            account[Attributes.Account.Address1FlatOrUnitNumber] = (!string.IsNullOrWhiteSpace(address.FlatNumberUnit)) ? address.FlatNumberUnit : string.Empty;
            account[Attributes.Account.Address1HouseNumberOrBuilding] = (!string.IsNullOrWhiteSpace(address.HouseNumberBuilding)) ? address.HouseNumberBuilding : string.Empty;
            account[Attributes.Account.Address1Town] = (!string.IsNullOrWhiteSpace(address.Town)) ? address.Town : string.Empty;
            account[Attributes.Account.Address1PostalCode] = (!string.IsNullOrWhiteSpace(address.PostalCode)) ? address.PostalCode : string.Empty;
            account[Attributes.Account.Address1CountryId] = (!string.IsNullOrWhiteSpace(address.Country)) ? new EntityReference(EntityName.Country
                                                                                                           , new Guid(address.Country))
                                                                                                           : null;
            account[Attributes.Account.Address1County] = (!string.IsNullOrWhiteSpace(address.County)) ? address.County : string.Empty;
            account[Attributes.Account.Address1Street] = (!string.IsNullOrWhiteSpace(address.Street)) ? address.Street : string.Empty;
            trace.Trace("Account populate address - end");

        }

        private static Address ClearAddress(Entity account)
        {
            account[Attributes.Account.Address1AdditionalInformation] = string.Empty;
            account[Attributes.Account.Address1FlatOrUnitNumber] = string.Empty;
            account[Attributes.Account.Address1HouseNumberOrBuilding] = string.Empty;
            account[Attributes.Account.Address1Town] = string.Empty;
            account[Attributes.Account.Address1PostalCode] = string.Empty;
            account[Attributes.Account.Address1CountryId] = null;
            account[Attributes.Account.Address1County] = string.Empty;
            return null;
        }

        public static Entity GetAccountEntityForBookingPayload(Customer customer, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null.");
            trace.Trace("Account populate fields - start");
            if (customer == null) throw new InvalidPluginExecutionException("Customer payload is null.");
            if (customer.CustomerIdentifier == null)
                throw new InvalidPluginExecutionException("Customer Identifier could not be retrieved from payload.");

            Entity account = !string.IsNullOrWhiteSpace(customer.CustomerIdentifier.CustomerId) ? new Entity(EntityName.Account
                                              , Attributes.Account.SourceSystemId
                                              , customer.CustomerIdentifier.CustomerId) : new Entity(EntityName.Account);

            if (customer.Company != null && !string.IsNullOrWhiteSpace(customer.Company.CompanyName))
                account[Attributes.Account.Name] = customer.Company.CompanyName;

            PopulateAddress(account, customer.Address, trace);
            PopulatePhone(account, customer.Phone, trace);
            PopulateEmail(account, customer.Email, trace);


            account[Attributes.Account.SourceMarketId] = (!string.IsNullOrWhiteSpace(customer.CustomerIdentifier.SourceMarket)) ?
                                                                new EntityReference(EntityName.Country,new Guid(customer.CustomerIdentifier.SourceMarket)) :
                                                                null;

            account[Attributes.Account.SourceSystemId] = (!string.IsNullOrWhiteSpace(customer.CustomerIdentifier.CustomerId)) ? customer.CustomerIdentifier.CustomerId : string.Empty;
            if (customer.CustomerIdentifier != null)
            {
                if (!string.IsNullOrWhiteSpace(customer.Owner))
                    account[Attributes.Booking.Owner] = new EntityReference(EntityName.Team, new Guid(customer.Owner));
            }
            trace.Trace("Account populate fields - end");
            return account;
        }
    }
}
