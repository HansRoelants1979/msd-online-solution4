using System;

namespace Tc.Crm.Common.IntegrationLayer.Helper
{
	public static class EnumHelper
	{
		public static Enum MapOptionSet(string value, System.Type enumeration)
		{			
			if (enumeration == typeof(Model.Schema.Gender))
			{
				return GetGender(value);
			}

			if (enumeration == typeof(Model.Schema.CustomerStatus))
			{
				return GetCustomerStatus(value);
			}

			if (enumeration == typeof(Model.Schema.PhoneType))
			{
				return GetPhoneType(value);
			}

			if (enumeration == typeof(Model.Schema.EmailType))
			{
				return GetEmailType(value);
			}

			return null;
		}


		private static Model.Schema.Gender GetGender(string value)
		{
			var crmGender = (Common.Gender)Enum.Parse(typeof(Common.Gender), value);
			switch (crmGender)
			{
				case Common.Gender.Male:
					return Model.Schema.Gender.Male;
				case Common.Gender.Female:
					return Model.Schema.Gender.Female;
				default:
					return Model.Schema.Gender.NotSpecified;
			}
		}

		private static Model.Schema.CustomerStatus GetCustomerStatus(string value)
		{
			var crmCustomerStatus = (Common.CustomerStatusCode)Enum.Parse(typeof(Common.CustomerStatusCode), value);
			switch (crmCustomerStatus)
			{
				case CustomerStatusCode.Active:
					return Model.Schema.CustomerStatus.Active;
				case CustomerStatusCode.Blacklisted:
					return Model.Schema.CustomerStatus.Blacklisted;
				case CustomerStatusCode.Deceased:
					return Model.Schema.CustomerStatus.Deceased;
				case CustomerStatusCode.Inactive:
					return Model.Schema.CustomerStatus.Inactive;
				default:
					return Model.Schema.CustomerStatus.NotSpecified;
			}
		}

		private static Model.Schema.PhoneType GetPhoneType(string value)
		{
			var crmPhoneType = (Common.PhoneType)Enum.Parse(typeof(Common.PhoneType), value);
			switch (crmPhoneType)
			{
				case Common.PhoneType.Home:
					return Model.Schema.PhoneType.Home;
				case Common.PhoneType.Business:
					return Model.Schema.PhoneType.Business;
				case Common.PhoneType.Mobile:
					return Model.Schema.PhoneType.Mobile;
				default:
					return Model.Schema.PhoneType.NotSpecified;
			}
		}

		private static Model.Schema.EmailType GetEmailType(string value)
		{
			var crmEmailType = (Common.EmailType)Enum.Parse(typeof(Common.EmailType), value);
			switch (crmEmailType)
			{
				case Common.EmailType.Primary:
					return Model.Schema.EmailType.Primary;
				case Common.EmailType.Promotion:
					return Model.Schema.EmailType.Promo;
				default:
					return Model.Schema.EmailType.NotSpecified;
			}
		}
	}
}
