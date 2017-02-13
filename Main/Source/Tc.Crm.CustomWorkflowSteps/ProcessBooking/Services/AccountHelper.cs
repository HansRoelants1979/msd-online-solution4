using Microsoft.Xrm.Sdk;
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

            if (emailList == null || emailList.Length == 0) return;

            var email1 = emailList[0];
            var email2 = emailList.Length > 1 ? emailList[1] : null;
            var email3 = emailList.Length > 2 ? emailList[2] : null;

            trace.Trace("email 1");
            if (email1 == null) return;

            account[Attributes.Account.EMailAddress1] = email1.Address;
            account[Attributes.Account.EmailAddress1_Type] = CommonXrm.GetOptionSetValue(email1.EmailType.ToString(), Attributes.Account.EmailAddress1_Type);

            trace.Trace("email 2");
            if (email2 == null) return;

            account[Attributes.Account.EMailAddress2] = email2.Address;
            account[Attributes.Account.EmailAddress2_Type] = CommonXrm.GetOptionSetValue(email2.EmailType.ToString(), Attributes.Account.EmailAddress2_Type);

            trace.Trace("email 3");
            if (email3 == null) return;

            account[Attributes.Account.EMailAddress3] = email3.Address;
            account[Attributes.Account.EmailAddress3_Type] = CommonXrm.GetOptionSetValue(email3.EmailType.ToString(), Attributes.Account.EmailAddress3_Type);

            trace.Trace("Account populate email - end");

        }
        public static void PopulatePhone(Entity account, Phone[] phoneList, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Account populate phone - start");
            if (account == null) throw new InvalidPluginExecutionException("Account entity is null;");

            if (phoneList == null || phoneList.Length == 0)
                return;

            var phone1 = phoneList[0];
            var phone2 = phoneList.Length > 1 ? phoneList[1] : null;
            var phone3 = phoneList.Length > 2 ? phoneList[2] : null;

            trace.Trace("phone 1");
            if (phone1 == null) return;

            account[Attributes.Account.Telephone1_Type] = CommonXrm.GetOptionSetValue(phone1.PhoneType.ToString(), Attributes.Account.Telephone1_Type);
            account[Attributes.Account.Telephone1] = phone1.Number;

            trace.Trace("phone 2");
            if (phone2 == null) return;

            account[Attributes.Account.Telephone2_Type] = CommonXrm.GetOptionSetValue(phone2.PhoneType.ToString(), Attributes.Account.Telephone2_Type);
            account[Attributes.Account.Telephone2] = phone2.Number;

            trace.Trace("phone 3");
            if (phone3 == null) return;

            account[Attributes.Account.Telephone3_Type] = CommonXrm.GetOptionSetValue(phone3.PhoneType.ToString(), Attributes.Account.Telephone2_Type);
            account[Attributes.Account.Telephone3] = phone3.Number;

            trace.Trace("Account populate phone - end");
        }
        public static void PopulateAddress(Entity account, Address[] addresses, ITracingService trace)

        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null.");
            trace.Trace("Account populate address - start");
            if (account == null) throw new InvalidPluginExecutionException("Account entity is null.");

            if (addresses == null || addresses.Length == 0) return;
            var address = addresses[0];

            if (address == null) return;
            account[Attributes.Account.Address1_AdditionalInformation] = address.AdditionalAddressInfo;
            account[Attributes.Account.Address1_FlatOrUnitNumber] = address.FlatNumberUnit;
            account[Attributes.Account.Address1_HouseNumberOrBuilding] = address.HouseNumberBuilding;
            account[Attributes.Account.Address1_Town] = address.Town;
            account[Attributes.Account.Address1_PostalCode] = address.PostalCode;

            if (!string.IsNullOrWhiteSpace(address.Country))
                account[Attributes.Account.Address1_CountryId] = new EntityReference(EntityName.Country
                                                                                    , Attributes.Country.ISO2Code
                                                                                    , address.Country);

            account[Attributes.Account.Address1_County] = address.County;
            trace.Trace("Account populate address - end");

        }
        public static Entity GetAccountEntityForBookingPayload(Customer customer, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null.");
            trace.Trace("Account populate fields - start");
            if (customer == null) throw new InvalidPluginExecutionException("Customer payload is null.");
            if (customer.CustomerIdentifier == null || string.IsNullOrWhiteSpace(customer.CustomerIdentifier.CustomerId))
                throw new InvalidPluginExecutionException("Customer Id could not be retrieved from payload.");

            Entity account = new Entity(EntityName.Account
                                              , Attributes.Account.SourceSystemID
                                              , customer.CustomerIdentifier.CustomerId);

            if (customer.Company == null || string.IsNullOrWhiteSpace(customer.Company.CompanyName))
                throw new InvalidPluginExecutionException("Account name could not be retrieved from payload.");

            account[Attributes.Account.Name] = customer.Company.CompanyName;
            PopulateAddress(account, customer.Address, trace);
            PopulatePhone(account, customer.Phone, trace);
            PopulateEmail(account, customer.Email, trace);

            if (!string.IsNullOrWhiteSpace(customer.CustomerIdentifier.SourceMarket))
                account[Attributes.Account.SourceMarketId] = new EntityReference(EntityName.Country
                                                                , Attributes.Country.ISO2Code
                                                                , customer.CustomerIdentifier.SourceMarket);

            account[Attributes.Account.SourceSystemID] = customer.CustomerIdentifier.CustomerId;
            trace.Trace("Account populate fields - end");
            return account;
        }
    }
}
