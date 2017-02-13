using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services
{
    public class ProcessBookingService
    {
        private PayloadBooking payloadBooking;
        private ITracingService trace;

        public ProcessBookingService(PayloadBooking payloadBooking)
        {
            this.payloadBooking = payloadBooking;
            this.trace = payloadBooking.Trace;
        }
        /// <summary>
        /// To process booking data
        /// </summary>
        /// <param name="json"></param>       
        /// <returns></returns>
        public string ProcessPayload()
        {
            trace.Trace("Processing Process payload - start");

            if (this.payloadBooking == null) throw new InvalidPluginExecutionException("Booking object created from payload json is null;");

            payloadBooking.DeleteBookingRole = true;
            payloadBooking.DeleteAccomodationOrTransportOrRemarks = true;

            ProcessCustomer();
            ProcessBookingInfo();
            ProcessRemarks();
            ProcessAccomodation();
            ProcessTransport();
            ProcessBookingRole();

            trace.Trace("Processing Process payload - end");
            return JsonHelper.SerializeJson(payloadBooking.Response,trace);
        }

        private void ProcessCustomer()
        {
            //Validate payload for customer
            if (payloadBooking.BookingInfo.Customer == null)
                throw new InvalidPluginExecutionException("Customer info missing in payload.");
            if (payloadBooking.BookingInfo.Customer.CustomerIdentifier == null)
                throw new InvalidPluginExecutionException("Customer Identifier is missing.");
            if (string.IsNullOrWhiteSpace(payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId))
                throw new InvalidPluginExecutionException("Customer source system id is missing.");

            if (payloadBooking.BookingInfo.Customer.CustomerGeneral.CustomerType == CustomerType.B)
            {
                ProcessAccount();
            }
            else if (payloadBooking.BookingInfo.Customer.CustomerGeneral.CustomerType == CustomerType.P)
            {
                ProcessContact();
            }
        }

        /// <summary>
        /// To process contact information
        /// </summary>       
        /// <returns></returns>
        public void ProcessContact()
        {
            trace.Trace("Processing Contact information - start");
            XrmResponse xrmResponse = null;
            
            var customer = payloadBooking.BookingInfo.Customer;
            var contact = ContactHelper.GetContactEntityForBookingPayload(customer, trace);
            xrmResponse = CommonXrm.UpsertEntity(contact, payloadBooking.CrmService);

            if (xrmResponse.Create)
                payloadBooking.DeleteBookingRole = false;

            payloadBooking.CustomerId = xrmResponse.Id;
            trace.Trace("Processing Contact information - end");
        }

        /// <summary>
        /// To process account information
        /// </summary>        
        /// <returns></returns>
        public void ProcessAccount()
        {
            trace.Trace("Processing Account information - start");
            XrmResponse xrmResponse = null;
            
            var customer = payloadBooking.BookingInfo.Customer;
            var account = AccountHelper.GetAccountEntityForBookingPayload(customer, trace);
            xrmResponse = CommonXrm.UpsertEntity(account, payloadBooking.CrmService);
            if (xrmResponse.Create)
                payloadBooking.DeleteBookingRole = false;

            payloadBooking.CustomerId = xrmResponse.Id;
            trace.Trace("Processing Account information - end");
        }


        /// <summary>
        /// To process booking information
        /// </summary>      
        /// <returns></returns>
        public void ProcessBookingInfo()
        {
            trace.Trace("Processing Booking - start");
            XrmResponse xrmResponse = null;
            var bookingEntity = BookingHelper.GetBookingEntityFromPayload(payloadBooking.BookingInfo, payloadBooking.Trace);
            xrmResponse = CommonXrm.UpsertEntity(bookingEntity, payloadBooking.CrmService);
            payloadBooking.Response = new BookingResponse();
            if (xrmResponse.Create)
            {
                payloadBooking.DeleteBookingRole = false;
                payloadBooking.DeleteAccomodationOrTransportOrRemarks = false;
                payloadBooking.Response.Created = true;

            }

            payloadBooking.BookingId = xrmResponse.Id;
            payloadBooking.Response.Id = xrmResponse.Id;
            trace.Trace("Processing Booking - end");
        }


        /// <summary>
        /// 
        /// </summary>
        public void ProcessRemarks()
        {
            trace.Trace("Processing Remarks information - start");
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
                    remarkEntity[Attributes.Remark.Type] = CommonXrm.GetOptionSetValue(payloadBooking.BookingInfo.Remark[i].RemarkType.ToString(), Attributes.Remark.Type);
                    remarkEntity[Attributes.Remark.RemarkName] = payloadBooking.BookingInfo.Remark[i].Text;
                    remarkEntity[Attributes.Remark.BookingId] = new EntityReference(EntityName.Booking, new Guid(payloadBooking.BookingId));
                    entityCollectionRemarks.Entities.Add(remarkEntity);

                }
                CommonXrm.BulkCreate(entityCollectionRemarks, payloadBooking.CrmService);
            }

            trace.Trace("Processing Remarks information - end");
        }


        /// <summary>
        /// To process accomodation informaation
        /// </summary>      
        /// <returns></returns>
        public void ProcessAccomodation()
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
                    accomodationEntity[Attributes.BookingAccommodation.BoardType] = CommonXrm.GetOptionSetValue(payloadBooking.BookingInfo.Services.Accommodation[i].BoardType.ToString(), Attributes.BookingAccommodation.BoardType);
                    accomodationEntity[Attributes.BookingAccommodation.HasSharedRoom] = payloadBooking.BookingInfo.Services.Accommodation[i].HasSharedRoom;
                    accomodationEntity[Attributes.BookingAccommodation.NumberofParticipants] = payloadBooking.BookingInfo.Services.Accommodation[i].NumberOfParticipants;
                    accomodationEntity[Attributes.BookingAccommodation.NumberofRooms] = payloadBooking.BookingInfo.Services.Accommodation[i].NumberOfRooms;
                    accomodationEntity[Attributes.BookingAccommodation.WithTransfer] = payloadBooking.BookingInfo.Services.Accommodation[i].WithTransfer;
                    accomodationEntity[Attributes.BookingAccommodation.IsExternalService] = payloadBooking.BookingInfo.Services.Accommodation[i].IsExternalService;
                    accomodationEntity[Attributes.BookingAccommodation.ExternalServiceCode] = CommonXrm.GetOptionSetValue(payloadBooking.BookingInfo.Services.Accommodation[i].ExternalServiceCode.ToString(), Attributes.BookingAccommodation.ExternalServiceCode);
                    accomodationEntity[Attributes.BookingAccommodation.NotificationRequired] = payloadBooking.BookingInfo.Services.Accommodation[i].NotificationRequired;
                    accomodationEntity[Attributes.BookingAccommodation.NeedTourGuideAssignment] = payloadBooking.BookingInfo.Services.Accommodation[i].NeedsTourGuideAssignment;
                    accomodationEntity[Attributes.BookingAccommodation.ExternalTransfer] = payloadBooking.BookingInfo.Services.Accommodation[i].IsExternalTransfer;
                    accomodationEntity[Attributes.BookingAccommodation.TransferServiceLevel] = CommonXrm.GetOptionSetValue(payloadBooking.BookingInfo.Services.Accommodation[i].TransferServiceLevel, Attributes.BookingAccommodation.TransferServiceLevel);
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
        public void ProcessAccomodationRemarks(List<XrmResponse> xrmResponseList)
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
                            remarkEntity[Attributes.Remark.Type] = CommonXrm.GetOptionSetValue(payloadBooking.BookingInfo.Services.Accommodation[i].Remark[j].RemarkType.ToString(), Attributes.Remark.Type);
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
        public void ProcessTransport()
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
                    transportEntity[Attributes.BookingTransport.TransferType] = CommonXrm.GetOptionSetValue(payloadBooking.BookingInfo.Services.Transport[i].TransferType.ToString(), Attributes.BookingTransport.TransferType);
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
        public void ProcessTransportRemarks(List<XrmResponse> xrmResponseList)
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
                        transportRemark[Attributes.Remark.Type] = CommonXrm.GetOptionSetValue(payloadBooking.BookingInfo.Services.Transport[i].Remark[j].RemarkType.ToString(), Attributes.Remark.Type);
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
        public void ProcessBookingRole()
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
        public void ProcessRecordsToDelete(string entityName, string[] columns, string[] filterKeys, string[] filterValues)

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


    }

    
}
