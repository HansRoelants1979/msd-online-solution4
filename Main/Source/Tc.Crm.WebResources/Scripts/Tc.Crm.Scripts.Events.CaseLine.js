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
Tc.Crm.Scripts.Events.CaseLine = ( function () {
    "use strict";

    var FORM_MODE_CREATE = 1;
    var FORM_MODE_UPDATE = 2;
    var FORM_MODE_QUICK_CREATE = 5;

    var VIEW_RANDOM_GUID = "{5A8261E7-71F5-4904-B046-EE8001A01CF5}";

    var CLIENT_MODE_MOBILE = "Mobile";
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
        PackageHoliday : 40
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

    ///
    /// Add OnLoad event handlers
    ///
    var addEventHandlers = function () {
        console.log("Register case category PreSearch handlers");
        // filter level 1 case categories by case type complain
        Xrm.Page.getControl(Attributes.CaseCategory1).addPreSearch(filterCategoryByComplainCaseType);
        // filter level 2 case categories by selected level 1
        Xrm.Page.getControl(Attributes.CaseCategory2).addPreSearch(filterLevel2CaseCategory);
        // filter level 3 case categories by selected level 2
        Xrm.Page.getControl(Attributes.CaseCategory3).addPreSearch(filterLevel3CaseCategory);
    }

    ///
    /// PreSearch method for case category level 1.
    /// Show only active level 1 case categories
    ///
    var filterCategoryByComplainCaseType = function () {
        console.log("Call PreSearch: " + Attributes.CaseCategory1);
        // clean-up selection of level 2 and level 3
        Xrm.Page.getAttribute(Attributes.CaseCategory2).setValue(null);
        Xrm.Page.getAttribute(Attributes.CaseCategory3).setValue(null);
        Xrm.Page.getAttribute(Attributes.CaseCategory3).fireOnChange();
        // add filter for lookup view by case type of case line
        var caseTypeComplainId = Xrm.Page.getAttribute(Attributes.CaseType).getValue();
        if (caseTypeComplainId == null) return;
        var filter = "<filter type='and'><condition attribute='statecode' operator='eq' value='0' /><condition attribute='tc_casetypeid' operator='eq' value='" + caseTypeComplainId[0].id + "'/></filter>";
        Xrm.Page.getControl(Attributes.CaseCategory1).addCustomFilter(filter, EntityNames.CaseCategory);
    }

    ///
    /// PreSearch method for case category level 2
    ///
    var filterLevel2CaseCategory = function () {
        console.log("Call PreSearch: " + Attributes.CaseCategory2);
        // clean-up case category level 3, 4 selection
        Xrm.Page.getAttribute(Attributes.CaseCategory3).setValue(null);
        Xrm.Page.getAttribute(Attributes.CaseCategory3).fireOnChange();
        filterCategoryByParentCaseCategory(Attributes.CaseCategory2);
    }

    ///
    /// PreSearch method for case category level 3
    ///
    var filterLevel3CaseCategory = function () {
        console.log("Call PreSearch: " + Attributes.CaseCategory3);
        filterCategoryByParentCaseCategory(Attributes.CaseCategory3);
    }

    ///
    /// Filter search result to show only active records related to selected parent case category.
    ///
    var filterCategoryByParentCaseCategory = function (controlId) {
        var parentControlId = '';
        switch (controlId)
        {
            case Attributes.CaseCategory2:
                parentControlId = Attributes.CaseCategory1;
                break
            case Attributes.CaseCategory3:
                parentControlId = Attributes.CaseCategory2;
                break
            default:
                console.warn("Call PreSearch for not supported field");
                return;
        }
        // get selected value of parent control
        var parentCaseCategory = Xrm.Page.getAttribute(parentControlId).getValue();
        if (parentCaseCategory == null) {
            console.warn("Parent control value is null.");
            return;
        }        
        var parentCaseCategoryId = parentCaseCategory[0].id;
        // filter by parent case category id
        var fetchXml =
            '<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="true">' +
                '<entity name="tc_casecategory">' +
                    '<attribute name="tc_casecategoryid" />' +
                    '<attribute name="tc_name" />' +
                    '<order attribute="tc_name" descending="false" />' +
                    '<filter type="and">' +
                      '<condition attribute="statecode" operator="eq" value="0" />' +
                    '</filter>' +
                    '<link-entity name="tc_tc_casecategory_tc_casecategory" from="tc_casecategoryidtwo" to="tc_casecategoryid" visible="false" intersect="true">' +
                        '<link-entity name="tc_casecategory" from="tc_casecategoryid" to="tc_casecategoryidone" alias="ab">' +
                            '<filter type="and">' +
                                '<condition attribute="tc_casecategoryid" operator="eq" uitype="tc_casecategory" value="' + parentCaseCategoryId + '" />' +
                            '</filter>' +
                        '</link-entity>' +
                    '</link-entity>' +
                '</entity>' +
            '</fetch>';
        var layoutXml =
            '<grid name="resultset" object="10037" jump="tc_name" select="1" icon="1" preview="1">' +
                '<row name="result" id="tc_casecategoryid">' +
                    '<cell name="tc_name" width="300"/>' +
                '</row>' +
            '</grid>';
        // set id to default view id, so results are filtered out
        var viewDisplayName = "Related Case Categories Lookup View";
        var control = Xrm.Page.getControl(controlId);
        control.addCustomView(VIEW_RANDOM_GUID, EntityNames.CaseCategory, viewDisplayName, fetchXml, layoutXml, false);
        control.setDefaultView(VIEW_RANDOM_GUID);
    }

    /// Foramting the name of the Case Line Entity.

    var formationTheNameOfTheCaseLineEntity = function () {
        console.log("formation The Name of The Case Line Entity - Start");
        var value = Xrm.Page.getAttribute(Attributes.CaseCategory1).getValue();
        if (value != null) {
            var CaseLineName = value[0].name;
            value = Xrm.Page.getAttribute(Attributes.CaseCategory2).getValue();
            if (value != null) {
                CaseLineName = CaseLineName + " > " + value[0].name;
                value = Xrm.Page.getAttribute(Attributes.CaseCategory3).getValue();
                if (value != null) {
                    CaseLineName = CaseLineName + " > " + value[0].name;
                }
            }
            Xrm.Page.getAttribute(Attributes.Name).setValue(CaseLineName);
        }
        console.log("formation The Name of The Case Line Entity - End");
    }

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
        (category1Id === "{2505f04f-f3f2-e611-8107-3863bb34fa70}" && category2Id === "{6705f04f-f3f2-e611-8107-3863bb34fa70}" && category3Id === "{9906f04f-f3f2-e611-8107-3863bb34fa70}") ||
        // Pre- Departure Services	Notification - Other	Pre Dept-Notification Building Work
        (category1Id === "{3905f04f-f3f2-e611-8107-3863bb34fa70}" && category2Id === "{c305f04f-f3f2-e611-8107-3863bb34fa70}" && category3Id === "{7f07f04f-f3f2-e611-8107-3863bb34fa70}") ||
        // Resort	Local Building Work	Resort-Pre Dept Local Building Work
        (category1Id === "{3b05f04f-f3f2-e611-8107-3863bb34fa70}" && category2Id === "{cd05f04f-f3f2-e611-8107-3863bb34fa70}" && category3Id === "{9907f04f-f3f2-e611-8107-3863bb34fa70}");
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
        if (IsOfflineMode()) {
            return promiseResponse.values != null ? promiseResponse.values : promiseResponse;
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

    function getCase(caseId) {
        if (IsOfflineMode()) {
            var query = "?$select=tc_sourcemarketid,tc_bookingreference,tc_bookingid,tc_producttype,tc_bookingtravelamount,tc_durationofstay&$expand=tc_sourcemarketid($select=tc_iso2code,tc_countryid)";            
            return Xrm.Mobile.offline.retrieveRecord(EntityNames.Case, caseId, query);
        }
        else {
            var query = "?$select=_tc_sourcemarketid_value,tc_bookingreference,_tc_bookingid_value,tc_producttype,tc_bookingtravelamount,tc_durationofstay&$expand=tc_sourcemarketid($select=tc_iso2code,tc_countryid)";
            return Tc.Crm.Scripts.Common.GetById(EntitySetNames.Case, caseId, query);
        }
    }

    function getBooking(bookingId) {
        if (IsOfflineMode()) {
            var query = "?$select=tc_sourcemarketid,tc_travelamount,tc_duration&$expand=tc_SourceMarketId($select=tc_iso2code,tc_countryid)";
            return Xrm.Mobile.offline.retrieveRecord(EntityNames.Booking, bookingId, query);
            
        } else {
            var query = "?$select=_tc_sourcemarketid_value,tc_travelamount,tc_duration&$expand=tc_SourceMarketId($select=tc_iso2code,tc_countryid)";
            return Tc.Crm.Scripts.Common.GetById(EntitySetNames.Booking, bookingId, query);
        }        
    }

    var getConfigurationValue = function (configName) {
        if (IsOfflineMode()) {
            var query = "?$filter=tc_name eq '" + configName + "' &$select=tc_value";
            return Xrm.Mobile.offline.retrieveMultipleRecords(EntityNames.Configuration, query);
        }
        else {
            var query = "?$filter=tc_name eq '" + configName + "' &$select=tc_value";
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
            return Tc.Crm.Scripts.Common.retrieveMultipleRecords(EntityNames.CompensationCalculationMatrix, query);
        }
        else {
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
            return Tc.Crm.Scripts.Common.retrieveMultipleRecords(EntityNames.CompensationCalculationMatrixLine, query);
        }
        else {
            return Tc.Crm.Scripts.Common.Get(EntitySetNames.CompensationCalculationMatrixLine, query);
        }
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
        var accommodationCost = bookingValue * accommodationPercentage / 100;
        // % Holiday = Days Affected / Total Days
        var daysAffected = Xrm.Page.getAttribute(Attributes.AffectedDays).getValue();
        var totalDays = Xrm.Page.getAttribute(Attributes.TotalDays).getValue();
        var holidayPercentage = daysAffected / totalDays;
        // Value for % Holidays = Accommodation Costs * % Holiday
        var holidayPercentageValue = accommodationCost * holidayPercentage;
        // Compensation Value = Value for % Holidays * Impact %
        var compensationValue = holidayPercentageValue * impact / 100;
        // Range Value = Value for % Holidays * Range %
        var rangeValue = holidayPercentageValue * range / 100;
        //var adjustment = -14.29; // TODO: get from configuration
        //var maximumTotalLimitToBePaid = 40; // TODO: get from configuration
        var limitCompensation = accommodationCost * maximumTotalLimitToBePaid / 100;
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
                var matrix = getPromiseResponse(matrixResponse, "Compensation Calculation Matrix");
                if (matrix.value.length === 0) {
                    addNotificationNoAutomaticCalculation();
                    return;
                }
                // search compensation matrix line for severity/impact or building work
                var matrixId = matrix.value[0].tc_compensationcalculationmatrixid;
                Promise.all([getImpactAndRange(matrixId), getConfigurationValue(Configuration.Adjustment), getConfigurationValue(Configuration.MaximumTotalLimitToBePaid)]).then(function (allResponses) {
                    var matrixLine = getPromiseResponse(allResponses[0], "Compensation Calculation Matrix Line"); // parse matrix
                    if (matrixLine.value.length === 0) {
                        Xrm.Utility.alertDialog("No compensation matrix line impact+severity or building work is present in configuration. Contact System configurator");
                        return;
                    }
                    var adjustment = getPromiseResponse(allResponses[1], "Configuration"); // parse adjustment
                    if (adjustment.value.length === 0 || adjustment.value[0].tc_value === null) {
                        Xrm.Utility.alertDialog("No value in configuration for " + Configuration.Adjustment + ". Contact System configurator");
                        return;
                    }
                    var maximumTotalLimitToBePaid = getPromiseResponse(allResponses[2], "Configuration"); // parse maximumTotalLimitToBePaid                        
                    if (maximumTotalLimitToBePaid.value.length === 0 || maximumTotalLimitToBePaid.value[0].tc_value === null) {
                        Xrm.Utility.alertDialog("No value in configuration for " + Configuration.MaximumTotalLimitToBePaid + ". Contact System configurator");
                        return;
                    }
                    calculateDefaultFormulaUk(matrixLine.value[0].tc_impactpercentage, matrixLine.value[0].tc_rangepercentage, parseFloat(adjustment.value[0].tc_value), parseFloat(maximumTotalLimitToBePaid.value[0].tc_value));
                },
                function (error) {
                    Xrm.Utility.alertDialog("Error getting compensation matrix values or retrieving configuration. Please retry");
                    console.warn("Error getting compensation matrix values or retrieving configuration");
                });
            },
            function (error) {
                Xrm.Utility.alertDialog("Error getting compensation matrix. Please retry");
                console.warn("Error getting compensation matrix");
            }
        );
    }

    // Calculate compensation for non-UK (continental) markets
    var calculateCompensationContinental = function () {
        // get configuration entry
        getMatrixId().then(
            function (response) {
                // parse from response, if null - show message according as no calculation should be done
                var matrix = JSON.parse(response.response); 
                if (matrix.value.length === 0) {
                    addNotificationNoAutomaticCalculation();
                    return;
                }
                var matrixId = matrix.value[0].tc_compensationcalculationmatrixid;
                // search compensation matrix line for impact
                getImpactAndRange(matrixId).then(
                    function (matrixResponse) {
                        var matrixLine = JSON.parse(matrixResponse.response); // parse matrix
                        if (matrixLine.value.length === 0) {
                            Xrm.Utility.alertDialog("No compensation matrix line impact+severity or building work is present in configuration. Contact System configurator");
                            return;
                        }
                        var impact = matrixLine.value[0].tc_impactpercentage;
                        // Proposed Compensation = (Travel Amount / Total Days) * Days Affected * Impact %
                        var travelAmount = Xrm.Page.getAttribute(Attributes.TotalBookingValue).getValue();
                        var totalDays = Xrm.Page.getAttribute(Attributes.TotalDays).getValue();
                        var daysAffected = Xrm.Page.getAttribute(Attributes.AffectedDays).getValue();
                        var value = travelAmount * daysAffected / totalDays * impact / 100;

                        Xrm.Page.getAttribute(Attributes.ProposedCompensation).setValue(value);
                        Xrm.Page.getAttribute(Attributes.MaxProposedCompensation).setValue(value);
                        Xrm.Page.getControl(Attributes.OfferedAmount).setFocus();
                    },
                    function (error) {
                        Xrm.Utility.alertDialog("Error getting compensation matrix values or retrieving configuration. Please retry");
                        console.warn("Error getting compensation matrix values or retrieving configuration");
                    }
                );
            },
            function (error) {
                Xrm.Utility.alertDialog("Error getting compensation matrix. Please retry");
                console.warn("Error getting compensation matrix");
            }
        );
    }
    
    // Validate mandatory field. Execute action on fild if needed
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
            validateMandatoryField(Attributes.ProductType, "Please enter total product type") &&
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
            validateMandatoryField(Attributes.Impact, "Please enter impact", function (value) {
                // set impact for further calculation
                switch (value) {
                    case Level.High:
                        impact = Level.H;
                        break;
                    case Level.Medium :
                        impact = Level.M;
                        break;
                    case Level.Low:
                        impact = Level.L;
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
        if (IsOfflineMode()) return;
        var caseIdAttr = Xrm.Page.getAttribute(Attributes.Case);
        var value = caseIdAttr.getValue();
        if (value == null || value.length == 0) return;

        var caseId = formatEntityId(value[0].id);
        var caseReceivedPromise = getCase(caseId);
        caseReceivedPromise.then(
            caseRetrieved,
            function (error) {
                Xrm.Utility.alertDialog("Problem getting case. Please retry");
            }
        );
    }

    // Calculation compensation for case line
    var calculateCompensation = function () {
        if (IsOfflineMode()) return;
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
        Xrm.Page.getAttribute(Attributes.ProductType).setValue(productType);
        Xrm.Page.getAttribute(Attributes.TotalBookingValue).setValue(travelAmount);
        Xrm.Page.getAttribute(Attributes.TotalDays).setValue(duration);
    }

        // Get source market
    var caseRetrieved = function (caseResponse) {
        var incident = getPromiseResponse(caseResponse, "Case");
        var isCreate = (Xrm.Page.ui.getFormType() === FORM_MODE_CREATE) || (Xrm.Page.ui.getFormType() === FORM_MODE_QUICK_CREATE);
        var bookingUsed = !IsOfflineMode() ? incident.tc_bookingreference : incident.tc_bookingreference === "true";
        if (bookingUsed) {
            var bookingId = incident._tc_bookingid_value;
            // Get source market from booking
            getBooking(bookingId).then(            
                function (bookingResponse) {
                    var booking = getPromiseResponse(bookingResponse, "Booking");
                    isUkMarket = booking.tc_SourceMarketId.tc_iso2code.toUpperCase() === SOURCE_MARKET_UK;
                    // compensation calculator field based on market
                    showHideCompensationCalculator();
                    // set defaults on create                    
                    if (isCreate) {
                        var productType = incident.tc_producttype;
                        var duration = booking.tc_duration;
                        var travelAmount = booking.tc_travelamount;
                        setDefaultsOnCreate(productType, travelAmount, duration);
                    }
                },
                function (error) {
                    Xrm.Utility.alertDialog("Problem getting booking. Please retry");
                    console.warn("Problem getting booking");
            });
        }
        else {
            // Set source market from case
            isUkMarket = incident.tc_sourcemarketid.tc_iso2code.toUpperCase() === SOURCE_MARKET_UK;
            // compensation calculator field based on market
            showHideCompensationCalculator();
            // set defaults on create                    
            if (isCreate) {
                var productType = incident.tc_producttype;
                var duration = incident.tc_durationofstay;
                var travelAmount = incident.tc_bookingtravelamount;
                setDefaultsOnCreate(productType, travelAmount, duration);
            }
        }
    }

        // Configure visibility of compensation calculator tab\button and sections
    var showHideCompensationCalculator = function () {
        if (IsOfflineMode()) return;
        // show-hide calculation button
        Xrm.Page.ui.refreshRibbon();
        // hide tab if category level 3 is not selected
        var tab = Xrm.Page.ui.tabs.get(TabsAndSections.CompensationCalculator);
        if (tab == null) return;
        var attr = Xrm.Page.getAttribute(Attributes.CaseCategory3).getValue();
        if (attr == null || attr.length == 0) {
            tab.setVisible(false);
            return;
    }
        // show tab        
        tab.setVisible(true);
        // show building work
        var isBuildingWork = isBuildingWorkCaseLine();
        tab.sections.get(TabsAndSections.BuildingWork).setVisible(isBuildingWork);
        // show wrong room type
        var isWrongRoomType = isWrongRoomTypeCaseLine();
        tab.sections.get(TabsAndSections.RoomType).setVisible(isWrongRoomType);
    }

        // public methods
    return {
        OnLoad: function () {
            addEventHandlers();
            loadSourceMarketAndSetDefaults();
        },
        OnCaseLineSave: function () {
            formationTheNameOfTheCaseLineEntity();
        },
        OnCompensationCalculate: calculateCompensation,
        OnCaseCategoryLevel3Change: function () {
            var attr = Xrm.Page.getAttribute(Attributes.ProposedCompensation);
            if (attr != null) attr.setValue(null);
            attr = Xrm.Page.getAttribute(Attributes.MaxProposedCompensation);
            if (attr != null) attr.setValue(null);
            attr = Xrm.Page.getAttribute(Attributes.OfferedAmount);
            if (attr != null) attr.setValue(null);
            showHideCompensationCalculator();
        }
    };
})();