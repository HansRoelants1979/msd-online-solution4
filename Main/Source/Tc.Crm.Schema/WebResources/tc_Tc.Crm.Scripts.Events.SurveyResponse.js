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

	var ConvertToComplaint = function ()
	{
	    debugger;
		var customer = Xrm.Page.getAttribute("customers");
		var booking = Xrm.Page.getAttribute("tc_bookingid");
		var beginTime = Xrm.Page.getAttribute("tc_begintime");
		var customerEmail = Xrm.Page.getAttribute("tc_customeremail"); 
		var customerPhone = Xrm.Page.getAttribute("tc_customerphone");

		var id = Xrm.Page.data.entity.getId();
		var properties = [
       "tc_question_name",
       "tc_question_response"].join();
		var query = "?$filter=_tc_surveyfeedbackid_value eq " + formatEntityId(id) + "&$select=" + properties;
	    Tc.Crm.Scripts.Common.Get("tc_surveyresponsefeedbacks", query).then(function (request) {	        
		    var feedbackCollection = JSON.parse(request.response);
		    if (feedbackCollection && feedbackCollection.value)
		    {
		        var expectedResolution, departureDate, returnDate, flightNumber;
		        for (var i = 0; i < feedbackCollection.value.length; i++)
		        {
		            if (feedbackCollection.value[i].tc_question_name)
		            {
		                switch (feedbackCollection.value[i].tc_question_name)
		                {
		                    case "TC_RT_HolidayExpectation":
		                        if (feedbackCollection.value[i].tc_question_response)
		                            expectedResolution = feedbackCollection.value[i].tc_question_response;
		                        break;
		                    case "TCIDS Time of day Contact":
		                        break;
		                    case "TCDIS Contact Method":
		                        break;
		                    case "TCDIS Best_Method_Contact_Other":
		                        break;
		                    case "TCDIS Booking_ref":
		                        break;
		                    case "TCDIS Hotel Name":
		                        break;
		                    case "TCDIS Dest_Airport":
		                        break;
		                    case "TC_IDS_DepartDate":
		                        if (feedbackCollection.value[i].tc_question_response)
		                            departureDate = feedbackCollection.value[i].tc_question_response;
		                        break;
		                    case "TCDIS Return Date":
		                        if (feedbackCollection.value[i].tc_question_response)
		                            returnDate = feedbackCollection.value[i].tc_question_response;
		                        break;
		                    case "TCDIS Flight Code":
		                        if (feedbackCollection.value[i].tc_question_response)
		                            flightNumber = feedbackCollection.value[i].tc_question_response;
		                        break;
		                    case "TC_TX_SourceMarketCode":
		                        break;
		                    case "TC_TX_Brand":
		                        break;
		                    case "TCDIS Language_code":
		                        break;
		                    case "TCDIS Customer Country":
		                        break;
		                    default:
		                        break;
		                }
		            }
		        }

		        var parameters = {};
		        if (beginTime && beginTime.getValue())
		        {
		            var dateReported = beginTime.getValue();
		            parameters["tc_datereported"] = dateReported.getFullYear() + "/" + (dateReported.getMonth() + 1) + "/" + dateReported.getDate();
		        }
		        if(customerEmail && customerEmail.getValue())
		            parameters["tc_alternativeemail"] = customerEmail.getValue();
		        if(customerPhone && customerPhone.getValue())
		            parameters["tc_alternativephone"] = customerPhone.getValue();
		        if(expectedResolution)
		            parameters["tc_expectedresolution"] = expectedResolution;
		        if (departureDate)
		            parameters["tc_arrivaldate"] = departureDate;
		        if (returnDate)
		            parameters["tc_departuredate"] = returnDate;
		        if (flightNumber)
		            parameters["tc_flightnumber"] = flightNumber;
		        if (customer) {
		            customer = customer.getValue();
		            parameters["customerid"] = customer[0].id;
		            parameters["customeridname"] = customer[0].name;
		            parameters["customeridtype"] = "contact";
		        }
		        Xrm.Utility.openEntityForm("incident", null, parameters);

		    }
		}).catch(function (err) {		   
                console.log("ERROR: " + err.message);
            });
	}

	function formatEntityId(id) {
	    return id !== null ? id.replace("{", "").replace("}", "") : null;
	}


	// public
	return {
		ConvertToComplaint: function () {
			ConvertToComplaint();
		}
	};
})();


