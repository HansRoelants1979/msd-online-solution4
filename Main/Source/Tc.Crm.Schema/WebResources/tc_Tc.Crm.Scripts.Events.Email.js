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
Tc.Crm.Scripts.Events.Email = ( function () {
    "use strict";
    var FORM_MODE_CREATE = 1;
    //put your private stuff here
    var clearEmailFrom = function () {
        if (Xrm.Page.ui.getFormType() === FORM_MODE_CREATE) {
            Xrm.Page.getAttribute("from").setValue(null);
        }
    }
    return {
        OnLoad: function () {
            clearEmailFrom();
        }
    };
})();