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
Tc.Crm.Scripts.Events.Account = (function () {
    "use strict";
    // private stuff
    var Attributes = {
        Telephone1: "telephone1"
    }
    function onLoad() {
        Tc.Crm.Scripts.Utils.Validation.ValidatePhoneNumber(Attributes.Telephone1);
    }
    var onChangeTelephone1 = function () {
        Tc.Crm.Scripts.Utils.Validation.ValidatePhoneNumber(Attributes.Telephone1);
    }

    // public methods     
    return {

        OnLoad: function () {
            onLoad();
        },
        OnChangeTelephone1: function () {
            onChangeTelephone1();
        }
    };
})();
