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
        private IOrganizationService crmService;

        public ProcessBookingService(PayloadBooking payloadBooking)
        {
            this.payloadBooking = payloadBooking;
            trace = payloadBooking.Trace;
            crmService = payloadBooking.CrmService;
        }
        /// <summary>
        /// To process booking data
        /// </summary>
        /// <param name="json"></param>       
        /// <returns></returns>
        public string ProcessPayload()
        {
            trace.Trace("Processing Process payload - start");

            if (payloadBooking == null) throw new InvalidPluginExecutionException("Booking object created from payload json is null;");

            payloadBooking.DeleteBookingRole = true;
            payloadBooking.DeleteAccommodationOrTransportOrRemarks = true;

            ProcessCustomer();
            ProcessBookingInfo();
            ProcessRemarks();
            ProcessAccommodation();
            ProcessTransport();
            ProcessBookingRole();

            trace.Trace("Processing Process payload - end");
            return JsonHelper.SerializeJson(payloadBooking.Response,trace);
        }

        private void ProcessCustomer()
        {
            //Validate payload for customer
            if (payloadBooking.BookingInfo == null)
                throw new InvalidPluginExecutionException("Booking info missing in payload.");
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

            if (payloadBooking.BookingInfo != null && payloadBooking.BookingInfo.Customer != null)
            {
                var customer = payloadBooking.BookingInfo.Customer;
                var contact = ContactHelper.GetContactEntityForBookingPayload(customer, trace);
                xrmResponse = CommonXrm.UpsertEntity(contact, crmService);

                if (xrmResponse.Create)
                    payloadBooking.DeleteBookingRole = false;

                payloadBooking.CustomerId = xrmResponse.Id;

                var contactToDeActivate = ContactHelper.DeActivateContact(customer, Guid.Parse(payloadBooking.CustomerId), trace);
                if (contactToDeActivate != null)
                    CommonXrm.UpsertEntity(contactToDeActivate, crmService);


                trace.Trace("Processing Contact information - end");
            }
        }

        /// <summary>
        /// To process account information
        /// </summary>        
        /// <returns></returns>
        public void ProcessAccount()
        {
            trace.Trace("Processing Account information - start");
            XrmResponse xrmResponse = null;

            if (payloadBooking.BookingInfo != null && payloadBooking.BookingInfo.Customer != null)
            {
                var customer = payloadBooking.BookingInfo.Customer;
                var account = AccountHelper.GetAccountEntityForBookingPayload(customer, trace);
                xrmResponse = CommonXrm.UpsertEntity(account, crmService);
                if (xrmResponse.Create)
                    payloadBooking.DeleteBookingRole = false;

                payloadBooking.CustomerId = xrmResponse.Id;
                trace.Trace("Processing Account information - end");
            }
        }


        /// <summary>
        /// To process booking information
        /// </summary>      
        /// <returns></returns>
        public void ProcessBookingInfo()
        {
            trace.Trace("Processing Booking - start");
            XrmResponse xrmResponse = null;
            var bookingEntity = BookingHelper.GetBookingEntityFromPayload(payloadBooking.BookingInfo, trace);
            xrmResponse = CommonXrm.UpsertEntity(bookingEntity, crmService);
            payloadBooking.Response = new BookingResponse();

            if (xrmResponse.Create)
            {
                payloadBooking.DeleteBookingRole = false;
                payloadBooking.DeleteAccommodationOrTransportOrRemarks = false;
                payloadBooking.Response.Created = true;
            }
            payloadBooking.BookingId = xrmResponse.Id;
            payloadBooking.Response.Id = xrmResponse.Id;

            var bookingToDeactivate = BookingHelper.DeActivateBooking(payloadBooking.BookingInfo, Guid.Parse(payloadBooking.BookingId), trace);
            if (bookingToDeactivate != null)
                CommonXrm.UpsertEntity(bookingToDeactivate,crmService);

            
            trace.Trace("Processing Booking - end");
        }


        /// <summary>
        /// 
        /// </summary>
        public void ProcessRemarks()
        {
            trace.Trace("Remarks information - start");
            if (payloadBooking.DeleteAccommodationOrTransportOrRemarks)
            {
                trace.Trace("Delete Remarks information - start");
                ProcessRecordsToDelete(EntityName.Remark,
                    new string[] { Attributes.Remark.RemarkId },
                    new string[] { Attributes.BookingAccommodation.BookingId },
                    new string[] { payloadBooking.BookingId });
                trace.Trace("Delete Remarks information - end");
            }
            if (payloadBooking.BookingInfo.Remark != null)
            {
                string bookingNumber = payloadBooking.BookingInfo.BookingIdentifier.BookingNumber;
                var entityCollectionRemarks = RemarksHelper.GetRemarksEntityFromPayload(bookingNumber, payloadBooking.BookingInfo.Remark, Guid.Parse(payloadBooking.BookingId), trace, RemarkType.Remark);
                CommonXrm.BulkCreate(entityCollectionRemarks, crmService);
            }
            trace.Trace("Remarks information - end");
        }


        /// <summary>
        /// To process accomodation informaation
        /// </summary>      
        /// <returns></returns>
        public void ProcessAccommodation()
        {
            
            if (payloadBooking.BookingInfo.Services == null) throw new InvalidPluginExecutionException("Booking Services is missing in payload.");
            if (payloadBooking.BookingInfo.Services.Accommodation == null) throw new InvalidPluginExecutionException("Booking Services Accommodation is missing in payload.");                    

            if (payloadBooking.BookingInfo.Services != null && payloadBooking.BookingInfo.Services.Accommodation != null)
            {
                if (payloadBooking.DeleteAccommodationOrTransportOrRemarks)
                {
                    trace.Trace("Delete Accommodation information - start");
                    ProcessRecordsToDelete(EntityName.BookingAccommodation,
                        new string[] { Attributes.BookingAccommodation.BookingAccommodationid },
                        new string[] { Attributes.BookingAccommodation.BookingId },
                        new string[] { payloadBooking.BookingId });
                    trace.Trace("Delete Accommodation information - end");
                }

                trace.Trace("Booking Accommodation information - start");
                string bookingNumber = payloadBooking.BookingInfo.BookingIdentifier.BookingNumber;
                var accommodation = payloadBooking.BookingInfo.Services.Accommodation;
                var entityCollectionAccommodation = BookingAccommodationHelper.GetBookingAccommodationEntityFromPayload(accommodation, bookingNumber, Guid.Parse(payloadBooking.BookingId), trace);
                List<XrmResponse> xrmResponseList = CommonXrm.BulkCreate(entityCollectionAccommodation, crmService);
                trace.Trace("Booking Accommodation information - end");                
                var bookingAccommodationToDeactivateList = BookingAccommodationHelper.DeActivateBookingAccommodation(accommodation, xrmResponseList, trace);
                if (bookingAccommodationToDeactivateList != null)
                {
                    foreach (Entity entityBookingAccomodation in bookingAccommodationToDeactivateList.Entities)
                    {
                        CommonXrm.UpsertEntity(entityBookingAccomodation, crmService);
                    }
                }
                ProcessAccommodationRemarks(xrmResponseList);
            }
        }

        /// <summary>
        /// To process accomodation remarks
        /// </summary>       
        /// <param name="xrmResponseList"></param>
        public void ProcessAccommodationRemarks(List<XrmResponse> xrmResponseList)
        {
            EntityCollection entityCollectionAccomodationRemarks = new EntityCollection();           
            if (payloadBooking.BookingInfo.Services != null && payloadBooking.BookingInfo.Services.Accommodation != null && xrmResponseList.Count > 0)
            {
                trace.Trace("Accommodation Remarks information - Start");
                string bookingNumber = payloadBooking.BookingInfo.BookingIdentifier.BookingNumber;
                for (int i = 0; i < payloadBooking.BookingInfo.Services.Accommodation.Length; i++)
                {
                    if (payloadBooking.BookingInfo.Services.Accommodation[i].Remark != null)
                    {
                        var accommodationRemarks = RemarksHelper.GetRemarksEntityFromPayload(bookingNumber, payloadBooking.BookingInfo.Services.Accommodation[i].Remark, Guid.Parse(xrmResponseList[i].Id), trace, RemarkType.AccomodationRemark);
                        entityCollectionAccomodationRemarks.Entities.AddRange(accommodationRemarks.Entities);
                    }
                                     
                }

                if (entityCollectionAccomodationRemarks.Entities.Count > 0)
                    CommonXrm.BulkCreate(entityCollectionAccomodationRemarks, crmService);
                trace.Trace("Accommodation Remarks information - End");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ProcessTransport()
        {            
            if (payloadBooking.BookingInfo.Services == null) throw new InvalidPluginExecutionException("Booking Services is missing in payload.");
            if (payloadBooking.BookingInfo.Services.Transport == null) throw new InvalidPluginExecutionException("Booking Services Transport is missing in payload.");

            if (payloadBooking.BookingInfo.Services != null && payloadBooking.BookingInfo.Services.Transport != null)
            {  
                if (payloadBooking.DeleteAccommodationOrTransportOrRemarks)
                {
                    trace.Trace("Delete Transport information - start");
                    ProcessRecordsToDelete(EntityName.BookingTransport,
                        new string[] { Attributes.BookingTransport.BookingTransportId },
                        new string[] { Attributes.BookingAccommodation.BookingId },
                        new string[] { payloadBooking.BookingId });
                    trace.Trace("Delete Transport information - end");
                }

                
                trace.Trace("Transport information - start");
                string bookingNumber = payloadBooking.BookingInfo.BookingIdentifier.BookingNumber;
                var transport = payloadBooking.BookingInfo.Services.Transport;
                var entityCollectionTransport = BookingTransportHelper.GetTransportEntityForBookingPayload(transport, bookingNumber, Guid.Parse(payloadBooking.BookingId), trace);
                List<XrmResponse> xrmResponseList = CommonXrm.BulkCreate(entityCollectionTransport, crmService);
                ProcessTransportRemarks(xrmResponseList);
                trace.Trace("Transport information - end");                

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xrmResponseList"></param>
        public void ProcessTransportRemarks(List<XrmResponse> xrmResponseList)
        {
            if (payloadBooking.BookingInfo.Services != null && payloadBooking.BookingInfo.Services.Transport != null && xrmResponseList.Count > 0)
            {
                EntityCollection entityColllectionTransportRemarks = new EntityCollection();
                string bookingNumber = payloadBooking.BookingInfo.BookingIdentifier.BookingNumber;
                trace.Trace("Transport Remarks information - Start");
                for (int i = 0; i < payloadBooking.BookingInfo.Services.Transport.Length; i++)
                {
                    if (payloadBooking.BookingInfo.Services.Transport[i].Remark != null)
                    {
                        //BN - Type
                        var transportRemarks = RemarksHelper.GetRemarksEntityFromPayload(bookingNumber, payloadBooking.BookingInfo.Services.Transport[i].Remark, Guid.Parse(xrmResponseList[i].Id), trace, RemarkType.TransportRemark);
                        entityColllectionTransportRemarks.Entities.AddRange(transportRemarks.Entities);
                    }
                    
                }

                if (entityColllectionTransportRemarks.Entities.Count > 0)
                    CommonXrm.BulkCreate(entityColllectionTransportRemarks, crmService);
                trace.Trace("Transport Remarks information - End");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public void ProcessBookingRole()
        {

            if (payloadBooking.DeleteBookingRole)
            {
                trace.Trace("Delete Booking Roles information - start");
                ProcessRecordsToDelete(EntityName.CustomerBookingRole,
                    new string[] { Attributes.CustomerBookingRole.CustomerBookingRoleId },
                    new string[] { Attributes.CustomerBookingRole.BookingId, Attributes.CustomerBookingRole.Customer },
                    new string[] { payloadBooking.BookingId, payloadBooking.CustomerId });
                trace.Trace("Delete Booking Roles information - end");
            }
            trace.Trace("Booking Roles information - start");
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

            CommonXrm.BulkCreate(entityCollection, crmService);
            trace.Trace("Booking Roles information - end");
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
            CommonXrm.DeleteRecords(entityName, columns, filterKeys, filterValues, crmService);
        }


    }

    
}
