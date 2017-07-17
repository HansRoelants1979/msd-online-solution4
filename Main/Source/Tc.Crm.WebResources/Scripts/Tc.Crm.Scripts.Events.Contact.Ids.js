
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
if (typeof (Tc.Crm.Scripts.Events.Contact) === "undefined") {
    Tc.Crm.Scripts.Events.Contact = {
        __namespace: true
    };
}

Tc.Crm.Scripts.Events.Contact.Ids = (function () {
    "use strict";
    // private stuff
    var FORM_MODE_CREATE = 1;
    var FORM_MODE_UPDATE = 2;
    var CLIENT_STATE_OFFLINE = "Offline";

    // public methods
    function onLoad() {

        Tc.Crm.Scripts.Library.Contact.GetNotificationForPhoneNumber("telephone1");
        Tc.Crm.Scripts.Library.Contact.GetNotificationForPhoneNumber("telephone2");
    }
    var onChangeTelephone1 = function () {
        Tc.Crm.Scripts.Library.Contact.GetNotificationForPhoneNumber("telephone1");
    }
    var onChangeTelephone2 = function () {
        Tc.Crm.Scripts.Library.Contact.GetNotificationForPhoneNumber("telephone2");
    }
    var onChangeTelephone3 = function () {
        Tc.Crm.Scripts.Library.Contact.GetNotificationForPhoneNumber("telephone3");
    }
    return {
       
        OnLoad: function () {
            onLoad();
        },
        OnChangeTelephone1:function(){
            onChangeTelephone1();
        },
        OnChangeTelephone2: function () {
            onChangeTelephone2();
        },
        OnChangeTelephone3: function () {
            onChangeTelephone3();
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