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
    var getQueueIdFromCrm = function (id) {
        var requestXml = '';
        requestXml += '<s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">';
        requestXml += '  <s:Body>';
        requestXml += '    <Execute xmlns="http://schemas.microsoft.com/xrm/2011/Contracts/Services" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">';
        requestXml += '      <request xmlns:a="http://schemas.microsoft.com/xrm/2011/Contracts">';
        requestXml += '        <a:Parameters xmlns:b="http://schemas.datacontract.org/2004/07/System.Collections.Generic">';
        requestXml += '          <a:KeyValuePairOfstringanyType>';
        requestXml += '            <b:key>Case</b:key>';
        requestXml += '            <b:value i:type="a:EntityReference">';
        requestXml += '              <a:Id>' + id + '</a:Id>';
        requestXml += '              <a:KeyAttributes xmlns:c="http://schemas.microsoft.com/xrm/7.1/Contracts" />';
        requestXml += '              <a:LogicalName>incident</a:LogicalName>';
        requestXml += '              <a:Name i:nil="true" />';
        requestXml += '              <a:RowVersion i:nil="true" />';
        requestXml += '            </b:value>';
        requestXml += '          </a:KeyValuePairOfstringanyType>';
        requestXml += '        </a:Parameters>';
        requestXml += '        <a:RequestId i:nil="true" />';
        requestXml += '        <a:RequestName>tc_QueueIdentifier</a:RequestName>';
        requestXml += '      </request>';
        requestXml += '    </Execute>';
        requestXml += '  </s:Body>';
        requestXml += '</s:Envelope>';
        var req = new XMLHttpRequest();
        req.open("POST", GetClientUrl(), true)
        req.setRequestHeader("Accept", "application/xml, text/xml, */*");
        req.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
        req.setRequestHeader("SOAPAction", "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Execute");
        req.onreadystatechange = function () {
            processOnQueueIdReceived(this, req);
        };
        req.send(requestXml);
        
    }
    var GetClientUrl = function () {
        var clientUrl = '';
        if (typeof Xrm.Page.context == "object") {
            clientUrl = Xrm.Page.context.getClientUrl();
        }
        var ServicePath = "/XRMServices/2011/Organization.svc/web";
        return clientUrl + ServicePath;
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
                if (data == null || data.responseXML == null || data.responseXML.childNodes == null) return;
                var resultXml = data.responseXML.childNodes[0].innerHTML;
                if (resultXml == null || resultXml == undefined || resultXml == '') return;
                var queueId = parseXml(resultXml);
                getQueueName(queueId);

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
    var addCommonWebApiHeaders = function (request) {
        request.setRequestHeader("OData-MaxVersion", "4.0");
        request.setRequestHeader("OData-Version", "4.0");
        request.setRequestHeader("Accept", "application/json");
        request.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        return request;
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
    var parseXml = function (resultXml) {
        resultXml = resultXml.substring(resultXml.indexOf('a:Id'), resultXml.indexOf('/a:Id'))
        resultXml = resultXml.substring(resultXml.indexOf('>') + 1, resultXml.indexOf('<'));
        return resultXml;
    }
    return {
        OnLoad: function () {
            clearEmailFrom();
            setFromFieldForCase();
        }
    };
})();