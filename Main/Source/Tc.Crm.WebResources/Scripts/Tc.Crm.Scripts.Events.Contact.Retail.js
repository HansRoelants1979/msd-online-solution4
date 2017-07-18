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
    var Attributes = {
        Salutation: "tc_salutation",
        FirstName: "firstname",
        LastName: "lastname",
        BirthDate: "birthdate",      
        ContactChangeReason: "tc_keycontactchangereason"
    }
    var FORM_MODE_UPDATE = 2;
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

    var onCustomerKeyInformationUpdate = function () {
        if (isCustomerKeyInformationUpdated())
            showContactChangeReason();
        else
            hideContactChangeReason();
    };

    var hideContactChangeReason = function () {
        if (Xrm.Page.getAttribute(Attributes.ContactChangeReason)) {
            Xrm.Page.getAttribute(Attributes.ContactChangeReason).setRequiredLevel("none");
            Xrm.Page.getControl(Attributes.ContactChangeReason).setVisible(false);
        }
    };

    var showContactChangeReason = function () {
        if (isControlVisible(Attributes.ContactChangeReason))
            return;
        if (Xrm.Page.getAttribute(Attributes.ContactChangeReason)) {
            Xrm.Page.getAttribute(Attributes.ContactChangeReason).setValue(null);
            Xrm.Page.getControl(Attributes.ContactChangeReason).setVisible(true);
            Xrm.Page.getAttribute(Attributes.ContactChangeReason).setRequiredLevel("required");
        }
    };

    var isCustomerKeyInformationUpdated = function () {
        if (isFormTypeUpdate())
        {
            var titleUpdated = getIsDirty(Attributes.Salutation);
            var firstNameUpdated = getIsDirty(Attributes.FirstName);
            var lastNameUpdated = getIsDirty(Attributes.LastName);
            var dateOfBirthUpdated = getIsDirty(Attributes.BirthDate);
            if (titleUpdated || firstNameUpdated || lastNameUpdated || dateOfBirthUpdated)
                return true;
        }
        return false;
    };

  
    var onSave = function (econtext) {
        if (isCustomerKeyInformationUpdated()) {
            var eventArgs = econtext.getEventArgs();
            if (eventArgs.getSaveMode() != 70)//AutoSave
            {
                if (confirm("Is this a new customer?"))
                    yesCloseCallback(econtext);
                else
                    noCloseCallback();
            }
            else {
                eventArgs.preventDefault();
            }
        }
    };

    var yesCloseCallback = function (econtext) {
        Xrm.Utility.alertDialog("Please create a new customer record");
        var eventArgs = econtext.getEventArgs();
        eventArgs.preventDefault();
    };

    var noCloseCallback = function () {
        hideContactChangeReason();     
    };
    

    var isFormTypeUpdate = function () {
        var formType = Xrm.Page.ui.getFormType();
        if (formType == FORM_MODE_UPDATE)
            return true;
        else
            return false;
    };
    
    var getIsDirty = function (controlName) {
        if (Xrm.Page.getAttribute(controlName))
            return Xrm.Page.getAttribute(controlName).getIsDirty()
        else
            return false;
    };

    var isControlVisible = function (controlName) {
        if (Xrm.Page.getControl(controlName))
          return Xrm.Page.getControl(controlName).getVisible();
        else
            return false;
    };


    // public methods
    return {
        OnSave: function (econtext) {
            onSave(econtext);
        },
        OnPhoneChanged: function (context) {
            if (context === null) {
                console.log("Tc.Crm.Uk.Scripts.Events.Contact.OnPhoneChanged should be configured to pass execution context");
                return;
            }
            onPhoneChanged(context);
        },
        OnCustomerKeyInformationUpdate: function () {
            onCustomerKeyInformationUpdate();
        }
    };
})();