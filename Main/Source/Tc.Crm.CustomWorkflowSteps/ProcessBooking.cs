using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using Microsoft.Xrm.Sdk;


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
            string response = string.Empty;

            payloadBooking.BookingInfo = DeSerializeJson(json);
            List<XrmResponse> successMsg = new List<XrmResponse>();

            //Need to update this condition based on customer type
            if (payloadBooking.BookingInfo.Customer.CustomerGeneral.CustomerType.ToString() == "")
            {
                ProcessAccount();
                payloadBooking.IsCustomerTypeAccount = true;
            }
            else if (payloadBooking.BookingInfo.Customer.CustomerGeneral.CustomerType.ToString() == "")
            {
                ProcessContact();
                payloadBooking.IsCustomerTypeAccount = false;
            }

            ProcessBookingInfo();

            ProcessAccomodation();

            ProcessTransport();




            response = SerializeJson(payloadBooking.Response);
            return response;
        }

        /// <summary>
        /// To serialize object to json
        /// </summary>
        /// <param name="successMsg"></param>
        /// <returns></returns>
        public string SerializeJson(BookingResponse response)
        {
            var jsonSerializer = new JavaScriptSerializer();
            return jsonSerializer.Serialize(response);
        }


        /// <summary>
        /// To deserialize json to object
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        Booking DeSerializeJson(string json)
        {
            var jsonSerializer = new JavaScriptSerializer();
            var bookingInfo = jsonSerializer.Deserialize<Booking>(json);
            return bookingInfo;
        }

        /// <summary>
        /// To process contact information
        /// </summary>       
        /// <returns></returns>
        XrmResponse ProcessContact()
        {
            XrmResponse sucMsg = null;
            if (payloadBooking.BookingInfo.Customer != null)
            {

                Entity contactEntity = new Entity(EntityName.Contact, Attributes.Contact.SourceSystemID, payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId);

                contactEntity[Attributes.Contact.FirstName] = payloadBooking.BookingInfo.Customer.CustomerIdentity.FirstName;
                contactEntity[Attributes.Contact.MiddleName] = payloadBooking.BookingInfo.Customer.CustomerIdentity.MiddleName;
                contactEntity[Attributes.Contact.LastName] = payloadBooking.BookingInfo.Customer.CustomerIdentity.LastName;
                contactEntity[Attributes.Contact.Language] = payloadBooking.BookingInfo.Customer.CustomerIdentity.Language;
                contactEntity[Attributes.Contact.Gender] = payloadBooking.BookingInfo.Customer.CustomerIdentity.Gender;
                contactEntity[Attributes.Contact.AcademicTitle] = payloadBooking.BookingInfo.Customer.CustomerIdentity.AcademicTitle;
                contactEntity[Attributes.Contact.Salutation] = payloadBooking.BookingInfo.Customer.CustomerIdentity.Salutation;
                contactEntity[Attributes.Contact.BirthDate] = payloadBooking.BookingInfo.Customer.CustomerIdentity.Birthdate;
                contactEntity[Attributes.Contact.StatusCode] = payloadBooking.BookingInfo.Customer.CustomerGeneral.CustomerStatus;
                // couldn't find segment,DateofDeath in booking.customer.identity.additional
                contactEntity[Attributes.Contact.Segment] = payloadBooking.BookingInfo.Customer.Additional.Segment;
                contactEntity[Attributes.Contact.DateofDeath] = payloadBooking.BookingInfo.Customer.Additional.DateOfDeath;
                if (payloadBooking.BookingInfo.Customer.Address != null && payloadBooking.BookingInfo.Customer.Address.Length > 0)
                {
                    contactEntity[Attributes.Contact.Address1_AdditionalInformation] = payloadBooking.BookingInfo.Customer.Address[0].AdditionalAddressInfo;
                    contactEntity[Attributes.Contact.Address1_FlatOrUnitNumber] = payloadBooking.BookingInfo.Customer.Address[0].FlatNumberUnit;
                    contactEntity[Attributes.Contact.Address1_HouseNumberOrBuilding] = payloadBooking.BookingInfo.Customer.Address[0].HouseNumberBuilding;
                    contactEntity[Attributes.Contact.Address1_Town] = payloadBooking.BookingInfo.Customer.Address[0].Town;
                    contactEntity[Attributes.Contact.Address1_CountryId] = payloadBooking.BookingInfo.Customer.Address[0].Country;
                    contactEntity[Attributes.Contact.Address1_County] = payloadBooking.BookingInfo.Customer.Address[0].County;
                    contactEntity[Attributes.Contact.Address1_PostalCode] = payloadBooking.BookingInfo.Customer.Address[0].PostalCode;
                    contactEntity[Attributes.Contact.Address2_AdditionalInformation] = payloadBooking.BookingInfo.Customer.Address[1].AdditionalAddressInfo;
                    contactEntity[Attributes.Contact.Address2_FlatOrUnitNumber] = payloadBooking.BookingInfo.Customer.Address[1].FlatNumberUnit;
                    contactEntity[Attributes.Contact.Address2_HouseNumberorBuilding] = payloadBooking.BookingInfo.Customer.Address[1].HouseNumberBuilding;
                    contactEntity[Attributes.Contact.Address2_Town] = payloadBooking.BookingInfo.Customer.Address[1].Town;
                    contactEntity[Attributes.Contact.Address2_CountryId] = payloadBooking.BookingInfo.Customer.Address[1].Country;
                    contactEntity[Attributes.Contact.Address2_County] = payloadBooking.BookingInfo.Customer.Address[1].County;
                    contactEntity[Attributes.Contact.Address2_PostalCode] = payloadBooking.BookingInfo.Customer.Address[1].PostalCode;
                }
                if (payloadBooking.BookingInfo.Customer.Phone != null && payloadBooking.BookingInfo.Customer.Phone.Length > 0)
                {
                    contactEntity[Attributes.Contact.Telephone2Type] = payloadBooking.BookingInfo.Customer.Phone[0].PhoneType;
                    contactEntity[Attributes.Contact.Telephone1] = payloadBooking.BookingInfo.Customer.Phone[0].Number;
                    contactEntity[Attributes.Contact.Telephone2Type] = payloadBooking.BookingInfo.Customer.Phone[1].PhoneType;
                    contactEntity[Attributes.Contact.Telephone2] = payloadBooking.BookingInfo.Customer.Phone[1].Number;
                    contactEntity[Attributes.Contact.Telephone3Type] = payloadBooking.BookingInfo.Customer.Phone[2].PhoneType;
                    contactEntity[Attributes.Contact.Telephone3] = payloadBooking.BookingInfo.Customer.Phone[2].Number;
                }
                if (payloadBooking.BookingInfo.Customer.Email != null && payloadBooking.BookingInfo.Customer.Email.Length > 0)
                {
                    contactEntity[Attributes.Contact.EMailAddress1] = payloadBooking.BookingInfo.Customer.Email[0].Address;
                    contactEntity[Attributes.Contact.EmailAddress1Type] = payloadBooking.BookingInfo.Customer.Email[0].EmailType;
                    contactEntity[Attributes.Contact.EMailAddress2] = payloadBooking.BookingInfo.Customer.Email[1].Address;
                    contactEntity[Attributes.Contact.EmailAddress2Type] = payloadBooking.BookingInfo.Customer.Email[1].EmailType;
                    contactEntity[Attributes.Contact.EMailAddress3] = payloadBooking.BookingInfo.Customer.Email[2].Address;
                    contactEntity[Attributes.Contact.EmailAddress3Type] = payloadBooking.BookingInfo.Customer.Email[2].EmailType;
                }
                contactEntity[Attributes.Contact.SourceMarketId] = payloadBooking.BookingInfo.Customer.CustomerIdentifier.SourceMarket;
                contactEntity[Attributes.Contact.SourceSystemID] = payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId;
                sucMsg = CommonXrm.UpsertEntity(contactEntity, payloadBooking.CrmService);

                if (sucMsg.Create)
                    payloadBooking.DeleteBookingRole = false;

                payloadBooking.CustomerId = sucMsg.Id;
            }

            return sucMsg;
        }

        /// <summary>
        /// To process account information
        /// </summary>        /
        /// <returns></returns>
        XrmResponse ProcessAccount()
        {
            XrmResponse sucMsg = null;

            if (payloadBooking.BookingInfo.Customer != null)
            {
                Entity accountEntity = new Entity(EntityName.Account, Attributes.Account.SourceSystemID, payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId);
                accountEntity[Attributes.Account.Name] = payloadBooking.BookingInfo.Customer.Company.CompanyName;
                if (payloadBooking.BookingInfo.Customer.Address != null && payloadBooking.BookingInfo.Customer.Address.Length > 0)
                {
                    accountEntity[Attributes.Account.Address1_AdditionalInformation] = payloadBooking.BookingInfo.Customer.Address[0].AdditionalAddressInfo;
                    accountEntity[Attributes.Account.Address1_FlatOrUnitNumber] = payloadBooking.BookingInfo.Customer.Address[0].FlatNumberUnit;
                    accountEntity[Attributes.Account.Address1_HouseNumberOrBuilding] = payloadBooking.BookingInfo.Customer.Address[0].HouseNumberBuilding;
                    accountEntity[Attributes.Account.Address1_Town] = payloadBooking.BookingInfo.Customer.Address[0].Town;
                    accountEntity[Attributes.Account.Address1_PostalCode] = payloadBooking.BookingInfo.Customer.Address[0].PostalCode;
                    //accountEntity[Attributes.Account.Address1_CountryId] = payloadBooking.customer.Address[0].Country;
                    accountEntity[Attributes.Account.Address1_County] = payloadBooking.BookingInfo.Customer.Address[0].County;
                }
                if (payloadBooking.BookingInfo.Customer.Phone != null && payloadBooking.BookingInfo.Customer.Phone.Length > 0)
                {
                    accountEntity[Attributes.Account.Telephone1_Type] = payloadBooking.BookingInfo.Customer.Phone[0].PhoneType;
                    accountEntity[Attributes.Account.Telephone1] = payloadBooking.BookingInfo.Customer.Phone[0].Number;
                    if (payloadBooking.BookingInfo.Customer.Phone.Length > 1)
                    {
                        accountEntity[Attributes.Account.Telephone2_Type] = payloadBooking.BookingInfo.Customer.Phone[1].PhoneType;
                        accountEntity[Attributes.Account.Telephone2] = payloadBooking.BookingInfo.Customer.Phone[1].Number;
                    }
                    if (payloadBooking.BookingInfo.Customer.Phone.Length > 2)
                    {
                        accountEntity[Attributes.Account.Telephone3_Type] = payloadBooking.BookingInfo.Customer.Phone[2].PhoneType;
                        accountEntity[Attributes.Account.Telephone3] = payloadBooking.BookingInfo.Customer.Phone[2].Number;
                    }
                }
                if (payloadBooking.BookingInfo.Customer.Email != null && payloadBooking.BookingInfo.Customer.Email.Length > 0)
                {
                    accountEntity[Attributes.Account.EMailAddress1] = payloadBooking.BookingInfo.Customer.Email[0].Address;
                    accountEntity[Attributes.Account.EmailAddress1_Type] = payloadBooking.BookingInfo.Customer.Email[0].EmailType;
                    if (payloadBooking.BookingInfo.Customer.Email.Length > 1)
                    {
                        accountEntity[Attributes.Account.EMailAddress2] = payloadBooking.BookingInfo.Customer.Email[1].Address;
                        accountEntity[Attributes.Account.EmailAddress2_Type] = payloadBooking.BookingInfo.Customer.Email[1].EmailType;
                    }
                    if (payloadBooking.BookingInfo.Customer.Email.Length > 2)
                    {
                        accountEntity[Attributes.Account.EMailAddress3] = payloadBooking.BookingInfo.Customer.Email[2].Address;
                        accountEntity[Attributes.Account.EmailAddress3_Type] = payloadBooking.BookingInfo.Customer.Email[2].EmailType;
                    }
                }
                accountEntity[Attributes.Account.SourceMarketId] = payloadBooking.BookingInfo.Customer.CustomerIdentifier.SourceMarket;
                accountEntity[Attributes.Account.SourceSystemID] = payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId;


                sucMsg = CommonXrm.UpsertEntity(accountEntity, payloadBooking.CrmService);
                if (sucMsg.Create)
                    payloadBooking.DeleteBookingRole = false;

                payloadBooking.CustomerId = sucMsg.Id;
            }
            //sucMsg.Key = payloadBooking.CustomerInfo.CustomerIdentifier.SourceSystem;

            return sucMsg;
        }

        /// <summary>
        /// To process booking information
        /// </summary>      
        /// <returns></returns>
        XrmResponse ProcessBookingInfo()
        {
            XrmResponse xrmResp = null;
            if (payloadBooking.BookingInfo.BookingIdentifier != null)
            {

                Entity bookingEntity = new Entity(EntityName.Booking, Attributes.Booking.Name, payloadBooking.BookingInfo.BookingIdentifier.BookingNumber);

                bookingEntity[Attributes.Booking.Name] = payloadBooking.BookingInfo.BookingIdentifier.BookingNumber;
                bookingEntity[Attributes.Booking.OnTourVersion] = payloadBooking.BookingInfo.BookingIdentifier.BookingVersionOnTour;
                bookingEntity[Attributes.Booking.TourOperatorVersion] = payloadBooking.BookingInfo.BookingIdentifier.BookingVersionTourOperator;
                bookingEntity[Attributes.Booking.OnTourUpdatedDate] = payloadBooking.BookingInfo.BookingIdentifier.BookingUpdateDateOnTour;
                bookingEntity[Attributes.Booking.TourOperatorUpdatedDate] = payloadBooking.BookingInfo.BookingIdentifier.BookingUpdateDateTourOperator;
                bookingEntity[Attributes.Booking.BookingDate] = payloadBooking.BookingInfo.BookingGeneral.BookingDate;
                bookingEntity[Attributes.Booking.DepartureDate] = payloadBooking.BookingInfo.BookingGeneral.DepartureDate;
                bookingEntity[Attributes.Booking.ReturnDate] = payloadBooking.BookingInfo.BookingGeneral.ReturnDate;
                bookingEntity[Attributes.Booking.Duration] = payloadBooking.BookingInfo.BookingGeneral.Duration;
                bookingEntity[Attributes.Booking.DestinationGatewayId] = new EntityReference(EntityName.Gateway, Attributes.Gateway.IATA, payloadBooking.BookingInfo.BookingGeneral.Destination);
                bookingEntity[Attributes.Booking.TourOperatorId] = new EntityReference(EntityName.TourOperator, Attributes.TourOperator.TourOperatorCode, payloadBooking.BookingInfo.BookingGeneral.ToCode);
                bookingEntity[Attributes.Booking.BrandId] = new EntityReference(EntityName.Brand, Attributes.Brand.BrandCode, payloadBooking.BookingInfo.BookingGeneral.Brand);
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
                //bookingEntity[Attributes.Booking.SourceMarketId] = new EntityReference(EntityName.Country, Attributes.Country., payloadBooking.bookingIdentifier.SourceMarket);
                bookingEntity[Attributes.Booking.Statuscode] = payloadBooking.BookingInfo.BookingGeneral.BookingStatus;
                bookingEntity[Attributes.Booking.TravelAmount] = payloadBooking.BookingInfo.BookingGeneral.TravelAmount;
                bookingEntity[Attributes.Booking.TransactionCurrencyId] = payloadBooking.BookingInfo.BookingGeneral.Currency;
                bookingEntity[Attributes.Booking.HasSourceMarketComplaint] = payloadBooking.BookingInfo.BookingGeneral.HasComplaint;
                bookingEntity[Attributes.Booking.BookerEmail] = payloadBooking.BookingInfo.BookingIdentity.Booker.Email;
                xrmResp = CommonXrm.UpsertEntity(bookingEntity,payloadBooking.CrmService);

                if (xrmResp.Create)
                {
                    payloadBooking.DeleteBookingRole = false;
                    payloadBooking.DeleteAccomdOrTrnsprt = false;
                    payloadBooking.Response.Created = true;
                    
                }

                payloadBooking.BookingId = xrmResp.Id;
                payloadBooking.Response.Success = true;
                payloadBooking.Response.Id = xrmResp.Id;
                
            }
            return xrmResp;
        }

        /// <summary>
        /// To prepare travel participants information
        /// </summary>        
        /// <returns></returns>
        string PrepareTravelParticipantsInfo()
        {
            string travelParticipants = string.Empty;
            if (payloadBooking.BookingInfo.TravelParticipant != null)
                for (int i = 0; i < payloadBooking.BookingInfo.TravelParticipant.Length; i++)
                {
                    if (!string.IsNullOrEmpty(travelParticipants))
                        travelParticipants += payloadBooking.NextLine;

                    travelParticipants += payloadBooking.BookingInfo.TravelParticipant[i].TravelParticipantIdOnTour + payloadBooking.Seperator +
                                          payloadBooking.BookingInfo.TravelParticipant[i].FirstName + payloadBooking.Seperator +
                                          payloadBooking.BookingInfo.TravelParticipant[i].LastName + payloadBooking.Seperator +
                                          payloadBooking.BookingInfo.TravelParticipant[i].Age + payloadBooking.Seperator +
                                          payloadBooking.BookingInfo.TravelParticipant[i].Birthdate + payloadBooking.Seperator +
                                          payloadBooking.BookingInfo.TravelParticipant[i].Gender + payloadBooking.Seperator +
                                          payloadBooking.BookingInfo.TravelParticipant[i].Relation + payloadBooking.Seperator +
                                          payloadBooking.BookingInfo.TravelParticipant[i].Language;

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
            if (payloadBooking.BookingInfo.TravelParticipant  != null)
                for (int i = 0; i < payloadBooking.BookingInfo.TravelParticipant.Length; i++)
                {
                    if (payloadBooking.BookingInfo.TravelParticipant[i].Remark != null)
                        for (int j = 0; j < payloadBooking.BookingInfo.TravelParticipant[i].Remark.Length; j++)
                        {
                            if (!string.IsNullOrEmpty(remarks))
                                remarks += payloadBooking.NextLine;

                            remarks += payloadBooking.BookingInfo.TravelParticipant[i].TravelParticipantIdOnTour + payloadBooking.Seperator +
                                       payloadBooking.BookingInfo.TravelParticipant[i].Remark[j].RemarkType + payloadBooking.Seperator +
                                       payloadBooking.BookingInfo.TravelParticipant[i].Remark[j].Text;
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
                for (int i = 0; i < payloadBooking.BookingInfo.Services.Transfer.Length; i++)
                {
                    if (!string.IsNullOrEmpty(transferInfo))
                        transferInfo += payloadBooking.NextLine;

                    transferInfo += payloadBooking.BookingInfo.Services.Transfer[i].TransferCode + payloadBooking.Seperator +
                                    payloadBooking.BookingInfo.Services.Transfer[i].TransferDescription + payloadBooking.Seperator +
                                    payloadBooking.BookingInfo.Services.Transfer[i].Order + payloadBooking.Seperator +
                                    payloadBooking.BookingInfo.Services.Transfer[i].StartDate + payloadBooking.Seperator +
                                    payloadBooking.BookingInfo.Services.Transfer[i].EndDate + payloadBooking.Seperator +
                                    payloadBooking.BookingInfo.Services.Transfer[i].Category + payloadBooking.Seperator +
                                    payloadBooking.BookingInfo.Services.Transfer[i].TransferType + payloadBooking.Seperator +
                                    payloadBooking.BookingInfo.Services.Transfer[i].DepartureAirport + payloadBooking.Seperator +
                                    payloadBooking.BookingInfo.Services.Transfer[i].ArrivalAirport;


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
                for (int i = 0; i < payloadBooking.BookingInfo.Services.Transfer.Length; i++)
                {
                    if (payloadBooking.BookingInfo.Services.Transfer[i].Remark != null)
                        for (int j = 0; j < payloadBooking.BookingInfo.Services.Transfer[i].Remark.Length; j++)
                        {
                            if (!string.IsNullOrEmpty(remarks))
                                remarks += payloadBooking.NextLine;

                            remarks += payloadBooking.BookingInfo.Services.Transfer[i].Remark[j].RemarkType + payloadBooking.Seperator +
                                       payloadBooking.BookingInfo.Services.Transfer[i].Remark[j].Text;
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
                for (int i = 0; i < payloadBooking.BookingInfo.Services.ExtraService.Length; i++)
                {

                    if (!string.IsNullOrEmpty(extraServices))
                        extraServices += payloadBooking.NextLine;

                    extraServices += payloadBooking.BookingInfo.Services.ExtraService[i].ExtraServiceCode + payloadBooking.Seperator +
                                     payloadBooking.BookingInfo.Services.ExtraService[i].ExtraServiceDescription + payloadBooking.Seperator +
                                     payloadBooking.BookingInfo.Services.ExtraService[i].Order + payloadBooking.Seperator +
                                     payloadBooking.BookingInfo.Services.ExtraService[i].StartDate + payloadBooking.Seperator +
                                     payloadBooking.BookingInfo.Services.ExtraService[i].EndDate;


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
                for (int i = 0; i < payloadBooking.BookingInfo.Services.ExtraService.Length; i++)
                {
                    if (payloadBooking.BookingInfo.Services.ExtraService[i].Remark != null)
                        for (int j = 0; j < payloadBooking.BookingInfo.Services.ExtraService[i].Remark.Length; j++)
                        {
                            if (!string.IsNullOrEmpty(extraServiceRemarks))
                                extraServiceRemarks += payloadBooking.NextLine;

                            extraServiceRemarks += payloadBooking.BookingInfo.Services.ExtraService[i].Remark[j].RemarkType + payloadBooking.Seperator +
                                             payloadBooking.BookingInfo.Services.ExtraService[i].Remark[j].Text;

                        }

                }
            return extraServiceRemarks;
        }


        /// <summary>
        /// 
        /// </summary>
        void ProcessRemarks()
        {
            if (payloadBooking.DeleteAccomdOrTrnsprt)
            {
                ProcessRecordsToDelete(EntityName.Remark,
                    new string[] { Attributes.Remark.RemarkId },
                    new string[] { Attributes.BookingAccommodation.BookingId },
                    new string[] { "" });

            }

            if (payloadBooking.BookingInfo.Remark != null)
            {
                EntityCollection entColRemark = new EntityCollection();
                Entity remarkEntity = null;
                for (int i = 0; i < payloadBooking.BookingInfo.Remark.Length; i++)
                {
                    remarkEntity = new Entity(EntityName.Remark, "", "");
                    remarkEntity[Attributes.Remark.Type] = payloadBooking.BookingInfo.Remark[i].RemarkType;
                    remarkEntity[Attributes.Remark.RemarkName] = payloadBooking.BookingInfo.Remark[i].Text;
                    entColRemark.Entities.Add(remarkEntity);

                }
                CommonXrm.BulkCreate(entColRemark, payloadBooking.CrmService);
            }
        }


        /// <summary>
        /// To process accomodation informaation
        /// </summary>      
        /// <returns></returns>
        void ProcessAccomodation()
        {
            if (payloadBooking.DeleteAccomdOrTrnsprt)
            {
                ProcessRecordsToDelete(EntityName.BookingAccommodation,
                    new string[] { Attributes.BookingAccommodation.BookingAccommodationid },
                    new string[] { Attributes.BookingAccommodation.BookingId },
                    new string[] { payloadBooking.BookingId });

            }

            if (payloadBooking.BookingInfo.Services != null && payloadBooking.BookingInfo.Services.Accommodation != null)
            {
                EntityCollection entColAccomodation = new EntityCollection();
                Entity accomodationEntity = null;
                for (int i = 0; i < payloadBooking.BookingInfo.Services.Accommodation.Length; i++)
                {
                    accomodationEntity = new Entity(EntityName.BookingAccommodation);
                    accomodationEntity[Attributes.BookingAccommodation.HotelId] = new EntityReference(EntityName.Hotel, Attributes.Hotel.MasterHotelID, payloadBooking.BookingInfo.Services.Accommodation[i].GroupAccommodationCode);
                    accomodationEntity[Attributes.BookingAccommodation.Order] = payloadBooking.BookingInfo.Services.Accommodation[i].Order;
                    accomodationEntity[Attributes.BookingAccommodation.StartDateandTime] = payloadBooking.BookingInfo.Services.Accommodation[i].StartDate;
                    accomodationEntity[Attributes.BookingAccommodation.EndDateandTime] = payloadBooking.BookingInfo.Services.Accommodation[i].EndDate;
                    accomodationEntity[Attributes.BookingAccommodation.RoomType] = payloadBooking.BookingInfo.Services.Accommodation[i].RoomType;
                    accomodationEntity[Attributes.BookingAccommodation.BoardType] = payloadBooking.BookingInfo.Services.Accommodation[i].BoardType;
                    accomodationEntity[Attributes.BookingAccommodation.HasSharedRoom] = payloadBooking.BookingInfo.Services.Accommodation[i].HasSharedRoom;
                    accomodationEntity[Attributes.BookingAccommodation.NumberofParticipants] = payloadBooking.BookingInfo.Services.Accommodation[i].NumberOfParticipants;
                    accomodationEntity[Attributes.BookingAccommodation.NumberofRooms] = payloadBooking.BookingInfo.Services.Accommodation[i].NumberOfRooms;
                    accomodationEntity[Attributes.BookingAccommodation.WithTransfer] = payloadBooking.BookingInfo.Services.Accommodation[i].WithTransfer;
                    accomodationEntity[Attributes.BookingAccommodation.IsExternalService] = payloadBooking.BookingInfo.Services.Accommodation[i].IsExternalService;
                    accomodationEntity[Attributes.BookingAccommodation.ExternalServiceCode] = payloadBooking.BookingInfo.Services.Accommodation[i].ExternalServiceCode;
                    accomodationEntity[Attributes.BookingAccommodation.NotificationRequired] = payloadBooking.BookingInfo.Services.Accommodation[i].NotificationRequired;
                    accomodationEntity[Attributes.BookingAccommodation.NeedTourGuideAssignment] = payloadBooking.BookingInfo.Services.Accommodation[i].NeedsTourGuideAssignment;
                    accomodationEntity[Attributes.BookingAccommodation.ExternalTransfer] = payloadBooking.BookingInfo.Services.Accommodation[i].IsExternalTransfer;
                    accomodationEntity[Attributes.BookingAccommodation.TransferServiceLevel] = payloadBooking.BookingInfo.Services.Accommodation[i].TransferServiceLevel;
                    accomodationEntity[Attributes.BookingAccommodation.SourceMarketHotelName] = payloadBooking.BookingInfo.Services.Accommodation[i].AccommodationDescription;

                    accomodationEntity[Attributes.BookingAccommodation.BookingId] = new EntityReference(EntityName.Booking, new Guid(payloadBooking.BookingId));

                    entColAccomodation.Entities.Add(accomodationEntity);

                }

                List<XrmResponse> listXrmResp = CommonXrm.BulkCreate(entColAccomodation, payloadBooking.CrmService);
                ProcessAccomodationRemarks(listXrmResp);
            }
        }

        /// <summary>
        /// To process accomodation remarks
        /// </summary>       
        /// <param name="listXrmResp"></param>
        void ProcessAccomodationRemarks(List<XrmResponse> listXrmResp)
        {
            EntityCollection entColAccomodationRemarks = new EntityCollection();
            Entity remarkEntity = null;
            if (payloadBooking.BookingInfo.Services != null && payloadBooking.BookingInfo.Services.Accommodation != null)
            {
                for (int i = 0; i < payloadBooking.BookingInfo.Services.Accommodation.Length; i++)
                {
                    if (payloadBooking.BookingInfo.Services.Accommodation[i].Remark != null)
                    {
                        for (int j = 0; j < payloadBooking.BookingInfo.Services.Accommodation[i].Remark.Length; j++)
                        {
                            remarkEntity = new Entity(EntityName.Remark);
                            remarkEntity[Attributes.Remark.Type] = payloadBooking.BookingInfo.Services.Accommodation[i].Remark[j].RemarkType;
                            remarkEntity[Attributes.Remark.RemarkName] = payloadBooking.BookingInfo.Services.Accommodation[i].Remark[j].Text;
                            //messageList.Find()
                            remarkEntity[Attributes.Remark.BookingAccommodationId] = new EntityReference(EntityName.BookingAccommodation, Guid.Parse(listXrmResp[i].Id));
                            entColAccomodationRemarks.Entities.Add(remarkEntity);
                        }
                    }
                }

                if (entColAccomodationRemarks.Entities.Count > 0)
                    CommonXrm.BulkCreate(entColAccomodationRemarks, payloadBooking.CrmService);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void ProcessTransport()
        {
            EntityCollection entColTransport = new EntityCollection();
            Entity transportEntity = null;

            if (payloadBooking.DeleteAccomdOrTrnsprt)
            {
                ProcessRecordsToDelete(EntityName.BookingTransport,
                    new string[] { Attributes.BookingTransport.BookingTransportId },
                    new string[] { Attributes.BookingAccommodation.BookingId },
                    new string[] { payloadBooking.BookingId });

            }

            for (int i = 0; i < payloadBooking.BookingInfo.Services.Transport.Length; i++)
            {
                transportEntity = new Entity(EntityName.BookingTransport);
                transportEntity[Attributes.BookingTransport.TransportCode] = payloadBooking.BookingInfo.Services.Transport[i].TransportCode;
                transportEntity[Attributes.BookingTransport.Description] = payloadBooking.BookingInfo.Services.Transport[i].TransportDescription;
                transportEntity[Attributes.BookingTransport.Order] = payloadBooking.BookingInfo.Services.Transport[i].Order;
                transportEntity[Attributes.BookingTransport.StartDateandTime] = payloadBooking.BookingInfo.Services.Transport[i].StartDate;
                transportEntity[Attributes.BookingTransport.EndDateandTime] = payloadBooking.BookingInfo.Services.Transport[i].EndDate;
                transportEntity[Attributes.BookingTransport.TransferType] = payloadBooking.BookingInfo.Services.Transport[i].TransferType;
                //transportEntity[Attributes.BookingTransport.DepartureGatewayId] = new EntityReference(EntityName.Gateway, "", payloadBooking.BookingInfo.Services.Transport[i].DepartureAirport);
                //transportEntity[Attributes.BookingTransport.ArrivalGatewayId] = new EntityReference(EntityName.Gateway, "", payloadBooking.BookingInfo.Services.Transport[i].ArrivalAirport);
                transportEntity[Attributes.BookingTransport.CarrierCode] = payloadBooking.BookingInfo.Services.Transport[i].CarrierCode;
                transportEntity[Attributes.BookingTransport.FlightNumber] = payloadBooking.BookingInfo.Services.Transport[i].FlightNumber;
                transportEntity[Attributes.BookingTransport.FlightIdentifier] = payloadBooking.BookingInfo.Services.Transport[i].FlightIdentifier;
                transportEntity[Attributes.BookingTransport.NumberofParticipants] = payloadBooking.BookingInfo.Services.Transport[i].NumberOfParticipants;
                transportEntity[Attributes.BookingTransport.FlightNumber] = payloadBooking.BookingInfo.Services.Transport[i].FlightNumber;

                transportEntity[Attributes.BookingTransport.BookingId] = new EntityReference(EntityName.Booking, new Guid(payloadBooking.BookingId));

                entColTransport.Entities.Add(transportEntity);


            }

            List<XrmResponse> listXrmResp = CommonXrm.BulkCreate(entColTransport, payloadBooking.CrmService);
            ProcessTransportRemarks(listXrmResp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listXrmResp"></param>
        void ProcessTransportRemarks(List<XrmResponse> listXrmResp)
        {

            EntityCollection entColTransportRemark = new EntityCollection();
            Entity TransportRemark = null;


            for (int i = 0; i < payloadBooking.BookingInfo.Services.Transport.Length; i++)
            {
                for (int j = 0; j < payloadBooking.BookingInfo.Services.Transport[i].Remark.Length; j++)
                {
                    TransportRemark = new Entity(EntityName.BookingTransport);
                    TransportRemark[Attributes.Remark.Type] = payloadBooking.BookingInfo.Services.Transport[i].Remark[j].RemarkType;
                    TransportRemark[Attributes.Remark.RemarkName] = payloadBooking.BookingInfo.Services.Transport[i].Remark[j].Text;
                    TransportRemark[Attributes.Remark.BookingTransportId] = new EntityReference(EntityName.BookingTransport, new Guid(listXrmResp[i].Id));

                    entColTransportRemark.Entities.Add(TransportRemark);
                }


            }

            if (entColTransportRemark.Entities.Count > 0)
                CommonXrm.BulkCreate(entColTransportRemark, payloadBooking.CrmService);

        }

        /// <summary>
        /// 
        /// </summary>
        void ProcessBookingRole()
        {
            if (payloadBooking.DeleteBookingRole)
            {
                ProcessRecordsToDelete(EntityName.CustomerBookingRole,
                    new string[] { Attributes.CustomerBookingRole.CustomerBookingRoleId },
                    new string[] { Attributes.CustomerBookingRole.BookingId, Attributes.CustomerBookingRole.Customer },
                    new string[] { payloadBooking.BookingId, payloadBooking.CustomerId });

            }

            Entity entCustBookingRole = new Entity(EntityName.CustomerBookingRole);
            entCustBookingRole[Attributes.CustomerBookingRole.BookingId] = new EntityReference(EntityName.Booking, Attributes.Booking.Name, payloadBooking.BookingInfo.BookingIdentifier.BookingNumber);
            if (payloadBooking.IsCustomerTypeAccount)
            {
                entCustBookingRole[Attributes.CustomerBookingRole.Customer] = new EntityReference(EntityName.Account, Attributes.Account.SourceSystemID, payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId);
            }
            else
            {
                entCustBookingRole[Attributes.CustomerBookingRole.Customer] = new EntityReference(EntityName.Contact, Attributes.Contact.SourceSystemID, payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId);
            }

            EntityCollection entCol = new EntityCollection();
            entCol.Entities.Add(entCustBookingRole);

            CommonXrm.BulkCreate(entCol, payloadBooking.CrmService);
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
            EntityCollection entCollection = CommonXrm.RetrieveMultipleRecords(entityName, columns, filterKeys, filterValues, payloadBooking.CrmService);
            if (entCollection != null && entCollection.Entities.Count > 0)
            {
                EntityReferenceCollection entRefCollection = new EntityReferenceCollection();
                foreach (Entity ent in entCollection.Entities)
                {
                    entRefCollection.Add(new EntityReference(ent.LogicalName, ent.Id));
                }
                CommonXrm.BulkDelete(entRefCollection, payloadBooking.CrmService);
            }
        }



    }


    public class BookingResponse
    {
       
        public bool Created { get; set; }
       
        public string Id { get; set; }
      
        public bool Success { get; set; }
       
        public string ErrorMessage { get; set; }
    }
}
