var scriptLoader = scriptLoader || {
    delayedLoads : [],
    load: function (name, requires, script) {
        window._loadedScripts = window._loadedScripts || { };
        // Check for loaded scripts, if not all loaded then register delayed Load
        if (requires == null || requires.length == 0 || scriptLoader.areLoaded(requires)) {
            scriptLoader.runScript(name, script);
        }
        else {
            // Register an onload check
            scriptLoader.delayedLoads.push({ name : name, requires: requires, script : script });
        }
    },
    runScript: function (name, script) {
        script.call(window);
        window._loadedScripts[name]= true;
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
            allLoaded = allLoaded && (window._loadedScripts[requires[i]]!= null);
            if (!allLoaded)
                break;
        }
        return allLoaded;
    }
};
scriptLoader.load("Tc.Crm.Scripts.Utils.Validation", ["Tc.Crm.Scripts.Common"], function () {
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
    var CLIENT_STATE_OFFLINE = "Offline";
    var FormMode = {
        Create: 1,
        Update: 2
    }
    var EntitySetNames = {
        Configuration: "tc_configurations"
    }
    var Configuration = {
        CreditCardPattern: 'Tc.Validation.GDPR.CreditCardPattern'
    }
    var Messages = {
        InvalidPhone: "The telephone number does not match the required format. The number should start with a + followed by the country dialing code and contain no spaces or other special characters i.e. +44 for UK.",
        HasCreditCard: "Data you entered contain potentional credit card number(16 streight digits). Field: "
    }

    // retrieve configuration value
    var syncGetConfigurationValue = function (configName) {
        try {
            var query = "?$filter=tc_name eq '" + configName + "' &$select=tc_value";
            var configuration = Tc.Crm.Scripts.Common.SyncGet(EntitySetNames.Configuration, query);
            return configuration.value[0].tc_value;
        } catch (e) {
            console.log("Error retrieving configuration value " + configName);
            return null;
        }
    }

    function IsOfflineMode() {
        return Xrm.Page.context.client.getClientState() === CLIENT_STATE_OFFLINE
    }

    var validatePhoneNumber = function (telephoneFieldName) {
        var phone = Xrm.Page.data.entity.attributes.get(telephoneFieldName);
        if (phone == null) return;
        var phoneValue = phone.getValue();
        if (phoneValue == null || phoneValue == "") return;
        var regex = /^\+(?:[0-9] ?){9,14}[0-9]$/;
        if (regex.test(phoneValue)) {
            if (Xrm.Page.ui.getFormType() == FormMode.Create) {
                phone.controls.forEach(function (control, i) { control.clearNotification(); });
            }
            if (Xrm.Page.ui.getFormType() == FormMode.Update) {
                Xrm.Page.ui.clearFormNotification("TelNumNotification");
            }
        }
        else {
            if (Xrm.Page.ui.getFormType() == FormMode.Create) {
                phone.controls.forEach(function (control, i) { control.setNotification(Messages.InvalidPhone)});
            }
            if (Xrm.Page.ui.getFormType() == FormMode.Update) {
                phone.controls.forEach(function (control, i) { control.clearNotification(); });
                Xrm.Page.ui.setFormNotification(Messages.InvalidPhone, "WARNING", "TelNumNotification");
            }
        }
    }

    var validateGdprCompliance = function(context)
    {
        // don't validate in offline mode
        if (IsOfflineMode()) return;        
        // requires context
        if (context === null) {
            console.log("Tc.Crm.Scripts.Utils.Validation.validateGdprCompliance requires OnSave context");
            return;
        }
        Xrm.Page.ui.clearFormNotification('GDPR_Compliance');
        // validate
        var patternValue = syncGetConfigurationValue(Configuration.CreditCardPattern);
        if (patternValue == null) {
            return;
        }
        var pattern = new RegExp(patternValue);
        var isValid = true;
        Xrm.Page.data.entity.attributes.forEach(function (attribute) {
            if (isValid && attribute.getIsDirty()) {
                if ('string' === attribute.getAttributeType() || 'memo' === attribute.getAttributeType()) {
                    var hasCreditCard = pattern.test(attribute.getValue());
                    if (hasCreditCard) {
                        isValid = false;
                        var label;
                        if (attribute.controls.getLength() > 0) {
                            label = attribute.controls.get(0).getLabel();
                        } else {
                            label = attribute.getName();
                        }
                        Xrm.Page.ui.setFormNotification(Messages.HasCreditCard + label, 'WARNING', 'GDPR_Compliance');
                        context.getEventArgs().preventDefault();                        
                    }
                }
            }
        });
    }

    var isPastDate = function (date)
    {
        if (date == null || date == undefined || date == "") return false;
        var today = new Date();
        today.setHours(0, 0, 0, 0);
        date.setHours(0, 0, 0, 0);
        return (date < today);
       
    }

    // public
    return {
        ValidatePhoneNumber: validatePhoneNumber,
        IsPastDate: isPastDate,
        ValidateGdprCompliance: validateGdprCompliance
    };
})();

// end script
console.log('loaded utils.validation');
});