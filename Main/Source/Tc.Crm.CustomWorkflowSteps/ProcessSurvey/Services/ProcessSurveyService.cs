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
            if (payloadSurvey.SurveyResponse == null) throw new InvalidPluginExecutionException("SurveyResponse is null in json payload");
            if (payloadSurvey.SurveyResponse.Responses == null) throw new InvalidPluginExecutionException("Response object in payload json is null");
            List<Response> responses = payloadSurvey.SurveyResponse.Responses;
            var surveyResponseCollection = new EntityCollection();
            for (int i = 0; i < responses.Count; i++)
            {
                trace.Trace("Processing Response " + i + " - start");
                var surveyResponse = SurveyResponseHelper.GetResponseEntityFromPayload(responses[i], trace);
                if (responses[i].Answers != null && responses[i].Answers.Count > 0)
                {
                    MapBookingContact(surveyResponse, responses[i].Answers);
                    ProcessFeedback(surveyResponse, responses[i].Answers);
                }
                surveyResponseCollection.Entities.Add(surveyResponse);
                trace.Trace("Processing Response " + i + " - end");
            }
            if (surveyResponseCollection.Entities.Count > 0)
                CommonXrm.BulkCreate(surveyResponseCollection, payloadSurvey.CrmService);
            trace.Trace("Processing ProcessResponses - end");
        }



        /// <summary>
        /// To map booking and contact records
        /// </summary>
        /// <param name="surveyResponse"></param>
        /// <param name="answers"></param>
        private void MapBookingContact(Entity surveyResponse, List<Answer> answers)
        {
            trace.Trace("Processing MapBookingContact - start");
            var bookingNumber = AnswerHelper.FindBooking(answers,trace);
            var contactLastName = AnswerHelper.FindCustomer(answers,trace);
            if (!string.IsNullOrWhiteSpace(bookingNumber))
            {
                FetchBookingContact(bookingNumber, contactLastName, surveyResponse);
            }
            trace.Trace("Processing MapBookingContact - end");
        }


        /// <summary>
        /// To fetch booking, contact
        /// </summary>
        /// <param name="bookingNumber"></param>
        /// <param name="contactLastName"></param>
        /// <param name="surveyResponse"></param>
        private void FetchBookingContact(string bookingNumber, string contactLastName, Entity surveyResponse)
        {
            trace.Trace("Processing FetchBookingContact - start");
            var contactCondition = PrepareContactCondition(contactLastName);
            var query = string.Format(@"<fetch output-format='xml-platform' distinct='false' version='1.0' mapping='logical'>
                                        <entity name='tc_customerbookingrole'>
                                          <link-entity name='tc_booking' alias='booking' from='tc_bookingid' to='tc_bookingid'>
                                            <attribute name='tc_bookingid' />                                               
                                              <filter type = 'and'>
                                                <condition attribute='tc_name' operator='eq' value='{0}' />
                                              </filter>
                                          </link-entity>
                                        {1}
                                        </entity>
                                        </fetch>", bookingNumber, contactCondition);

            var entColBookingContact = CommonXrm.RetrieveMultipleRecordsFetchXml(query, payloadSurvey.CrmService);
            MapBooking(surveyResponse, entColBookingContact);
            MapContact(surveyResponse, entColBookingContact);
            trace.Trace("Processing FetchBookingContact - end");
        }

        /// <summary>
        /// To prepare link entity for contact when contact name is not empty
        /// </summary>
        /// <param name="contactLastName"></param>
        /// <returns></returns>
        private string PrepareContactCondition(string contactLastName)
        {
            var contactCondition = string.Empty;
            if (!string.IsNullOrWhiteSpace(contactLastName))
            {
                contactCondition = string.Format(@"<link-entity name='contact' alias='contact' from='contactid' to='tc_customer' link-type='outer'>
                                                    <attribute name='contactid' />
                                                        <filter type='and' >
                                                            <condition attribute='lastname' operator='eq' value='{0}' />
                                                        </filter>
                                                   </link-entity>", contactLastName);
            }
            return contactCondition;
        } 


        /// <summary>
        /// To map booking to survey response
        /// </summary>
        /// <param name="surveyResponse"></param>
        /// <param name="entColBookingContact"></param>
        private void MapBooking(Entity surveyResponse, EntityCollection entColBookingContact)
        {
            trace.Trace("Processing MapBooking - start");
            if (entColBookingContact != null && entColBookingContact.Entities.Count > 0)
            {
                var fieldBooking = AliasName.Booking + Attributes.Booking.BookingId;
                var entity = entColBookingContact.Entities[0];
                if (entity != null && entity.Attributes.Contains(fieldBooking) && entity.Attributes[fieldBooking] != null)
                {
                    var booking = (AliasedValue)entity.Attributes[fieldBooking];
                    surveyResponse[Attributes.SurveyResponse.Regarding] = new EntityReference(booking.EntityLogicalName, Guid.Parse(booking.Value.ToString()));
                }
            }
            trace.Trace("Processing MapBooking - end");
        }

        /// <summary>
        /// To map contact to survey response (when only one contact found)
        /// </summary>
        /// <param name="surveyResponse"></param>
        /// <param name="entColBookingContact"></param>
        private void MapContact(Entity surveyResponse, EntityCollection entColBookingContact)
        {
            trace.Trace("Processing MapContact - start");
            if (entColBookingContact != null && entColBookingContact.Entities.Count == 1)
            {
                var fieldContact = AliasName.Contact + Attributes.Contact.ContactId;
                var entity = entColBookingContact.Entities[0];
                if (entity != null && entity.Attributes.Contains(fieldContact) && entity.Attributes[fieldContact] != null)
                {
                    var contact = (AliasedValue)entity.Attributes[fieldContact];
                    var customer = GetPartyList(contact);
                    if (customer != null && customer.Entities.Count > 0)
                        surveyResponse[Attributes.SurveyResponse.CustomerId] = customer;
                }
            }
            trace.Trace("Processing MapContact - end");
        }


        /// <summary>
        /// To prepare partylist for contact
        /// </summary>
        /// <param name="aliasValue"></param>
        /// <returns></returns>
        private EntityCollection GetPartyList(AliasedValue aliasValue)
        {
            trace.Trace("Processing GetPartyList - start");
            var customers = new EntityCollection();
            if (aliasValue != null)
            {   
                var party = new Entity(EntityName.ActivityParty);
                party[Attributes.ActivityParty.PartyId] = new EntityReference() { LogicalName = aliasValue.EntityLogicalName, Id = Guid.Parse(aliasValue.Value.ToString()) };
                customers.Entities.Add(party);
            }
            trace.Trace("Processing GetPartyList - end");
            return customers;
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
                surveyResponse.RelatedEntities.Add(new Relationship(Relationships.SurveyResponseFeedback), feedbackCollection);
            trace.Trace("Processing ProcessFeedback - end");
        }

        /// <summary>
        /// To process answer records
        /// </summary>
        /// <param name="answers"></param>
        /// <returns></returns>
        private EntityCollection ProcessAnswers(List<Answer> answers)
        {
            trace.Trace("Processing ProcessAnswers - start");
            var feedbackCollection = new EntityCollection();
            if (answers != null)
            {
                for (int i = 0; i < answers.Count; i++)
                {
                    trace.Trace("Processing Answer " + i + " - start");
                    var feedback = AnswerHelper.GetFeedbackEntityFromPayload(answers[i],trace);                    
                    feedbackCollection.Entities.Add(feedback);
                    trace.Trace("Processing Answer " + i + " - end");
                }                
            }
            trace.Trace("Processing ProcessAnswers - end");
            return feedbackCollection;
        }

    }
}
