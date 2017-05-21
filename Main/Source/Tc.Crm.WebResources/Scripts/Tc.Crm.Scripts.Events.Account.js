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
    function validateAccountPhoneNum(ExecutionContext, telephone1, telephone2) {
        Tc.Crm.Scripts.Events.Contact.ValidatePhoneNum(ExecutionContext, telephone1, telephone2);
    }

    // public methods     
    return {

        OnLoad: function (executioncontext, telephone1, telephone2) {
            debugger;
            validateAccountPhoneNum(executioncontext, telephone1, telephone2);
        },
        OnAccountTelephoneFieldChange: function (executioncontext, telephone1, telephone2) {
            debugger;
            validateAccountPhoneNum(executioncontext, telephone1, telephone2);
        }

    };
})();// JavaScript source code
