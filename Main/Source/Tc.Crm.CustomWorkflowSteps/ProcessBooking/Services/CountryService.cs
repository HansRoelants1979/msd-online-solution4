﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services
{
    public class CountryService
    {
        public static Dictionary<string, Guid> CountryList;
        public static void Init()
        {
            if (CountryList != null && CountryList.Count == 167) return;
            CountryList = new Dictionary<string, Guid>();
            CountryList.Add("AL", new Guid("5a766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("DZ", new Guid("94766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("AS", new Guid("91776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("AD", new Guid("54766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("AO", new Guid("61776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("AG", new Guid("58766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("AR", new Guid("5e766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("AW", new Guid("64766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("AU", new Guid("62766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("AT", new Guid("60766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("BS", new Guid("78766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("BH", new Guid("6e766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("BD", new Guid("8f776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("BB", new Guid("68766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("BY", new Guid("77776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("BE", new Guid("6a766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("BZ", new Guid("5b776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("BM", new Guid("72766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("BO", new Guid("74766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("BA", new Guid("66766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("BW", new Guid("7a766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("BR", new Guid("76766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("BN", new Guid("75776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("BG", new Guid("6c766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("BF", new Guid("81776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("BI", new Guid("79776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("KH", new Guid("cb766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("CA", new Guid("7c766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("CV", new Guid("8a766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("KY", new Guid("d1766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("CL", new Guid("80766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("CN", new Guid("82766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("CO", new Guid("84766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("KM", new Guid("7b776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("CG", new Guid("57776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("CK", new Guid("55776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("CR", new Guid("86766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("HR", new Guid("b3766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("CU", new Guid("88766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("CY", new Guid("8c766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("CZ", new Guid("8e766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("DK", new Guid("92766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("DO", new Guid("50766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("EC", new Guid("96766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("EG", new Guid("9a766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("SV", new Guid("8b776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("EE", new Guid("98766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("ET", new Guid("73776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("FJ", new Guid("a1766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("FI", new Guid("9f766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("FR", new Guid("a3766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("GF", new Guid("65776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("PF", new Guid("0f776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("GM", new Guid("ad766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("GE", new Guid("a9766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("DE", new Guid("90766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("GH", new Guid("6b776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("GI", new Guid("ab766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("GR", new Guid("b1766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("GD", new Guid("a7766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("GP", new Guid("af766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("GT", new Guid("53776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("GY", new Guid("93776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("HT", new Guid("6d776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("HN", new Guid("89776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("HU", new Guid("b5766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("IS", new Guid("bf766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("IN", new Guid("bd766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("ID", new Guid("b7766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("IE", new Guid("b9766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("IL", new Guid("bb766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("IT", new Guid("c1766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("JM", new Guid("c3766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("JP", new Guid("c7766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("JO", new Guid("c5766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("KZ", new Guid("71776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("KE", new Guid("c9766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("KW", new Guid("cf766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("LA", new Guid("d3766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("LV", new Guid("e3766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("LB", new Guid("d5766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("LS", new Guid("dd766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("LI", new Guid("d9766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("LT", new Guid("df766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("LU", new Guid("e1766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("MK", new Guid("ed766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("MG", new Guid("83776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("MW", new Guid("99776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("MY", new Guid("fb766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("MV", new Guid("f7766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("MT", new Guid("f3766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("MQ", new Guid("f1766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("MU", new Guid("f5766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("MX", new Guid("f9766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("MD", new Guid("e7766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("MC", new Guid("e5766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("ME", new Guid("e9766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("MA", new Guid("4e766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("MZ", new Guid("fd766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("MM", new Guid("ef766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("NA", new Guid("ff766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("NP", new Guid("87776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("NL", new Guid("03776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("AN", new Guid("5c766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("NZ", new Guid("07776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("NI", new Guid("01776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("NO", new Guid("05776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("OM", new Guid("09776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("PA", new Guid("0b776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("PY", new Guid("69776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("PE", new Guid("0d776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("PH", new Guid("11776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("PL", new Guid("13776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("PT", new Guid("17776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("PR", new Guid("15776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("QA", new Guid("19776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("RE", new Guid("1b776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("RO", new Guid("1d776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("RU", new Guid("21776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("BL", new Guid("70766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("SH", new Guid("5f776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("KN", new Guid("6f776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("LC", new Guid("d7766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("MF", new Guid("eb766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("VC", new Guid("5d776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("WS", new Guid("9b776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("ST", new Guid("4f776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("SA", new Guid("23776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("SN", new Guid("2f776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("RS", new Guid("1f776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("SC", new Guid("25776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("SG", new Guid("29776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("SK", new Guid("2d776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("SI", new Guid("2b776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("ZA", new Guid("52766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("KR", new Guid("cd766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("ES", new Guid("9d766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("LK", new Guid("db766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("SR", new Guid("7d776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("SZ", new Guid("31776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("SE", new Guid("27776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("CH", new Guid("7e766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("SY", new Guid("59776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("TW", new Guid("3d776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("TZ", new Guid("3f776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("TH", new Guid("35776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("TG", new Guid("51776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("TO", new Guid("85776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("TT", new Guid("3b776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("TN", new Guid("37776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("TR", new Guid("39776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("TM", new Guid("95776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("TC", new Guid("33776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("UG", new Guid("63776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("UA", new Guid("41776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("AE", new Guid("56766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("GB", new Guid("a5766a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("US", new Guid("43776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("UY", new Guid("45776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("UZ", new Guid("67776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("VE", new Guid("47776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("VN", new Guid("49776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("VG", new Guid("97776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("VI", new Guid("8d776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("YE", new Guid("7f776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("ZM", new Guid("4b776a7d-410a-e711-8108-3863bb34da28"));
            CountryList.Add("ZW", new Guid("4d776a7d-410a-e711-8108-3863bb34da28"));


        }

        public static Guid GetBy(string isoCode)
        {
            var id = Guid.Empty;
            if (CountryList.TryGetValue(isoCode, out id))
                return id;
            return Guid.Empty;
        }
    }
}