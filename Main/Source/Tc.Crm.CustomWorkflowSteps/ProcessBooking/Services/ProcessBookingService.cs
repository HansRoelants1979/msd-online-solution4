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
            payloadBooking.DeleteAccommodationOrTransportOrRemarksOrTransferOrExtraService = true;

            ProcessCustomer();
            ProcessBookingInfo();
           // ProcessRemarks();
            ProcessAccommodation();
            ProcessTransport();
            ProcessTransfers();
            ProcessExtraServices();
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
                payloadBooking.DeleteAccommodationOrTransportOrRemarksOrTransferOrExtraService = false;
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
            if (payloadBooking.DeleteAccommodationOrTransportOrRemarksOrTransferOrExtraService)
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
                var entityCollectionRemarks = RemarksHelper.GetRemarksEntityFromPayload( payloadBooking.BookingInfo.Remark, Guid.Parse(payloadBooking.BookingId), trace, RemarkType.Remark);
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
                if (payloadBooking.DeleteAccommodationOrTransportOrRemarksOrTransferOrExtraService)
                {
                    trace.Trace("Delete Accommodation information - start");
                    ProcessRecordsToDelete(EntityName.BookingAccommodation,
                        new string[] { Attributes.BookingAccommodation.BookingAccommodationid },
                        new string[] { Attributes.BookingAccommodation.BookingId },
                        new string[] { payloadBooking.BookingId });
                    trace.Trace("Delete Accommodation information - end");
                }

                trace.Trace("Booking Accommodation information - start");
                var bookinginfo = payloadBooking.BookingInfo;
                var accommodation = payloadBooking.BookingInfo.Services.Accommodation;
                var entityCollectionAccommodation = BookingAccommodationHelper.GetBookingAccommodationEntityFromPayload(bookinginfo, Guid.Parse(payloadBooking.BookingId), trace);
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
                //string bookingNumber = payloadBooking.BookingInfo.BookingIdentifier.BookingNumber;
                for (int i = 0; i < payloadBooking.BookingInfo.Services.Accommodation.Length; i++)
                {
                    if (payloadBooking.BookingInfo.Services.Accommodation[i].Remark != null)
                    {
                        var accommodationRemarks = RemarksHelper.GetRemarksEntityFromPayload( payloadBooking.BookingInfo.Services.Accommodation[i].Remark, Guid.Parse(xrmResponseList[i].Id), trace, RemarkType.AccomodationRemark);
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
                if (payloadBooking.DeleteAccommodationOrTransportOrRemarksOrTransferOrExtraService)
                {
                    trace.Trace("Delete Transport information - start");
                    ProcessRecordsToDelete(EntityName.BookingTransport,
                        new string[] { Attributes.BookingTransport.BookingTransportId },
                        new string[] { Attributes.BookingAccommodation.BookingId },
                        new string[] { payloadBooking.BookingId });
                    trace.Trace("Delete Transport information - end");
                }

                
                trace.Trace("Transport information - start");
                var bookinginfo = payloadBooking.BookingInfo;
                var transport = payloadBooking.BookingInfo.Services.Transport;
                var entityCollectionTransport = BookingTransportHelper.GetTransportEntityForBookingPayload(bookinginfo, Guid.Parse(payloadBooking.BookingId), trace);
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
                        var transportRemarks = RemarksHelper.GetRemarksEntityFromPayload( payloadBooking.BookingInfo.Services.Transport[i].Remark, Guid.Parse(xrmResponseList[i].Id), trace, RemarkType.TransportRemark);
                        entityColllectionTransportRemarks.Entities.AddRange(transportRemarks.Entities);
                    }
                    
                }

                if (entityColllectionTransportRemarks.Entities.Count > 0)
                    CommonXrm.BulkCreate(entityColllectionTransportRemarks, crmService);
                trace.Trace("Transport Remarks information - End");
            }

        }

        public void ProcessTransfers()
        {
            if (payloadBooking.BookingInfo.Services == null) throw new InvalidPluginExecutionException("Booking Services is missing in payload.");
            if (payloadBooking.BookingInfo.Services.Transfer == null) throw new InvalidPluginExecutionException("Booking Services Transfer is missing in payload.");

            if (payloadBooking.BookingInfo.Services != null && payloadBooking.BookingInfo.Services.Transfer != null)
            {
                if (payloadBooking.DeleteAccommodationOrTransportOrRemarksOrTransferOrExtraService)
                {
                    trace.Trace("Delete Transfer information - start");
                    ProcessRecordsToDelete(EntityName.BookingTransfer,
                        new string[] { Attributes.BookingTransfer.BookingTransferId },
                        new string[] { Attributes.BookingTransfer.BookingId },
                        new string[] { payloadBooking.BookingId });
                    trace.Trace("Delete Transfer information - end");
                }

                trace.Trace("Booking Transfer information - start");
                //string bookingNumber = payloadBooking.BookingInfo.BookingIdentifier.BookingNumber;
                var bookinginfo = payloadBooking.BookingInfo;
                var transfer = payloadBooking.BookingInfo.Services.Transfer;
                var entityCollectionTransfer = BookingTransferHelper.GetBookingTransferEntityFromPayload(bookinginfo, Guid.Parse(payloadBooking.BookingId), trace);
                List<XrmResponse> xrmResponseList = CommonXrm.BulkCreate(entityCollectionTransfer, crmService);
                trace.Trace("Booking Transfer information - end");

                 ProcessTransferRemarks(xrmResponseList);
            }
        }

        public void ProcessTransferRemarks(List<XrmResponse> xrmResponseList)
        {
            if (payloadBooking.BookingInfo.Services != null && payloadBooking.BookingInfo.Services.Transfer != null && xrmResponseList.Count > 0)
            {
                EntityCollection entityColllectionTransferRemarks = new EntityCollection();

                trace.Trace("Transfer Remarks information - Start");
                for (int i = 0; i < payloadBooking.BookingInfo.Services.Transfer.Length; i++)
                {
                    if (payloadBooking.BookingInfo.Services.Transfer[i].Remark != null)
                    {
                        //BN - Type
                        var transferRemarks = RemarksHelper.GetRemarksEntityFromPayload(payloadBooking.BookingInfo.Services.Transfer[i].Remark, Guid.Parse(xrmResponseList[i].Id), trace, RemarkType.TransferRemark);
                        entityColllectionTransferRemarks.Entities.AddRange(transferRemarks.Entities);
                    }

                }

                if (entityColllectionTransferRemarks.Entities.Count > 0)
                    CommonXrm.BulkCreate(entityColllectionTransferRemarks, crmService);
                trace.Trace("Transfer Remarks information - End");
            }

        }

        public void ProcessExtraServices()
        {
            if (payloadBooking.BookingInfo.Services == null) throw new InvalidPluginExecutionException("Booking Services is missing in payload.");
            if (payloadBooking.BookingInfo.Services.ExtraService == null) throw new InvalidPluginExecutionException("Booking Services Transfer is missing in payload.");

            if (payloadBooking.BookingInfo.Services != null && payloadBooking.BookingInfo.Services.ExtraService != null)
            {
                if (payloadBooking.DeleteAccommodationOrTransportOrRemarksOrTransferOrExtraService)
                {
                    trace.Trace("Delete Extra Service information - start");
                    ProcessRecordsToDelete(EntityName.BookingExtraService,
                        new string[] { Attributes.BookingExtraService.BookingExtraServiceId },
                        new string[] { Attributes.BookingExtraService.BookingId },
                        new string[] { payloadBooking.BookingId });
                    trace.Trace("Delete Extra Service information - end");
                }

                trace.Trace("Booking ExtraService information - start");
                //string bookingNumber = payloadBooking.BookingInfo.BookingIdentifier.BookingNumber;
                var bookinginfo = payloadBooking.BookingInfo;
                var transfer = payloadBooking.BookingInfo.Services.Transfer;
                var entityCollectionExtraService = BookingExtraServiceHelper.GetBookingExtraServicerEntityFromPayload(bookinginfo, Guid.Parse(payloadBooking.BookingId), trace);
                List<XrmResponse> xrmResponseList = CommonXrm.BulkCreate(entityCollectionExtraService, crmService);
                trace.Trace("Booking Transfer information - end");

                 ProcessExtraServiceRemarks(xrmResponseList);
            }
        }

        public void ProcessExtraServiceRemarks(List<XrmResponse> xrmResponseList)
        {
            if (payloadBooking.BookingInfo.Services != null && payloadBooking.BookingInfo.Services.ExtraService != null && xrmResponseList.Count > 0)
            {
                EntityCollection entityColllectionExtraServiceRemarks = new EntityCollection();

                trace.Trace("ExtraService Remarks information - Start");
                for (int i = 0; i < payloadBooking.BookingInfo.Services.ExtraService.Length; i++)
                {
                    if (payloadBooking.BookingInfo.Services.ExtraService[i].Remark != null)
                    {
                        //BN - Type
                        var transferRemarks = RemarksHelper.GetRemarksEntityFromPayload(payloadBooking.BookingInfo.Services.ExtraService[i].Remark, Guid.Parse(xrmResponseList[i].Id), trace, RemarkType.ExtraServiceRemark);
                        entityColllectionExtraServiceRemarks.Entities.AddRange(transferRemarks.Entities);
                    }

                }

                if (entityColllectionExtraServiceRemarks.Entities.Count > 0)
                    CommonXrm.BulkCreate(entityColllectionExtraServiceRemarks, crmService);
                trace.Trace("ExtraService Remarks information - End");
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
                if (payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId != null && payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId != "")
                {
                    entityBookingRole[Attributes.CustomerBookingRole.Customer] = new EntityReference(EntityName.Account, Attributes.Contact.SourceSystemID, payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId);
                }
                else
                {
                    entityBookingRole[Attributes.CustomerBookingRole.Customer] = new EntityReference(EntityName.Account, new Guid(payloadBooking.CustomerId));
                }
            }
            else
            {
                if (payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId != null && payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId != "")
                {
                    entityBookingRole[Attributes.CustomerBookingRole.Customer] = new EntityReference(EntityName.Contact, Attributes.Contact.SourceSystemID, payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId);
                }
                else
                {
                    entityBookingRole[Attributes.CustomerBookingRole.Customer] = new EntityReference(EntityName.Contact, new Guid(payloadBooking.CustomerId));
                }
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
