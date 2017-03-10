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
    var CASE_TYPE_CONTROL_REG = "header_tc_casetypeid";
    var CASE_TYPE_CONTROL_QUICK_CREATE = "tc_casetypeid";
    var CASE_CATEGORY_1_ID = "tc_categorylevel1id";
    var CASE_CATEGORY_2_ID = "tc_casecategory2id";
    var CASE_CATEGORY_3_ID = "tc_category3id";
    var CASE_CATEGORY_4_ID = "tc_categorylevel4id";
    var CASE_CATEGORY_ENTITY = "tc_casecategory";
    var FORM_MODE_CREATE = 1;
    var VIEW_RANDOM_GUID = "{5A8261E7-71F5-4904-B046-EE8001A01CF5}";

    ///
    /// Add OnLoad event handlers
    ///
    var addEventHandlers = function () {
        // filter level 1 case categories by case type complain
        Xrm.Page.getControl(CASE_CATEGORY_1_ID).addPreSearch(filterCategoryByComplainCaseType);
        // filter level 2 case categories by selected level 1
        Xrm.Page.getControl(CASE_CATEGORY_2_ID).addPreSearch(filterCategoryByParentCaseCategory);
        // filter level 3 case categories by selected level 2
        Xrm.Page.getControl(CASE_CATEGORY_3_ID).addPreSearch(filterCategoryByParentCaseCategory);
        // filter level 4 case categories by selected level 3
        Xrm.Page.getControl(CASE_CATEGORY_4_ID).addPreSearch(filterCategoryByParentCaseCategory);
    }

    ///
    /// PreSearch method for case category level 1.
    /// Show only active level 1 case categories
    ///
    var filterCategoryByComplainCaseType = function () {
        // clean-up selection of level 2 and level 3
        Xrm.Page.getControl(CASE_CATEGORY_2_ID).getAttribute().setValue(null);
        Xrm.Page.getControl(CASE_CATEGORY_3_ID).getAttribute().setValue(null);
        Xrm.Page.getControl(CASE_CATEGORY_4_ID).getAttribute().setValue(null);
        // add filter for lookup view by case type of case line
        // get the proper control: in header for main form, field in case of quick create
        var caseTypeControl = Xrm.Page.getControl(CASE_TYPE_CONTROL_REG);
        if (Xrm.Page.ui.getFormType() === FORM_MODE_CREATE) {
            caseTypeControl = Xrm.Page.getControl(CASE_TYPE_CONTROL_QUICK_CREATE);
        }
        var caseTypeComplainId = caseTypeControl.getAttribute().getValue()[0].id;        
        var filter = "<filter type='and'><condition attribute='statecode' operator='eq' value='0' /><condition attribute='tc_casetypeid' operator='eq' value='" + caseTypeComplainId + "'/></filter>";
        Xrm.Page.getControl(CASE_CATEGORY_1_ID).addCustomFilter(filter, CASE_CATEGORY_ENTITY);
    }

    ///
    /// PreSearch method for case category level 2 and 3 look up.
    /// Filter search result to show only active records related to selected parent case category.
    ///
    var filterCategoryByParentCaseCategory = function (context) {
        var viewModel = context.getEventSource();
        var controlId = viewModel.getKey();
        var parentControlId = '';
        switch (controlId)
        {
            case CASE_CATEGORY_2_ID:
                parentControlId = CASE_CATEGORY_1_ID;
                // clean-up case category level 3, 4 selection
                Xrm.Page.getControl(CASE_CATEGORY_3_ID).getAttribute().setValue(null);
                Xrm.Page.getControl(CASE_CATEGORY_4_ID).getAttribute().setValue(null);
                break
            case CASE_CATEGORY_3_ID:
                // clean-up case category level 4 selection
                Xrm.Page.getControl(CASE_CATEGORY_4_ID).getAttribute().setValue(null);
                parentControlId = CASE_CATEGORY_2_ID;
                break
            case CASE_CATEGORY_4_ID:
                parentControlId = CASE_CATEGORY_3_ID;
                break
            default:
                return;
        }
        // get selected value of parent control
        var parentCaseCategory = Xrm.Page.getControl(parentControlId).getAttribute().getValue();
        if (parentCaseCategory == null) {
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
        control.addCustomView(VIEW_RANDOM_GUID, CASE_CATEGORY_ENTITY, viewDisplayName, fetchXml, layoutXml, false);
        control.setDefaultView(VIEW_RANDOM_GUID);
    }

    // public methods
    return {
        OnLoad: function () {
            addEventHandlers();
        }
    };
})();