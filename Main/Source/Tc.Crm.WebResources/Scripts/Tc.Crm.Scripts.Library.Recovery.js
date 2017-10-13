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
if (typeof (Tc.Crm.Scripts.Library) === "undefined") {
    Tc.Crm.Scripts.Library = {
        __namespace: true
    };
}
Tc.Crm.Scripts.Library.Recovery = (function () {
    "use strict";

    var Attributes = {
        CaseLineId: "tc_caselineid",
        NotifySupplier: "tc_notifysupplier",
        SupplierEmailAddress: "emailaddress"
    }
    var EntitySetNames = {
        CaseLine: "tc_caselines",
    }
    var EntityName = {
        CaseLine: "tc_caseline",
    }
    var Messages = {
        SupplierContactNotPresent: "There are no contact details saved for the supplier of the related case line, please add"
    }
    var NotificationType = {
        Warning : "WARNING"
    }
    var NotificationId = {
        SupplierEmailNotification: "SupplierEmailNotification"
    }

    var getControlValue = function (controlName) {
        if (Xrm.Page.getAttribute(controlName) && (Xrm.Page.getAttribute(controlName).getValue() != null))
            return Xrm.Page.getAttribute(controlName).getValue();
        else
            return null;
    }

    var getCaseLineSupplierEmail = function (caseLineId) {
        try {
            var query = "?$select=emailaddress";
            var result = Tc.Crm.Scripts.Common.GetById(EntitySetNames.CaseLine, caseLineId, query);
            return result;
        } catch (e) {
            console.log("Error in retrieving caseline record.");
            return null;
        }
    }

    var formatEntityId = function(id) {
        return id !== null ? id.replace("{", "").replace("}", "") : null;
    }

    var validateSupplierEmailSpecified = function () {
        var notifySupplier = getControlValue(Attributes.NotifySupplier);
        if (notifySupplier == null || notifySupplier == false)
            return;
        var caseLineObj = getControlValue(Attributes.CaseLineId);
        if (caseLineObj == null) {
            Xrm.Page.ui.clearFormNotification(NotificationId.SupplierEmailNotification);
            Xrm.Page.ui.setFormNotification(Messages.SupplierContactNotPresent, NotificationType.Warning, NotificationId.SupplierEmailNotification);
            Xrm.Page.getAttribute(Attributes.NotifySupplier).setValue(false);
            return;
        }
        var caseLineId = formatEntityId(caseLineObj[0].id);
        
        getCaseLineSupplierEmail(caseLineId).then(
            function (caseLineResponse) {
                var caseLine = JSON.parse(caseLineResponse.response);
                var emailAddress = caseLine.emailaddress;
                if (emailAddress == null || emailAddress == undefined || emailAddress == "")
                {
                    Xrm.Page.ui.clearFormNotification(NotificationId.SupplierEmailNotification);
                    Xrm.Page.ui.setFormNotification(Messages.SupplierContactNotPresent, NotificationType.Warning, NotificationId.SupplierEmailNotification);
                    Xrm.Page.getAttribute(Attributes.NotifySupplier).setValue(false);
                    return;
                }

            }
            ).catch(function (err) {
                throw new Error("Problem in retrieving the supplier email address from case line.");
            });
    }

    return {
        ValidateSupplierEmailSpecified: validateSupplierEmailSpecified
};
})();