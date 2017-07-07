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
if (typeof (Tc.Crm.Scripts.Html) === "undefined") {
    Tc.Crm.Scripts.Html = {
        __namespace: true
    };
    Tc.Crm.Scripts.Html.TravellerPlanner = (function () {
        "use strict";
        var getKillingQuestionsRecords = function ()
        {
                var records = getQuestionsRecords().then(function (questionsResponse) {
                var questions = JSON.parse(questionsResponse.response);
                if (questions == null || questions == "" || questions == "undefined") return;
                   loadKillerQuestionsHtml(questions);
            }).catch(function (err) {
                throw new Error("Problem in retrieving the Killing question records ");
            });
        }
        var getQuestionsRecords = function () {      
                var query = "?$select=tc_name&$filter=statuscode eq 1";
                var entityName = "tc_killerquestionses";                
                return Tc.Crm.Scripts.Common.Get(entityName, query);             
         
        }
        var loadKillerQuestionsHtml = function (results)
        {
            var $table = $("<table></table>");

            for (var i = 0; i < results.value.length; i++) {

                var question = results.value[i]["tc_name"];      
                
                var $line = $("<tr></tr>");
                $line.append($("<td></td>").html(question));
                $table.append($line);
            }

            $table.appendTo(document.body);           
            
        }

        return {
            OnHTMLWebresourceOnLoad: getKillingQuestionsRecords
            
        };
    })();
}