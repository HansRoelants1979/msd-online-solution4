using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Tc.Crm.CustomWorkflowSteps.ProcessSurvey.Models;


namespace Tc.Crm.CustomWorkflowSteps.ProcessSurvey.Services
{
    public class ProcessSurveyService
    {
        private PayloadSurvey payloadSurvey;
        private ITracingService trace;

        public ProcessSurveyService(PayloadSurvey payloadSurvey)
        {
            this.payloadSurvey = payloadSurvey;
            this.trace = payloadSurvey.Trace;
        }

        /// <summary>
        /// To process survey information
        /// </summary>
        /// <param name="json"></param>       
        /// <returns></returns>
        public string ProcessSurveyResponse()
        {
            trace.Trace("Processing Process payload - start");
            if (payloadSurvey == null) throw new InvalidPluginExecutionException("payloadSurvey is null;");
            ProcessResponses();
            trace.Trace("Processing Process payload - end");
            return JsonHelper.SerializeSurveyJson(new SurveyReturnResponse() { Created = true }, trace);
        }

        /// <summary>
        /// To process all survey responses
        /// </summary>
        private void ProcessResponses()
        {
            trace.Trace("Processing ProcessResponses - start");
            if (payloadSurvey.SurveyResponse.Responses == null) throw new InvalidPluginExecutionException("Response object created from payload json is null;");
            List<Response> responses = payloadSurvey.SurveyResponse.Responses;
            for (int i = 0; i < responses.Count; i++)
            {
                var surveyResponse = SurveyResponseHelper.GetResponseEntityFromPayLoad(responses[i]);
                MapBookingContact(surveyResponse, responses[i].Answers);
                ProcessFeedback(surveyResponse, responses[i].Answers);
                var surveyResponseCollection = new EntityCollection();
                surveyResponseCollection.Entities.Add(surveyResponse);
                CommonXrm.BulkCreate(surveyResponseCollection, payloadSurvey.CrmService);
            }
            trace.Trace("Processing ProcessResponses - end");
        }

        /// <summary>
        /// To process feedback information
        /// </summary>
        /// <param name="surveyResponse"></param>
        /// <param name="answers"></param>
        private void ProcessFeedback(Entity surveyResponse, List<Answer> answers)
        {
            trace.Trace("Processing ProcessFeedback - start");
            var feedbackCollection = ProcessAnswers(answers);
            if (feedbackCollection != null && feedbackCollection.Entities.Count > 0)
                surveyResponse.RelatedEntities.Add(new Relationship(RelationShips.SurveyResponseFeedback), feedbackCollection);
            trace.Trace("Processing ProcessFeedback - end");
        }

        /// <summary>
        /// To map booking and contact records to survey response
        /// </summary>
        /// <param name="surveyResponse"></param>
        /// <param name="answers"></param>
        private void MapBookingContact(Entity surveyResponse, List<Answer> answers)
        {
            trace.Trace("Processing MapBookingContact - start");
            var bookingId = MapBooking(surveyResponse, answers);
            if (bookingId != Guid.Empty)
                MapContact(surveyResponse, answers, bookingId);
            trace.Trace("Processing MapBookingContact - end");
        }


        /// <summary>
        /// To map booking to survey response
        /// </summary>
        /// <param name="surveyResponse"></param>
        /// <param name="answers"></param>
        /// <returns></returns>
        private Guid MapBooking(Entity surveyResponse, List<Answer> answers)
        {
            trace.Trace("Processing MapBooking - start");
            var bookingId = Guid.Empty;
            var bookingNumber = AnswerHelper.FindBooking(answers);
            if (!string.IsNullOrWhiteSpace(bookingNumber))
            {
                var booking = FetchBooking(bookingNumber);
                if (booking != null)
                {
                    surveyResponse[Attributes.SurveyResponse.BookingId] = booking;
                    bookingId = booking.Id;
                }
            }
            trace.Trace("Processing MapBooking - end");
            return bookingId;
        }


        /// <summary>
        /// To map contact to survey response
        /// </summary>
        /// <param name="surveyResponse"></param>
        /// <param name="answers"></param>
        /// <param name="bookingId"></param>
        private void MapContact(Entity surveyResponse, List<Answer> answers, Guid bookingId)
        {
            trace.Trace("Processing MapContact - start");
            var customerName = AnswerHelper.FindCustomer(answers);
            if (!string.IsNullOrWhiteSpace(customerName))
            {
                var contact = FetchContact(bookingId, customerName);
                if (contact != null)
                    surveyResponse[Attributes.SurveyResponse.CustomerId] = contact;
            }
            trace.Trace("Processing MapContact - end");
        }

        /// <summary>
        /// To fetch booking based on booking number
        /// </summary>
        /// <param name="bookingNumber"></param>
        /// <returns></returns>
        private EntityReference FetchBooking(string bookingNumber)
        {
            trace.Trace("Processing FetchBooking Booking Number: '"+ bookingNumber +"' - start");
            var booking = CommonXrm.RetrieveMultipleRecords(EntityName.Booking,
                                                                    new string[] { Attributes.Booking.BookingId },
                                                                    new string[] { Attributes.Booking.Name },
                                                                    new string[] { bookingNumber },
                                                                    payloadSurvey.CrmService);
            if (booking != null && booking.Entities.Count == 1)
            {
                trace.Trace("Processing FetchBooking (1 booking found) - end");
                return new EntityReference(EntityName.Booking, booking.Entities[0].Id);
            }
            trace.Trace("Processing FetchBooking (No booking found) - end");
            return null;
        }

        /// <summary>
        /// To fetch contact based on bookingid and contact name
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="contactName"></param>
        /// <returns></returns>
        private EntityCollection FetchContact(Guid bookingId, string contactName)
        {
            trace.Trace("Processing FetchContact - start");
            var customers = CommonXrm.RetrieveMultipleRecords(EntityName.CustomerBookingRole,
                                                                               new string[] { Attributes.CustomerBookingRole.Customer },
                                                                               new string[] { Attributes.CustomerBookingRole.BookingId },
                                                                               new string[] { bookingId.ToString() },
                                                                               payloadSurvey.CrmService);
            if (customers != null && customers.Entities.Count == 1)
            {
                if (customers.Entities[0].Attributes.Contains(Attributes.CustomerBookingRole.Customer)
                   &&
                   customers.Entities[0].Attributes[Attributes.CustomerBookingRole.Customer] != null)
                {
                    var customer = (EntityReference)customers.Entities[0].Attributes[Attributes.CustomerBookingRole.Customer];
                    if (customer.LogicalName == EntityName.Contact && customer.Name == contactName)
                    {
                        trace.Trace("Processing FetchContact (1 contact found) - end");
                        return GetPartyList(new EntityReferenceCollection() { customer });
                    }
                }
            }
            trace.Trace("Processing FetchContact (No contact found) - end");
            return null;
        }

        /// <summary>
        /// To prepare partylist
        /// </summary>
        /// <param name="entityReferenceCollection"></param>
        /// <returns></returns>
        private EntityCollection GetPartyList(EntityReferenceCollection entityReferenceCollection)
        {
            trace.Trace("Processing GetPartyList - start");
            var entityCollection = new EntityCollection();
            for (int i = 0; i < entityReferenceCollection.Count; i++)
            {
                var party = new Entity(EntityName.ActivityParty);
                party[Attributes.ActivityParty.PartyId] = entityReferenceCollection[i];
                entityCollection.Entities.Add(party);
            }
            trace.Trace("Processing GetPartyList - end");
            return entityCollection;
        }

        /// <summary>
        /// To process answer records
        /// </summary>
        /// <param name="answers"></param>
        /// <returns></returns>
        private EntityCollection ProcessAnswers(List<Answer> answers)
        {
            trace.Trace("Processing ProcessAnswers - start");
            if (answers == null) throw new InvalidPluginExecutionException("Answers in Survey reponse is null");
            var feedbackCollection = new EntityCollection();
            for (int i = 0; i < answers.Count; i++)
            {
                var feedback = AnswerHelper.GetFeedbackEntityFromPayLoad(answers[i]);
                feedback.Id = Guid.NewGuid();
                feedbackCollection.Entities.Add(feedback);
            }
            trace.Trace("Processing ProcessAnswers - end");
            return feedbackCollection;
        }
    }
}
