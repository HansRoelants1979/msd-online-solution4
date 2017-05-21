
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
Tc.Crm.Scripts.Events.Contact = ( function () {
    "use strict";
    // private stuff
    var FORM_MODE_CREATE = 1;
    var FORM_MODE_UPDATE = 2;
    var CLIENT_STATE_OFFLINE = "Offline";

    // public methods
    function validatePhoneNum(ExecutionContext, telephone1, telephone2) {

        var phone1 = Xrm.Page.data.entity.attributes.get(telephone1);
        if (phone1 != null) {
            var phone1Value = phone1.getValue();
            if (phone1Value != null && phone1Value != "") {
                GetNotificationFrPhNumVal(phone1Value, telephone1);
            }
        }
        var phone2 = Xrm.Page.data.entity.attributes.get(telephone2);
        if (phone2 != null) {
            var phone2Value = phone2.getValue();
            if (phone2Value != null && phone2Value != "") {
                GetNotificationFrPhNumVal(phone2Value, telephone2);
            }
        }
    }
    function GetNotificationFrPhNumVal(phone, telephoneFieldName) {
        var regex = /^\+(?:[0-9] ?){9,14}[0-9]$/;
        if (regex.test(phone)) {
            if (Xrm.Page.ui.getFormType() == FORM_MODE_CREATE) {
                Xrm.Page.getControl(telephoneFieldName).clearNotification();
            }
            if (Xrm.Page.ui.getFormType() == FORM_MODE_UPDATE) {
                Xrm.Page.ui.clearFormNotification("TelNumNotification");
            }
        }
        else {
            if (Xrm.Page.ui.getFormType() == FORM_MODE_CREATE) {
                Xrm.Page.getControl(telephoneFieldName).setNotification("telephone number is not valid");
            }
            if (Xrm.Page.ui.getFormType() == FORM_MODE_UPDATE) {
                Xrm.Page.getControl(telephoneFieldName).clearNotification();
                Xrm.Page.ui.setFormNotification("telephone number is not valid", "WARNING", "TelNumNotification");
            }
        }
    }
    return {
        ValidatePhoneNum: function (executioncontext, telephone1, telephone2) {
            validatePhoneNum(executioncontext, telephone1, telephone2);
        },
        OnLoad: function (executioncontext, telephone1, telephone2) {
            validatePhoneNum(executioncontext, telephone1, telephone2);
        },
        OnContactTelephoneFieldChange: function (executioncontext, telephone1, telephone2) {
            validatePhoneNum(executioncontext, telephone1, telephone2);
        },
        OnSave: function () {
            if (Xrm.Page.context.client.getClientState() !== CLIENT_STATE_OFFLINE) {
                Tc.Crm.Scripts.Library.Contact.UpdateCustomerCompensationsLanguage();
            }
        },
        OnChangeLanguage: function () {
            if (Xrm.Page.context.client.getClientState() === CLIENT_STATE_OFFLINE) {
                Tc.Crm.Scripts.Library.Contact.UpdateCustomerCompensationsLanguage();
            }
        }
    };
})();