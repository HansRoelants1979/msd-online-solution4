using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.CustomWorkflowSteps.Constants;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services
{
    public class BookingRulesService
    {
        private PayloadBooking payloadBooking;
        private ITracingService trace;
        private IOrganizationService crmService;

        Guid bookingId;

        public bool updateBooking = false;
        public bool returnErrorCode422 = false;
        public bool returnErrorCode400 = false;
        public bool returnSuccessCode20x = false;
        public bool createBooking = false;
        public bool createCustomer = false;
        public bool delinkCustomer = false;
        public bool linkCustomer = false;

        public BookingRulesService(PayloadBooking payloadBooking)
        {
            if (payloadBooking == null) throw new InvalidPluginExecutionException("Cannot create instance of Service - payload instance is null.");
            this.payloadBooking = payloadBooking;
            trace = payloadBooking.Trace;
            crmService = payloadBooking.CrmService;
        }
        public void SetRequiredFlags()
        {
            if (payloadBooking.BookingInfo != null && payloadBooking.BookingInfo.BookingIdentifier != null && !string.IsNullOrEmpty(payloadBooking.BookingInfo.BookingIdentifier.ConsultationReference))
                ResolveConsultationReferenceId(payloadBooking.BookingInfo.BookingIdentifier.ConsultationReference);

            if (IsOnTourorTCVAtCoreBooking(payloadBooking.BookingInfo.BookingIdentifier))
            {
                if (IsMatchedBooking(payloadBooking.BookingInfo.BookingIdentifier.BookingNumber,
                    payloadBooking.BookingInfo.BookingIdentifier.BookingSystem.ToString()))
                {
                    if (IsOnTourBooking(payloadBooking.BookingInfo.BookingIdentifier))
                    {
                        if (bookingId != null && IsBookingLinkedtoCustomer(bookingId))
                        {
                            if (IsCutomerDataProvided(payloadBooking.BookingInfo.Customer))
                            {
                                if (bookingId != null && IsSameCustomer(bookingId))
                                {
                                    delinkCustomer = true;
                                    updateBooking = true;
                                    return;
                                }
                                else
                                {
                                    if (IsMatchedCustomer(payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId))
                                    {
                                        delinkCustomer = true;
                                        linkCustomer = true;
                                        updateBooking = true;
                                        return;
                                    }
                                    else
                                    {
                                        createCustomer = true;
                                        delinkCustomer = true;
                                        linkCustomer = true;
                                        updateBooking = true;
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                delinkCustomer = true;
                                updateBooking = true;
                                return;
                            }
                        }
                        else
                        {
                            if (IsCutomerDataProvided(payloadBooking.BookingInfo.Customer))
                            {
                                if (IsMatchedCustomer(payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId))
                                {
                                    linkCustomer = true;
                                    updateBooking = true;
                                    return;
                                }
                                else
                                {
                                    createCustomer = true;
                                    linkCustomer = true;
                                    updateBooking = true;
                                    return;
                                }
                            }
                            else
                            {
                                updateBooking = true;
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (bookingId != null && IsBookingOnTour(bookingId))
                        {
                            returnSuccessCode20x = true;
                            return;
                        }
                        else
                        {
                            if (bookingId != null && IsSameCustomer(bookingId))
                            {
                                updateBooking = true;
                                return;
                            }
                            else
                            {
                                if (IsMatchedCustomer(payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId))
                                {
                                    updateBooking = true;
                                    return;
                                }
                                else
                                {
                                    returnErrorCode422 = true;
                                    return;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (IsOnTourBooking(payloadBooking.BookingInfo.BookingIdentifier))
                    {
                        if (IsCutomerDataProvided(payloadBooking.BookingInfo.Customer))
                        {
                            if (IsMatchedCustomer(payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId))
                            {
                                createBooking = true;
                                return;
                            }
                            else
                            {
                                createCustomer = true;
                                createBooking = true;
                                return;
                            }
                        }
                        else
                        {
                            createBooking = true;
                            return;
                        }
                    }
                    else
                    {
                        if (IsBookingConsultationEmpty(payloadBooking.BookingInfo.BookingIdentifier))
                        {
                            returnErrorCode400 = true;
                            return;
                        }
                        else
                        {
                            if (IsMatchedBooking(payloadBooking.BookingInfo.BookingIdentifier.DealSequenceNumber,
                                        payloadBooking.BookingInfo.BookingIdentifier.ConsultationReference))
                            {
                                if (bookingId != null && IsSameCustomer(bookingId))
                                {
                                    updateBooking = true;
                                    return;
                                }
                                else
                                {
                                    if (IsMatchedCustomer(payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId))
                                    {
                                        updateBooking = true;
                                        return;
                                    }
                                    else
                                    {
                                        returnErrorCode422 = true;
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (IsBookingConsultationEmpty(payloadBooking.BookingInfo.BookingIdentifier))
                {
                    returnErrorCode400 = true;
                    return;
                }
                else
                {
                    if (IsMatchedBooking(payloadBooking.BookingInfo.BookingIdentifier.DealSequenceNumber,
                        payloadBooking.BookingInfo.BookingIdentifier.ConsultationReference))
                    {
                        if (bookingId != null && IsSameCustomer(bookingId))
                        {
                            updateBooking = true;
                            return;
                        }
                        else
                        {
                            if (IsMatchedCustomer(payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId))
                            {
                                updateBooking = true;
                                return;
                            }
                            else
                            {
                                returnErrorCode422 = true;
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (IsMatchedCustomer(payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId))
                        {
                            createBooking = true;
                            return;
                        }
                        else
                        {
                            returnErrorCode422 = true;
                            return;
                        }
                    }
                }
            }
        }

        public void ResolveConsultationReferenceId(string consultationReference)
        {
            var bookingFetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' no-lock='true'>			
                                    <entity name='opportunity'>			
                                    <attribute name='opportunityid' />			
                                    <filter type='and'>			
                                    <condition attribute='name' operator='eq' value='{consultationReference}' />			
                                    </filter>			
                                    </entity>			
                                    </fetch>";

            var bookingCollection = CommonXrm.RetrieveMultipleRecordsFetchXml(bookingFetch, crmService);
            if (bookingCollection != null && bookingCollection.Entities.Count > 0)
            {
                var consultationRef = bookingCollection.Entities[0].Attributes["opportunityid"];
                payloadBooking.BookingInfo.BookingIdentifier.ConsultationReference = consultationRef.ToString();
            }
        }

        public bool IsOnTourorTCVAtCoreBooking(BookingIdentifier bookingIdentifier)
        {
            if (bookingIdentifier.SourceSystem == SourceSystem.OnTour ||
               (bookingIdentifier.SourceSystem == SourceSystem.TCV && bookingIdentifier.BookingSystem == BookingSystem.AtCore))
            {
                return true;
            }
            return false;
        }
        public bool IsBookingConsultationEmpty(BookingIdentifier bookingIdentifier)
        {
            if (string.IsNullOrEmpty(bookingIdentifier.ConsultationReference)) return true;
            return false;
        }
        public bool IsOnTourBooking(BookingIdentifier bookingIdentifier)
        {
            if (bookingIdentifier.SourceSystem == SourceSystem.OnTour) return true;
            return false;
        }
        public bool IsCutomerDataProvided(Customer customer)
        {
            if (customer == null) return false;
            return true;
        }
        public bool IsSameCustomer(Guid bookingId)
        {
            var sourceSystemId = GetCustomerSourceSystemId(bookingId);
            if (!string.IsNullOrEmpty(sourceSystemId) &&
                string.Equals(sourceSystemId, payloadBooking.BookingInfo.Customer.CustomerIdentifier.CustomerId))
                return true;
            return false;
        }
        public bool IsBookingLinkedtoCustomer(Guid bookingId)
        {
            var sourceSystemId = GetCustomerSourceSystemId(bookingId);
            if (!string.IsNullOrEmpty(sourceSystemId)) return true;
            return false;
        }
        public string GetCustomerSourceSystemId(Guid bookingId)
        {
            string sourceSystemId = string.Empty;

            var customerBookingRoleFetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' no-lock='true'>
                          <entity name='tc_customerbookingrole'>
                            <attribute name='tc_customerbookingroleid' />
                            <attribute name='tc_customer' />
                            <order attribute='tc_customer' descending='false' />
                            <filter type='and'>
                              <condition attribute='tc_bookingid' operator='eq' uitype='tc_booking' value='{bookingId}' />
                              <condition attribute='statecode' operator='eq' value='0' />
                            </filter>
                            <link-entity name='contact' from='contactid' to='tc_customer' visible='false' link-type='outer' alias='con'>
                              <attribute name='tc_sourcesystemid' />
                            </link-entity>
                            <link-entity name='account' from='accountid' to='tc_customer' visible='false' link-type='outer' alias='acc'>
                              <attribute name='tc_sourcesystemid' />
                            </link-entity>
                          </entity>
                        </fetch>";


            var customerBookingRoleCollection = CommonXrm.RetrieveMultipleRecordsFetchXml(customerBookingRoleFetch, crmService);

            if (customerBookingRoleCollection != null && customerBookingRoleCollection.Entities.Count > 0)
            {
                var customerBookingRole = customerBookingRoleCollection.Entities[0];
                if (customerBookingRole.Attributes.Contains("con.tc_sourcesystemid") && customerBookingRole.Attributes["con.tc_sourcesystemid"] != null)
                {
                    sourceSystemId = ((AliasedValue)customerBookingRole.Attributes["con.tc_sourcesystemid"]).Value.ToString();
                }
                else if (customerBookingRole.Attributes.Contains("acc.tc_sourcesystemid") && customerBookingRole.Attributes["acc.tc_sourcesystemid"] != null)
                {
                    sourceSystemId = ((AliasedValue)customerBookingRole.Attributes["acc.tc_sourcesystemid"]).Value.ToString();
                }
            }
            return sourceSystemId;
        }
        public bool IsMatchedBooking(string bookingNumber, string bookingSystem)
        {

            var bookingFetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' no-lock='true'>
                  <entity name='tc_booking'>
                    <attribute name='tc_bookingid' />
                    <attribute name='tc_name' />
                    <order attribute='tc_name' descending='false' />
                    <filter type='and'>
                      <condition attribute='tc_name' operator='eq' value='{bookingNumber}' />
                      <condition attribute='tc_sourcesystem' operator='eq' value='{bookingSystem}' />
                      <condition attribute='statecode' operator='eq' value='0' />
                    </filter>
                  </entity>
                </fetch>";

            var bookingCollection = CommonXrm.RetrieveMultipleRecordsFetchXml(bookingFetch, crmService);
            if (bookingCollection != null && bookingCollection.Entities.Count == 1)
            {
                bookingId = bookingCollection.Entities[0].Id;
                return true;
            }
            else if (bookingCollection != null && bookingCollection.Entities.Count > 1)
                throw new InvalidPluginExecutionException("More than one matching booking record found in MSD with BookingNumber and SourceSystem");
            return false;
        }
        public bool IsMatchedBooking(int dealSequenceNumber, string consultationReference)
        {
            payloadBooking.Response = new BookingResponse();

            var bookingFetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' no-lock='true'>
                      <entity name='tc_booking'>
                        <attribute name='tc_bookingid' />
                        <attribute name='tc_name' />
                        <order attribute='tc_name' descending='false' />
                        <filter type='and'>
                          <condition attribute='tc_dealsequencenumber' operator='eq' value='{dealSequenceNumber}' />
                          <condition attribute='statecode' operator='eq' value='0' />
                        </filter>
                        <link-entity name='opportunity' from='opportunityid' to='tc_consultationreferenceid' alias='ad'>
                          <filter type='and'>
                            <condition attribute='opportunityid' operator='eq' value='{consultationReference}' />
                          </filter>
                        </link-entity>
                      </entity>
                    </fetch>";

            var bookingCollection = CommonXrm.RetrieveMultipleRecordsFetchXml(bookingFetch, crmService);
            if (bookingCollection != null && bookingCollection.Entities.Count == 1)
            {
                bookingId = bookingCollection.Entities[0].Id;
                return true;
            }
            else if (bookingCollection != null && bookingCollection.Entities.Count > 1)
                payloadBooking.Response.ResponseCode = ResponseDetails.ReturnErrorCode400;
                payloadBooking.Response.ResponseMessage = "More than one matching booking record found in MSD with DealSequenceNumber and ConsultationReference";
            return false;
        }
        public bool IsMatchedCustomer(string customerId)
        {
            //Validate payload for customer
            if (payloadBooking.BookingInfo == null)
                throw new InvalidPluginExecutionException("Booking info missing in payload.");
            if (payloadBooking.BookingInfo.Customer == null)
                throw new InvalidPluginExecutionException("Customer is missing in payload."); ;
            if (payloadBooking.BookingInfo.Customer.CustomerIdentifier == null)
                throw new InvalidPluginExecutionException("Customer Identifier is missing.");


            if (payloadBooking.BookingInfo.Customer.CustomerGeneral.CustomerType == CustomerType.Company)
            {
                var accountFetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' no-lock='true'>
                      <entity name='account'>
                        <attribute name='accountid' />
                        <filter type='and'>
                          <condition attribute='tc_sourcesystemid' operator='eq' value='{customerId}' />
                          <condition attribute='statecode' operator='eq' value='0' />
                        </filter>
                      </entity>
                    </fetch>";

                var accountCollection = CommonXrm.RetrieveMultipleRecordsFetchXml(accountFetch, crmService);
                if (accountCollection != null && accountCollection.Entities.Count == 1)
                {
                    return true;
                }
                else if (accountCollection != null && accountCollection.Entities.Count > 1)
                    throw new InvalidPluginExecutionException("More than one matching customer found in MSD with SourceSystemId");

                return false;
            }
            else if (payloadBooking.BookingInfo.Customer.CustomerGeneral.CustomerType == CustomerType.Person)
            {
                var contactFetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' no-lock='true'>
                      <entity name='contact'>
                        <attribute name='contactid' />
                        <filter type='and'>
                          <condition attribute='tc_sourcesystemid' operator='eq' value='{customerId}' />
                          <condition attribute='statecode' operator='eq' value='0' />
                        </filter>
                      </entity>
                    </fetch>";

                var contactCollection = CommonXrm.RetrieveMultipleRecordsFetchXml(contactFetch, crmService);
                if (contactCollection != null && contactCollection.Entities.Count == 1)
                {
                    return true;
                }
                else if (contactCollection != null && contactCollection.Entities.Count > 1)
                    throw new InvalidPluginExecutionException("More than one matching customer found in MSD with SourceSystemId");
                return false;
            }
            else
                throw new InvalidPluginExecutionException("Customer type has not been specified.");
        }
        public bool IsBookingOnTour(Guid bookingId)
        {
            var bookingFetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' no-lock='true'>
                  <entity name='tc_booking'>
                    <attribute name='tc_bookingid' />
                    <filter type='and'>
                      <condition attribute='tc_bookingid' operator='eq'  uitype='tc_booking' value='{bookingId}' />
                      <condition attribute='tc_sourcesystem' operator='eq' value='{SourceSystem.OnTour}' />
                      <condition attribute='statecode' operator='eq' value='0' />
                    </filter>
                  </entity>
                </fetch>";


            var bookingCollection = CommonXrm.RetrieveMultipleRecordsFetchXml(bookingFetch, crmService);
            if (bookingCollection != null && bookingCollection.Entities.Count == 1)
            {
                return true;
            }
            else if (bookingCollection != null && bookingCollection.Entities.Count > 1)
                payloadBooking.Response.ResponseCode = ResponseDetails.ReturnErrorCode400;
                payloadBooking.Response.ResponseMessage = "More than one matching booking record found in MSD with BookingId and SourceSystem OnTour";
            return false;
        }
    }
}
