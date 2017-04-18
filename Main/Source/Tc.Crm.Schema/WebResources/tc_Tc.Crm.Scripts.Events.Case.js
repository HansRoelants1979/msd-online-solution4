//<script src ="scripts/es6promise.js" type ="text/javascript"></script>
//<script src ="scripts/Tc.Crm.Scripts.Common.js" type ="text/javascript"></script>

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

    var CASE_TYPE_CONTROL_BOOKINGREF = "tc_bookingreference";
    var CASE_BOOKING_NUMBER = "tc_bookingid";
    var CASE_SOURCE_MARKET_ID = "tc_sourcemarketid";
    var CASE_SOURCE_MARKET_CURRENCY = "transactioncurrencyid";
    var CASE_ENTITY_NAME = "incident";
    var CASE_BOOKING_ENTITY_NAME = "tc_booking"
    var CASE_SOURCE_MARKET_ENTITY_NAME = "tc_country"
    var FORM_MODE_CREATE = 1;
    var FORM_MODE_UPDATE = 2;

    var GetTheSourceMarketCurrency = function () {


        
        console.log("Get The Source Market Currency - Start");
        if (Xrm.Page.getControl(CASE_TYPE_CONTROL_BOOKINGREF).getAttribute().getValue() == true) {
            debugger;

            if (Xrm.Page.getControl(CASE_BOOKING_NUMBER).getAttribute().getValue() != null) {

                var BookingId = Xrm.Page.getControl(CASE_BOOKING_NUMBER).getAttribute().getValue()[0].id;
                if (BookingId != null) {

                    BookingId = BookingId.replace("{", "").replace("}", "");
                    debugger;

                    var SourceMarketReceivedPromise = getBooking(BookingId).then(
                        function (bookingResponse) {
                            var booking = JSON.parse(bookingResponse.response);

                            var sourceMarketId = booking._tc_sourcemarketid_value;
                            if (sourceMarketId != null) {
                                return getSourceMarketCurrency(sourceMarketId);
                            }



                            alert("success");
                        }).catch(function (err) {

                            alert(err.message);

                            //Exception Handling functionality, we can get exception message by using [err.message]

                        });

                    var Currencyid ;
                   var SourceMarketCurrency = SourceMarketReceivedPromise.then(
                        function (sourceMarketResponse) {
                            var sourceMarket = JSON.parse(sourceMarketResponse.response);

                             Currencyid = sourceMarket._transactioncurrencyid_value;

                            if (Currencyid != null) {
                                return getSourceMarketCurrencyname(Currencyid);
                            }
                            //var CurrencyName = sourceMarket._transactioncurrencyid;
                            //alert(CurrencyName);

                            //if(Currencyid != null)
                            //{
                            //    var currencyReference = [];
                            //    currencyReference[0] = {};
                            //    currencyReference[0].id = Currencyid;
                            //    currencyReference[0].entityType = "transactioncurrency"; //YTODO: test
                            //    currencyReference[0].name = CurrencyName.currencyname;

                            //    Xrm.Page.getControl(CASE_SOURCE_MARKET_CURRENCY).getAttribute().setValue(currencyReference);
                            //}

                            //return getSourceMarketCurrency(sourceMarketId);
                            // alert("success");
                        }).catch(function (err) {

                            alert(err.message);

                            //Exception Handling functionality, we can get exception message by using [err.message]

                        });

                    SourceMarketCurrency.then(
                       function (sourceMarketCurrencyNameResponse) {
                           var Currency = JSON.parse(sourceMarketCurrencyNameResponse.response);

                           

                           //var CurrencyName = sourceMarket._transactioncurrencyid;
                           //alert(CurrencyName);

                           if (Currency != null && Currency.currencyname != null && Currencyid !=null)
                           {
                               var currencyReference = [];
                               currencyReference[0] = {};
                               currencyReference[0].id = Currencyid;
                               currencyReference[0].entityType = "transactioncurrency"; //YTODO: test
                               currencyReference[0].name = Currency.currencyname;

                               Xrm.Page.getControl(CASE_SOURCE_MARKET_CURRENCY).getAttribute().setValue(currencyReference);
                           }

                           //return getSourceMarketCurrency(sourceMarketId);
                           // alert("success");
                       }).catch(function (err) {

                           alert(err.message);

                           //Exception Handling functionality, we can get exception message by using [err.message]

                       });

                    //var sourcMarketId = booking._tc_sourcemarketid_value;

                    //return getSourceMarket(sourcMarketId);
                    //},
                    //    function (error) {
                    //        // unable to get the case
                    //        return error.message;
                    //        //throw error;
                    //    }
                    //);






                }

            }

        }

        console.log("Get The Source Market Currency - End");
    }


    function getBooking(bookingId) {

        var query = "?$select=_tc_sourcemarketid_value";

        var entityName = "tc_booking";

        var id = bookingId;


        return Tc.Crm.Scripts.Common.GetById(entityName, id, query);




    }

    function getSourceMarketCurrency(sourcMarketId) {
        var query = "?$select=_transactioncurrencyid_value";
        //var query = "?$select=_transactioncurrencyid";
        //var query = "?$select=_TransactionCurrencyId";
        //var query = "?$select=transactioncurrencyid";
        var entityName = "tc_countrie";

        return Tc.Crm.Scripts.Common.GetById(entityName, sourcMarketId, query);
    }


    function getSourceMarketCurrencyname(Currencyid)
    {
        var query = "?$select=currencyname";
        //var query = "?$select=_transactioncurrencyid";
        //var query = "?$select=_TransactionCurrencyId";
        //var query = "?$select=transactioncurrencyid";
        var entityName = "transactioncurrency";

        return Tc.Crm.Scripts.Common.GetById(entityName, Currencyid, query);
    }


    // public methods
    return {
        OnLoad: function () {

        },
        OnCaseSave: function () {

        },
        OnCaseFieldChange: function () {
            GetTheSourceMarketCurrency();
        }
    };
})();