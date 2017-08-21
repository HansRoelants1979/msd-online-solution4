var scriptLoader = scriptLoader || {
    delayedLoads : [],
    load: function (name, requires, script) {
        window._loadedScripts = window._loadedScripts || { };
        // Check for loaded scripts, if not all loaded then register delayed Load
        if (requires == null || requires.length == 0 || scriptLoader.areLoaded(requires)) {
            scriptLoader.runScript(name, script);
        }
        else {
            // Register an onload check
            scriptLoader.delayedLoads.push({ name : name, requires: requires, script : script });
        }
    },
    runScript: function (name, script) {
        script.call(window);
        window._loadedScripts[name]= true;
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
            allLoaded = allLoaded && (window._loadedScripts[requires[i]]!= null);
            if (!allLoaded)
                break;
        }
        return allLoaded;
    }
};
scriptLoader.load("Tc.Crm.Scripts.Events.Case", ["Tc.Crm.Scripts.Library.Contact"], function () {
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

Tc.Crm.Scripts.Events.Case = (function () {
    "use strict";

    var CLIENT_STATE_OFFLINE = "Offline";
    var CASE_TYPE_CONTROL_BOOKINGREF = "tc_bookingreference";
    var CASE_BOOKING_NUMBER = "tc_bookingid";
    var CASE_SOURCE_MARKET_ID = "tc_sourcemarketid";
    var CASE_SOURCE_MARKET_CURRENCY = "transactioncurrencyid";
    var CASE_ENTITY_NAME = "incident";
    var CASE_BOOKING_ENTITY_NAME = "tc_booking"
    var CASE_SOURCE_MARKET_ENTITY_NAME = "tc_country"
    var FORM_MODE_CREATE = 1;
    var FORM_MODE_UPDATE = 2;
    

    function OnLoad() {
        Tc.Crm.Scripts.Library.Contact.GetNotificationForPhoneNumber("tc_alternativephone");
        Tc.Crm.Scripts.Library.Contact.GetNotificationForPhoneNumber("tc_otherpartyphone");
        validateCaseAssociatedCustomerPhoneNum();
        preFilterLocationOfficeLookup();
        setNotification();
    }
    var setNotification = function () {
        if (Xrm.Page.getAttribute("tc_reportnotes")) {
            if (Xrm.Page.getAttribute("tc_reportnotes").getValue() == null) {
                Xrm.Page.ui.setFormNotification(" You must provide a value for Rep Notes to Resolve Case. ", "WARNING", 'note')
            } else {
                Xrm.Page.ui.clearFormNotification('note')
            }
        }
    }
    var GetTheSourceMarketCurrency = function () {
               
        var sourceMarketId;
        var currencyId;
        console.log("Get The Source Market Currency - Start");
        if (Xrm.Page.getAttribute(CASE_TYPE_CONTROL_BOOKINGREF).getValue() == true) {

            if (Xrm.Page.getAttribute(CASE_BOOKING_NUMBER).getValue() != null) {

                var BookingId = Xrm.Page.getAttribute(CASE_BOOKING_NUMBER).getValue()[0].id;
                if (BookingId != null) {
                    BookingId = BookingId.replace("{", "").replace("}", "");
                    var SourceMarketReceivedPromise = getBooking(BookingId).then(
                        function (bookingResponse) {

                            var booking = JSON.parse(bookingResponse.response);
                            if (booking == null || booking == "" || booking == "undefined") return;
                            if (booking.tc_SourceMarketId == null || booking.tc_SourceMarketId == "" || booking.tc_SourceMarketId == "undefined") return;
                            if (booking.tc_SourceMarketId._transactioncurrencyid_value == null || booking.tc_SourceMarketId._transactioncurrencyid_value == "" || booking.tc_SourceMarketId._transactioncurrencyid_value == "undefined") return;
                            currencyId = booking.tc_SourceMarketId._transactioncurrencyid_value;
                            if (currencyId != null) {
                                return getSourceMarketCurrencyname(currencyId);
                            }

                            else {

                                Xrm.Utility.alertDialog("Currency is not associated with Source Market");

                            }

                        }).catch(function (err) {
                            throw new Error("Problem in retrieving the Source Market Currency");
                        });


                    SourceMarketReceivedPromise.then(
                    function (SourceMarketReceivedPromiseResponse) {
                        if (SourceMarketReceivedPromiseResponse == null || SourceMarketReceivedPromiseResponse == "" || SourceMarketReceivedPromiseResponse == "undefined") return;
                        var Currency = JSON.parse(SourceMarketReceivedPromiseResponse.response);
                        if (Currency == null || Currency == "" || Currency == "undefined") return;
                        if (Currency.currencyname == null || Currency.currencyname == "" || Currency.currencyname == "undefined") return;
                        if (currencyId != null) {
                            var currencyReference = [];
                            currencyReference[0] = {};
                            currencyReference[0].id = currencyId;
                            currencyReference[0].entityType = "transactioncurrency";
                            currencyReference[0].name = Currency.currencyname;

                            Xrm.Page.getAttribute(CASE_SOURCE_MARKET_CURRENCY).setValue(currencyReference);
                        }

                    }).catch(function (err) {
                        throw new Error("Problem in retrieving the Source Market Currency");
                    });

                }

            }

        }
        else {
            if (Xrm.Page.getAttribute(CASE_SOURCE_MARKET_ID).getValue() != null) {

                sourceMarketId = Xrm.Page.getAttribute(CASE_SOURCE_MARKET_ID).getValue()[0].id;
                if (sourceMarketId != null) {
                    sourceMarketId = sourceMarketId.replace("{", "").replace("}", "");
                    var SourceMarketCurrency = getSourceMarketCurrency(sourceMarketId).then(
                       function (sourceMarketResponse) {
                           if (sourceMarketResponse == null || sourceMarketResponse == "" || sourceMarketResponse == "undefined") return;
                           var sourceMarket = JSON.parse(sourceMarketResponse.response);
                           if (sourceMarket == null || sourceMarket == "" || sourceMarket == "undefined") return;
                           if (sourceMarket._transactioncurrencyid_value == null || sourceMarket._transactioncurrencyid_value == "" || sourceMarket._transactioncurrencyid_value == "undefined") return;
                           currencyId = sourceMarket._transactioncurrencyid_value;
                           var currencyName = sourceMarket.transactioncurrencyid.currencyname;
                           if (currencyId != null) {
                               var currencyReference = [];
                               currencyReference[0] = {};
                               currencyReference[0].id = currencyId;
                               currencyReference[0].entityType = "transactioncurrency";
                               currencyReference[0].name = currencyName;

                               Xrm.Page.getAttribute(CASE_SOURCE_MARKET_CURRENCY).setValue(currencyReference);

                           }

                           else {
                               Xrm.Utility.alertDialog("Currency is not associated with SourceMarket");
                           }

                       }).catch(function (err) {
                           throw new Error("Problem in retrieving the Source Market Currency");
                       });

                }

            }
        }

        console.log("Get The Source Market Currency - End");
    }
    function getBooking(bookingId) {

        var query = "?$select=_tc_sourcemarketid_value&$expand=tc_SourceMarketId($select=_transactioncurrencyid_value)";
        var entityName = "tc_bookings";
        var id = bookingId;
        return Tc.Crm.Scripts.Common.GetById(entityName, id, query);

    }
    function getSourceMarketCurrency(sourcMarketId) {
        var query = "?$select=_transactioncurrencyid_value&$expand=transactioncurrencyid($select=currencyname)";
        var entityName = "tc_countries";
        return Tc.Crm.Scripts.Common.GetById(entityName, sourcMarketId, query);
    }
    function getSourceMarketCurrencyname(Currencyid) {
        var query = "?$select=currencyname";
        var entityName = "transactioncurrencies";
        return Tc.Crm.Scripts.Common.GetById(entityName, Currencyid, query);
    }
    function MandatoryMetConditions() {
        if (Xrm.Page.getControl("tc_bookingreference").getAttribute().getValue() == true) {
            if (Xrm.Page.getControl("tc_mandatoryconditionsmet").getAttribute().getValue() == false) {
                if (Xrm.Page.getControl("tc_casetypeid").getAttribute().getValue() != null) {
                    if (Xrm.Page.getControl("tc_casetypeid").getAttribute().getValue()[0].name == "Complaint") {
                        if (Xrm.Page.getControl("tc_resortofficeid").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("tc_datereported").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("tc_preferredmethodofcommunication").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("caseorigincode").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("description").getAttribute().getValue() != null
                            ) {
                            if (Xrm.Page.getControl("tc_producttype").getAttribute().getValue() == null || Xrm.Page.getControl("tc_producttype").getAttribute().getValue() != null &&
                                Xrm.Page.getControl("tc_producttype").getAttribute().getValue() != "950000000") {
                                if (Xrm.Page.getControl("tc_istheholidaystoppingshorterthanplanned").getAttribute().getValue() != null &&
                                    Xrm.Page.getControl("tc_3rdpartyresponserequired").getAttribute().getValue() != null) {
                                    Xrm.Page.getControl("tc_mandatoryconditionsmet").getAttribute().setValue(true);
                                    Xrm.Page.data.entity.save();
                                }
                            }
                            else {
                                Xrm.Page.getControl("tc_mandatoryconditionsmet").getAttribute().setValue(true);
                                Xrm.Page.data.entity.save();
                            }
                        }
                    }
                }
            }
        }

        else {
            if (Xrm.Page.getControl("tc_casetypeid").getAttribute().getValue() != null) {
                if (Xrm.Page.getControl("tc_mandatoryconditionsmet").getAttribute().getValue() == false) {
                    if (Xrm.Page.getControl("tc_casetypeid").getAttribute().getValue()[0].name == "Complaint") {
                        if (Xrm.Page.getControl("tc_resortofficeid").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("tc_datereported").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("tc_preferredmethodofcommunication").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("caseorigincode").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("tc_sourcemarketid").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("tc_brandid").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("tc_destinationid").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("tc_locationid").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("tc_gateway").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("description").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("tc_departuredate").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("tc_bookingtravelamount").getAttribute().getValue() != null
                             ) {
                            if (Xrm.Page.getControl("tc_producttype").getAttribute().getValue() == null || Xrm.Page.getControl("tc_producttype").getAttribute().getValue() != null &&
                                Xrm.Page.getControl("tc_producttype").getAttribute().getValue() != "950000000") {
                                if (Xrm.Page.getControl("tc_istheholidaystoppingshorterthanplanned").getAttribute().getValue() != null &&
                                    Xrm.Page.getControl("tc_3rdpartyresponserequired").getAttribute().getValue() != null &&
                                    Xrm.Page.getControl("tc_durationofstay").getAttribute().getValue() != null) {
                                    Xrm.Page.getControl("tc_mandatoryconditionsmet").getAttribute().setValue(true);
                                    Xrm.Page.data.entity.save();
                                }
                            }
                            else {
                                Xrm.Page.getControl("tc_mandatoryconditionsmet").getAttribute().setValue(true);
                                Xrm.Page.data.entity.save();
                            }
                        }
                    }
                }
            }
        }
    }
    function validateCaseAssociatedCustomerPhoneNum() {
        var Customer = Xrm.Page.getAttribute("customerid").getValue();
        if (Customer == null)
            return;
        var CustomerId = Customer[0].id;
        if (CustomerId == null || CustomerId == "")
            return;
        CustomerId = CustomerId.replace("{", "").replace("}", "");
        var entityType = Customer[0].entityType;
        if (entityType == null || entityType == "")
            return;

        var ValidateCustomerPhoneNum = getCustomerTelephoneNum(CustomerId, entityType).then(
                        function (customerPhoneNumResponse) {
                            var customer = JSON.parse(customerPhoneNumResponse.response);
                            var telephoneNum = customer.telephone1;
                            if (telephoneNum == null || telephoneNum == "") {
                                Xrm.Page.ui.clearFormNotification("TelNumNotification2");
                                Xrm.Page.ui.setFormNotification("Customer's telephone number is not Present", "WARNING", "TelNumNotification1");
                                return;
                            }

                            var regex = /^\+(?:[0-9] ?){9,14}[0-9]$/;
                            if (regex.test(telephoneNum) == false) {
                                Xrm.Page.ui.clearFormNotification("TelNumNotification1");
                                Xrm.Page.ui.setFormNotification("The Customer's telephone number does not match the required format. The number should start with a + followed by the country dialing code and contain no spaces or other special characters i.e. +44 for UK.", "WARNING", "TelNumNotification2");
                            }
                            else {
                                Xrm.Page.ui.clearFormNotification("TelNumNotification2");
                                Xrm.Page.ui.clearFormNotification("TelNumNotification1");
                            }

                        }).catch(function (err) {

                            throw new Error("Error in retrieving Customer's PhoneNumber");
                        });
    }
    function getCustomerTelephoneNum(customerId,entityType) {

        var query = "?$select=telephone1";
        var entityName = entityType + "s";
        var id = customerId;
        if (IsOfflineMode()) {
            return Xrm.Mobile.offline.retrieveRecord(entityName, id, query);
        }
        else {
            return Tc.Crm.Scripts.Common.GetById(entityName, id, query);
        }
    }
    var onChangeTelephone1 = function () {

        Tc.Crm.Scripts.Library.Contact.GetNotificationForPhoneNumber("tc_alternativephone");
    }
    var onChangeTelephone2 = function () {
        Tc.Crm.Scripts.Library.Contact.GetNotificationForPhoneNumber("tc_otherpartyphone");
    }

    var preFilterLocationOfficeLookup = function ()
    {
        if (!Xrm.Page.getAttribute("tc_resortofficeid")) return;
        Xrm.Page.getControl("tc_resortofficeid").addPreSearch(function ()
        {
            filterLocationOfficeBasedOnSelectedGateway();
        });
    }

    var filterLocationOfficeBasedOnSelectedGateway = function()
    {
        if (!Xrm.Page.getAttribute("tc_resortofficeid")) return;
        if (Xrm.Page.getAttribute("tc_gateway") && Xrm.Page.getAttribute("tc_gateway").getValue() && Xrm.Page.getAttribute("tc_gateway").getValue().length > 0)
        {
            var gatewayId = Xrm.Page.getAttribute("tc_gateway").getValue()[0].id;
            addCustomFilterForLocationOffice(gatewayId);
        }
        else
        {
            getGateWayFromBooking();
        }
    }

    var getGateWayFromBooking = function()
    {   
        if (Xrm.Page.getAttribute("tc_bookingid") && Xrm.Page.getAttribute("tc_bookingid").getValue() && Xrm.Page.getAttribute("tc_bookingid").getValue().length > 0)
        {
            var bookingId = Xrm.Page.getAttribute("tc_bookingid").getValue()[0].id;
            var query = "?$select=_tc_destinationgatewayid_value";
            var entityName = "tc_bookings";
            var id = formatEntityId(bookingId);
            if (IsOfflineMode())
            {
                processCustomFilterPromise(Xrm.Mobile.offline.retrieveRecord(entityName, id, query));
            }
            else
            {
                processCustomFilterPromise(Tc.Crm.Scripts.Common.GetById(entityName, id, query));
            }
        }
    }

    var processCustomFilterPromise = function(promise)
    {
        promise.then(function (request)
        {   
            var booking = JSON.parse(request.response);
            if (booking && booking._tc_destinationgatewayid_value)
            {
                var gatewayId = booking._tc_destinationgatewayid_value;
                addCustomFilterForLocationOffice(gatewayId);
            }
                
        }).catch(function (err) {
            console.log("ERROR: " + err.message);
        });
    }

    var addCustomFilterForLocationOffice = function(gatewayId)
    {
        var fetchXml = "<fetch distinct='true' mapping='logical' output-format='xml-platform' version='1.0'>" +
                        "<entity name='tc_locationoffice'>" +
                        "<attribute name='tc_locationofficeid'/>" +
                        "<attribute name='tc_name'/>" +
                        "<attribute name='tc_address1_flatorunitnumber'/>" +
                        "<attribute name='tc_address1_housenumberorbuilding'/>" +
                        "<attribute name='tc_address1_street'/>" +
                        "<attribute name='tc_address1_town'/>" +
                        "<attribute name='tc_address1_postcode'/>" +
                        "<attribute name='tc_address1_county'/>" +
                        "<attribute name='tc_address1_country'/>" +
                        "<attribute name='tc_address1_additionalinformation'/>" +
                        "<order descending='false' attribute='tc_name'/>" +
                        "<link-entity name='tc_gateway_tc_locationoffice' intersect='true' visible='false' to='tc_locationofficeid' from='tc_locationofficeid'>" +
                            "<link-entity name='tc_gateway' to='tc_gatewayid' from='tc_gatewayid' alias='aa'>" +
                                "<filter type='and'>" +
                                    "<condition attribute='tc_gatewayid' value='" + gatewayId + "' operator='eq'/>" +
                                "</filter>" +
                            "</link-entity>" +
                       "</link-entity>" +
                       "</entity>" +
                       "</fetch>";

        var layoutXml = "<grid name='resultset' object='1' jump='tc_locationofficeid' select='1' icon='1' preview='1'>" +
                        "<row name='result' id='tc_locationofficeid'>" +
                        "<cell name='tc_name' width='90' />" +
                        "<cell name='tc_address1_flatorunitnumber' width='90' />" +
                        "<cell name='tc_address1_housenumberorbuilding' width='90' />" +
                        "<cell name='tc_address1_street' width='100' />" +
                        "<cell name='tc_address1_town' width='100' />" +
                        "<cell name='tc_address1_postcode' width='90' />" +
                        "<cell name='tc_address1_county' width='100' />" +
                        "<cell name='tc_address1_country' width='100' />" +
                        "<cell name='tc_address1_additionalinformation' width='130' />" +
                        "</row>" +
                        "</grid>";
        
        Xrm.Page.getControl("tc_resortofficeid").addCustomView("{6fd72744-3676-41d4-8003-ae4cde9ac282}", "tc_locationoffice", "Associated Offices Of Gateway", fetchXml, layoutXml, true);
    }

    var formatEntityId = function (id) {
        return id !== null ? id.replace("{", "").replace("}", "") : null;
    }
    function IsOfflineMode() {
        return Xrm.Page.context.client.getClientState() === CLIENT_STATE_OFFLINE
    }

    // public methods
    return {
        OnLoad: function () {
            OnLoad();
        },
        OnChangeTelephone1: function () {
            onChangeTelephone1();
        },
        OnChangeTelephone2: function () {
            onChangeTelephone2();
        },
        OnChangeRepNotes: function () {
            setNotification();
        },
        OnSave: function () {
            if (Xrm.Page.context.client.getClientState() !== CLIENT_STATE_OFFLINE) {
                Tc.Crm.Scripts.Library.Case.UpdateRelatedCompensationsSourceMarket();
            }
        },
        OnCaseFieldChange: function () {
            GetTheSourceMarketCurrency();
        },
        OnCaseFieldChangeMandatoryMetConditions: function () {
            MandatoryMetConditions();
        },
        OnChangeCustomer: function () {
            validateCaseAssociatedCustomerPhoneNum();
        },
        OnChangeSourceMarket: function () {
            if (Xrm.Page.context.client.getClientState() === CLIENT_STATE_OFFLINE) {
                Tc.Crm.Scripts.Library.Case.UpdateRelatedCompensationsSourceMarket();
            }
        }
    };
})();

// end script
console.log('loaded events.case');
});
