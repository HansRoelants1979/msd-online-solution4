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
    var RETAIL_LEVEL2_ACCESS = "Tc.Uk.Retail.Level2";
    var RETAIL_LEVEL3_ACCESS = "Tc.Uk.Retail.Level3";
    var LIME_WEB_PAGE_URL = "http://event/?eventname=Tc.Event.OnClickLimeRibbonButton";
    var OWR_EVENT_URL = "http://event/?eventname=Tc.Event.OnOwrClick";
    var WEBRIO_WEB_PAGE_URL = "http://event/?eventname=Tc.Event.OnClickWebRioRibbonButton";
    



    var Attributes = {
        Validation: "tc_validation",
        StateCode: "statecode",
        CustomerId: "customerid",
        ReviewDate: "tc_reviewdate",
        Surprise: "tc_surprise",
        Name: "name",
    }

    var EntitySetNames = {
        SecurityRole: "roles",
        ExternalLogins: "tc_externallogins",
    }
    var EntityName = {
        TravellerPlanner: "opportunity",
        FollowUp: "tc_followup",
    }
    var StateCode = {
        Open: 0,
        Won: 1,
        Lost: 2,
    }
    var getUserRoles = function () {
        var enable = false;
        var UserRole = window.parent.Xrm.Page.context.getUserRoles();
        if (UserRole == null || UserRole == "" || UserRole == undefined) return enable;
        for (var i = 0; i < UserRole.length; i++) {
            var userRoleId = UserRole[i];
            var results = syncGetSecurityRoles(userRoleId);
            var name = results.value[0]["name"];
            if (name == RETAIL_LEVEL2_ACCESS || name == RETAIL_LEVEL3_ACCESS) {
                //var ValidationAttr = getControlValue(Attributes.Validation);
                //if (ValidationAttr == null) return enable;
                //if (ValidationAttr == false && Xrm.Page.ui.getFormType() != FORM_MODE_CREATE) {
                    enable = true;
               // }
            }
            if (enable == true) break;
        }
        return enable;
    }
    var enableValidateRibbonButton = function () {
        var enable = false;
        var ValidationAttr = getControlValue(Attributes.Validation);
        if (ValidationAttr == null) return enable;
        if (ValidationAttr == true) return enable;
        if(Xrm.Page.ui.getFormType() == FORM_MODE_CREATE) return enable;
        enable = getUserRoles();
        if (enable != null && enable != "" && enable != undefined)
            return enable;
    }
    var enableReOpenRibbonButton = function () {        
        var enable = false;
        if (Xrm.Page.getAttribute(Attributes.StateCode).getValue() != StateCode.Lost) return enable;
        enable = getUserRoles();
        if (enable != null && enable != "" && enable != undefined)
            return enable;
    }
    var setValidationFieldValue = function () {

        Xrm.Page.getAttribute(Attributes.Validation).setValue(true);
        Xrm.Page.data.entity.save();


    }
    var setSurpriseFieldValue = function () {
        var surpriseFieldValue = getControlValue(Attributes.Surprise);
        if (surpriseFieldValue == null) return;
        Xrm.Page.getAttribute(Attributes.Surprise).setValue(!surpriseFieldValue);

    }
    var enableOWRorWebrioButton = function () {

        var enable = false;
        if (window.IsUSD != true) return enable;
        if (Xrm.Page.ui.getFormType() == FORM_MODE_CREATE) return enable;
        if (Xrm.Page.getAttribute(Attributes.StateCode).getValue() != StateCode.Open) return enable;
        if (Xrm.Page.getAttribute(Attributes.CustomerId).getValue() == null) return enable;
        var ssoLoggined = getExternalLogin();
        if (ssoLoggined != false && ssoLoggined != undefined && ssoLoggined != "")
            return ssoLoggined;

    }
    var enableLimeButton = function () {

        var enable = false;
        if (window.IsUSD != true) return enable;
        if (Xrm.Page.ui.getFormType() == FORM_MODE_CREATE) return enable;
        if (Xrm.Page.getAttribute(Attributes.StateCode).getValue() != StateCode.Open) return enable;
        return enable = true;

    }

    var owrRibbonButtonClick = function () {
            window.open(OWR_EVENT_URL);
    }

    var limeRibbonButtonClick = function () {
        if (window.IsUSD == true) {
            window.open(LIME_WEB_PAGE_URL);

        }
    }
    var webrioRibbonButtonClick = function () {
        if (window.IsUSD == true) {
            var consultationNo = Xrm.Page.getAttribute("name").getValue();
            var customer = Xrm.Page.getAttribute("customerid").getValue();
            var customerId;
            if (customer != null) {
                customerId = customer[0].id;
            }
            var strUrl = WEBRIO_WEB_PAGE_URL + "&ConsultationNo=" + consultationNo + "&CustomerId=" + customerId;
            strUrl = strUrl.replace(/[{}]/g, "");

            window.open(strUrl);
        }
    }

    var reviewDateOnChange = function () {

        Xrm.Page.getControl(Attributes.ReviewDate).clearNotification();
        var reviewDateValueAttr = getControlValue(Attributes.ReviewDate);
        var isPastDate = Tc.Crm.Scripts.Utils.Validation.IsPastDate(reviewDateValueAttr);
        if (isPastDate)
            Xrm.Page.getControl(Attributes.ReviewDate).setNotification("Review Date cannot be set in the past. Please choose a future date.");

    }
    var openNewFollowUp = function () {

        var travellerPlannerId = Xrm.Page.data.entity.getId();
        var travellerPlannerName = getControlValue(Attributes.Name)
        var customerId = getControlValue(Attributes.CustomerId);
        var parameters = {};

        //setting CustomerLookup
        if (customerId != null) {
            if (customerId[0] != null && customerId[0] != undefined) {
                if (customerId[0].id != null && customerId[0].id != undefined)
                    parameters["tc_customer"] = customerId[0].id;
                if (customerId[0].name != null && customerId[0].id != undefined && customerId[0].id != "")
                    parameters["tc_customername"] = customerId[0].name;
            }
        }

        //setting ReagardingLookup
        parameters["parameter_regardingid"] = formatEntityId(travellerPlannerId);
        parameters["parameter_regardingname"] = travellerPlannerName;
        parameters["parameter_regardingtype"] = EntityName.TravellerPlanner;

        var windowOptions = {
            openInNewWindow: true
        };
        Xrm.Utility.openEntityForm(EntityName.FollowUp, null, parameters, windowOptions);
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
        OnSave: function (context) {
            var isValid = Tc.Crm.Scripts.Utils.Validation.ValidateGdprCompliance(context);
            // uncomment in case of additional save actions
            //if (isValid) { }
        },
        OnValidateRibbonButtonClick: function () {
            setValidationFieldValue();
        },
        OnLimeRibbonButtonClick: function () {
            limeRibbonButtonClick();
        },
        OnWebRioRibbonButtonClick: function () {
            webrioRibbonButtonClick();
        },
        OnAddFollowUpRibbonButtonClick: function () {
            openNewFollowUp();
        },
        EnableDisableValidateButton: function () {
            return enableValidateRibbonButton();
        },
        EnableLimeButton: function () {
            return enableLimeButton();
        },
        EnableDisableOWRorWebrioButton: function () {
            return enableOWRorWebrioButton();
        },
        OnSurpriseRibbonButtonClick: function () {
            setSurpriseFieldValue();
        },
        OnReviewDateFieldChange: function () {
            reviewDateOnChange();
        },
        OnOwrRibbonButtonClick: function() {
            owrRibbonButtonClick();
        },
        EnableReOpenRibbonButton: function () {          
           return enableReOpenRibbonButton();
        }

    };
})();