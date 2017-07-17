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

Tc.Crm.Scripts.Events.Contact.Retail = (function () {
    "use strict";

    function onPhoneChanged(context) {
        var attribute = context.getEventSource();
        var value = attribute.getValue();
        var isValid = true;
        if (value != null && value != '') {
            isValid = /^[0-9]{10,15}$/.test(value);
        }
        attribute.controls.forEach(
            function (control, i) {
                if (isValid) {
                    control.clearNotification('validatePhone');
                } else {
                    control.setNotification('Should be 10 to 15 digits only', 'validatePhone');
                }
            });
    }
 
    // public methods
    return {
        OnPhoneChanged: function (context) {
            if (context === null) {
                console.log("Tc.Crm.Uk.Scripts.Events.Contact.OnPhoneChanged should be configured to pass execution context");
                return;
            }
            onPhoneChanged(context);
        }
    };
})();