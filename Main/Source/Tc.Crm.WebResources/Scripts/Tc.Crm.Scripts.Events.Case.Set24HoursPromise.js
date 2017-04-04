//*************************************************************************
// Description:  Auto set 24 Hours promise based on business rules
//
// Author: Balamurali Srinvasagan (PO)
// User Story: CRM-160
//**************************************************************************

/// <reference path="Tc.Crm.Scripts.Common.js" />
/// <reference path="Tc.Crm.Scripts.Common.js" />

"use strict";
var Tc = Tc || {};
Tc.Crm = Tc.Crm || {};
Tc.Crm.Scripts = Tc.Crm.Scripts || {};
Tc.Crm.Scripts.Events = Tc.Crm.Scripts.Events || {};
Tc.Crm.Scripts.Events.Case = Tc.Crm.Scripts.Events.Case || {};
Tc.Crm.Scripts.Events.Case.Main = Tc.Crm.Scripts.Events.Case.Main || {};
Tc.Crm.Scripts.Events.Case.DayPromiseBL = Tc.Crm.Scripts.Events.DayPromiseBL || {};

(function () {
    /** @description Auto set 24 Hours promise based on business rules
     * @param {string} entitySetName The name of the entity set for the type of entity you want to create.
     * @param {object} entity An object with the properties for the entity you want to create.
     */
    this.Set24HoursPromise = function () {
        debugger;
        writeToConsole("Tc.Crm.Scripts.Events.Case.DayPromiseBL.Set24HoursPromise - Start.");

        /// <summary>Create a new entity</summary>
        /// <param name="entitySetName" type="String">The name of the entity set for the entity you want to create.</param>
        /// <param name="entity" type="Object">An object with the properties for the entity you want to create.</param>       
        //if (!isString(entitySetName)) {
        //    throw new Error("Tc.Crm.Scripts.Events.Case.create entitySetName parameter must be a string.");
        //}
        //if (isNullOrUndefined(entity)) {
        //    throw new Error("Tc.Crm.Scripts.Events.Case.create entity parameter must not be null or undefined.");
        //}

        writeToConsole("Tc.Crm.Scripts.Events.Case.DayPromiseBL.Set24HoursPromise - Parameter Validation Successful.");

        var isCrmForMobile = (Xrm.Page.context.client.getClient() == "Mobile")
        if (isCrmForMobile) {
            // Code for CRM for phones and tablets only goes here.

            if (!Xrm.Mobile.offline.isOfflineEnabled("entityType")) {
                console.log("Entity is not enabled for off-line.");
                return;
            }

            if (!(Xrm.Page.ui.getFormType() === 1 || Xrm.Page.ui.getFormType() === 2)) {
                console.log("Form Type is not supported.");
                return;
            }

            var entityType = "incidents";
            var bookingReference = Xrm.Page.getControl("tc_bookingreference").getAttribute().getValue();

            writeToConsole("Booking Reference: " + bookingReference);

            if (bookingReference) {

            }
            else {

            }

            var options = "?$select=name,tc_bookingreference";  //&$expand=primarycontactid($select=contactid,fullname)";



            //Xrm.Mobile.offline.updateRecord(entityType, caseId, options).then(function (request) {
            //    writeToConsole("Successfully set field.");
            //})

            writeToConsole("Called from Mobile");

            Xrm.Utility.alertDialog("Mobile client");
        }
        else {
            // Code for web browser or CRM for Outlook only goes here.

            Xrm.Utility.alertDialog("Web / CRM for Outlook  client");
        }
    };

    //Internal supporting functions
    function getClientUrl() {
        //Get the organization URL
        if (typeof GetGlobalContext == "function" &&
            typeof GetGlobalContext().getClientUrl == "function") {
            return GetGlobalContext().getClientUrl();
        }
        else {
            //If GetGlobalContext is not defined check for Xrm.Page.context;
            if (typeof Xrm != "undefined" &&
                typeof Xrm.Page != "undefined" &&
                typeof Xrm.Page.context != "undefined" &&
                typeof Xrm.Page.context.getClientUrl == "function") {
                try {
                    return Xrm.Page.context.getClientUrl();
                } catch (e) {
                    throw new Error("Xrm.Page.context.getClientUrl is not available.");
                }
            }
            else { throw new Error("Context is not available."); }
        }
    }
    function getWebAPIPath() {
        return getClientUrl() + "/api/data/v8.1/";
    }

    //Internal validation functions
    function isString(obj) {
        if (typeof obj === "string") {
            return true;
        }
        return false;

    }
    function isNull(obj) {
        if (obj === null)
        { return true; }
        return false;
    }
    function isUndefined(obj) {
        if (typeof obj === "undefined") {
            return true;
        }
        return false;
    }
    function isFunction(obj) {
        if (typeof obj === "function") {
            return true;
        }
        return false;
    }
    function isNullOrUndefined(obj) {
        if (isNull(obj) || isUndefined(obj)) {
            return true;
        }
        return false;
    }
    function isFunctionOrNull(obj) {
        if (isNull(obj))
        { return true; }
        if (isFunction(obj))
        { return true; }
        return false;
    }

    // This function is called when an error callback parses the JSON response.
    // It is a public function because the error callback occurs in the onreadystatechange 
    // event handler and an internal function wouldn’t be in scope.
    this.errorHandler = function (resp) {
        try {
            return JSON.parse(resp).error;
        } catch (e) {
            return new Error("Unexpected Error")
        }
    }

    function writeToConsole(message) {
        if (typeof console != 'undefined') {
            console.log(message);
        }
    }


}).call(Tc.Crm.Scripts.Events.Case.DayPromiseBL);
