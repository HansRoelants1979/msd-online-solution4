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

        /// <summary>
        /// To process booking data
        /// </summary>
        /// <param name="json"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public string ProcessPayload(string json, IOrganizationService service)
        {
            string response = string.Empty;

            GlobalVariables globalVariable = new GlobalVariables();
            globalVariable.service = service;
            globalVariable.xrm = new CommonXrm();


            PayloadBooking payloadInfo = DeSerializeJson(json);
            List<SuccessMessage> successMsg = new List<SuccessMessage>();

            //Need to update this condition based on customer type
            if (payloadInfo.CustomerInfo.CustomerGeneral.CustomerType.ToString() == PayloadBooking.AccountType)
            {
                ProcessAccount(payloadInfo, globalVariable);
                globalVariable.IsCustomerTypeAccount = true;
            }
            else if (payloadInfo.CustomerInfo.CustomerGeneral.CustomerType.ToString() == PayloadBooking.ContactType)
            {
                ProcessContact(payloadInfo, globalVariable);
                globalVariable.IsCustomerTypeAccount = false;
            }

            ProcessBookingInfo(payloadInfo, globalVariable);

            ProcessAccomodation(payloadInfo, globalVariable);

            ProcessTransport(payloadInfo, globalVariable);

            ProcessAccomodationRemarks(payloadInfo, globalVariable);

            ProcessTransportRemarks(payloadInfo, globalVariable);


            response = SerializeJson(successMsg);
            return response;
        }

        /// <summary>
        /// To serialize object to json
        /// </summary>
        /// <param name="successMsg"></param>
        /// <returns></returns>
        string SerializeJson(List<SuccessMessage> successMsg)
        {
            var jsonSerializer = new JavaScriptSerializer();
            return jsonSerializer.Serialize(successMsg);
        }


        /// <summary>
        /// To deserialize json to object
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        PayloadBooking DeSerializeJson(string json)
        {
            var jsonSerializer = new JavaScriptSerializer();
            PayloadBooking bookingInfo = jsonSerializer.Deserialize<PayloadBooking>(json);
            return bookingInfo;
        }

        /// <summary>
        /// To process contact information
        /// </summary>
        /// <param name="bookingInfo"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        SuccessMessage ProcessContact(PayloadBooking payloadInfo, GlobalVariables globalVariable)
        {
            SuccessMessage sucMsg = null;
            if (payloadInfo.BookingInfo.Customer != null)
            {

                Entity contactEntity = new Entity(EntityName.Contact, "", "");

                contactEntity[Attributes.Contact.FirstName] = payloadInfo.BookingInfo.Customer.CustomerIdentity.FirstName;
                contactEntity[Attributes.Contact.MiddleName] = payloadInfo.BookingInfo.Customer.CustomerIdentity.MiddleName;
                contactEntity[Attributes.Contact.LastName] = payloadInfo.BookingInfo.Customer.CustomerIdentity.LastName;
                contactEntity[Attributes.Contact.Language] = payloadInfo.BookingInfo.Customer.CustomerIdentity.Language;
                contactEntity[Attributes.Contact.Gender] = payloadInfo.BookingInfo.Customer.CustomerIdentity.Gender;
                contactEntity[Attributes.Contact.AcademicTitle] = payloadInfo.BookingInfo.Customer.CustomerIdentity.AcademicTitle;
                contactEntity[Attributes.Contact.Salutation] = payloadInfo.BookingInfo.Customer.CustomerIdentity.Salutation;
                contactEntity[Attributes.Contact.BirthDate] = payloadInfo.BookingInfo.Customer.CustomerIdentity.Birthdate;
                contactEntity[Attributes.Contact.StatusCode] = payloadInfo.BookingInfo.Customer.CustomerGeneral.CustomerStatus;
                // couldn't find segment,DateofDeath in booking.customer.identity.additional
                contactEntity[Attributes.Contact.Segment] = payloadInfo.BookingInfo.Customer.Additional.Segment;
                contactEntity[Attributes.Contact.DateofDeath] = payloadInfo.BookingInfo.Customer.Additional.DateOfDeath;
                if (payloadInfo.BookingInfo.Customer.Address != null && payloadInfo.BookingInfo.Customer.Address.Length > 0)
                {
                    contactEntity[Attributes.Contact.Address1_AdditionalInformation] = payloadInfo.BookingInfo.Customer.Address[0].AdditionalAddressInfo;
                    contactEntity[Attributes.Contact.Address1_FlatOrUnitNumber] = payloadInfo.BookingInfo.Customer.Address[0].FlatNumberUnit;
                    contactEntity[Attributes.Contact.Address1_HouseNumberOrBuilding] = payloadInfo.BookingInfo.Customer.Address[0].HouseNumberBuilding;
                    contactEntity[Attributes.Contact.Address1_Town] = payloadInfo.BookingInfo.Customer.Address[0].Town;
                    contactEntity[Attributes.Contact.Address1_CountryId] = payloadInfo.BookingInfo.Customer.Address[0].Country;
                    contactEntity[Attributes.Contact.Address1_County] = payloadInfo.BookingInfo.Customer.Address[0].County;
                    contactEntity[Attributes.Contact.Address1_PostalCode] = payloadInfo.BookingInfo.Customer.Address[0].PostalCode;
                    contactEntity[Attributes.Contact.Address2_AdditionalInformation] = payloadInfo.BookingInfo.Customer.Address[1].AdditionalAddressInfo;
                    contactEntity[Attributes.Contact.Address2_FlatOrUnitNumber] = payloadInfo.BookingInfo.Customer.Address[1].FlatNumberUnit;
                    contactEntity[Attributes.Contact.Address2_HouseNumberorBuilding] = payloadInfo.BookingInfo.Customer.Address[1].HouseNumberBuilding;
                    contactEntity[Attributes.Contact.Address2_Town] = payloadInfo.BookingInfo.Customer.Address[1].Town;
                    contactEntity[Attributes.Contact.Address2_CountryId] = payloadInfo.BookingInfo.Customer.Address[1].Country;
                    contactEntity[Attributes.Contact.Address2_County] = payloadInfo.BookingInfo.Customer.Address[1].County;
                    contactEntity[Attributes.Contact.Address2_PostalCode] = payloadInfo.BookingInfo.Customer.Address[1].PostalCode;
                }
                if (payloadInfo.BookingInfo.Customer.Phone != null && payloadInfo.BookingInfo.Customer.Phone.Length > 0)
                {
                    contactEntity[Attributes.Contact.Telephone2Type] = payloadInfo.BookingInfo.Customer.Phone[0].PhoneType;
                    contactEntity[Attributes.Contact.Telephone1] = payloadInfo.BookingInfo.Customer.Phone[0].Number;
                    contactEntity[Attributes.Contact.Telephone2Type] = payloadInfo.BookingInfo.Customer.Phone[1].PhoneType;
                    contactEntity[Attributes.Contact.Telephone2] = payloadInfo.BookingInfo.Customer.Phone[1].Number;
                    contactEntity[Attributes.Contact.Telephone3Type] = payloadInfo.BookingInfo.Customer.Phone[2].PhoneType;
                    contactEntity[Attributes.Contact.Telephone3] = payloadInfo.BookingInfo.Customer.Phone[2].Number;
                }
                if (payloadInfo.BookingInfo.Customer.Email != null && payloadInfo.BookingInfo.Customer.Email.Length > 0)
                {
                    contactEntity[Attributes.Contact.EMailAddress1] = payloadInfo.BookingInfo.Customer.Email[0].Address;
                    contactEntity[Attributes.Contact.EmailAddress1Type] = payloadInfo.BookingInfo.Customer.Email[0].EmailType;
                    contactEntity[Attributes.Contact.EMailAddress2] = payloadInfo.BookingInfo.Customer.Email[1].Address;
                    contactEntity[Attributes.Contact.EmailAddress2Type] = payloadInfo.BookingInfo.Customer.Email[1].EmailType;
                    contactEntity[Attributes.Contact.EMailAddress3] = payloadInfo.BookingInfo.Customer.Email[2].Address;
                    contactEntity[Attributes.Contact.EmailAddress3Type] = payloadInfo.BookingInfo.Customer.Email[2].EmailType;
                }
                contactEntity[Attributes.Contact.SourceMarketId] = payloadInfo.BookingInfo.Customer.CustomerIdentifier.SourceMarket;
                contactEntity[Attributes.Contact.SourceSystemID] = payloadInfo.BookingInfo.Customer.CustomerIdentifier.CustomerId;
                sucMsg = globalVariable.xrm.UpsertEntity(contactEntity, globalVariable.service);

                if (sucMsg.Create)
                    globalVariable.DeleteBookingRole = false;
            }

            return sucMsg;
        }

        /// <summary>
        /// To process account information
        /// </summary>
        /// <param name="bookingInfo"></param>
        /// <returns></returns>
        SuccessMessage ProcessAccount(PayloadBooking payloadInfo, GlobalVariables globalVariable)
        {
            SuccessMessage sucMsg = null;

            if (payloadInfo.BookingInfo.Customer != null)
            {
                Entity accountEntity = new Entity(EntityName.Account, "", "");
                accountEntity[Attributes.Account.Name] = payloadInfo.BookingInfo.Customer.Company;
                if (payloadInfo.BookingInfo.Customer.Address != null && payloadInfo.BookingInfo.Customer.Address.Length > 0)
                {
                    accountEntity[Attributes.Account.Address1_AdditionalInformation] = payloadInfo.BookingInfo.Customer.Address[0].AdditionalAddressInfo;
                    accountEntity[Attributes.Account.Address1_FlatOrUnitNumber] = payloadInfo.BookingInfo.Customer.Address[0].FlatNumberUnit;
                    accountEntity[Attributes.Account.Address1_HouseNumberOrBuilding] = payloadInfo.BookingInfo.Customer.Address[0].HouseNumberBuilding;
                    accountEntity[Attributes.Account.Address1_Town] = payloadInfo.BookingInfo.Customer.Address[0].Town;
                    accountEntity[Attributes.Account.Address1_PostalCode] = payloadInfo.BookingInfo.Customer.Address[0].PostalCode;
                    accountEntity[Attributes.Account.Address1_CountryId] = payloadInfo.BookingInfo.Customer.Address[0].Country;
                    accountEntity[Attributes.Account.Address1_Count] = payloadInfo.BookingInfo.Customer.Address[0].County;
                }
                if (payloadInfo.BookingInfo.Customer.Phone != null && payloadInfo.BookingInfo.Customer.Phone.Length > 0)
                {
                    accountEntity[Attributes.Account.Telephone1_Type] = payloadInfo.BookingInfo.Customer.Phone[0].PhoneType;
                    accountEntity[Attributes.Account.Telephone1] = payloadInfo.BookingInfo.Customer.Phone[0].Number;
                    accountEntity[Attributes.Account.Telephone2_Type] = payloadInfo.BookingInfo.Customer.Phone[1].PhoneType;
                    accountEntity[Attributes.Account.Telephone2] = payloadInfo.BookingInfo.Customer.Phone[1].Number;
                    accountEntity[Attributes.Account.Telephone3_Type] = payloadInfo.BookingInfo.Customer.Phone[2].PhoneType;
                    accountEntity[Attributes.Account.Telephone3] = payloadInfo.BookingInfo.Customer.Phone[2].Number;
                }
                if (payloadInfo.BookingInfo.Customer.Email != null && payloadInfo.BookingInfo.Customer.Email.Length > 0)
                {
                    accountEntity[Attributes.Account.EMailAddress1] = payloadInfo.BookingInfo.Customer.Email[0].Address;
                    accountEntity[Attributes.Account.EmailAddress1_Type] = payloadInfo.BookingInfo.Customer.Email[0].EmailType;
                    accountEntity[Attributes.Account.EMailAddress2] = payloadInfo.BookingInfo.Customer.Email[1].Address;
                    accountEntity[Attributes.Account.EmailAddress2_Type] = payloadInfo.BookingInfo.Customer.Email[1].EmailType;
                    accountEntity[Attributes.Account.EMailAddress3] = payloadInfo.BookingInfo.Customer.Email[2].Address;
                    accountEntity[Attributes.Account.EmailAddress3_Type] = payloadInfo.BookingInfo.Customer.Email[2].EmailType;
                }
                accountEntity[Attributes.Account.SourceMarketId] = payloadInfo.BookingInfo.Customer.CustomerIdentifier.SourceMarket;
                accountEntity[Attributes.Account.SourceSystemID] = payloadInfo.BookingInfo.Customer.CustomerIdentifier.CustomerId;


                sucMsg = globalVariable.xrm.UpsertEntity(accountEntity, globalVariable.service);
                if (sucMsg.Create)
                    globalVariable.DeleteBookingRole = false;
            }
            //sucMsg.Key = payloadInfo.CustomerInfo.CustomerIdentifier.SourceSystem;

            return sucMsg;
        }

        /// <summary>
        /// To process booking information
        /// </summary>
        /// <param name="payloadInfo"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        SuccessMessage ProcessBookingInfo(PayloadBooking payloadInfo, GlobalVariables globalVariable)
        {
            SuccessMessage sucMsg = null;
            if (payloadInfo.BookingInfo.BookingIdentifier != null)
            {

                Entity bookingEntity = new Entity(EntityName.Booking, "", payloadInfo.BookingInfo.BookingIdentifier.SourceSystem);

                bookingEntity[Attributes.Booking.Name] = payloadInfo.BookingInfo.BookingIdentifier.BookingNumber;
                bookingEntity[Attributes.Booking.OnTourVersion] = payloadInfo.BookingInfo.BookingIdentifier.BookingVersionOnTour;
                bookingEntity[Attributes.Booking.TourOperatorVersion] = payloadInfo.BookingInfo.BookingIdentifier.BookingVersionTourOperator;
                bookingEntity[Attributes.Booking.OnTourUpdatedDate] = payloadInfo.BookingInfo.BookingIdentifier.BookingUpdateDateOnTour;
                bookingEntity[Attributes.Booking.TourOperatorUpdatedDate] = payloadInfo.BookingInfo.BookingIdentifier.BookingUpdateDateTourOperator;
                bookingEntity[Attributes.Booking.BookingDate] = payloadInfo.BookingInfo.BookingGeneral.BookingDate;
                bookingEntity[Attributes.Booking.DepartureDate] = payloadInfo.BookingInfo.BookingGeneral.DepartureDate;
                bookingEntity[Attributes.Booking.ReturnDate] = payloadInfo.BookingInfo.BookingGeneral.ReturnDate;
                bookingEntity[Attributes.Booking.Duration] = payloadInfo.BookingInfo.BookingGeneral.Duration;
                bookingEntity[Attributes.Booking.DestinationGatewayId] = new EntityReference(EntityName.Gateway, "", payloadInfo.BookingInfo.BookingGeneral.Destination);
                bookingEntity[Attributes.Booking.TourOperatorId] = new EntityReference(EntityName.TourOperator, "", payloadInfo.BookingInfo.BookingGeneral.ToCode);
                bookingEntity[Attributes.Booking.BrandId] = new EntityReference(EntityName.Brand, "", payloadInfo.BookingInfo.BookingGeneral.Brand);
                bookingEntity[Attributes.Booking.BrochureCode] = payloadInfo.BookingInfo.BookingGeneral.BrochureCode;
                bookingEntity[Attributes.Booking.IsLateBooking] = payloadInfo.BookingInfo.BookingGeneral.IsLateBooking;
                bookingEntity[Attributes.Booking.NumberofParticipants] = payloadInfo.BookingInfo.BookingGeneral.NumberOfParticipants;
                bookingEntity[Attributes.Booking.NumberofAdults] = payloadInfo.BookingInfo.BookingGeneral.NumberOfAdults;
                bookingEntity[Attributes.Booking.NumberofChildren] = payloadInfo.BookingInfo.BookingGeneral.NumberOfChildren;
                bookingEntity[Attributes.Booking.NumberofInfants] = payloadInfo.BookingInfo.BookingGeneral.NumberOfInfants;
                bookingEntity[Attributes.Booking.BookerPhone1] = payloadInfo.BookingInfo.BookingIdentity.Booker.Phone;
                bookingEntity[Attributes.Booking.BookerPhone2] = payloadInfo.BookingInfo.BookingIdentity.Booker.Mobile;
                bookingEntity[Attributes.Booking.BookerEmergencyPhone] = payloadInfo.BookingInfo.BookingIdentity.Booker.EmergencyNumber;
                bookingEntity[Attributes.Booking.Participants] = PrepareTravelParticipantsInfo(payloadInfo, globalVariable);
                bookingEntity[Attributes.Booking.ParticipantRemarks] = PrepareTravelParticipantsRemarks(payloadInfo, globalVariable);
                bookingEntity[Attributes.Booking.Transfer] = PrepareTransferInfo(payloadInfo, globalVariable);
                bookingEntity[Attributes.Booking.TransferRemarks] = PrepareTransferRemarks(payloadInfo, globalVariable);
                bookingEntity[Attributes.Booking.ExtraService] = PrepareExtraServicesInfo(payloadInfo, globalVariable);
                bookingEntity[Attributes.Booking.ExtraServiceRemarks] = PrepareExtraServiceRemarks(payloadInfo, globalVariable);
                bookingEntity[Attributes.Booking.SourceMarketId] = new EntityReference(EntityName.Country, "", payloadInfo.BookingInfo.BookingIdentifier.SourceMarket);
                bookingEntity[Attributes.Booking.Statuscode] = payloadInfo.BookingInfo.BookingGeneral.BookingStatus;
                bookingEntity[Attributes.Booking.TravelAmount] = payloadInfo.BookingInfo.BookingGeneral.TravelAmount;
                bookingEntity[Attributes.Booking.TransactionCurrencyId] = payloadInfo.BookingInfo.BookingGeneral.Currency;
                bookingEntity[Attributes.Booking.HasSourceMarketComplaint] = payloadInfo.BookingInfo.BookingGeneral.HasComplaint;
                bookingEntity[Attributes.Booking.BookerEmail] = payloadInfo.BookingInfo.BookingIdentity.Booker.Email;
                sucMsg = globalVariable.xrm.UpsertEntity(bookingEntity, globalVariable.service);

                if (sucMsg.Create)
                {
                    globalVariable.DeleteBookingRole = false;
                    globalVariable.DeleteAccomdOrTrnsprt = false;
                }
            }
            return sucMsg;
        }

        /// <summary>
        /// To prepare travel participants information
        /// </summary>
        /// <param name="payloadInfo"></param>
        /// <returns></returns>
        string PrepareTravelParticipantsInfo(PayloadBooking payloadInfo, GlobalVariables globalVariable)
        {
            string travelParticipants = string.Empty;
            if (payloadInfo.BookingInfo.TravelParticipant != null)
                for (int i = 0; i < payloadInfo.BookingInfo.TravelParticipant.Length; i++)
                {
                    if (!string.IsNullOrEmpty(travelParticipants))
                        travelParticipants += globalVariable.NextLine;

                    travelParticipants += payloadInfo.BookingInfo.TravelParticipant[i].TravelParticipantIdOnTour + globalVariable.Seperator +
                                          payloadInfo.BookingInfo.TravelParticipant[i].FirstName + globalVariable.Seperator +
                                          payloadInfo.BookingInfo.TravelParticipant[i].LastName + globalVariable.Seperator +
                                          payloadInfo.BookingInfo.TravelParticipant[i].Age + globalVariable.Seperator +
                                          payloadInfo.BookingInfo.TravelParticipant[i].Birthdate + globalVariable.Seperator +
                                          payloadInfo.BookingInfo.TravelParticipant[i].Gender + globalVariable.Seperator +
                                          payloadInfo.BookingInfo.TravelParticipant[i].Relation + globalVariable.Seperator +
                                          payloadInfo.BookingInfo.TravelParticipant[i].Language;

                }
            return travelParticipants;
        }

        /// <summary>
        /// To prepare travel participants remarks information
        /// </summary>
        /// <param name="payloadInfo"></param>
        /// <returns></returns>
        string PrepareTravelParticipantsRemarks(PayloadBooking payloadInfo, GlobalVariables globalVariable)
        {
            string remarks = string.Empty;
            if (payloadInfo.BookingInfo.TravelParticipant != null)
                for (int i = 0; i < payloadInfo.BookingInfo.TravelParticipant.Length; i++)
                {
                    if (payloadInfo.BookingInfo.TravelParticipant[i].Remark != null)
                        for (int j = 0; j < payloadInfo.BookingInfo.TravelParticipant[i].Remark.Length; j++)
                        {
                            if (!string.IsNullOrEmpty(remarks))
                                remarks += globalVariable.NextLine;

                            remarks += payloadInfo.BookingInfo.TravelParticipant[i].TravelParticipantIdOnTour + globalVariable.Seperator +
                                       payloadInfo.BookingInfo.TravelParticipant[i].Remark[j].RemarkType + globalVariable.Seperator +
                                       payloadInfo.BookingInfo.TravelParticipant[i].Remark[j].Text;
                        }

                }
            return remarks;
        }

        /// <summary>
        /// To prepare transfer information
        /// </summary>
        /// <param name="payloadInfo"></param>
        /// <returns></returns>
        string PrepareTransferInfo(PayloadBooking payloadInfo, GlobalVariables globalVariable)
        {
            string transferInfo = string.Empty;
            if (payloadInfo.BookingInfo.Services != null && payloadInfo.BookingInfo.Services.Transfer != null)
                for (int i = 0; i < payloadInfo.BookingInfo.Services.Transfer.Length; i++)
                {
                    if (!string.IsNullOrEmpty(transferInfo))
                        transferInfo += globalVariable.NextLine;

                    transferInfo += payloadInfo.BookingInfo.Services.Transfer[i].TransferCode + globalVariable.Seperator +
                                    payloadInfo.BookingInfo.Services.Transfer[i].TransferDescription + globalVariable.Seperator +
                                    payloadInfo.BookingInfo.Services.Transfer[i].Order + globalVariable.Seperator +
                                    payloadInfo.BookingInfo.Services.Transfer[i].StartDate + globalVariable.Seperator +
                                    payloadInfo.BookingInfo.Services.Transfer[i].EndDate + globalVariable.Seperator +
                                    payloadInfo.BookingInfo.Services.Transfer[i].Category + globalVariable.Seperator +
                                    payloadInfo.BookingInfo.Services.Transfer[i].TransferType + globalVariable.Seperator +
                                    payloadInfo.BookingInfo.Services.Transfer[i].DepartureAirport + globalVariable.Seperator +
                                    payloadInfo.BookingInfo.Services.Transfer[i].ArrivalAirport;


                }
            return transferInfo;
        }

        /// <summary>
        /// To prepare trasnfer remarks information
        /// </summary>
        /// <param name="payloadInfo"></param>
        /// <returns></returns>
        string PrepareTransferRemarks(PayloadBooking payloadInfo, GlobalVariables globalVariable)
        {
            string remarks = string.Empty;
            if (payloadInfo.BookingInfo.Services != null && payloadInfo.BookingInfo.Services.Transfer != null)
                for (int i = 0; i < payloadInfo.BookingInfo.Services.Transfer.Length; i++)
                {
                    if (payloadInfo.BookingInfo.Services.Transfer[i].Remark != null)
                        for (int j = 0; j < payloadInfo.BookingInfo.Services.Transfer[i].Remark.Length; j++)
                        {
                            if (!string.IsNullOrEmpty(remarks))
                                remarks += globalVariable.NextLine;

                            remarks += payloadInfo.BookingInfo.Services.Transfer[i].Remark[j].RemarkType + globalVariable.Seperator +
                                       payloadInfo.BookingInfo.Services.Transfer[i].Remark[j].Text;
                        }

                }
            return remarks;
        }

        /// <summary>
        /// To prepare extraservice information
        /// </summary>
        /// <param name="payloadInfo"></param>
        /// <returns></returns>
        string PrepareExtraServicesInfo(PayloadBooking payloadInfo, GlobalVariables globalVariable)
        {
            string extraServices = string.Empty;
            if (payloadInfo.BookingInfo.Services != null && payloadInfo.BookingInfo.Services.ExtraService != null)
                for (int i = 0; i < payloadInfo.BookingInfo.Services.ExtraService.Length; i++)
                {

                    if (!string.IsNullOrEmpty(extraServices))
                        extraServices += globalVariable.NextLine;

                    extraServices += payloadInfo.BookingInfo.Services.ExtraService[i].ExtraServiceCode + globalVariable.Seperator +
                                     payloadInfo.BookingInfo.Services.ExtraService[i].ExtraServiceDescription + globalVariable.Seperator +
                                     payloadInfo.BookingInfo.Services.ExtraService[i].Order + globalVariable.Seperator +
                                     payloadInfo.BookingInfo.Services.ExtraService[i].StartDate + globalVariable.Seperator +
                                     payloadInfo.BookingInfo.Services.ExtraService[i].EndDate;


                }
            return extraServices;
        }

        /// <summary>
        /// To prepare extra service remarks information
        /// </summary>
        /// <param name="payloadInfo"></param>
        /// <returns></returns>
        string PrepareExtraServiceRemarks(PayloadBooking payloadInfo, GlobalVariables globalVariable)
        {
            string extraServiceRemarks = string.Empty;
            if (payloadInfo.BookingInfo.Services != null && payloadInfo.BookingInfo.Services.ExtraService != null)
                for (int i = 0; i < payloadInfo.BookingInfo.Services.ExtraService.Length; i++)
                {
                    if (payloadInfo.BookingInfo.Services.ExtraService[i].Remark != null)
                        for (int j = 0; j < payloadInfo.BookingInfo.Services.ExtraService[i].Remark.Length; j++)
                        {
                            if (!string.IsNullOrEmpty(extraServiceRemarks))
                                extraServiceRemarks += globalVariable.NextLine;

                            extraServiceRemarks += payloadInfo.BookingInfo.Services.ExtraService[i].Remark[j].RemarkType + globalVariable.Seperator +
                                             payloadInfo.BookingInfo.Services.ExtraService[i].Remark[j].Text;

                        }

                }
            return extraServiceRemarks;
        }


        /// <summary>
        /// To process accomodation informaation
        /// </summary>
        /// <param name="payloadInfo"></param>
        /// <returns></returns>
        void ProcessAccomodation(PayloadBooking payloadInfo, GlobalVariables globalVariable)
        {
            if (globalVariable.DeleteAccomdOrTrnsprt)
            {
                ProcessRecordsToDelete(EntityName.BookingAccommodation,
                    new string[] { Attributes.BookingAccommodation.BookingId },
                    new string[] { Attributes.BookingAccommodation.BookingId },
                    new string[] { "" },
                    globalVariable);

            }

            if (payloadInfo.BookingInfo.Services != null && payloadInfo.BookingInfo.Services.Accommodation != null)
            {
                EntityCollection entColAccomodation = new EntityCollection();
                Entity accomodationEntity = null;
                for (int i = 0; i < payloadInfo.BookingInfo.Services.Accommodation.Length; i++)
                {
                    accomodationEntity = new Entity(EntityName.BookingAccommodation, "", "");
                    accomodationEntity[Attributes.BookingAccommodation.HotelId] = new EntityReference(EntityName.Hotel, "", payloadInfo.BookingInfo.Services.Accommodation[i].GroupAccommodationCode);
                    accomodationEntity[Attributes.BookingAccommodation.Order] = payloadInfo.BookingInfo.Services.Accommodation[i].Order;
                    accomodationEntity[Attributes.BookingAccommodation.StartDateandTime] = payloadInfo.BookingInfo.Services.Accommodation[i].StartDate;
                    accomodationEntity[Attributes.BookingAccommodation.EndDateandTime] = payloadInfo.BookingInfo.Services.Accommodation[i].EndDate;
                    accomodationEntity[Attributes.BookingAccommodation.RoomType] = payloadInfo.BookingInfo.Services.Accommodation[i].RoomType;
                    accomodationEntity[Attributes.BookingAccommodation.BoardType] = payloadInfo.BookingInfo.Services.Accommodation[i].BoardType;
                    accomodationEntity[Attributes.BookingAccommodation.HasSharedRoom] = payloadInfo.BookingInfo.Services.Accommodation[i].HasSharedRoom;
                    accomodationEntity[Attributes.BookingAccommodation.NumberofParticipants] = payloadInfo.BookingInfo.Services.Accommodation[i].NumberOfParticipants;
                    accomodationEntity[Attributes.BookingAccommodation.NumberofRooms] = payloadInfo.BookingInfo.Services.Accommodation[i].NumberOfRooms;
                    accomodationEntity[Attributes.BookingAccommodation.WithTransfer] = payloadInfo.BookingInfo.Services.Accommodation[i].WithTransfer;
                    accomodationEntity[Attributes.BookingAccommodation.IsExternalService] = payloadInfo.BookingInfo.Services.Accommodation[i].IsExternalService;
                    accomodationEntity[Attributes.BookingAccommodation.ExternalServiceCode] = payloadInfo.BookingInfo.Services.Accommodation[i].ExternalServiceCode;
                    accomodationEntity[Attributes.BookingAccommodation.NotificationRequired] = payloadInfo.BookingInfo.Services.Accommodation[i].NotificationRequired;
                    accomodationEntity[Attributes.BookingAccommodation.NeedTourGuideAssignment] = payloadInfo.BookingInfo.Services.Accommodation[i].NeedsTourGuideAssignment;
                    accomodationEntity[Attributes.BookingAccommodation.ExternalTransfer] = payloadInfo.BookingInfo.Services.Accommodation[i].IsExternalTransfer;
                    accomodationEntity[Attributes.BookingAccommodation.TransferServiceLevel] = payloadInfo.BookingInfo.Services.Accommodation[i].TransferServiceLevel;
                    accomodationEntity[Attributes.BookingAccommodation.SourceMarketHotelName] = payloadInfo.BookingInfo.Services.Accommodation[i].AccommodationDescription;
                    entColAccomodation.Entities.Add(accomodationEntity);

                }

                globalVariable.xrm.BulkCreate(entColAccomodation, globalVariable.service);
            }
        }

        /// <summary>
        /// To process accomodation remarks
        /// </summary>
        /// <param name="payloadInfo"></param>
        /// <param name="messageList"></param>
        void ProcessAccomodationRemarks(PayloadBooking payloadInfo, GlobalVariables globalVariable)
        {
            EntityCollection entColAccomodationRemarks = new EntityCollection();
            Entity remarkEntity = null;
            if (payloadInfo.BookingInfo.Services != null && payloadInfo.BookingInfo.Services.Accommodation != null)
            {
                for (int i = 0; i < payloadInfo.BookingInfo.Services.Accommodation.Length; i++)
                {
                    if (payloadInfo.BookingInfo.Services.Accommodation[i].Remark != null)
                    {
                        for (int j = 0; j < payloadInfo.BookingInfo.Services.Accommodation[i].Remark.Length; j++)
                        {
                            remarkEntity = new Entity(EntityName.Remark);
                            remarkEntity[Attributes.Remark.Type] = payloadInfo.BookingInfo.Services.Accommodation[i].Remark[j].RemarkType;
                            remarkEntity[Attributes.Remark.RemarkName] = payloadInfo.BookingInfo.Services.Accommodation[i].Remark[j].Text;
                            //messageList.Find()
                            remarkEntity[Attributes.Remark.BookingAccommodationId] = new EntityReference(EntityName.BookingAccommodation, "", "");
                            entColAccomodationRemarks.Entities.Add(remarkEntity);
                        }
                    }
                }

                if (entColAccomodationRemarks.Entities.Count > 0)
                    globalVariable.xrm.BulkCreate(entColAccomodationRemarks, globalVariable.service);
            }
        }

        List<SuccessMessage> ProcessTransport(PayloadBooking payloadInfo, GlobalVariables globalVariable)
        {
            EntityCollection entColTransport = new EntityCollection();
            Entity transportEntity = null;

            if (globalVariable.DeleteAccomdOrTrnsprt)
            {
                ProcessRecordsToDelete(EntityName.BookingTransport,
                    new string[] { Attributes.BookingTransport.BookingId },
                    new string[] { Attributes.BookingAccommodation.BookingId },
                    new string[] { "" },
                    globalVariable);

            }

            for (int i = 0; i < payloadInfo.BookingInfo.Services.Transport.Length; i++)
            {
                transportEntity = new Entity(EntityName.BookingTransport, "", "");
                transportEntity[Attributes.BookingTransport.TransportCode] = payloadInfo.BookingInfo.Services.Transport[i].TransportCode;
                transportEntity[Attributes.BookingTransport.Description] = payloadInfo.BookingInfo.Services.Transport[i].TransportDescription;
                transportEntity[Attributes.BookingTransport.Order] = payloadInfo.BookingInfo.Services.Transport[i].Order;
                transportEntity[Attributes.BookingTransport.StartDateandTime] = payloadInfo.BookingInfo.Services.Transport[i].StartDate;
                transportEntity[Attributes.BookingTransport.EndDateandTime] = payloadInfo.BookingInfo.Services.Transport[i].EndDate;
                transportEntity[Attributes.BookingTransport.TransferType] = payloadInfo.BookingInfo.Services.Transport[i].TransferType;
                transportEntity[Attributes.BookingTransport.DepartureGatewayId] = new EntityReference(EntityName.Gateway, "", payloadInfo.BookingInfo.Services.Transport[i].DepartureAirport);
                transportEntity[Attributes.BookingTransport.ArrivalGatewayId] = new EntityReference(EntityName.Gateway, "", payloadInfo.BookingInfo.Services.Transport[i].ArrivalAirport);
                transportEntity[Attributes.BookingTransport.CarrierCode] = payloadInfo.BookingInfo.Services.Transport[i].CarrierCode;
                transportEntity[Attributes.BookingTransport.FlightNumber] = payloadInfo.BookingInfo.Services.Transport[i].FlightNumber;
                transportEntity[Attributes.BookingTransport.FlightIdentifier] = payloadInfo.BookingInfo.Services.Transport[i].FlightIdentifier;
                transportEntity[Attributes.BookingTransport.NumberofParticipants] = payloadInfo.BookingInfo.Services.Transport[i].NumberOfParticipants;
                transportEntity[Attributes.BookingTransport.FlightNumber] = payloadInfo.BookingInfo.Services.Transport[i].FlightNumber;

                entColTransport.Entities.Add(transportEntity);


            }

            return globalVariable.xrm.BulkCreate(entColTransport, globalVariable.service);
        }

        void ProcessTransportRemarks(PayloadBooking payloadInfo, GlobalVariables globalVariable)
        {

            EntityCollection entColTransportRemark = new EntityCollection();
            Entity TransportRemark = null;


            for (int i = 0; i < payloadInfo.BookingInfo.Services.Transport.Length; i++)
            {
                for (int j = 0; j < payloadInfo.BookingInfo.Services.Transport[i].Remark.Length; j++)
                {
                    TransportRemark = new Entity(EntityName.BookingTransport, "", "");
                    TransportRemark[Attributes.Remark.Type] = payloadInfo.BookingInfo.Services.Transport[i].Remark[j].RemarkType;
                    TransportRemark[Attributes.Remark.RemarkName] = payloadInfo.BookingInfo.Services.Transport[i].Remark[j].Text;
                    TransportRemark[Attributes.Remark.BookingTransportId] = new EntityReference(EntityName.BookingTransport, "", "");

                    entColTransportRemark.Entities.Add(TransportRemark);
                }


            }

            if (entColTransportRemark.Entities.Count > 0)
                globalVariable.xrm.BulkCreate(entColTransportRemark, globalVariable.service);

        }

        void ProcessBookingRole(PayloadBooking payloadInfo, GlobalVariables globalVariable)
        {
            if (globalVariable.DeleteBookingRole)
            {
                ProcessRecordsToDelete(EntityName.CustomerBookingRole,
                    new string[] { Attributes.CustomerBookingRole.CustomerBookingRoleId },
                    new string[] { Attributes.CustomerBookingRole.BookingId, Attributes.CustomerBookingRole.Customer },
                    new string[] { "" },
                    globalVariable);

            }

            Entity entCustBookingRole = new Entity(EntityName.CustomerBookingRole);
            entCustBookingRole[Attributes.CustomerBookingRole.BookingId] = new EntityReference(EntityName.Booking, "", "");
            if (globalVariable.IsCustomerTypeAccount)
            {
                entCustBookingRole[Attributes.CustomerBookingRole.Customer] = new EntityReference(EntityName.Account, "", "");
            }
            else
            {
                entCustBookingRole[Attributes.CustomerBookingRole.Customer] = new EntityReference(EntityName.Contact, "", "");
            }

            EntityCollection entCol = new EntityCollection();
            entCol.Entities.Add(entCustBookingRole);

            globalVariable.xrm.BulkCreate(entCol, globalVariable.service);
        }

        /// <summary>
        /// To process records to delete
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="columns"></param>
        /// <param name="filterKeys"></param>
        /// <param name="filterValues"></param>
        void ProcessRecordsToDelete(string entityName, string[] columns, string[] filterKeys, string[] filterValues, GlobalVariables globalVariable)
        {
            EntityCollection entCollection = globalVariable.xrm.RetrieveMultipleRecords(entityName, columns, filterKeys, filterValues, globalVariable.service);
            if (entCollection != null && entCollection.Entities.Count > 0)
            {
                EntityReferenceCollection entRefCollection = new EntityReferenceCollection();
                foreach (Entity ent in entCollection.Entities)
                {
                    entRefCollection.Add(new EntityReference(ent.LogicalName, ent.Id));
                }
                globalVariable.xrm.BulkDelete(entRefCollection, globalVariable.service);
            }
        }



    }

    public class GlobalVariables
    {
        public CommonXrm xrm { get; set; }

        public IOrganizationService service { get; set; }

        public string Seperator { get { return ","; }  }

        public string NextLine { get { return "\r\n"; } }

        public bool IsCustomerTypeAccount { get; set; }

        public bool DeleteBookingRole  { get; set; }

        public bool DeleteAccomdOrTrnsprt { get; set; }
    }
}
