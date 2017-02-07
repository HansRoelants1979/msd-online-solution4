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
                successMsg.Add(ProcessAccount(payloadInfo));
            }
            else if (payloadInfo.CustomerInfo.CustomerGeneral.CustomerType == PayloadBooking.ContactType)
            {
                successMsg.Add(ProcessContact(payloadInfo));
            }

            successMsg.Add(ProcessBookingInfo(payloadInfo));
            successMsg.AddRange(ProcessAccomodation(payloadInfo));
            

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
        SuccessMessage ProcessContact(PayloadBooking payloadInfo)
        {
            
            Entity contactEntity = new Entity(EntityName.Contact, "", "");
            
           
            return _xrm.UpsertEntity(contactEntity);
        }

        /// <summary>
        /// To process account information
        /// </summary>
        /// <param name="bookingInfo"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        SuccessMessage ProcessAccount(PayloadBooking payloadInfo)
        {
            
            Entity accountEntity = new Entity(EntityName.Account, "", "");
            

            return _xrm.UpsertEntity(accountEntity);
        }


        /// <summary>
        /// To process booking information
        /// </summary>
        /// <param name="payloadInfo"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        SuccessMessage ProcessBookingInfo(PayloadBooking payloadInfo)
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
            bookingEntity[Attributes.Booking.DestinationGateway] = _xrm.SetLookupValueUsingAlternateKey(EntityName.Gateway,"", payloadInfo.BookingInfo.BookingGeneral.Destination);
            bookingEntity[Attributes.Booking.TourOperatorId] = _xrm.SetLookupValueUsingAlternateKey(EntityName.TourOperator,"", payloadInfo.BookingInfo.BookingGeneral.ToCode);
            bookingEntity[Attributes.Booking.BrandId] = _xrm.SetLookupValueUsingAlternateKey(EntityName.Brand, "", payloadInfo.BookingInfo.BookingGeneral.Brand);
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

            return _xrm.UpsertEntity(bookingEntity);
        }

        /// <summary>
        /// To prepare travel participants information
        /// </summary>
        /// <param name="payloadInfo"></param>
        /// <returns></returns>
        string PrepareTravelParticipantsInfo(PayloadBooking payloadInfo)
        {
            string travelParticipants = string.Empty;
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
            for (int i = 0; i < payloadInfo.BookingInfo.TravelParticipant.Length; i++)
            {
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
            for (int i = 0; i < payloadInfo.BookingInfo.Services.Transfer.Length; i++)
            {
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



        List<SuccessMessage> ProcessAccomodation(PayloadBooking bookingInfo)
        {
            Entity accomodationEntity = new Entity(EntityName.BookingAccommodation, "", "");
            //accomodationEntity[Attributes.BookingAccommodation.Name] = 

            return _xrm.BulkCreate(null);
        }

    }
}
