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
Tc.Crm.Scripts.Events.Recovery = (function () {
    "use strict";

    var FORM_MODE_CREATE = 1;
    var HTTP_SUCCESS = 200;
    var STATUS_READY = 4;
    var NO_CONTENT = 204;
    var RETAIL_LEVEL2_ACCESS = "Tc.Uk.Retail.Level2";
    var RETAIL_LEVEL3_ACCESS = "Tc.Uk.Retail.Level3";
    var LIME_WEB_PAGE_URL = "http://event/?eventname=Tc.Event.OnClickLimeRibbonButton";
    var OWR_EVENT_URL = "http://event/?eventname=Tc.Event.OnOwrClick";
    var WEBRIO_WEB_PAGE_URL = "http://event/?eventname=Tc.Event.OnClickWebRioRibbonButton";
    var WEBRIO_USD_PARAM_CONSULTATION = "&ConsultationNo=";
    var WEBRIO_USD_PARAM_CUSTOMERID = "&CustomerId=";
    var WEBRIO_USD_PARAM_TPID = "&EntityId=";


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
            console.log("Error in retrieving ExternalLoginRecords");
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
            });;
    }

    return {

        OnNotifySupplierFieldChange: function () {
            validateSupplierEmailSpecified();
        }
    };
})();