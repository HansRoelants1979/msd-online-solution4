if (typeof (Tc) === "undefined") {
    Tc = {
        __namespace: true
    };
}
if (typeof (Tc.Crm) === "undefined") {
    Tc.Crm = {
        __namespace: true
    };
}
if (typeof (Tc.Crm.Scripts) === "undefined") {
    Tc.Crm.Scripts = {
        __namespace: true
    };
}
if (typeof (Tc.Crm.Scripts.Events) === "undefined") {
    Tc.Crm.Scripts.Events = {
        __namespace: true
    };
}
Tc.Crm.Scripts.Events.Compensation = (function () {
    "use strict";
    var FORM_MODE_CREATE = 1;
    var CLIENT_MODE_MOBILE = "Mobile";
    var CLIENT_STATE_OFFLINE = "Offline";

    var SOURCE_MARKET_ENTITY = "tc_country";
    var SOURCE_MARKET_ENTITY_PLURAL = "tc_countries";
    var CASE_ENTITY = "incident";
    var CONTACT_ENTITY = "contact";
    var BOOKING_ENTITY = "tc_booking";
    var COMPENSATION_ENTITY = "tc_compensation";

    var COMPENSATION_SOURCEMARKETID_FIELDNAME = "tc_SourceMarketId";

    var COMPENSATION_CASEID_CONTROLNAME = "tc_caseid";
    var COMPENSATION_SOURCEMARKETID_CONTROLNAME = "tc_sourcemarketid";
    var COMPENSATION_LANGUAGE_CONTROLNAME = "tc_language";
    var CUSTOMER_LANGUAGE_CONTROLNAME = "tc_language";
    var CASE_SOURCEMARKETID_CONTROLNAME = "tc_sourcemarketid";

    var updateCompensationFields = function () {

        if (Xrm.Page.ui.getFormType() !== FORM_MODE_CREATE) {
            return;
        }

        // Web and Outlook or phones and tablets in online mode
        var caseId = formatEntityId(Xrm.Page.getAttribute(COMPENSATION_CASEID_CONTROLNAME).getValue()[0].id);

        var caseReceivedPromise = getCase(caseId);

        var customerReceivedPromise = caseReceivedPromise.then(
            function (caseResponse) {
                var incident = null;

                if (Xrm.Page.context.client.getClient() === CLIENT_MODE_MOBILE && Xrm.Page.context.client.getClientState() === CLIENT_STATE_OFFLINE) {
                    incident = caseResponse;
                } else {
                    incident = JSON.parse(caseResponse.response);
                }

                var customer = incident.customerid_contact;

                if (customer !== null) {
                    var customerId;
                    if (incident._customerid_value !== null) {
                        customerId = incident._customerid_value;
                    }
                    else {
                        customerId = incident.customerid;
                    }

                    return getCustomer(customerId);
                }
            },
            function (error) {
                console.warn("Unable to get the case");
            }
        );

        // Booking and source information from case
        var bookingReceivedPromise = caseReceivedPromise.then(
            function (caseResponse) {
                var incident = JSON.parse(caseResponse.response);
                var bookingUsed = incident.tc_bookingreference;
                var bookingId = incident._tc_bookingid_value;

                if (bookingUsed) {
                    // Get source market from booking
                    return getBooking(bookingId);
                }
                else {
                    // Set source market from case
                    var sourceMarket = incident.tc_sourcemarketid;

                    var sourceMarketReference = [];

                    if (sourceMarket !== null) {
                        sourceMarketReference[0] = {};
                        sourceMarketReference[0].id = sourceMarket.tc_countryid;
                        sourceMarketReference[0].entityType = SOURCE_MARKET_ENTITY;
                        sourceMarketReference[0].name = sourceMarket.tc_iso_code;
                    }

                    Xrm.Page.getAttribute(COMPENSATION_SOURCEMARKETID_CONTROLNAME).setValue(sourceMarketReference);
                }
            },
            function (error) {
                console.warn("Unable to get the booking");
            }
        );

        customerReceivedPromise.then(
            function (customerResponse) {
                if (customerResponse === undefined) {
                    // Customer is account, do nothing in that situation
                    return;
                }

                var customer = JSON.parse(customerResponse.response);
                var language = customer.tc_language;

                Xrm.Page.getControl(COMPENSATION_LANGUAGE_CONTROLNAME).getAttribute().setValue(language);
            },
            function (error) {
                console.warn("Unable to get the customer");
            }
        );

        // Set source market from booking
        bookingReceivedPromise.then(
            function (bookingResponse) {
                if (bookingResponse === undefined) {
                    // Booking is not assigned to the case
                    return;
                }

                var booking = JSON.parse(bookingResponse.response);

                var sourceMarket = booking.tc_SourceMarketId;

                var sourceMarketReference = [];

                if (sourceMarket !== null) {
                    sourceMarketReference[0] = {};
                    sourceMarketReference[0].id = sourceMarket.tc_countryid;
                    sourceMarketReference[0].entityType = SOURCE_MARKET_ENTITY;
                    sourceMarketReference[0].name = sourceMarket.tc_iso_code;
                }

                Xrm.Page.getAttribute(COMPENSATION_SOURCEMARKETID_CONTROLNAME).setValue(sourceMarketReference);
            },
            function (error) {
                console.warn("Problem getting booking");
            }
        );

    }

    function showOfflineMessage(message) {
        if (Xrm.Page.context.client.getClient() === CLIENT_MODE_MOBILE && Xrm.Page.context.client.getClientState() === CLIENT_STATE_OFFLINE) {
            Xrm.Utility.alertDialog(message);
        }
    }

    function getCase(caseId) {

        var entityName = CASE_ENTITY;

        if (Xrm.Page.context.client.getClient() === CLIENT_MODE_MOBILE && Xrm.Page.context.client.getClientState() === CLIENT_STATE_OFFLINE) {
            // phones and tablets in offline mode
            var query = "?$select=tc_sourcemarketid,customerid,tc_bookingreference,tc_bookingid&$expand=tc_sourcemarketid($select=tc_iso_code,tc_countryid),customerid_contact($select=tc_language,contactid,fullname)";
            return Xrm.Mobile.offline.retrieveRecord(entityName, caseId, query);
        }
        else {
            var query = "?$select=_tc_sourcemarketid_value,_customerid_value,tc_bookingreference,_tc_bookingid_value&$expand=tc_sourcemarketid($select=tc_iso_code,tc_countryid),customerid_contact($select=tc_language,contactid,fullname)";
            return Tc.Crm.Scripts.Common.GetById(entityName, caseId, query);
        }

        return Tc.Crm.Scripts.Common.GetById(entityName, caseId, query);
    }

    function getCustomer(customerId) {
        var query = "?$select=tc_language,contactid,fullname";
        var entityName = CONTACT_ENTITY;

        return Tc.Crm.Scripts.Common.GetById(entityName, customerId, query);
    }

    function getBooking(bookingId) {
        var query = "?$select=_tc_sourcemarketid_value&$expand=tc_SourceMarketId($select=tc_iso_code,tc_countryid)";
        var entityName = BOOKING_ENTITY;

        return Tc.Crm.Scripts.Common.GetById(entityName, bookingId, query);
    }

    var updateRelatedCompensationsForCustomer = function () {

        if (Xrm.Page.ui.getFormType() === FORM_MODE_CREATE) {
            return;
        }

        var languageChanged = Xrm.Page.getControl(CUSTOMER_LANGUAGE_CONTROLNAME).getAttribute().getIsDirty();

        if (languageChanged) {
            var customerId = formatEntityId(Xrm.Page.data.entity.getId());
            var language = Xrm.Page.getControl(CUSTOMER_LANGUAGE_CONTROLNAME).getAttribute().getValue();

            getCustomerCases(customerId).then(
                function (casesResponse) {
                    var cases = null;
                    if (Xrm.Page.context.client.getClient() === CLIENT_MODE_MOBILE && Xrm.Page.context.client.getClientState() === CLIENT_STATE_OFFLINE) {
                        cases = casesResponse.values;
                    } else {
                        cases = JSON.parse(casesResponse.response).value;
                    }

                    for (var i = 0; i < cases.length; i++) {
                        var caseId = cases[i].incidentid;

                        if (language !== null) {
                            updateDataInRelatedCompensations(getCaseCompensations(caseId), { tc_language: language });
                        }
                    }
                },
                function (error) {
                    console.warn("Error getting customer cases");
                }
            );
        }
    }

    function getCustomerCases(customerId) {
        var entityName = CASE_ENTITY;

        if (Xrm.Page.context.client.getClient() === CLIENT_MODE_MOBILE && Xrm.Page.context.client.getClientState() === CLIENT_STATE_OFFLINE) {
            // phones and tablets in offline mode
            var query = "?$filter=customerid eq " + customerId + " &$select=incidentid";
            return Xrm.Mobile.offline.retrieveMultipleRecords(entityName, query);
        }
        else {
            var query = "?$filter=_customerid_value eq " + customerId + " &$select=incidentid";
            return Tc.Crm.Scripts.Common.Get(entityName, query);
        }
    }

    var updateSourceMarketInRelatedCompensationsForCase = function () {

        if (Xrm.Page.ui.getFormType() === FORM_MODE_CREATE) {
            return;
        }

        var sourceMarketChanged = Xrm.Page.getControl(CASE_SOURCEMARKETID_CONTROLNAME).getAttribute().getIsDirty();

        if (sourceMarketChanged) {
            var caseId = formatEntityId(Xrm.Page.data.entity.getId());
            var sourceMarketId = formatEntityId(Xrm.Page.getControl(CASE_SOURCEMARKETID_CONTROLNAME).getAttribute().getValue()[0].id);

            var compensation = {};
            compensation[COMPENSATION_SOURCEMARKETID_FIELDNAME + "@odata.bind"] = "/" + SOURCE_MARKET_ENTITY_PLURAL + "(" + sourceMarketId + ")";

            updateDataInRelatedCompensations(getCaseCompensations(caseId), compensation);
        }
    }

    function updateDataInRelatedCompensations(compensationsReceivedPromise, compensationFields) {
        compensationsReceivedPromise.then(
            function (compensationsResponse) {
                var compensations = null;
                if (Xrm.Page.context.client.getClient() === CLIENT_MODE_MOBILE && Xrm.Page.context.client.getClientState() === CLIENT_STATE_OFFLINE) {
                    compensations = compensationsResponse.values;
                }
                else {
                    compensations = JSON.parse(compensationsResponse.response).value;
                }

                for (var i = 0; i < compensations.length; i++) {

                    var compensationId = compensations[i].tc_compensationid;

                    if (Xrm.Page.context.client.getClient() === CLIENT_MODE_MOBILE && Xrm.Page.context.client.getClientState() === CLIENT_STATE_OFFLINE) {
                        //showOfflineMessage("compensationId: " + compensationId);
                        //TODO
                    }
                    else {
                        updateCompensation(compensationId, compensationFields).then(
                            function (response) {
                            },
                            function (error) {
                                console.warn("Error updating compensation");
                            });
                    }
                }
            },
            function (error) {
                //showOfflineMessage("Error updateDataInRelatedCompensations: " + error.message);
                console.warn("Error getting compensations");
            }
        )
    }

    function updateCompensation(compensationId, compensation) {
        var entityName = COMPENSATION_ENTITY;

        return Tc.Crm.Scripts.Common.Update(entityName, compensationId, compensation);
    }

    function getCaseCompensations(caseId) {
        var entityName = COMPENSATION_ENTITY;

        if (Xrm.Page.context.client.getClient() === CLIENT_MODE_MOBILE && Xrm.Page.context.client.getClientState() === CLIENT_STATE_OFFLINE) {
            // phones and tablets in offline mode
            var query = "?$filter=tc_caseid eq " + caseId + " &$select=tc_compensationid";
            return Xrm.Mobile.offline.retrieveMultipleRecords(entityName, query);
        }
        else {
            var query = "?$filter=_tc_caseid_value eq " + caseId + " &$select=tc_compensationid";
            return Tc.Crm.Scripts.Common.Get(entityName, query);

        }

    }

    function formatEntityId(id) {
        if (id != null) {
            id = id.replace("{", "").replace("}", "");
        }

        return id;
    }

    return {
        OnLoad: function () {
            //updateCompensationFields();
            // TODO: commented till code review
        },
        OnCustomerSave: function () {
            //updateRelatedCompensationsForCustomer();
            // TODO: commented till code review
        },
        OnCaseSave: function () {
            //updateSourceMarketInRelatedCompensationsForCase();
            // TODO: commented till code review
        }
    };
})();