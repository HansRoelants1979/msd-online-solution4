var scriptLoader = scriptLoader || {
    delayedLoads: [],
    load: function (name, requires, script) {
        window._loadedScripts = window._loadedScripts || {};
        // Check for loaded scripts, if not all loaded then register delayed Load
        if (requires == null || requires.length == 0 || scriptLoader.areLoaded(requires)) {
            scriptLoader.runScript(name, script);
        }
        else {
            // Register an onload check
            scriptLoader.delayedLoads.push({ name: name, requires: requires, script: script });
        }
    },
    runScript: function (name, script) {
        script.call(window);
        window._loadedScripts[name] = true;
        scriptLoader.onScriptLoaded(name);
    },
    onScriptLoaded: function (name) {
        // Check for any registered delayed Loads
        scriptLoader.delayedLoads.forEach(function (script) {
            if (script.loaded == null && scriptLoader.areLoaded(script.requires)) {
                script.loaded = true;
                scriptLoader.runScript(script.name, script.script);
            }
        });
    },
    areLoaded: function (requires) {
        var allLoaded = true;
        for (var i = 0; i < requires.length; i++) {
            allLoaded = allLoaded && (window._loadedScripts[requires[i]] != null);
            if (!allLoaded)
                break;
        }
        return allLoaded;
    }
};
scriptLoader.load("Tc.Crm.Scripts.Events.Compensation", ["Tc.Crm.Scripts.Common", "Tc.Crm.Scripts.Library.Compensation"], function () {
    // start script

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

        var reopenCompensationButtonRibbonButtonEnabled = null;
        var isComplainCaseType = null;
        var isIncidentCaseType = null;

        var SourceMarket = {
            UK: "GB"
        };

        var ClientState = {
            Offline: "Offline"
        };

        var SecurityRole = {
            CustomerRelationsManager: "Tc.CustomerRelations.Manager",
            CustomerRelationsAgent: "Tc.CustomerRelations.Agent",
            SystemAdministrator: "System Administrator",
            SystemMaintenance: "Tc.Administator.SystemMaintenance"
        };

        var StatusReason = {
            PaymentProcessed: 950000001,
            ComebackInProgress: 950000002,
            ComebackPaymentProcessed: 950000003
        };

        var FormMode = {
            Create: 1,
            Update: 2
        };

        var CaseType = {
            Complain: "478C99E9-93E4-E611-8109-1458D041F8E8",
            Incident: "D264C3F0-93E4-E611-8109-1458D041F8E8"
        };

        var EntitySetNames = {
            CaseLine: "tc_caselines",
            Configuration: "tc_configurations",
            Country: "tc_countries",
            Booking: "tc_bookings",
            Case: "incidents",
            Compensation: "tc_compensations",
            SecurityRole: "roles",
        }

        var EntityNames = {
            CaseLine: "tc_caseline",
            Configuration: "tc_configuration",
            Country: "tc_country",
            Booking: "tc_booking",
            Case: "incident",
            Compensation: "tc_compensation"
        }

        var Attributes = {
            Case: "tc_caseid",
            CaseLine1: "tc_compensation1id",
            CaseLine2: "tc_compensation2id",
            CaseLine3: "tc_compensation3id",
            CaseLine4: "tc_compensation4id",
            Amount: "tc_amount",
            CompensationAmountLimit: "tc_compensationamountlimit",
            SourceMarket: "tc_sourcemarketid",
            CashAmount: "tc_cashamount",
            BacsAmount: "tc_bacsamount",
            NonFinAmount: "tc_nonfinancialamount",
            ChapsAmount: "tc_chapsamount",
            VoucherAmount: "tc_voucheramount",
            StateCode: "statecode",
            StatusCode: "statuscode",
            CompensationId: "tc_compensationid",
            BacsAccountNumber: "tc_bacsaccountnumber"
        }

        var Configuration = {
            LimitUk: "Tc.Compensation.UpperLimit.UnitedKingdom",
            LimitContinental: "Tc.Compensation.UpperLimit.Continental",
            IncidentApprovalLimitUk: "Tc.Incident.ApprovalLimit.UK",
            IncidentApprovalLimitNonUk: "Tc.Incident.ApprovalLimit.Non-UK"
        }

        function formatEntityId(id) {
            return id !== null ? id.replace("{", "").replace("}", "") : null;
        }

        function isOfflineMode() {
            return Xrm.Page.context.client.getClientState() === ClientState.Offline;
        }

        function getPromiseResponse(promiseResponse, entity) {
            if (promiseResponse == null) return null;
            if (isOfflineMode()) {
                return promiseResponse.values != null ? promiseResponse.values : promiseResponse;
            }
            else {
                if (promiseResponse.response === null || promiseResponse.response === undefined) {
                    console.warn(entity + " information can't be retrieved");
                    return null;
                }
                try {
                    return JSON.parse(promiseResponse.response);
                }
                catch (e) {
                    console.warn(entity + " information can't be parsed");
                    return null;
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
            var attr = Xrm.Page.getAttribute(caseLine);
            if (attr == null) return;
            var value = attr.getValue();
            if (value == null || value.length == 0 || value[0] == null) return;

            var id = formatEntityId(value[0].id);
            var promise = getCaseLine(id);
            promises.push(promise);
            names.push(value[0].name);
        }

        var getConfigurationValue = function (configName) {
            if (isOfflineMode()) {
                var query = "?$filter=tc_name eq '" + configName + "' &$select=tc_value";
                return Xrm.Mobile.offline.retrieveMultipleRecords(EntityNames.Configuration, query);
            }
            else {
                var query = "?$filter=tc_name eq '" + configName + "' &$select=tc_value";
                return Tc.Crm.Scripts.Common.Get(EntitySetNames.Configuration, query);
            }
        }

        function getIncident(caseId) {
            if (isOfflineMode()) {
                var query = "?$select=_tc_bookingid_value,tc_bookingreference,_tc_sourcemarketid_value&$expand=tc_BookingId($select=_tc_sourcemarketid_value),tc_sourcemarketid($select=tc_countryname,tc_iso2code)";
                return Xrm.Mobile.offline.retrieveRecord(EntityNames.Case, caseId, query);
            }
            else {
                var query = "?$select=_tc_bookingid_value,tc_bookingreference,_tc_sourcemarketid_value&$expand=tc_BookingId($select=_tc_sourcemarketid_value),tc_sourcemarketid($select=tc_countryname,tc_iso2code)";
                return Tc.Crm.Scripts.Common.GetById(EntitySetNames.Case, caseId, query);

            }
        }

        function getCase(caseId) {
            if (isOfflineMode()) {
                var query = "?$select=tc_bookingreference,tc_bookingid,tc_bookingtravelamount,tc_durationofstay";
                return Xrm.Mobile.offline.retrieveRecord(EntityNames.Case, caseId, query);
            }
            else {
                var query = "?$select=tc_bookingreference,_tc_bookingid_value,tc_bookingtravelamount,tc_durationofstay";
                return Tc.Crm.Scripts.Common.GetById(EntitySetNames.Case, caseId, query);
            }
        }

        function getCaseType(caseId) {
            if (isOfflineMode()) {
                var query = "?$select=tc_casetypeid";
                return Xrm.Mobile.offline.retrieveRecord(EntityNames.Case, caseId, query);
            }
            else {
                var query = "?$select=_tc_casetypeid_value";
                return Tc.Crm.Scripts.Common.GetById(EntitySetNames.Case, caseId, query);
            }
        }

        function getCaseLine(caseLineId) {
            var query = "?$select=tc_offeredamount";
            if (isOfflineMode()) {
                return Xrm.Mobile.offline.retrieveRecord(EntityNames.CaseLine, caseLineId, query);
            }
            else {
                return Tc.Crm.Scripts.Common.GetById(EntitySetNames.CaseLine, caseLineId, query);
            }
        }

        function getBooking(bookingId) {
            var query = "?$select=tc_duration,tc_travelamount";
            if (isOfflineMode()) {
                return Xrm.Mobile.offline.retrieveRecord(EntityNames.Booking, bookingId, query);
            } else {
                return Tc.Crm.Scripts.Common.GetById(EntitySetNames.Booking, bookingId, query);
            }
        }

        var getSourceMarketIso2Code = function (countryId) {
            var query = "?$select=tc_iso2code";
            if (isOfflineMode()) {
                return Xrm.Mobile.offline.retrieveRecord(EntityNames.Country, countryId, query);
            }
            else {
                return Tc.Crm.Scripts.Common.GetById(EntitySetNames.Country, countryId, query);
            }
        }

        function parseConfigurationValue(configurationResponse) {
            if (configurationResponse == null) return null;
            var result = null;
            if (!isOfflineMode()) {
                if (configurationResponse.value != null && configurationResponse.value.length > 0 && configurationResponse.value[0] != null && configurationResponse.value[0].tc_value != null) {
                    result = parseFloat(configurationResponse.value[0].tc_value);
                }
            } else {
                if (configurationResponse.length > 0 && configurationResponse[0] != null && configurationResponse[0].tc_value != null) {
                    result = parseFloat(configurationResponse[0].tc_value);
                }
            }
            return result;
        }

        function parseCase(caseResponse) {
            if (caseResponse == null) return null;
            var result = {
                travelAmount: caseResponse.tc_bookingtravelamount,
                durationOfStay: caseResponse.tc_durationofstay,
                bookingId: caseResponse._tc_bookingid_value
            };
            if (!isOfflineMode()) {
                result.hasBookingReference = caseResponse.tc_bookingreference;
            } else {
                result.hasBookingReference = caseResponse.tc_bookingreference.toLowerCase() === "true";
            }
            return result;
        }

        function parseCaseType(caseResponse) {
            if (caseResponse == null) return null;
            return caseResponse._tc_casetypeid_value;
        }

        function parseBooking(bookingResponse) {
            if (bookingResponse == null) return null;
            var result = {
                travelAmount: bookingResponse.tc_travelamount,
                duration: bookingResponse.tc_duration
            };
            return result;
        }

        function parseCaseLine(caseLineResponse) {
            if (caseLineResponse == null || caseLineResponse.tc_offeredamount == null) return null;
            return !isOfflineMode() ? caseLineResponse.tc_offeredamount : parseFloat(caseLineResponse.tc_offeredamount);
        }

        function parseSourceMarketIso2Code(countryResponse) {
            if (countryResponse == null) return null;
            return countryResponse.tc_iso2code;
        }

        var caseRetrieved = function (caseResponse) {
            getConfigurationValue(Configuration.LimitContinental).then(function (response) {
                var parsedResponse = getPromiseResponse(response, "Configuration");
                var limit = parseConfigurationValue(parsedResponse);
                if (limit == null) {
                    Xrm.Utility.alertDialog("No value in configuration for " + Configuration.LimitContinental + ". Contact System configuration");
                    return;
                }
                parsedResponse = getPromiseResponse(caseResponse, "Case");
                var incident = parseCase(parsedResponse);
                if (incident == null) return;
                if (incident.hasBookingReference) {
                    getBooking(incident.bookingId).then(
                        function (bookingResponse) {
                            parsedResponse = getPromiseResponse(bookingResponse, "Booking");
                            var booking = parseBooking(parsedResponse);
                            if (booking == null) return;
                            var value = booking.travelAmount;
                            var duration = booking.duration;
                            setContinentalCompensationAmountLimit(value, duration, limit);
                        },
                        function (error) {
                            console.warn("Problem getting booking");
                        }
                    );
                }
                else {
                    var value = incident.travelAmount;
                    var duration = incident.durationOfStay;
                    setContinentalCompensationAmountLimit(value, duration, limit);
                }
            },
                function (error) {
                    console.warn("Problem getting configuration value");
                });
        }

        var setContinentalCompensationAmountLimit = function (bookingValue, duration, maxLimit) {
            if (duration === 0) {
                Xrm.Utility.alertDialog("Cannot calculate compensation amount limit for 0 days duration of holiday");
                return;
            }
            var limit = bookingValue / duration * (maxLimit / 100);
            var attr = Xrm.Page.getAttribute(Attributes.CompensationAmountLimit);
            if (attr != null) {
                attr.setValue(limit);
            }
        }

        var setCompensationAmountLimitUk = function () {
            getConfigurationValue(Configuration.LimitUk).then(function (response) {
                var parsedResponse = getPromiseResponse(response, "Configuration");
                var limit = parseConfigurationValue(parsedResponse);
                if (limit == null) {
                    Xrm.Utility.alertDialog("No value in configuration for " + Configuration.LimitUk + "Contact System configurator");
                    return;
                }
                var attr = Xrm.Page.getAttribute(Attributes.CompensationAmountLimit);
                if (attr != null) {
                    attr.setValue(limit);
                }
            },
                function (error) {
                    console.warn("Problem getting configuration value");
                });
        }

        var setCompensationAmountLimitBasedOnIncidentApprovalLimit = function (configValue) {
            getConfigurationValue(configValue).then(function (response) {
                var parsedResponse = getPromiseResponse(response, "Configuration");
                var limit = parseConfigurationValue(parsedResponse);
                if (limit == null) {
                    Xrm.Utility.alertDialog("No value in configuration for " + configValue + "");
                    return;
                }
                var attr = Xrm.Page.getAttribute(Attributes.CompensationAmountLimit);
                if (attr != null) {
                    attr.setValue(limit);
                }
            },
                function (error) {
                    console.warn("Problem getting configuration value");
                });
        }

        var setCompensationAmountLimitContinental = function () {
            var caseIdAttr = Xrm.Page.getAttribute(Attributes.Case);
            if (caseIdAttr == null) return;
            var value = caseIdAttr.getValue();
            if (value == null || value.length == 0 || value[0] == null) return;
            var caseId = formatEntityId(value[0].id);
            var caseReceivedPromise = getCase(caseId);
            caseReceivedPromise.then(
                caseRetrieved,
                function (error) {
                    console.warn("Problem getting case");
                }
            );
        }

        // Set compensation amount limit based on source market
        var setCompensationAmountLimit = function () {
            var country = Xrm.Page.getAttribute(Attributes.SourceMarket);
            if (country == null) return;
            var value = country.getValue();
            if (value == null || value.length == 0 || value[0] == null) return;
            getSourceMarketIso2Code(formatEntityId(value[0].id)).then(function (response) {
                var parsedResponse = getPromiseResponse(response);
                var iso2Code = parseSourceMarketIso2Code(parsedResponse);
                if (iso2Code != null) {
                    var isUkMarket = iso2Code.toUpperCase() === SourceMarket.UK;
                    if (isUkMarket) {
                        setCompensationAmountLimitUk();
                    } else {
                        setCompensationAmountLimitContinental();
                    }
                }
            },
                function (error) {
                    console.warn("Problem getting source market");
                });
        }

        // Calculate compensation
        var calculateCompensation = function () {
            if (isComplainCaseType == null || !isComplainCaseType) return;
            var promises = [];
            var names = [];
            addCaseLineToCalculation(promises, names, Attributes.CaseLine1);
            addCaseLineToCalculation(promises, names, Attributes.CaseLine2);
            addCaseLineToCalculation(promises, names, Attributes.CaseLine3);
            addCaseLineToCalculation(promises, names, Attributes.CaseLine4);
            Promise.all(promises).then(
                function (response) {
                    if (response == null) {
                        console.warn("Problem getting case lines");
                        return;
                    }
                    var totalAmount = 0;
                    var totalAmountAttr = Xrm.Page.getAttribute(Attributes.Amount);
                    if (totalAmountAttr == null) return;
                    for (var i = 0; i < response.length; i++) {
                        var parsedResponse = getPromiseResponse(response[i]);
                        var caseLineOffer = parseCaseLine(parsedResponse);
                        if (caseLineOffer == null) {
                            totalAmountAttr.controls.forEach(
                                function (control, j) {
                                    control.setNotification("Offered amount for case line " + names[i] + " is not set");
                                });
                            return;
                        }
                        totalAmount = totalAmount + caseLineOffer;
                    }
                    totalAmountAttr.controls.forEach(
                        function (control, j) {
                            control.clearNotification();
                        });
                    totalAmountAttr.setValue(totalAmount);
                    setCompensationAmountLimit();
                },
                function (error) {
                    console.warn("Problem getting case lines");
                });
        }

        // retrieve case type on load
        var initCaseType = function () {
            var caseIdAttr = Xrm.Page.getAttribute(Attributes.Case);
            if (caseIdAttr == null) return;
            var value = caseIdAttr.getValue();
            if (value == null | value.length == 0) return;
            var caseId = formatEntityId(caseIdAttr.getValue()[0].id);
            var caseReceivedPromise = getCaseType(caseId);
            caseReceivedPromise.then(
                function (caseResponse) {
                    var parsedResponse = getPromiseResponse(caseResponse);
                    var caseType = parseCaseType(parsedResponse);
                    if (caseType != null) {
                        isComplainCaseType = caseType.toUpperCase() === CaseType.Complain;
                        isIncidentCaseType = caseType.toUpperCase() === CaseType.Incident;
                        if (isIncidentCaseType == false) return;
                        if (Xrm.Page.ui.getFormType() != FormMode.Create) return;
                        setCompensationAmountLimitBasedOnBookingSourceMarket();
                    }
                },
                function (error) {
                    console.warn("Problem getting case");
                });
        }
        var validateBacsAccountNumber = function () {
            var bacsAccountNumber = Xrm.Page.data.entity.attributes.get(Attributes.BacsAccountNumber);
            if (bacsAccountNumber == null) return;
            var bacsAccountNumberValue = bacsAccountNumber.getValue();
            if (bacsAccountNumberValue == null || bacsAccountNumberValue == "") return;

            var regex = new RegExp("^([0-9]){8}$");
            if (regex.test(bacsAccountNumberValue)) {
                if (Xrm.Page.ui.getFormType() == FormMode.Create) {
                    Xrm.Page.getControl(Attributes.BacsAccountNumber).clearNotification();
                }
                if (Xrm.Page.ui.getFormType() == FormMode.Update) {
                    Xrm.Page.ui.clearFormNotification("BacsAccountNumberNotification");
                }
            }
            else {
                if (Xrm.Page.ui.getFormType() == FormMode.Create) {
                    Xrm.Page.getControl(Attributes.BacsAccountNumber).setNotification("The BACS Account number does not match the required format. The number should contain 8 digits and no spaces or other special characters i.e. 01234567.");
                }
                if (Xrm.Page.ui.getFormType() == FormMode.Update) {
                    Xrm.Page.getControl(Attributes.BacsAccountNumber).clearNotification();
                    Xrm.Page.ui.setFormNotification("The BACS Account number does not match the required format. The number should contain 8 digits and no spaces or other special characters i.e. 01234567.", "WARNING", "BacsAccountNumberNotification");
                }
            }
        }

        var setCompensationAmountLimitBasedOnBookingSourceMarket = function () {

            var caseIdAttr = Xrm.Page.getAttribute(Attributes.Case);
            if (caseIdAttr == null) return;
            var value = caseIdAttr.getValue();
            if (value == null | value.length == 0) return;
            var caseId = formatEntityId(caseIdAttr.getValue()[0].id);

            var sourceMarketReceivedPromise = getIncident(caseId).then(
                            function (incidentResponse) {
                                var incident = JSON.parse(incidentResponse.response);
                                if (incident == null) return;
                                if (incident.tc_sourcemarketid == null && incident.tc_BookingId == null) return;
                                if (incident.tc_sourcemarketid != null)
                                    if (incident.tc_sourcemarketid.tc_iso2code != null && incident.tc_sourcemarketid.tc_iso2code != "")
                                        if (incident.tc_sourcemarketid.tc_iso2code == SourceMarket.UK)
                                            setCompensationAmountLimitBasedOnIncidentApprovalLimit(Configuration.IncidentApprovalLimitUk);
                                        else
                                            setCompensationAmountLimitBasedOnIncidentApprovalLimit(Configuration.IncidentApprovalLimitNonUk);

                                if (incident.tc_BookingId == null) return;
                                if (incident.tc_BookingId._tc_sourcemarketid_value != null && incident.tc_BookingId._tc_sourcemarketid_value != "") {
                                    var sourceMarketId = incident.tc_BookingId._tc_sourcemarketid_value;
                                    if (sourceMarketId) {
                                        getSourceMarketIso2Code(formatEntityId(sourceMarketId)).then(
                                          function (sourceMarketIso2CodeResponse) {
                                              var sourceMarket = JSON.parse(sourceMarketIso2CodeResponse.response);
                                              if (sourceMarket.tc_iso2code != null && sourceMarket.tc_iso2code != "");
                                              if (sourceMarket.tc_iso2code == SourceMarket.UK)
                                                  setCompensationAmountLimitBasedOnIncidentApprovalLimit(Configuration.IncidentApprovalLimitUk);
                                              else
                                                  setCompensationAmountLimitBasedOnIncidentApprovalLimit(Configuration.IncidentApprovalLimitNonUk);
                                          },
                        function (error) {
                            console.warn("Problem getting booking");
                        }
                    );
                                    }
                                }
                            }).catch(function (err) {
                                throw new Error("Problem in setting Compensation AmountLimit based on Booking SourceMarket");
                            });

        }

        var isProcessedStatusReason = function () {
            var compensationStatusReasonAttr = Xrm.Page.getAttribute(Attributes.StatusCode);
            if (compensationStatusReasonAttr == null)
                return false;

            var value = compensationStatusReasonAttr.getValue();
            if (value === StatusReason.PaymentProcessed || value === StatusReason.ComebackPaymentProcessed) {
                return true;
            }
            return false;
        }

        var onReopenCompensationRibbonButtonEnabled = function () {
            debugger;
            if (!isProcessedStatusReason()) {
                reopenCompensationButtonRibbonButtonEnabled = false;
                return;
            }
            var userRole = window.parent.Xrm.Page.context.getUserRoles();
            if (userRole == null || userRole === "") {
                reopenCompensationButtonRibbonButtonEnabled = false;
                return;
            }

            var promises = [];
            var query = "?$select=name";
            for(var i = 0; i < userRole.length; i++) {
                var promise = Tc.Crm.Scripts.Common.GetById(EntitySetNames.SecurityRole, userRole[i], query);
                promises.push(promise);
            }
            Promise.all(promises).then(
                function (securityRoleNameResponse) {
                    if (securityRoleNameResponse == null) {
                        console.warn("Problem getting security roles");
                        return;
                    }
                    debugger;
                    for (var i = 0; i < securityRoleNameResponse.length; i++) {
                        var role = JSON.parse(securityRoleNameResponse[i].response);
                        if (role == null || role == "" || role == "undefined") continue;
                        if (role.name === SecurityRole.SystemAdministrator || role.name === SecurityRole.CustomerRelationsAgent || 
                            role.name === SecurityRole.CustomerRelationsManager || role.name === SecurityRole.SystemMaintenance) {
                            reopenCompensationButtonRibbonButtonEnabled = true;
                            break;
                        }
                    }
                    if (reopenCompensationButtonRibbonButtonEnabled == null) reopenCompensationButtonRibbonButtonEnabled = false;
                    Xrm.Page.ui.refreshRibbon();
                },
                function (error) {
                    console.warn("Problem getting case lines");
                    reopenCompensationButtonRibbonButtonEnabled = false;
                });
        }

        var getCurrencyValue = function (currencyField) {

            var currencyValue = Xrm.Page.data.entity.attributes.get(currencyField).getValue();

            // Checking for empty and strings
            if (isNaN(currencyValue) || currencyValue == "")
                currencyValue = 0;
            return currencyValue;

        }

        // Validate Total Offer Amount against sum of all compensations
        var validateTotalOfferAmount = function () {

            var totalOffer = getCurrencyValue(Attributes.Amount);

            // Get the individual compensation values
            var chapsAmount = getCurrencyValue(Attributes.ChapsAmount);
            var cashAmount = getCurrencyValue(Attributes.CashAmount);
            var nonFinAmount = getCurrencyValue(Attributes.NonFinAmount);
            var bacsAmount = getCurrencyValue(Attributes.BacsAmount);
            var voucherAmount = getCurrencyValue(Attributes.VoucherAmount);

            // Comparing values
            if ((chapsAmount + cashAmount + nonFinAmount + bacsAmount + voucherAmount) > totalOffer) {
                // Throw error message for total offer field
                Xrm.Page.getControl(Attributes.Amount).setNotification("The total of individual compensation lines does match the Total Offered.");

            }
            else
                Xrm.Page.getControl(Attributes.Amount).clearNotification();
        }
        ///Mark need to confirm whether we uncomment this code or not on CRM-3098
        //var onCancelRibbonButtonClick = function () {

        //    Xrm.Utility.confirmDialog("Do  you want to Cancel the selected Compensation? You Can't activate it later.", cancelCompensation, null);
        //}
        var cancelCompensation = function () {
            var compensationEntityObj = {};
            var compensationRecId = Xrm.Page.data.entity.getId();
            if (compensationRecId === null || compensationRecId === undefined) return;
            var compensationGuid = formatEntityId(compensationRecId);
            compensationEntityObj[Attributes.StateCode] = 1;
            compensationEntityObj[Attributes.StatusCode] = 2;

            var compensationRecievedPromise = deactivateCompensation(compensationGuid, compensationEntityObj).then(function (compensationResponse) {
                if (compensationResponse.status === 204) {
                    Xrm.Page.data.refresh(false);
                } else {
                    Xrm.Utility.alertDialog(compensationResponse.statusText);
                }

            }).catch(function (err) {
                throw new Error("Problem in updating Compensation record ");
            });

        }
        var deactivateCompensation = function (compensationGuid, compensationEntityObj) {
            if (isOfflineMode()) {
                return Xrm.Mobile.offline.updateRecord(EntityNames.Compensation, compensationGuid, compensationEntityObj);
            } else {
                return Tc.Crm.Scripts.Common.Update(EntitySetNames.Compensation, compensationGuid, compensationEntityObj);
            }
        }

        var onReopenCompensationButtonClick = function () {
            Xrm.Utility.confirmDialog("Compensation has previously been issued on this case. Click Ok to proceed.",
                function () {
                    var compensationStatusReasonAttr = Xrm.Page.getAttribute(Attributes.StatusCode);
                    if (compensationStatusReasonAttr != null) {
                        compensationStatusReasonAttr.setValue(StatusReason.ComebackInProgress);
                        reopenCompensationButtonRibbonButtonEnabled = false;
                        Xrm.Page.ui.refreshRibbon();
                    }
                });
        }

        // public
        return {
            OnLoad: function () {
                Tc.Crm.Scripts.Library.Compensation.SetDefaultsOnCreate();
                // init case type for compensation calculation
                initCaseType();
            },
            OnSave: function (context) {
                var isValid = Tc.Crm.Scripts.Utils.Validation.ValidateGdprCompliance(context);
                // uncomment in case of additional save actions
                if (isValid) {
                    //validate total offer amount against sum of all compensations
                    validateTotalOfferAmount();
                }
            },
            OnChangeBacsAccountNumber: function () {
                validateBacsAccountNumber();
            },
            OnClickCancelRibbonButton: function () {
                cancelCompensation();
            },
            OnAccountSortCodeChanged: function (context) {
                if (context === null) {
                    console.log("Tc.Crm.Scripts.Events.Compensation.OnAccountSortCodeChanged should be configured to pass execution context");
                    return;
                }
                formatValue6DigitsWithHyphen(context);
            },
            OnCompensationAmountChange: function () {
                Xrm.Page.getControl(Attributes.Amount).clearNotification();
            },
            OnCompensationCalculate: calculateCompensation,
            OnReopenCompensationButtonClick: onReopenCompensationButtonClick,
            EnableReopenCompensationButton: function() {
                if (!isOfflineMode()) {
                    if (reopenCompensationButtonRibbonButtonEnabled == null) {
                        onReopenCompensationRibbonButtonEnabled();
                    }
                    return reopenCompensationButtonRibbonButtonEnabled;
                } else return false;
            }
        };
    })();

    // end script
    console.log('loaded events.compensation');
});