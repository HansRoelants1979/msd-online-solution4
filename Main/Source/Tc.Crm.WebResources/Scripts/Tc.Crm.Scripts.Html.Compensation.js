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
if (typeof (Tc.Crm.Scripts.Html) === "undefined") {
    Tc.Crm.Scripts.Html = {
        __namespace: true
    };
    Tc.Crm.Scripts.Html.Compensation = (function () {
        "use strict";
        function onLoadGetCompensationSignature() {

            var sigDataAttr = parent.Xrm.Page.getAttribute("tc_compensationsignature");
            if (sigDataAttr != null && sigDataAttr != "undefined") {
                var sigData = sigDataAttr.getValue();
                if (sigData != null) {
                    $("#SignatureImg").attr("src", sigData);
                }
                else {
                    $("body").html("No signature captured.");
                }
            }

        }

        return {
            OnLoad: onLoadGetCompensationSignature
        };
    })();
}
