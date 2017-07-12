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
if (typeof (Tc.Crm.Uk) === "undefined") {
    Tc.Crm.Uk = {
        __namespace: true
    };
}
if (typeof (Tc.Crm.Uk.Scripts) === "undefined") {
    Tc.Crm.Uk.Scripts = {
        __namespace: true
    };
}
if (typeof (Tc.Crm.Uk.Scripts.Events) === "undefined") {
    Tc.Crm.Uk.Scripts.Events = {
        __namespace: true
    };
}

Tc.Crm.Uk.Scripts.Events.Contact = (function () {
    "use strict";

    function onPhoneChanged(context) {
        debugger;
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