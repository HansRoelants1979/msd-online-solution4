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

    var CLIENT_MODE_MOBILE = "Mobile";
    var CLIENT_STATE_OFFLINE = "Offline";

    var SOURCE_MARKET_UK = "GB";

    var CASE_LINE_ENTITY_SET_NAME = "tc_caselines";
    var CONFIGURATION_ENTITY_SET_NAME = "tc_configurations";
    var COUNTRY_ENTITY_SET_NAME = "tc_countries";
    var CASE_ENTITY_SET_NAME = "incidents";

    var Attributes = {
        Case: "tc_caseid",
        CaseLine1: "tc_compensation1id",
        CaseLine2: "tc_compensation2id",
        CaseLine3: "tc_compensation3id",
        CaseLine4: "tc_compensation4id",
        Amount: "tc_amount",
        CompensationAmountLimit: "tc_compensationamountlimit",
        SourceMarket: "tc_sourcemarketid"
    }

    var Configuration = {
        LimitUk: "Tc.Compensation.UpperLimit.UnitedKingdom",
        LimitContinental: "Tc.Compensation.UpperLimit.Continental"
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

    var addCaseLineToCalculation = function (promises, names, caseLine) {
        var value = Xrm.Page.getAttribute(caseLine).getValue();
        if (value != null && value.length > 0 && value[0] != null)
        {
            var id = formatEntityId(value[0].id);
            var promise = getCaseLine(id);
            promises.push(promise);
            names.push(value[0].name);
        }
    }

    var getConfigurationValue = function (configName) {
        if (IsMobileOfflineMode()) {
            // TODO: offline query
        }
        else {
            var query = "?$filter=tc_name eq '" + configName + "' &$select=tc_value";
            return Tc.Crm.Scripts.Common.Get(CONFIGURATION_ENTITY_SET_NAME, query);
        }
    }

    function getCase(caseId) {
        if (IsMobileOfflineMode()) {
        }
        else {
            var query = "?$select=tc_bookingreference,_tc_bookingid_value,tc_bookingtravelamount,tc_durationofstay";
            return Tc.Crm.Scripts.Common.GetById(CASE_ENTITY_SET_NAME, caseId, query);
        }
    }

    var caseRetrieved = function (caseResponse) {
        getConfigurationValue(Configuration.LimitContinental).then(function (response) {
                var limit = JSON.parse(response.response);
                if (limit.value.length === 0 || limit.value[0].tc_value === null) {
                    Xrm.Utility.alertDialog("No value in configuration for " + Configuration.LimitContinental + ". Contact System configuration");
                    return;
                }
                var limitValue = parseFloat(limit.value[0].tc_value);
                var incident = JSON.parse(caseResponse.response);
                var bookingUsed = incident.tc_bookingreference;
                if (bookingUsed) {
                    var bookingId = incident._tc_bookingid_value;
                    getBooking(bookingId).then(
                        function (bookingResponse) {
                            var booking = JSON.parse(bookingResponse.response);
                            var value = booking.tc_travelamount;
                            var duration = incident.tc_duration;
                            setContinentalCompensationAmountLimit(value, duration, limitValue);
                        },
                        function (error) {
                            Xrm.Utility.alertDialog("Problem getting booking");
                            console.warn("Problem getting booking");
                        }
                    );
                }
                else {
                    var value = incident.tc_bookingtravelamount;
                    var duration = incident.tc_durationofstay;
                    setContinentalCompensationAmountLimit(value, duration, limitValue);
                }
                
            },
            function (error) {
            });
    }


    function getCaseLine(caseLineId) {
        if (IsMobileOfflineMode()) {
            // phones and tablets in offline mode
        }
        else {
            var query = "?$select=tc_offeredamount";
            return Tc.Crm.Scripts.Common.GetById(CASE_LINE_ENTITY_SET_NAME, caseLineId, query);
        }
    }

    function getBooking(bookingId) {
        if (IsMobileOfflineMode()) {
            // TODO:
        } else {
            var query = "?$select=tc_duration,tc_travelamount";
            return Tc.Crm.Scripts.Common.GetById(BOOKING_ENTITY_SET_NAME, bookingId, query);
        }
    }

    var getSourceMarketIso2Code = function (countryId) {
        if (IsMobileOfflineMode()) {
            // phones and tablets in offline mode
        }
        else {
            var query = "?$select=tc_iso2code";
            return Tc.Crm.Scripts.Common.GetById(COUNTRY_ENTITY_SET_NAME, countryId, query);
        }
    }

    var setContinentalCompensationAmountLimit = function (bookingValue, duration, maxLimit) {
        var limit = bookingValue / duration * maxLimit / 100;
        Xrm.Page.getAttribute(Attributes.CompensationAmountLimit).setValue(limit);
    }

    var setCompensationAmountLimitUk = function () {
        getConfigurationValue(Configuration.LimitUk).then(function (response) {
                var limit = JSON.parse(response.response); 
                if (limit.value.length === 0 || limit.value[0].tc_value === null) {
                    Xrm.Utility.alertDialog("No value in configuration for " + Configuration.LimitUk + "Contact System configurator");
                    return;
                }
                var limitValue = parseFloat(limit.value[0].tc_value);
                Xrm.Page.getAttribute(Attributes.CompensationAmountLimit).setValue(limitValue);
            },
            function (error) {
                Xrm.Utility.alertDialog("Problem getting configuration value");
                console.warn("Problem getting configuration value");
            });
    }

    var setCompensationAmountLimitContinental = function () {
        var caseIdAttr = Xrm.Page.getAttribute(Attributes.Case);
        var caseId = formatEntityId(caseIdAttr.getValue()[0].id);
        var caseReceivedPromise = getCase(caseId);
        caseReceivedPromise.then(
            caseRetrieved,
            function (error) {
                Xrm.Utility.alertDialog("Problem getting case");
            }
        );
    }


    // Set compensation amount limit based on source market
    var setCompensationAmountLimit = function () {
        var country = Xrm.Page.getAttribute(Attributes.SourceMarket).getValue()[0].id;        
        getSourceMarketIso2Code(formatEntityId(country)).then(function (response) {
                var iso2Code = JSON.parse(response.response);
                var isUkMarket = iso2Code.tc_iso2code.toUpperCase() === SOURCE_MARKET_UK;
                if (isUkMarket) {
                    setCompensationAmountLimitUk();
                } else {
                    setCompensationAmountLimitContinental();
                }
            },
            function (error) {
                Xrm.Utility.alertDialog("Problem getting source market");
                console.warn("Problem getting source market:" + error);
            });
    }

    // Calculate compensation
    var calculateCompensation = function () {
        var promises = [];
        var names = [];
        addCaseLineToCalculation(promises, names, Attributes.CaseLine1);
        addCaseLineToCalculation(promises, names, Attributes.CaseLine2);
        addCaseLineToCalculation(promises, names, Attributes.CaseLine3);
        addCaseLineToCalculation(promises, names, Attributes.CaseLine4);
        Promise.all(promises).then(
            function (response) {
                var totalAmount = 0;
                var totalAmountAttr = Xrm.Page.getAttribute(Attributes.Amount);
                for (var i = 0; i < response.length; i++) {
                    var caseLineOffer = JSON.parse(response[i].response);
                    if (caseLineOffer == null || caseLineOffer.tc_offeredamount === null) {
                        totalAmountAttr.controls.forEach(
                            function (control, j) {
                                control.setNotification("Offered amount for case line " + names[i] + " is not set");
                            });
                        return;
                    }
                    totalAmount = totalAmount + caseLineOffer.tc_offeredamount;
                }
                totalAmountAttr.controls.forEach(
                    function (control, j) {
                        control.clearNotification();
                    });
                totalAmountAttr.setValue(totalAmount);
                setCompensationAmountLimit();
            },
            function (error) {
                Xrm.Utility.alertDialog("Problem getting case lines");
                console.warn("Problem getting case lines:" + error);
            });
    }


    // public
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
        OnCompensationCalculate: calculateCompensation
    };
})();