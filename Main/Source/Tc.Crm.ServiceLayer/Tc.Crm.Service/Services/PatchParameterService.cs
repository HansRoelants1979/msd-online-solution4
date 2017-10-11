using System.Collections.Generic;
using Tc.Crm.Service.Constants;

namespace Tc.Crm.Service.Services
{
    public class PatchParameterService: IPatchParameterService
    {
        public Dictionary<string,string> MapCustomer { get; set; }

        public Dictionary<string, string> MapAccount { get; set; }

        public PatchParameterService()
        {
            MapCustomer = MapCustomerFields();
            MapAccount = MapAccountFields(); 
        }

        private Dictionary<string, string> MapCustomerFields()
        {
            Dictionary<string, string>  Map = new Dictionary<string, string>();
            Map.Add(Path.Customeridentifier_Sourcemarket, Attributes.SourceMarketId);
            Map.Add(Path.Customergeneral_Customerstatus, Attributes.StatusCode);
            Map.Add(Path.Customeridentity_Salutation, Attributes.Salutation);
            Map.Add(Path.Customeridentity_Academictitle, Attributes.AcademicTitle);
            Map.Add(Path.Customeridentity_Firstname, Attributes.FirstName);
            Map.Add(Path.Customeridentity_Middlename, Attributes.MiddleName);
            Map.Add(Path.Customeridentity_Lastname, Attributes.LastName);
            Map.Add(Path.Customeridentity_Language, Attributes.Language);
            Map.Add(Path.Customeridentity_Gender, Attributes.Gender);
            Map.Add(Path.Customeridentity_Birthdate, Attributes.Birthdate);
            Map.Add(Path.Company_Companyname, Attributes.Name);
            Map.Add(Path.Additional_Segment, Attributes.Segment);
            Map.Add(Path.Additional_DateOfDeath, Attributes.DateOfDeath);
            Map.Add(Path.Address1_Additionaladdressinfo, Attributes.Address1AdditionalInformation);
            Map.Add(Path.Address1_Flatnumberunit, Attributes.Address1FlatOrUnitNumber);
            Map.Add(Path.Address1_Housenumberbuilding, Attributes.Address1HouseNumberOrBuilding);
            Map.Add(Path.Address1_Street, Attributes.Address1Street);
            Map.Add(Path.Address1_Town, Attributes.Address1Town);
            Map.Add(Path.Address1_Country, Attributes.Address1CountryId);
            Map.Add(Path.Address1_County, Attributes.Address1County);
            Map.Add(Path.Address1_Postalcode, Attributes.Address1PostalCode);
            Map.Add(Path.Address2_Additionaladdressinfo, Attributes.Address2AdditionalInformation);
            Map.Add(Path.Address2_Flatnumberunit, Attributes.Address2FlatOrUnitNumber);
            Map.Add(Path.Address2_Housenumberbuilding, Attributes.Address2HouseNumberOrBuilding);
            Map.Add(Path.Address2_Street, Attributes.Address2Street);
            Map.Add(Path.Address2_Town, Attributes.Address2Town);
            Map.Add(Path.Address2_Country, Attributes.Address2CountryId);
            Map.Add(Path.Address2_County, Attributes.Address2County);
            Map.Add(Path.Address2_Postalcode, Attributes.Address2PostalCode);
            Map.Add(Path.Phone1_Type, Attributes.Telephone1Type);
            Map.Add(Path.Phone1_Number, Attributes.Telephone1);
            Map.Add(Path.Phone2_Type, Attributes.Telephone2Type);
            Map.Add(Path.Phone2_Number, Attributes.Telephone2);
            Map.Add(Path.Phone3_Type, Attributes.Telephone3Type);
            Map.Add(Path.Phone3_Number, Attributes.Telephone3);
            Map.Add(Path.Email1_Type, Attributes.EmailAddress1Type);
            Map.Add(Path.Email1_Email, Attributes.EmailAddress1);
            Map.Add(Path.Email2_Type, Attributes.EmailAddress2Type);
            Map.Add(Path.Email2_Email, Attributes.EmailAddress2);
            Map.Add(Path.Email3_Type, Attributes.EmailAddress3Type);
            Map.Add(Path.Email3_Email, Attributes.EmailAddress3);
            Map.Add(Path.Permissions_Allowmarketing, Attributes.ThomasCookMarketingConsent);
            Map.Add(Path.Permissions_Donotallowemail, Attributes.SendMarketingByEmail);
            Map.Add(Path.Permissions_Donotallowmail, Attributes.SendMarketingByPost);
            Map.Add(Path.Permissions_Donotallowphonecalls, Attributes.MarketingByPhone);
            Map.Add(Path.Permissions_Donotallowsms, Attributes.SendMarketingBySms);
            return Map;
        }

        private Dictionary<string, string> MapAccountFields()
        {
            Dictionary<string, string> Map = new Dictionary<string, string>();
            Map.Add(Path.Customeridentifier_Sourcemarket, Attributes.SourceMarketId);
            Map.Add(Path.Customergeneral_Customerstatus, Attributes.StatusCode);
            Map.Add(Path.Customeridentity_Salutation, Attributes.Salutation);
            Map.Add(Path.Customeridentity_Academictitle, Attributes.AcademicTitle);
            Map.Add(Path.Customeridentity_Firstname, Attributes.FirstName);
            Map.Add(Path.Customeridentity_Middlename, Attributes.MiddleName);
            Map.Add(Path.Customeridentity_Lastname, Attributes.LastName);
            Map.Add(Path.Customeridentity_Language, Attributes.Language);
            Map.Add(Path.Customeridentity_Gender, Attributes.Gender);
            Map.Add(Path.Customeridentity_Birthdate, Attributes.Birthdate);
            Map.Add(Path.Company_Companyname, Attributes.Name);
            Map.Add(Path.Additional_Segment, Attributes.Segment);
            Map.Add(Path.Additional_DateOfDeath, Attributes.DateOfDeath);
            Map.Add(Path.Address1_Additionaladdressinfo, Attributes.Address1AdditionalInformation);
            Map.Add(Path.Address1_Flatnumberunit, Attributes.Address1FlatOrUnitNumber);
            Map.Add(Path.Address1_Housenumberbuilding, Attributes.Address1HouseNumberOrBuilding);
            Map.Add(Path.Address1_Street, Attributes.Address1Street);
            Map.Add(Path.Address1_Town, Attributes.Address1Town);
            Map.Add(Path.Address1_Country, Attributes.Address1CountryId);
            Map.Add(Path.Address1_County, Attributes.Address1County);
            Map.Add(Path.Address1_Postalcode, Attributes.Address1PostalCode);
            Map.Add(Path.Address2_Additionaladdressinfo, Attributes.Address2AdditionalInformation);
            Map.Add(Path.Address2_Flatnumberunit, Attributes.Address2FlatOrUnitNumber);
            Map.Add(Path.Address2_Housenumberbuilding, Attributes.Address2HouseNumberOrBuilding);
            Map.Add(Path.Address2_Street, Attributes.Address2Street);
            Map.Add(Path.Address2_Town, Attributes.Address2Town);
            Map.Add(Path.Address2_Country, Attributes.Address2CountryId);
            Map.Add(Path.Address2_County, Attributes.Address2County);
            Map.Add(Path.Address2_Postalcode, Attributes.Address2PostalCode);
            Map.Add(Path.Phone1_Type, Attributes.Telephone1Type_Account);
            Map.Add(Path.Phone1_Number, Attributes.Telephone1);
            Map.Add(Path.Phone2_Type, Attributes.Telephone2Type_Account);
            Map.Add(Path.Phone2_Number, Attributes.Telephone2);
            Map.Add(Path.Phone3_Type, Attributes.Telephone3Type_Account);
            Map.Add(Path.Phone3_Number, Attributes.Telephone3);
            Map.Add(Path.Email1_Type, Attributes.EmailAddress1Type_Account);
            Map.Add(Path.Email1_Email, Attributes.EmailAddress1);
            Map.Add(Path.Email2_Type, Attributes.EmailAddress2Type_Account);
            Map.Add(Path.Email2_Email, Attributes.EmailAddress2);
            Map.Add(Path.Email3_Type, Attributes.EmailAddress3Type_Account);
            Map.Add(Path.Email3_Email, Attributes.EmailAddress3);
            Map.Add(Path.Permissions_Allowmarketing, Attributes.ThomasCookMarketingConsent);
            Map.Add(Path.Permissions_Donotallowemail, Attributes.SendMarketingByEmail);
            Map.Add(Path.Permissions_Donotallowmail, Attributes.SendMarketingByPost);
            Map.Add(Path.Permissions_Donotallowphonecalls, Attributes.MarketingByPhone);
            Map.Add(Path.Permissions_Donotallowsms, Attributes.SendMarketingBySms);
            return Map;
        }
    }
}
 