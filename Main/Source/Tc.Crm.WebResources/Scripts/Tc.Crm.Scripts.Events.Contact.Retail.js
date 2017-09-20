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
scriptLoader.load("Tc.Crm.Scripts.Events.Contact.Retail", ["Tc.Crm.Scripts.Utils.Validation"], function () {

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

    if (typeof (Tc.Crm.Scripts.Events.Contact) === "undefined") {
        Tc.Crm.Scripts.Events.Contact = {
            __namespace: true
        };
    }

    Tc.Crm.Scripts.Events.Contact.Retail = (function () {
        "use strict";
        var Attributes = {
            Salutation: "tc_salutation",
            FirstName: "firstname",
            LastName: "lastname",
            BirthDate: "birthdate",
            ContactChangeReason: "tc_keycontactchangereason",
            Telephone1: "telephone1",
            Telephone2: "telephone2",
            Telephone3: "telephone3",
            MarketingConsentUpdatedOn: "tc_marketingupdated",
            MarketingConsentUpdatedBy: "tc_marketingupdatedby",
            MarketingConsentUpdatedCheckbox: "tc_marketingconsentupdated"
        }
        var FormMode = {
            Create: 1,
            Update: 2
        }
        var Tabs = {
            CustomerHolidays: "tab_customersholidays",
            Indicators: "tab_indicators",
            CustomerPreferences: "tab_customerpreferences",
            AdditionalCustomerDetails: "tab_additionalcustomerdetails",
            MarketingConsent: "tab_marketingconsent",
            PastHolidays: "tab_pastholidays",
            Cases: "tab_cases",
            ContactPreference: "tab_contactpreference",
            ExternalLogin: "tab_externallogin",
            PastBookings: "tab_bookings",
            Activities: "tab_activitiesnotes",
            CustomerDetails: "SUMMARY_TAB"
        }
        var Sections = {
        }
        var Messages = {
            MarketingConsent365DaysNotification: "As one year has elapsed, please check the Marketing Consent with the Customer",
            MarketingConsentNewlyMigratedNotification: "Please check the Marketing consent with the Customer",
            MarketingConsentContactDetailsChangedNotification: "As one or more of the customers address, telephone number(s) or email address have been updated, please ask the customer to re-select their marketing preferences"
        }

        function onPhoneChanged(context) {
            var attribute = context.getEventSource();
            Tc.Crm.Scripts.Utils.Validation.ValidatePhoneNumber(attribute.getName());
        }

        function hideTabsSections() {
            setTabVisibility(Tabs.CustomerHolidays, false);
            setTabVisibility(Tabs.Indicators, false);
            setTabVisibility(Tabs.CustomerPreferences, false);
            setTabVisibility(Tabs.AdditionalCustomerDetails, false);
            setTabVisibility(Tabs.MarketingConsent, false);
            setTabVisibility(Tabs.PastHolidays, false);
            setTabVisibility(Tabs.Cases, false);
            setTabVisibility(Tabs.ExternalLogin, false);
            setTabVisibility(Tabs.PastBookings, false);
            setTabVisibility(Tabs.Activities, false);
        }

        function showTabsSections() {
            setTabVisibility(Tabs.CustomerHolidays, true);
            setTabVisibility(Tabs.Indicators, true);
            setTabVisibility(Tabs.CustomerPreferences, true);
            setTabVisibility(Tabs.AdditionalCustomerDetails, true);
            setTabVisibility(Tabs.MarketingConsent, true);
            setTabVisibility(Tabs.PastHolidays, true);
            setTabVisibility(Tabs.Cases, true);
            setTabVisibility(Tabs.ExternalLogin, true);
            setTabVisibility(Tabs.PastBookings, true);
            setTabVisibility(Tabs.Activities, true);
        }

        function setTabVisibility(tabName, isVisible) {
            var tab = Xrm.Page.ui.tabs.get(tabName);
            if (tab) {
                tab.setVisible(isVisible);
            }
        }

        function toggleTab(tabName, isExpanded) {
            var tab = Xrm.Page.ui.tabs.get(tabName);
            if (tab) {
                tab.setVisible(true);
                if (isExpanded) {
                    tab.setDisplayState("expanded");
                }
                else {
                    tab.setDisplayState("collapsed");
                }
            }
        }

        var onCustomerKeyInformationUpdate = function () {
            if (isCustomerKeyInformationUpdated())
                showContactChangeReason();
            else
                hideContactChangeReason();
        };

        var hideContactChangeReason = function () {
            if (Xrm.Page.getAttribute(Attributes.ContactChangeReason)) {
                Xrm.Page.getAttribute(Attributes.ContactChangeReason).setRequiredLevel("none");
                Xrm.Page.getControl(Attributes.ContactChangeReason).setVisible(false);
            }
        };

        var showContactChangeReason = function () {
            if (isControlVisible(Attributes.ContactChangeReason))
                return;
            if (Xrm.Page.getAttribute(Attributes.ContactChangeReason)) {
                Xrm.Page.getAttribute(Attributes.ContactChangeReason).setValue(null);
                Xrm.Page.getControl(Attributes.ContactChangeReason).setVisible(true);
                Xrm.Page.getAttribute(Attributes.ContactChangeReason).setRequiredLevel("required");
            }
        };

        var isCustomerKeyInformationUpdated = function () {
            if (isFormTypeUpdate()) {
                var titleUpdated = getIsDirty(Attributes.Salutation);
                var firstNameUpdated = getIsDirty(Attributes.FirstName);
                var lastNameUpdated = getIsDirty(Attributes.LastName);
                var dateOfBirthUpdated = getIsDirty(Attributes.BirthDate);
                if (titleUpdated || firstNameUpdated || lastNameUpdated || dateOfBirthUpdated)
                    return true;
            }
            return false;
        };

        function is365DaysPassedSinceLastCheck(lastUpdatedDate) {
            var currentDate = new Date().setHours(0, 0, 0, 0);
            var millisecondsPerDay = 24 * 60 * 60 * 1000;

            var differenceInDays = (currentDate - lastUpdatedDate.setHours(0, 0, 0, 0)) / millisecondsPerDay;
            return differenceInDays >= 365;
        }

        function checkMarketingConsentOnLoad() {
            if (!Xrm.Page.getAttribute(Attributes.MarketingConsentUpdatedOn).getValue()) {
                Xrm.Page.ui.setFormNotification(Messages.MarketingConsentNewlyMigratedNotification, "WARNING", "MarketingConsentNotificationNewlyMigrated");
                Xrm.Page.getAttribute(Attributes.MarketingConsentUpdatedCheckbox).setValue(false);
            } else {
                if (!Xrm.Page.getAttribute(Attributes.MarketingConsentUpdatedCheckbox).getValue()) {
                    Xrm.Page.ui.setFormNotification(Messages.MarketingConsentContactDetailsChangedNotification, "WARNING", "MarketingConsentNotificationContactDetails");
                    if (is365DaysPassedSinceLastCheck(Xrm.Page.getAttribute(Attributes.MarketingConsentUpdatedOn).getValue())) {
                        Xrm.Page.ui.setFormNotification(Messages.MarketingConsent365DaysNotification, "WARNING", "MarketingConsentNotification365");
                    }
                }
            }
        }

        function marketingConsentCheckOnSave() {
            if (Xrm.Page.getAttribute(Attributes.MarketingConsentUpdatedCheckbox) && Xrm.Page.getAttribute(Attributes.MarketingConsentUpdatedCheckbox).getValue()) {
                updateMarketingConsentTimestamp();
                Xrm.Page.ui.clearFormNotification("MarketingConsentNotificationNewlyMigrated");
                Xrm.Page.ui.clearFormNotification("MarketingConsentNotificationContactDetails");
                Xrm.Page.ui.clearFormNotification("MarketingConsentNotification365");
            }
        }

        function updateMarketingConsentTimestamp() {
            Xrm.Page.getAttribute(Attributes.MarketingConsentUpdatedOn).setSubmitMode("always");
            Xrm.Page.getAttribute(Attributes.MarketingConsentUpdatedBy).setSubmitMode("always");
            Xrm.Page.getAttribute(Attributes.MarketingConsentUpdatedOn).setValue(new Date());

            var setUservalue = [];
            setUservalue[0] = {};
            setUservalue[0].id = Xrm.Page.context.getUserId();
            setUservalue[0].entityType = "systemuser";
            setUservalue[0].name = Xrm.Page.context.getUserName();

            Xrm.Page.getAttribute(Attributes.MarketingConsentUpdatedBy).setValue(setUservalue);
        }

        function marketingConsentFieldsOnChange() {
            if (isFormTypeUpdate()) {
                Xrm.Page.getAttribute(Attributes.MarketingConsentUpdatedCheckbox).setValue(true);
                Xrm.Page.getControl(Attributes.MarketingConsentUpdatedCheckbox).setDisabled(true);
            }
        }

        function customerDetailsMarketingConsentFieldsOnChange() {
            if (isFormTypeUpdate()) {
                Xrm.Page.getControl(Attributes.MarketingConsentUpdatedCheckbox).setDisabled(false);
                Xrm.Page.ui.setFormNotification(Messages.MarketingConsentContactDetailsChangedNotification, "WARNING", "MarketingConsentNotificationContactDetails");
                Xrm.Page.getAttribute(Attributes.MarketingConsentUpdatedCheckbox).setValue(false);
            }
        }

        var onLoad = function () {
            Tc.Crm.Scripts.Utils.Validation.ValidatePhoneNumber(Attributes.Telephone1);
            Tc.Crm.Scripts.Utils.Validation.ValidatePhoneNumber(Attributes.Telephone2);
            Tc.Crm.Scripts.Utils.Validation.ValidatePhoneNumber(Attributes.Telephone3);
            if (isFormTypeCreate()) {
                hideTabsSections();
                toggleTab(Tabs.CustomerDetails, true);
            }
            if (isFormTypeUpdate()) {
                showTabsSections();
                toggleTab(Tabs.CustomerDetails, false);
                checkMarketingConsentOnLoad();
            }
        }

        var onSave = function (econtext) {
            if (isCustomerKeyInformationUpdated()) {
                var eventArgs = econtext.getEventArgs();
                if (eventArgs.getSaveMode() != 70)//AutoSave
                {
                    if (!confirm("The changes you have requested will update this customer record only"))
                        noCloseCallback(econtext);
                }
                else {
                    eventArgs.preventDefault();
                }
            }
            if (isFormTypeUpdate()) {
                marketingConsentCheckOnSave();
            }
        };

        var noCloseCallback = function (econtext) {
            Xrm.Utility.alertDialog("You need to create a new customer record, do not save the changes you have made");
            var eventArgs = econtext.getEventArgs();
            eventArgs.preventDefault();
            Xrm.Page.data.refresh(false);
            //Xrm.Page.getAttribute(Attributes.BirthDate).setSubmitMode('never');
        };


        var isFormTypeUpdate = function () {
            var formType = Xrm.Page.ui.getFormType();
            return (formType === FormMode.Update)
        };

        var isFormTypeCreate = function () {
            var formType = Xrm.Page.ui.getFormType();
            return (formType === FormMode.Create)
        };

        var getIsDirty = function (controlName) {
            if (Xrm.Page.getAttribute(controlName))
                return Xrm.Page.getAttribute(controlName).getIsDirty()
            else
                return false;
        };

        var isControlVisible = function (controlName) {
            if (Xrm.Page.getControl(controlName))
                return Xrm.Page.getControl(controlName).getVisible();
            else
                return false;
        };
        var onModifiedOnChanged = function () {
            if (window.IsUSD == true) {
                var eventName ="RefreshFollowUp";

                window.open("http://event/?eventname="+eventName);

            }
        };


        // public methods
        return {
            OnLoad: onLoad,
            OnSave: function (context) {
                var isValid = Tc.Crm.Scripts.Utils.Validation.ValidateGdprCompliance(context);
                if (isValid) {
                    onSave(context);
                }
            },
            OnPhoneChanged: function (context) {
                if (context === null) {
                    console.log("Tc.Crm.Uk.Scripts.Events.Contact.OnPhoneChanged should be configured to pass execution context");
                    return;
                }
                onPhoneChanged(context);
            },
            OnCustomerKeyInformationUpdate: function () {
                onCustomerKeyInformationUpdate();
            },
            OnModifiedOnChanged: function () {
                onModifiedOnChanged();
            },
            OnChangeMarketingConsent: function() {
                marketingConsentFieldsOnChange();
            },
            OnChangeCustomerDetailsMarketingConsent: function() {
                customerDetailsMarketingConsentFieldsOnChange();
            }
        };
    })();

    // end script
    console.log('loaded events.contact.retail');
});