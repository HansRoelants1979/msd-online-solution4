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
scriptLoader.load("Tc.Crm.Scripts.Common", null, function () {
    // start script

    //Common JavaScript file to do all CRUD operations using WebApi and Promise
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

    Tc.Crm.Scripts.Common = (function () {
        "use strict";
        var Sdk = window.Sdk || {};

        /**
         * @function getClientUrl
         * @description Get the client URL.
         * @returns {string} The client URL.
         */
        Sdk.getClientUrl = function () {
            var context;
            // GetGlobalContext defined by including reference to 
            // ClientGlobalContext.js.aspx in the HTML page.
            if (typeof GetGlobalContext != "undefined") {
                context = GetGlobalContext();
            } else {
                if (typeof Xrm != "undefined") {
                    // Xrm.Page.context defined within the Xrm.Page object model for form scripts.
                    context = Xrm.Page.context;
                } else {
                    throw new Error("Context is not available.");
                }
            }
            return context.getClientUrl();
        }

        var clientUrl = Sdk.getClientUrl();     // e.g.: https://org.crm.dynamics.com
        var webAPIPath = "/api/data/v8.2";      // Path to the web API.

        /**
         * @function createXMLHttpRequest
         * @description Generic helper function to create  createXMLHttpRequest.
         * @param {bool} async - true if request should be asyncronous.
         * @param {string} action - The request action. String is case-sensitive.
         * @param {string} uri - An absolute or relative URI. Relative URI starts with a "/".
         * @param {object} data - content to be sent with request
         * @param {function} resolve - method to execute on success
         * @param {function} reject - method to execute on failure
         */
        Sdk.createXMLHttpRequest = function (async, action, uri, data, resolve, reject) {
            var request = new XMLHttpRequest();
            request.open(action, encodeURI(uri), async);
            request.setRequestHeader("OData-MaxVersion", "4.0");
            request.setRequestHeader("OData-Version", "4.0");
            request.setRequestHeader("Accept", "application/json");
            request.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            request.onreadystatechange = function () {
                if (this.readyState === 4) {
                    request.onreadystatechange = null;
                    switch (this.status) {
                        case 200: // Success with content returned in response body.
                        case 204: // Success with no content returned in response body.
                            resolve(this);
                            break;
                        default: // All other statuses are unexpected so are treated like errors.
                            var error;
                            try {
                                error = JSON.parse(request.response).error;
                            } catch (e) {
                                error = new Error("Unexpected Error");
                            }
                            reject(error);
                            break;
                    }
                }
            };
            request.send(JSON.stringify(data));
        }

        /**
         * @function validateParameters
         * @description Validate request parameters
         * @param {string} action - The request action. String is case-sensitive.
         * @param {string} uri - An absolute or relative URI. Relative URI starts with a "/".
         * @param {object} data - An object representing an entity. Required for create and update action.
         * @throws {error} - if request parameters are not valid
         */
        Sdk.validateParameters = function (action, uri, data) {
            if (!RegExp(action, "g").test("POST PATCH PUT GET DELETE")) { // Expected action verbs.
                throw new Error("Sdk.request: action parameter must be one of the following: " +
                    "POST, PATCH, PUT, GET, or DELETE.");
            }
            if (!typeof uri === "string") {
                throw new Error("Sdk.request: uri parameter must be a string.");
            }
            if ((RegExp(action, "g").test("POST PATCH PUT")) && (data === null || data === undefined)) {
                throw new Error("Sdk.request: data parameter must not be null for operations that create or modify data.");
            }
        }

        /**
         * @function request
         * @description Generic helper function to handle basic asyncronous XMLHttpRequest calls.
         * @param {string} action - The request action. String is case-sensitive.
         * @param {string} uri - An absolute or relative URI. Relative URI starts with a "/".
         * @param {object} data - An object representing an entity. Required for create and update action.
         * @returns {Promise} - A Promise that returns either the request object or an error object.
         */
        Sdk.request = function (action, uri, data) {
            Sdk.validateParameters(action, uri, data);
            // Construct a fully qualified URI if a relative URI is passed in.
            if (uri.charAt(0) === "/") {
                uri = clientUrl + webAPIPath + uri;
            }
            return new Promise(function (resolve, reject) {
                Sdk.createXMLHttpRequest(true, action, uri, data, resolve, reject);
            });
        }

        /**
         * @function requestSync
         * @description Generic helper function to handle basic syncronous XMLHttpRequest calls.
         * @param {string} action - The request action. String is case-sensitive.
         * @param {string} uri - An absolute or relative URI. Relative URI starts with a "/".
         * @param {object} data - An object representing an entity. Required for create and update action.
         * @returns {Object} - Result of request.
         */
        Sdk.requestSync = function (action, uri, data) {
            Sdk.validateParameters(action, uri, data);

            // Construct a fully qualified URI if a relative URI is passed in.
            if (uri.charAt(0) === "/") {
                uri = clientUrl + webAPIPath + uri;
            }

            var result;
            Sdk.createXMLHttpRequest(false, action, uri, data, function (response) {
                result = JSON.parse(response.response);
            }, function (error) {
                throw error;
            });
            return result;
        }

        // public members
        return {
            /**
                * @function Get                             
                * @param {string} entityName - Entity set name as per /api/data/v8.2/EntityDefinitions.
                * @param {string} query - Query to Select, Expand so on.
                * @returns {Promise} - A Promise that returns either the request object or an error object.
                */
            Get: function (entityName, query) {
                var entityUri = "/" + entityName;
                return Sdk.request("GET", entityUri + query, null);
            },
            /**
                * @function SyncGet - execute get request syncronously                             
                * @param {string} entityName - Entity set name as per /api/data/v8.2/EntityDefinitions.
                * @param {string} query - Query to Select, Expand so on.
                * @returns {Object} - Result of request.
                * @throws {exception} if failed to process for any reason
                */
            SyncGet: function (entityName, query) {
                var entityUri = "/" + entityName;
                return Sdk.requestSync("GET", entityUri + query, null);
            },
            /**
                * @function Get                             
                * @param {string} entityName - Entity set name as per /api/data/v8.2/EntityDefinitions.
                * @param {string} id - GUID of the Record.
                * @param {string} query - Query to Select, Expand so on.
                * @returns {Promise} - A Promise that returns either the request object or an error object.
                */
            GetById: function (entityName, id, query) {
                var entityUri = "/" + entityName + "(" + id + ")";
                return Sdk.request("GET", entityUri + query, null);
            },

            /**
                * @function GetByUri                             
                * @param {string} entityUri - An absolute or relative URI. Relative URI starts with a "/".
                * @param {string} query - Query to Select, Expand so on.                           
                * @returns {Promise} - A Promise that returns either the request object or an error object.
                */
            GetByUri: function (entityUri, query) {
                return Sdk.request("GET", entityUri + query, null);
            },
            /**
                * @function Create                             
                * @param {string} entityName - Entity set name as per /api/data/v8.2/EntityDefinitions.
                * @param {object} data - An object representing an entity, required for Create action.
                * @returns {Promise} - A Promise that returns either the request object or an error object.
                */
            Create: function (entityName, object) {
                var entitySetName = "/" + entityName;
                return Sdk.request("POST", entitySetName, object);
            },
            /**
                * @function Update                             
                * @param {string} entityName - Entity set name as per /api/data/v8.2/EntityDefinitions.
                * @param {string} id - GUID of the Record to Update.
                * @param {object} data - An object representing an entity, required for Update action.
                * @returns {Promise} - A Promise that returns either the request object or an error object.
                */
            Update: function (entityName, id, object) {
                var entityUri = "/" + entityName + "(" + id + ")";
                return Sdk.request("PATCH", entityUri, object);
            },
            /**
            * @function UpdateSingleAttribute                             
            * @param {string} entityName - Entity set name as per /api/data/v8.2/EntityDefinitions.
            * @param {string} id - GUID of the Record to Update.
            * @param {string} attribute - Attribute to Update.
            * @param {string} value - Attribute Value to Update.
            * @returns {Promise} - A Promise that returns either the request object or an error object.
            */
            UpdateSingleAttribute: function (entityName, id, attribute, value) {
                var entityUri = "/" + entityName + "(" + id + ")";
                return Sdk.request("PUT", entityUri + "/" + attribute, value);
            },
            /**
            * @function Upsert                             
            * @param {string} entityName - Entity set name as per /api/data/v8.2/EntityDefinitions.
            * @param {string} id - GUID of the Record to Create / Update.
            * @param {object} data - An object representing an entity, required to Create / Update action.
            * @returns {Promise} - A Promise that returns either the request object or an error object.
            */
            Upsert: function (entityName, id, object) {
                var entityUri = "/" + entityName + "(" + id + ")";
                return Sdk.request("PATCH", entityUri, object);
            },
            /**
                * @function Delete                             
                * @param {string} entityName - Entity set name as per /api/data/v8.2/EntityDefinitions.
                * @param {string} id - GUID of the Record to Delete.
                * @returns {Promise} - A Promise that returns either the request object or an error object.
                */
            Delete: function (entityName, id) {
                var entityUri = "/" + entityName + "(" + id + ")";
                return Sdk.request("DELETE", entityUri, null);
            }
        };
    })();

    // end script
    console.log('loaded common');
});