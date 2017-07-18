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

Tc.Crm.Scripts.Events.SurveyResponse = (function () {
	"use strict";
	var FORM_MODE_CREATE = 1;
	var CLIENT_MODE_MOBILE = "Mobile";
	var CLIENT_STATE_OFFLINE = "Offline";	
	var parameters = {};
	var feedBack = [];
	var contact = {};
	var convertToComplaint = function ()
	{
	    try
	    {
	         parameters = {};
	         feedBack = [];
	         contact = {};	         
	         var id = Xrm.Page.data.entity.getId();
	        var properties = [
           "tc_question_name",
           "tc_question_response"].join();
	        var query = "?$filter=_tc_surveyfeedbackid_value eq " + formatEntityId(id) + "&$select=" + properties;
	        Tc.Crm.Scripts.Common.Get("tc_surveyresponsefeedbacks", query).then(function (request) {
	            var feedbackCollection = JSON.parse(request.response);
	            if (feedbackCollection && feedbackCollection.value) {
	                feedBack.push({ key: 'TC_RT_HolidayExpectation', value: '' });
	                feedBack.push({ key: 'TCIDS Time of day Contact', value: '' });
	                feedBack.push({ key: 'TCDIS Contact Method', value: '' });
	                feedBack.push({ key: 'TCDIS Best_Method_Contact_Other', value: '' });
	                feedBack.push({ key: 'TCDIS Booking_ref', value: '' });
	                feedBack.push({ key: 'TCDIS Hotel Name', value: '' });
	                feedBack.push({ key: 'TCDIS Dest_Airport', value: '' });
	                feedBack.push({ key: 'TC_IDS_DepartDate', value: '' });
	                feedBack.push({ key: 'TCDIS Return Date', value: '' });
	                feedBack.push({ key: 'TCDIS Flight Code', value: '' });
	                feedBack.push({ key: 'TC_TX_SourceMarketCode', value: '' });
	                feedBack.push({ key: 'TC_TX_Brand', value: '' });
	                feedBack.push({ key: 'TCDIS Language_code', value: '' });
	                feedBack.push({ key: 'TCDIS Customer Country', value: '' });
	                for (var i = 0; i < feedbackCollection.value.length; i++) {
	                    if (feedbackCollection.value[i].tc_question_name) {
	                        var index = feedBack.map(function (e) { return e.key; }).indexOf(feedbackCollection.value[i].tc_question_name);
	                        if (index >= 0)
	                            feedBack[index].value = feedbackCollection.value[i].tc_question_response;
	                    }
	                }
	                getSourceMarket(feedBack.filter(function (item) { return item.key === 'TC_TX_SourceMarketCode'; })[0].value);
	            }
	        }).catch(function (err) {
	            console.log("ERROR: " + err.message);
	        });
	    }
	    catch(e)
	    {
	        console.log("ERROR: " + e.message);
	    }
	}
    	    	
	var getSourceMarket = function (sourceMarketCode)
	{
	    var sourceMarketId = "";
	    if (sourceMarketCode)
	    {
	        var properties = [
            "tc_countryid",
            "tc_iso_code"].join();
	        var query = "?$filter=tc_iso2code eq '" + encodeURIComponent(sourceMarketCode) + "'&$select=" + properties;
	        return Tc.Crm.Scripts.Common.Get("tc_countries", query).then(function (request)
	        {
	            var sourceMarket = JSON.parse(request.response);
	            if (sourceMarket && sourceMarket.value.length > 0)
	            {
	                parameters["tc_sourcemarketid"] = sourceMarket.value[0].tc_countryid;
	                parameters["tc_sourcemarketidname"] = sourceMarket.value[0].tc_iso_code;
	                sourceMarketId = sourceMarket.value[0].tc_countryid;
	            }
	            getCustomer(sourceMarketId);

	        }).catch(function (err) {
	            console.log("ERROR: " + err.message);
	        });
	    }
	    else
	    {
	        getCustomer(sourceMarketId);
	    }
	}
	
	var getCustomer = function (sourceMarketId)
	{
	    var customer = getControlValue("customers");
	    if (customer && customer.length > 0)
	    {   
	        parameters["customerid"] = customer[0].id;
	        parameters["customeridname"] = customer[0].name;
	        parameters["customeridtype"] = "contact";
	        getBrand(feedBack.filter(function (item) { return item.key === 'TC_TX_Brand'; })[0].value);
	    }
	    else
	    {
	        var customerFirstName = getControlValue("tc_customerfirstname");
	        var customerLastName = getControlValue("tc_customerlastname");
	        var customerEmail = getControlValue("tc_customeremail");
	        var customerPhone = getControlValue("tc_customerphone");
	       
	        if ((!customerLastName || !customerFirstName) || (!customerEmail && !customerPhone))
	        {
	            return showCustomerInformationMissingMessage();
	        }
	        else
	        {
	            if (customerFirstName)
	                contact.firstname = customerFirstName;
	            if (customerLastName)
	                contact.lastname = customerLastName;
	            if (customerEmail)
	                contact.emailaddress1 = customerEmail;
	            if (customerPhone)
	                contact.telephone1 = customerPhone;
	            contact.tc_language = getLanguage(feedBack.filter(function (item) { return item.key === 'TCDIS Language_code'; })[0].value);
	            getCountry(feedBack.filter(function (item) { return item.key === 'TCDIS Customer Country'; })[0].value, sourceMarketId);
	        }
	    }
	}

	var getCountry = function (countryCode, sourceMarketId)
	{
	    if (countryCode && sourceMarketId)
	    {
	        var query = "?$filter=tc_iso2code eq '" + encodeURIComponent(countryCode) + "'&$select=tc_countryid";
	        Tc.Crm.Scripts.Common.Get("tc_countries", query).then(function (request)
	        {
	            var country = JSON.parse(request.response);
	            if (!country || country.value.length == 0)
	            {
	                return showCustomerInformationMissingMessage();
	            }
	            else
	            {
	                contact["tc_Address1_CountryId@odata.bind"] = "/tc_countries(" + country.value[0].tc_countryid + ")";
	                contact["tc_SourceMarketId@odata.bind"] = "/tc_countries(" + sourceMarketId + ")";
	                createContact();
	            }
	        }).catch(function (err) {
	            console.log("ERROR: " + err.message);
	        });
	    }
	    else
	    {
	        showCustomerInformationMissingMessage();
	    }
	}

	var createContact = function () {
	    Tc.Crm.Scripts.Common.Create("contacts", contact).then(function (request) {
	        var customerUri = request.getResponseHeader("OData-EntityId");
	        var customerId = customerUri.split(/[()]/);
	        customerId = customerId[1];
	        parameters["customerid"] = customerId;
	        parameters["customeridname"] = contact.firstname + " " + contact.lastname;
	        parameters["customeridtype"] = "contact";
	        setLookUp("customers", "contact", customerId, contact.firstname + " " + contact.lastname);
	        var bookingId = getControlValue("tc_bookingid");
	        if (bookingId && bookingId.length > 0)
	        {
	            createCustomerBookingRole(customerId, formatEntityId(bookingId[0].id));
	        }
	        else {
	            getBrand(feedBack.filter(function (item) { return item.key === 'TC_TX_Brand'; })[0].value);
	        }

	    }).catch(function (err) {
	        console.log("ERROR: " + err.message);
	    });
	}

	var createCustomerBookingRole = function (customerId, bookingId) {
	    var customerBookingRole = {};
	    customerBookingRole["tc_name"] = "Created through Survey Response";
	    customerBookingRole["tc_Customer_contact@odata.bind"] = "/contacts(" + customerId + ")";
	    customerBookingRole["tc_BookingId@odata.bind"] = "/tc_bookings(" + bookingId + ")";
	    customerBookingRole["tc_bookingrole"] = 950000002;
	    Tc.Crm.Scripts.Common.Create("tc_customerbookingroles", customerBookingRole).then(function (request) {
	        getBrand(feedBack.filter(function (item) { return item.key === 'TC_TX_Brand'; })[0].value);

	    }).catch(function (err) {
	        console.log("ERROR: " + err.message);
	    });
	}

	var getBrand = function (brandCode) {
	    if (brandCode) {
	        var properties = [
            "tc_brandid",
            "tc_name"].join();
	        var query = "?$filter=tc_brandcode eq '" + encodeURIComponent(brandCode) + "'&$select=" + properties;
	        Tc.Crm.Scripts.Common.Get("tc_brands", query).then(function (request)
	        {
	            var brand = JSON.parse(request.response);
	            if (brand && brand.value.length > 0)
	            {
	                parameters["tc_brandid"] = brand.value[0].tc_brandid;
	                parameters["tc_brandidname"] = brand.value[0].tc_name;
	            }
	            getHotel(feedBack.filter(function (item) { return item.key === 'TCDIS Hotel Name'; })[0].value);
	        }).catch(function (err) {
	            console.log("ERROR: " + err.message);
	        });
	    }
	    else {
	        getHotel(feedBack.filter(function (item) { return item.key === 'TCDIS Hotel Name'; })[0].value);
	    }
	}

	var getHotel = function (hotelCode) {
	    if (hotelCode) {
	        var properties = [
            "tc_hotelid",
            "tc_name"].join();
	        var query = "?$filter=tc_name eq '" + encodeURIComponent(hotelCode) + "'&$select=" + properties;
	        return Tc.Crm.Scripts.Common.Get("tc_hotels", query).then(function (request) {
	            var hotel = JSON.parse(request.response);
	            if (hotel && hotel.value.length > 0) {
	                parameters["tc_accommodation"] = hotel.value[0].tc_hotelid;
	                parameters["tc_accommodationname"] = hotel.value[0].tc_name;
	            }
	            getGateWay(feedBack.filter(function (item) { return item.key === 'TCDIS Dest_Airport'; })[0].value);
	        }).catch(function (err) {
	            console.log("ERROR: " + err.message);
	        });

	    }
	    else {
	        getGateWay(feedBack.filter(function (item) { return item.key === 'TCDIS Dest_Airport'; })[0].value);
	    }
	}

	var getGateWay = function (gateWayCode) {
	    if (gateWayCode) {
	        var properties = [
            "tc_gatewayid",
            "tc_iata"].join();
	        var query = "?$filter=tc_iata eq '" + encodeURIComponent(gateWayCode) + "'&$select=" + properties;
	        Tc.Crm.Scripts.Common.Get("tc_gatewaies", query).then(function (request) {
	            var gateway = JSON.parse(request.response);
	            if (gateway && gateway.value.length > 0) {
	                parameters["tc_gateway"] = gateway.value[0].tc_gatewayid;
	                parameters["tc_gatewayname"] = gateway.value[0].tc_iata;
	            }
	            openCase();
	        }).catch(function (err) {
	            console.log("ERROR: " + err.message);
	        });
	    }
	    else {
	        openCase();
	    }
	}
    	
	var openCase = function ()
	{
	    var booking = getControlValue("tc_bookingid");
	    var beginTime = getControlValue("tc_begintime");
	    var customerEmail = getControlValue("tc_customeremail");
	    var customerPhone = getControlValue("tc_customerphone");

	    if (beginTime)
	        parameters["tc_datereported"] = beginTime.getFullYear() + "/" + (beginTime.getMonth() + 1) + "/" + beginTime.getDate();	    
	    if (customerEmail)
	        parameters["tc_alternativeemail"] = customerEmail;
	    if (customerPhone)
	        parameters["tc_alternativephone"] = customerPhone;
	    var expectedResolution = feedBack.filter(function (item) { return item.key === 'TC_RT_HolidayExpectation'; })[0].value;
	    if (expectedResolution)
	        parameters["tc_expectedresolution"] = expectedResolution;
	    var arrivalDate = feedBack.filter(function (item) { return item.key === 'TC_IDS_DepartDate'; })[0].value;
	    if (arrivalDate && isValidDate(arrivalDate))
	        parameters["tc_arrivaldate"] = arrivalDate;
	    var returnDate = feedBack.filter(function (item) { return item.key === 'TCDIS Return Date'; })[0].value;
	    if (returnDate && isValidDate(returnDate))
	        parameters["tc_departuredate"] = returnDate;
	    if (arrivalDate && returnDate)
	    {
	        var days = getDifferenceInDays(arrivalDate, returnDate);
	        if (days)
	            parameters["tc_durationofstay"] = days;
	    }
	    var flightNumber = feedBack.filter(function (item) { return item.key === 'TCDIS Flight Code'; })[0].value;
	    if (flightNumber)
	        parameters["tc_flightnumber"] = flightNumber;
	    parameters["caseorigincode"] = 100000006;
	    parameters["tc_casetypeid"] = '478C99E9-93E4-E611-8109-1458D041F8E8';
	    parameters["tc_casetypeidname"] = 'Complaint';
	    parameters["tc_originatingbusinessarea"] = 950000001;
	    var preferredCommunication = getPreferredMethodOfCommunication(feedBack.filter(function (item) { return item.key === 'TCDIS Contact Method'; })[0].value);
	    if (preferredCommunication > 0)
	        parameters["tc_preferredmethodofcommunication"] = preferredCommunication;
	    if (booking && booking.length > 0)
	    {
	        parameters["tc_bookingreference"] = 1;
	        parameters["tc_bookingid"] = booking[0].id;
	        parameters["tc_bookingidname"] = booking[0].name;
	    }
	    else
	    {
	        parameters["tc_bookingreference"] = 0;
	        var bookingNumber = feedBack.filter(function (item) { return item.key === 'TCDIS Booking_ref'; })[0].value;
	        if (bookingNumber)
	            parameters["tc_bookingreferencefreetext"] = bookingNumber;
	    }
	    parameters["tc_surveyid"] = Xrm.Page.data.entity.getId();
	    Xrm.Utility.openEntityForm("incident", null, parameters);
	}

	var getLanguage = function (languageCode) {
	    var value;
	    switch (languageCode) {
	        case "EN":
	            value = 950000000;
	            break;
	        case "DE":
	            value = 950000001;
	            break;
	        case "NL":
	            value = 950000002;
	            break;
	        case "FR":
	            value = 950000003;
	            break;
	        case "ES":
	            value = 950000004;
	            break;
	        case "DA":
	            value = 950000005;
	            break;
	        case "":
	        case null:
	            value = 950000006;
	            break;
	        default:
	            value = 950000006;
	            break;
	    }
	    return value;
	}

	var getPreferredMethodOfCommunication = function (preferredCommunication) {
	    var value;
	    switch (preferredCommunication) {
	        case "SMS":
	            value = 950000000;
	            break;
	        case "Whitemail":
	            value = 950000002;
	            break;
	        case "Email":
	            value = 950000001;
	            break;
	        default:
	            value = -1;
	            break;
	    }
	    return value;
	}

	var setLookUp = function (fieldName, fieldType, fieldId, value) {
	    var object = new Array();
	    object[0] = new Object();
	    object[0].id = fieldId;
	    object[0].name = value;
	    object[0].entityType = fieldType;
	    Xrm.Page.getAttribute(fieldName).setValue(object);
	}

	var getControlValue = function (controlName) {
	    if (Xrm.Page.getAttribute(controlName))
	        return Xrm.Page.getAttribute(controlName).getValue();
	    else
	        return null;
	}

	var showCustomerInformationMissingMessage = function ()
	{   
	    Xrm.Utility.alertDialog('We do not have enough Customer data to create the Case, please create the Customer manually before proceeding');
	    return false;
	}

	var formatEntityId = function (id) {
	    return id !== null ? id.replace("{", "").replace("}", "") : null;
	}

	var getDifferenceInDays = function (arrivalDate, departureDate)
	{
	    //Get 1 day in milliseconds
	    var one_day = 1000 * 60 * 60 * 24;	   
	    if (!isValidDate(arrivalDate) || !isValidDate(departureDate)) return "";
	    arrivalDate = new Date(arrivalDate);
	    departureDate = new Date(departureDate);
	    // Convert both dates to milliseconds
	    var arrivalDate_ms = arrivalDate.getTime();
	    var departureDate_ms = departureDate.getTime();
	    // Calculate the difference in milliseconds
	    var difference_ms = departureDate_ms - arrivalDate_ms;
	    // Convert back to days and return
	    return Math.round(difference_ms / one_day);
	}

	var isValidDate = function(date)
	{
	    var dateStamp = Date.parse(date);
	    if (isNaN(dateStamp) == false)
	        return true;
	    else
	        return false;
	}


	// public
	return {
		OnConvertToComplaintClick: function () {
			convertToComplaint();
		}
	};
})();


