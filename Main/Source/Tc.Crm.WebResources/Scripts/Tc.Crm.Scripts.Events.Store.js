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
Tc.Crm.Scripts.Events.Store = (function () {
    "use strict";

    var Attributes = {
        StoreClosed: "tc_storeclosed"
    }

    var getControlValue = function (controlName) {
        if (Xrm.Page.getAttribute(controlName))
            return Xrm.Page.getAttribute(controlName).getValue();
        else
            return null;
    }

    var showNewTravelPlanButton = function () {
        var storeClosed = getControlValue(Attributes.StoreClosed);
        return ((storeClosed != null && storeClosed == true) ? false : true);
    };

    return {       
        ShowNewTravelPlanButton: function () {
            return showNewTravelPlanButton();
        }

    };
})();