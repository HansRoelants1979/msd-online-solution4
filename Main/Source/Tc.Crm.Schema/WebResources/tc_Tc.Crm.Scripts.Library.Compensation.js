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
if (typeof (Tc.Crm.Scripts.Library) === "undefined") {
    Tc.Crm.Scripts.Library = {
        __namespace: true
    };
}
Tc.Crm.Scripts.Library.Compensation = (function () {
    "use strict";
    var FORM_MODE_CREATE = 1;
    var CLIENT_MODE_MOBILE = "Mobile";
    var CLIENT_STATE_OFFLINE = "Offline";

    var SOURCE_MARKET_ENTITY = "tc_country";
    var SOURCE_MARKET_ENTITY_PLURAL = "tc_countries";
    var CASE_ENTITY_SET_NAME = "incidents";
    var CONTACT_ENTITY_SET_NAME = "contacts";
    var BOOKING_ENTITY_SET_NAME = "tc_bookings";
    var COMPENSATION_ENTITY_SET_NAME = "tc_compensations";

    var COMPENSATION_SOURCEMARKETID_FIELDNAME = "tc_SourceMarketId";

    var COMPENSATION_CASEID_ATTR_NAME = "tc_caseid";
    var COMPENSATION_SOURCEMARKETID_ATTR_NAME = "tc_sourcemarketid";
    var COMPENSATION_LANGUAGE_ATTR_NAME = "tc_language";
    var CUSTOMER_LANGUAGE_ATTR_NAME = "tc_language";
    var CASE_SOURCEMARKETID_ATTR_NAME = "tc_sourcemarketid";

    function formatEntityId(id) {
        return id !== null ? id.replace("{", "").replace("}", "") : null;
    }

    function IsMobileOfflineMode() {
        return Xrm.Page.context.client.getClient() === CLIENT_MODE_MOBILE && Xrm.Page.context.client.getClientState() === CLIENT_STATE_OFFLINE
    }

    function getBooking(bookingId) {
        var query = "?$select=_tc_sourcemarketid_value&$expand=tc_SourceMarketId($select=tc_iso_code,tc_countryid)";
        return Tc.Crm.Scripts.Common.GetById(BOOKING_ENTITY_SET_NAME, bookingId, query);
        // TODO: offline mode
    }
     
    function getCustomer(customerId) {
        var query = "?$select=tc_language,contactid,fullname";
        return Tc.Crm.Scripts.Common.GetById(CONTACT_ENTITY_SET_NAME, customerId, query);
        // TODO: offline mode
    }

    function getCustomerCases(customerId) {
        if (IsMobileOfflineMode()) {
            // phones and tablets in offline mode
            var query = "?$filter=customerid eq " + customerId + " &$select=incidentid";
            return Xrm.Mobile.offline.retrieveMultipleRecords(CASE_ENTITY_SET_NAME, query);
        }
        else {
            var query = "?$filter=_customerid_value eq " + customerId + " &$select=incidentid";
            return Tc.Crm.Scripts.Common.Get(CASE_ENTITY_SET_NAME, query);
        }
    }

    function getCase(caseId) {
        if (IsMobileOfflineMode()) {
            // phones and tablets in offline mode
            var query = "?$select=tc_sourcemarketid,customerid,tc_bookingreference,tc_bookingid&$expand=tc_sourcemarketid($select=tc_iso_code,tc_countryid),customerid_contact($select=tc_language,contactid,fullname)";
            return Xrm.Mobile.offline.retrieveRecord(CASE_ENTITY_SET_NAME, caseId, query);
        }
        else {
            var query = "?$select=_tc_sourcemarketid_value,_customerid_value,tc_bookingreference,_tc_bookingid_value&$expand=tc_sourcemarketid($select=tc_iso_code,tc_countryid),customerid_contact($select=tc_language,contactid,fullname)";
            return Tc.Crm.Scripts.Common.GetById(CASE_ENTITY_SET_NAME, caseId, query);
        }
    }

    function getCaseCompensations(caseId) {
        if (IsMobileOfflineMode()) {
            // phones and tablets in offline mode
            var query = "?$filter=tc_caseid eq " + caseId + " &$select=tc_compensationid";
            return Xrm.Mobile.offline.retrieveMultipleRecords(COMPENSATION_ENTITY_SET_NAME, query);
        }
        else {
            var query = "?$filter=_tc_caseid_value eq " + caseId + " &$select=tc_compensationid";
            return Tc.Crm.Scripts.Common.Get(COMPENSATION_ENTITY_SET_NAME, query);
        }
    }

    function setSourceMarket(sourceMarket) {
        var sourceMarketReference = [1];
        sourceMarketReference[0] = {};
        sourceMarketReference[0].id = sourceMarket.tc_countryid;
        sourceMarketReference[0].entityType = SOURCE_MARKET_ENTITY;
        sourceMarketReference[0].name = sourceMarket.tc_iso_code;

        if (Xrm.Page.getAttribute(COMPENSATION_SOURCEMARKETID_ATTR_NAME) === null) {
            Xrm.Utility.alertDialog("Unable to find source market");
            console.warn("Unable to find source market");
            return;
        }

        Xrm.Page.getAttribute(COMPENSATION_SOURCEMARKETID_ATTR_NAME).setValue(sourceMarketReference);
    }

    function getPromiseResponse(promiseResponse, entity) {
        if (IsMobileOfflineMode()) {
            return promiseResponse.values;
        }
        else {
            if (promiseResponse.response === null || promiseResponse.response === undefined) {
                Xrm.Utility.alertDialog(entity + " information can't be retrieved");
                console.warn(entity + " information can't be retrieved");
                return [];
            }
            try {
                return JSON.parse(promiseResponse.response).value;
            }
            catch (e) {
                Xrm.Utility.alertDialog(entity + " information can't be parsed");
                console.warn(entity + " information can't be parsed");
                return [];
            }
        }
    }

    function updateCompensations(compensationsReceivedPromise, compensationFields) {
        compensationsReceivedPromise.then(
            function (compensationsResponse) {
                var compensations = getPromiseResponse(compensationsResponse, "Compensation");
                compensations.forEach(function (compensation) {
                    if (IsMobileOfflineMode()) {
                        //TODO
                    }
                    else {
                        return Tc.Crm.Scripts.Common.Update(COMPENSATION_ENTITY_SET_NAME, compensation.tc_compensationid, compensationFields).then(
                            function (response) { },
                            function (error) {
                                Xrm.Utility.alertDialog("Error updating compensation");
                                console.warn("Error updating compensation");
                            });
                    }
                });
            },
            function (error) {
                Xrm.Utility.alertDialog("Error getting compensations");
                console.warn("Error getting compensations");
            }
        )
    }

    function initLanguage(caseResponse) {
        var incident = JSON.parse(caseResponse.response);
        var customerId = incident._customerid_value !== null ? incident._customerid_value :incident.customerid;
        getCustomer(customerId).then(
            function (customerResponse) {
                var customer = JSON.parse(customerResponse.response);
                var language = customer.tc_language;
                var attribute = Xrm.Page.getAttribute(COMPENSATION_LANGUAGE_ATTR_NAME);
                attribute.setValue(language);
            },
            function (error) {
                if (error && error.message && error.message.toUpperCase().indexOf("DOES NOT EXIST") > -1) {
                    // Customer is account, do nothing in that situation
                    return;
                }
                Xrm.Utility.alertDialog("Unable to get the customer");
                console.warn("Unable to get the customer");
            }
        );
    }

    function initSourceMarket(caseResponse) {
        var incident = JSON.parse(caseResponse.response);
        var bookingUsed = incident.tc_bookingreference;
        if (bookingUsed) {
            var bookingId = incident._tc_bookingid_value;
            // Get source market from booking
            getBooking(bookingId).then(
                function (bookingResponse) {
                    var booking = JSON.parse(bookingResponse.response);
                    var sourceMarket = booking.tc_SourceMarketId;
                    setSourceMarket(sourceMarket);
                },
                function (error) {
                    Xrm.Utility.alertDialog("Problem getting booking");
                    console.warn("Problem getting booking");
                }
            );
        }
        else {
            // Set source market from case
            var sourceMarket = incident.tc_sourcemarketid;
            setSourceMarket(sourceMarket);
        }
    }

    /// Public implementation

    var updateCustomerCompensationsLanguage = function () {
        if (IsMobileOfflineMode()) return;
        if (Xrm.Page.ui.getFormType() === FORM_MODE_CREATE)
            return;
        var attrLanguage = Xrm.Page.getAttribute(CUSTOMER_LANGUAGE_ATTR_NAME);
        if (attrLanguage === null || !attrLanguage.getIsDirty())
            return;
        var language = attrLanguage.getValue();
        var customerId = formatEntityId(Xrm.Page.data.entity.getId());
        getCustomerCases(customerId).then(
            function (casesResponse) {
                var cases = getPromiseResponse(casesResponse, "Case");
                cases.forEach(function (incident) {
                    var caseCompensationsPromise = getCaseCompensations(incident.incidentid);
                    updateCompensations(caseCompensationsPromise, { tc_language: language });
                });
            },
            function (error) {
                Xrm.Utility.alertDialog("Error getting customer cases");
                console.warn("Error getting customer cases");
            }
        );
    }

    var updateCaseCompensationsSourceMarket = function () {
        if (IsMobileOfflineMode()) return;
        if (Xrm.Page.ui.getFormType() === FORM_MODE_CREATE)
            return;
        var attribute = Xrm.Page.getAttribute(CASE_SOURCEMARKETID_ATTR_NAME);
        if (attribute === null || !attribute.getIsDirty())
            return;
        var caseId = formatEntityId(Xrm.Page.data.entity.getId());
        var sourceMarketId = formatEntityId(attribute.getValue()[0].id);
        var compensation = {};
        compensation[COMPENSATION_SOURCEMARKETID_FIELDNAME + "@odata.bind"] = "/" + SOURCE_MARKET_ENTITY_PLURAL + "(" + sourceMarketId + ")";
        updateCompensations(getCaseCompensations(caseId), compensation);
    }

    var setDefaultsOnCreate = function () {
        if (IsMobileOfflineMode()) return;
        if (Xrm.Page.ui.getFormType() !== FORM_MODE_CREATE)
            return;
        var caseIdAttr = Xrm.Page.getAttribute(COMPENSATION_CASEID_ATTR_NAME);
        var caseId = formatEntityId(caseIdAttr.getValue()[0].id);
        var caseReceivedPromise = getCase(caseId);
        // set language
        caseReceivedPromise.then(
            initLanguage,
            function (error) {
                console.warn("Unable to get the case");
            }
        );
        // set  source market from booking or case
        caseReceivedPromise.then(
            initSourceMarket,
            function (error) {}
        );
    }

    // public
    return {
        SetDefaultsOnCreate: function () {            
            setDefaultsOnCreate();
        },
        UpdateCustomerCompensationsLanguage: function () {
            updateCustomerCompensationsLanguage();
        },
        UpdateCaseCompensationsSourceMarket: function () {
            updateCaseCompensationsSourceMarket();
        }
    };
})();