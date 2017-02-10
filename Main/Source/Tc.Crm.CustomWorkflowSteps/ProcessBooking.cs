using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace Tc.Crm.CustomWorkflowSteps
{
    public class ProcessBooking
    {
        private PayloadBooking payloadBooking;
        public ProcessBooking(PayloadBooking payloadBooking)
        {
            this.payloadBooking = payloadBooking;
        }
        /// <summary>
        /// To process booking data
        /// </summary>
        /// <param name="json"></param>       
        /// <returns></returns>
        public string ProcessPayload(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) throw new InvalidPluginExecutionException("Payload is null.");

            payloadBooking.Trace.Trace("Processing Process payload");
            string response = string.Empty;
            payloadBooking.BookingInfo = DeSerializeJson(json);
            payloadBooking.DeleteBookingRole = true;
            payloadBooking.DeleteAccomodationOrTransportOrRemarks = true;

            //Validate payload for customer
            if (payloadBooking.BookingInfo.Customer == null)
                throw new InvalidPluginExecutionException("Customer info missing in payload.");
            if (payloadBooking.BookingInfo.Customer.CustomerIdentifier == null)
                throw new InvalidPluginExecutionException("Customer Identifier is missing.");
            if (string.IsNullOrWhiteSpace(payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId))
                throw new InvalidPluginExecutionException("Customer source system id is missing.");

            //B:Account
            //P:Contact
            if (payloadBooking.BookingInfo.Customer.CustomerGeneral.CustomerType == CustomerType.B)
            {
                ProcessAccount();
            }
            else if (payloadBooking.BookingInfo.Customer.CustomerGeneral.CustomerType == CustomerType.P)
            {
                ProcessContact();
            }


            ProcessBookingInfo();

            ProcessRemarks();

            ProcessAccomodation();

            ProcessTransport();

            ProcessBookingRole();


            response = SerializeJson(payloadBooking.Response);
            return response;
        }

        /// <summary>
        /// To serialize object to json
        /// </summary>
        /// <param name="BookingResponse"></param>
        /// <returns></returns>
        string SerializeJson(BookingResponse response)
        {
            payloadBooking.Trace.Trace("Processing Serialization of BookingResponse");
            MemoryStream memoryStream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(BookingResponse));

            serializer.WriteObject(memoryStream, response);
            byte[] json = memoryStream.ToArray();
            memoryStream.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }


        /// <summary>
        /// To deserialize json to object
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        Booking DeSerializeJson(string json)
        {
            payloadBooking.Trace.Trace("Processing DeSerialization of json Payload");

            Booking bookingInfo = new Booking();
            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer deSerializer = new DataContractJsonSerializer(bookingInfo.GetType());
            bookingInfo = deSerializer.ReadObject(memoryStream) as Booking;
            memoryStream.Close();
            return bookingInfo;

        }

        /// <summary>
        /// To process contact information
        /// </summary>       
        /// <returns></returns>
        XrmResponse ProcessContact()
        {
            XrmResponse xrmResponse = null;
            
            payloadBooking.Trace.Trace("Processing Contact information");

            Entity contactEntity = new Entity(EntityName.Contact, Attributes.Contact.SourceSystemID, payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId);

            contactEntity[Attributes.Contact.FirstName] = payloadBooking.BookingInfo.Customer.CustomerIdentity.FirstName;
            contactEntity[Attributes.Contact.MiddleName] = payloadBooking.BookingInfo.Customer.CustomerIdentity.MiddleName;
            contactEntity[Attributes.Contact.LastName] = payloadBooking.BookingInfo.Customer.CustomerIdentity.LastName;
            contactEntity[Attributes.Contact.Language] = GetOptionSetValue(payloadBooking.BookingInfo.Customer.CustomerIdentity.Language, Attributes.Contact.Language);
            contactEntity[Attributes.Contact.Gender] = GetOptionSetValue(payloadBooking.BookingInfo.Customer.CustomerIdentity.Gender.ToString(), Attributes.Contact.Gender);
            contactEntity[Attributes.Contact.AcademicTitle] = payloadBooking.BookingInfo.Customer.CustomerIdentity.AcademicTitle;
            contactEntity[Attributes.Contact.Salutation] = GetOptionSetValue(payloadBooking.BookingInfo.Customer.CustomerIdentity.Salutation, Attributes.Contact.Salutation);
            contactEntity[Attributes.Contact.BirthDate] = Convert.ToDateTime(payloadBooking.BookingInfo.Customer.CustomerIdentity.Birthdate);

            if (payloadBooking.BookingInfo.Customer.CustomerGeneral.CustomerStatus.ToString() == General.Blacklisted ||
                payloadBooking.BookingInfo.Customer.CustomerGeneral.CustomerStatus.ToString() == General.Deceased)
            {
                contactEntity[Attributes.Contact.StateCode] = new OptionSetValue((int)Statecode.InActive);
                contactEntity[Attributes.Contact.StatusCode] = GetOptionSetValue(payloadBooking.BookingInfo.Customer.CustomerGeneral.CustomerStatus.ToString(), Attributes.Contact.StatusCode);
            }
            // couldn't find segment,DateofDeath in booking.customer.identity.additional
            contactEntity[Attributes.Contact.Segment] = GetOptionSetValue(payloadBooking.BookingInfo.Customer.Additional.Segment, Attributes.Contact.Segment);
            contactEntity[Attributes.Contact.DateofDeath] = Convert.ToDateTime(payloadBooking.BookingInfo.Customer.Additional.DateOfDeath);
            if (payloadBooking.BookingInfo.Customer.Address != null && payloadBooking.BookingInfo.Customer.Address.Length > 0)
            {
                payloadBooking.Trace.Trace("Processing Contact Address information");
                contactEntity[Attributes.Contact.Address1_AdditionalInformation] = payloadBooking.BookingInfo.Customer.Address[0].AdditionalAddressInfo;
                contactEntity[Attributes.Contact.Address1_FlatOrUnitNumber] = payloadBooking.BookingInfo.Customer.Address[0].FlatNumberUnit;
                contactEntity[Attributes.Contact.Address1_HouseNumberOrBuilding] = payloadBooking.BookingInfo.Customer.Address[0].HouseNumberBuilding;
                contactEntity[Attributes.Contact.Address1_Town] = payloadBooking.BookingInfo.Customer.Address[0].Town;
                if (payloadBooking.BookingInfo.Customer.Address[0].Country != null)
                {
                    contactEntity[Attributes.Contact.Address1_CountryId] = new EntityReference(EntityName.Country, Attributes.Country.ISO2Code, payloadBooking.BookingInfo.Customer.Address[0].Country);
                }
                contactEntity[Attributes.Contact.Address1_County] = payloadBooking.BookingInfo.Customer.Address[0].County;
                contactEntity[Attributes.Contact.Address1_PostalCode] = payloadBooking.BookingInfo.Customer.Address[0].PostalCode;

                if (payloadBooking.BookingInfo.Customer.Address.Length > 1)
                {

                    contactEntity[Attributes.Contact.Address2_AdditionalInformation] = payloadBooking.BookingInfo.Customer.Address[1].AdditionalAddressInfo;
                    contactEntity[Attributes.Contact.Address2_FlatOrUnitNumber] = payloadBooking.BookingInfo.Customer.Address[1].FlatNumberUnit;
                    contactEntity[Attributes.Contact.Address2_HouseNumberorBuilding] = payloadBooking.BookingInfo.Customer.Address[1].HouseNumberBuilding;
                    contactEntity[Attributes.Contact.Address2_Town] = payloadBooking.BookingInfo.Customer.Address[1].Town;

                    if (payloadBooking.BookingInfo.Customer.Address[1].Country != null)
                        contactEntity[Attributes.Contact.Address2_CountryId] = new EntityReference(EntityName.Country, Attributes.Country.ISO2Code, payloadBooking.BookingInfo.Customer.Address[1].Country);

                    contactEntity[Attributes.Contact.Address2_County] = payloadBooking.BookingInfo.Customer.Address[1].County;
                    contactEntity[Attributes.Contact.Address2_PostalCode] = payloadBooking.BookingInfo.Customer.Address[1].PostalCode;
                }
            }
            if (payloadBooking.BookingInfo.Customer.Phone != null && payloadBooking.BookingInfo.Customer.Phone.Length > 0)
            {
                payloadBooking.Trace.Trace("Processing Contact Phone information");
                contactEntity[Attributes.Contact.Telephone1Type] = GetOptionSetValue(payloadBooking.BookingInfo.Customer.Phone[0].PhoneType.ToString(), Attributes.Contact.Telephone1Type);
                contactEntity[Attributes.Contact.Telephone1] = payloadBooking.BookingInfo.Customer.Phone[0].Number;
                if (payloadBooking.BookingInfo.Customer.Phone.Length > 1)
                {
                    contactEntity[Attributes.Contact.Telephone2Type] = GetOptionSetValue(payloadBooking.BookingInfo.Customer.Phone[1].PhoneType.ToString(), Attributes.Contact.Telephone2Type);
                    contactEntity[Attributes.Contact.Telephone2] = payloadBooking.BookingInfo.Customer.Phone[1].Number;
                }
                if (payloadBooking.BookingInfo.Customer.Phone.Length > 2)
                {
                    contactEntity[Attributes.Contact.Telephone3Type] = GetOptionSetValue(payloadBooking.BookingInfo.Customer.Phone[2].PhoneType.ToString(), Attributes.Contact.Telephone3Type);
                    contactEntity[Attributes.Contact.Telephone3] = payloadBooking.BookingInfo.Customer.Phone[2].Number;
                }
            }
            if (payloadBooking.BookingInfo.Customer.Email != null && payloadBooking.BookingInfo.Customer.Email.Length > 0)
            {
                payloadBooking.Trace.Trace("Processing Contact Email information");
                contactEntity[Attributes.Contact.EMailAddress1] = payloadBooking.BookingInfo.Customer.Email[0].Address;
                contactEntity[Attributes.Contact.EmailAddress1Type] = GetOptionSetValue(payloadBooking.BookingInfo.Customer.Email[0].EmailType.ToString(), Attributes.Contact.EmailAddress1Type);
                if (payloadBooking.BookingInfo.Customer.Email.Length > 1)
                {
                    contactEntity[Attributes.Contact.EMailAddress2] = payloadBooking.BookingInfo.Customer.Email[1].Address;
                    contactEntity[Attributes.Contact.EmailAddress2Type] = GetOptionSetValue(payloadBooking.BookingInfo.Customer.Email[1].EmailType.ToString(), Attributes.Contact.EmailAddress2Type);
                }
                if (payloadBooking.BookingInfo.Customer.Email.Length > 2)
                {
                    contactEntity[Attributes.Contact.EMailAddress3] = payloadBooking.BookingInfo.Customer.Email[2].Address;
                    contactEntity[Attributes.Contact.EmailAddress3Type] = GetOptionSetValue(payloadBooking.BookingInfo.Customer.Email[2].EmailType.ToString(), Attributes.Contact.EmailAddress3Type);
                }
            }
            if (payloadBooking.BookingInfo.Customer.CustomerIdentifier.SourceMarket != null)
            {
                contactEntity[Attributes.Contact.SourceMarketId] = new EntityReference(EntityName.Country, Attributes.Country.ISO2Code, payloadBooking.BookingInfo.Customer.CustomerIdentifier.SourceMarket);
            }
            contactEntity[Attributes.Contact.SourceSystemID] = payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId;
            xrmResponse = CommonXrm.UpsertEntity(contactEntity, payloadBooking.CrmService);

            if (xrmResponse.Create)
                payloadBooking.DeleteBookingRole = false;

            payloadBooking.CustomerId = xrmResponse.Id;


            return xrmResponse;
        }

        /// <summary>
        /// To process account information
        /// </summary>        
        /// <returns></returns>
        XrmResponse ProcessAccount()
        {
            XrmResponse xrmResponse = null;
            
            payloadBooking.Trace.Trace("Processing Account information");

            Entity accountEntity = new Entity(EntityName.Account, Attributes.Account.SourceSystemID, payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId);
            accountEntity[Attributes.Account.Name] = payloadBooking.BookingInfo.Customer.Company.CompanyName;
            if (payloadBooking.BookingInfo.Customer.Address != null && payloadBooking.BookingInfo.Customer.Address.Length > 0)
            {
                payloadBooking.Trace.Trace("Processing Account Address information");
                accountEntity[Attributes.Account.Address1_AdditionalInformation] = payloadBooking.BookingInfo.Customer.Address[0].AdditionalAddressInfo;
                accountEntity[Attributes.Account.Address1_FlatOrUnitNumber] = payloadBooking.BookingInfo.Customer.Address[0].FlatNumberUnit;
                accountEntity[Attributes.Account.Address1_HouseNumberOrBuilding] = payloadBooking.BookingInfo.Customer.Address[0].HouseNumberBuilding;
                accountEntity[Attributes.Account.Address1_Town] = payloadBooking.BookingInfo.Customer.Address[0].Town;
                accountEntity[Attributes.Account.Address1_PostalCode] = payloadBooking.BookingInfo.Customer.Address[0].PostalCode;

                if (payloadBooking.BookingInfo.Customer.Address[0].Country != null)
                    accountEntity[Attributes.Account.Address1_CountryId] = new EntityReference(EntityName.Country, Attributes.Country.ISO2Code, payloadBooking.BookingInfo.Customer.Address[0].Country);

                accountEntity[Attributes.Account.Address1_County] = payloadBooking.BookingInfo.Customer.Address[0].County;
            }
            if (payloadBooking.BookingInfo.Customer.Phone != null && payloadBooking.BookingInfo.Customer.Phone.Length > 0)
            {
                payloadBooking.Trace.Trace("Processing Account Phone information");
                accountEntity[Attributes.Account.Telephone1_Type] = GetOptionSetValue(payloadBooking.BookingInfo.Customer.Phone[0].PhoneType.ToString(), Attributes.Account.Telephone1_Type);

                accountEntity[Attributes.Account.Telephone1] = payloadBooking.BookingInfo.Customer.Phone[0].Number;
                if (payloadBooking.BookingInfo.Customer.Phone.Length > 1)
                {
                    accountEntity[Attributes.Account.Telephone2_Type] = GetOptionSetValue(payloadBooking.BookingInfo.Customer.Phone[1].PhoneType.ToString(), Attributes.Account.Telephone2_Type);
                    accountEntity[Attributes.Account.Telephone2] = payloadBooking.BookingInfo.Customer.Phone[1].Number;
                }
                if (payloadBooking.BookingInfo.Customer.Phone.Length > 2)
                {
                    accountEntity[Attributes.Account.Telephone3_Type] = GetOptionSetValue(payloadBooking.BookingInfo.Customer.Phone[2].PhoneType.ToString(), Attributes.Account.Telephone3_Type);
                    accountEntity[Attributes.Account.Telephone3] = payloadBooking.BookingInfo.Customer.Phone[2].Number;
                }
            }
            if (payloadBooking.BookingInfo.Customer.Email != null && payloadBooking.BookingInfo.Customer.Email.Length > 0)
            {
                payloadBooking.Trace.Trace("Processing Account Email information");
                accountEntity[Attributes.Account.EMailAddress1] = payloadBooking.BookingInfo.Customer.Email[0].Address;
                accountEntity[Attributes.Account.EmailAddress1_Type] = GetOptionSetValue(payloadBooking.BookingInfo.Customer.Email[0].EmailType.ToString(), Attributes.Account.EmailAddress1_Type);
                if (payloadBooking.BookingInfo.Customer.Email.Length > 1)
                {
                    accountEntity[Attributes.Account.EMailAddress2] = payloadBooking.BookingInfo.Customer.Email[1].Address;
                    accountEntity[Attributes.Account.EmailAddress2_Type] = GetOptionSetValue(payloadBooking.BookingInfo.Customer.Email[1].EmailType.ToString(), Attributes.Account.EmailAddress2_Type);
                }
                if (payloadBooking.BookingInfo.Customer.Email.Length > 2)
                {
                    accountEntity[Attributes.Account.EMailAddress3] = payloadBooking.BookingInfo.Customer.Email[2].Address;
                    accountEntity[Attributes.Account.EmailAddress3_Type] = GetOptionSetValue(payloadBooking.BookingInfo.Customer.Email[2].EmailType.ToString(), Attributes.Account.EmailAddress3_Type);
                }
            }
            if (payloadBooking.BookingInfo.Customer.CustomerIdentifier.SourceMarket != null)
            {
                accountEntity[Attributes.Account.SourceMarketId] = new EntityReference(EntityName.Country, Attributes.Country.ISO2Code, payloadBooking.BookingInfo.Customer.CustomerIdentifier.SourceMarket);
            }

            accountEntity[Attributes.Account.SourceSystemID] = payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId;


            xrmResponse = CommonXrm.UpsertEntity(accountEntity, payloadBooking.CrmService);
            if (xrmResponse.Create)
                payloadBooking.DeleteBookingRole = false;

            payloadBooking.CustomerId = xrmResponse.Id;



            return xrmResponse;
        }

        /// <summary>
        /// To process booking information
        /// </summary>      
        /// <returns></returns>
        XrmResponse ProcessBookingInfo()
        {
            XrmResponse xrmResponse = null;
            if (payloadBooking.BookingInfo.BookingIdentifier != null)
            {
                payloadBooking.Trace.Trace("Processing Booking information");
                Entity bookingEntity = new Entity(EntityName.Booking, Attributes.Booking.Name, payloadBooking.BookingInfo.BookingIdentifier.BookingNumber);

                bookingEntity[Attributes.Booking.Name] = payloadBooking.BookingInfo.BookingIdentifier.BookingNumber;
                bookingEntity[Attributes.Booking.OnTourVersion] = payloadBooking.BookingInfo.BookingIdentifier.BookingVersionOnTour;
                bookingEntity[Attributes.Booking.TourOperatorVersion] = payloadBooking.BookingInfo.BookingIdentifier.BookingVersionTourOperator;
                bookingEntity[Attributes.Booking.OnTourUpdatedDate] = Convert.ToDateTime(payloadBooking.BookingInfo.BookingIdentifier.BookingUpdateDateOnTour);
                bookingEntity[Attributes.Booking.TourOperatorUpdatedDate] = Convert.ToDateTime(payloadBooking.BookingInfo.BookingIdentifier.BookingUpdateDateTourOperator);
                bookingEntity[Attributes.Booking.BookingDate] = Convert.ToDateTime(payloadBooking.BookingInfo.BookingGeneral.BookingDate);
                bookingEntity[Attributes.Booking.DepartureDate] = Convert.ToDateTime(payloadBooking.BookingInfo.BookingGeneral.DepartureDate);
                bookingEntity[Attributes.Booking.ReturnDate] = Convert.ToDateTime(payloadBooking.BookingInfo.BookingGeneral.ReturnDate);
                bookingEntity[Attributes.Booking.Duration] = Convert.ToInt64(payloadBooking.BookingInfo.BookingGeneral.Duration);
                if (payloadBooking.BookingInfo.BookingGeneral.Destination != null)
                {
                    bookingEntity[Attributes.Booking.DestinationGatewayId] = new EntityReference(EntityName.Gateway, Attributes.Gateway.IATA, payloadBooking.BookingInfo.BookingGeneral.Destination);
                }
                if (payloadBooking.BookingInfo.BookingGeneral.ToCode != null)
                {
                    bookingEntity[Attributes.Booking.TourOperatorId] = new EntityReference(EntityName.TourOperator, Attributes.TourOperator.TourOperatorCode, payloadBooking.BookingInfo.BookingGeneral.ToCode);
                }
                if (payloadBooking.BookingInfo.BookingGeneral.Brand != null)
                {
                    bookingEntity[Attributes.Booking.BrandId] = new EntityReference(EntityName.Brand, Attributes.Brand.BrandCode, payloadBooking.BookingInfo.BookingGeneral.Brand);
                }
                bookingEntity[Attributes.Booking.BrochureCode] = payloadBooking.BookingInfo.BookingGeneral.BrochureCode;
                bookingEntity[Attributes.Booking.IsLateBooking] = payloadBooking.BookingInfo.BookingGeneral.IsLateBooking;
                bookingEntity[Attributes.Booking.NumberofParticipants] = payloadBooking.BookingInfo.BookingGeneral.NumberOfParticipants;
                bookingEntity[Attributes.Booking.NumberofAdults] = payloadBooking.BookingInfo.BookingGeneral.NumberOfAdults;
                bookingEntity[Attributes.Booking.NumberofChildren] = payloadBooking.BookingInfo.BookingGeneral.NumberOfChildren;
                bookingEntity[Attributes.Booking.NumberofInfants] = payloadBooking.BookingInfo.BookingGeneral.NumberOfInfants;
                bookingEntity[Attributes.Booking.BookerPhone1] = payloadBooking.BookingInfo.BookingIdentity.Booker.Phone;
                bookingEntity[Attributes.Booking.BookerPhone2] = payloadBooking.BookingInfo.BookingIdentity.Booker.Mobile;
                bookingEntity[Attributes.Booking.BookerEmergencyPhone] = payloadBooking.BookingInfo.BookingIdentity.Booker.EmergencyNumber;
                bookingEntity[Attributes.Booking.Participants] = PrepareTravelParticipantsInfo();
                bookingEntity[Attributes.Booking.ParticipantRemarks] = PrepareTravelParticipantsRemarks();
                bookingEntity[Attributes.Booking.Transfer] = PrepareTransferInfo();
                bookingEntity[Attributes.Booking.TransferRemarks] = PrepareTransferRemarks();
                bookingEntity[Attributes.Booking.ExtraService] = PrepareExtraServicesInfo();
                bookingEntity[Attributes.Booking.ExtraServiceRemarks] = PrepareExtraServiceRemarks();
                bookingEntity[Attributes.Booking.SourceMarketId] = new EntityReference(EntityName.Country, Attributes.Country.ISO2Code, payloadBooking.BookingInfo.BookingIdentifier.SourceMarket);

                if (payloadBooking.BookingInfo.BookingGeneral.BookingStatus.ToString() == General.Booked || payloadBooking.BookingInfo.BookingGeneral.BookingStatus.ToString() == General.Cancelled)
                {
                    bookingEntity[Attributes.Booking.StateCode] = new OptionSetValue((int)Statecode.InActive);
                    bookingEntity[Attributes.Booking.StatusCode] = GetOptionSetValue(payloadBooking.BookingInfo.BookingGeneral.BookingStatus.ToString(), Attributes.Booking.StatusCode);
                }
                
                bookingEntity[Attributes.Booking.TravelAmount] = new Money(payloadBooking.BookingInfo.BookingGeneral.TravelAmount);
                bookingEntity[Attributes.Booking.TransactionCurrencyId] = new EntityReference(EntityName.Currency,Attributes.Currency.Name, payloadBooking.BookingInfo.BookingGeneral.Currency);
                bookingEntity[Attributes.Booking.HasSourceMarketComplaint] = payloadBooking.BookingInfo.BookingGeneral.HasComplaint;
                bookingEntity[Attributes.Booking.BookerEmail] = payloadBooking.BookingInfo.BookingIdentity.Booker.Email;
                xrmResponse = CommonXrm.UpsertEntity(bookingEntity, payloadBooking.CrmService);
                payloadBooking.Response = new BookingResponse();
                if (xrmResponse.Create)
                {
                    payloadBooking.DeleteBookingRole = false;
                    payloadBooking.DeleteAccomodationOrTransportOrRemarks = false;
                    payloadBooking.Response.Created = true;

                }

                payloadBooking.BookingId = xrmResponse.Id;
                payloadBooking.Response.Success = true;
                payloadBooking.Response.Id = xrmResponse.Id;

            }
            return xrmResponse;
        }

        /// <summary>
        /// To prepare travel participants information
        /// </summary>        
        /// <returns></returns>
        string PrepareTravelParticipantsInfo()
        {
            string travelParticipants = string.Empty;
            if (payloadBooking.BookingInfo.TravelParticipant != null)
            {
                payloadBooking.Trace.Trace("Processing Travel Participants information");
                for (int i = 0; i < payloadBooking.BookingInfo.TravelParticipant.Length; i++)
                {
                    if (!string.IsNullOrEmpty(travelParticipants))
                        travelParticipants += General.NextLine;

                    travelParticipants += payloadBooking.BookingInfo.TravelParticipant[i].TravelParticipantIdOnTour + General.Seperator +
                                          payloadBooking.BookingInfo.TravelParticipant[i].FirstName + General.Seperator +
                                          payloadBooking.BookingInfo.TravelParticipant[i].LastName + General.Seperator +
                                          payloadBooking.BookingInfo.TravelParticipant[i].Age.ToString() + General.Seperator +
                                          payloadBooking.BookingInfo.TravelParticipant[i].Birthdate + General.Seperator +
                                          payloadBooking.BookingInfo.TravelParticipant[i].Gender.ToString() + General.Seperator +
                                          payloadBooking.BookingInfo.TravelParticipant[i].Relation.ToString() + General.Seperator +
                                          payloadBooking.BookingInfo.TravelParticipant[i].Language;

                }
            }
            return travelParticipants;
        }

        /// <summary>
        /// To prepare travel participants remarks information
        /// </summary>       
        /// <returns></returns>
        string PrepareTravelParticipantsRemarks()
        {
            string remarks = string.Empty;
            if (payloadBooking.BookingInfo.TravelParticipant != null)
            {
                payloadBooking.Trace.Trace("Processing Travel Participant Remarks information");
                for (int i = 0; i < payloadBooking.BookingInfo.TravelParticipant.Length; i++)
                {
                    if (payloadBooking.BookingInfo.TravelParticipant[i].Remark != null)
                        for (int j = 0; j < payloadBooking.BookingInfo.TravelParticipant[i].Remark.Length; j++)
                        {
                            if (!string.IsNullOrEmpty(remarks))
                                remarks += General.NextLine;

                            remarks += payloadBooking.BookingInfo.TravelParticipant[i].TravelParticipantIdOnTour + General.Seperator +
                                       payloadBooking.BookingInfo.TravelParticipant[i].Remark[j].RemarkType.ToString() + General.Seperator +
                                       payloadBooking.BookingInfo.TravelParticipant[i].Remark[j].Text;
                        }

                }
            }
            return remarks;
        }

        /// <summary>
        /// To prepare transfer information
        /// </summary>     
        /// <returns></returns>
        string PrepareTransferInfo()
        {
            string transferInfo = string.Empty;
            if (payloadBooking.BookingInfo.Services != null && payloadBooking.BookingInfo.Services.Transfer != null)
            {
                payloadBooking.Trace.Trace("Processing Transfer information");
                for (int i = 0; i < payloadBooking.BookingInfo.Services.Transfer.Length; i++)
                {
                    if (!string.IsNullOrEmpty(transferInfo))
                        transferInfo += General.NextLine;

                    transferInfo += payloadBooking.BookingInfo.Services.Transfer[i].TransferCode + General.Seperator +
                                    payloadBooking.BookingInfo.Services.Transfer[i].TransferDescription + General.Seperator +
                                    payloadBooking.BookingInfo.Services.Transfer[i].Order.ToString() + General.Seperator +
                                    payloadBooking.BookingInfo.Services.Transfer[i].StartDate + General.Seperator +
                                    payloadBooking.BookingInfo.Services.Transfer[i].EndDate + General.Seperator +
                                    payloadBooking.BookingInfo.Services.Transfer[i].Category + General.Seperator +
                                    payloadBooking.BookingInfo.Services.Transfer[i].TransferType.ToString() + General.Seperator +
                                    payloadBooking.BookingInfo.Services.Transfer[i].DepartureAirport + General.Seperator +
                                    payloadBooking.BookingInfo.Services.Transfer[i].ArrivalAirport;
                }

            }
            return transferInfo;
        }

        /// <summary>
        /// To prepare trasnfer remarks information
        /// </summary>  
        /// <returns></returns>
        string PrepareTransferRemarks()
        {
            string remarks = string.Empty;
            if (payloadBooking.BookingInfo.Services != null && payloadBooking.BookingInfo.Services.Transfer != null)
            {
                payloadBooking.Trace.Trace("Processing Transfer Remarks information");
                for (int i = 0; i < payloadBooking.BookingInfo.Services.Transfer.Length; i++)
                {
                    if (payloadBooking.BookingInfo.Services.Transfer[i].Remark != null)
                        for (int j = 0; j < payloadBooking.BookingInfo.Services.Transfer[i].Remark.Length; j++)
                        {
                            if (!string.IsNullOrEmpty(remarks))
                                remarks += General.NextLine;

                            remarks += payloadBooking.BookingInfo.Services.Transfer[i].Remark[j].RemarkType.ToString() + General.Seperator +
                                       payloadBooking.BookingInfo.Services.Transfer[i].Remark[j].Text;
                        }

                }
            }
            return remarks;
        }

        /// <summary>
        /// To prepare extraservice information
        /// </summary>       
        /// <returns></returns>
        string PrepareExtraServicesInfo()
        {
            string extraServices = string.Empty;
            if (payloadBooking.BookingInfo.Services != null && payloadBooking.BookingInfo.Services.ExtraService != null)
            {
                payloadBooking.Trace.Trace("Processing Extra Services information");
                for (int i = 0; i < payloadBooking.BookingInfo.Services.ExtraService.Length; i++)
                {

                    if (!string.IsNullOrEmpty(extraServices))
                        extraServices += General.NextLine;

                    extraServices += payloadBooking.BookingInfo.Services.ExtraService[i].ExtraServiceCode.ToString() + General.Seperator +
                                     payloadBooking.BookingInfo.Services.ExtraService[i].ExtraServiceDescription.ToString() + General.Seperator +
                                     payloadBooking.BookingInfo.Services.ExtraService[i].Order.ToString() + General.Seperator +
                                     payloadBooking.BookingInfo.Services.ExtraService[i].StartDate + General.Seperator +
                                     payloadBooking.BookingInfo.Services.ExtraService[i].EndDate;


                }
            }
            return extraServices;
        }

        /// <summary>
        /// To prepare extra service remarks information
        /// </summary>     
        /// <returns></returns>
        string PrepareExtraServiceRemarks()
        {
            string extraServiceRemarks = string.Empty;
            if (payloadBooking.BookingInfo.Services != null && payloadBooking.BookingInfo.Services.ExtraService != null)
            {
                payloadBooking.Trace.Trace("Processing ExtraService Remarks information");
                for (int i = 0; i < payloadBooking.BookingInfo.Services.ExtraService.Length; i++)
                {
                    if (payloadBooking.BookingInfo.Services.ExtraService[i].Remark != null)
                        for (int j = 0; j < payloadBooking.BookingInfo.Services.ExtraService[i].Remark.Length; j++)
                        {
                            if (!string.IsNullOrEmpty(extraServiceRemarks))
                                extraServiceRemarks += General.NextLine;

                            extraServiceRemarks += payloadBooking.BookingInfo.Services.ExtraService[i].Remark[j].RemarkType.ToString() + General.Seperator +
                                                   payloadBooking.BookingInfo.Services.ExtraService[i].Remark[j].Text;

                        }

                }
            }
            return extraServiceRemarks;
        }


        /// <summary>
        /// 
        /// </summary>
        void ProcessRemarks()
        {
            payloadBooking.Trace.Trace("Processing Remarks information");
            if (payloadBooking.DeleteAccomodationOrTransportOrRemarks)
            {
                ProcessRecordsToDelete(EntityName.Remark,
                    new string[] { Attributes.Remark.RemarkId },
                    new string[] { Attributes.BookingAccommodation.BookingId },
                    new string[] { payloadBooking.BookingId });

            }

            if (payloadBooking.BookingInfo.Remark != null)
            {
                EntityCollection entityCollectionRemarks = new EntityCollection();
                Entity remarkEntity = null;
                for (int i = 0; i < payloadBooking.BookingInfo.Remark.Length; i++)
                {
                    remarkEntity = new Entity(EntityName.Remark);
                    remarkEntity[Attributes.Remark.Name] = payloadBooking.BookingInfo.BookingIdentifier.BookingNumber + " - " + payloadBooking.BookingInfo.Remark[i].RemarkType.ToString();
                    remarkEntity[Attributes.Remark.Type] = GetOptionSetValue(payloadBooking.BookingInfo.Remark[i].RemarkType.ToString(), Attributes.Remark.Type);
                    remarkEntity[Attributes.Remark.RemarkName] = payloadBooking.BookingInfo.Remark[i].Text;
                    remarkEntity[Attributes.Remark.BookingId] = new EntityReference(EntityName.Booking, new Guid(payloadBooking.BookingId));
                    entityCollectionRemarks.Entities.Add(remarkEntity);

                }
                CommonXrm.BulkCreate(entityCollectionRemarks, payloadBooking.CrmService);
            }
        }


        /// <summary>
        /// To process accomodation informaation
        /// </summary>      
        /// <returns></returns>
        void ProcessAccomodation()
        {


            if (payloadBooking.DeleteAccomodationOrTransportOrRemarks)
            {
                payloadBooking.Trace.Trace("Processing Delete Accomodation information");
                ProcessRecordsToDelete(EntityName.BookingAccommodation,
                    new string[] { Attributes.BookingAccommodation.BookingAccommodationid },
                    new string[] { Attributes.BookingAccommodation.BookingId },
                    new string[] { payloadBooking.BookingId });

            }

            if (payloadBooking.BookingInfo.Services != null && payloadBooking.BookingInfo.Services.Accommodation != null)
            {
                payloadBooking.Trace.Trace("Processing Accomodation information");
                EntityCollection entityCollectionAccomodation = new EntityCollection();
                Entity accomodationEntity = null;
                for (int i = 0; i < payloadBooking.BookingInfo.Services.Accommodation.Length; i++)
                {
                    accomodationEntity = new Entity(EntityName.BookingAccommodation);
                    //BN - HotelId 
                    accomodationEntity[Attributes.BookingAccommodation.Name] = payloadBooking.BookingInfo.BookingIdentifier.BookingNumber + " - " + payloadBooking.BookingInfo.Services.Accommodation[i].GroupAccommodationCode;
                    accomodationEntity[Attributes.BookingAccommodation.HotelId] = new EntityReference(EntityName.Hotel, Attributes.Hotel.MasterHotelID, payloadBooking.BookingInfo.Services.Accommodation[i].GroupAccommodationCode);
                    accomodationEntity[Attributes.BookingAccommodation.Order] = payloadBooking.BookingInfo.Services.Accommodation[i].Order.ToString();
                    accomodationEntity[Attributes.BookingAccommodation.StartDateandTime] = DateTime.Parse(payloadBooking.BookingInfo.Services.Accommodation[i].StartDate);
                    accomodationEntity[Attributes.BookingAccommodation.EndDateandTime] = DateTime.Parse(payloadBooking.BookingInfo.Services.Accommodation[i].EndDate);
                    accomodationEntity[Attributes.BookingAccommodation.RoomType] = payloadBooking.BookingInfo.Services.Accommodation[i].RoomType;
                    accomodationEntity[Attributes.BookingAccommodation.BoardType] = GetOptionSetValue(payloadBooking.BookingInfo.Services.Accommodation[i].BoardType.ToString(), Attributes.BookingAccommodation.BoardType);
                    accomodationEntity[Attributes.BookingAccommodation.HasSharedRoom] = payloadBooking.BookingInfo.Services.Accommodation[i].HasSharedRoom;
                    accomodationEntity[Attributes.BookingAccommodation.NumberofParticipants] = payloadBooking.BookingInfo.Services.Accommodation[i].NumberOfParticipants;
                    accomodationEntity[Attributes.BookingAccommodation.NumberofRooms] = payloadBooking.BookingInfo.Services.Accommodation[i].NumberOfRooms;
                    accomodationEntity[Attributes.BookingAccommodation.WithTransfer] = payloadBooking.BookingInfo.Services.Accommodation[i].WithTransfer;
                    accomodationEntity[Attributes.BookingAccommodation.IsExternalService] = payloadBooking.BookingInfo.Services.Accommodation[i].IsExternalService;
                    accomodationEntity[Attributes.BookingAccommodation.ExternalServiceCode] = GetOptionSetValue(payloadBooking.BookingInfo.Services.Accommodation[i].ExternalServiceCode.ToString(), Attributes.BookingAccommodation.ExternalServiceCode);
                    accomodationEntity[Attributes.BookingAccommodation.NotificationRequired] = payloadBooking.BookingInfo.Services.Accommodation[i].NotificationRequired;
                    accomodationEntity[Attributes.BookingAccommodation.NeedTourGuideAssignment] = payloadBooking.BookingInfo.Services.Accommodation[i].NeedsTourGuideAssignment;
                    accomodationEntity[Attributes.BookingAccommodation.ExternalTransfer] = payloadBooking.BookingInfo.Services.Accommodation[i].IsExternalTransfer;
                    accomodationEntity[Attributes.BookingAccommodation.TransferServiceLevel] = GetOptionSetValue(payloadBooking.BookingInfo.Services.Accommodation[i].TransferServiceLevel, Attributes.BookingAccommodation.TransferServiceLevel);
                    accomodationEntity[Attributes.BookingAccommodation.SourceMarketHotelName] = payloadBooking.BookingInfo.Services.Accommodation[i].AccommodationDescription;

                    accomodationEntity[Attributes.BookingAccommodation.BookingId] = new EntityReference(EntityName.Booking, new Guid(payloadBooking.BookingId));

                    entityCollectionAccomodation.Entities.Add(accomodationEntity);

                }

                List<XrmResponse> xrmResponseList = CommonXrm.BulkCreate(entityCollectionAccomodation, payloadBooking.CrmService);
                ProcessAccomodationRemarks(xrmResponseList);
            }
        }

        /// <summary>
        /// To process accomodation remarks
        /// </summary>       
        /// <param name="xrmResponseList"></param>
        void ProcessAccomodationRemarks(List<XrmResponse> xrmResponseList)
        {
            EntityCollection entityCollectionAccomodationRemarks = new EntityCollection();
            Entity remarkEntity = null;
            if (payloadBooking.BookingInfo.Services != null && payloadBooking.BookingInfo.Services.Accommodation != null && xrmResponseList.Count > 0)
            {
                payloadBooking.Trace.Trace("Processing Accomodation Remarks information");
                for (int i = 0; i < payloadBooking.BookingInfo.Services.Accommodation.Length; i++)
                {
                    if (payloadBooking.BookingInfo.Services.Accommodation[i].Remark != null)
                    {
                        for (int j = 0; j < payloadBooking.BookingInfo.Services.Accommodation[i].Remark.Length; j++)
                        {
                            remarkEntity = new Entity(EntityName.Remark);
                            remarkEntity[Attributes.Remark.Name] = payloadBooking.BookingInfo.BookingIdentifier.BookingNumber + " - " + payloadBooking.BookingInfo.Services.Accommodation[i].Remark[j].RemarkType.ToString();
                            remarkEntity[Attributes.Remark.Type] = GetOptionSetValue(payloadBooking.BookingInfo.Services.Accommodation[i].Remark[j].RemarkType.ToString(), Attributes.Remark.Type);
                            remarkEntity[Attributes.Remark.RemarkName] = payloadBooking.BookingInfo.Services.Accommodation[i].Remark[j].Text;
                            //messageList.Find()
                            remarkEntity[Attributes.Remark.BookingAccommodationId] = new EntityReference(EntityName.BookingAccommodation, Guid.Parse(xrmResponseList[i].Id));
                            entityCollectionAccomodationRemarks.Entities.Add(remarkEntity);
                        }
                    }
                }

                if (entityCollectionAccomodationRemarks.Entities.Count > 0)
                    CommonXrm.BulkCreate(entityCollectionAccomodationRemarks, payloadBooking.CrmService);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void ProcessTransport()
        {
            if (payloadBooking.BookingInfo.Services != null && payloadBooking.BookingInfo.Services.Transport != null)
            {
                EntityCollection entityCollectionTransport = new EntityCollection();
                Entity transportEntity = null;

                if (payloadBooking.DeleteAccomodationOrTransportOrRemarks)
                {
                    payloadBooking.Trace.Trace("Processing Delete Transport information");
                    ProcessRecordsToDelete(EntityName.BookingTransport,
                        new string[] { Attributes.BookingTransport.BookingTransportId },
                        new string[] { Attributes.BookingAccommodation.BookingId },
                        new string[] { payloadBooking.BookingId });

                }

                for (int i = 0; i < payloadBooking.BookingInfo.Services.Transport.Length; i++)
                {
                    payloadBooking.Trace.Trace("Processing Transport information");
                    // BN -  DepartureGFate - ArrivalAGateway
                    transportEntity = new Entity(EntityName.BookingTransport);
                    transportEntity[Attributes.BookingTransport.Name] = payloadBooking.BookingInfo.BookingIdentifier.BookingNumber + " - " + payloadBooking.BookingInfo.Services.Transport[i].DepartureAirport + " - " + payloadBooking.BookingInfo.Services.Transport[i].ArrivalAirport;
                    transportEntity[Attributes.BookingTransport.TransportCode] = payloadBooking.BookingInfo.Services.Transport[i].TransportCode;
                    transportEntity[Attributes.BookingTransport.Description] = payloadBooking.BookingInfo.Services.Transport[i].TransportDescription;
                    transportEntity[Attributes.BookingTransport.Order] = payloadBooking.BookingInfo.Services.Transport[i].Order;
                    transportEntity[Attributes.BookingTransport.StartDateandTime] = DateTime.Parse(payloadBooking.BookingInfo.Services.Transport[i].StartDate);
                    transportEntity[Attributes.BookingTransport.EndDateandTime] = DateTime.Parse(payloadBooking.BookingInfo.Services.Transport[i].EndDate);
                    transportEntity[Attributes.BookingTransport.TransferType] = GetOptionSetValue(payloadBooking.BookingInfo.Services.Transport[i].TransferType.ToString(), Attributes.BookingTransport.TransferType);
                    transportEntity[Attributes.BookingTransport.DepartureGatewayId] = new EntityReference(EntityName.Gateway, Attributes.Gateway.IATA, payloadBooking.BookingInfo.Services.Transport[i].DepartureAirport);
                    transportEntity[Attributes.BookingTransport.ArrivalGatewayId] = new EntityReference(EntityName.Gateway, Attributes.Gateway.IATA, payloadBooking.BookingInfo.Services.Transport[i].ArrivalAirport);
                    transportEntity[Attributes.BookingTransport.CarrierCode] = payloadBooking.BookingInfo.Services.Transport[i].CarrierCode;
                    transportEntity[Attributes.BookingTransport.FlightNumber] = payloadBooking.BookingInfo.Services.Transport[i].FlightNumber;
                    transportEntity[Attributes.BookingTransport.FlightIdentifier] = payloadBooking.BookingInfo.Services.Transport[i].FlightIdentifier;
                    transportEntity[Attributes.BookingTransport.NumberofParticipants] = payloadBooking.BookingInfo.Services.Transport[i].NumberOfParticipants;


                    transportEntity[Attributes.BookingTransport.BookingId] = new EntityReference(EntityName.Booking, new Guid(payloadBooking.BookingId));

                    entityCollectionTransport.Entities.Add(transportEntity);


                }

                List<XrmResponse> xrmResponseList = CommonXrm.BulkCreate(entityCollectionTransport, payloadBooking.CrmService);
                ProcessTransportRemarks(xrmResponseList);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xrmResponseList"></param>
        void ProcessTransportRemarks(List<XrmResponse> xrmResponseList)
        {
            if (payloadBooking.BookingInfo.Services.Transport != null && xrmResponseList.Count > 0)
            {
                EntityCollection entityColllectionTransportRemark = new EntityCollection();
                Entity transportRemark = null;

                payloadBooking.Trace.Trace("Processing Transport Remarks information");

                for (int i = 0; i < payloadBooking.BookingInfo.Services.Transport.Length; i++)
                {
                    for (int j = 0; j < payloadBooking.BookingInfo.Services.Transport[i].Remark.Length; j++)
                    {
                        //BN - Type

                        transportRemark = new Entity(EntityName.Remark);
                        transportRemark[Attributes.Remark.Name] = payloadBooking.BookingInfo.BookingIdentifier.BookingNumber + " - " + payloadBooking.BookingInfo.Services.Transport[i].Remark[j].RemarkType.ToString();
                        transportRemark[Attributes.Remark.Type] = GetOptionSetValue(payloadBooking.BookingInfo.Services.Transport[i].Remark[j].RemarkType.ToString(), Attributes.Remark.Type);
                        transportRemark[Attributes.Remark.RemarkName] = payloadBooking.BookingInfo.Services.Transport[i].Remark[j].Text;
                        transportRemark[Attributes.Remark.BookingTransportId] = new EntityReference(EntityName.BookingTransport, new Guid(xrmResponseList[i].Id));

                        entityColllectionTransportRemark.Entities.Add(transportRemark);
                    }


                }

                if (entityColllectionTransportRemark.Entities.Count > 0)
                    CommonXrm.BulkCreate(entityColllectionTransportRemark, payloadBooking.CrmService);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        void ProcessBookingRole()
        {

            if (payloadBooking.DeleteBookingRole)
            {
                payloadBooking.Trace.Trace("Processing Delete Booking Roles information");
                ProcessRecordsToDelete(EntityName.CustomerBookingRole,
                    new string[] { Attributes.CustomerBookingRole.CustomerBookingRoleId },
                    new string[] { Attributes.CustomerBookingRole.BookingId, Attributes.CustomerBookingRole.Customer },
                    new string[] { payloadBooking.BookingId, payloadBooking.CustomerId });

            }
            payloadBooking.Trace.Trace("Processing Booking Roles information");
            Entity entityBookingRole = new Entity(EntityName.CustomerBookingRole);
            entityBookingRole[Attributes.CustomerBookingRole.BookingId] = new EntityReference(EntityName.Booking, Attributes.Booking.Name, payloadBooking.BookingInfo.BookingIdentifier.BookingNumber);
            if (payloadBooking.BookingInfo.Customer.CustomerGeneral.CustomerType == CustomerType.B)
            {
                entityBookingRole[Attributes.CustomerBookingRole.Customer] = new EntityReference(EntityName.Account, Attributes.Account.SourceSystemID, payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId);
            }
            else
            {
                entityBookingRole[Attributes.CustomerBookingRole.Customer] = new EntityReference(EntityName.Contact, Attributes.Contact.SourceSystemID, payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId);
            }

            EntityCollection entityCollection = new EntityCollection();
            entityCollection.Entities.Add(entityBookingRole);

            CommonXrm.BulkCreate(entityCollection, payloadBooking.CrmService);
        }

        /// <summary>
        /// To process records to delete
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="columns"></param>
        /// <param name="filterKeys"></param>
        /// <param name="filterValues"></param>
        void ProcessRecordsToDelete(string entityName, string[] columns, string[] filterKeys, string[] filterValues)
        {
            EntityCollection entityCollection = CommonXrm.RetrieveMultipleRecords(entityName, columns, filterKeys, filterValues, payloadBooking.CrmService);
            if (entityCollection != null && entityCollection.Entities.Count > 0)
            {
                EntityReferenceCollection entityReferenceCollection = new EntityReferenceCollection();
                foreach (Entity entity in entityCollection.Entities)
                {
                    entityReferenceCollection.Add(new EntityReference(entity.LogicalName, entity.Id));
                }
                CommonXrm.BulkDelete(entityReferenceCollection, payloadBooking.CrmService);
            }
        }



        /// <summary>
        /// To get OptionSetValue by text and optionSetName
        /// </summary>
        /// <param name="text"></param>
        /// <param name="entityName"></param>
        /// <returns></returns>
        OptionSetValue GetOptionSetValue(string text, string optionSetName)
        {
            int value = -1;
            switch (optionSetName)
            {
                case "tc_language":
                    {
                        switch (text)
                        {
                            case "English":
                                value = 950000000;
                                break;
                            case "German":
                                value = 950000001;
                                break;
                            case "Dutch":
                                value = 950000002;
                                break;
                            case "French":
                                value = 950000003;
                                break;
                            case "Spanish":
                                value = 950000004;
                                break;
                            case "Danish":
                                value = 950000005;
                                break;

                        }
                    }
                    break;

                case "tc_gender":
                    {
                        switch (text)
                        {
                            case "M":
                                value = 950000000;
                                break;
                            case "F":
                                value = 950000001;
                                break;
                            case "U":
                                value = 950000002;
                                break;
                        }
                    }
                    break;
                case "tc_segment":
                    {
                        switch (text)
                        {
                            case "1":
                                value = 950000000;
                                break;
                            case "2":
                                value = 950000001;
                                break;
                            case "3":
                                value = 950000002;
                                break;
                            case "4":
                                value = 950000003;
                                break;
                            case "5":
                                value = 950000004;
                                break;

                        }
                    }
                    break;
                case "tc_salutation":
                    {
                        switch (text)
                        {
                            case "Mr":
                                value = 950000000;
                                break;
                            case "Mrs":
                                value = 950000001;
                                break;
                            case "Ms":
                                value = 950000002;
                                break;
                            case "Miss":
                                value = 950000003;
                                break;
                            case "Dr":
                                value = 950000004;
                                break;
                            case "Sir":
                                value = 950000005;
                                break;
                            case "Prof.":
                                value = 950000006;
                                break;
                            case "Lord":
                                value = 950000007;
                                break;
                            case "Lady":
                                value = 950000008;
                                break;
                        }
                    }
                    break;
                case "tc_type":
                    {
                        switch (text)
                        {
                            case "T":
                                value = 950000000;
                                break;
                            case "A":
                                value = 950000001;
                                break;
                        }
                    }
                    break;
                case "tc_transfertype":
                    {
                        switch (text)
                        {
                            case "I":
                                value = 950000000;
                                break;
                            case "O":
                                value = 950000001;
                                break;
                            case "TH":
                                value = 950000002;
                                break;
                        }
                    }
                    break;
                case "tc_transferservicelevel":
                    {
                        switch (text)
                        {
                            case "Service Level 1":
                                value = 950000000;
                                break;
                            case "Service Level 2":
                                value = 950000001;
                                break;
                            case "Service Level 3":
                                value = 950000002;
                                break;
                        }
                    }
                    break;
                case "tc_externalservicecode":
                    {
                        switch (text)
                        {
                            case "Service Code A":
                                value = 950000000;
                                break;
                            case "Service Code B":
                                value = 950000001;
                                break;
                            case "Service Code C":
                                value = 950000002;
                                break;
                        }
                    }
                    break;
                case "tc_boardtype":
                    {
                        switch (text)
                        {
                            case "AI":
                                value = 950000000;
                                break;
                            case "HB":
                                value = 950000002;
                                break;
                            case "FB":
                                value = 950000001;
                                break;
                        }
                    }
                    break;
                case "tc_emailaddress1_type":
                case "tc_emailaddress2_type":
                case "tc_emailaddress3_type":
                    {
                        switch (text)
                        {
                            case "Pri":
                                value = 950000000;
                                break;
                            case "Pro":
                                value = 950000001;
                                break;

                        }
                    }
                    break;
                case "tc_telephone1_type":
                case "tc_telephone2_type":
                case "tc_telephone3_type":
                    {
                        switch (text)
                        {
                            case "H":
                                value = 950000001;
                                break;
                            case "M":
                                value = 950000000;
                                break;

                        }
                    }
                    break;
                case "statuscode":
                    {
                        switch (text)
                        {
                            case "A":
                                value = 1;
                                break;
                            case "B":
                                value = 950000001;
                                break;
                            case "C":
                                value = 950000000;
                                break;
                            case "D":
                                value = 950000000;
                                break;
                            case "Inactive":
                                value = 2;
                                break;

                        }
                    }
                    break;
            }
            return (value != -1) ? new OptionSetValue(value) : null;
        }

        enum Statecode
        {
            Active = 0,
            InActive = 1
        };

    }

    


    public class BookingResponse
    {
        public bool Created { get; set; }

        public string Id { get; set; }

        public bool Success { get; set; }

        public string ErrorMessage { get; set; }
    }
}
