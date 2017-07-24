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
        ContactChangeReason: "tc_keycontactchangereason",
        Telephone1: "telephone1",
        Telephone2: "telephone2",
        Telephone3: "telephone3",
    }
    var FORM_MODE_UPDATE = 2;
    function onPhoneChanged(context) {
        var attribute = context.getEventSource();
        Tc.Crm.Scripts.Utils.Validation.ValidatePhoneNumber(attribute.getName());
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

    var onLoad = function () {
        Tc.Crm.Scripts.Utils.Validation.ValidatePhoneNumber(Attributes.Telephone1);
        Tc.Crm.Scripts.Utils.Validation.ValidatePhoneNumber(Attributes.Telephone2);
        Tc.Crm.Scripts.Utils.Validation.ValidatePhoneNumber(Attributes.Telephone3);
    }
  
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
        OnLoad: onLoad,
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