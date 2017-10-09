using System;
using System.Runtime.Serialization;

namespace Tc.Usd.HostedControls.Models
{
    [DataContract(Name = "owrRequest")]
    public class OwrSearch
    {
        [DataMember(Name = "requestId")]
        public Guid RequestId { get; set; }

        [DataMember(Name = "travelPlanner")]
        public TravelPlanner TravelPlanner { get; set; }
    }

    [DataContract(Name = "travelPlanner")]
    public class TravelPlanner
    {
        [DataMember(Name = "travelPlannerId")]
        public Guid TravelPlannerId { get; set; }

        [DataMember(Name = "consultationReference")]
        public string ConsultationReference { get; set; }

        [DataMember(Name = "departureDateFrom")]
        public string DepartureDateFrom { get; set; }

        [DataMember(Name = "departureDateTo")]
        public string DepartureDateTo { get; set; }

        [DataMember(Name = "numberOfNights")]
        public NumberOfNights NumberOfNights { get; set; }

        [DataMember(Name = "includedDestinations")]
        public string[] IncludedDestinations { get; set; }

        [DataMember(Name = "excludedDestinations")]
        public string[] ExcludedDestinations { get; set; }

        [DataMember(Name = "departurePoints")]
        public string[] DeparturePoints { get; set; }

        [DataMember(Name = "rooms")]
        public RoomOwr[] Rooms { get; set; }

        [DataMember(Name = "customer")]
        public Tc.Usd.HostedControls.Models.CustomerOwr Customer { get; set; }
       
    }

    [DataContract(Name = "customer")]
    public class CustomerOwr
    {
        [DataMember(Name = "customerIdentifier")]
        public CustomerIdentifierOwr CustomerIdentifier { get; set; }
        [DataMember(Name = "customerIdentity")]
        public CustomerIdentityOwr CustomerIdentity { get; set; }

        [DataMember(Name= "address")]
        public AddressOwr[] Address { get; set; }

        [DataMember(Name = "phone")]
        public PhoneOwr[] Phone { get; set; }

        [DataMember(Name = "email")]
        public EmailOwr[] Email { get; set; }
    }

    [DataContract(Name = "customerIdentifier")]
    public class CustomerIdentifierOwr
    {
        [DataMember(Name = "customerID")]
        public string CustomerId { get; set; }
    }

    [DataContract(Name = "customerIdentity")]
    public class CustomerIdentityOwr
    {
        [DataMember(Name = "salutation")]
        public string Salutation { get; set; }

        [DataMember(Name = "firstName")]
        public string FirstName { get; set; }

        [DataMember(Name = "middleName")]
        public string MiddleName { get; set; }

        [DataMember(Name = "lastName")]
        public string LastName { get; set; }

        [DataMember(Name = "birthDate")]
        public string BirthDate { get; set; }
        
    }

    [DataContract(Name = "rooms")]
    public class RoomOwr
    {
        [DataMember(Name = "numberOfAdults")]
        public int NumberOfAdults { get; set; }

        [DataMember(Name = "numberOfChildren")]
        public int NumberOfChildren { get; set; }

        [DataMember(Name = "childrensAges")]
        public ChildrenAges[] ChildrensAges { get; set; }
    }

    [DataContract(Name = "address")]
    public class AddressOwr
    {
        [DataMember(Name = "flatNumberUnit")]
        public string FlatNumberUnit { get; set; }

        [DataMember(Name = "houseNumberBuilding")]
        public string HouseNumberBuilding { get; set; }

        [DataMember(Name = "street")]
        public string Street { get; set; }

        [DataMember(Name = "town")]
        public string Town { get; set; }

        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "county")]
        public string County { get; set; }

        [DataMember(Name = "postalCode")]
        public string PostalCode { get; set; }


    }

    [DataContract(Name = "phone")]
    public class PhoneOwr
    {

        [DataMember(Name = "type")]
        public PhoneType PhoneType { get; set; }

        [DataMember(Name = "number")]
        public string Number { get; set; }
    }

    [DataContract(Name = "email")]
    public class EmailOwr
    {

        [DataMember(Name = "type")]
        public EmailType EmailType { get; set; }

        [DataMember(Name = "address")]
        public string Address { get; set; }
    }

    [DataContract(Name= "numberOfNights")]
    public enum NumberOfNights
    {
        [EnumMember(Value = "2-3 nights")]
        TwoThreeNights = 950000000,
        [EnumMember(Value= "4-6 nights")]
        FourSixNights = 950000021,
        [EnumMember(Value = "8-13 nights")]
        EightThirteenNights = 950000020,
        [EnumMember (Value = "29-35 nights")]
        TwentyNineThirtyFiveNights = 950000001,
        [EnumMember(Value= "36-70 nights")]
        ThirtySixSeventyNights = 950000012,
        [EnumMember(Value = "1 night")]
        OneNight = 950000005,
        [EnumMember(Value = "2 nights")]
        TwoNights = 950000006,
        [EnumMember(Value= "3 nights")]
        ThreeNights = 950000007,
        [EnumMember(Value = "4 nights")]
        FourNights = 950000008,
        [EnumMember(Value= "5 nights")]
        FiveNights = 950000009,
        [EnumMember(Value= "6 nights")]
        SixNights = 950000010,
        [EnumMember(Value = "7 nights")]
        SevenNights = 950000002,
        [EnumMember(Value= "8 nights")]
        EightNights = 950000011,
        [EnumMember(Value = "9 nights")]
        NineNights = 950000013,
        [EnumMember(Value= "10 nights")]
        TenNights = 950000014,
        [EnumMember(Value = "11 nights")]
        ElevenNights = 950000015,
        [EnumMember(Value = "12 nights")]
        TwelveNights = 950000016,
        [EnumMember(Value= "13 nights")]
        ThirteenNights = 950000017,
        [EnumMember(Value= "14 nights")]
        FourteenNights = 950000003,
        [EnumMember(Value= "15 nights")]
        FifteenNights = 950000004,
        [EnumMember(Value= "16 nights")]
        SixteenNights = 950000022,
        [EnumMember(Value= "17 nights")]
        SeventeenNights = 950000023,
        [EnumMember(Value= "18 nights")]
        EighteenNights = 950000024,
        [EnumMember(Value = "19 nights")]
        NineteenNights = 950000025,
        [EnumMember(Value= "20 nights")]
        TwentyNights = 950000026,
        [EnumMember(Value= "21 nights")]
        TwentyOneNights = 950000018,
        [EnumMember(Value= "22 nights")]
        TwentyTwoNights = 950000027,
        [EnumMember(Value= "23 nights")]
        TwentyThreeNights = 950000028,
        [EnumMember(Value= "24 nights")]
        TwentyFourNights = 950000029,
        [EnumMember(Value= "25 nights")]
        TwentyFiveNights = 950000030,
        [EnumMember(Value= "26 nights")]
        TwentySixNights = 950000031,
        [EnumMember(Value= "27 nights")]
        TwentySevenNights = 950000032,
        [EnumMember(Value= "28 nights")]
        TwentyEightNights = 950000033,
        [EnumMember(Value= "29 nights")]
        TwentyNineNights = 950000034,
        [EnumMember(Value= "30 nights")]
        ThirtyNights = 950000035,
        [EnumMember(Value= "31 nights")]
        ThirtyOneNights = 950000036,
        [EnumMember(Value= "32 nights")]
        ThirtyTwoNights = 950000019,
        [EnumMember(Value= "33 nights")]
        ThirtyThreeNights = 950000037,
        [EnumMember(Value= "34 nights")]
        ThirtyFourNights = 950000038,
        [EnumMember(Value= "35 nights")]
        ThirtyFiveNights = 950000039,
        [EnumMember(Value= "36 nights")]
        ThirtySixNights = 950000040,
        [EnumMember(Value= "37 nights")]
        ThirtySevenNights = 950000041,
        [EnumMember(Value= "38 nights")]
        ThirtyEightNights = 950000042,
        [EnumMember(Value = "39 nights")]
        ThirtyNineNights = 950000043,
        [EnumMember(Value= "40 nights")]
        FourtyNights = 950000044,
        [EnumMember(Value= "41 nights")]
        FourtyOneNights = 950000045,
        [EnumMember(Value= "42 nights")]
        FourtyTwoNights = 950000046,
        [EnumMember(Value= "43 nights")]
        FourtyThreeNights = 950000047,
        [EnumMember(Value = "44 nights")]
        FourtyFourNights = 950000048,
        [EnumMember(Value = "45 nights")]
        FourtyFiveNights = 950000049
    }

    [DataContract(Name = "childrensAges")]
    public enum ChildrenAges
    {
        [EnumMember(Value= "Unknown")]
        Unknown = 950000000,
        [EnumMember(Value= "< 2 years old")]
        LessTwoYearsOld = 950000001,
        [EnumMember(Value = "2 year old")]
        TwoYearsOld = 950000002,
        [EnumMember(Value= "3 year old")]
        ThreeYearOld = 950000003,
        [EnumMember(Value= "4 year old")]
        FourYearOld = 950000004,
        [EnumMember(Value= "5 year old")]
        FiveYearOld = 950000005,
        [EnumMember(Value= "6 year old")]
        SixYearOld = 950000006,
        [EnumMember(Value= "7 year old")]
        SevenYearOld = 950000007,
        [EnumMember(Value= "8 year old")]
        EightYearOld = 950000008,
        [EnumMember(Value= "9 year old")]
        NineYearOld = 950000009,
        [EnumMember(Value = "10 year old")]
        TenYearOld = 950000010,
        [EnumMember(Value= "11 year old")]
        ElevenYearOld = 950000011,
        [EnumMember(Value= "12 year old")]
        TwelveYearOld = 950000012,
        [EnumMember(Value= "13 year old")]
        ThirteenYearOld = 950000013,
        [EnumMember(Value= "14 year old")]
        FourteenYearOld = 950000014,
        [EnumMember(Value = "15 year old")]
        FifteenYearOld = 950000015,
        [EnumMember(Value= "16 year old")]
        SixteenYearOld = 950000016,
        [EnumMember(Value= "17 year old")]
        SeventeenYearOld = 950000017,
        [EnumMember(Value= "18 year old")]
        EighteenYearOld = 950000018,
        [EnumMember(Value= "19 year old")]
        NineteenYearOld = 950000019
    }

    [DataContract(Name = "PhoneType")]
    public enum PhoneType
    {
        [EnumMember(Value = "Unknown")]
        Unknown = 950000002,
        [EnumMember(Value= "Mobile")]
        Mobile = 950000000,
        [EnumMember(Value = "Home")]
        Home = 950000001,
        [EnumMember(Value = "Business")]
        Business = 950000003
    }

    [DataContract(Name = "EmailType")]
    public enum EmailType
    {
        [EnumMember(Value = "Unknown")]
        Unknown = 950000002,
        [EnumMember(Value = "Primary")]
        Primary = 950000000,
        [EnumMember(Value= "Promotion")]
        Promotion = 950000001
    }

    public enum HowDoYouWantToSearch
    {
        [EnumMember(Value= "All")]
        All = 950000002,
        [EnumMember(Value= "By Country i.e. Spain")]
        ByCountry = 950000000,
        [EnumMember(Value= "By Region i.e. Andalusia")]
        ByRegion = 950000001,
        [EnumMember(Value="By Hotel Code / Name")]
        ByHotel = 950000003,
        [EnumMember(Value= "By Destination Airport")]
        ByDestinationAirport = 950000005
    }
}
