using System;
using Microsoft.Xrm.Sdk;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services
{
    public static class ContactHelper
    {
        public static Entity GetContactEntityForBookingPayload(Customer customer, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Contact populate fields - start");
            if (customer == null) throw new InvalidPluginExecutionException("Customer payload is null");

            if (customer.CustomerIdentifier == null || string.IsNullOrWhiteSpace(customer.CustomerIdentifier.CustomerId))
                throw new InvalidPluginExecutionException("Customer Id could not be retrieved from payload.");


            Entity contact = new Entity(EntityName.Contact
                                        , Attributes.Contact.SourceSystemID
                                        , customer.CustomerIdentifier.CustomerId);

            PopulateIdentityInformation(contact, customer.CustomerIdentity, trace);

            if (customer.Additional != null)
            {
                trace.Trace("Contact populate Additional details - start");
                if (customer.Additional.Segment != null)
                    contact[Attributes.Contact.Segment] = CommonXrm.GetOptionSetValue(customer.Additional.Segment, Attributes.Contact.Segment);
                else
                    contact[Attributes.Contact.Segment] = null;

                if (customer.Additional.DateOfDeath != null)
                    contact[Attributes.Contact.DateofDeath] = Convert.ToDateTime(customer.Additional.DateOfDeath);
                else
                    contact[Attributes.Contact.DateofDeath] = null;
                trace.Trace("Contact populate Additional details - end");
            }

            PopulateAddress(contact, customer.Address, trace);
            PopulatePhone(contact, customer.Phone, trace);
            PopulateEmail(contact, customer.Email, trace);            
            contact[Attributes.Contact.SourceMarketId] = (!string.IsNullOrWhiteSpace(customer.CustomerIdentifier.SourceMarket)) ?
                                                                            new EntityReference(EntityName.Country
                                                                            , Attributes.Country.ISO2Code,
                                                                            customer.CustomerIdentifier.SourceMarket) : null;
            contact[Attributes.Contact.SourceSystemID] = (customer.CustomerIdentifier.CustomerId !=null) ? customer.CustomerIdentifier.CustomerId : string.Empty;

            trace.Trace("Contact populate fields - end");

            return contact;
        }

        public static Entity DeActivateContact(Customer customer, Guid contactId, ITracingService trace)
        {
            Entity contact = null;
            if (customer.CustomerGeneral != null)
            {
                if (customer.CustomerGeneral.CustomerStatus == CustomerStatus.B ||
                    customer.CustomerGeneral.CustomerStatus == CustomerStatus.D)
                {
                    trace.Trace("Processing Customer Deactivation - start");
                    contact = new Entity(EntityName.Contact, contactId);
                    contact[Attributes.Contact.StateCode] = new OptionSetValue((int)Statecode.InActive);
                    contact[Attributes.Contact.StatusCode] = CommonXrm.GetOptionSetValue(customer.CustomerGeneral.CustomerStatus.ToString(), Attributes.Contact.StatusCode);
                    trace.Trace("Processing Customer Deactivation - end");
                }
            }

            return contact;
        }

        private static void PopulateEmail(Entity contact, Email[] emails, ITracingService trace)
        {
            trace.Trace("Contact populate email - start");
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            if (emails == null || emails.Length == 0) { ClearEmailList(contact); return; }

            var email1 = emails[0];
            var email2 = emails.Length > 1 ? emails[1] : ClearEmail2(contact);
            var email3 = emails.Length > 2 ? emails[2] : ClearEmail3(contact);

            if (email1 == null) return;
            trace.Trace("email 1");
            contact[Attributes.Contact.EMailAddress1] = (email1.Address != null) ? email1.Address : string.Empty ;
            contact[Attributes.Contact.EmailAddress1Type] = CommonXrm.GetOptionSetValue(email1.EmailType.ToString(), Attributes.Contact.EmailAddress1Type);
            if (email2 == null) return;

            trace.Trace("email 2");
            contact[Attributes.Contact.EMailAddress2] = (email2.Address != null) ? email2.Address:string.Empty;
            contact[Attributes.Contact.EmailAddress2Type] = CommonXrm.GetOptionSetValue(email2.EmailType.ToString(), Attributes.Contact.EmailAddress2Type);

            trace.Trace("email 3");
            if (email3 == null) return;
            contact[Attributes.Contact.EMailAddress3] = (email3.Address != null) ? email3.Address:string.Empty;
            contact[Attributes.Contact.EmailAddress3Type] = CommonXrm.GetOptionSetValue(email3.EmailType.ToString(), Attributes.Contact.EmailAddress3Type);
            trace.Trace("Contact populate email - end");

        }


        private static Email ClearEmailList(Entity contact)
        {
            ClearEmail1(contact);
            ClearEmail2(contact);
            ClearEmail3(contact);
            return null;
        }

        private static Email ClearEmail1(Entity contact)
        {
            contact[Attributes.Contact.EMailAddress1] = string.Empty;
            contact[Attributes.Contact.EmailAddress1Type] = null;
            return null;
        }

        private static Email ClearEmail2(Entity contact)
        {
            contact[Attributes.Contact.EMailAddress2] = string.Empty;
            contact[Attributes.Contact.EmailAddress2Type] = null;
            return null;
        }

        private static Email ClearEmail3(Entity contact)
        {
            contact[Attributes.Contact.EMailAddress3] = string.Empty;
            contact[Attributes.Contact.EmailAddress3Type] = null;
            return null;
        }

        private static void PopulatePhone(Entity contact, Phone[] phoneNumbers, ITracingService trace)
        {
            trace.Trace("Contact populate phone - start");
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            if (contact == null) throw new InvalidPluginExecutionException("Populate Phone: contact entity is null.");
            if (phoneNumbers == null || phoneNumbers.Length == 0) { ClearPhoneList(contact); return; };

            var phone1 = phoneNumbers[0];
            var phone2 = phoneNumbers.Length > 1 ? phoneNumbers[1] : ClearPhone2(contact);
            var phone3 = phoneNumbers.Length > 2 ? phoneNumbers[2] : ClearPhone3(contact);

            trace.Trace("phone 1");
            if (phone1 == null) return;
            if (phone1.PhoneType == PhoneType.H || phone1.PhoneType == PhoneType.M)
                contact[Attributes.Contact.Telephone1Type] = CommonXrm.GetOptionSetValue(phone1.PhoneType.ToString(), Attributes.Contact.Telephone1Type);
            contact[Attributes.Contact.Telephone1] = (phone1.Number != null) ? phone1.Number:string.Empty ;

            trace.Trace("phone 2");
            if (phone2 == null) return;
            if (phone2.PhoneType == PhoneType.H || phone2.PhoneType == PhoneType.M)
                contact[Attributes.Contact.Telephone2Type] = CommonXrm.GetOptionSetValue(phone2.PhoneType.ToString(), Attributes.Contact.Telephone2Type);
            contact[Attributes.Contact.Telephone2] = (phone2.Number != null) ? phone2.Number : string.Empty;

            trace.Trace("phone 3");
            if (phone3 == null) return;
            if (phone3.PhoneType == PhoneType.H || phone3.PhoneType == PhoneType.M)
                contact[Attributes.Contact.Telephone3Type] = CommonXrm.GetOptionSetValue(phone3.PhoneType.ToString(), Attributes.Contact.Telephone3Type);
            contact[Attributes.Contact.Telephone3] = (phone3.Number != null) ? phone3.Number : string.Empty;

            trace.Trace("Contact populate phone - end");
        }

        private static void ClearPhoneList(Entity contact)
        {
            ClearPhone1(contact);
            ClearPhone2(contact);
            ClearPhone3(contact);
        }

        private static Phone ClearPhone1(Entity contact)
        {
            contact[Attributes.Contact.Telephone1Type] = null;
            contact[Attributes.Contact.Telephone1] = string.Empty;
            return null;
        }

        private static Phone ClearPhone2(Entity contact)
        {
            contact[Attributes.Contact.Telephone2Type] = null;
            contact[Attributes.Contact.Telephone2] = string.Empty;
            return null;
        }

        private static Phone ClearPhone3(Entity contact)
        {
            contact[Attributes.Contact.Telephone3Type] = null;
            contact[Attributes.Contact.Telephone3] = string.Empty;
            return null;
        }

        private static void PopulateIdentityInformation(Entity contact, CustomerIdentity identity, ITracingService trace)
        {
            trace.Trace("Contact populate idenity - start");
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            if (identity == null) return;

            if (string.IsNullOrWhiteSpace(identity.LastName))
                throw new InvalidPluginExecutionException("Last name could not be retrieved from payload.");

            contact[Attributes.Contact.FirstName] = (identity.FirstName != null) ? identity.FirstName : string.Empty;
            contact[Attributes.Contact.MiddleName] = (identity.MiddleName != null) ? identity.MiddleName : string.Empty;
            contact[Attributes.Contact.LastName] = (identity.LastName != null) ? identity.LastName : string.Empty;
            contact[Attributes.Contact.Language] = (identity.Language != null) ?
                                                   CommonXrm.GetOptionSetValue(identity.Language, Attributes.Contact.Language) 
                                                   : null;
            contact[Attributes.Contact.Gender] = CommonXrm.GetOptionSetValue(identity.Gender.ToString(), Attributes.Contact.Gender);
            contact[Attributes.Contact.AcademicTitle] = (identity.AcademicTitle != null) ? identity.AcademicTitle : string.Empty;
            contact[Attributes.Contact.Salutation] = (identity.Salutation != null) ?
                                                      CommonXrm.GetOptionSetValue(identity.Salutation, Attributes.Contact.Salutation)
                                                      : null;
            contact[Attributes.Contact.BirthDate] = (identity.Birthdate != null) ? Convert.ToDateTime(identity.Birthdate) : (DateTime?)null;
            trace.Trace("Contact populate idenity - end");
        }

        private static void PopulateAddress(Entity contact, Address[] addresses, ITracingService trace)
        {
            trace.Trace("Contact populate address - start");
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            if (addresses == null || addresses.Length == 0) { ClearAddress(contact); return; }
            var address1 = addresses[0];
            var address2 = addresses.Length > 1 ? addresses[1] : ClearAddress2(contact);

            trace.Trace("Address 1");
            if (address1 == null) return;
            contact[Attributes.Contact.Address1_AdditionalInformation] = (address1.AdditionalAddressInfo != null) ? address1.AdditionalAddressInfo:string.Empty;
            contact[Attributes.Contact.Address1_FlatOrUnitNumber] = (address1.FlatNumberUnit != null)? address1.FlatNumberUnit : string.Empty;
            contact[Attributes.Contact.Address1_HouseNumberOrBuilding] = (address1.HouseNumberBuilding != null) ? address1.HouseNumberBuilding : string.Empty ;
            contact[Attributes.Contact.Address1_Town] = (address1.Town != null) ? address1.Town : string.Empty;
            contact[Attributes.Contact.Address1_CountryId] = (!string.IsNullOrWhiteSpace(address1.Country)) ?
                                                                             new EntityReference(EntityName.Country,
                                                                             Attributes.Country.ISO2Code,
                                                                             address1.Country) : null;

            contact[Attributes.Contact.Address1_County] = (address1.County != null) ? address1.County :string.Empty ;
            contact[Attributes.Contact.Address1_PostalCode] = (address1.PostalCode != null) ? address1.PostalCode : string.Empty;

            trace.Trace("Address 2");
            if (address2 == null) return;
            contact[Attributes.Contact.Address2_AdditionalInformation] = (address2.AdditionalAddressInfo != null)?address2.AdditionalAddressInfo:string.Empty;
            contact[Attributes.Contact.Address2_FlatOrUnitNumber] = (address2.FlatNumberUnit != null)?address2.FlatNumberUnit:string.Empty;
            contact[Attributes.Contact.Address2_HouseNumberorBuilding] = (address2.HouseNumberBuilding != null)?address2.HouseNumberBuilding:string.Empty;
            contact[Attributes.Contact.Address2_Town] = (address2.Town != null) ? address2.Town:string.Empty;
            contact[Attributes.Contact.Address2_CountryId] = (!string.IsNullOrWhiteSpace(address2.Country)) ?
                                                                                 new EntityReference(EntityName.Country,
                                                                                 Attributes.Country.ISO2Code,
                                                                                 address2.Country) : null;
            contact[Attributes.Contact.Address2_County] = (address2.County != null) ? address2.Country:string.Empty;
            contact[Attributes.Contact.Address2_PostalCode] = (address2.PostalCode != null) ? address2.PostalCode :string.Empty;
            trace.Trace("Contact populate address - end");
        }

        private static Address ClearAddress(Entity contact)
        {
            ClearAddress1(contact);
            ClearAddress2(contact);
            return null;
        }

        private static Address ClearAddress1(Entity contact)
        {
            contact[Attributes.Contact.Address1_AdditionalInformation] = string.Empty;
            contact[Attributes.Contact.Address1_FlatOrUnitNumber] = string.Empty;
            contact[Attributes.Contact.Address1_HouseNumberOrBuilding] = string.Empty;
            contact[Attributes.Contact.Address1_Town] = string.Empty;
            contact[Attributes.Contact.Address1_CountryId] = null;
            contact[Attributes.Contact.Address1_County] = string.Empty;
            contact[Attributes.Contact.Address1_PostalCode] = string.Empty;
            return null;
        }

        private static Address ClearAddress2(Entity contact)
        {
            contact[Attributes.Contact.Address2_AdditionalInformation] = string.Empty;
            contact[Attributes.Contact.Address2_FlatOrUnitNumber] = string.Empty;
            contact[Attributes.Contact.Address2_HouseNumberorBuilding] = string.Empty;
            contact[Attributes.Contact.Address2_Town] = string.Empty;
            contact[Attributes.Contact.Address2_CountryId] = null;
            contact[Attributes.Contact.Address2_County] = string.Empty;
            contact[Attributes.Contact.Address2_PostalCode] = string.Empty;
            return null;
        }
    }
}
