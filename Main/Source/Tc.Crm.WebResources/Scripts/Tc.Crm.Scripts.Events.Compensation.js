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
Tc.Crm.Scripts.Events.Compensation = (function () {
    "use strict";

    var formatValue6DigitsWithHyphen = function (context) {
        var formatted = false;
        var attribute = context.getEventSource();
        var value = attribute.getValue();
        if (/^\d{6}$/.test(value)) {
            var formattedValue = value[0] + value[1] + '-' + value[2] + value[3] + '-' + value[4] + value[5];
            attribute.setValue(formattedValue);
            formatted = true;
        }
        var isValid = formatted || /^\d{2}-\d{2}-\d{2}$/.test(value);
        attribute.controls.forEach(
            function (control, i) {
                if (isValid) {
                    control.clearNotification();
                } else {
                    control.setNotification("Should be 6 digit format nn-nn-nn");
                }
            });
    }

    return {
        OnLoad: function () {
            Tc.Crm.Scripts.Library.Compensation.SetDefaultsOnCreate();
        },
        OnAccountSortCodeChanged: function (context) {
            if (context == null) {
                console.log("Tc.Crm.Scripts.Events.Compensation.OnAccountSortCodeChanged should be configured to pass execution context");
                return;
            }
            formatValue6DigitsWithHyphen(context);
        }
    };
})();