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

            if (emailList == null || emailList.Length == 0) { ClearEmailList(account); return; }

            var email1 = emailList[0];
            var email2 = emailList.Length > 1 ? emailList[1] : ClearEmail2(account);
            var email3 = emailList.Length > 2 ? emailList[2] : ClearEmail3(account);

            trace.Trace("email 1");
            if (email1 == null) return;

            account[Attributes.Account.EMailAddress1] = (email1.Address !=null) ? email1.Address : string.Empty ;
            account[Attributes.Account.EmailAddress1_Type] = CommonXrm.GetOptionSetValue(email1.EmailType.ToString() , Attributes.Account.EmailAddress1_Type);

            trace.Trace("email 2");
            if (email2 == null) return;

            account[Attributes.Account.EMailAddress2] = (email2.Address != null) ? email2.Address : string.Empty ;
            account[Attributes.Account.EmailAddress2_Type] = CommonXrm.GetOptionSetValue(email2.EmailType.ToString(), Attributes.Account.EmailAddress2_Type);

            trace.Trace("email 3");
            if (email3 == null) return;

            account[Attributes.Account.EMailAddress3] = (email3.Address != null) ? email3.Address : string.Empty;
            account[Attributes.Account.EmailAddress3_Type] = CommonXrm.GetOptionSetValue(email3.EmailType.ToString(), Attributes.Account.EmailAddress3_Type);

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
            account[Attributes.Account.EMailAddress1] = string.Empty;
            account[Attributes.Account.EmailAddress1_Type] = null;
            return null;
        }

        private static Email ClearEmail2(Entity account)
        {
            account[Attributes.Account.EMailAddress2] = string.Empty;
            account[Attributes.Account.EmailAddress2_Type] = null;
            return null;
        }

        private static Email ClearEmail3(Entity account)
        {
            account[Attributes.Account.EMailAddress3] = string.Empty;
            account[Attributes.Account.EmailAddress3_Type] = null;
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

            account[Attributes.Account.Telephone1_Type] = CommonXrm.GetOptionSetValue(phone1.PhoneType.ToString(), Attributes.Account.Telephone1_Type);
            account[Attributes.Account.Telephone1] = (phone1.Number != null) ? phone1.Number :string.Empty;

            trace.Trace("phone 2");
            if (phone2 == null) return;

            account[Attributes.Account.Telephone2_Type] = CommonXrm.GetOptionSetValue(phone2.PhoneType.ToString(), Attributes.Account.Telephone2_Type);
            account[Attributes.Account.Telephone2] = (phone2.Number != null) ? phone2.Number :string.Empty;

            trace.Trace("phone 3");
            if (phone3 == null) return;

            account[Attributes.Account.Telephone3_Type] = CommonXrm.GetOptionSetValue(phone3.PhoneType.ToString(), Attributes.Account.Telephone3_Type);
            account[Attributes.Account.Telephone3] = (phone3.Number != null) ? phone3.Number :string.Empty ;

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
            account[Attributes.Account.Telephone1_Type] = null;
            account[Attributes.Account.Telephone1] = string.Empty;
            return null;
        }

        private static Phone ClearPhone2(Entity account)
        {
            account[Attributes.Account.Telephone2_Type] = null;
            account[Attributes.Account.Telephone2] = string.Empty;
            return null;            
        }

        private static Phone ClearPhone3(Entity account)
        {
            account[Attributes.Account.Telephone3_Type] = null;
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
            account[Attributes.Account.Address1_AdditionalInformation] = (address.AdditionalAddressInfo != null)? address.AdditionalAddressInfo:string.Empty;
            account[Attributes.Account.Address1_FlatOrUnitNumber] = (address.FlatNumberUnit != null) ? address.FlatNumberUnit :string.Empty;
            account[Attributes.Account.Address1_HouseNumberOrBuilding] = (address.HouseNumberBuilding != null) ? address.HouseNumberBuilding : string.Empty;
            account[Attributes.Account.Address1_Town] = (address.Town != null) ? address.Town : string.Empty ;
            account[Attributes.Account.Address1_PostalCode] = (address.PostalCode != null) ? address.PostalCode : string.Empty ;            
            account[Attributes.Account.Address1_CountryId] = (!string.IsNullOrWhiteSpace(address.Country)) ? new EntityReference(EntityName.Country
                                                                                                           , Attributes.Country.ISO2Code
                                                                                                           , address.Country)
                                                                                                           : null;
            account[Attributes.Account.Address1_County] = (address.County!= null) ? address.County:string.Empty ;
            trace.Trace("Account populate address - end");

        }

        private static Address ClearAddress(Entity account)
        {
            account[Attributes.Account.Address1_AdditionalInformation] = string.Empty;
            account[Attributes.Account.Address1_FlatOrUnitNumber] = string.Empty;
            account[Attributes.Account.Address1_HouseNumberOrBuilding] = string.Empty;
            account[Attributes.Account.Address1_Town] = string.Empty;
            account[Attributes.Account.Address1_PostalCode] = string.Empty;
            account[Attributes.Account.Address1_CountryId] = null;
            account[Attributes.Account.Address1_County] = string.Empty;
            return null;
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

            
            account[Attributes.Account.SourceMarketId] = (!string.IsNullOrWhiteSpace(customer.CustomerIdentifier.SourceMarket)) ?
                                                                new EntityReference(EntityName.Country
                                                                , Attributes.Country.ISO2Code
                                                                , customer.CustomerIdentifier.SourceMarket):
                                                                null;

            account[Attributes.Account.SourceSystemID] = (customer.CustomerIdentifier.CustomerId !=null) ? customer.CustomerIdentifier.CustomerId:string.Empty;
            trace.Trace("Account populate fields - end");
            return account;
        }
    }
}
