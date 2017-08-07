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
scriptLoader.load("Tc.Crm.Scripts.Library.CaseLine", ["Tc.Crm.Scripts.Common"], function () {
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
if (typeof (Tc.Crm.Scripts.Library) === "undefined") {
    Tc.Crm.Scripts.Library = {
        __namespace: true
    };
}
Tc.Crm.Scripts.Library.CaseLine = (function () {
    "use strict";

    var FORM_MODE_CREATE = 1;
    var FORM_MODE_UPDATE = 2;
    var FORM_MODE_QUICK_CREATE = 5;
    var CASE_TYPE_COMPLAIN = "{478C99E9-93E4-E611-8109-1458D041F8E8}";
    var CLIENT_STATE_OFFLINE = "Offline";

    var Configuration = {
        Adjustment: "Tc.Compensation.Adjustment",
        MaximumTotalLimitToBePaid: "Tc.Compensation.MaximumTotalLimitToBePaid"
    }

    var SOURCE_MARKET_UK = "GB";

    var SourceMarket = {
        UK: 950000000,
        Continental: 950000001
    }

    var Level = {
        High: 950000000,
        Medium: 950000001,
        Low: 950000002,
        H: 950000002,
        M: 950000001,
        L: 950000000
    }

    var ProductType = {
        FlightOnly: 950000000,
        HotelOnly: 950000001,
        PackageHoliday: 950000002
    }

    var AccommodationPercentage = {
        FlightOnly: 0,
        HotelOnly: 100,
        PackageHoliday: 40
    }

    var Attributes = {
        Name: "tc_name",
        Case: "tc_caseid",
        CaseType: "tc_casetypeid",
        CaseCategory1: "tc_categorylevel1id",
        CaseCategory2: "tc_casecategory2id",
        CaseCategory3: "tc_category3id",
        ProductType: "tc_producttype",
        TotalBookingValue: "tc_totalbookingvalue",
        AffectedDays: "tc_numberdaysaffected",
        TotalDays: "tc_durationofstay",
        ProposedCompensation: "tc_proposedcompensationvalue",
        MaxProposedCompensation: "tc_proposedmaxcompensationvalue",
        CostWrongRoomType: "tc_costwrongroomtype",
        Severity: "tc_severity",
        Impact: "tc_impact",
        BuildingWorkGrade: "tc_buildingworkgrade",
        OfferedAmount: "tc_offeredamount",
        Comments: "tc_impactseveritycomments"
    }

    var TabsAndSections = {
        CompensationCalculator: "tab_compensationCalculator",
        BuildingWork: "tab_CompensationCalculator_BuildingWork",
        RoomType: "tab_CompensationCalculator_RoomType"
    }

    var EntityNames = {
        CaseCategory: "tc_casecategory",
        Case: "incident",
        Booking: "tc_booking",
        Configuration: "tc_configuration",
        CompensationCalculationMatrix: "tc_compensationcalculationmatrix",
        CompensationCalculationMatrixLine: "tc_compensationcalculationline"
    }

    var EntitySetNames = {
        Case: "incidents",
        Booking: "tc_bookings",
        Configuration: "tc_configurations",
        CompensationCalculationMatrix: "tc_compensationcalculationmatrixes",
        CompensationCalculationMatrixLine: "tc_compensationcalculationlines"
    }

    var isUkMarket = null;
    var isBuildingWork = false;
    var isWrongRoomType = false;
    var costWrongRoomTypeValue = null;
    var buildingWorkValue = null;
    var severity = null;
    var impact = null;

    // Check if case line falls into building work complensation calculation
    // If UK and specific case category 1,2,3
    var isBuildingWorkCaseLine = function () {
        var categoryAttr = Xrm.Page.getAttribute(Attributes.CaseCategory1).getValue();
        var category1Id = categoryAttr != null && categoryAttr.length > 0 ? categoryAttr[0].id.toLowerCase() : null;
        categoryAttr = Xrm.Page.getAttribute(Attributes.CaseCategory2).getValue();
        var category2Id = categoryAttr != null && categoryAttr.length > 0 ? categoryAttr[0].id.toLowerCase() : null;
        categoryAttr = Xrm.Page.getAttribute(Attributes.CaseCategory3).getValue();
        var category3Id = categoryAttr != null && categoryAttr.length > 0 ? categoryAttr[0].id.toLowerCase() : null;
        return isUkMarket != null && isUkMarket &&
        // Accommodation	General Standards	Accom-Building/Refurbishment Work Property
        ((category1Id === "{2505f04f-f3f2-e611-8107-3863bb34fa70}" && category2Id === "{6705f04f-f3f2-e611-8107-3863bb34fa70}" && category3Id === "{9906f04f-f3f2-e611-8107-3863bb34fa70}") ||
        // Pre- Departure Services	Notification - Other	Pre Dept-Notification Building Work
        (category1Id === "{3905f04f-f3f2-e611-8107-3863bb34fa70}" && category2Id === "{c305f04f-f3f2-e611-8107-3863bb34fa70}" && category3Id === "{7f07f04f-f3f2-e611-8107-3863bb34fa70}") ||
        // Resort	Local Building Work	Resort-Pre Dept Local Building Work
        (category1Id === "{3b05f04f-f3f2-e611-8107-3863bb34fa70}" && category2Id === "{cd05f04f-f3f2-e611-8107-3863bb34fa70}" && category3Id === "{9907f04f-f3f2-e611-8107-3863bb34fa70}"));
    }

    // Check if case line falls into wrong room type complensation calculation
    // If UK and specific case category 1,2,3
    var isWrongRoomTypeCaseLine = function () {
        var categoryAttr = Xrm.Page.getAttribute(Attributes.CaseCategory1).getValue();
        var category1Id = categoryAttr != null && categoryAttr.length > 0 ? categoryAttr[0].id.toLowerCase() : null;
        categoryAttr = Xrm.Page.getAttribute(Attributes.CaseCategory2).getValue();
        var category2Id = categoryAttr != null && categoryAttr.length > 0 ? categoryAttr[0].id.toLowerCase() : null;
        categoryAttr = Xrm.Page.getAttribute(Attributes.CaseCategory3).getValue();
        var category3Id = categoryAttr != null && categoryAttr.length > 0 ? categoryAttr[0].id.toLowerCase() : null;
        // Accommodation	Room Allocation	Accom-Room Wrong Room Type
        return isUkMarket != null && isUkMarket && (category1Id === "{2505f04f-f3f2-e611-8107-3863bb34fa70}" && category2Id === "{6d05f04f-f3f2-e611-8107-3863bb34fa70}" && category3Id === "{c106f04f-f3f2-e611-8107-3863bb34fa70}");
    }

    function formatEntityId(id) {
        return id !== null ? id.replace("{", "").replace("}", "") : null;
    }

    function IsOfflineMode() {
        return Xrm.Page.context.client.getClientState() === CLIENT_STATE_OFFLINE
    }

    function getPromiseResponse(promiseResponse, entity) {
        if (promiseResponse == null) return null;
        if (IsOfflineMode()) {
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

    // get related case data
    function getCase(caseId) {
        if (IsOfflineMode()) {
            var query = "?$select=tc_bookingreference,tc_bookingid,tc_producttype,tc_bookingtravelamount,tc_durationofstay&$expand=tc_sourcemarketid($select=tc_iso2code)";
            return Xrm.Mobile.offline.retrieveRecord(EntityNames.Case, caseId, query);
        }
        else {
            var query = "?$select=tc_bookingreference,_tc_bookingid_value,tc_producttype,tc_bookingtravelamount,tc_durationofstay&$expand=tc_sourcemarketid($select=tc_iso2code)";
            return Tc.Crm.Scripts.Common.GetById(EntitySetNames.Case, caseId, query);
        }
    }

    // get related booking data
    function getBooking(bookingId) {
        var query = "?$select=tc_travelamount,tc_duration&$expand=tc_SourceMarketId($select=tc_iso2code)";
        if (IsOfflineMode()) {
            return Xrm.Mobile.offline.retrieveRecord(EntityNames.Booking, bookingId, query);
        } else {
            return Tc.Crm.Scripts.Common.GetById(EntitySetNames.Booking, bookingId, query);
        }
    }

    // retrieve configuration value
    var getConfigurationValue = function (configName) {
        var query = "?$filter=tc_name eq '" + configName + "' &$select=tc_value";
        if (IsOfflineMode()) {
            return Xrm.Mobile.offline.retrieveMultipleRecords(EntityNames.Configuration, query);
        } else {
            return Tc.Crm.Scripts.Common.Get(EntitySetNames.Configuration, query);
        }
    }

    // Search configured compensation calculation matrix for case line per market
    var getMatrixId = function () {
        var category1Id = Xrm.Page.getAttribute(Attributes.CaseCategory1).getValue()[0].id;
        var category2Id = Xrm.Page.getAttribute(Attributes.CaseCategory2).getValue()[0].id;
        var category3Id = Xrm.Page.getAttribute(Attributes.CaseCategory3).getValue()[0].id;
        if (IsOfflineMode()) {
            var query = "?$filter=tc_sourcemarket eq " + (isUkMarket ? SourceMarket.UK : SourceMarket.Continental) + " and tc_category1 eq " + formatEntityId(category1Id) + " and tc_category2 eq " + formatEntityId(category2Id) + " and tc_category3 eq " + formatEntityId(category3Id) + "&$select=tc_compensationcalculationmatrixid";
            return Xrm.Mobile.offline.retrieveMultipleRecords(EntityNames.CompensationCalculationMatrix, query);
        } else {
            var query = "?$filter=tc_sourcemarket eq " + (isUkMarket ? SourceMarket.UK : SourceMarket.Continental) + " and _tc_category1_value eq " + formatEntityId(category1Id) + " and _tc_category2_value eq " + formatEntityId(category2Id) + " and _tc_category3_value eq " + formatEntityId(category3Id) + "&$select=tc_compensationcalculationmatrixid";
            return Tc.Crm.Scripts.Common.Get(EntitySetNames.CompensationCalculationMatrix, query);
        }
    }

    // Search configured compensation calculation matrix line for matrix with proper impact\range\building work
    // search for severity and impact for uk
    // search by building work for uk building
    // search for impact for contitental
    var getImpactAndRange = function (matrixId) {
        var query = !IsOfflineMode() ? "?$filter=_tc_compensationmatrix_value eq " + matrixId : "?$filter=tc_compensationmatrix eq " + matrixId;
        if (isUkMarket) {
            if (!isBuildingWork) {
                query = query + " and tc_impactlevel eq " + impact + " and tc_impactseverity eq " + severity;
            } else {
                query = query + " and tc_buildingworkgrade eq " + buildingWorkValue;
            }
        } else {
            query = query + " and tc_impactlevel eq " + impact;
        }

        query = query + "&$select=tc_impactpercentage,tc_rangepercentage";

        if (IsOfflineMode()) {
            return Xrm.Mobile.offline.retrieveMultipleRecords(EntityNames.CompensationCalculationMatrixLine, query);
        } else {
            return Tc.Crm.Scripts.Common.Get(EntitySetNames.CompensationCalculationMatrixLine, query);
        }
    }

    // parse case request response basing on offline\online
    function parseCase(caseResponse) {
        if (caseResponse == null) return null;
        var result = {
            productType: caseResponse.tc_producttype,
            travelAmount: caseResponse.tc_bookingtravelamount,
            durationOfStay: caseResponse.tc_durationofstay,
            bookingId: caseResponse._tc_bookingid_value
        };
        if (!IsOfflineMode()) {
            result.hasBookingReference = caseResponse.tc_bookingreference;
            result.sourceMarketIso2Code = caseResponse.tc_sourcemarketid != null ? caseResponse.tc_sourcemarketid.tc_iso2code : null;
        } else {
            result.hasBookingReference = caseResponse.tc_bookingreference.toLowerCase() === "true";
            result.sourceMarketIso2Code = caseResponse.tc_sourcemarketid != null && caseResponse.tc_sourcemarketid.length > 0 ? caseResponse.tc_sourcemarketid[0].tc_iso2code : null;
        }
        return result;
    }

    // parse booking request response basing on offline\online
    function parseBooking(bookingResponse) {
        if (bookingResponse == null) return null;
        var result = {
            travelAmount: bookingResponse.tc_travelamount,
            duration: bookingResponse.tc_duration
        };
        if (!IsOfflineMode()) {
            result.sourceMarketIso2Code = bookingResponse.tc_SourceMarketId != null ? bookingResponse.tc_SourceMarketId.tc_iso2code : null;
        } else {
            result.sourceMarketIso2Code = bookingResponse.tc_SourceMarketId != null && bookingResponse.tc_SourceMarketId.length > 0 ? bookingResponse.tc_SourceMarketId[0].tc_iso2code : null;
        }
        return result;
    }

    // parse matrix request response basing on offline\online
    function parseMatrix(matrixResponse) {
        if (matrixResponse == null) return null;
        var result = null;
        if (!IsOfflineMode()) {
            if (matrixResponse.value != null && matrixResponse.value.length > 0 && matrixResponse.value[0] != null) {
                result = matrixResponse.value[0].tc_compensationcalculationmatrixid;
            }
        } else {
            if (matrixResponse.length > 0 && matrixResponse[0] != null) {
                result = matrixResponse[0].tc_compensationcalculationmatrixid;
            }
        }
        return result;
    }

    // parse compensation line request response basing on offline\online
    function parseCompensationLine(lineResponse) {
        if (lineResponse == null) return null;
        var result = null;
        if (!IsOfflineMode()) {
            if (lineResponse.value != null && lineResponse.value.length > 0 && lineResponse.value[0] != null) {
                result = {
                    impactPercentage: lineResponse.value[0].tc_impactpercentage,
                    rangePercentage: lineResponse.value[0].tc_rangepercentage
                }
            }
        } else {
            if (lineResponse.length > 0 && lineResponse[0] != null) {
                result = {
                    impactPercentage: parseFloat(lineResponse[0].tc_impactpercentage),
                    rangePercentage: parseFloat(lineResponse[0].tc_rangepercentage)
                }
            }
        }
        return result;
    }

    // parse configuration request response basing on offline\online
    function parseConfigurationValue(configurationResponse) {
        if (configurationResponse == null) return null;
        var result = null;
        if (!IsOfflineMode()) {
            if (configurationResponse.value != null && configurationResponse.value.length > 0 && configurationResponse.value[0] != null) {
                result = parseFloat(configurationResponse.value[0].tc_value);
            }
        } else {
            if (configurationResponse.length > 0) {
                result = parseFloat(configurationResponse[0].tc_value);
            }
        }
        return result;
    }

    // Calculate wrong room cost compensation
    var calculateWrongRoomCost = function () {
        var cost = Xrm.Page.getAttribute(Attributes.CostWrongRoomType).getValue();
        var daysAffected = Xrm.Page.getAttribute(Attributes.AffectedDays).getValue();
        var value = cost * daysAffected;

        Xrm.Page.getAttribute(Attributes.ProposedCompensation).setValue(value);
        Xrm.Page.getAttribute(Attributes.MaxProposedCompensation).setValue(value);
        Xrm.Page.getControl(Attributes.OfferedAmount).setFocus();
    }

    // Calculate default formula compensation for UK
    var calculateDefaultFormulaUk = function (impact, range, adjustment, maximumTotalLimitToBePaid) {
        // building work does not use range
        if (isBuildingWork) {
            range = 0;
        }
        // Accommodation Costs = Booking.tc_travelamount * Accomodation Split
        var productType = Xrm.Page.getAttribute(Attributes.ProductType).getValue();
        var bookingValue = Xrm.Page.getAttribute(Attributes.TotalBookingValue).getValue();
        var accommodationPercentage = 0;
        switch (productType) {
            case ProductType.HotelOnly:
                accommodationPercentage = AccommodationPercentage.HotelOnly;
                break;
            case ProductType.PackageHoliday:
                accommodationPercentage = AccommodationPercentage.PackageHoliday;
                break;
            case ProductType.FlightOnly:
                accommodationPercentage = AccommodationPercentage.FlightOnly;
                break;
        }
        var accommodationCost = bookingValue * (accommodationPercentage / 100);
        // % Holiday = Days Affected / Total Days
        var daysAffected = Xrm.Page.getAttribute(Attributes.AffectedDays).getValue();
        var totalDays = Xrm.Page.getAttribute(Attributes.TotalDays).getValue();
        if (totalDays === 0) {
            Xrm.Utility.alertDialog("Cannot calculate compensation for 0 days duration of holiday");
            return;
        }
        var holidayPercentage = daysAffected / totalDays;
        // Value for % Holidays = Accommodation Costs * % Holiday
        var holidayPercentageValue = accommodationCost * holidayPercentage;
        // Compensation Value = Value for % Holidays * Impact %
        var compensationValue = holidayPercentageValue * (impact / 100);
        // Range Value = Value for % Holidays * Range %
        var rangeValue = holidayPercentageValue * (range / 100);
        var limitCompensation = accommodationCost * (maximumTotalLimitToBePaid / 100);
        // Proposed Compensation = MIN (Compensation Value + (Compensation Value * Adjustment), Limit Compensation)
        var proposedCompensation = Math.min(compensationValue * (1 + adjustment / 100), limitCompensation);
        // Maximum Compensation = MIN ((Compensation Value + Range Value) + ((Compensation Value + Range Value) * Adjustment), Limit Compensation)
        var maximumCompensation = Math.min((compensationValue + rangeValue) * (1 + adjustment / 100), limitCompensation);

        Xrm.Page.getAttribute(Attributes.ProposedCompensation).setValue(proposedCompensation);
        Xrm.Page.getAttribute(Attributes.MaxProposedCompensation).setValue(maximumCompensation);
        Xrm.Page.getControl(Attributes.OfferedAmount).setFocus();
    }

    // Show notification that no compensation calculation occured
    var addNotificationNoAutomaticCalculation = function () {
        var control = Xrm.Page.getControl(Attributes.OfferedAmount);
        control.addNotification({
            messages: ['No automatic calculation for this case line'],
            notificationLevel: 'RECOMMENDATION'
        });
        control.setFocus();
    }

    // Calculate compensation for UK market
    var calculateCompensationUk = function () {
        // specific calculation for wrong room cost
        if (isWrongRoomType) {
            calculateWrongRoomCost();
            return;
        }
        // get configuration entry
        getMatrixId().then(
            function (matrixResponse) {
                // parse from response, if null - show message as no calculation should be done
                var response = getPromiseResponse(matrixResponse, "Compensation Calculation Matrix");
                var matrixId = parseMatrix(response);
                if (matrixId == null) {
                    addNotificationNoAutomaticCalculation();
                    return;
                }
                // search compensation matrix line for severity/impact or building work
                Promise.all([getImpactAndRange(matrixId), getConfigurationValue(Configuration.Adjustment), getConfigurationValue(Configuration.MaximumTotalLimitToBePaid)]).then(function (allResponses) {
                    if (allResponses == null || allResponses.length < 3) {
                        console.warn("Error getting compensation matrix values or retrieving configuration");
                        return;
                    }
                    response = getPromiseResponse(allResponses[0], "Compensation Calculation Matrix Line"); // parse matrix
                    var matrixLine = parseCompensationLine(response);
                    if (matrixLine == null) {
                        Xrm.Utility.alertDialog("No compensation matrix line impact+severity or building work is present in configuration. Contact System configurator");
                        return;
                    }
                    response = getPromiseResponse(allResponses[1], "Configuration"); // parse adjustment
                    var adjustment = parseConfigurationValue(response);
                    if (adjustment == null) {
                        Xrm.Utility.alertDialog("No value in configuration for " + Configuration.Adjustment + ". Contact System configurator");
                        return;
                    }
                    response = getPromiseResponse(allResponses[2], "Configuration"); // parse maximumTotalLimitToBePaid                        
                    var maximumTotalLimitToBePaid = parseConfigurationValue(response);
                    if (maximumTotalLimitToBePaid == null) {
                        Xrm.Utility.alertDialog("No value in configuration for " + Configuration.MaximumTotalLimitToBePaid + ". Contact System configurator");
                        return;
                    }
                    calculateDefaultFormulaUk(matrixLine.impactPercentage, matrixLine.rangePercentage, adjustment, maximumTotalLimitToBePaid);
                },
                function (error) {
                    console.warn("Error getting compensation matrix values or retrieving configuration");
                });
            },
            function (error) {
                console.warn("Error getting compensation matrix");
            }
        );
    }

    // Calculate compensation for non-UK (continental) markets
    var calculateCompensationContinental = function () {
        // get configuration entry
        getMatrixId().then(
            function (matrixResponse) {
                // parse from response, if null - show message as no calculation should be done
                var response = getPromiseResponse(matrixResponse, "Compensation Calculation Matrix");
                var matrixId = parseMatrix(response);
                if (matrixId == null) {
                    addNotificationNoAutomaticCalculation();
                    return;
                }
                // search compensation matrix line for impact
                getImpactAndRange(matrixId).then(
                    function (caseLineResponse) {
                        response = getPromiseResponse(caseLineResponse, "Compensation Calculation Matrix Line"); // parse matrix
                        var matrixLine = parseCompensationLine(response);
                        if (matrixLine == null) {
                            Xrm.Utility.alertDialog("No compensation matrix line impact+severity or building work is present in configuration. Contact System configurator");
                            return;
                        }
                        // Proposed Compensation = (Travel Amount / Total Days) * Days Affected * Impact %
                        var travelAmount = Xrm.Page.getAttribute(Attributes.TotalBookingValue).getValue();
                        var totalDays = Xrm.Page.getAttribute(Attributes.TotalDays).getValue();
                        if (totalDays === 0) {
                            Xrm.Utility.alertDialog("Cannot calculate compensation for 0 days duration of holiday");
                            return;
                        }
                        var daysAffected = Xrm.Page.getAttribute(Attributes.AffectedDays).getValue();
                        var value = travelAmount * daysAffected / totalDays * (matrixLine.impactPercentage / 100);

                        Xrm.Page.getAttribute(Attributes.ProposedCompensation).setValue(value);
                        Xrm.Page.getAttribute(Attributes.MaxProposedCompensation).setValue(value);
                        Xrm.Page.getControl(Attributes.OfferedAmount).setFocus();
                    },
                    function (error) {
                        console.warn("Error getting compensation matrix values or retrieving configuration");
                    }
                );
            },
            function (error) {
                console.warn("Error getting compensation matrix");
            }
        );
    }

    // Validate mandatory field. Execute action on field if needed
    var validateMandatoryField = function (attribute, errorMessage, setter) {
        var attribute = Xrm.Page.getAttribute(attribute);
        if (attribute == null) return false;
        var value = attribute.getValue();
        var isValid = value !== null;
        if (isValid && (setter != null)) {
            setter(value);
        }
        attribute.controls.forEach(
        function (control, i) {
            if (isValid) {
                control.clearNotification();
            } else {
                control.setNotification(errorMessage);
            }
        });
        return isValid;
    }

    // Check if case line is valid for compensation calculation: all mandatory fields are filled in
    var isValidForCalculation = function () {
        // isUkMarket is initialized on form load. Don't process if has not been initialized yet
        if (isUkMarket == null) {
            return false;
        }
        var isValid =
            validateMandatoryField(Attributes.CaseCategory1, "Please enter case category 1") &&
            validateMandatoryField(Attributes.CaseCategory2, "Please enter case category 2") &&
            validateMandatoryField(Attributes.CaseCategory3, "Please enter case category 3") &&
            validateMandatoryField(Attributes.TotalBookingValue, "Please enter total booking value") &&
            validateMandatoryField(Attributes.TotalDays, "Please enter total days") &&
            validateMandatoryField(Attributes.AffectedDays, "Please enter affected days") &&
            validateMandatoryField(Attributes.ProductType, "Please enter product type") &&
            validateMandatoryField(Attributes.Impact, "Please enter impact", function (value) {
                // set impact for further calculation
                switch (value) {
                    case Level.High:
                        impact = Level.H;
                        break;
                    case Level.Medium:
                        impact = Level.M;
                        break;
                    case Level.Low:
                        impact = Level.L;
                }
            }) &&
            validateMandatoryField(Attributes.Severity, "Please enter severity", function (value) {
                // set secerity for further calculation
                switch (value) {
                    case Level.High:
                        severity = Level.H;
                        break;
                    case Level.Medium:
                        severity = Level.M;
                        break;
                    case Level.Low:
                        severity = Level.L;
                }
            }) &&
            validateMandatoryField(Attributes.Comments, "Please enter comments");
        isBuildingWork = isBuildingWorkCaseLine();
        if (isBuildingWork) {
            var isValid = isValid && validateMandatoryField(Attributes.BuildingWorkGrade, "Please enter building work grade", function (value) { buildingWorkValue = value; });
        }
        isWrongRoomType = isWrongRoomTypeCaseLine();
        if (isWrongRoomType) {
            var isValid = isValid && validateMandatoryField(Attributes.CostWrongRoomType, "Please enter cost wrong room type", function (value) {
                isWrongRoomType = value > 0;
                costWrongRoomTypeValue = value;
            });
        }
        return isValid;
    }

    // Get source market from case or booking
    // Populate product type, booking value and duration on create
    var loadSourceMarketAndSetDefaults = function () {
        var attr = Xrm.Page.getAttribute(Attributes.Case);
        if (attr == null) return;
        var value = attr.getValue();
        if (value == null || value.length == 0) return;

        var caseId = formatEntityId(value[0].id);
        var caseReceivedPromise = getCase(caseId);
        caseReceivedPromise.then(
            caseRetrieved,
            function (error) {
                console.warn("Problem getting case.");
            }
        );
    }

    // Calculation compensation for case line
    var calculateCompensation = function () {
        // don't do calculations if fields are not on the form
        var attr = Xrm.Page.getAttribute(Attributes.ProposedCompensation);
        if (attr == null) return;
        attr = Xrm.Page.getAttribute(Attributes.MaxProposedCompensation);
        if (attr == null) return;
        attr = Xrm.Page.getAttribute(Attributes.OfferedAmount);
        if (attr == null) return;
        // clear validation errors, if any
        Xrm.Page.getAttribute(Attributes.OfferedAmount).controls.forEach(
            function (control, i) {
                control.clearNotification();
            });
        if (!isValidForCalculation())
            return;
        if (isUkMarket) {
            calculateCompensationUk();
        } else {
            calculateCompensationContinental();
        }
    }

    var setDefaultsOnCreate = function (productType, travelAmount, duration) {
        var attr = Xrm.Page.getAttribute(Attributes.ProductType);
        if (attr !== null) {
            attr.setValue(productType);
        }
        attr = Xrm.Page.getAttribute(Attributes.TotalBookingValue);
        if (attr !== null) {
            attr.setValue(travelAmount);
        }
        attr = Xrm.Page.getAttribute(Attributes.TotalDays);
        if (attr != null) {
            attr.setValue(duration);
        }
    }

    // Get source market
    var caseRetrieved = function (caseResponse) {
        var parsedResponse = getPromiseResponse(caseResponse, "Case");
        var incident = parseCase(parsedResponse);
        if (incident == null) return;
        var isCreate = (Xrm.Page.ui.getFormType() === FORM_MODE_CREATE) || (Xrm.Page.ui.getFormType() === FORM_MODE_QUICK_CREATE);
        if (incident.hasBookingReference) {
            // Get source market from booking
            getBooking(incident.bookingId).then(
                function (bookingResponse) {
                    parsedResponse = getPromiseResponse(bookingResponse, "Booking");
                    var booking = parseBooking(parsedResponse);
                    if (booking == null) return;
                    if (booking.sourceMarketIso2Code != null) {
                        isUkMarket = booking.sourceMarketIso2Code.toUpperCase() === SOURCE_MARKET_UK;
                    }
                    showHideCompensationCalculator(false);
                    // set defaults on create                    
                    if (isCreate) {
                        setDefaultsOnCreate(incident.productType, booking.travelAmount, booking.duration);
                    }
                },
                function (error) {
                    console.warn("Problem getting booking");
                });
        }
        else {
            if (incident.sourceMarketIso2Code != null) {
                isUkMarket = incident.sourceMarketIso2Code.toUpperCase() === SOURCE_MARKET_UK;
            }
            showHideCompensationCalculator(false);
            // set defaults on create
            if (isCreate) {
                setDefaultsOnCreate(incident.productType, incident.travelAmount, incident.durationOfStay);
            }
        }
    }

    var shouldShowCompensationCalculator = function () {
        // don't show if source market is not initialized
        if (isUkMarket == null) return false;
        // don't proceed if tab is not on the form
        var tab = Xrm.Page.ui.tabs.get(TabsAndSections.CompensationCalculator);
        if (tab == null) return false;
        // only for case type complain
        var caseTypeId = Xrm.Page.getAttribute(Attributes.CaseType).getValue();
        if (caseTypeId == null || caseTypeId[0].id !== CASE_TYPE_COMPLAIN) {
            return false;
        }
        // when case category 3 is selected
        var attr = Xrm.Page.getAttribute(Attributes.CaseCategory3).getValue();
        if (attr == null || attr.length == 0) {
            return false;
        }
        return true;
    }
    // Configure visibility of compensation calculator tab\button and sections
    var showHideCompensationCalculator = function (triggerCalculation) {
        var tab = Xrm.Page.ui.tabs.get(TabsAndSections.CompensationCalculator);
        if (tab == null) return;
        var showCalculator = shouldShowCompensationCalculator();
        // show-hide calculation button
        Xrm.Page.ui.refreshRibbon();
        if (!showCalculator) {
            tab.setVisible(false);
            return;
        }
        // show tab        
        tab.setVisible(true);
        // show building work
        var isBuildingWork = isBuildingWorkCaseLine();
        var tabSection = tab.sections.get(TabsAndSections.BuildingWork);
        if (tabSection != null) {
            tabSection.setVisible(isBuildingWork);
        }
        // show wrong room type
        var isWrongRoomType = isWrongRoomTypeCaseLine();
        tabSection = tab.sections.get(TabsAndSections.RoomType);
        if (tabSection != null) {
            tabSection.setVisible(isWrongRoomType);
        }
        if (triggerCalculation) {
            calculateCompensation();
        }
    }

    // public
    return {
        LoadSourceMarketAndSetDefaults: loadSourceMarketAndSetDefaults,
        CalculateCompensation: calculateCompensation,
        ShowHideCompensationCalculator: showHideCompensationCalculator,
        ClearCompensationCalculatorValues: function () {
            var attr = Xrm.Page.getAttribute(Attributes.ProposedCompensation);
            if (attr != null) attr.setValue(null);
            attr = Xrm.Page.getAttribute(Attributes.MaxProposedCompensation);
            if (attr != null) attr.setValue(null);
            attr = Xrm.Page.getAttribute(Attributes.OfferedAmount);
            if (attr != null) attr.setValue(null);
        }
    };
})();
console.log('loaded library.caseline');
// end script
});