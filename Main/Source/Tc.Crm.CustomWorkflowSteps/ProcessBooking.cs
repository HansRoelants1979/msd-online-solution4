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
            if (payloadInfo.customer.CustomerGeneral.CustomerType.ToString() == PayloadBooking.AccountType)
            {
                ProcessAccount(payloadInfo, globalVariable);
                globalVariable.IsCustomerTypeAccount = true;
            }
            else if (payloadInfo.customer.CustomerGeneral.CustomerType.ToString() == PayloadBooking.ContactType)
            {
                ProcessContact(payloadInfo, globalVariable);
                globalVariable.IsCustomerTypeAccount = false;
            }

            ProcessBookingInfo(payloadInfo, globalVariable);

            ProcessAccomodation(payloadInfo, globalVariable);

            ProcessTransport(payloadInfo, globalVariable);




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
            if (payloadInfo.customer != null)
            {

                Entity contactEntity = new Entity(EntityName.Contact, Attributes.Contact.SourceSystemID, payloadInfo.customer.CustomerIdentifier.CustomerId);

                contactEntity[Attributes.Contact.FirstName] = payloadInfo.customer.CustomerIdentity.FirstName;
                contactEntity[Attributes.Contact.MiddleName] = payloadInfo.customer.CustomerIdentity.MiddleName;
                contactEntity[Attributes.Contact.LastName] = payloadInfo.customer.CustomerIdentity.LastName;
                contactEntity[Attributes.Contact.Language] = payloadInfo.customer.CustomerIdentity.Language;
                contactEntity[Attributes.Contact.Gender] = payloadInfo.customer.CustomerIdentity.Gender;
                contactEntity[Attributes.Contact.AcademicTitle] = payloadInfo.customer.CustomerIdentity.AcademicTitle;
                contactEntity[Attributes.Contact.Salutation] = payloadInfo.customer.CustomerIdentity.Salutation;
                contactEntity[Attributes.Contact.BirthDate] = payloadInfo.customer.CustomerIdentity.Birthdate;
                contactEntity[Attributes.Contact.StatusCode] = payloadInfo.customer.CustomerGeneral.CustomerStatus;
                // couldn't find segment,DateofDeath in booking.customer.identity.additional
                contactEntity[Attributes.Contact.Segment] = payloadInfo.customer.Additional.Segment;
                contactEntity[Attributes.Contact.DateofDeath] = payloadInfo.customer.Additional.DateOfDeath;
                if (payloadInfo.customer.Address != null && payloadInfo.customer.Address.Length > 0)
                {
                    contactEntity[Attributes.Contact.Address1_AdditionalInformation] = payloadInfo.customer.Address[0].AdditionalAddressInfo;
                    contactEntity[Attributes.Contact.Address1_FlatOrUnitNumber] = payloadInfo.customer.Address[0].FlatNumberUnit;
                    contactEntity[Attributes.Contact.Address1_HouseNumberOrBuilding] = payloadInfo.customer.Address[0].HouseNumberBuilding;
                    contactEntity[Attributes.Contact.Address1_Town] = payloadInfo.customer.Address[0].Town;
                    contactEntity[Attributes.Contact.Address1_CountryId] = payloadInfo.customer.Address[0].Country;
                    contactEntity[Attributes.Contact.Address1_County] = payloadInfo.customer.Address[0].County;
                    contactEntity[Attributes.Contact.Address1_PostalCode] = payloadInfo.customer.Address[0].PostalCode;
                    contactEntity[Attributes.Contact.Address2_AdditionalInformation] = payloadInfo.customer.Address[1].AdditionalAddressInfo;
                    contactEntity[Attributes.Contact.Address2_FlatOrUnitNumber] = payloadInfo.customer.Address[1].FlatNumberUnit;
                    contactEntity[Attributes.Contact.Address2_HouseNumberorBuilding] = payloadInfo.customer.Address[1].HouseNumberBuilding;
                    contactEntity[Attributes.Contact.Address2_Town] = payloadInfo.customer.Address[1].Town;
                    contactEntity[Attributes.Contact.Address2_CountryId] = payloadInfo.customer.Address[1].Country;
                    contactEntity[Attributes.Contact.Address2_County] = payloadInfo.customer.Address[1].County;
                    contactEntity[Attributes.Contact.Address2_PostalCode] = payloadInfo.customer.Address[1].PostalCode;
                }
                if (payloadInfo.customer.Phone != null && payloadInfo.customer.Phone.Length > 0)
                {
                    contactEntity[Attributes.Contact.Telephone2Type] = payloadInfo.customer.Phone[0].PhoneType;
                    contactEntity[Attributes.Contact.Telephone1] = payloadInfo.customer.Phone[0].Number;
                    contactEntity[Attributes.Contact.Telephone2Type] = payloadInfo.customer.Phone[1].PhoneType;
                    contactEntity[Attributes.Contact.Telephone2] = payloadInfo.customer.Phone[1].Number;
                    contactEntity[Attributes.Contact.Telephone3Type] = payloadInfo.customer.Phone[2].PhoneType;
                    contactEntity[Attributes.Contact.Telephone3] = payloadInfo.customer.Phone[2].Number;
                }
                if (payloadInfo.customer.Email != null && payloadInfo.customer.Email.Length > 0)
                {
                    contactEntity[Attributes.Contact.EMailAddress1] = payloadInfo.customer.Email[0].Address;
                    contactEntity[Attributes.Contact.EmailAddress1Type] = payloadInfo.customer.Email[0].EmailType;
                    contactEntity[Attributes.Contact.EMailAddress2] = payloadInfo.customer.Email[1].Address;
                    contactEntity[Attributes.Contact.EmailAddress2Type] = payloadInfo.customer.Email[1].EmailType;
                    contactEntity[Attributes.Contact.EMailAddress3] = payloadInfo.customer.Email[2].Address;
                    contactEntity[Attributes.Contact.EmailAddress3Type] = payloadInfo.customer.Email[2].EmailType;
                }
                contactEntity[Attributes.Contact.SourceMarketId] = payloadInfo.customer.CustomerIdentifier.SourceMarket;
                contactEntity[Attributes.Contact.SourceSystemID] = payloadInfo.customer.CustomerIdentifier.CustomerId;
                sucMsg = globalVariable.xrm.UpsertEntity(contactEntity, globalVariable.service);

                if (sucMsg.Create)
                    globalVariable.DeleteBookingRole = false;

                globalVariable.CustomerId = sucMsg.Id;
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

            if (payloadInfo.customer != null)
            {
                Entity accountEntity = new Entity(EntityName.Account, Attributes.Account.SourceSystemID, payloadInfo.customer.CustomerIdentifier.CustomerId);
                accountEntity[Attributes.Account.Name] = payloadInfo.customer.Company.CompanyName;
                if (payloadInfo.customer.Address != null && payloadInfo.customer.Address.Length > 0)
                {
                    accountEntity[Attributes.Account.Address1_AdditionalInformation] = payloadInfo.customer.Address[0].AdditionalAddressInfo;
                    accountEntity[Attributes.Account.Address1_FlatOrUnitNumber] = payloadInfo.customer.Address[0].FlatNumberUnit;
                    accountEntity[Attributes.Account.Address1_HouseNumberOrBuilding] = payloadInfo.customer.Address[0].HouseNumberBuilding;
                    accountEntity[Attributes.Account.Address1_Town] = payloadInfo.customer.Address[0].Town;
                    accountEntity[Attributes.Account.Address1_PostalCode] = payloadInfo.customer.Address[0].PostalCode;
                    //accountEntity[Attributes.Account.Address1_CountryId] = payloadInfo.customer.Address[0].Country;
                    accountEntity[Attributes.Account.Address1_County] = payloadInfo.customer.Address[0].County;
                }
                if (payloadInfo.customer.Phone != null && payloadInfo.customer.Phone.Length > 0)
                {
                    accountEntity[Attributes.Account.Telephone1_Type] = payloadInfo.customer.Phone[0].PhoneType;
                    accountEntity[Attributes.Account.Telephone1] = payloadInfo.customer.Phone[0].Number;
                    if (payloadInfo.customer.Phone.Length > 1)
                    {
                        accountEntity[Attributes.Account.Telephone2_Type] = payloadInfo.customer.Phone[1].PhoneType;
                        accountEntity[Attributes.Account.Telephone2] = payloadInfo.customer.Phone[1].Number;
                    }
                    if (payloadInfo.customer.Phone.Length > 2)
                    {
                        accountEntity[Attributes.Account.Telephone3_Type] = payloadInfo.customer.Phone[2].PhoneType;
                        accountEntity[Attributes.Account.Telephone3] = payloadInfo.customer.Phone[2].Number;
                    }
                }
                if (payloadInfo.customer.Email != null && payloadInfo.customer.Email.Length > 0)
                {
                    accountEntity[Attributes.Account.EMailAddress1] = payloadInfo.customer.Email[0].Address;
                    accountEntity[Attributes.Account.EmailAddress1_Type] = payloadInfo.customer.Email[0].EmailType;
                    if (payloadInfo.customer.Email.Length > 1)
                    {
                        accountEntity[Attributes.Account.EMailAddress2] = payloadInfo.customer.Email[1].Address;
                        accountEntity[Attributes.Account.EmailAddress2_Type] = payloadInfo.customer.Email[1].EmailType;
                    }
                    if (payloadInfo.customer.Email.Length > 2)
                    {
                        accountEntity[Attributes.Account.EMailAddress3] = payloadInfo.customer.Email[2].Address;
                        accountEntity[Attributes.Account.EmailAddress3_Type] = payloadInfo.customer.Email[2].EmailType;
                    }
                }
                accountEntity[Attributes.Account.SourceMarketId] = payloadInfo.customer.CustomerIdentifier.SourceMarket;
                accountEntity[Attributes.Account.SourceSystemID] = payloadInfo.customer.CustomerIdentifier.CustomerId;


                sucMsg = globalVariable.xrm.UpsertEntity(accountEntity, globalVariable.service);
                if (sucMsg.Create)
                    globalVariable.DeleteBookingRole = false;

                globalVariable.CustomerId = sucMsg.Id;
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
            if (payloadInfo.bookingIdentifier != null)
            {

                Entity bookingEntity = new Entity(EntityName.Booking, Attributes.Booking.Name, payloadInfo.bookingIdentifier.BookingNumber);

                bookingEntity[Attributes.Booking.Name] = payloadInfo.bookingIdentifier.BookingNumber;
                bookingEntity[Attributes.Booking.OnTourVersion] = payloadInfo.bookingIdentifier.BookingVersionOnTour;
                bookingEntity[Attributes.Booking.TourOperatorVersion] = payloadInfo.bookingIdentifier.BookingVersionTourOperator;
                bookingEntity[Attributes.Booking.OnTourUpdatedDate] = payloadInfo.bookingIdentifier.BookingUpdateDateOnTour;
                bookingEntity[Attributes.Booking.TourOperatorUpdatedDate] = payloadInfo.bookingIdentifier.BookingUpdateDateTourOperator;
                bookingEntity[Attributes.Booking.BookingDate] = payloadInfo.bookingGeneral.BookingDate;
                bookingEntity[Attributes.Booking.DepartureDate] = payloadInfo.bookingGeneral.DepartureDate;
                bookingEntity[Attributes.Booking.ReturnDate] = payloadInfo.bookingGeneral.ReturnDate;
                bookingEntity[Attributes.Booking.Duration] = payloadInfo.bookingGeneral.Duration;
                bookingEntity[Attributes.Booking.DestinationGatewayId] = new EntityReference(EntityName.Gateway, Attributes.Gateway.IATA, payloadInfo.bookingGeneral.Destination);
                bookingEntity[Attributes.Booking.TourOperatorId] = new EntityReference(EntityName.TourOperator, Attributes.TourOperator.TourOperatorCode, payloadInfo.bookingGeneral.ToCode);
                bookingEntity[Attributes.Booking.BrandId] = new EntityReference(EntityName.Brand, Attributes.Brand.BrandCode, payloadInfo.bookingGeneral.Brand);
                bookingEntity[Attributes.Booking.BrochureCode] = payloadInfo.bookingGeneral.BrochureCode;
                bookingEntity[Attributes.Booking.IsLateBooking] = payloadInfo.bookingGeneral.IsLateBooking;
                bookingEntity[Attributes.Booking.NumberofParticipants] = payloadInfo.bookingGeneral.NumberOfParticipants;
                bookingEntity[Attributes.Booking.NumberofAdults] = payloadInfo.bookingGeneral.NumberOfAdults;
                bookingEntity[Attributes.Booking.NumberofChildren] = payloadInfo.bookingGeneral.NumberOfChildren;
                bookingEntity[Attributes.Booking.NumberofInfants] = payloadInfo.bookingGeneral.NumberOfInfants;
                bookingEntity[Attributes.Booking.BookerPhone1] = payloadInfo.bookingIdentity.Booker.Phone;
                bookingEntity[Attributes.Booking.BookerPhone2] = payloadInfo.bookingIdentity.Booker.Mobile;
                bookingEntity[Attributes.Booking.BookerEmergencyPhone] = payloadInfo.bookingIdentity.Booker.EmergencyNumber;
                bookingEntity[Attributes.Booking.Participants] = PrepareTravelParticipantsInfo(payloadInfo, globalVariable);
                bookingEntity[Attributes.Booking.ParticipantRemarks] = PrepareTravelParticipantsRemarks(payloadInfo, globalVariable);
                bookingEntity[Attributes.Booking.Transfer] = PrepareTransferInfo(payloadInfo, globalVariable);
                bookingEntity[Attributes.Booking.TransferRemarks] = PrepareTransferRemarks(payloadInfo, globalVariable);
                bookingEntity[Attributes.Booking.ExtraService] = PrepareExtraServicesInfo(payloadInfo, globalVariable);
                bookingEntity[Attributes.Booking.ExtraServiceRemarks] = PrepareExtraServiceRemarks(payloadInfo, globalVariable);
                //bookingEntity[Attributes.Booking.SourceMarketId] = new EntityReference(EntityName.Country, Attributes.Country., payloadInfo.bookingIdentifier.SourceMarket);
                bookingEntity[Attributes.Booking.Statuscode] = payloadInfo.bookingGeneral.BookingStatus;
                bookingEntity[Attributes.Booking.TravelAmount] = payloadInfo.bookingGeneral.TravelAmount;
                bookingEntity[Attributes.Booking.TransactionCurrencyId] = payloadInfo.bookingGeneral.Currency;
                bookingEntity[Attributes.Booking.HasSourceMarketComplaint] = payloadInfo.bookingGeneral.HasComplaint;
                bookingEntity[Attributes.Booking.BookerEmail] = payloadInfo.bookingIdentity.Booker.Email;
                sucMsg = globalVariable.xrm.UpsertEntity(bookingEntity, globalVariable.service);

                if (sucMsg.Create)
                {
                    globalVariable.DeleteBookingRole = false;
                    globalVariable.DeleteAccomdOrTrnsprt = false;
                }

                globalVariable.BookingId = sucMsg.Id;
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
            if (payloadInfo.travelParticipant != null)
                for (int i = 0; i < payloadInfo.travelParticipant.Length; i++)
                {
                    if (!string.IsNullOrEmpty(travelParticipants))
                        travelParticipants += globalVariable.NextLine;

                    travelParticipants += payloadInfo.travelParticipant[i].TravelParticipantIdOnTour + globalVariable.Seperator +
                                          payloadInfo.travelParticipant[i].FirstName + globalVariable.Seperator +
                                          payloadInfo.travelParticipant[i].LastName + globalVariable.Seperator +
                                          payloadInfo.travelParticipant[i].Age + globalVariable.Seperator +
                                          payloadInfo.travelParticipant[i].Birthdate + globalVariable.Seperator +
                                          payloadInfo.travelParticipant[i].Gender + globalVariable.Seperator +
                                          payloadInfo.travelParticipant[i].Relation + globalVariable.Seperator +
                                          payloadInfo.travelParticipant[i].Language;

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
            if (payloadInfo.travelParticipant != null)
                for (int i = 0; i < payloadInfo.travelParticipant.Length; i++)
                {
                    if (payloadInfo.travelParticipant[i].Remark != null)
                        for (int j = 0; j < payloadInfo.travelParticipant[i].Remark.Length; j++)
                        {
                            if (!string.IsNullOrEmpty(remarks))
                                remarks += globalVariable.NextLine;

                            remarks += payloadInfo.travelParticipant[i].TravelParticipantIdOnTour + globalVariable.Seperator +
                                       payloadInfo.travelParticipant[i].Remark[j].RemarkType + globalVariable.Seperator +
                                       payloadInfo.travelParticipant[i].Remark[j].Text;
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
            if (payloadInfo.services != null && payloadInfo.services.Transfer != null)
                for (int i = 0; i < payloadInfo.services.Transfer.Length; i++)
                {
                    if (!string.IsNullOrEmpty(transferInfo))
                        transferInfo += globalVariable.NextLine;

                    transferInfo += payloadInfo.services.Transfer[i].TransferCode + globalVariable.Seperator +
                                    payloadInfo.services.Transfer[i].TransferDescription + globalVariable.Seperator +
                                    payloadInfo.services.Transfer[i].Order + globalVariable.Seperator +
                                    payloadInfo.services.Transfer[i].StartDate + globalVariable.Seperator +
                                    payloadInfo.services.Transfer[i].EndDate + globalVariable.Seperator +
                                    payloadInfo.services.Transfer[i].Category + globalVariable.Seperator +
                                    payloadInfo.services.Transfer[i].TransferType + globalVariable.Seperator +
                                    payloadInfo.services.Transfer[i].DepartureAirport + globalVariable.Seperator +
                                    payloadInfo.services.Transfer[i].ArrivalAirport;


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
            if (payloadInfo.services != null && payloadInfo.services.Transfer != null)
                for (int i = 0; i < payloadInfo.services.Transfer.Length; i++)
                {
                    if (payloadInfo.services.Transfer[i].Remark != null)
                        for (int j = 0; j < payloadInfo.services.Transfer[i].Remark.Length; j++)
                        {
                            if (!string.IsNullOrEmpty(remarks))
                                remarks += globalVariable.NextLine;

                            remarks += payloadInfo.services.Transfer[i].Remark[j].RemarkType + globalVariable.Seperator +
                                       payloadInfo.services.Transfer[i].Remark[j].Text;
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
            if (payloadInfo.services != null && payloadInfo.services.ExtraService != null)
                for (int i = 0; i < payloadInfo.services.ExtraService.Length; i++)
                {

                    if (!string.IsNullOrEmpty(extraServices))
                        extraServices += globalVariable.NextLine;

                    extraServices += payloadInfo.services.ExtraService[i].ExtraServiceCode + globalVariable.Seperator +
                                     payloadInfo.services.ExtraService[i].ExtraServiceDescription + globalVariable.Seperator +
                                     payloadInfo.services.ExtraService[i].Order + globalVariable.Seperator +
                                     payloadInfo.services.ExtraService[i].StartDate + globalVariable.Seperator +
                                     payloadInfo.services.ExtraService[i].EndDate;


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
            if (payloadInfo.services != null && payloadInfo.services.ExtraService != null)
                for (int i = 0; i < payloadInfo.services.ExtraService.Length; i++)
                {
                    if (payloadInfo.services.ExtraService[i].Remark != null)
                        for (int j = 0; j < payloadInfo.services.ExtraService[i].Remark.Length; j++)
                        {
                            if (!string.IsNullOrEmpty(extraServiceRemarks))
                                extraServiceRemarks += globalVariable.NextLine;

                            extraServiceRemarks += payloadInfo.services.ExtraService[i].Remark[j].RemarkType + globalVariable.Seperator +
                                             payloadInfo.services.ExtraService[i].Remark[j].Text;

                        }

                }
            return extraServiceRemarks;
        }


        void ProcessRemarks(PayloadBooking payloadInfo, GlobalVariables globalVariable)
        {
            if (globalVariable.DeleteAccomdOrTrnsprt)
            {
                ProcessRecordsToDelete(EntityName.Remark,
                    new string[] { Attributes.Remark.RemarkId },
                    new string[] { Attributes.BookingAccommodation.BookingId },
                    new string[] { "" },
                    globalVariable);

            }

            if (payloadInfo.remark != null)
            {
                EntityCollection entColRemark = new EntityCollection();
                Entity remarkEntity = null;
                for (int i = 0; i < payloadInfo.remark.Length; i++)
                {
                    remarkEntity = new Entity(EntityName.Remark, "", "");
                    remarkEntity[Attributes.Remark.Type] = payloadInfo.remark[i].RemarkType;
                    remarkEntity[Attributes.Remark.RemarkName] = payloadInfo.remark[i].Text;
                    entColRemark.Entities.Add(remarkEntity);

                }
                globalVariable.xrm.BulkCreate(entColRemark, globalVariable.service);
            }
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
                    new string[] { Attributes.BookingAccommodation.BookingAccommodationid },
                    new string[] { Attributes.BookingAccommodation.BookingId },
                    new string[] { globalVariable.BookingId },
                    globalVariable);

            }

            if (payloadInfo.services != null && payloadInfo.services.Accommodation != null)
            {
                EntityCollection entColAccomodation = new EntityCollection();
                Entity accomodationEntity = null;
                for (int i = 0; i < payloadInfo.services.Accommodation.Length; i++)
                {
                    accomodationEntity = new Entity(EntityName.BookingAccommodation);
                    accomodationEntity[Attributes.BookingAccommodation.HotelId] = new EntityReference(EntityName.Hotel, Attributes.Hotel.MasterHotelID, payloadInfo.services.Accommodation[i].GroupAccommodationCode);
                    accomodationEntity[Attributes.BookingAccommodation.Order] = payloadInfo.services.Accommodation[i].Order;
                    accomodationEntity[Attributes.BookingAccommodation.StartDateandTime] = payloadInfo.services.Accommodation[i].StartDate;
                    accomodationEntity[Attributes.BookingAccommodation.EndDateandTime] = payloadInfo.services.Accommodation[i].EndDate;
                    accomodationEntity[Attributes.BookingAccommodation.RoomType] = payloadInfo.services.Accommodation[i].RoomType;
                    accomodationEntity[Attributes.BookingAccommodation.BoardType] = payloadInfo.services.Accommodation[i].BoardType;
                    accomodationEntity[Attributes.BookingAccommodation.HasSharedRoom] = payloadInfo.services.Accommodation[i].HasSharedRoom;
                    accomodationEntity[Attributes.BookingAccommodation.NumberofParticipants] = payloadInfo.services.Accommodation[i].NumberOfParticipants;
                    accomodationEntity[Attributes.BookingAccommodation.NumberofRooms] = payloadInfo.services.Accommodation[i].NumberOfRooms;
                    accomodationEntity[Attributes.BookingAccommodation.WithTransfer] = payloadInfo.services.Accommodation[i].WithTransfer;
                    accomodationEntity[Attributes.BookingAccommodation.IsExternalService] = payloadInfo.services.Accommodation[i].IsExternalService;
                    accomodationEntity[Attributes.BookingAccommodation.ExternalServiceCode] = payloadInfo.services.Accommodation[i].ExternalServiceCode;
                    accomodationEntity[Attributes.BookingAccommodation.NotificationRequired] = payloadInfo.services.Accommodation[i].NotificationRequired;
                    accomodationEntity[Attributes.BookingAccommodation.NeedTourGuideAssignment] = payloadInfo.services.Accommodation[i].NeedsTourGuideAssignment;
                    accomodationEntity[Attributes.BookingAccommodation.ExternalTransfer] = payloadInfo.services.Accommodation[i].IsExternalTransfer;
                    accomodationEntity[Attributes.BookingAccommodation.TransferServiceLevel] = payloadInfo.services.Accommodation[i].TransferServiceLevel;
                    accomodationEntity[Attributes.BookingAccommodation.SourceMarketHotelName] = payloadInfo.services.Accommodation[i].AccommodationDescription;

                    accomodationEntity[Attributes.BookingAccommodation.BookingId] = new EntityReference(EntityName.Booking, new Guid(globalVariable.BookingId));

                    entColAccomodation.Entities.Add(accomodationEntity);

                }

                List<SuccessMessage> lstSucMsg = globalVariable.xrm.BulkCreate(entColAccomodation, globalVariable.service);
                ProcessAccomodationRemarks(payloadInfo, globalVariable, lstSucMsg);
            }
        }

        /// <summary>
        /// To process accomodation remarks
        /// </summary>
        /// <param name="payloadInfo"></param>
        /// <param name="messageList"></param>
        void ProcessAccomodationRemarks(PayloadBooking payloadInfo, GlobalVariables globalVariable, List<SuccessMessage> lstSucMsg)
        {
            EntityCollection entColAccomodationRemarks = new EntityCollection();
            Entity remarkEntity = null;
            if (payloadInfo.services != null && payloadInfo.services.Accommodation != null)
            {
                for (int i = 0; i < payloadInfo.services.Accommodation.Length; i++)
                {
                    if (payloadInfo.services.Accommodation[i].Remark != null)
                    {
                        for (int j = 0; j < payloadInfo.services.Accommodation[i].Remark.Length; j++)
                        {
                            remarkEntity = new Entity(EntityName.Remark);
                            remarkEntity[Attributes.Remark.Type] = payloadInfo.services.Accommodation[i].Remark[j].RemarkType;
                            remarkEntity[Attributes.Remark.RemarkName] = payloadInfo.services.Accommodation[i].Remark[j].Text;
                            //messageList.Find()
                            remarkEntity[Attributes.Remark.BookingAccommodationId] = new EntityReference(EntityName.BookingAccommodation, Guid.Parse(lstSucMsg[i].Id));
                            entColAccomodationRemarks.Entities.Add(remarkEntity);
                        }
                    }
                }

                if (entColAccomodationRemarks.Entities.Count > 0)
                    globalVariable.xrm.BulkCreate(entColAccomodationRemarks, globalVariable.service);
            }
        }

        void ProcessTransport(PayloadBooking payloadInfo, GlobalVariables globalVariable)
        {
            EntityCollection entColTransport = new EntityCollection();
            Entity transportEntity = null;

            if (globalVariable.DeleteAccomdOrTrnsprt)
            {
                ProcessRecordsToDelete(EntityName.BookingTransport,
                    new string[] { Attributes.BookingTransport.BookingTransportId },
                    new string[] { Attributes.BookingAccommodation.BookingId },
                    new string[] { globalVariable.BookingId },
                    globalVariable);

            }

            for (int i = 0; i < payloadInfo.services.Transport.Length; i++)
            {
                transportEntity = new Entity(EntityName.BookingTransport);
                transportEntity[Attributes.BookingTransport.TransportCode] = payloadInfo.services.Transport[i].TransportCode;
                transportEntity[Attributes.BookingTransport.Description] = payloadInfo.services.Transport[i].TransportDescription;
                transportEntity[Attributes.BookingTransport.Order] = payloadInfo.services.Transport[i].Order;
                transportEntity[Attributes.BookingTransport.StartDateandTime] = payloadInfo.services.Transport[i].StartDate;
                transportEntity[Attributes.BookingTransport.EndDateandTime] = payloadInfo.services.Transport[i].EndDate;
                transportEntity[Attributes.BookingTransport.TransferType] = payloadInfo.services.Transport[i].TransferType;
                //transportEntity[Attributes.BookingTransport.DepartureGatewayId] = new EntityReference(EntityName.Gateway, "", payloadInfo.services.Transport[i].DepartureAirport);
                //transportEntity[Attributes.BookingTransport.ArrivalGatewayId] = new EntityReference(EntityName.Gateway, "", payloadInfo.services.Transport[i].ArrivalAirport);
                transportEntity[Attributes.BookingTransport.CarrierCode] = payloadInfo.services.Transport[i].CarrierCode;
                transportEntity[Attributes.BookingTransport.FlightNumber] = payloadInfo.services.Transport[i].FlightNumber;
                transportEntity[Attributes.BookingTransport.FlightIdentifier] = payloadInfo.services.Transport[i].FlightIdentifier;
                transportEntity[Attributes.BookingTransport.NumberofParticipants] = payloadInfo.services.Transport[i].NumberOfParticipants;
                transportEntity[Attributes.BookingTransport.FlightNumber] = payloadInfo.services.Transport[i].FlightNumber;

                transportEntity[Attributes.BookingTransport.BookingId] = new EntityReference(EntityName.Booking, new Guid(globalVariable.BookingId));

                entColTransport.Entities.Add(transportEntity);


            }

            List<SuccessMessage> listSucMsg = globalVariable.xrm.BulkCreate(entColTransport, globalVariable.service);
            ProcessTransportRemarks(payloadInfo, globalVariable, listSucMsg);
        }

        void ProcessTransportRemarks(PayloadBooking payloadInfo, GlobalVariables globalVariable, List<SuccessMessage> listSucMsg)
        {

            EntityCollection entColTransportRemark = new EntityCollection();
            Entity TransportRemark = null;


            for (int i = 0; i < payloadInfo.services.Transport.Length; i++)
            {
                for (int j = 0; j < payloadInfo.services.Transport[i].Remark.Length; j++)
                {
                    TransportRemark = new Entity(EntityName.BookingTransport);
                    TransportRemark[Attributes.Remark.Type] = payloadInfo.services.Transport[i].Remark[j].RemarkType;
                    TransportRemark[Attributes.Remark.RemarkName] = payloadInfo.services.Transport[i].Remark[j].Text;
                    TransportRemark[Attributes.Remark.BookingTransportId] = new EntityReference(EntityName.BookingTransport, new Guid(listSucMsg[i].Id));

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
                    new string[] { globalVariable.BookingId, globalVariable.CustomerId },
                    globalVariable);

            }

            Entity entCustBookingRole = new Entity(EntityName.CustomerBookingRole);
            entCustBookingRole[Attributes.CustomerBookingRole.BookingId] = new EntityReference(EntityName.Booking, Attributes.Booking.Name, payloadInfo.bookingIdentifier.BookingNumber);
            if (globalVariable.IsCustomerTypeAccount)
            {
                entCustBookingRole[Attributes.CustomerBookingRole.Customer] = new EntityReference(EntityName.Account, Attributes.Account.SourceSystemID, payloadInfo.customer.CustomerIdentifier.CustomerId);
            }
            else
            {
                entCustBookingRole[Attributes.CustomerBookingRole.Customer] = new EntityReference(EntityName.Contact, Attributes.Contact.SourceSystemID, payloadInfo.customer.CustomerIdentifier.CustomerId);
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

        public string Seperator { get { return ","; } }

        public string NextLine { get { return "\r\n"; } }

        public bool IsCustomerTypeAccount { get; set; }

        public bool DeleteBookingRole { get; set; }

        public bool DeleteAccomdOrTrnsprt { get; set; }

        public string BookingId { get; set; }

        public string CustomerId { get; set; }

    }
}
