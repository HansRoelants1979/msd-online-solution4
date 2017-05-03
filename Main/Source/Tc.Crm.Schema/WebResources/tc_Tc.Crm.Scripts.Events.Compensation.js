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
Tc.Crm.Scripts.Events.Compensation = (function () {
    "use strict";

    var FORM_MODE_CREATE = 1;
    var CLIENT_MODE_MOBILE = "Mobile";
    var CLIENT_STATE_OFFLINE = "Offline";

    var CASE_LINE_ENTITY = "tc_caseline";
    var COMPENSATION_MATRIX_ENTITY = "tc_compensationcalculationmatrix";
    var COMPENSATION_MATRIX_ENTITY_PLURAL = "tc_compensationcalculationmatrixe";

    var SOURCEMARKET_ATTR_NAME = "tc_sourcemarketid";

    var COMPENSATION1_ATTR_NAME = "tc_compensation1id";
    var COMPENSATION2_ATTR_NAME = "tc_compensation2id";
    var COMPENSATION3_ATTR_NAME = "tc_compensation3id";
    var COMPENSATION4_ATTR_NAME = "tc_compensation4id";
    var COMPENSATION5_ATTR_NAME = "tc_compensation5id";

    // Function to store returned values for the calculation
    function CATRESPONSE()
    {
        var catResponse = {};
        catResponse.cat1 = null;
        catResponse.cat2 = null;
        catResponse.cat3 = null;
        catResponse.severity = null;
        catResponse.impact = null;
        catResponse.matrixId = null;
        return catResponse;
    }

    var CATRESPONSE1 = CATRESPONSE();
    var CATRESPONSE2 = CATRESPONSE();
    var CATRESPONSE3 = CATRESPONSE();
    var CATRESPONSE4 = CATRESPONSE();
    var CATRESPONSE5 = CATRESPONSE();

    var SEVERITYCAT1_ATTR_NAME = "tc_cat1severity";
    var SEVERITYCAT2_ATTR_NAME = "tc_cat2severity";
    var SEVERITYCAT3_ATTR_NAME = "tc_cat3severity";
    var SEVERITYCAT4_ATTR_NAME = "tc_cat4severity";
    var SEVERITYCAT5_ATTR_NAME = "tc_cat5severity";

    var IMPACTCAT1_ATTR_NAME = "tc_cat1impact";
    var IMPACTCAT2_ATTR_NAME = "tc_cat2impact";
    var IMPACTCAT3_ATTR_NAME = "tc_cat3impact";
    var IMPACTCAT4_ATTR_NAME = "tc_cat4impact";
    var IMPACTCAT5_ATTR_NAME = "tc_cat5impact";

    var DAYSAFFECTEDCAT1_ATTR_NAME = "tc_cat1_daysaffected";
    var DAYSAFFECTEDCAT2_ATTR_NAME = "tc_cat2_daysaffected";
    var DAYSAFFECTEDCAT3_ATTR_NAME = "tc_cat3_daysaffected";
    var DAYSAFFECTEDCAT4_ATTR_NAME = "tc_cat4_daysaffected";
    var DAYSAFFECTEDCAT5_ATTR_NAME = "tc_cat5_daysaffected";
    // Common functions
    var formatValue6DigitsWithHyphen = function (context) {
        var formatted = false;
        var attribute = context.getEventSource();
        var value = attribute.getValue();
        if (/^\d{6}$/.test(value)) {
            var formattedValue = value[0] + value[1] + '-' + value[2] + value[3] + '-' + value[4] + value[5];
            attribute.setValue(formattedValue);
            formatted = true;
        }
        var isValid = formatted || /^\d{2}-\d{2}-\d{2}$/.test(value);
        attribute.controls.forEach(
            function (control, i) {
                if (isValid) {
                    control.clearNotification();
                } else {
                    control.setNotification("Should be 6 digit format nn-nn-nn");
                }
            });
    }

    function formatEntityId(id) {
        return id !== null ? id.replace("{", "").replace("}", "") : null;
    }

    function IsMobileOfflineMode() {
        return Xrm.Page.context.client.getClient() === CLIENT_MODE_MOBILE && Xrm.Page.context.client.getClientState() === CLIENT_STATE_OFFLINE
    }

    function getPromiseResponse(promiseResponse, entity) {
        if (IsMobileOfflineMode()) {
            return promiseResponse.values;
        }
        else {
            if (promiseResponse.response === null || promiseResponse.response === undefined) {
                Xrm.Utility.alertDialog(entity + " information can't be retrieved");
                console.warn(entity + " information can't be retrieved");
                return [];
            }
            try {
                return JSON.parse(promiseResponse.response);
            }
            catch (e) {
                Xrm.Utility.alertDialog(entity + " information can't be parsed");
                console.warn(entity + " information can't be parsed");
                return [];
            }
        }
    }
    //End common functions


    //Compensation Calculation Start Function
    var compensationCalculation = function () {
        if (Xrm.Page.ui.getFormType() === FORM_MODE_CREATE) {
            Xrm.Utility.alertDialog("Please save the record to calculate the proposed compensation.");
            console.warn("Please save the record to calculate the proposed compensation.");
            return;
        }
        
        
        var sourceMarket = Xrm.Page.getAttribute(SOURCEMARKET_ATTR_NAME).getValue();
        if (sourceMarket !== null && sourceMarket !== undefined) {
            var sourceMarketId = formatEntityId(sourceMarket[0].id);
        
            getAllSeverityAndImpact();
            getCategoriesFromCaseLines();


        }
        else
        {
            Xrm.Utility.alertDialog("Unable to obtain Source Market to perform the calculation.");
            console.warn("Unable to obtain Source Market to perform the calculation.");
        }

    }




    function getMatrix(catResponse1, catResponse2, catResponse3, catResponse4, catResponse5) {

        Promise.all([retrieveCompensationMatrix(catResponse1), retrieveCompensationMatrix(catResponse2), retrieveCompensationMatrix(catResponse3), retrieveCompensationMatrix(catResponse4), retrieveCompensationMatrix(catResponse5)]).then(
                function (allData) {
                    parseAlldataMatrix(allData, catResponse1, 0);
                    parseAlldataMatrix(allData, catResponse2, 1);
                    parseAlldataMatrix(allData, catResponse3, 2);
                    parseAlldataMatrix(allData, catResponse4, 3);
                    parseAlldataMatrix(allData, catResponse5, 4);

                });

    }

    function parseAlldataMatrix(allData, responseField, index) {
        if (allData[index] !== null) {
            var matrix = getPromiseResponse(allData[index], COMPENSATION_MATRIX_ENTITY);
            if (matrix.value.lenght > 0) {
                responseField.matrix = matrix.value[0].tc_compensationcalculationmatrixid;
            }
            else
            {
                responseField.matrix = null;
            }

        }
    }

    function retrieveCompensationMatrix(catResponse)
    {
        if (IsMobileOfflineMode()) {
            // phones and tablets in offline mode
            var query = "?$filter=customerid eq " + customerId + " &$select=incidentid";
            return Xrm.Mobile.offline.retrieveMultipleRecords(COMPENSATION_MATRIX_ENTITY, query);
        }
        else {
            var query = "?$filter=_tc_category1_value eq " + catResponse.cat1 + " and _tc_category2_value eq " + catResponse.cat2 + " and _tc_category3_value eq " + catResponse.cat3 + " &$select=tc_compensationcalculationmatrixid";
            return Tc.Crm.Scripts.Common.Get(COMPENSATION_MATRIX_ENTITY_PLURAL, query);
        }
    }

    function getAllSeverityAndImpact()
    {
        getSeverityAndImpact(IMPACTCAT1_ATTR_NAME, SEVERITYCAT1_ATTR_NAME, CATRESPONSE1);
        getSeverityAndImpact(IMPACTCAT2_ATTR_NAME, SEVERITYCAT2_ATTR_NAME, CATRESPONSE2);
        getSeverityAndImpact(IMPACTCAT3_ATTR_NAME, SEVERITYCAT3_ATTR_NAME, CATRESPONSE3);
        getSeverityAndImpact(IMPACTCAT4_ATTR_NAME, SEVERITYCAT4_ATTR_NAME, CATRESPONSE4);
        getSeverityAndImpact(IMPACTCAT5_ATTR_NAME, SEVERITYCAT5_ATTR_NAME, CATRESPONSE5);
    }

    function getSeverityAndImpact(impactField, severityField, responseReference)
    {
        responseReference.impact = Xrm.Page.getAttribute(impactField).getValue();
        responseReference.severity = Xrm.Page.getAttribute(severityField).getValue();

    }

    function getCategoriesFromCaseLines() {
        getCategoryFromCaseLine(COMPENSATION1_ATTR_NAME, CATRESPONSE1,COMPENSATION2_ATTR_NAME, CATRESPONSE2,COMPENSATION3_ATTR_NAME, CATRESPONSE3,COMPENSATION4_ATTR_NAME, CATRESPONSE4,COMPENSATION5_ATTR_NAME, CATRESPONSE5);

    }

    function getCategoryFromCaseLine(caseLineField1,response1,caseLineField2,response2,caseLineField3,response3,caseLineField4,response4,caseLineField5,response5) {
        
        Promise.all([retrieveCaseLine(caseLineField1), retrieveCaseLine(caseLineField2), retrieveCaseLine(caseLineField3), retrieveCaseLine(caseLineField4), retrieveCaseLine(caseLineField5)]).then(
                function (allData){
                    parseAlldataCaseLine(allData, response1, 0);
                    parseAlldataCaseLine(allData, response2, 1);
                    parseAlldataCaseLine(allData, response3, 2);
                    parseAlldataCaseLine(allData, response4, 3);
                    parseAlldataCaseLine(allData, response5, 4);
                    getMatrix(CATRESPONSE1,CATRESPONSE2,CATRESPONSE3,CATRESPONSE4,CATRESPONSE5);

                });
        
    }

    function parseAlldataCaseLine(allData,responseField,index)
    {
        if (allData[index] !== null) {
            var line = getPromiseResponse(allData[index], CASE_LINE_ENTITY);
            responseField.cat1 = line._tc_categorylevel1id_value;
            responseField.cat2 = line._tc_casecategory2id_value;
            responseField.cat3 = line._tc_category3id_value;
        }
    }

    function retrieveCaseLine(caseLineField) {
        var caseLineId = null;
        var compensation = Xrm.Page.getAttribute(caseLineField).getValue();
        if (compensation === null || compensation === undefined) {
            caseLineId = null;
        }
        else
        {
            caseLineId = formatEntityId(compensation[0].id)
        }
        if (caseLineId === null)
        {
            return null;
        }
        var entityName = CASE_LINE_ENTITY;
        var query = "?$select=_tc_categorylevel1id_value,_tc_casecategory2id_value,_tc_category3id_value";
        if (Xrm.Page.context.client.getClient() === CLIENT_MODE_MOBILE && Xrm.Page.context.client.getClientState() === CLIENT_STATE_OFFLINE) {
            // phones and tablets in offline mode
            return Xrm.Mobile.offline.retrieveRecord(entityName, caseLineId, query);
        }
        else {
            return Tc.Crm.Scripts.Common.GetById(entityName, caseLineId, query);
        }
    }

    return {
        OnLoad: function () {
            Tc.Crm.Scripts.Library.Compensation.SetDefaultsOnCreate();
        },
        OnAccountSortCodeChanged: function (context) {
            if (context === null) {
                console.log("Tc.Crm.Scripts.Events.Compensation.OnAccountSortCodeChanged should be configured to pass execution context");
                return;
            }
            formatValue6DigitsWithHyphen(context);
        },
        OnCompensationCalculate: function () {
            compensationCalculation();
        }
    };
})();