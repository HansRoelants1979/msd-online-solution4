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
    var CASE_LINE_ENTITY = "tc_caseline";

    var COMPENSATION_SOURCEMARKETID_FIELDNAME = "tc_SourceMarketId";

    var COMPENSATION_CASEID_ATTR_NAME = "tc_caseid";
    var COMPENSATION_SOURCEMARKETID_ATTR_NAME = "tc_sourcemarketid";
    var COMPENSATION_LANGUAGE_ATTR_NAME = "tc_language";
    var CUSTOMER_LANGUAGE_ATTR_NAME = "tc_language";
    var CASE_SOURCEMARKETID_ATTR_NAME = "tc_sourcemarketid";

    var COMPENSATION1_ATTR_NAME = "tc_compensation1id";
    var COMPENSATION2_ATTR_NAME = "tc_compensation2id";
    var COMPENSATION3_ATTR_NAME = "tc_compensation3id";
    var COMPENSATION4_ATTR_NAME = "tc_compensation4id";
    var COMPENSATION5_ATTR_NAME = "tc_compensation5id";

    var SEVERITYCAT1_ATTR_NAME = "tc_cat1severity";
    var SEVERITYCAT2_ATTR_NAME = "tc_cat2severity";
    var SEVERITYCAT3_ATTR_NAME = "tc_cat3severity";
    var SEVERITYCAT4_ATTR_NAME = "tc_cat4severity";
    var SEVERITYCAT5_ATTR_NAME = "tc_cat5severity";

    var IMPACTCAT1_ATTR_NAME = "tc_cat1impact";
    var IMPACTCAT2_ATTR_NAME = "tc_cat2impact";
    var IMPACTCAT3_ATTR_NAME = "tc_cat3impact";
    var IMPACTCAT4_ATTR_NAME = "tc_cat4impact";
    var IMPACTCAT5_ATTR_NAME = "tc_cat5impact";

    var DAYSAFFECTEDCAT1_ATTR_NAME = "tc_cat1_daysaffected";
    var DAYSAFFECTEDCAT2_ATTR_NAME = "tc_cat2_daysaffected";
    var DAYSAFFECTEDCAT3_ATTR_NAME = "tc_cat3_daysaffected";
    var DAYSAFFECTEDCAT4_ATTR_NAME = "tc_cat4_daysaffected";
    var DAYSAFFECTEDCAT5_ATTR_NAME = "tc_cat5_daysaffected";

    

    var formatValue6DigitsWithHyphen = function (context) {
        var formatted = false;
        var attribute = context.getEventSource();
        var value = attribute.getValue();
        if (/^\d{6}$/.test(value)) {
            var formattedValue = value[0] + value[1] + '-' + value[2] + value[3] + '-' + value[4] + value[5];
            attribute.setValue(formattedValue);
            formatted = true;
        }
        var isValid = formatted || /^\d{2}-\d{2}-\d{2}$/.test(value);
        attribute.controls.forEach(
            function (control, i) {
                if (isValid) {
                    control.clearNotification();
                } else {
                    control.setNotification("Should be 6 digit format nn-nn-nn");
                }
            });
    }

    var updateCompensationFields = function () {
        if (Xrm.Page.ui.getFormType() !== FORM_MODE_CREATE) {
            return;
        }

        if (Xrm.Page.getAttribute(COMPENSATION_CASEID_ATTR_NAME) === null) {
            Xrm.Utility.alertDialog("Unable to find case id");
            console.warn("Unable to find case id");
            return;
        }

        var caseId = formatEntityId(Xrm.Page.getAttribute(COMPENSATION_CASEID_ATTR_NAME).getValue()[0].id);

        var caseReceivedPromise = getCase(caseId);

        caseReceivedPromise.then(
            function (caseResponse) {
                if (caseResponse === null || caseResponse === undefined) {
                    Xrm.Utility.alertDialog("Unable to get case information");
                    console.warn("Unable to get case information");
                    return;
                }

                var incident = null;

                if (Xrm.Page.context.client.getClient() === CLIENT_MODE_MOBILE && Xrm.Page.context.client.getClientState() === CLIENT_STATE_OFFLINE) {
                    incident = caseResponse;
                } else {
                    if (caseResponse.response === null || caseResponse.response === undefined) {
                        Xrm.Utility.alertDialog("Case information can't be retrieved");
                        console.warn("Case information can't be retrieved");
                        return;
                    }

                    try {
                        incident = JSON.parse(caseResponse.response);
                    }
                    catch (e) {
                        Xrm.Utility.alertDialog("Case information can't be parsed");
                        console.warn("Case information can't be parsed");
                        return;
                    }
                }

                var customerId;
                if (incident._customerid_value !== null) {
                    customerId = incident._customerid_value;
                }
                else {
                    customerId = incident.customerid;
                }

                if (customerId === null || customerId === undefined) {
                    Xrm.Utility.alertDialog("Error getting customer Id");
                    console.warn("Error getting customer Id");
                    return;
                }

                getCustomer(customerId).then(
                    function (customerResponse) {

                        if (customerResponse === null || customerResponse === undefined) {
                            Xrm.Utility.alertDialog("Unable to get customer information");
                            console.warn("Unable to get customer information");
                            return;
                        }

                        var customer;
                        try {
                            customer = JSON.parse(customerResponse.response);
                        }
                        catch (e) {
                            Xrm.Utility.alertDialog("Customer information can't be parsed");
                            console.warn("Customer information can't be parsed");
                            return;
                        }

                        var language = customer.tc_language;

                        if (Xrm.Page.getAttribute(COMPENSATION_LANGUAGE_ATTR_NAME) === null) {
                            Xrm.Utility.alertDialog("Unable to find language");
                            console.warn("Unable to find language");
                            return;
                        }

                        Xrm.Page.getAttribute(COMPENSATION_LANGUAGE_ATTR_NAME).setValue(language);
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
            },
            function (error) {
                Xrm.Utility.alertDialog("Unable to get the case");
                console.warn("Unable to get the case");
            }
        );

        // Booking and source information from case
        caseReceivedPromise.then(
            function (caseResponse) {

                if (caseResponse === null || caseResponse === undefined) {
                    //Do nothing, should be logged before
                    return;
                }

                var incident = null;

                try {
                    incident = JSON.parse(caseResponse.response);
                }
                catch (e) {
                    //Do nothing, should be logged before
                    return;
                }

                var bookingUsed = incident.tc_bookingreference;

                if (bookingUsed) {

                    var bookingId = incident._tc_bookingid_value;

                    if (bookingId === null || bookingId === undefined) {
                        Xrm.Utility.alertDialog("Error getting Booking Id");
                        console.warn("Error getting Booking Id");
                        return;
                    }

                    // Get source market from booking
                    getBooking(bookingId).then(
                        function (bookingResponse) {

                            if (bookingResponse === null || bookingResponse === undefined) {
                                Xrm.Utility.alertDialog("Unable to get Booking information");
                                console.warn("Unable to get Booking information");
                                return;
                            }

                            var booking = null;
                            try {
                                booking = JSON.parse(bookingResponse.response);
                            }
                            catch (e) {
                                Xrm.Utility.alertDialog("Booking information can't be parsed");
                                console.warn("Booking information can't be parsed");
                                return;
                            }

                            var sourceMarket = booking.tc_SourceMarketId;

                            if (sourceMarket === null || sourceMarket === undefined) {
                                Xrm.Utility.alertDialog("Error getting source market");
                                console.warn("Error getting source market");
                                return;
                            }

                            var sourceMarketReference = [];
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

                    if (sourceMarket === null || sourceMarket === undefined) {
                        Xrm.Utility.alertDialog("Error getting source market from case");
                        console.warn("Error getting source market from case");
                        return;
                    }

                    var sourceMarketReference = [];

                    if (sourceMarket !== null) {
                        sourceMarketReference[0] = {};
                        sourceMarketReference[0].id = sourceMarket.tc_countryid;
                        sourceMarketReference[0].entityType = SOURCE_MARKET_ENTITY;
                        sourceMarketReference[0].name = sourceMarket.tc_iso_code;
                    }

                    if (Xrm.Page.getAttribute(COMPENSATION_SOURCEMARKETID_ATTR_NAME) === null) {
                        Xrm.Utility.alertDialog("Unable to find source market");
                        console.warn("Unable to find source market");
                        return;
                    }

                    Xrm.Page.getAttribute(COMPENSATION_SOURCEMARKETID_ATTR_NAME).setValue(sourceMarketReference);
                }
            },
            function (error) {
                //Do nothing, should be logged before
            }
        );
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

        if (Xrm.Page.getAttribute(CUSTOMER_LANGUAGE_ATTR_NAME) === null) {
            Xrm.Utility.alertDialog("Unable to find language");
            console.warn("Unable to find language");
            return;
        }

        var languageChanged = Xrm.Page.getAttribute(CUSTOMER_LANGUAGE_ATTR_NAME).getIsDirty();

        if (languageChanged) {
            var customerId = formatEntityId(Xrm.Page.data.entity.getId());

            var language = Xrm.Page.getAttribute(CUSTOMER_LANGUAGE_ATTR_NAME).getValue();

            getCustomerCases(customerId).then(
                function (casesResponse) {
                    if (casesResponse === null || casesResponse === undefined) {
                        Xrm.Utility.alertDialog("Unable to get cases information");
                        console.warn("Unable to get cases information");
                        return;
                    }

                    var cases = null;
                    if (Xrm.Page.context.client.getClient() === CLIENT_MODE_MOBILE && Xrm.Page.context.client.getClientState() === CLIENT_STATE_OFFLINE) {
                        cases = casesResponse.values;
                    } else {
                        if (casesResponse.response === null || casesResponse.response === undefined) {
                            Xrm.Utility.alertDialog("Cases information can't be retrieved");
                            console.warn("Cases information can't be retrieved");
                            return;
                        }

                        try {
                            cases = JSON.parse(casesResponse.response).value;
                        }
                        catch (e) {
                            Xrm.Utility.alertDialog("Cases information can't be parsed");
                            console.warn("Cases information can't be parsed");
                            return;
                        }
                    }

                    for (var i = 0; i < cases.length; i++) {
                        var caseId = cases[i].incidentid;

                        if (caseId === null || caseId === undefined) {
                            Xrm.Utility.alertDialog("Error getting case Id");
                            console.warn("Error getting case Id");
                        }

                        if (language !== null) {
                            updateDataInRelatedCompensations(getCaseCompensations(caseId), { tc_language: language });
                        }
                    }
                },
                function (error) {
                    Xrm.Utility.alertDialog("Error getting customer cases");
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

        if (Xrm.Page.getAttribute(CASE_SOURCEMARKETID_ATTR_NAME) === null) {
            Xrm.Utility.alertDialog("Unable to find source market");
            console.warn("Unable to find source market");
            return;
        }

        var sourceMarketChanged = Xrm.Page.getAttribute(CASE_SOURCEMARKETID_ATTR_NAME).getIsDirty();

        if (sourceMarketChanged) {
            var caseId = formatEntityId(Xrm.Page.data.entity.getId());

            var sourceMarketId = formatEntityId(Xrm.Page.getAttribute(CASE_SOURCEMARKETID_ATTR_NAME).getValue()[0].id);

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
                    if (compensationsResponse.response === null || compensationsResponse.response === undefined) {
                        Xrm.Utility.alertDialog("Cases information can't be retrieved");
                        console.warn("Cases information can't be retrieved");
                        return;
                    }

                    try {
                        compensations = JSON.parse(compensationsResponse.response).value;
                    }
                    catch (e) {
                        Xrm.Utility.alertDialog("Cases information can't be parsed");
                        console.warn("Cases information can't be parsed");
                        return;
                    }
                }

                for (var i = 0; i < compensations.length; i++) {

                    var compensationId = compensations[i].tc_compensationid;
                    if (compensationId === null || compensationId === undefined) {
                        Xrm.Utility.alertDialog("Error getting compensation Id");
                        console.warn("Error getting compensation Id");
                        return;
                    }

                    if (Xrm.Page.context.client.getClient() === CLIENT_MODE_MOBILE && Xrm.Page.context.client.getClientState() === CLIENT_STATE_OFFLINE) {
                        //TODO
                    }
                    else {
                        updateCompensation(compensationId, compensationFields).then(
                            function (response) {
                            },
                            function (error) {
                                Xrm.Utility.alertDialog("Error updating compensation");
                                console.warn("Error updating compensation");
                            });
                    }
                }
            },
            function (error) {
                Xrm.Utility.alertDialog("Error getting compensations");
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
        if (id !== null) {
            id = id.replace("{", "").replace("}", "");
        }

        return id;
    }

    var compensationCalculation = function () {
        if (Xrm.Page.ui.getFormType() === FORM_MODE_CREATE) {
            Xrm.Utility.alertDialog("Please save the record to calculate the proposed compensation.");
            console.warn("Please save the record to calculate the proposed compensation.");
            return;
        }

    }

    function getMatrixValues()
    {
        
        return;
    }

    function retrieveCompensationLine(caseLineId)
    {
        var entityName = COMPENSATION_LINE_ENTITY;
        var query = "?$select=tc_categorylevel1id,tc_casecategory2id,tc_category3id";
        if (Xrm.Page.context.client.getClient() === CLIENT_MODE_MOBILE && Xrm.Page.context.client.getClientState() === CLIENT_STATE_OFFLINE) {
            // phones and tablets in offline mode
            return Xrm.Mobile.offline.retrieveRecord(entityName, caseLineId, query);
        }
        else {
            return Tc.Crm.Scripts.Common.GetById(entityName, caseLineId, query);
        }
    }

    return {
        OnLoad: function () {
            updateCompensationFields();
        },
        OnCustomerSave: function () {
            updateRelatedCompensationsForCustomer();
        },
        OnCaseSave: function () {
            updateSourceMarketInRelatedCompensationsForCase();
        },
        OnAccountSortCodeChanged: function (context) {
            if (context == null) {
                console.log("Tc.Crm.Scripts.Events.Compensation.OnAccountSortCodeChanged should be configured to pass execution context");
                return;
            }
            formatValue6DigitsWithHyphen(context);
        }
    };
})();