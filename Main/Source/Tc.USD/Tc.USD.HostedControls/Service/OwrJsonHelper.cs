using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using Newtonsoft.Json;
using Tc.Crm.Common.Constants;
using Tc.Crm.Common.Constants.Attributes;
using Tc.Usd.HostedControls.Models;

namespace Tc.Usd.HostedControls.Service
{
    public class OwrJsonHelper
    {
        private readonly CrmServiceClient _client;
        private readonly Entity _opportunity;
        public OwrJsonHelper(CrmServiceClient client, Entity opportunity)
        {
            _client = client;
            _opportunity = opportunity;
        }

        public string GetCustomerTravelPlannerJson(DataCollection<Entity> rooms)
        {
            var owrSearch = new OwrSearch();
            owrSearch.RequestId = Guid.NewGuid();
            owrSearch.TravelPlanner = GetTravelPlanner(rooms);
            var owrSearchJson = JsonConvert.SerializeObject(owrSearch);

            return owrSearchJson;
        }

        private TravelPlanner GetTravelPlanner( DataCollection<Entity> rooms)
        {
            var travelPlanner = new TravelPlanner();
            travelPlanner.TravelPlannerId = _opportunity.Id;
            travelPlanner.ConsultationReference = _opportunity.GetAttributeValue<string>(Opportunity.Name);
            travelPlanner.DepartureDateFrom = _opportunity.Contains(Opportunity.EarliestDepartureDate)
                ? GetOwrDateFormat(_opportunity.GetAttributeValue<DateTime>(Opportunity.EarliestDepartureDate))
                : null;
            travelPlanner.DepartureDateTo = _opportunity.Contains(Opportunity.LatestDepartureDate)
                ? GetOwrDateFormat(_opportunity.GetAttributeValue<DateTime>(Opportunity.LatestDepartureDate))
                : null;
            var duration = _opportunity.GetAttributeValue<OptionSetValue>(Opportunity.Duration)?.Value;
            if (duration != null)
            {
                travelPlanner.NumberOfNights = (NumberOfNights)duration;
            }

            travelPlanner.IncludedDestinations = GetIncludedDestinations();
            travelPlanner.ExcludedDestinations = GetExcludedDestinations();

            travelPlanner.DeparturePoints = new[]
                {
                    GetGatewayOwrName(_opportunity.GetAttributeValue<EntityReference>(Opportunity.DeparturePoint1)),
                    GetGatewayOwrName(_opportunity.GetAttributeValue<EntityReference>(Opportunity.DeparturePoint2)),
                    GetGatewayOwrName(_opportunity.GetAttributeValue<EntityReference>(Opportunity.DeparturePoint3))
                };
            travelPlanner.Rooms = GetRoomsByEntity(rooms);
            travelPlanner.Customer = GetCustomer();
            return travelPlanner;
        }

        private CustomerOwr GetCustomer()
        {
            var customer = new CustomerOwr();

            customer.CustomerIdentifier = new CustomerIdentifierOwr
            {
                CustomerId = _opportunity.Contains(AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.SourceSystemId) ?
                    ((AliasedValue)_opportunity[AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.SourceSystemId]).Value.ToString():""
            };
            customer.CustomerIdentity = new CustomerIdentityOwr
            {
                Salutation = _opportunity.Contains(AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Salutation) ?
                _opportunity.FormattedValues[AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Salutation] : "",
                FirstName = _opportunity.Contains(AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.FirstName) ?
                    ((AliasedValue)_opportunity[AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.FirstName]).Value.ToString() : "",
                MiddleName = _opportunity.Contains(AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.MiddleName) ?
                    ((AliasedValue)_opportunity[AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.MiddleName]).Value.ToString() : "",
                LastName = _opportunity.Contains(AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.LastName) ?
                    ((AliasedValue)_opportunity[AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.LastName]).Value.ToString() : "",
                BirthDate = _opportunity.Contains(AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Birthdate) ?
                   GetOwrDateFormat((DateTime)((AliasedValue)_opportunity[AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Birthdate]).Value) : ""

            };
            customer.Address = GetAddresses();

            customer.Phone = GetPhones();
            customer.Email = GetEmails();
            return customer;
        }

        private string[] GetExcludedDestinations()
        {
            var excluded1 = _opportunity.GetAttributeValue<string>(Opportunity.Exclude1);
            var excluded2 = _opportunity.GetAttributeValue<string>(Opportunity.Exclude2);
            var excluded3 = _opportunity.GetAttributeValue<string>(Opportunity.Exclude3);
            var excludedDestinations = new List<string>();
            if (excluded1 != null)
            {
                excludedDestinations.Add(excluded1);
            }
            if (excluded2 != null)
            {
                excludedDestinations.Add(excluded2);
            }
            if (excluded3 != null)
            {
                excludedDestinations.Add(excluded3);
            }

            return excludedDestinations.ToArray();
        }

        private string[] GetIncludedDestinations()
        {
            if (_opportunity.Contains(Opportunity.HowDoYouWantToSearch))
            {
                var howDoYouWantToSearch =
                    (HowDoYouWantToSearch)_opportunity.GetAttributeValue<OptionSetValue>(Opportunity.HowDoYouWantToSearch).Value;
                switch (howDoYouWantToSearch)
                {
                    case HowDoYouWantToSearch.All:
                        return new string[] { ((int)HowDoYouWantToSearch.All).ToString() };
                    case HowDoYouWantToSearch.ByCountry:
                        var destinationCountry1 = CrmService.GetIso2Code(_client, _opportunity.GetAttributeValue<EntityReference>(Opportunity.DestinationCountry1)?.Id) + " - " + _opportunity.GetAttributeValue<EntityReference>(Opportunity.DestinationCountry1)?.Name;
                        return new[] { destinationCountry1, ((int)HowDoYouWantToSearch.ByCountry).ToString() };
                    case HowDoYouWantToSearch.ByRegion:
                        var destinationRegion1 = CrmService.GetRegionCode(_client, _opportunity.GetAttributeValue<EntityReference>(Opportunity.Region1)?.Id);
                        return new[] { destinationRegion1, ((int)HowDoYouWantToSearch.ByRegion).ToString() };
                    case HowDoYouWantToSearch.ByHotel:
                        var hotel1 = CrmService.GetHotelCode(_client, _opportunity.GetAttributeValue<EntityReference>(Opportunity.Hotel1)?.Id);
                        return new[] { hotel1, ((int)HowDoYouWantToSearch.ByHotel).ToString()};
                    case HowDoYouWantToSearch.ByDestinationAirport:
                        var destinationAirport1 = GetGatewayOwrName(_opportunity.GetAttributeValue<EntityReference>(Opportunity.DestinationAirport1));
                        return new[] { destinationAirport1, ((int)HowDoYouWantToSearch.ByDestinationAirport).ToString() };
                    default:
                        return new string[] { };
                }
            }
            return new string[] { };
        }

        private RoomOwr[] GetRoomsByEntity(DataCollection<Entity> rooms)
        {
            if (rooms == null)
            {
                return null;
            }
            var roomsOwr = new List<RoomOwr>();
            foreach (var room in rooms)
            {
                var roomOwr = new RoomOwr { NumberOfAdults = room.GetAttributeValue<int>(Room.NoOfAdults) };
                if (room.Contains(Room.NumberOfChildren))
                {
                    roomOwr.NumberOfChildren = Int32.Parse(room.FormattedValues[Room.NumberOfChildren]);
                }

                var childrenAges = new List<ChildrenAges>();
                var child1 = room.GetAttributeValue<OptionSetValue>(Room.Child1)?.Value;
                if (child1 != null)
                {
                    childrenAges.Add((ChildrenAges)child1);
                }
                var child2 = room.GetAttributeValue<OptionSetValue>(Room.Child2)?.Value;
                if (child2 != null)
                {
                    childrenAges.Add((ChildrenAges)child2);
                }
                var child3 = room.GetAttributeValue<OptionSetValue>(Room.Child3)?.Value;
                if (child3 != null)
                {
                    childrenAges.Add((ChildrenAges)child3);
                }
                var child4 = room.GetAttributeValue<OptionSetValue>(Room.Child4)?.Value;
                if (child4 != null)
                {
                    childrenAges.Add((ChildrenAges)child4);
                }
                var child5 = room.GetAttributeValue<OptionSetValue>(Room.Child5)?.Value;
                if (child5 != null)
                {
                    childrenAges.Add((ChildrenAges)child5);
                }
                var child6 = room.GetAttributeValue<OptionSetValue>(Room.Child6)?.Value;
                if (child6 != null)
                {
                    childrenAges.Add((ChildrenAges)child6);
                }
                roomOwr.ChildrensAges = childrenAges.ToArray();
                roomsOwr.Add(roomOwr);
            }
            return roomsOwr.ToArray();
        }

        private AddressOwr[] GetAddresses()
        {
           var addresses = new[]
            {
                    new AddressOwr
                    {
                        FlatNumberUnit = _opportunity.Contains(AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Address1FlatorUnitNumber) ?
                            ((AliasedValue)_opportunity[AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Address1FlatorUnitNumber]).Value.ToString():"",
                        HouseNumberBuilding = _opportunity.Contains(AliasName.ContactAliasName +
                                                                  Crm.Common.Constants.Attributes.Customer.Address1HouseNumberoBuilding)?
                           ((AliasedValue)_opportunity[AliasName.ContactAliasName +
                                                                  Crm.Common.Constants.Attributes.Customer.Address1HouseNumberoBuilding]).Value.ToString():"",
                        Town = _opportunity.Contains(AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Address1Town)?
                            ((AliasedValue)_opportunity[AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Address1Town]).Value.ToString():"",
                        Country = _opportunity.Contains(AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Address1CountryId) ?
                           CrmService.GetIso2Code(_client, ((EntityReference)((AliasedValue) _opportunity[AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Address1CountryId]).Value)?.Id) : "",
                        County = _opportunity.Contains(AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Address1County)?
                           ((AliasedValue) _opportunity[AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Address1County]).Value.ToString():"",
                        PostalCode = _opportunity.Contains(AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Address1PostalCode)?
                            ((AliasedValue)_opportunity[AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Address1PostalCode]).Value.ToString():""
                    },
                    new AddressOwr
                    {
                        FlatNumberUnit = _opportunity.Contains(AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Address2FlatorUnitNumber)?
                            ((AliasedValue)_opportunity[AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Address2FlatorUnitNumber]).Value.ToString():"",
                        HouseNumberBuilding = _opportunity.Contains(AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Address2HouseNumberoBuilding)?
                            ((AliasedValue)_opportunity[AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Address2HouseNumberoBuilding]).Value.ToString():"",
                        Town = _opportunity.Contains(AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Address2Town)?
                            ((AliasedValue)_opportunity[AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Address2Town]).Value.ToString():"",
                        Country = _opportunity.Contains(AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Address2CountryId)?
                          CrmService.GetIso2Code(_client, ((EntityReference)((AliasedValue) _opportunity[AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Address1CountryId]).Value)?.Id) : "",
                        County = _opportunity.Contains(AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Address2County)?
                            ((AliasedValue)_opportunity[AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Address2County]).Value.ToString():"",
                        PostalCode = _opportunity.Contains(AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Address2PostalCode)?
                            ((AliasedValue)_opportunity[AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Address2PostalCode]).Value.ToString():""
                    }
                };
            return addresses;
        }
        private PhoneOwr[] GetPhones()
        {
            var phones = new List<PhoneOwr>();

            var phoneName1 = AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Telephone1;
            var phoneTypeName1 = AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Telephone1Type;
            if (_opportunity.Contains(phoneName1))
            {
                var phone1 = new PhoneOwr();
                phone1.Number =  ((AliasedValue)_opportunity[phoneName1])?.Value.ToString();
                if (_opportunity.Contains(phoneTypeName1))
                {
                    phone1.PhoneType = (PhoneType)((OptionSetValue)((AliasedValue)_opportunity[phoneTypeName1]).Value).Value;
                }
                phones.Add(phone1);
            }

            var phoneName2 = AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Telephone2;
            var phoneTypeName2 = AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Telephone2Type;
            if (_opportunity.Contains(phoneName2))
            {
                var phone2 = new PhoneOwr();
                phone2.Number = ((AliasedValue)_opportunity[phoneName2])?.Value.ToString();
                if (_opportunity.Contains(phoneTypeName2))
                {
                    phone2.PhoneType =
                        (PhoneType)
                        ((OptionSetValue)
                            ((AliasedValue)
                                    _opportunity[
                                       phoneTypeName2])
                                .Value).Value;
                }

                phones.Add(phone2);
            }


            var phoneName3 = AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Telephone3;
            var phoneTypeName3 = AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.Telephone3Type;
            if (_opportunity.Contains(phoneName3))
            {
                var phone3 = new PhoneOwr();
                phone3.Number =  ((AliasedValue)_opportunity[phoneName3])?.Value?.ToString();

                if (
                    _opportunity.Contains(phoneTypeName3))
                {
                    phone3.PhoneType =(PhoneType)((OptionSetValue)((AliasedValue)_opportunity[phoneTypeName3]).Value).Value;
                }
                phones.Add(phone3);
            }
            return phones.ToArray();
        }

        private EmailOwr[] GetEmails()
        {
            var emails = new List<EmailOwr>();
            var emailAddressName1 = AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.EmailAddress1;
            var emailAddressTypeName1 = AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.EmailAddress1Type;
            if (_opportunity.Contains(emailAddressName1))
            {
                var email1 = new EmailOwr();
                email1.Address = ((AliasedValue)_opportunity[emailAddressName1])?.Value.ToString();

                if (_opportunity.Contains(emailAddressTypeName1))
                {
                    email1.EmailType =(EmailType)((OptionSetValue)((AliasedValue)_opportunity[emailAddressTypeName1]).Value).Value;
                }
                emails.Add(email1);
            }
            var emailAddressName2 = AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.EmailAddress2;
            var emailAddressTypeName2 = AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.EmailAddress2Type;
            if (_opportunity.Contains(emailAddressName2))
            {
                var email2 = new EmailOwr();
                email2.Address = ((AliasedValue)_opportunity[emailAddressName2]).Value.ToString();

                if (_opportunity.Contains(emailAddressTypeName2))
                {
                    email2.EmailType = (EmailType)((OptionSetValue)((AliasedValue)_opportunity[emailAddressTypeName2]).Value).Value;
                }
                emails.Add(email2);
            }

            var emailAddressName3 = AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.EmailAddress3;
            var emailAddressTypeName3 = AliasName.ContactAliasName + Crm.Common.Constants.Attributes.Customer.EmailAddress3Type;
            if (_opportunity.Contains(emailAddressName3))
            {
                var email3 = new EmailOwr();
                email3.Address = ((AliasedValue)_opportunity[emailAddressName3]).Value.ToString();

                if (_opportunity.Contains(emailAddressTypeName3))
                {
                    email3.EmailType =
                        (EmailType)
                        ((OptionSetValue)
                            ((AliasedValue)
                                    _opportunity[emailAddressTypeName3])
                                .Value).Value;
                }

                emails.Add(email3);
            }
            return emails.ToArray();
        }

        private string GetGatewayOwrName(EntityReference gatewayRef)
        {
            if (gatewayRef != null)
            {
                var gatewayOwrName = gatewayRef.Id + " - " + CrmService.GetAirportName(_client, gatewayRef.Id) + " - " + gatewayRef.Name;
                return gatewayOwrName;
            }
            return "";
        }

        private string GetOwrDateFormat(DateTime crmDate)
        {
            var correctDate = $"{crmDate:yyyy-MM-dd}";
            return correctDate;
        }
    }
}
