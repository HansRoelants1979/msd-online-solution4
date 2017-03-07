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
        public static EntityCollection GetRemarksEntityFromPayload(Remark[] remark, Guid parentRecordId, ITracingService trace, RemarkType type)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Populate Remarks information - start");



            EntityCollection entityCollectionRemarks = new EntityCollection();
            if (remark != null && remark.Length > 0)
            {
                trace.Trace("Processing " + remark.Length.ToString() + " Remarks - start");
                for (int i = 0; i < remark.Length; i++)
                {
                    trace.Trace("Processing Remark " + i.ToString() + " - start");
                    var annotationEntity = PrepareBookingRemarks(remark[i], trace);
                    switch (type)
                    {
                        case RemarkType.Remark:
                            annotationEntity[Attributes.Annotation.Regarding] = new EntityReference(EntityName.Booking, parentRecordId);
                            break;
                        case RemarkType.AccomodationRemark:
                            annotationEntity[Attributes.Annotation.Regarding] = new EntityReference(EntityName.BookingAccommodation, parentRecordId);
                            break;
                        case RemarkType.TransportRemark:
                            annotationEntity[Attributes.Annotation.Regarding] = new EntityReference(EntityName.BookingTransport, parentRecordId);
                            break;
                        case RemarkType.TransferRemark:
                            annotationEntity[Attributes.Annotation.Regarding] = new EntityReference(EntityName.BookingTransfer, parentRecordId);
                            break;
                        case RemarkType.ExtraServiceRemark:
                            annotationEntity[Attributes.Annotation.Regarding] = new EntityReference(EntityName.BookingExtraService, parentRecordId);
                            break;
                    }
                    entityCollectionRemarks.Entities.Add(annotationEntity);
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
        private static Entity PrepareBookingRemarks(Remark remark, ITracingService trace)
        {
            trace.Trace("Preparing Remark information - Start");
            var annotationEntity = new Entity(EntityName.Annotation);
            annotationEntity[Attributes.Annotation.Subject] = remark.RemarkType.ToString();
            if (!string.IsNullOrWhiteSpace(remark.Text))
                annotationEntity[Attributes.Annotation.NoteText] = remark.Text;
            trace.Trace("Preparing Remark information - End");
            return annotationEntity;
        }
    }
}
