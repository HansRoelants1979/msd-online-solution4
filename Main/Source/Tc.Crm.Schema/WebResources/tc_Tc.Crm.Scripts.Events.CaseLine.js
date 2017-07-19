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
Tc.Crm.Scripts.Events.CaseLine = (function () {
    "use strict";

    var VIEW_RANDOM_GUID = "{5A8261E7-71F5-4904-B046-EE8001A01CF5}";
    var CLIENT_STATE_OFFLINE = "Offline";

    var Attributes = {
        Name: "tc_name",
        CaseType: "tc_casetypeid",
        CaseCategory1: "tc_categorylevel1id",
        CaseCategory2: "tc_casecategory2id",
        CaseCategory3: "tc_category3id",
        ServiceType: "tc_servicetype",
        AccommodationId: "tc_bookingaccommodationid",
        EmailAddress: "emailaddress",
        AccommodationPropertyName: "tc_accommodationpropertyname",
    }

    var EntityNames = {
        CaseCategory: "tc_casecategory"
    }

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
        switch (controlId) {
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

    var setAlternateEmailAddressOnAccommodation = function () {
        console.log("setAlternateEmailAddressOnAccomdation - Start");
        var serviceTypeAttribute = Xrm.Page.getAttribute(Attributes.ServiceType);
        if (serviceTypeAttribute == null || serviceTypeAttribute == undefined) return;
        var serviceType = serviceTypeAttribute.getValue();
        if (serviceType !== 950000000) return;

        var accommodationAttribute = Xrm.Page.getAttribute(Attributes.AccommodationId);
        if (accommodationAttribute == null || accommodationAttribute == undefined) return;
        
        var accommodation = accommodationAttribute.getValue();
        if (accommodation == null || accommodation == "undefined" || accommodation == "")
            return;
        var accommodationId = accommodation[0].id;
        if (accommodationId == null || accommodationId == "" || accommodationId == "undefined")
            return;
        accommodationId = accommodationId.replace("{", "").replace("}", "");
        var entityType = accommodation[0].entityType;
        if (entityType == null || entityType == "" || entityType == "undefined")
            return;

        var accommodationReceivedPromise = getAccommodation(accommodationId).then(
                        function (accommodationResponse) {

                            var accommodationRecord = JSON.parse(accommodationResponse.response);
                            if (accommodationRecord == null || accommodationRecord == "" || accommodationRecord == "undefined") return;
                            if (accommodationRecord.tc_HotelId == null || accommodationRecord.tc_HotelId == "" || accommodationRecord.tc_HotelId == "undefined") return;
                            if (accommodationRecord.tc_HotelId.tc_primaryemailaddress == null || accommodationRecord.tc_HotelId.tc_primaryemailaddress == "" || accommodationRecord.tc_HotelId.tc_primaryemailaddress == "undefined") return;
                            var emailAddressAttribute = Xrm.Page.getAttribute(Attributes.EmailAddress);
                            if (emailAddressAttribute == null || emailAddressAttribute == undefined) return;                            
                                emailAddressAttribute.setValue(accommodationRecord.tc_HotelId.tc_primaryemailaddress);

                        }).catch(function (err) {                            
                            throw new Error("Problem in retrieving the Accommodation");
                        });

        console.log("setAlternateEmailAddressOnAccomdation - End");


    }
    var setAlternateEmailAddressOnPropertyName = function () {
        console.log("setAlternateEmailAddressOnPropertyName - Start");
        var serviceTypeAttribute = Xrm.Page.getAttribute(Attributes.ServiceType);
        if (serviceTypeAttribute == null || serviceTypeAttribute == undefined) return;
        var serviceType = serviceTypeAttribute.getValue();
        if (serviceType !== 950000000) return;

        var accommodationPropertyNameAttribute = Xrm.Page.getAttribute(Attributes.AccommodationPropertyName);
        if (accommodationPropertyNameAttribute == null || accommodationPropertyNameAttribute == undefined) return;

        var accommodationPropertyName = accommodationPropertyNameAttribute.getValue();
        if (accommodationPropertyName == null || accommodationPropertyName == "undefined" || accommodationPropertyName == "")
            return;
        var accommodationPropertyNameId = accommodationPropertyName[0].id;
        if (accommodationPropertyNameId == null || accommodationPropertyNameId == "" || accommodationPropertyNameId == "undefined")
            return;
        accommodationPropertyNameId = accommodationPropertyNameId.replace("{", "").replace("}", "");
        var entityType = accommodationPropertyName[0].entityType;
        if (entityType == null || entityType == "" || entityType == "undefined")
            return;

        var accommodationReceivedPromise = getHotel(accommodationPropertyNameId).then(
                        function (accommodationPropertyNameResponse) {

                            var hotelRecord = JSON.parse(accommodationPropertyNameResponse.response);
                            if (hotelRecord == null || hotelRecord == "" || hotelRecord == "undefined") return;
                            if (hotelRecord.tc_primaryemailaddress == null || hotelRecord.tc_primaryemailaddress == "" || hotelRecord.tc_primaryemailaddress == "undefined") return;
                            var emailAddressAttribute = Xrm.Page.getAttribute(Attributes.EmailAddress);
                            if (emailAddressAttribute == null || emailAddressAttribute == undefined) return;
                                emailAddressAttribute.setValue(hotelRecord.tc_primaryemailaddress);

                        }).catch(function (err) {                            
                            throw new Error("Problem in retrieving the Hotel");
                        });

        console.log("setAlternateEmailAddressOnPropertyName - End");


    }

    function getAccommodation(accommodationId) {

        var query = "?$select=_tc_hotelid_value&$expand=tc_HotelId($select=tc_primaryemailaddress)";
        var entityName = "tc_bookingaccommodations";
        var id = accommodationId;
        if (IsOfflineMode()) {
            return Xrm.Mobile.offline.retrieveRecord(entityName, id, query);
        }
        else {
            return Tc.Crm.Scripts.Common.GetById(entityName, id, query);
        }

    }
    function getHotel(accommodationPropertyNameId) {

        var query = "?$select=tc_primaryemailaddress";
        var entityName = "tc_hotels";
        var id = accommodationPropertyNameId;
        if (IsOfflineMode()) {
            return Xrm.Mobile.offline.retrieveRecord(entityName, id, query);
        }
        else {
            return Tc.Crm.Scripts.Common.GetById(entityName, id, query);
        }

    }

    function IsOfflineMode() {
        return Xrm.Page.context.client.getClientState() === CLIENT_STATE_OFFLINE
    }

    // public methods
    return {
        OnLoad: function () {
            addEventHandlers();
            Tc.Crm.Scripts.Library.CaseLine.LoadSourceMarketAndSetDefaults();
        },
        OnCaseLineSave: function () {
            formationTheNameOfTheCaseLineEntity();
        },
        OnCompensationCalculate: function () {
            Tc.Crm.Scripts.Library.CaseLine.CalculateCompensation();
        },
        OnCaseCategoryLevel3Change: function () {
            Tc.Crm.Scripts.Library.CaseLine.ClearCompensationCalculatorValues();
            Tc.Crm.Scripts.Library.CaseLine.ShowHideCompensationCalculator(true);
        },
        OnCaseLineAccommodationChange: function() {
            setAlternateEmailAddressOnAccommodation();
        },
        OnCaseLinePropertyNameChange: function () {
            setAlternateEmailAddressOnPropertyName();
        }
    };
})();