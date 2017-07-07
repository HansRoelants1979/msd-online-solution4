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
if (typeof (Tc.Crm.Scripts.Ribbon) === "undefined") {
    Tc.Crm.Scripts.Ribbon = {
        __namespace: true
    };
}
Tc.Crm.Scripts.Ribbon.TravellerPlanner = (function () {
    "use strict";

    var FORM_MODE_UPDATE = 2;
    var HTTP_SUCCESS = 200;
    var STATUS_READY = 4;
    var NO_CONTENT = 204;


    var Attributes = {
        Validation: "tc_validation",
    }

    var GetUserRoles = function () {
        var bool = false;
        var UserRole = window.parent.Xrm.Page.context.getUserRoles();
        if (UserRole == null || UserRole == "" || UserRole == undefined) return bool;
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
                    if (name == "System Customizer" || name == "Sales Manager") {
                        if (Xrm.Page.getAttribute(Attributes.Validation).getValue() == false && Xrm.Page.ui.getFormType() == FORM_MODE_UPDATE) {
                            bool = true;
                        }
                    }                    
                }
            };
            req.send();
            if (bool == true) break;
        }
        return bool;
    }
    var EnableValidateRibbonButton = function () {
        var bool = GetUserRoles();
        if (bool != null && bool != "" && bool != undefined)
            return bool;
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
                }
            }
        };
        req.send(JSON.stringify(entity));
    }
    function formatEntityId(id) {
        return id !== null ? id.replace("{", "").replace("}", "") : null;
    }
    return {
        OnValidateRibbonButtonClick: function () {
            setValidationFieldValue();
        },
        refreshRibbonNavigation: function () {
            Xrm.Page.ui.refreshRibbon();
        },
        OnRibbonLoad: function () {            
          return EnableValidateRibbonButton();            
        }
    };
})();