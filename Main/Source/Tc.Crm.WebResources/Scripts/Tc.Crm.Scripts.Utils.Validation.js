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
scriptLoader.load("Tc.Crm.Scripts.Utils.Validation", null, function () {

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
if (typeof (Tc.Crm.Scripts.Utils) === "undefined") {
    Tc.Crm.Scripts.Utils = {
        __namespace: true
    };
}
Tc.Crm.Scripts.Utils.Validation = (function () {
    "use strict";
    var FORM_MODE_CREATE = 1;
    var FORM_MODE_UPDATE = 2;

    var Messages = {
        InvalidPhone: "The telephone number does not match the required format. The number should start with a + followed by the country dialing code and contain no spaces or other special characters i.e. +44 for UK."
    }

    var validatePhoneNumber = function (telephoneFieldName) {
        var phone = Xrm.Page.data.entity.attributes.get(telephoneFieldName);
        if (phone == null) return;
        var phoneValue = phone.getValue();
        if (phoneValue == null || phoneValue == "") return;
        var regex = /^\+(?:[0-9] ?){9,14}[0-9]$/;
        if (regex.test(phoneValue)) {
            if (Xrm.Page.ui.getFormType() == FORM_MODE_CREATE) {
                phone.controls.forEach(function (control, i) { control.clearNotification(); });
            }
            if (Xrm.Page.ui.getFormType() == FORM_MODE_UPDATE) {
                Xrm.Page.ui.clearFormNotification("TelNumNotification");
            }
        }
        else {
            if (Xrm.Page.ui.getFormType() == FORM_MODE_CREATE) {
                phone.controls.forEach(function (control, i) { control.setNotification(Messages.InvalidPhone)});
            }
            if (Xrm.Page.ui.getFormType() == FORM_MODE_UPDATE) {
                phone.controls.forEach(function (control, i) { control.clearNotification(); });
                Xrm.Page.ui.setFormNotification(Messages.InvalidPhone, "WARNING", "TelNumNotification");
            }
        }
    }

    // public
    return {
        ValidatePhoneNumber: validatePhoneNumber
    };
})();

// end script
console.log('loaded utils.validation');
});