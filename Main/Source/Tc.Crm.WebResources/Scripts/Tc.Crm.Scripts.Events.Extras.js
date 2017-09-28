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

if (typeof (Tc.Crm.Scripts.Events.Extras) === "undefined") {
	Tc.Crm.Scripts.Events.Extras = {
        __namespace: true
    };
}

Tc.Crm.Scripts.Events.Extras = (function () {
    "use strict";
	var id = "00000000-0000-0000-0000-000000000000";
	var twoOptionsDataType = 950000001;
	var textDataType = 950000000;
	var extraResponses = "";
	var categories = "";
	var extras = [];
	var entitySetNames = {
	    Opportunities: "opportunities",
        ExtraCategories: "tc_extrascategories",
        ExtraResponses: "tc_extrasresponses",
        Extras:"tc_extras"
	}

	var getResponseText = function (dataType, responseId) {
	    if ($('#'+responseId).length < 1) {
	        return null;
	    }
    	switch (dataType) {
    		case twoOptionsDataType:
    			return $("#" + responseId).is(':checked').toString();
    		case textDataType:
    			return $("#" + responseId).val();
    		default:
    			return $("#" + responseId).val();
    	}
    }

    var getHtmlValue = function (responseText, dataType) {
    	switch (dataType) {
    		case twoOptionsDataType:
    			return responseText === 'true' ? "checked" : "";
    	    case textDataType:
	            if (responseText) {
	                return " value=" + responseText;
	            } else return "";
		    default:
    			return " value=" + responseText;;
    	}
    }

    var getControlByDataType = function (dataType) {
    	switch (dataType) {
    		case twoOptionsDataType:
    			return "checkbox";
    		case textDataType:
    			return "text";
    		default:
    			return "text";
    	}
    }

    var updateExtrasResponses = function () {
    	if (extraResponses !== "" && extraResponses.value.length > 0) {
    		extraResponses.value.forEach(function (item) {
    			var extraResponse = {};
    			extraResponse.tc_name = getResponseText(item.tc_ExtraId.tc_responsedatatype, item.tc_extrasresponseid);
    		    Tc.Crm.Scripts.Common.Update(entitySetNames.ExtraResponses, item.tc_extrasresponseid, extraResponse)
                    .then(function () {
                    	console.log('Extra Response was updated successfully.');
                    })
                    .catch(function (err) {
                    	console.log("ERROR: " + err.message);

                    });
    		});
    	}
    }

    var createExtrasResponses = function () {
        var itemsProcessed = 0;
        extras.forEach(function (item) {
            var resp = getResponseText(item.tc_responsedatatype, item.tc_extraid);
            if (resp == null) {
                return true;
            }
            var extraResponse = {};
            extraResponse.tc_name = resp;
            extraResponse["tc_ExtraId@odata.bind"] =
                "/" + entitySetNames.Extras + "(" + item.tc_extraid + ")";
            extraResponse["tc_TravelPlannerId@odata.bind"] =
                "/"+entitySetNames.Opportunities+"(" + id + ")";
            Tc.Crm.Scripts.Common.Create(entitySetNames.ExtraResponses, extraResponse)
                .then(function () {
                    itemsProcessed++;
                    console.log('Response was created successfully.');
                    if (itemsProcessed === extras.length) {
                        window.location.reload(true);
                    }
                })
                .catch(function (err) {
                    console.log("ERROR: " + err.message);
                });
        });
    }

    var getCategoryQuery = function () {
    	var categoryProperties = [
            "tc_label",
            "tc_name",
            "tc_extrascategoryid"
    	].join();
    	var categoryQuery = "?$select=" +
            categoryProperties +
            "&$orderby=tc_order";
    	return categoryQuery;
    }

    var getExtrasQuery = function (extrasCategoryId) {
    	var extrasProperties = [
                        "tc_label",
                        "tc_name",
                        "tc_responsedatatype",
                        "tc_extraid",
                        "_tc_extrascategoryid_value"
    	].join();
    	var extrasQuery = "?$filter=_tc_extrascategoryid_value eq " +
                           extrasCategoryId +
                           "&$select=" +
                           extrasProperties +
                           "&$orderby=tc_order";
    	return extrasQuery;
    }

    var getExtrasResponseQuery = function () {
    	var responseProperties = [
                        "tc_name",
                        "tc_extrasresponseid",
                        "_tc_extraid_value"
    	].join();
    	var expandProperties = [
            "tc_responsedatatype"
    	].join();

    	var requestQuery = "?$filter=_tc_travelplannerid_value eq " +
            id +
            "&$select=" +
            responseProperties +
            "&$expand=tc_ExtraId($select=" + expandProperties + ")";
    	return requestQuery;
    }
	
	//extract encoded querystring values from the "data" value
    var parseDataValue = function (datavalue) {
    	if (datavalue !== "") {
    		var vals = new Array();
    		vals = decodeURIComponent(datavalue).split("&");
	        var i;
	        for (i in vals) {
    			vals[i] = vals[i].replace(/\+/g, " ").split("=");
    		}
    		for (i in vals) {
    			if (vals[i][0] === "id") {
    				id = vals[i][1];
    			}
    		}
    	}
    }
	//parse the data querystring value
    var getDataParams = function () {
    	var vals = new Array();
    	if (location.search !== "") {
    		vals = location.search.substr(1).split("&");
    		for (var i in vals) {
    			vals[i] = vals[i].replace(/\+/g, " ").split("=");
    		}
    		//look for the parameter named 'data'
    		var found = false;
    		for (var i in vals) {
    			if (vals[i][0].toLowerCase() === "data") {
    				parseDataValue(vals[i][1]);
    				found = true;
    				break;
    			}
    		}
    	}
    }

    var getResponse = function (extrasId) {
        var result = $.grep(extraResponses.value, function (e) { return e._tc_extraid_value === extrasId; });
        if (result.length > 0) {
            return result[0];
        }
        else return null;
    }

    var createTable = function() {
        var table = [];
        table.push("<table>");
        for (let i = 0; i < categories.value.length; i++) {
            if (categories.value[i]) {
                table.push("<tr><th value=" +
                    categories.value[i].tc_extrascategoryid +
                    ">" +
                    categories.value[i].tc_label +
                    "</th></tr>");
                try {
                    for (let j = 0; j < extras.length; j++) {
                        let response = null;
                        if (extraResponses.value.length > 0) {
                            response = getResponse(extras[j].tc_extraid);
                        }
                        if (extras[j] &&
                            extras[j]._tc_extrascategoryid_value ===
                            categories.value[i].tc_extrascategoryid) {
                            var label = extras[j].tc_label ? extras[j].tc_label : "";
                            table.push("<tr>");
                            if (label !== "") {
                                table.push("<td>" +
                                    label +
                                    "</td>");
                            }
                            table.push("<td><input type=" +
                                getControlByDataType(extras[j].tc_responsedatatype));
                            if (response == null) {
                                table.push(" id=" +
                                    extras[j].tc_extraid +
                                    "></input></td></tr>");
                            } else {
                                table.push(
                                    " id=" +
                                    response.tc_extrasresponseid +
                                    " " +
                                    getHtmlValue(response.tc_name,
                                        response.tc_ExtraId.tc_responsedatatype) +
                                    "></input></td></tr>");
                            }
                        }
                    }
                }
                catch (e) {
                    console.log(e.message);
                }
            }
        }
        table.push("</table>");
        $("#divExtras").html(table.join(''));
    } 

    var loadExtrasData = function () {
        Tc.Crm.Scripts.Common.Get(entitySetNames.ExtraResponses, getExtrasResponseQuery())
            .then(function(requestResponses) {
                extraResponses = JSON.parse(requestResponses.response);
                Tc.Crm.Scripts.Common.Get(entitySetNames.ExtraCategories, getCategoryQuery())
                    .then(function(request) {
                        categories = JSON.parse(request.response);
                        if (categories && categories.value) {
                            var itemsProcessed = 0;
                            categories.value.forEach(function(item) {
                                Tc.Crm.Scripts.Common.Get(entitySetNames.Extras,
                                        getExtrasQuery(item.tc_extrascategoryid))
                                    .then(function(requestExtras) {
                                        itemsProcessed++;
                                        if (extras.length > 0) {
                                            extras = extras.concat(JSON.parse(requestExtras.response).value);
                                        } else {
                                            extras = JSON.parse(requestExtras.response).value;
                                        }
                                        if (itemsProcessed === categories.value.length) {
                                            createTable();
                                        }
                                    })
                                    .catch(function(err) {
                                        console.log("ERROR: " + err.message);
                                    });

                            });
                        }
                    })
                    .catch(function(err) {
                        console.log("ERROR: " + err.message);
                    });
            })
            .catch(function(err) {
                console.log("ERROR: " + err.message);
            });
    }; 
	
    var submitExtraResponses = function () {
        createExtrasResponses();
    	updateExtrasResponses();
    }

    var loadExtras = function () {
    	getDataParams();
    	if (id === "") {
    		return;
    	}
        loadExtrasData();
    };
    return {
        LoadExtras: loadExtras,
        SubmitExtraResponses: submitExtraResponses
    };
})();