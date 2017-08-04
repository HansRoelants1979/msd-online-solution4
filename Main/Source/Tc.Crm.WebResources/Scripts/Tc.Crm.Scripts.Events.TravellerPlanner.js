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
        StateCode : "statecode",
        CustomerId: "customerid",
        ReviewDate: "tc_reviewdate",
    }
    var StateCode = {
        Open : 0,
    }
    var getUserRoles = function () {
        var enable = false;
        var UserRole = window.parent.Xrm.Page.context.getUserRoles();
        if (UserRole == null || UserRole == "" || UserRole == undefined) return enable;
        for (var i = 0; i < UserRole.length; i++) {
            var req = new XMLHttpRequest();
            var userRoleId = UserRole[i];
            req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v8.2/roles?$select=name&$filter=roleid eq " + userRoleId, false);
            req.setRequestHeader("Accept", "application/json");
            req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            req.onreadystatechange = function () {
                if (req.readyState == STATUS_READY && req.status == HTTP_SUCCESS) {
                    var results = JSON.parse(req.response);
                    var name = results.value[0]["name"];
                    if (name == CLUSTER_MANAGER || name == ASSISTANT_MANAGER || name == REGIONAL_MANAGER) {
                        var ValidationAttr = getControlValue(Attributes.Validation);
                        if (ValidationAttr == null) return enable;
                        if (ValidationAttr == false && Xrm.Page.ui.getFormType() != FORM_MODE_CREATE) {
                                enable = true;
                        }
                    }                    
                }
            };
            req.send();
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
        var entity = {};
        entity.tc_validation = true;

        var req = new XMLHttpRequest();
        req.open("PATCH", Xrm.Page.context.getClientUrl() + "/api/data/v8.2/opportunities(" + formatEntityId(Xrm.Page.data.entity.getId()) + ")", true);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState === STATUS_READY) {
                req.onreadystatechange = null;
                if (this.status === NO_CONTENT) {
                    Xrm.Page.data.save().then(function () { Xrm.Page.data.refresh(false); });
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                    console.log("ERROR: " + this.statusText);
                }
            }
        };
        req.send(JSON.stringify(entity));
    }
    var enableOWRorLimeButton = function ()
    {
        var enable = false;
        if (window.IsUSD != true) return enable;       
        if (Xrm.Page.getAttribute(Attributes.StateCode).getValue() != StateCode.Open) return enable;        
        if (Xrm.Page.getAttribute(Attributes.CustomerId).getValue() == null) return enable;        
           return enable = true;        

    }
    var reviewDateOnChange = function () {

        Xrm.Page.getControl(Attributes.ReviewDate).clearNotification();
        var reviewDateValueAttr = getControlValue(Attributes.ReviewDate);
        var isPastDate = Tc.Crm.Scripts.Utils.Validation.IsPastDate(reviewDateValueAttr);
        if (isPastDate)
            Xrm.Page.getControl(Attributes.DueDate).setNotification("Review Date cannot be set in the past. Please choose a future date.");

    }
    function formatEntityId(id) {
        return id !== null ? id.replace("{", "").replace("}", "") : null;
    }
    var getControlValue = function (controlName) {
        if (Xrm.Page.getAttribute(controlName) && (Xrm.Page.getAttribute(controlName).getValue() !=null))
            return Xrm.Page.getAttribute(controlName).getValue();
        else
            return null;
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
        OnReviewDateFieldChange: function () {
            reviewDateOnChange();
        }
        
    };
})();