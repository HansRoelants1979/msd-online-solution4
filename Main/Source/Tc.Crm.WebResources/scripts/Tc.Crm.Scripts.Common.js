// JavaScript source code
//Common JavaScript file to do all CRUD operations using WebApi and Promise

var Tc = {

	Crm:
        {
        	Scripts:
                {
                	Common:
                        {
                        	/**
                             * @function Get                             
                             * @param {string} entityName - Name of the entity.
                             * @param {string} query - Query to Select, Expand so on.
                             * @returns {Promise} - A Promise that returns either the request object or an error object.
                             */
                        	Get: function (entityName, query) {
                        		var entityUri = "/" + entityName + "s";
                        		return Sdk.request("GET", entityUri + query, null);
                        	},
                        	/**
                             * @function Get                             
                             * @param {string} entityName - Name of the entity.
                             * @param {string} id - GUID of the Record.
                             * @param {string} query - Query to Select, Expand so on.
                             * @returns {Promise} - A Promise that returns either the request object or an error object.
                             */
                        	GetById: function (entityName, id, query) {
                        		var entityUri = "/" + entityName + "s(" + id + ")";
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
                             * @param {string} entityName - Name of the entity to Create.
                             * @param {object} data - An object representing an entity, required for Create action.
                             * @returns {Promise} - A Promise that returns either the request object or an error object.
                             */
                        	Create: function (entityName, object) {
                        		var entitySetName = "/" + entityName + "s";
                        		return Sdk.request("POST", entitySetName, object);
                        	},
                        	/**
                             * @function Update                             
                             * @param {string} entityName - Name of the entity to Update.
                             * @param {string} id - GUID of the Record to Update.
                             * @param {object} data - An object representing an entity, required for Update action.
                             * @returns {Promise} - A Promise that returns either the request object or an error object.
                             */
                        	Update: function (entityName, id, object) {
                        		var entityUri = "/" + entityName + "s(" + id + ")";
                        		return Sdk.request("PATCH", entityUri, object);
                        	},
                        	/**
                            * @function UpdateSingleAttribute                             
                            * @param {string} entityName - Name of the entity.
                            * @param {string} id - GUID of the Record to Update.
                            * @param {string} attribute - Attribute to Update.
                            * @param {string} value - Attribute Value to Update.
                            * @returns {Promise} - A Promise that returns either the request object or an error object.
                            */
                        	UpdateSingleAttribute: function (entityName, id, attribute, value) {
                        		var entityUri = "/" + entityName + "s(" + id + ")";
                        		return Sdk.request("PUT", entityUri + "/" + attribute, value);
                        	},
                        	/**
                            * @function Upsert                             
                            * @param {string} entityName - Name of the entity.
                            * @param {string} id - GUID of the Record to Create / Update.
                            * @param {object} data - An object representing an entity, required to Create / Update action.
                            * @returns {Promise} - A Promise that returns either the request object or an error object.
                            */
                        	Upsert: function (entityName, id, object) {
                        		var entityUri = "/" + entityName + "s(" + id + ")";
                        		return Sdk.request("PATCH", entityUri, object);
                        	},
                        	/**
                             * @function Delete                             
                             * @param {string} entityName - Name of the entity to Delete.
                             * @param {string} id - GUID of the Record to Delete.
                             * @returns {Promise} - A Promise that returns either the request object or an error object.
                             */
                        	Delete: function (entityName, id) {
                        		var entityUri = "/" + entityName + "s(" + id + ")";
                        		return Sdk.request("DELETE", entityUri, null);
                        	}

                        }
                }
        }
};


/*******************************************************************************************************/
/*******************************************************************************************************/
/*******************************************************************************************************/


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
};

// Global variables
var clientUrl = Sdk.getClientUrl();     // e.g.: https://org.crm.dynamics.com
var webAPIPath = "/api/data/v8.2";      // Path to the web API.


/**
 * @function request
 * @description Generic helper function to handle basic XMLHttpRequest calls.
 * @param {string} action - The request action. String is case-sensitive.
 * @param {string} uri - An absolute or relative URI. Relative URI starts with a "/".
 * @param {object} data - An object representing an entity. Required for create and update action.
 * @returns {Promise} - A Promise that returns either the request object or an error object.
 */
Sdk.request = function (action, uri, data) {
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

	// Construct a fully qualified URI if a relative URI is passed in.
	if (uri.charAt(0) === "/") {
		uri = clientUrl + webAPIPath + uri;
	}

	return new Promise(function (resolve, reject) {
		var request = new XMLHttpRequest();
		request.open(action, encodeURI(uri), true);
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
	});
};









