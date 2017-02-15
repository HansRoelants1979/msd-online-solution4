using Microsoft.Xrm.Sdk;
using System;
using System.Text;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services
{
    public static class RemarksHelper
    {
        /// <summary>
        /// To process Remarks information
        /// </summary>
        /// <param name="bookingNumber"></param>
        /// <param name="remark"></param>
        /// <param name="parentRecordId"></param>
        /// <param name="trace"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static EntityCollection GetRemarksEntityFromPayload(string bookingNumber, Remark[] remark, Guid parentRecordId, ITracingService trace, RemarkType type)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Populate Remarks information - start");
                      
            if (string.IsNullOrWhiteSpace(bookingNumber))
                throw new InvalidPluginExecutionException("Booking Number should not be null.");
            
            EntityCollection entityCollectionRemarks = new EntityCollection();
            if (remark != null && remark.Length > 0)
            {
                trace.Trace("Processing "+ remark.Length .ToString()+ " Remarks - start");
                for (int i = 0; i < remark.Length; i++)
                {
                    trace.Trace("Processing Remark " + i.ToString() + " - start");
                    var remarkEntity = PrepareBookingRemarks(bookingNumber, remark[i], trace);
                    switch (type)
                    {
                        case RemarkType.Remark:
                            remarkEntity[Attributes.Booking.BookingId] = new EntityReference(EntityName.Booking, parentRecordId);
                            break;
                        case RemarkType.AccomodationRemark:
                            remarkEntity[Attributes.Remark.BookingAccommodationId] = new EntityReference(EntityName.BookingAccommodation, parentRecordId);
                            break;
                        case RemarkType.TransportRemark:
                            remarkEntity[Attributes.Remark.BookingTransportId] = new EntityReference(EntityName.BookingTransport, parentRecordId);
                            break;
                    }
                    entityCollectionRemarks.Entities.Add(remarkEntity);
                    trace.Trace("Processing Remark " + i.ToString() + " - end");
                }
                trace.Trace("Processing " + remark.Length.ToString() + " Remarks - end");
            }
            trace.Trace("Populate Remarks information - end");
            return entityCollectionRemarks;

        }

        /// <summary>
        /// To prepare Remark entity
        /// </summary>
        /// <param name="bookingNumber"></param>
        /// <param name="remark"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        private static Entity PrepareBookingRemarks(string bookingNumber, Remark remark,  ITracingService trace)
        {
            trace.Trace("Preparing Remark information - Start");
            var remarkEntity = new Entity(EntityName.Remark);
            remarkEntity[Attributes.Remark.Name] = bookingNumber + General.Concatenator + remark.RemarkType.ToString();
            remarkEntity[Attributes.Remark.Type] = CommonXrm.GetOptionSetValue(remark.RemarkType.ToString(), Attributes.Remark.Type);
            if(remark.Text != null)
            remarkEntity[Attributes.Remark.RemarkName] = remark.Text;            
            trace.Trace("Preparing Remark information - End");
            return remarkEntity;
        }
    }
}
