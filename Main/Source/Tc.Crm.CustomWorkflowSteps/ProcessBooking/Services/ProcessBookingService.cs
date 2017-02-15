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

            if (payloadBooking == null) throw new InvalidPluginExecutionException("Booking object created from payload json is null;");

            payloadBooking.DeleteBookingRole = true;
            payloadBooking.DeleteAccommodationOrTransportOrRemarks = true;

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

            var contactToDeActivate = ContactHelper.DeActivateContact(customer, Guid.Parse(payloadBooking.CustomerId), trace);
            if (contactToDeActivate != null)
                CommonXrm.UpsertEntity(contactToDeActivate, payloadBooking.CrmService);           

           
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
                payloadBooking.DeleteAccommodationOrTransportOrRemarks = false;
                payloadBooking.Response.Created = true;
            }
            payloadBooking.BookingId = xrmResponse.Id;
            payloadBooking.Response.Id = xrmResponse.Id;

            var bookingToDeactivate = BookingHelper.DeActivateBooking(payloadBooking.BookingInfo, Guid.Parse(payloadBooking.BookingId), trace);
            if (bookingToDeactivate != null)
                CommonXrm.UpsertEntity(bookingToDeactivate,payloadBooking.CrmService);

            
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
                CommonXrm.BulkCreate(entityCollectionRemarks, payloadBooking.CrmService);
            }
            trace.Trace("Remarks information - end");
        }


        /// <summary>
        /// To process accomodation informaation
        /// </summary>      
        /// <returns></returns>
        public void ProcessAccomodation()
        {
            if (payloadBooking.DeleteAccommodationOrTransportOrRemarks)
            {
                payloadBooking.Trace.Trace("Delete Accommodation information - start");
                ProcessRecordsToDelete(EntityName.BookingAccommodation,
                    new string[] { Attributes.BookingAccommodation.BookingAccommodationid },
                    new string[] { Attributes.BookingAccommodation.BookingId },
                    new string[] { payloadBooking.BookingId });
                payloadBooking.Trace.Trace("Delete Accommodation information - end");
            }

            if (payloadBooking.BookingInfo.Services != null && payloadBooking.BookingInfo.Services.Accommodation != null)
            {
                payloadBooking.Trace.Trace("Booking Accommodation information - start");                
                var entityCollectionAccomodation = BookingAccommodationHelper.GetBookingAccommodationEntityFromPayload(payloadBooking.BookingInfo, Guid.Parse(payloadBooking.BookingId), trace);
                List<XrmResponse> xrmResponseList = CommonXrm.BulkCreate(entityCollectionAccomodation, payloadBooking.CrmService);
                payloadBooking.Trace.Trace("Booking Accommodation information - end");
                var accommodation = payloadBooking.BookingInfo.Services.Accommodation;
                var bookingAccommodationToDeactivate = BookingAccommodationHelper.DeActivateBookingAccommodation(accommodation, xrmResponseList, trace);
                if (bookingAccommodationToDeactivate != null)
                {
                    foreach (Entity entityBookingAccomodation in bookingAccommodationToDeactivate.Entities)
                    {
                        CommonXrm.UpsertEntity(entityBookingAccomodation, payloadBooking.CrmService);
                    }
                }
                ProcessAccomodationRemarks(xrmResponseList);
            }
        }

        /// <summary>
        /// To process accomodation remarks
        /// </summary>       
        /// <param name="xrmResponseList"></param>
        public void ProcessAccomodationRemarks(List<XrmResponse> xrmResponseList)
        {
            EntityCollection entityCollectionAccomodationRemarks = null;           
            if (payloadBooking.BookingInfo.Services != null && payloadBooking.BookingInfo.Services.Accommodation != null && xrmResponseList.Count > 0)
            {
                payloadBooking.Trace.Trace("Accommodation Remarks information - Start");
                string bookingNumber = payloadBooking.BookingInfo.BookingIdentifier.BookingNumber;
                for (int i = 0; i < payloadBooking.BookingInfo.Services.Accommodation.Length; i++)
                {  
                    if(payloadBooking.BookingInfo.Services.Accommodation[i].Remark != null)                 
                    for (int j = 0; j < payloadBooking.BookingInfo.Services.Accommodation[i].Remark.Length; j++)
                    {
                        entityCollectionAccomodationRemarks = RemarksHelper.GetRemarksEntityFromPayload(bookingNumber, payloadBooking.BookingInfo.Services.Accommodation[i].Remark, Guid.Parse(xrmResponseList[i].Id), trace, RemarkType.AccomodationRemark);
                    }                   
                }

                if (entityCollectionAccomodationRemarks.Entities.Count > 0)
                    CommonXrm.BulkCreate(entityCollectionAccomodationRemarks, payloadBooking.CrmService);
                payloadBooking.Trace.Trace("Accommodation Remarks information - End");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ProcessTransport()
        {
            if (payloadBooking.BookingInfo.Services != null && payloadBooking.BookingInfo.Services.Transport != null)
            {  
                if (payloadBooking.DeleteAccommodationOrTransportOrRemarks)
                {
                    payloadBooking.Trace.Trace("Delete Transport information - start");
                    ProcessRecordsToDelete(EntityName.BookingTransport,
                        new string[] { Attributes.BookingTransport.BookingTransportId },
                        new string[] { Attributes.BookingAccommodation.BookingId },
                        new string[] { payloadBooking.BookingId });
                    payloadBooking.Trace.Trace("Delete Transport information - end");
                }

                if (payloadBooking.BookingInfo.Services != null && payloadBooking.BookingInfo.Services.Transport != null)
                {
                    payloadBooking.Trace.Trace("Transport information - start");
                    var entityCollectionTransport = BookingTransportHelper.GetTransportEntityForBookingPayload(payloadBooking.BookingInfo, Guid.Parse(payloadBooking.BookingId), trace);
                    List<XrmResponse> xrmResponseList = CommonXrm.BulkCreate(entityCollectionTransport, payloadBooking.CrmService);
                    ProcessTransportRemarks(xrmResponseList);
                    payloadBooking.Trace.Trace("Transport information - end");
                }

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
                EntityCollection entityColllectionTransportRemark = null;
                string bookingNumber = payloadBooking.BookingInfo.BookingIdentifier.BookingNumber;
                payloadBooking.Trace.Trace("Transport Remarks information - Start");
                for (int i = 0; i < payloadBooking.BookingInfo.Services.Transport.Length; i++)
                {
                    if(payloadBooking.BookingInfo.Services.Transport[i].Remark != null)
                    for (int j = 0; j < payloadBooking.BookingInfo.Services.Transport[i].Remark.Length; j++)
                    {
                        //BN - Type
                        entityColllectionTransportRemark = RemarksHelper.GetRemarksEntityFromPayload(bookingNumber, payloadBooking.BookingInfo.Services.Transport[i].Remark, Guid.Parse(xrmResponseList[i].Id), trace, RemarkType.TransportRemark);
                    }
                }

                if (entityColllectionTransportRemark.Entities.Count > 0)
                    CommonXrm.BulkCreate(entityColllectionTransportRemark, payloadBooking.CrmService);
                payloadBooking.Trace.Trace("Transport Remarks information - End");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public void ProcessBookingRole()
        {

            if (payloadBooking.DeleteBookingRole)
            {
                payloadBooking.Trace.Trace("Delete Booking Roles information - start");
                ProcessRecordsToDelete(EntityName.CustomerBookingRole,
                    new string[] { Attributes.CustomerBookingRole.CustomerBookingRoleId },
                    new string[] { Attributes.CustomerBookingRole.BookingId, Attributes.CustomerBookingRole.Customer },
                    new string[] { payloadBooking.BookingId, payloadBooking.CustomerId });
                payloadBooking.Trace.Trace("Delete Booking Roles information - end");
            }
            payloadBooking.Trace.Trace("Booking Roles information - start");
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
            payloadBooking.Trace.Trace("Booking Roles information - end");
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
            CommonXrm.DeleteRecords(entityName, columns, filterKeys, filterValues, payloadBooking.CrmService);
        }


    }

    
}
