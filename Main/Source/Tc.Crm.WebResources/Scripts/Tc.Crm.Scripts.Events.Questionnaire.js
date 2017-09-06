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

if (typeof (Tc.Crm.Scripts.Events.Questionnaire) === "undefined") {
    Tc.Crm.Scripts.Events.Questionnaire = {
        __namespace: true
    };
}

Tc.Crm.Scripts.Events.Questionnaire = (function () {
    var questionnaireArray = [];
    var showQuestionnaire = function ()
    {
        var properties = [
         "tc_question",
         "tc_questionnaireid",
         "tc_name"].join();
        var query = "?$filter=tc_name ne null&$select=" + properties;
        Tc.Crm.Scripts.Common.Get("tc_questionnaires", query).then(function (request) {
            var questionnare = JSON.parse(request.response);
            if (questionnare && questionnare.value)
            {
                var table = [];
                var questionNo = 0;
                table.push("<table>");                                   
                for (var i = 0; i < questionnare.value.length; i++)
                {
                    questionnaireArray.push(questionnare.value[i].tc_questionnaireid);
                    if (questionnare.value[i].tc_question)
                    {
                        questionNo += 1;
                        table.push("<tr><td>" + questionNo + ") " + questionnare.value[i].tc_question + "</td></tr>");
                        table.push("<tr><td><textarea rows='2' cols='100'  id='text" + i + "' /></td></tr>");
                        table.push("<tr><td>&nbsp;</td></tr>");
                    }
                }
                table.push("</table>");
                $("#divQuestionnare").html(table.join(''));
            }
        }).catch(function (err) {
            console.log("ERROR: " + err.message);
        });
       
    };

    var createQuestionnaireResponse = function (name, answer, questionId)
    {
        var response = {};
        response.tc_name = name;
        response.tc_response = answer;
        response["tc_questionnaireid@odata.bind"] = "/tc_questionnaires(" + questionId + ")";
        response["tc_caselineid@odata.bind"] = "/tc_caselines(" + formatEntityId(window.parent.Xrm.Page.data.entity.getId()) + ")";
        Tc.Crm.Scripts.Common.Create("tc_questionnaireresponses", response).then(function (request)
        {
            console.log('Questionnaire response was created successfully.');
        }).catch(function (err) {
            console.log("ERROR: " + err.message);
        });
    };

    var showQuestionnaireTemp = function () {

        var table = [];
        table.push("<table>");
        for (var i = 1; i < 10; i++)
        {
            table.push("<tr><td> " + i + ") If the child was not supervised, where were the parents/guardians at the time of the incident " + i + "</td></tr>");
            table.push("<tr><td><textarea rows='4' cols='100'  id='text" + i + "' /></td></tr>");
            table.push("<tr><td>&nbsp;</td></tr>");
        }
        table.push("</table>");
        $("#divQuestionnare").html(table.join(''));
    };
       

    var formatEntityId = function (id) {
        return id !== null ? id.replace("{", "").replace("}", "") : null;
    }

    var submitQuestionnaire = function ()
    {
        for (var i = 0; i < questionnaireArray.length; i++)
        {          
            createQuestionnaireResponse("Response " + i.toString(), $("#text" + i).val(), formatEntityId(questionnaireArray[i]));
        }
    };
    // public
    return {
        ShowQuestionnaire: showQuestionnaire,
        SubmitQuestionnaire: submitQuestionnaire
    };
})();