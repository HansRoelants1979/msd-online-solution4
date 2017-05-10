var id = "";
function Sample()
{  
    var contact = {};
    contact.firstname = "Test";
    contact.lastname = "Promiseq";

    var entityName = "contacts";
    var guid = 'C337C189-34D9-E611-8229-C4446BDC3CC1';
    
    var deleteData = document.getElementsByName("removesampledata")[0].checked;
    
    
                                
    Tc.Crm.Scripts.Common.Upsert(entityName, guid, contact)
    .then(function (request) {
                                   
        entityUri = request.getResponseHeader("OData-EntityId");
        var properties = [
        "fullname",
        "annualincome",
        "jobtitle",
        "description",
        "contactid"].join();
        var query = "?$select=" + properties;
        return Tc.Crm.Scripts.Common.GetByUri(entityUri, query);
       
                                    
    })
     .then(function (request) {
        var contact1 = JSON.parse(request.response);
        var successMsg = "Contact '%s' retrieved:\n"
            + "\tAnnual income: %s \n"
            + "\tJob title: %s \n"
            + "\tDescription: %s"
            + "\contactid: %s";
        console.log(successMsg,
            contact1.fullname,
            contact1.annualincome,
            contact1.jobtitle,
            contact1.description,
            contact1.contactid);
        var contact = {};
        contact.firstname = "Test";
        contact.lastname = "Promiseq Success";
        contact.jobtitle = "Developer";
        contact.annualincome = 5000.00;
        var entityName = "contact";
        id = contact1.contactid;
        return Tc.Crm.Scripts.Common.Update(entityName, id, contact);
     }).then(function () {
         var entityName = "contact";
         var properties = [
        "fullname",
        "annualincome",
        "jobtitle",
        "description",
        "contactid"].join();
         var query = "?$select=" + properties;
         return Tc.Crm.Scripts.Common.GetById(entityName, id, query);
     }).then(function (request) {
         var contact1 = JSON.parse(request.response);
         var successMsg = "Contact '%s' retrieved with updated values:\n"
             + "\tAnnual income: %s \n"
             + "\tJob title: %s \n"
             + "\tDescription: %s"
             + "\contactid: %s";
         console.log(successMsg,
             contact1.fullname,
             contact1.annualincome,
             contact1.jobtitle,
             contact1.description,
             contact1.contactid);
         var entityName = "contact";
         
         if (deleteData)
         return Tc.Crm.Scripts.Common.Delete(entityName, id);
     }).then(function () {
         if (deleteData) {
             var successMsg = "Contact '%s' is Deleted Successfully";
             console.log(successMsg, id);
         }
     })
        .catch(function (err) {
        console.log("ERROR: " + err.message);
    });
                               
}

function CreateGuid() {  
   function _p8(s) {  
      var p = (Math.random().toString(16)+"000000000").substr(2,8);  
      return s ? "-" + p.substr(0,4) + "-" + p.substr(4,4) : p ;  
   }  
   var guid = _p8() + _p8(true) + _p8(true) + _p8();
   guid = guid.toUpperCase();
   guid = guid.replace("{", "");
   guid = guid.replace("}", "");
   return guid;
}  



var contact = {};
contact.firstname = "Test";
contact.lastname = "Promiseq";

var entityName = "contact";

var id = 'C337C189-34D9-E611-8229-C4446BDC3CC1';
var attribute = 'telephone1';
var value = { value: "555-0105" };

Tc.Crm.Scripts.Common.UpdateSingleAttribute(entityName, id, attribute, value)
 .then(function () {
     //We can do our logic here
 }).catch(function (err) {
     //Exception Handling functionality, we can get exception message by using [err.message]
 });
