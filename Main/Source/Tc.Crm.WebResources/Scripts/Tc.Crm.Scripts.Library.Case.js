var scriptLoader = scriptLoader || {
    delayedLoads: [],
    load: function (name, requires, script) {
        window._loadedScripts = window._loadedScripts || {};
        // Check for loaded scripts, if not all loaded then register delayed Load
        if (requires == null || requires.length == 0 || scriptLoader.areLoaded(requires)) {
            scriptLoader.runScript(name, script);
        }
        else {
            // Register an onload check
            scriptLoader.delayedLoads.push({ name: name, requires: requires, script: script });
        }
    },
    runScript: function (name, script) {
        script.call(window);
        window._loadedScripts[name] = true;
        scriptLoader.onScriptLoaded(name);
    },
    onScriptLoaded: function (name) {
        // Check for any registered delayed Loads
        scriptLoader.delayedLoads.forEach(function (script) {
            if (script.loaded == null && scriptLoader.areLoaded(script.requires)) {
                script.loaded = true;
                scriptLoader.runScript(script.name, script.script);
            }
        });
    },
    areLoaded: function (requires) {
        var allLoaded = true;
        for (var i = 0; i < requires.length; i++) {
            allLoaded = allLoaded && (window._loadedScripts[requires[i]] != null);
            if (!allLoaded)
                break;
        }
        return allLoaded;
    }
};
scriptLoader.load("Tc.Crm.Scripts.Library.Case", ["Tc.Crm.Scripts.Common"], function () {
// start script

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
Tc.Crm.Scripts.Library.Case = (function () {
    "use strict";
    var FORM_MODE_CREATE = 1;
    var FORM_MODE_QUICK_CREATE = 5;
    var CLIENT_STATE_OFFLINE = "Offline";

    var CASE_SOURCEMARKETID_ATTR_NAME = "tc_sourcemarketid";
    var COMPENSATION_SOURCEMARKETID_FIELDNAME = "tc_SourceMarketId";
    var COMPENSATION_SORCE_MARKET_NAME = "tc_sourcemarketid";

    var EntityNames = {
        Case: "incident",        
        Compensation: "tc_compensation",
        Country: "tc_country"
    }

    var EntitySetNames = {
        Case: "incidents",
        Compensation: "tc_compensations",
        Country: "tc_countries"
    }

    function formatEntityId(id) {
        return id !== null ? id.replace("{", "").replace("}", "") : null;
    }

    function getPromiseResponse(promiseResponse, entity) {
        if (promiseResponse == null) return;
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

    function IsOfflineMode() {
        return Xrm.Page.context.client.getClientState() === CLIENT_STATE_OFFLINE;
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

    var updateRelatedCompensationsSourceMarket = function () {
        if (Xrm.Page.ui.getFormType() === FORM_MODE_CREATE || Xrm.Page.ui.getFormType() === FORM_MODE_QUICK_CREATE)
            return;
        var attribute = Xrm.Page.getAttribute(CASE_SOURCEMARKETID_ATTR_NAME);
        if (attribute === null || !attribute.getIsDirty())
            return;
        var caseId = formatEntityId(Xrm.Page.data.entity.getId());
        var sourceMarketId = formatEntityId(attribute.getValue()[0].id);
        var compensation = {};
        if (!IsOfflineMode()) {
            compensation[COMPENSATION_SOURCEMARKETID_FIELDNAME + "@odata.bind"] = "/" + EntitySetNames.Country + "(" + sourceMarketId + ")";
        } else {
            compensation[COMPENSATION_SORCE_MARKET_NAME] = {
                "logicalname": EntityNames.Country,
                "id": sourceMarketId
            };
        }
        updateCompensations(getCaseCompensations(caseId), compensation);
    }

    // public
    return {
        UpdateRelatedCompensationsSourceMarket: updateRelatedCompensationsSourceMarket
    };
})();

// end script
console.log('loaded library.case');
});