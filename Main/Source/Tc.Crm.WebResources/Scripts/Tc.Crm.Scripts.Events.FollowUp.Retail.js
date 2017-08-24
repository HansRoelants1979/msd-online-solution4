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

if (typeof (Tc.Crm.Scripts.Events.FollowUp) === "undefined") {
    Tc.Crm.Scripts.Events.FollowUp = {
        __namespace: true
    };
}

Tc.Crm.Scripts.Events.FollowUp.Retail = (function () {
    "use strict";
    var CUSTOMER_QUICK_VIEW_FORM = "AvailableContactDetails";
    var Attributes = {
        DueDate: "scheduledend",
        ContactPhone: "tc_contactphonenumber",
        ContactEmail: "tc_contactemail",
        ContactMethod: "tc_contactmethod",
        Telephone1: "telephone1",
        Telephone2: "telephone2",
        Telephone3: "telephone3",
        Emailaddress1: "emailaddress1",
        Emailaddress2: "emailaddress2",
        Emailaddress3: "emailaddress3",
    }
    var dueDateOnChange = function () {

        Xrm.Page.getControl(Attributes.DueDate).clearNotification();
        var dueDateValueAttr = getControlValue(Attributes.DueDate);
        var isPastDate = Tc.Crm.Scripts.Utils.Validation.IsPastDate(dueDateValueAttr);
        if (isPastDate)
            Xrm.Page.getControl(Attributes.DueDate).setNotification("Due Date cannot be set in the past. Please choose a future date.");

    }
    var contactMethodOnChange = function () {
        Xrm.Page.getControl(Attributes.ContactPhone).clearNotification();
        Xrm.Page.getControl(Attributes.ContactEmail).clearNotification();

    }
    var contactPhoneOrContactEmailOnChange = function (executionContext) {

        if (executionContext == null || executionContext == undefined) return;
        var attribute = executionContext.getEventSource();
        if (attribute == null || attribute == undefined) return;
        var fieldName = attribute.getName();
        if (fieldName == null || fieldName == undefined) return;
        var contactPhoneOrConatctEmailAttr = getControlValue(fieldName);
        if (contactPhoneOrConatctEmailAttr == null || contactPhoneOrConatctEmailAttr == 950000003) {
            Xrm.Page.getControl(Attributes.ContactPhone).clearNotification();
            Xrm.Page.getControl(Attributes.ContactEmail).clearNotification();
            return;
        }
        var contactMethodAttr = getControlValue(Attributes.ContactMethod);
        if (contactMethodAttr == null || contactMethodAttr == 950000002 || contactMethodAttr == 950000003) return;
        if (contactMethodAttr == 950000000) {
            Xrm.Page.getControl(Attributes.ContactEmail).clearNotification();
            var phoneValue = getAttributeValue(getPhoneFieldName(contactPhoneOrConatctEmailAttr));
            if (phoneValue == null || phoneValue == "" || phoneValue == undefined) {
                Xrm.Page.getControl(Attributes.ContactPhone).clearNotification();
                Xrm.Page.getControl(Attributes.ContactPhone).setNotification("The chosen " + Xrm.Page.getAttribute(Attributes.ContactPhone).getText() + " option is empty and does not contain a phone number. Please choose another phone option or update the " + getPhoneText(Xrm.Page.getAttribute(Attributes.ContactPhone).getText()) + " field within the customer record before choosing this option.");
            }
            else {
                Xrm.Page.getControl(Attributes.ContactPhone).clearNotification();
            }
        }
        else {
            Xrm.Page.getControl(Attributes.ContactPhone).clearNotification();
            var emailValue = getAttributeValue(getEmailFieldName(contactPhoneOrConatctEmailAttr));
            if (emailValue == null || emailValue == "" || emailValue == undefined) {
                Xrm.Page.getControl(Attributes.ContactEmail).clearNotification();
                Xrm.Page.getControl(Attributes.ContactEmail).setNotification("The chosen " + Xrm.Page.getAttribute(Attributes.ContactEmail).getText() + " option is empty and does not contain a Email Id. Please choose another Email option or update the " + Xrm.Page.getAttribute(Attributes.ContactEmail).getText() + " field within the customer record before choosing this option.");
            }
            else {
                Xrm.Page.getControl(Attributes.ContactEmail).clearNotification();
            }

        }

    }
    var getAttributeValue = function (attributeSchemaName) {
        var customerDeatilsQuickViewControl = Xrm.Page.ui.quickForms.get(CUSTOMER_QUICK_VIEW_FORM);
        if (customerDeatilsQuickViewControl != undefined) {
            if (customerDeatilsQuickViewControl.isLoaded()) {
                // Access the value of the attribute bound to the constituent control
                var Value = customerDeatilsQuickViewControl.getControl(attributeSchemaName).getAttribute().getValue();
                console.log(Value);
                return Value;
            }
            else {
                // Wait for some time and check again
                setTimeout(getAttributeValue, 10);
            }
        }
        else {
            console.log("No data to display in the quick view control.");
            return;
        }
    }
    var getPhoneFieldName = function (contactPhoneFieldName) {
        var phoneFieldSchemaName;

        switch (contactPhoneFieldName) {
            case 950000000:
                phoneFieldSchemaName = Attributes.Telephone1;
                break;
            case 950000001:
                phoneFieldSchemaName = Attributes.Telephone2;
                break;
            case 950000002:
                phoneFieldSchemaName = Attributes.Telephone3;
                break;
            default:
                phoneFieldSchemaName = null;
        }
        return phoneFieldSchemaName;
    }
    var getEmailFieldName = function (contactEmailFieldName) {
        var emailFieldSchemaName;
        switch (contactEmailFieldName) {
            case 950000000:
                emailFieldSchemaName = Attributes.Emailaddress1;
                break;
            case 950000001:
                emailFieldSchemaName = Attributes.Emailaddress2;
                break;
            case 950000002:
                emailFieldSchemaName = Attributes.Emailaddress3;
                break;
            default:
                emailFieldSchemaName = null;
        }
        return emailFieldSchemaName;
    }
    var getPhoneText = function (phoneName) {
        var phoneTextValue;
        switch (phoneName) {
            case "Phone 1":
                phoneTextValue = "Telephone 1";
                break;
            case "Phone 2":
                phoneTextValue = "Telephone 2";
                break;
            case "Phone 3":
                phoneTextValue = "Telephone 3";
                break;
            default:
                phoneTextValue = "Telephone";
        }
        return phoneTextValue;
    }
    var getControlValue = function (controlName) {
        if (Xrm.Page.getAttribute(controlName) && (Xrm.Page.getAttribute(controlName).getValue() != null))
            return Xrm.Page.getAttribute(controlName).getValue();
        else
            return null;
    }

    var setRegardingField = function () {

        // Get the Value of the Regarding through the traveller Planner Parameters

        var param = Xrm.Page.context.getQueryStringParameters();

        var regardingId = param["parameter_regardingid"];
        var regardingName = param["parameter_regardingname"];
        var regardingType = param["parameter_regardingtype"];

        //Populate the Regarding 

        if (regardingId != null && regardingId != undefined)

        { Xrm.Page.getAttribute("regardingobjectid").setValue([{ id: regardingId, name: regardingName, entityType: regardingType }]); }

    }

    // public methods
    return {
        OnDueDateFieldChange: function () {
            dueDateOnChange();
        },
        OnLoad: function () {
            setRegardingField();
        },
        OnContactPhoneOrContactEmailChange: function (executionContext) {
            contactPhoneOrContactEmailOnChange(executionContext);
        },
        OnContactMethodChange: function () {
            contactMethodOnChange();
        }

    };
})();