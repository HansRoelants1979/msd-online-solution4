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
    var FORM_MODE_QUICK_CREATE = 5;
    var CLIENT_STATE_OFFLINE = "Offline";

    var EntityNames = {
        Country: "tc_country",
        Case: "incident",
        Booking: "tc_booking",
        Compensation: "tc_compensation"
    }

    var EntitySetNames = {
        Country: "tc_countries",
        Case: "incidents",
        Booking: "tc_bookings",
        Compensation: "tc_compensations"
    }    

    var COMPENSATION_CASEID_ATTR_NAME = "tc_caseid";
    var COMPENSATION_SOURCEMARKETID_ATTR_NAME = "tc_sourcemarketid";
    var COMPENSATION_LANGUAGE_ATTR_NAME = "tc_language";

    function formatEntityId(id) {
        return id !== null ? id.replace("{", "").replace("}", "") : null;
    }

    function getPromiseResponse(promiseResponse, entity) {
        if (promiseResponse == null) return null;
        if (IsOfflineMode()) {
            return promiseResponse.values != null ? promiseResponse.values : promiseResponse;
        }
        else {
            if (promiseResponse.response === null || promiseResponse.response === undefined) {
                console.warn(entity + " information can't be retrieved");
                return null;
            }
            try {
                var result = JSON.parse(promiseResponse.response);
                return result.values != null ? result.values : result;
            }
            catch (e) {
                console.warn(entity + " information can't be parsed");
                return null;
            }
        }
    }

    function IsOfflineMode() {
        return Xrm.Page.context.client.getClientState() === CLIENT_STATE_OFFLINE;
    }

    function getBooking(bookingId) {
        // 1 random value in select to don't return all
        var query = "?$select=tc_duration&$expand=tc_SourceMarketId($select=tc_iso_code,tc_countryid)";
        if (!IsOfflineMode()) {            
            return Tc.Crm.Scripts.Common.GetById(EntitySetNames.Booking, bookingId, query);
        } else {
            return Xrm.Mobile.offline.retrieveRecord(EntityNames.Booking, bookingId, query);
        }        
    }
 
    function getCase(caseId) {
        if (!IsOfflineMode()) {
            var query = "?$select=tc_bookingreference,_tc_bookingid_value&$expand=tc_sourcemarketid($select=tc_countryid,tc_iso_code),customerid_contact($select=tc_language)";
            return Tc.Crm.Scripts.Common.GetById(EntitySetNames.Case, caseId, query);
        }
        else {
            var query = "?$select=tc_bookingreference,tc_bookingid&$expand=tc_sourcemarketid($select=tc_iso_code,tc_countryid),customerid_contact($select=tc_language)";
            return Xrm.Mobile.offline.retrieveRecord(EntityNames.Case, caseId, query);
        }
    }

    function parseCase(response) {
        if (response == null) return null;
        var result = {
            bookingId: response._tc_bookingid_value
        };
        if (!IsOfflineMode()) {
            result.hasBookingReference = response.tc_bookingreference;
            result.language = response.customerid_contact != null ? response.customerid_contact.tc_language : null;
            result.sourceMarket = response.tc_sourcemarketid != null ? {
                id: response.tc_sourcemarketid.tc_countryid,
                name: response.tc_sourcemarketid.tc_iso_code
            } : null;
        } else {
            result.hasBookingReference = response.tc_bookingreference.toLowerCase() === "true";
            result.language = response.customerid_contact != null && response.customerid_contact[0] != null ? response.customerid_contact[0].tc_language : null;
            result.sourceMarket = response.tc_sourcemarketid != null && response.tc_sourcemarketid[0] != null? {
                id: response.tc_sourcemarketid[0].tc_countryid,
                name: response.tc_sourcemarketid[0].tc_iso_code
            } : null;
        }
        return result;
    }

    function parseBooking(response) {
        if (response == null) return null;
        var result = null;
        if (!IsOfflineMode()) {
            if (response.tc_SourceMarketId != null) {
                result = {
                    sourceMarket: {
                        id: response.tc_SourceMarketId.tc_countryid,
                        name: response.tc_SourceMarketId.tc_iso_code
                    }
                }
            }
        } else {
            if (response.tc_SourceMarketId != null && response.tc_SourceMarketId[0] != null) {
                result = {
                    sourceMarket: {
                        id: response.tc_SourceMarketId[0].tc_countryid,
                        name: response.tc_SourceMarketId[0].tc_iso_code
                    }
                }
            }
        }
        return result;
    }

    function setSourceMarket(sourceMarket) {
        if (sourceMarket == null) return;
        var sourceMarketReference = [1];
        sourceMarketReference[0] = {
            id: sourceMarket.id,
            name: sourceMarket.name,
            entityType: EntityNames.Country            
        };
        var attr = Xrm.Page.getAttribute(COMPENSATION_SOURCEMARKETID_ATTR_NAME);
        if (attr != null) {
            attr.setValue(sourceMarketReference);
            attr.fireOnChange();
        }
    }

    function initSourceMarket(incident) {
        if (incident == null) return;
        if (incident.hasBookingReference) {
            // Get source market from booking
            getBooking(incident.bookingId).then(
                function (bookingResponse) {
                    var parsedResponse = getPromiseResponse(bookingResponse);
                    var booking = parseBooking(parsedResponse);
                    if (booking == null) return;
                    setSourceMarket(booking.sourceMarket);
                },
                function (error) {
                    console.warn("Problem getting booking");
                }
            );
        }
        else {
            // Set source market from case
            setSourceMarket(incident.sourceMarket);
        }
    }

    // set language and source market of compensation on create
    var setDefaultsOnCreate = function () {
        var isCreate = Xrm.Page.ui.getFormType() === FORM_MODE_CREATE || Xrm.Page.ui.getFormType() === FORM_MODE_QUICK_CREATE;
        if (!isCreate) return;
        var caseIdAttr = Xrm.Page.getAttribute(COMPENSATION_CASEID_ATTR_NAME);
        if (caseIdAttr == null || caseIdAttr.getValue() == null || caseIdAttr.getValue()[0] == null) return;
        var caseId = formatEntityId(caseIdAttr.getValue()[0].id);
        var caseReceivedPromise = getCase(caseId);        
        caseReceivedPromise.then(function (response) {
                var parsedResponse = getPromiseResponse(response);
                var incident = parseCase(parsedResponse);
                if (incident == null) return;
                // set language                
                var attribute = Xrm.Page.getAttribute(COMPENSATION_LANGUAGE_ATTR_NAME);
                if (attribute != null) {
                    attribute.setValue(incident.language);
                    attribute.fireOnChange();
                }
                // set  source market from booking or case
                initSourceMarket(incident);
            },
            function (error) {
                console.warn("Unable to get the case");
            }
        );        
    }

    // public
    return {
        SetDefaultsOnCreate: setDefaultsOnCreate
    };
})();