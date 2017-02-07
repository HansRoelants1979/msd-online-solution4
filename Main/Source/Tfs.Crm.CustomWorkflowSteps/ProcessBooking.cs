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

        CommonXrm _xrm = null;
        string _seperator = ",";
        string _nextLine = "\r\n";

        /// <summary>
        /// To process booking data
        /// </summary>
        /// <param name="json"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public string ProcessPayload(string json, IOrganizationService service)
        {
            string response = string.Empty;
            _xrm = new CommonXrm();
            _xrm._service = service;

            PayloadBooking payloadInfo = DeSerializeJson(json);
            List<SuccessMessage> successMsg = new List<SuccessMessage>();

            //Need to update this condition based on customer type
            if (payloadInfo.CustomerInfo.CustomerGeneral.CustomerType == PayloadBooking.AccountType)
            {
                ProcessAccount(payloadInfo);
            }
            else if (payloadInfo.CustomerInfo.CustomerGeneral.CustomerType == PayloadBooking.ContactType)
            {
                ProcessContact(payloadInfo);
            }

            ProcessBookingInfo(payloadInfo);
            ProcessAccomodation(payloadInfo);
            

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
        void ProcessContact(PayloadBooking payloadInfo)
        {
            
            Entity contactEntity = new Entity(EntityName.Contact, "", "");
            contactEntity[Attributes.Contact.FirstName] = payloadInfo.BookingInfo.Customer.CustomerIdentity.FirstName;
            contactEntity[Attributes.Contact.MiddleName] = payloadInfo.BookingInfo.Customer.CustomerIdentity.MiddleName;
            contactEntity[Attributes.Contact.LastName] = payloadInfo.BookingInfo.Customer.CustomerIdentity.LastName;
            contactEntity[Attributes.Contact.Language] = payloadInfo.BookingInfo.Customer.CustomerIdentity.Language;
            contactEntity[Attributes.Contact.Gender] = payloadInfo.BookingInfo.Customer.CustomerIdentity.Gender;
            contactEntity[Attributes.Contact.AcademicTitle] = payloadInfo.BookingInfo.Customer.CustomerIdentity.Academictitle;
            contactEntity[Attributes.Contact.Salutation] = payloadInfo.BookingInfo.Customer.CustomerIdentity.Salutation;
            contactEntity[Attributes.Contact.BirthDate] = payloadInfo.BookingInfo.Customer.CustomerIdentity.Birthdate;
            contactEntity[Attributes.Contact.StatusCode] = payloadInfo.BookingInfo.Customer.CustomerGeneral.CustomerStatus;
            // couldn't find segment,DateofDeath in booking.customer.identity.additional
            contactEntity[Attributes.Contact.Segment] = payloadInfo.BookingInfo.Customer.Additional.Segment;
            contactEntity[Attributes.Contact.DateofDeath] = payloadInfo.BookingInfo.Customer.Additional.DateOfdeath;
            
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
            contactEntity[Attributes.Contact.Telephone2Type] = payloadInfo.BookingInfo.Customer.Phone[0].Type;
            contactEntity[Attributes.Contact.Telephone1] = payloadInfo.BookingInfo.Customer.Phone[0].Number;
            contactEntity[Attributes.Contact.Telephone2Type] = payloadInfo.BookingInfo.Customer.Phone[1].Type;
            contactEntity[Attributes.Contact.Telephone2] = payloadInfo.BookingInfo.Customer.Phone[1].Number;
            contactEntity[Attributes.Contact.Telephone3Type] = payloadInfo.BookingInfo.Customer.Phone[2].Type;
            contactEntity[Attributes.Contact.Telephone3] = payloadInfo.BookingInfo.Customer.Phone[2].Type;
            contactEntity[Attributes.Contact.EMailAddress1] = payloadInfo.BookingInfo.Customer.Email[0].Address;
            contactEntity[Attributes.Contact.EmailAddress1Type] = payloadInfo.BookingInfo.Customer.Email[0].Type;
            contactEntity[Attributes.Contact.EMailAddress2] = payloadInfo.BookingInfo.Customer.Email[1].Address;
            contactEntity[Attributes.Contact.EmailAddress2Type] = payloadInfo.BookingInfo.Customer.Email[1].Type;
            contactEntity[Attributes.Contact.EMailAddress3] = payloadInfo.BookingInfo.Customer.Email[2].Address;
            contactEntity[Attributes.Contact.EmailAddress3Type] = payloadInfo.BookingInfo.Customer.Email[2].Type;
            contactEntity[Attributes.Contact.SourceMarketId] = payloadInfo.BookingInfo.Customer.CustomerIdentifier.SourceMarket;
           
            _xrm.UpsertEntity(contactEntity);
        }

        /// <summary>
        /// To process account information
        /// </summary>
        /// <param name="bookingInfo"></param>
        /// <returns></returns>
        void ProcessAccount(PayloadBooking payloadInfo)
        {
            
            Entity accountEntity = new Entity(EntityName.Account, "", "");
            accountEntity[Attributes.Account.Name] = payloadInfo.BookingInfo.Customer.Company;
            accountEntity[Attributes.Account.Address1_AdditionalInformation] = payloadInfo.BookingInfo.Customer.Address[0].AdditionalAddressInfo;
            accountEntity[Attributes.Account.Address1_FlatOrUnitNumber] = payloadInfo.BookingInfo.Customer.Address[0].FlatNumberUnit;
            accountEntity[Attributes.Account.Address1_HouseNumberOrBuilding] = payloadInfo.BookingInfo.Customer.Address[0].HouseNumberBuilding;
            accountEntity[Attributes.Account.Address1_Town] = payloadInfo.BookingInfo.Customer.Address[0].Town;
            accountEntity[Attributes.Account.Address1_PostalCode] = payloadInfo.BookingInfo.Customer.Address[0].PostalCode;
            accountEntity[Attributes.Account.Address1_CountryId] = payloadInfo.BookingInfo.Customer.Address[0].Country;
            accountEntity[Attributes.Account.Address1_Count] = payloadInfo.BookingInfo.Customer.Address[0].County;
            accountEntity[Attributes.Account.Telephone1_Type] = payloadInfo.BookingInfo.Customer.Phone[0].Type;
            accountEntity[Attributes.Account.Telephone1] = payloadInfo.BookingInfo.Customer.Phone[0].Number;
            accountEntity[Attributes.Account.Telephone2_Type] = payloadInfo.BookingInfo.Customer.Phone[1].Type;
            accountEntity[Attributes.Account.Telephone2] = payloadInfo.BookingInfo.Customer.Phone[1].Number;
            accountEntity[Attributes.Account.Telephone3_Type] = payloadInfo.BookingInfo.Customer.Phone[2].Type;
            accountEntity[Attributes.Account.Telephone3] = payloadInfo.BookingInfo.Customer.Phone[2].Type;
            accountEntity[Attributes.Account.EMailAddress1] = payloadInfo.BookingInfo.Customer.Email[0].Address;
            accountEntity[Attributes.Account.EmailAddress1_Type] = payloadInfo.BookingInfo.Customer.Email[0].Type;
            accountEntity[Attributes.Account.EMailAddress2] = payloadInfo.BookingInfo.Customer.Email[1].Address;
            accountEntity[Attributes.Account.EmailAddress2_Type] = payloadInfo.BookingInfo.Customer.Email[1].Type;
            accountEntity[Attributes.Account.EMailAddress3] = payloadInfo.BookingInfo.Customer.Email[2].Address;
            accountEntity[Attributes.Account.EmailAddress3_Type] = payloadInfo.BookingInfo.Customer.Email[2].Type;
            

             _xrm.UpsertEntity(accountEntity);
        }

        /// <summary>
        /// To process booking information
        /// </summary>
        /// <param name="payloadInfo"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        void ProcessBookingInfo(PayloadBooking payloadInfo)
        {
            if (payloadInfo.BookingInfo.BookingIdentifier != null)
            {

                Entity bookingEntity = new Entity(EntityName.Booking, "", "");

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
                bookingEntity[Attributes.Booking.NumberofParticipants] = payloadInfo.BookingInfo.BookingGeneral.NumberofParticipants;
                bookingEntity[Attributes.Booking.NumberofAdults] = payloadInfo.BookingInfo.BookingGeneral.NumberOfAdults;
                bookingEntity[Attributes.Booking.NumberofChildren] = payloadInfo.BookingInfo.BookingGeneral.NumberOfChildren;
                bookingEntity[Attributes.Booking.NumberofInfants] = payloadInfo.BookingInfo.BookingGeneral.NumberOfInfants;
                bookingEntity[Attributes.Booking.BookerPhone1] = payloadInfo.BookingInfo.BookingIdentity.Booker.Phone;
                bookingEntity[Attributes.Booking.BookerPhone2] = payloadInfo.BookingInfo.BookingIdentity.Booker.Mobile;
                bookingEntity[Attributes.Booking.BookerEmergencyPhone] = payloadInfo.BookingInfo.BookingIdentity.Booker.EmergencyNumber;
                bookingEntity[Attributes.Booking.Participants] = PrepareTravelParticipantsInfo(payloadInfo);
                bookingEntity[Attributes.Booking.ParticipantRemarks] = PrepareTravelParticipantsRemarks(payloadInfo);
                bookingEntity[Attributes.Booking.Transfer] = PrepareTransferInfo(payloadInfo);
                bookingEntity[Attributes.Booking.TransferRemarks] = PrepareTransferRemarks(payloadInfo);
                bookingEntity[Attributes.Booking.ExtraService] = PrepareExtraServicesInfo(payloadInfo);
                bookingEntity[Attributes.Booking.ExtraServiceRemarks] = PrepareExtraServiceRemarks(payloadInfo);

                _xrm.UpsertEntity(bookingEntity);
            }
        }

        /// <summary>
        /// To prepare travel participants information
        /// </summary>
        /// <param name="payloadInfo"></param>
        /// <returns></returns>
        string PrepareTravelParticipantsInfo(PayloadBooking payloadInfo)
        {
            string travelParticipants = string.Empty;
            if(payloadInfo.BookingInfo.TravelParticipant != null)
            for (int i = 0; i < payloadInfo.BookingInfo.TravelParticipant.Length; i++)
            {
                if (!string.IsNullOrEmpty(travelParticipants))
                    travelParticipants += _nextLine;

                travelParticipants += payloadInfo.BookingInfo.TravelParticipant[i].TravelParticipantIDOnTour + _seperator +
                                      payloadInfo.BookingInfo.TravelParticipant[i].FirstName + _seperator +
                                      payloadInfo.BookingInfo.TravelParticipant[i].LastName + _seperator +
                                      payloadInfo.BookingInfo.TravelParticipant[i].Age + _seperator +
                                      payloadInfo.BookingInfo.TravelParticipant[i].BirthDate + _seperator +
                                      payloadInfo.BookingInfo.TravelParticipant[i].Gender + _seperator +
                                      payloadInfo.BookingInfo.TravelParticipant[i].Relation + _seperator +
                                      payloadInfo.BookingInfo.TravelParticipant[i].Language;

            }
            return travelParticipants;
        }

        /// <summary>
        /// To prepare travel participants remarks information
        /// </summary>
        /// <param name="payloadInfo"></param>
        /// <returns></returns>
        string PrepareTravelParticipantsRemarks(PayloadBooking payloadInfo)
        {
            string remarks = string.Empty;
            if(payloadInfo.BookingInfo.TravelParticipant != null)
            for (int i = 0; i < payloadInfo.BookingInfo.TravelParticipant.Length; i++)
            {
                if(payloadInfo.BookingInfo.TravelParticipant[i].Remark != null)
                for (int j = 0; j < payloadInfo.BookingInfo.TravelParticipant[i].Remark.Length; j++)
                {
                    if (!string.IsNullOrEmpty(remarks))
                        remarks += _nextLine;

                    remarks += payloadInfo.BookingInfo.TravelParticipant[i].TravelParticipantIDOnTour + _seperator +
                               payloadInfo.BookingInfo.TravelParticipant[i].Remark[j].Type + _seperator +
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
        string PrepareTransferInfo(PayloadBooking payloadInfo)
        {
            string transferInfo = string.Empty;
            if(payloadInfo.BookingInfo.Services != null && payloadInfo.BookingInfo.Services.Transfer != null)
            for (int i = 0; i < payloadInfo.BookingInfo.Services.Transfer.Length; i++)
            {
                if (!string.IsNullOrEmpty(transferInfo))
                    transferInfo += _nextLine;

                transferInfo += payloadInfo.BookingInfo.Services.Transfer[i].TransferCode + _seperator +
                                payloadInfo.BookingInfo.Services.Transfer[i].TransferDescription + _seperator +
                                payloadInfo.BookingInfo.Services.Transfer[i].Order + _seperator +
                                payloadInfo.BookingInfo.Services.Transfer[i].StartDate + _seperator +
                                payloadInfo.BookingInfo.Services.Transfer[i].EndDate + _seperator +
                                payloadInfo.BookingInfo.Services.Transfer[i].Category + _seperator +
                                payloadInfo.BookingInfo.Services.Transfer[i].TransferType + _seperator +
                                payloadInfo.BookingInfo.Services.Transfer[i].DepartureAirport + _seperator +
                                payloadInfo.BookingInfo.Services.Transfer[i].ArrivalAirport;


            }
            return transferInfo;
        }

        /// <summary>
        /// To prepare trasnfer remarks information
        /// </summary>
        /// <param name="payloadInfo"></param>
        /// <returns></returns>
        string PrepareTransferRemarks(PayloadBooking payloadInfo)
        {
            string remarks = string.Empty;
            if(payloadInfo.BookingInfo.Services != null && payloadInfo.BookingInfo.Services.Transfer != null)
            for (int i = 0; i < payloadInfo.BookingInfo.Services.Transfer.Length; i++)
            {
                if(payloadInfo.BookingInfo.Services.Transfer[i].Remark != null)
                for (int j = 0; j < payloadInfo.BookingInfo.Services.Transfer[i].Remark.Length; j++)
                {
                    if (!string.IsNullOrEmpty(remarks))
                        remarks += _nextLine;

                    remarks += payloadInfo.BookingInfo.Services.Transfer[i].Remark[j].Type + _seperator +
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
        string PrepareExtraServicesInfo(PayloadBooking payloadInfo)
        {
            string extraServices = string.Empty;
            if(payloadInfo.BookingInfo.Services != null && payloadInfo.BookingInfo.Services.ExtraService != null)
            for (int i = 0; i < payloadInfo.BookingInfo.Services.ExtraService.Length; i++)
            {
               
                    if (!string.IsNullOrEmpty(extraServices))
                        extraServices += _nextLine;

                extraServices += payloadInfo.BookingInfo.Services.ExtraService[i].ExtraServiceCode + _seperator +
                                 payloadInfo.BookingInfo.Services.ExtraService[i].ExtraServiceDescription + _seperator +
                                 payloadInfo.BookingInfo.Services.ExtraService[i].Order + _seperator +
                                 payloadInfo.BookingInfo.Services.ExtraService[i].StartDate + _seperator +
                                 payloadInfo.BookingInfo.Services.ExtraService[i].EndDate;


            }
            return extraServices;
        }

        /// <summary>
        /// To prepare extra service remarks information
        /// </summary>
        /// <param name="payloadInfo"></param>
        /// <returns></returns>
        string PrepareExtraServiceRemarks(PayloadBooking payloadInfo)
        {
            string extraServiceRemarks = string.Empty;
            if(payloadInfo.BookingInfo.Services != null && payloadInfo.BookingInfo.Services.ExtraService != null)
            for (int i = 0; i < payloadInfo.BookingInfo.Services.ExtraService.Length; i++)
            {
                if(payloadInfo.BookingInfo.Services.ExtraService[i].Remark != null)
                for (int j = 0; j < payloadInfo.BookingInfo.Services.ExtraService[i].Remark.Length; j++)
                {
                    if (!string.IsNullOrEmpty(extraServiceRemarks))
                        extraServiceRemarks += _nextLine;

                    extraServiceRemarks += payloadInfo.BookingInfo.Services.ExtraService[i].Remark[j].Type + _seperator +
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
        void ProcessAccomodation(PayloadBooking payloadInfo)
        {
            if (payloadInfo.BookingInfo.Services != null && payloadInfo.BookingInfo.Services.Accommodation != null)
            {
                EntityCollection entColAccomodation = new EntityCollection();
                Entity accomodationEntity = null;
                for (int i = 0; i < payloadInfo.BookingInfo.Services.Accommodation.Length; i++)
                {
                    accomodationEntity = new Entity(EntityName.BookingAccommodation, "", "");
                    accomodationEntity[Attributes.BookingAccommodation.HotelId] = _xrm.SetLookupValueUsingAlternateKey(EntityName.Hotel, "", payloadInfo.BookingInfo.Services.Accommodation[i].GroupAccommodationCode);
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

                    entColAccomodation.Entities.Add(accomodationEntity);

                }

                _xrm.BulkCreate(entColAccomodation);
            }
        }

        /// <summary>
        /// To process accomodation remarks
        /// </summary>
        /// <param name="payloadInfo"></param>
        /// <param name="messageList"></param>
        void ProcessAccomodationRemarks(PayloadBooking payloadInfo, List<SuccessMessage> messageList)
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
                            remarkEntity[Attributes.Remark.Type] = payloadInfo.BookingInfo.Services.Accommodation[i].Remark[j].Type;
                            remarkEntity[Attributes.Remark.RemarkName] = payloadInfo.BookingInfo.Services.Accommodation[i].Remark[j].Text;
                            //messageList.Find()
                            remarkEntity[Attributes.Remark.BookingAccommodationId] = new EntityReference(EntityName.BookingAccommodation, Guid.Parse(messageList[i].Id));
                            entColAccomodationRemarks.Entities.Add(remarkEntity);
                        }
                    }
                }

                if (entColAccomodationRemarks.Entities.Count > 0)
                    _xrm.BulkCreate(entColAccomodationRemarks);
            }
        }

        

    }
}
