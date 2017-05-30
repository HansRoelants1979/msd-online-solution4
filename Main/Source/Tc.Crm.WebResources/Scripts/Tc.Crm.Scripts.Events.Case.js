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
    }
    var GetTheSourceMarketCurrency = function () {

        
        console.log("Get The Source Market Currency - Start");
        if (Xrm.Page.getControl(CASE_TYPE_CONTROL_BOOKINGREF).getAttribute().getValue() == true) {


            if (Xrm.Page.getControl(CASE_BOOKING_NUMBER).getAttribute().getValue() != null) {
                var sourceMarketId;
                var Currencyid;
                var BookingId = Xrm.Page.getControl(CASE_BOOKING_NUMBER).getAttribute().getValue()[0].id;
                if (BookingId != null) {

                    BookingId = BookingId.replace("{", "").replace("}", "");
                    

                    var SourceMarketReceivedPromise = getBooking(BookingId).then(
                        function (bookingResponse) {
                            var booking = JSON.parse(bookingResponse.response);

                            sourceMarketId = booking._tc_sourcemarketid_value;
                            if (sourceMarketId != null) {
                                return getSourceMarketCurrency(sourceMarketId);
                            }

                            else {
                                Xrm.Utility.alertDialog("Source Market is not associated with Booking");
                                throw new Error("Source Market is not associated with Booking");
                            }

                        }).catch(function (err) {
                            throw new Error("Source Market is not associated with Booking");                    
                        });

                    var SourceMarketCurrency = SourceMarketReceivedPromise.then(
                   function (sourceMarketResponse) {
                       var sourceMarket = JSON.parse(sourceMarketResponse.response);

                       Currencyid = sourceMarket._transactioncurrencyid_value;

                       if (Currencyid != null) {
                           return getSourceMarketCurrencyname(Currencyid);
                       }
                       else {
                           Xrm.Utility.alertDialog("Currency is not associated with SourceMarket");
                           throw new Error("Currency is not associated with SourceMarket");
                       }                      

                   }).catch(function (err) {
                       throw new Error("Source Market is not associated with Booking");
                   });

                    SourceMarketCurrency.then(
                   function (sourceMarketCurrencyNameResponse) {
                       var Currency = JSON.parse(sourceMarketCurrencyNameResponse.response);

                       if (Currency != null && Currency.currencyname != null && Currencyid != null) {
                           var currencyReference = [];
                           currencyReference[0] = {};
                           currencyReference[0].id = Currencyid;
                           currencyReference[0].entityType = "transactioncurrency"; //YTODO: test
                           currencyReference[0].name = Currency.currencyname;

                           Xrm.Page.getControl(CASE_SOURCE_MARKET_CURRENCY).getAttribute().setValue(currencyReference);
                       }

                   }).catch(function (err) {
                       throw new Error("Currency  is not associated with Source Market");
                   });

                }

            }

        }
        else {
            if (Xrm.Page.getControl(CASE_SOURCE_MARKET_ID).getAttribute().getValue() != null) {
                var Currencyid;
                var SourceMarketId = Xrm.Page.getControl(CASE_SOURCE_MARKET_ID).getAttribute().getValue()[0].id;
                if (SourceMarketId != null) {

                    SourceMarketId = SourceMarketId.replace("{", "").replace("}", "");

                    var SourceMarketCurrency = getSourceMarketCurrency(SourceMarketId).then(
                        function (sourceMarketResponse) {
                            var sourceMarket = JSON.parse(sourceMarketResponse.response);

                            Currencyid = sourceMarket._transactioncurrencyid_value;

                            if (Currencyid != null) {
                                return getSourceMarketCurrencyname(Currencyid);
                            }

                            else {
                                Xrm.Utility.alertDialog("Currency is not associated with SourceMarket");
                                throw new Error("Currency is not associated with SourceMarket");
                            }
                            
                        }).catch(function (err) {

                            throw new Error("Currency  is not associated with Source Market");                            
                        });


                    SourceMarketCurrency.then(
                       function (sourceMarketCurrencyNameResponse) {
                           var Currency = JSON.parse(sourceMarketCurrencyNameResponse.response);

                           if (Currency != null && Currency.currencyname != null && Currencyid != null) {
                               var currencyReference = [];
                               currencyReference[0] = {};
                               currencyReference[0].id = Currencyid;
                               currencyReference[0].entityType = "transactioncurrency"; //YTODO: test
                               currencyReference[0].name = Currency.currencyname;

                               Xrm.Page.getControl(CASE_SOURCE_MARKET_CURRENCY).getAttribute().setValue(currencyReference);
                           }


                       }).catch(function (err) {

                           throw new Error("Currency  is not associated with Source Market");

                           //Exception Handling functionality, we can get exception message by using [err.message]

                       });

                }

            }
        }

        console.log("Get The Source Market Currency - End");
    }
    function getBooking(bookingId) {

        var query = "?$select=_tc_sourcemarketid_value";
        var entityName = "tc_bookings";
        var id = bookingId;
        return Tc.Crm.Scripts.Common.GetById(entityName, id, query);

    }
    function getSourceMarketCurrency(sourcMarketId) {
        var query = "?$select=_transactioncurrencyid_value";        
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
                            Xrm.Page.getControl("tc_istheholidaystoppingshorterthanplanned").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("tc_3rdpartyresponserequired").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("tc_preferredmethodofcommunication").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("caseorigincode").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("description").getAttribute().getValue() != null
                            ) {
                            Xrm.Page.getControl("tc_mandatoryconditionsmet").getAttribute().setValue(true);
                            Xrm.Page.data.entity.save();
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
                            Xrm.Page.getControl("tc_istheholidaystoppingshorterthanplanned").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("tc_3rdpartyresponserequired").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("tc_preferredmethodofcommunication").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("caseorigincode").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("tc_sourcemarketid").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("tc_brandid").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("tc_destinationid").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("tc_locationid").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("tc_gateway").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("description").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("tc_departuredate").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("tc_bookingtravelamount").getAttribute().getValue() != null &&
                            Xrm.Page.getControl("tc_durationofstay").getAttribute().getValue() != null
                            ) {
                            Xrm.Page.getControl("tc_mandatoryconditionsmet").getAttribute().setValue(true);
                            Xrm.Page.data.entity.save();
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
        var entityName = entityType;
        var id = customerId;
        return Tc.Crm.Scripts.Common.GetById(entityName, id, query);

    }
    var onChangeTelephone1 = function () {

        Tc.Crm.Scripts.Library.Contact.GetNotificationForPhoneNumber("tc_alternativephone");
    }
    var onChangeTelephone2 = function () {
        Tc.Crm.Scripts.Library.Contact.GetNotificationForPhoneNumber("tc_otherpartyphone");
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