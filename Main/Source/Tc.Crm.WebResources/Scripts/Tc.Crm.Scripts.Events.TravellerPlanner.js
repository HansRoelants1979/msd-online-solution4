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
Tc.Crm.Scripts.Events.TravellerPlanner = (function () {
    "use strict";

    var FORM_MODE_CREATE = 1;
    var HTTP_SUCCESS = 200;
    var STATUS_READY = 4;
    var NO_CONTENT = 204;
    var CLUSTER_MANAGER = "Tc.Retail.ClusterManager";
    var ASSISTANT_MANAGER = "Tc.Retail.AssistantManager";
    var REGIONAL_MANAGER = "Tc.Retail.RegionalManager";



    var Attributes = {
        Validation: "tc_validation",
        StateCode: "statecode",
        CustomerId: "customerid",
        ReviewDate: "tc_reviewdate",
        Surprise: "tc_surprise",
    }

    var EntitySetNames = {
        SecurityRole: "roles",
        ExternalLogins :"tc_externallogins",
    }
    var StateCode = {
        Open: 0,
    }
    var getUserRoles = function () {
        var enable = false;
        var UserRole = window.parent.Xrm.Page.context.getUserRoles();
        if (UserRole == null || UserRole == "" || UserRole == undefined) return enable;
        for (var i = 0; i < UserRole.length; i++) {
            var userRoleId = UserRole[i];            
            var results = syncGetSecurityRoles(userRoleId);
                    var name = results.value[0]["name"];
                    if (name == CLUSTER_MANAGER || name == ASSISTANT_MANAGER || name == REGIONAL_MANAGER) {
                        var ValidationAttr = getControlValue(Attributes.Validation);
                        if (ValidationAttr == null) return enable;
                        if (ValidationAttr == false && Xrm.Page.ui.getFormType() != FORM_MODE_CREATE) {
                            enable = true;
                        }
                    }           
            if (enable == true) break;
        }
        return enable;
    }
    var enableValidateRibbonButton = function () {
        var enable = getUserRoles();
        if (enable != null && enable != "" && enable != undefined)
            return enable;
    }
    var setValidationFieldValue = function () {

        Xrm.Page.getAttribute(Attributes.Validation).setValue(true);
        Xrm.Page.data.entity.save();


    }
    var setSurpriseFieldValue = function () {
        var surpriseFieldVale = getControlValue(Attributes.Surprise);
        if (surpriseFieldVale == null) return;
        if (surpriseFieldVale == false)
            Xrm.Page.getAttribute(Attributes.Surprise).setValue(true);
        else
            Xrm.Page.getAttribute(Attributes.Surprise).setValue(false);

    }
    var enableOWRorLimeButton = function () {
       
        var enable = false;
        if (window.IsUSD != true) return enable;
        if (Xrm.Page.ui.getFormType() == FORM_MODE_CREATE) return enable;
        if (Xrm.Page.getAttribute(Attributes.StateCode).getValue() != StateCode.Open) return enable;
        if (Xrm.Page.getAttribute(Attributes.CustomerId).getValue() == null) return enable;
        var ssoLoggined = getExternalLogin();
        if (ssoLoggined != false && ssoLoggined != undefined && ssoLoggined != "")
            return ssoLoggined;

    }
    var reviewDateOnChange = function () {

        Xrm.Page.getControl(Attributes.ReviewDate).clearNotification();
        var reviewDateValueAttr = getControlValue(Attributes.ReviewDate);
        var isPastDate = Tc.Crm.Scripts.Utils.Validation.IsPastDate(reviewDateValueAttr);
        if (isPastDate)
            Xrm.Page.getControl(Attributes.ReviewDate).setNotification("Review Date cannot be set in the past. Please choose a future date.");

    }
    function formatEntityId(id) {
        return id !== null ? id.replace("{", "").replace("}", "") : null;
    }
    var getControlValue = function (controlName) {
        if (Xrm.Page.getAttribute(controlName) && (Xrm.Page.getAttribute(controlName).getValue() != null))
            return Xrm.Page.getAttribute(controlName).getValue();
        else
            return null;
    }
    function getExternalLogin() {
        var extraLoginRecordExist = false;
        var loggedInUserId = null;
        loggedInUserId = Xrm.Page.context.getUserId();
        if (loggedInUserId == null || loggedInUserId == undefined || loggedInUserId == "") return extraLoginRecordExist;
        loggedInUserId = loggedInUserId.replace("{", "").replace("}", "");
        var results = syncGetExternalLoginRecords(loggedInUserId);
        var recordCount = results["@odata.count"];
        if (recordCount === 1)
            extraLoginRecordExist = true;    
        return extraLoginRecordExist;
    }
    var syncGetSecurityRoles = function (userRoleId) {
        try {
            var query = "?$select=name&$filter=roleid eq " + userRoleId;
            var roleNameResults = Tc.Crm.Scripts.Common.SyncGet(EntitySetNames.SecurityRole, query);
            return roleNameResults;
        } catch (e) {
            console.log("Error in retrieving SecurityRoles");
            return null;
        }
    }

    var syncGetExternalLoginRecords = function (loggedInUserId) {
        try {
            var query = "?$select=tc_abtanumber,tc_branchcode,tc_employeeid,tc_initials,tc_name&$filter=_ownerid_value eq " + loggedInUserId + "&$count=true";
            var externalLoginResults = Tc.Crm.Scripts.Common.SyncGet(EntitySetNames.ExternalLogins, query);
            return externalLoginResults;
        } catch (e) {
            console.log("Error in retrieving ExternalLoginRecords");
            return null;
        }
    }
    return {
        OnValidateRibbonButtonClick: function () {
            setValidationFieldValue();
        },
        EnableDisableValidateButton: function () {
            return enableValidateRibbonButton();
        },
        EnableDisableOWRorLimeButton: function () {
            return enableOWRorLimeButton();
        },
        OnSurpriseRibbonButtonClick: function () {
            setSurpriseFieldValue();
        },
        OnReviewDateFieldChange: function () {
            reviewDateOnChange();
        }

    };
})();