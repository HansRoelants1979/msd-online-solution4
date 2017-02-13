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
            if (customer.CustomerGeneral != null)
            {
                if (customer.CustomerGeneral.CustomerStatus == CustomerStatus.B ||
                    customer.CustomerGeneral.CustomerStatus == CustomerStatus.D)
                {
                    contact[Attributes.Contact.StateCode] = new OptionSetValue((int)Statecode.InActive);
                    contact[Attributes.Contact.StatusCode] = CommonXrm.GetOptionSetValue(customer.CustomerGeneral.CustomerStatus.ToString(), Attributes.Contact.StatusCode);
                }
            }

            if (customer.Additional != null)
            {
                contact[Attributes.Contact.Segment] = CommonXrm.GetOptionSetValue(customer.Additional.Segment, Attributes.Contact.Segment);
                contact[Attributes.Contact.DateofDeath] = Convert.ToDateTime(customer.Additional.DateOfDeath);
            }

            PopulateAddress(contact, customer.Address, trace);
            PopulatePhone(contact, customer.Phone, trace);
            PopulateEmail(contact, customer.Email, trace);

            if (!string.IsNullOrWhiteSpace(customer.CustomerIdentifier.SourceMarket))
            {
                contact[Attributes.Contact.SourceMarketId] = new EntityReference(EntityName.Country
                                                                                , Attributes.Country.ISO2Code,
                                                                                customer.CustomerIdentifier.SourceMarket);
            }

            contact[Attributes.Contact.SourceSystemID] = customer.CustomerIdentifier.CustomerId;

            trace.Trace("Contact populate fields - end");

            return contact;
        }

        private static void PopulateEmail(Entity contact, Email[] emails, ITracingService trace)
        {
            trace.Trace("Contact populate email - start");
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            if (emails == null || emails.Length == 0) return;

            var email1 = emails[0];
            var email2 = emails.Length > 1 ? emails[1] : null;
            var email3 = emails.Length > 2 ? emails[2] : null;

            if (email1 == null) return;
            trace.Trace("email 1");
            contact[Attributes.Contact.EMailAddress1] = email1.Address;
            contact[Attributes.Contact.EmailAddress1Type] = CommonXrm.GetOptionSetValue(email1.EmailType.ToString(), Attributes.Contact.EmailAddress1Type);
            if (email2 == null) return;

            trace.Trace("email 2");
            contact[Attributes.Contact.EMailAddress2] = email2.Address;
            contact[Attributes.Contact.EmailAddress2Type] = CommonXrm.GetOptionSetValue(email2.EmailType.ToString(), Attributes.Contact.EmailAddress2Type);

            trace.Trace("email 3");
            if (email3 == null) return;
            contact[Attributes.Contact.EMailAddress3] = email3.Address;
            contact[Attributes.Contact.EmailAddress3Type] = CommonXrm.GetOptionSetValue(email3.EmailType.ToString(), Attributes.Contact.EmailAddress3Type);
            trace.Trace("Contact populate email - end");

        }

        private static void PopulatePhone(Entity contact, Phone[] phoneNumbers, ITracingService trace)
        {
            trace.Trace("Contact populate phone - start");
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            if (contact == null) throw new InvalidPluginExecutionException("Populate Phone: contact enity is null.");
            if (phoneNumbers == null || phoneNumbers.Length == 0) return;

            var phone1 = phoneNumbers[0];
            var phone2 = phoneNumbers.Length > 1 ? phoneNumbers[1] : null;
            var phone3 = phoneNumbers.Length > 2 ? phoneNumbers[2] : null;

            trace.Trace("phone 1");
            if (phone1 == null) return;
            if (phone1.PhoneType == PhoneType.H || phone1.PhoneType == PhoneType.M)
                contact[Attributes.Contact.Telephone1Type] = CommonXrm.GetOptionSetValue(phone1.PhoneType.ToString(), Attributes.Contact.Telephone1Type);
            contact[Attributes.Contact.Telephone1] = phone1.Number;

            trace.Trace("phone 2");
            if (phone2 == null) return;
            if (phone2.PhoneType == PhoneType.H || phone2.PhoneType == PhoneType.M)
                contact[Attributes.Contact.Telephone2Type] = CommonXrm.GetOptionSetValue(phone2.PhoneType.ToString(), Attributes.Contact.Telephone2Type);
            contact[Attributes.Contact.Telephone2] = phone2.Number;

            trace.Trace("phone 3");
            if (phone3 == null) return;
            if (phone3.PhoneType == PhoneType.H || phone3.PhoneType == PhoneType.M)
                contact[Attributes.Contact.Telephone3Type] = CommonXrm.GetOptionSetValue(phone3.PhoneType.ToString(), Attributes.Contact.Telephone3Type);
            contact[Attributes.Contact.Telephone3] = phone3.Number;

            trace.Trace("Contact populate phone - end");
        }

        private static void PopulateIdentityInformation(Entity contact, CustomerIdentity identity, ITracingService trace)
        {
            trace.Trace("Contact populate idenity - start");
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            if (identity == null) return;

            if (string.IsNullOrWhiteSpace(identity.LastName))
                throw new InvalidPluginExecutionException("Last name could not be retrieved from payload.");

            contact[Attributes.Contact.FirstName] = identity.FirstName;
            contact[Attributes.Contact.MiddleName] = identity.MiddleName;
            contact[Attributes.Contact.LastName] = identity.LastName;
            contact[Attributes.Contact.Language] = CommonXrm.GetOptionSetValue(identity.Language, Attributes.Contact.Language);
            contact[Attributes.Contact.Gender] = CommonXrm.GetOptionSetValue(identity.Gender.ToString(), Attributes.Contact.Gender);
            contact[Attributes.Contact.AcademicTitle] = identity.AcademicTitle;
            contact[Attributes.Contact.Salutation] = CommonXrm.GetOptionSetValue(identity.Salutation, Attributes.Contact.Salutation);
            contact[Attributes.Contact.BirthDate] = Convert.ToDateTime(identity.Birthdate);
            trace.Trace("Contact populate idenity - end");
        }

        private static void PopulateAddress(Entity contact, Address[] addresses, ITracingService trace)
        {
            trace.Trace("Contact populate address - start");
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            if (addresses == null || addresses.Length == 0) return;
            var address1 = addresses[0];
            var address2 = addresses.Length > 1 ? addresses[1] : null;

            trace.Trace("Address 1");
            if (address1 == null) return;
            contact[Attributes.Contact.Address1_AdditionalInformation] = address1.AdditionalAddressInfo;
            contact[Attributes.Contact.Address1_FlatOrUnitNumber] = address1.FlatNumberUnit;
            contact[Attributes.Contact.Address1_HouseNumberOrBuilding] = address1.HouseNumberBuilding;
            contact[Attributes.Contact.Address1_Town] = address1.Town;
            if (string.IsNullOrWhiteSpace(address1.Country))
                contact[Attributes.Contact.Address1_CountryId] = null;
            else
                contact[Attributes.Contact.Address1_CountryId] = new EntityReference(EntityName.Country
                                                                                                  , Attributes.Country.ISO2Code
                                                                                                  , address1.Country);

            contact[Attributes.Contact.Address1_County] = address1.County;
            contact[Attributes.Contact.Address1_PostalCode] = address1.PostalCode;

            trace.Trace("Address 2");
            if (address2 == null) return;
            contact[Attributes.Contact.Address2_AdditionalInformation] = address2.AdditionalAddressInfo;
            contact[Attributes.Contact.Address2_FlatOrUnitNumber] = address2.FlatNumberUnit;
            contact[Attributes.Contact.Address2_HouseNumberorBuilding] = address2.HouseNumberBuilding;
            contact[Attributes.Contact.Address2_Town] = address2.Town;
            if (string.IsNullOrWhiteSpace(address2.Country))
                contact[Attributes.Contact.Address2_CountryId] = null;
            else
                contact[Attributes.Contact.Address2_CountryId] = new EntityReference(EntityName.Country,
                                                                                     Attributes.Country.ISO2Code,
                                                                                     address2.Country);
            contact[Attributes.Contact.Address2_County] = address2.County;
            contact[Attributes.Contact.Address2_PostalCode] = address2.PostalCode;
            trace.Trace("Contact populate address - end");
        }
    }
}
