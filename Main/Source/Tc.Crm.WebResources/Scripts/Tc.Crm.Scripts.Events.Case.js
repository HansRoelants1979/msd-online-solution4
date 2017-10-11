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
scriptLoader.load("Tc.Crm.Scripts.Events.Case", ["Tc.Crm.Scripts.Utils.Validation", "Tc.Crm.Scripts.Library.Case"], function () {

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

        var tcProductRibbonButtonEnabled = null;

        var ClientState = {
            Offline: "Offline"
        };

        var CaseType = {
            Incident: "Incident",
            Complaint: "Complaint"
        };

        var SecurityRole = {
            SystemAdministrator: "System Administrator",
            HealthSafety: "Tc.Group.HealthSafety",
            SystemMaintenance: "Tc.Administator.SystemMaintenance"
        };

        var FormMode = {
            Create: 1,
            Update: 2
        };

        var EntityNames = {
            TransactionCurrency: "transactioncurrency",
            LocationOffice: "tc_locationoffice",
            Gateway: "tc_gateway",
            GatewayLocationOffice: "tc_gateway_tc_locationoffice"
        };

        var EntitySetNames = {
            Case: "incidents",
            SecurityRole: "roles",
            Booking: "tc_bookings",
            Country: "tc_countries",
            TransactionCurrency: "transactioncurrencies"
        }

        var ProductTypes = {
            TcProduct: 950000000,
            NonTcProduct: 950000001
        };

        var Attributes = {
            AlternativePhone: "tc_alternativephone",
            OtherPartyPhone: "tc_otherpartyphone",
            ArrivalDate: "tc_arrivaldate",
            DepartureDate: "tc_departuredate",
            RepNotes: "tc_reportnotes",
            TcProductRelated: "tc_tcproductrelated",
            BookingReference: "tc_bookingreference",
            BookingId: "tc_bookingid",
            SourceMarketId: "tc_sourcemarketid",
            SourceMarketCurrency: "transactioncurrencyid",
            CaseType: "tc_casetypeid",
            Gateway: "tc_gateway",
            ResortOffice: "tc_resortofficeid",
            ProductType: "tc_producttype",
            ReportedDate: "tc_datereported",
            MandatoryConditions: "tc_mandatoryconditionsmet",
            Description: "description",
            ThirdPartyResponse: "tc_3rdpartyresponserequired",
            CaseOriginCode: "caseorigincode",
            CommunicationMethod: "tc_preferredmethodofcommunication",
            BrandId: "tc_brandid",
            Destination: "tc_destinationid",
            Location: "tc_locationid",
            Customer: "customerid",
            IsHolidayStopping: "tc_istheholidaystoppingshorterthanplanned",
            BookingTravelAmount: "tc_bookingtravelamount",
            DurationOfStay: "tc_durationofstay",
            LocationOfficeId: "tc_locationofficeid",
            Name: "tc_name",
            Address1FlatNumber: "tc_address1_flatorunitnumber",
            Address1HouseNumber: "tc_address1_housenumberorbuilding",
            Address1Street: "tc_address1_street",
            Address1Town: "tc_address1_town",
            Address1PostCode: "tc_address1_postcode",
            Address1County: "tc_address1_county",
            Address1Country: "tc_address1_country",
            Address1AdditionalInformation: "tc_address1_additionalinformation",
            GatewayId: "tc_gatewayid"
        }

        function OnLoad() {
            Tc.Crm.Scripts.Utils.Validation.ValidatePhoneNumber(Attributes.AlternativePhone);
            Tc.Crm.Scripts.Utils.Validation.ValidatePhoneNumber(Attributes.OtherPartyPhone);
            validateCaseAssociatedCustomerPhoneNum();
            preFilterLocationOfficeLookup();
            var currentItem = Xrm.Page.ui.formSelector.getCurrentItem();
            if (currentItem == null || currentItem.getLabel() === "iDS Case") {
                setNotification();
            }
        }

        var setNotification = function () {
            if (Xrm.Page.getAttribute(Attributes.RepNotes)) {
                if (Xrm.Page.getAttribute(Attributes.RepNotes).getValue() == null) {
                    Xrm.Page.ui.setFormNotification(" You must provide a value for Rep Notes to Resolve Case. ", "WARNING", 'note');
                } else {
                    Xrm.Page.ui.clearFormNotification('note');
                }
            }
        }

        var GetTheSourceMarketCurrency = function () {
            var sourceMarketId;
            var currencyId;
            console.log("Get The Source Market Currency - Start");
            if (Xrm.Page.getAttribute(Attributes.BookingReference).getValue() == true) {

                if (Xrm.Page.getAttribute(Attributes.BookingId).getValue() != null) {

                    var bookingId = Xrm.Page.getAttribute(Attributes.BookingId).getValue()[0].id;
                    if (bookingId != null) {
                        bookingId = bookingId.replace("{", "").replace("}", "");
                        var SourceMarketReceivedPromise = getBooking(bookingId).then(
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
                                currencyReference[0].entityType = EntityNames.TransactionCurrency;
                                currencyReference[0].name = Currency.currencyname;

                                Xrm.Page.getAttribute(Attributes.SourceMarketCurrency).setValue(currencyReference);
                            }
                        }).catch(function (err) {
                            throw new Error("Problem in retrieving the Source Market Currency");
                        });
                    }
                }
            }
            else {
                if (Xrm.Page.getAttribute(Attributes.SourceMarketId).getValue() != null) {

                    sourceMarketId = Xrm.Page.getAttribute(Attributes.SourceMarketId).getValue()[0].id;
                    if (sourceMarketId != null) {
                        sourceMarketId = sourceMarketId.replace("{", "").replace("}", "");
                        getSourceMarketCurrency(sourceMarketId).then(
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
                                   currencyReference[0].entityType = EntityNames.TransactionCurrency;
                                   currencyReference[0].name = currencyName;

                                   Xrm.Page.getAttribute(Attributes.SourceMarketCurrency).setValue(currencyReference);
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
            return Tc.Crm.Scripts.Common.GetById(EntitySetNames.Booking, bookingId, query);
        }

        function getSourceMarketCurrency(sourcMarketId) {
            var query = "?$select=_transactioncurrencyid_value&$expand=transactioncurrencyid($select=currencyname)";
            return Tc.Crm.Scripts.Common.GetById(EntitySetNames.Country, sourcMarketId, query);
        }

        function getSourceMarketCurrencyname(currencyId) {
            var query = "?$select=currencyname";
            return Tc.Crm.Scripts.Common.GetById(EntitySetNames.TransactionCurrency, currencyId, query);
        }

        function mandatoryMetConditions() {
            if (Xrm.Page.getControl(Attributes.BookingReference).getAttribute().getValue() == true) {
                if (Xrm.Page.getControl(Attributes.MandatoryConditions).getAttribute().getValue() == false) {
                    if (Xrm.Page.getControl(Attributes.CaseType).getAttribute().getValue() != null) {
                        if (Xrm.Page.getControl(Attributes.CaseType).getAttribute().getValue()[0].name == CaseType.Complaint) {
                            if (Xrm.Page.getControl(Attributes.ResortOffice).getAttribute().getValue() != null &&
                                Xrm.Page.getControl(Attributes.ReportedDate).getAttribute().getValue() != null &&
                                Xrm.Page.getControl(Attributes.CommunicationMethod).getAttribute().getValue() != null &&
                                Xrm.Page.getControl(Attributes.CaseOriginCode).getAttribute().getValue() != null &&
                                Xrm.Page.getControl(Attributes.Description).getAttribute().getValue() != null
                                ) {
                                if (Xrm.Page.getControl(Attributes.ProductType).getAttribute().getValue() == null || Xrm.Page.getControl(Attributes.ProductType).getAttribute().getValue() != null &&
                                    Xrm.Page.getControl(Attributes.ProductType).getAttribute().getValue() != "950000000") {
                                    if (Xrm.Page.getControl(Attributes.IsHolidayStopping).getAttribute().getValue() != null &&
                                        Xrm.Page.getControl(Attributes.ThirdPartyResponse).getAttribute().getValue() != null) {
                                        Xrm.Page.getControl(Attributes.MandatoryConditions).getAttribute().setValue(true);
                                        Xrm.Page.data.entity.save();
                                    }
                                }
                                else {
                                    Xrm.Page.getControl(Attributes.MandatoryConditions).getAttribute().setValue(true);
                                    Xrm.Page.data.entity.save();
                                }
                            }
                        }
                    }
                }
            }

            else {
                if (Xrm.Page.getControl(Attributes.CaseType).getAttribute().getValue() != null) {
                    if (Xrm.Page.getControl(Attributes.MandatoryConditions).getAttribute().getValue() == false) {
                        if (Xrm.Page.getControl(Attributes.CaseType).getAttribute().getValue()[0].name == CaseType.Complaint) {
                            if (Xrm.Page.getControl(Attributes.ResortOffice).getAttribute().getValue() != null &&
                                Xrm.Page.getControl(Attributes.ReportedDate).getAttribute().getValue() != null &&
                                Xrm.Page.getControl(Attributes.CommunicationMethod).getAttribute().getValue() != null &&
                                Xrm.Page.getControl(Attributes.CaseOriginCode).getAttribute().getValue() != null &&
                                Xrm.Page.getControl(Attributes.SourceMarketId).getAttribute().getValue() != null &&
                                Xrm.Page.getControl(Attributes.BrandId).getAttribute().getValue() != null &&
                                Xrm.Page.getControl(Attributes.Destination).getAttribute().getValue() != null &&
                                Xrm.Page.getControl(Attributes.Location).getAttribute().getValue() != null &&
                                Xrm.Page.getControl(Attributes.Gateway).getAttribute().getValue() != null &&
                                Xrm.Page.getControl(Attributes.Description).getAttribute().getValue() != null &&
                                Xrm.Page.getControl(Attributes.DepartureDate).getAttribute().getValue() != null &&
                                Xrm.Page.getControl(Attributes.BookingTravelAmount).getAttribute().getValue() != null
                                 ) {
                                if (Xrm.Page.getControl(Attributes.ProductType).getAttribute().getValue() == null || Xrm.Page.getControl(Attributes.ProductType).getAttribute().getValue() != null &&
                                    Xrm.Page.getControl(Attributes.ProductType).getAttribute().getValue() != "950000000") {
                                    if (Xrm.Page.getControl(Attributes.IsHolidayStopping).getAttribute().getValue() != null &&
                                        Xrm.Page.getControl(Attributes.ThirdPartyResponse).getAttribute().getValue() != null &&
                                        Xrm.Page.getControl(Attributes.DurationOfStay).getAttribute().getValue() != null) {
                                        Xrm.Page.getControl(Attributes.MandatoryConditions).getAttribute().setValue(true);
                                        Xrm.Page.data.entity.save();
                                    }
                                }
                                else {
                                    Xrm.Page.getControl(Attributes.MandatoryConditions).getAttribute().setValue(true);
                                    Xrm.Page.data.entity.save();
                                }
                            }
                        }
                    }
                }
            }
        }

        function validateCaseAssociatedCustomerPhoneNum() {
            var customer = Xrm.Page.getAttribute(Attributes.Customer).getValue();
            if (customer == null)
                return;
            var customerId = customer[0].id;
            if (customerId == null || customerId == "")
                return;
            customerId = customerId.replace("{", "").replace("}", "");
            var entityType = customer[0].entityType;
            if (entityType == null || entityType == "")
                return;

            getCustomerTelephoneNum(customerId, entityType).then(
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

        function getCustomerTelephoneNum(customerId, entityType) {

            var query = "?$select=telephone1";
            var entityName = entityType + "s";
            if (isOfflineMode()) {
                return Xrm.Mobile.offline.retrieveRecord(entityName, customerId, query);
            }
            else {
                return Tc.Crm.Scripts.Common.GetById(entityName, customerId, query);
            }
        }

        var onChangeTelephone1 = function () {
            Tc.Crm.Scripts.Utils.Validation.ValidatePhoneNumber(Attributes.AlternativePhone);
        }

        var onChangeTelephone2 = function () {
            Tc.Crm.Scripts.Utils.Validation.ValidatePhoneNumber(Attributes.OtherPartyPhone);
        }

        var preFilterLocationOfficeLookup = function () {
            if (!Xrm.Page.getAttribute(Attributes.ResortOffice)) return;
            Xrm.Page.getControl(Attributes.ResortOffice).addPreSearch(function () {
                filterLocationOfficeBasedOnSelectedGateway();
            });
        }

        var filterLocationOfficeBasedOnSelectedGateway = function () {
            if (!Xrm.Page.getAttribute(Attributes.ResortOffice)) return;
            if (Xrm.Page.getAttribute(Attributes.Gateway) && Xrm.Page.getAttribute(Attributes.Gateway).getValue() && Xrm.Page.getAttribute(Attributes.Gateway).getValue().length > 0) {
                var gatewayId = Xrm.Page.getAttribute(Attributes.Gateway).getValue()[0].id;
                addCustomFilterForLocationOffice(gatewayId);
            }
            else {
                getGateWayFromBooking();
            }
        }

        var getGateWayFromBooking = function () {
            if (Xrm.Page.getAttribute(Attributes.BookingId) && Xrm.Page.getAttribute(Attributes.BookingId).getValue() && Xrm.Page.getAttribute(Attributes.BookingId).getValue().length > 0) {
                var bookingId = Xrm.Page.getAttribute(Attributes.BookingId).getValue()[0].id;
                var query = "?$select=_tc_destinationgatewayid_value";
                var id = formatEntityId(bookingId);
                if (isOfflineMode()) {
                    processCustomFilterPromise(Xrm.Mobile.offline.retrieveRecord(EntitySetNames.Booking, id, query));
                }
                else {
                    processCustomFilterPromise(Tc.Crm.Scripts.Common.GetById(EntitySetNames.Booking, id, query));
                }
            }
        }

        var processCustomFilterPromise = function (promise) {
            promise.then(function (request) {
                var booking = JSON.parse(request.response);
                if (booking && booking._tc_destinationgatewayid_value) {
                    var gatewayId = booking._tc_destinationgatewayid_value;
                    addCustomFilterForLocationOffice(gatewayId);
                }

            }).catch(function (err) {
                console.log("ERROR: " + err.message);
            });
        }

        var addCustomFilterForLocationOffice = function (gatewayId) {
            var fetchXml = "<fetch distinct='true' mapping='logical' output-format='xml-platform' version='1.0'>" +
                            "<entity name='" + EntityNames.LocationOffice + "'>" +
                            "<attribute name='" + Attributes.LocationOfficeId + "'/>" +
                            "<attribute name='" + Attributes.Name + "'/>" +
                            "<attribute name='" + Attributes.Address1FlatNumber + "'/>" +
                            "<attribute name='" + Attributes.Address1HouseNumber + "'/>" +
                            "<attribute name='" + Attributes.Address1Street + "'/>" +
                            "<attribute name='" + Attributes.Address1Town + "'/>" +
                            "<attribute name='" + Attributes.Address1PostCode + "'/>" +
                            "<attribute name='" + Attributes.Address1County + "'/>" +
                            "<attribute name='" + Attributes.Address1Country + "'/>" +
                            "<attribute name='" + Attributes.Address1AdditionalInformation + "'/>" +
                            "<order descending='false' attribute='" + Attributes.Name + "'/>" +
                            "<link-entity name='" + EntityNames.GatewayLocationOffice + "' intersect='true' visible='false' to='" + Attributes.LocationOfficeId + "' from='" + Attributes.LocationOfficeId + "'>" +
                                "<link-entity name='" + EntityNames.Gateway + "' to='" + Attributes.GatewayId + "' from='" + Attributes.GatewayId + "' alias='aa'>" +
                                    "<filter type='and'>" +
                                        "<condition attribute='" + Attributes.GatewayId + "' value='" + gatewayId + "' operator='eq'/>" +
                                    "</filter>" +
                                "</link-entity>" +
                           "</link-entity>" +
                           "</entity>" +
                           "</fetch>";

            var layoutXml = "<grid name='resultset' object='1' jump='" + Attributes.LocationOfficeId + "' select='1' icon='1' preview='1'>" +
                            "<row name='result' id='" + Attributes.LocationOfficeId + "'>" +
                            "<cell name='" + Attributes.Name + "' width='90' />" +
                            "<cell name='" + Attributes.Address1FlatNumber + "' width='90' />" +
                            "<cell name='" + Attributes.Address1HouseNumber + "' width='90' />" +
                            "<cell name='" + Address1Street + "' width='100' />" +
                            "<cell name='" + Attributes.Address1Town + "' width='100' />" +
                            "<cell name='" + Attributes.Address1PostCode + "' width='90' />" +
                            "<cell name='" + Attributes.Address1County + "' width='100' />" +
                            "<cell name='" + Attributes.Address1Country + "' width='100' />" +
                            "<cell name='" + Attributes.Address1AdditionalInformation + "' width='130' />" +
                            "</row>" +
                            "</grid>";

            Xrm.Page.getControl(Attributes.ResortOffice).addCustomView("{6fd72744-3676-41d4-8003-ae4cde9ac282}", EntityNames.LocationOffice, "Associated Offices Of Gateway", fetchXml, layoutXml, true);
        }

        var formatEntityId = function (id) {
            return id !== null ? id.replace("{", "").replace("}", "") : null;
        }

        function isOfflineMode() {
            return Xrm.Page.context.client.getClientState() === ClientState.Offline;
        }

        var getNewTcProductValue = function (tcProductValue) {
            if (tcProductValue === ProductTypes.TcProduct)
                return ProductTypes.NonTcProduct;
            else
                return ProductTypes.TcProduct;
        }

        var getControlValue = function (controlName) {
            return Xrm.Page.getAttribute(controlName) ? Xrm.Page.getAttribute(controlName).getValue() : null;
        }

        var setTcProductRelatedFieldValue = function () {
            var tcProductRelatedFieldValue = getControlValue(Attributes.TcProductRelated);
            if (tcProductRelatedFieldValue == null)
                return;
            Xrm.Page.getAttribute(Attributes.TcProductRelated).setValue(getNewTcProductValue(tcProductRelatedFieldValue));
            Xrm.Page.data.entity.save();
        }

        var isIncidentType = function () {
            var caseIdAttr = Xrm.Page.getAttribute(Attributes.CaseType);
            if (caseIdAttr == null)
                return false;

            var value = caseIdAttr.getValue();
            if (value == null || value.length === 0)
                return false;

            return value[0].name === CaseType.Incident;
        }

        var onTcProductRibbonButtonEnabled = function () {
            if (!isIncidentType()) {
                tcProductRibbonButtonEnabled = false;
                return;
            }

            var userRole = window.parent.Xrm.Page.context.getUserRoles();
            if (userRole == null || userRole === "") {
                tcProductRibbonButtonEnabled = false;
                return;
            }

            var promises = [];
            var query = "?$select=name";
            for (var i = 0; i < userRole.length; i++) {
                var promise = Tc.Crm.Scripts.Common.GetById(EntitySetNames.SecurityRole, userRole[i], query);
                promises.push(promise);
            }
            Promise.all(promises).then(
                function (securityRoleNameResponse) {
                    if (securityRoleNameResponse == null) {
                        console.warn("Problem getting security roles");
                        return;
                    }
                    for (var i = 0; i < securityRoleNameResponse.length; i++) {
                        var role = JSON.parse(securityRoleNameResponse[i].response);
                        if (role == null || role == "" || role == "undefined") continue;
                        if (role.name === SecurityRole.SystemAdministrator || role.name === SecurityRole.HealthSafety || role.name === SecurityRole.SystemMaintenance) {
                            tcProductRibbonButtonEnabled = true;
                            break;
                        }
                    }
                    if (tcProductRibbonButtonEnabled == null) tcProductRibbonButtonEnabled = false;
                    Xrm.Page.ui.refreshRibbon();
                },
                function (error) {
                    console.warn("Problem getting case lines");
                    tcProductRibbonButtonEnabled = false;
                });
        }

        var validateArrivalDateGreaterOrEqualDeparture = function () {
            var arrivalDateControl = Xrm.Page.getControl(Attributes.ArrivalDate);
            var arrivalDate = Xrm.Page.data.entity.attributes.get(Attributes.ArrivalDate).getValue();
            var departureDate = Xrm.Page.data.entity.attributes.get(Attributes.DepartureDate).getValue();

            arrivalDateControl.clearNotification();
            if (arrivalDate != null && departureDate != null) {
                if (arrivalDate.setHours(0, 0, 0, 0) > departureDate.setHours(0, 0, 0, 0)) {
                    arrivalDateControl.setNotification("Departure date should be equal or greater than Arrival date");
                }
            }
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
            OnSave: function (context) {
                var isValid = Tc.Crm.Scripts.Utils.Validation.ValidateGdprCompliance(context);
                if (isValid) {
                    if (!isOfflineMode()) {
                        Tc.Crm.Scripts.Library.Case.UpdateRelatedCompensationsSourceMarket();
                    }
                }
            },
            OnValidateRepNotes: function () {
                var currentItem = Xrm.Page.ui.formSelector.getCurrentItem();
                if (currentItem == null || currentItem.getLabel() === "iDS Case") {
                    var repNotes = Xrm.Page.data.entity.attributes.get(Attributes.RepNotes);
                    return repNotes != null && repNotes.getValue() != null && repNotes.getValue() !== "";
                }
                return true;
            },
            OnCaseFieldChange: function () {
                GetTheSourceMarketCurrency();
            },
            OnCaseFieldChangeMandatoryMetConditions: function () {
                mandatoryMetConditions();
            },
            OnChangeCustomer: function () {
                validateCaseAssociatedCustomerPhoneNum();
            },
            OnChangeSourceMarket: function () {
                if (isOfflineMode()) {
                    Tc.Crm.Scripts.Library.Case.UpdateRelatedCompensationsSourceMarket();
                }
            },
            OnChangeArrivalOrDepartureDates: function () {
                validateArrivalDateGreaterOrEqualDeparture();
            },
            OnTcProductRibbonButtonClick: function () {
                setTcProductRelatedFieldValue();
            },
            EnableTcProductRibbonButton: function () {
                if (!isOfflineMode()) {
                    if (tcProductRibbonButtonEnabled == null) {
                        onTcProductRibbonButtonEnabled();
                    }
                    return tcProductRibbonButtonEnabled;
                } else return false;
            }
        };
    })();

    // end script
    console.log('loaded events.case');
});