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
Tc.Crm.Scripts.Library.Contact = (function () {
    "use strict";
    var FORM_MODE_CREATE = 1;
    var FORM_MODE_UPDATE = 2;
    var FORM_MODE_QUICK_CREATE = 5;
    var CLIENT_STATE_OFFLINE = "Offline";

    var CUSTOMER_LANGUAGE_ATTR_NAME = "tc_language";


    var EntityNames = {
        Contact: "contact",
        Case: "incident",        
        Compensation: "tc_compensation"
    }

    var EntitySetNames = {
        Contact: "contacts",
        Case: "incidents",
        Compensation: "tc_compensations"
    }

    function formatEntityId(id) {
        return id !== null ? id.replace("{", "").replace("}", "") : null;
    }

    function IsOfflineMode() {
        return Xrm.Page.context.client.getClientState() === CLIENT_STATE_OFFLINE;
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
                return result.value != null ? result.value : result;
            }
            catch (e) {
                console.warn(entity + " information can't be parsed");
                return null;
            }
        }
    }

    function getCustomerCases(customerId) {
        if (!IsOfflineMode()) {
            var query = "?$filter=_customerid_value eq " + customerId + " &$select=incidentid";
            return Tc.Crm.Scripts.Common.Get(EntitySetNames.Case, query);
        }
        else {
            var query = "?$filter=customerid eq " + customerId + " &$select=incidentid";
            return Xrm.Mobile.offline.retrieveMultipleRecords(EntityNames.Case, query);
        }
    }

    function getCaseCompensations(caseId) {
        if (!IsOfflineMode()) {
            var query = "?$filter=_tc_caseid_value eq " + caseId + " &$select=tc_compensationid";
            return Tc.Crm.Scripts.Common.Get(EntitySetNames.Compensation, query);
        }
        else {
            var query = "?$filter=tc_caseid eq " + caseId + " &$select=tc_compensationid";
            return Xrm.Mobile.offline.retrieveMultipleRecords(EntityNames.Compensation, query);
        }
    }


    function updateCompensations(compensationsReceivedPromise, compensationFields) {
        compensationsReceivedPromise.then(
            function (compensationsResponse) {
                var compensations = getPromiseResponse(compensationsResponse, "Compensation");
                if (compensations == null) return;
                compensations.forEach(function (compensation) {
                    if (IsOfflineMode()) {
                        Xrm.Mobile.offline.updateRecord(EntityNames.Compensation, compensation.tc_compensationid, compensationFields).then(
                            function () { },
                            function (error) {
                                console.warn("Error updating compensation");
                            });
                    }
                    else {
                        Tc.Crm.Scripts.Common.Update(EntitySetNames.Compensation, compensation.tc_compensationid, compensationFields).then(
                            function () { },
                            function (error) {
                                console.warn("Error updating compensation");
                            });
                    }
                });
            },
            function (error) {
                console.warn("Error getting compensations");
            }
        )
    }

    /// Public implementation
    var updateCustomerCompensationsLanguage = function () {
        if (Xrm.Page.ui.getFormType() === FORM_MODE_CREATE || Xrm.Page.ui.getFormType() === FORM_MODE_QUICK_CREATE)
            return;
        var attrLanguage = Xrm.Page.getAttribute(CUSTOMER_LANGUAGE_ATTR_NAME);
        if (attrLanguage === null || !attrLanguage.getIsDirty())
            return;
        var language = attrLanguage.getValue();
        var customerId = formatEntityId(Xrm.Page.data.entity.getId());
        getCustomerCases(customerId).then(
            function (casesResponse) {
                var cases = getPromiseResponse(casesResponse, "Case");
                if (cases == null) return;
                cases.forEach(function (incident) {
                    var caseCompensationsPromise = getCaseCompensations(incident.incidentid);
                    updateCompensations(caseCompensationsPromise, { tc_language: language });
                });
            },
            function (error) {
                console.warn("Error getting customer cases");
            }
        );
    }
    var getNotificationForPhoneNumber = function(telephoneFieldName) {
        var phone = Xrm.Page.data.entity.attributes.get(telephoneFieldName);
        if (phone == null) return;
        var phoneValue = phone.getValue();
        if (phoneValue == null || phoneValue == "") return;

        var regex = /^\+(?:[0-9] ?){9,14}[0-9]$/;
        if (regex.test(phoneValue)) {
            if (Xrm.Page.ui.getFormType() == FORM_MODE_CREATE) {
                Xrm.Page.getControl(telephoneFieldName).clearNotification();
            }
            if (Xrm.Page.ui.getFormType() == FORM_MODE_UPDATE) {
                Xrm.Page.ui.clearFormNotification("TelNumNotification");
            }
        }
        else {
            if (Xrm.Page.ui.getFormType() == FORM_MODE_CREATE) {
                Xrm.Page.getControl(telephoneFieldName).setNotification("The telephone number does not match the required format. The number should start with a + followed by the country dialing code and contain no spaces or other special characters i.e. +44 for UK.");
            }
            if (Xrm.Page.ui.getFormType() == FORM_MODE_UPDATE) {
                Xrm.Page.getControl(telephoneFieldName).clearNotification();
                Xrm.Page.ui.setFormNotification("The telephone number does not match the required format. The number should start with a + followed by the country dialing code and contain no spaces or other special characters i.e. +44 for UK.", "WARNING", "TelNumNotification");
            }
        }
    }
    // public
    return {
        UpdateCustomerCompensationsLanguage: updateCustomerCompensationsLanguage,
        GetNotificationForPhoneNumber: getNotificationForPhoneNumber
    };
})();