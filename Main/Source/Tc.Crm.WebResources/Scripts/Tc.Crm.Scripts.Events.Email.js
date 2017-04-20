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
Tc.Crm.Scripts.Events.Email = (function () {
    "use strict";
    var FORM_MODE_CREATE = 1;
    var ENTITY_INCIDENT = 'incident';
    var FIELD_REGARDING = 'regardingobjectid';
    var FIELD_FROM = 'from';
    var ENTITY_FQ_INCIDENT = 'Microsoft.Dynamics.CRM.incident';
    var PARAM_ODATA_TYPE = '@odata.type';
    var ENTITY_QUEUE = 'queue';
    var FIELD_QUEUE_ID = 'queueid';
    var FIELD_NAME = 'name';
    var HTTP_SUCCESS = 200;
    var STATUS_READY = 4;
    //put your private stuff here
    var clearEmailFrom = function () {
        if (Xrm.Page.ui.getFormType() === FORM_MODE_CREATE) {
            console.log("Clear from field");
            Xrm.Page.getAttribute("from").setValue(null);
        }
    }
    var getLookupObject = function (attributeName) {
        return Xrm.Page.getAttribute(attributeName).getValue();
    }
    var getLookupType = function (lookupObj) {
        if (lookupObj == null) return null;
        return lookupObj[0].entityType;
    }
    var getLookupId = function (lookupObj) {
        if (lookupObj == null) return null;
        return lookupObj[0].id;
    }
    var processOnQueueIdReceived = function (data, request) {
        if (data.readyState === STATUS_READY) {
            request.onreadystatechange = null;
            if (data.status === HTTP_SUCCESS) {
                var results = JSON.parse(data.response);
                if (results == null || results == undefined) {
                    console.log("results is null.");
                    return;
                }
                if (results[FIELD_QUEUE_ID] == null || results[FIELD_QUEUE_ID] == undefined || results[FIELD_QUEUE_ID] == "") {
                    console.log('queueid is null');
                    return;
                }
                getQueueName(results['queueid']);

            } else {
                if (data.response == null || data.response == "" || data.response == undefined)
                    console.log("response is empty")
                else {
                    var error = JSON.parse(data.response).error;
                    console.log(error.message);
                }
            }
        }
    }
    var processOnQueueNameReceived = function (data, request, id) {
        if (data.readyState === STATUS_READY) {
            request.onreadystatechange = null;
            if (data.status === HTTP_SUCCESS) {
                var result = JSON.parse(data.response);
                var name = result[FIELD_NAME];
                var queueLookup = new Array();
                queueLookup[0] = new Object();
                queueLookup[0].id = id;
                queueLookup[0].name = name
                queueLookup[0].entityType = ENTITY_QUEUE;

                var from = Xrm.Page.getAttribute(FIELD_FROM);
                if (from == null || from == undefined) {
                    console.log("unable to locate from on the email form.");
                    return;
                }
                from.setValue(queueLookup);
            } else {
                console.log("Unable to fetch the name of the queue.")
            }
        }
    }
    var getQueueName = function (id) {
        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v8.2/queues(" + id + ")?$select=name", true);
        req = addCommonWebApiHeaders(req);
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        req.onreadystatechange = function () {
            processOnQueueNameReceived(this, req, id);
        };
        req.send();
    }
    var getQueueIdFromCrm = function (incidentId) {
        var parameters = {};
        var incident = {};
        incident.incidentid = incidentId;
        incident[PARAM_ODATA_TYPE] = ENTITY_FQ_INCIDENT;
        parameters.Case = incident;

        var req = new XMLHttpRequest();
        req.open("POST", Xrm.Page.context.getClientUrl() + "/api/data/v8.2/tc_QueueIdentifier", true);
        req = addCommonWebApiHeaders(req);
        req.onreadystatechange = function () {
            processOnQueueIdReceived(this, req);
        };
        req.send(JSON.stringify(parameters));
    }
    var addCommonWebApiHeaders = function (request) {
        request.setRequestHeader("OData-MaxVersion", "4.0");
        request.setRequestHeader("OData-Version", "4.0");
        request.setRequestHeader("Accept", "application/json");
        request.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    }
    var setFromFieldForCase = function () {
        try {

            console.log("setFromFieldForCase invoked.");
            if (Xrm.Page.ui.getFormType() != FORM_MODE_CREATE) return;

            var regardingObject = getLookupObject(FIELD_REGARDING);
            var entityType = getLookupType(regardingObject);

            if (entityType != ENTITY_INCIDENT) return;
            console.log('regarding is of type incident.');
            var incidentId = getLookupId(regardingObject);

            if (incidentId == null || incidentId == undefined) return;
            console.log('incident id is not null or undefined:');
            console.log(incidentId);

            getQueueIdFromCrm(incidentId);
        } catch (e) {
            console.log("unexpected error has occurred in method setFromFieldForCase");
        }

    }
    return {
        OnLoad: function () {
            clearEmailFrom();
            setFromFieldForCase();
        }
    };
})();