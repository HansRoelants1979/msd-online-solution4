
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using Newtonsoft.Json;
using Tc.Crm.Service.Models;
using JWT;
using System.Security.Cryptography;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Tc.Crm.Service.Client.Console
{
    class Program
    {


       
        static string Url = string.Empty;
        static void Main(string[] args)
        {
            try
            {
                System.Console.WriteLine("Enter 1 to process Booking OR 2 to process survey");
               
                var option = System.Console.ReadLine();
                if (option == "1")
                {
                    ProcessBooking();
                }
                else if (option == "2")
                {
                    ProcessSurvey();
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Unhandled Exception:: Message: {0} ", ex.Message);
                System.Console.WriteLine("Unhandled Exception:: Stack Trace: {0} ", ex.StackTrace.ToString());
            }
            System.Console.ReadLine();
        }

        private static void ProcessSurvey()
        {
           
            while (true)
            {
                System.Console.WriteLine("Reading Survey json payload.");
                var data = File.ReadAllText("survey.json");
                var api = "api/survey/create";

                //create the token
                var token = CreateJWTToken();

                //Call
                HttpClient cons = new HttpClient();

                cons.BaseAddress = new Uri(GetUrl());

                cons.DefaultRequestHeaders.Accept.Clear();
                cons.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                cons.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Task<HttpResponseMessage> t = cons.PutAsync(api, new StringContent(data, Encoding.UTF8, "application/json"));

                var response = t.Result;

                Task<string> task = response.Content.ReadAsStringAsync();
                var content = task.Result;

                System.Console.WriteLine("Response Code: {0}", response.StatusCode.GetHashCode());
                if (response.StatusCode == HttpStatusCode.Created)
                    System.Console.WriteLine("Survey has been created", content);            
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                    System.Console.WriteLine("Bad Request.");
                else if (response.StatusCode == HttpStatusCode.InternalServerError)
                    System.Console.WriteLine("Internal Server Error.");
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                    System.Console.WriteLine("Forbidden.");

                if (string.IsNullOrWhiteSpace(content))
                    System.Console.WriteLine("No content.");
                else
                    System.Console.WriteLine("Content:{0}", content);

                System.Console.Write("Do one more test(y/n):");
                var ans = System.Console.ReadLine();
                if (ans == "n") break;

            }
        }

        private static void ProcessBooking()
        {
            #region main
            System.Console.WriteLine("Select one from the following:");
            System.Console.WriteLine("1. Generate sample json payload.");
            System.Console.WriteLine("2. Create booking(you should have json payload available).");
            var option = System.Console.ReadLine();
            if (option == "1")
            {
                Booking booking = new Booking
                {
                    #region Booking.bookingidentifier
                    BookingIdentifier = new BookingIdentifier
                    {
                        BookingNumber = "BKG00001",
                        BookingUpdateDateOnTour = "20170202",
                        BookingUpdateDateTourOperator = "20170202",
                        BookingVersionOnTour = "v1",
                        BookingVersionTourOperator = "v1",
                        SourceMarket = "AX",
                        SourceSystem = "OnTour"
                    },
                    #endregion
                    #region booking.customer
                    Customer = new Customer
                    {
                        #region booking.customer.additional
                        Additional = new Additional
                        {
                            DateOfDeath = "20170102",
                            Segment = "deceased"
                        },
                        #endregion booking.customer.additional
                        #region booking.customer.address
                        Address = new Address[] {
                        new Address {
                            AdditionalAddressInfo = "Near to Cricket stadium",
                            Box = "Hmm",
                            Country = "AX",
                            County = "Worcestorshire",
                            FlatNumberUnit = "402",
                            HouseNumberBuilding = "22",
                            Number = "22",
                            PostalCode = "WA1123",
                            Street = "Nathans",
                            Town = "UU Gully",
                            AddressType = AddressType.M
                        }

                    },
                        #endregion booking.customer.address
                        #region booking.customer.company
                        Company = new Company
                        {
                            CompanyName = "Innovate Me"
                        },
                        #endregion booking.customer.company
                        #region booking.customer.identifier
                        CustomerIdentifier = new CustomerIdentifier
                        {
                            BusinessArea = "Hotel",
                            CustomerId = "CNTBST0001",
                            SourceMarket = "AX",
                            SourceSystem = "OnTour",
                        },
                        #endregion booking.customer.identifier
                        #region booking.customer.email
                        Email = new Email[] {
                        new Email {
                            Address = "joe@innovateme.com",
                            EmailType = EmailType.Pri
                        }
                    },
                        #endregion booking.customer.email
                        #region booking.customer.general
                        CustomerGeneral = new CustomerGeneral
                        {
                            CustomerStatus = CustomerStatus.A,
                            CustomerType = CustomerType.B
                        },
                        #endregion booking.customer.general
                        #region booking.customer.identity
                        CustomerIdentity = new CustomerIdentity
                        {
                            AcademicTitle = "Mr",
                            Birthdate = "19821008",
                            FirstName = "Barney",
                            Gender = Gender.Male,
                            Language = "Bro-English",
                            LastName = "Stinston",
                            MiddleName = "bro",
                            Salutation = "Mr"
                        },
                        #endregion booking.customer.identity
                        #region booking.customer.phone
                        Phone = new Phone[] {
                        new Phone
                        {
                            Number = "32323232",
                            PhoneType = PhoneType.H
                        }
                    },
                        #endregion booking.customer.phone
                        #region booking.customer.social
                        Social = new Social[]
                        {
                        new Social
                        {
                            SocialType = "I am",
                            Value = "Social"
                        }
                        },

                        #endregion booking.customer.social
                    },
                    #endregion booking.customer
                    #region booking.identity
                    BookingIdentity = new BookingIdentity
                    {
                        #region booking.edentity.booker
                        Booker = new BookingBooker
                        {
                            #region booking.edentity.booker.address
                            Address = new Address
                            {
                                AdditionalAddressInfo = "ad",
                                Box = "2",
                                Country = "England",
                                County = "Hmm",
                                FlatNumberUnit = "2",
                                HouseNumberBuilding = "3",
                                Number = "1",
                                PostalCode = "wa001",
                                Street = "hmmmm",
                                Town = "t",
                                AddressType = AddressType.M,

                            },
                            #endregion booking.edentity.booker.address
                            Email = "booker@tc.com",
                            EmergencyNumber = "100",
                            Mobile = "0856387455",
                            Phone = "0567865433",

                        }
                        #endregion booking.edentity.booker
                    },
                    #endregion booking.identity
                    #region booking.general
                    BookingGeneral = new BookingGeneral
                    {
                        BookingDate = "20170202",
                        BookingStatus = BookingStatus.Booked,
                        Brand = "AT",
                        BrochureCode = "BROCODE1",
                        Currency = "EUR",
                        DepartureDate = "20170304",
                        Destination = "ASW",
                        Duration = 2,
                        HasComplaint = false,
                        IsLateBooking = false,
                        NumberOfAdults = 1,
                        NumberOfChildren = 2,
                        NumberOfInfants = 0,
                        ReturnDate = "20170324",
                        ToCode = "CDG",
                        TravelAmount = 2000

                    },
                    #endregion booking.general
                    #region booking.remark
                    Remark = new Remark[] {
                    new Remark {
                        Text = "Every room should have good sea view",
                        RemarkType = RemarkType.A
                    }
                },
                    #endregion booking.remark
                    #region booking.services
                    Services = new BookingServices
                    {
                        #region booking.services.accomodation
                        Accommodation = new Accommodation[] {
                        new Accommodation
                        {
                            AccommodationCode = "ACM0001",
                            AccommodationDescription ="5star",
                            BoardType = BoardType.AI,
                            EndDate = "20170306",
                            ExternalServiceCode = "extserv02",
                            GroupAccommodationCode = "RYP0001",
                            HasSharedRoom = false,
                            IsExternalService = false,
                            IsExternalTransfer = false,
                            NeedsTourGuideAssignment= false,
                            NotificationRequired = true,
                              NumberOfParticipants=4,
                              NumberOfRooms = 2,
                              Order = 1,
                              #region booking.services.accomodation.Remark
                              Remark = new Remark[] {
                                  new Remark
                                  {
                                      Text = "Room should have 2 beds and a sofa",
                                      RemarkType = RemarkType.A
                                  }
                              },
                              #endregion booking.services.accomodation.Remark
                              RoomType = "sea view",
                              StartDate = "20170304",
                              Status = AccommodationStatus.OK,
                              #region booking.services.accomodation.tourguideassignment
                              TourGuideAssignment = new TourGuideAssignment
                              {
                                  TourGuide = new TourGuide
                                  {
                                      Brands = new Brands[]
                                      {
                                          new Brands
                                          {

                                          }
                                      },
                                      TourGuideId = "tg001",
                                      TourGuideName = "Mori Aami"
                                  }
                              },
                              #endregion booking.services.accomodation.tourguideassignment
                              TransferServiceLevel = "tsl01",
                              #region booking.services.accomodation.travelparticipantassignment
                              TravelParticipantAssignment = new TravelParticipantAssignment[]
                              {
                                  new TravelParticipantAssignment
                                  {
                                      TravelParticipantId = "TP001"
                                  }
                              },
                            #endregion booking.services.accomodation.travelparticipantassignment
                            WithTransfer = true,
                        },
                        new Accommodation
                        {
                            AccommodationCode = "ACM0002",
                            AccommodationDescription ="5star",
                            BoardType = BoardType.AI,
                            EndDate = "20170318",
                            ExternalServiceCode = "extserv02",
                            GroupAccommodationCode = "HHM001",
                            HasSharedRoom = false,
                            IsExternalService = false,
                            IsExternalTransfer = false,
                            NeedsTourGuideAssignment= false,
                            NotificationRequired = true,
                              NumberOfParticipants=4,
                              NumberOfRooms = 2,
                              Order = 1,
                              #region booking.services.accomodation.Remark
                              Remark = new Remark[] {
                                  new Remark
                                  {
                                      Text = "Room should have 2 beds and a sofa",
                                      RemarkType = RemarkType.A
                                  }
                              },
                              #endregion booking.services.accomodation.Remark
                              RoomType = "sea view",
                              StartDate = "20170309",
                              Status = AccommodationStatus.OK,
                              #region booking.services.accomodation.tourguideassignment
                              TourGuideAssignment = new TourGuideAssignment
                              {
                                  TourGuide = new TourGuide
                                  {
                                      Brands = new Brands[]
                                      {
                                          new Brands
                                          {

                                          }
                                      },
                                      TourGuideId = "tg001",
                                      TourGuideName = "Mori Aami"
                                  }
                              },
                              #endregion booking.services.accomodation.tourguideassignment
                              TransferServiceLevel = "tsl01",
                              #region booking.services.accomodation.travelparticipantassignment
                              TravelParticipantAssignment = new TravelParticipantAssignment[]
                              {
                                  new TravelParticipantAssignment
                                  {
                                      TravelParticipantId = "TP001"
                                  }
                              },
                            #endregion booking.services.accomodation.travelparticipantassignment
                            WithTransfer = true,
                        },
                        new Accommodation
                        {
                            AccommodationCode = "ACM0003",
                            AccommodationDescription ="5star",
                            BoardType = BoardType.AI,
                            EndDate = "20170324",
                            ExternalServiceCode = "extserv02",
                            GroupAccommodationCode = "GH0011",
                            HasSharedRoom = false,
                            IsExternalService = false,
                            IsExternalTransfer = false,
                            NeedsTourGuideAssignment= false,
                            NotificationRequired = true,
                              NumberOfParticipants=4,
                              NumberOfRooms = 2,
                              Order = 1,
                              #region booking.services.accomodation.Remark
                              Remark = new Remark[] {
                                  new Remark
                                  {
                                      Text = "Room should have 2 beds and a sofa",
                                      RemarkType = RemarkType.A
                                  }
                              },
                              #endregion booking.services.accomodation.Remark
                              RoomType = "sea view",
                              StartDate = "20170319",
                              Status = AccommodationStatus.OK,
                              #region booking.services.accomodation.tourguideassignment
                              TourGuideAssignment = new TourGuideAssignment
                              {
                                  TourGuide = new TourGuide
                                  {
                                      Brands = new Brands[]
                                      {
                                          new Brands
                                          {

                                          }
                                      },
                                      TourGuideId = "tg001",
                                      TourGuideName = "Mori Aami"
                                  }
                              },
                              #endregion booking.services.accomodation.tourguideassignment
                              TransferServiceLevel = "tsl01",
                              #region booking.services.accomodation.travelparticipantassignment
                              TravelParticipantAssignment = new TravelParticipantAssignment[]
                              {
                                  new TravelParticipantAssignment
                                  {
                                      TravelParticipantId = "TP001"
                                  }
                              },
                            #endregion booking.services.accomodation.travelparticipantassignment
                            WithTransfer = true,
                        }

                    },
                        #endregion booking.services.accomodation
                        #region booking.services.extraservice
                        ExtraService = new ExtraService[]
                        {
                        new ExtraService {
                            EndDate = "20170324",
                            #region booking.services.extraservice.extraservicecode
                            ExtraServiceCode = "ESCODE001",
                            #endregion booking.services.extraservice.extraservicecode
                            #region  booking.services.extraservicedescription
                            ExtraServiceDescription = "Need a cab everyday to go out.",
                            #endregion  booking.services.extraservicedescription
                            Order = 1,
                            #region booking.services.remark
                            Remark = new Remark[]
                            {
                                new Remark
                                {
                                    Text = "Cab should be of type MUV with 3 seats with good luggage space",
                                    RemarkType = RemarkType.A
                                }
                            },
                             #endregion booking.services.remark
                            StartDate = "20170304",
                            #region booking.services.travelparticipantassignment
                            TravelParticipantAssignment = new TravelParticipantAssignment[]
                            {
                                new TravelParticipantAssignment
                                {
                                    TravelParticipantId = "TP001"
                                }
                             }
                            #endregion
                        }
                        },
                        #endregion booking.services.extraservice
                        #region booking.services,transport
                        Transport = new Transport[]
                            {
                            new Transport {
                                FlightNumber = "SGP3231",
                                ArrivalAirport = "ACA",
                                CarrierCode = "CC001",
                                DepartureAirport = "MEX",
                                EndDate = "20170318",
                                #region booking.services,transport.travelparticipantassignment
                                TravelParticipantAssignment = new TravelParticipantAssignment[]
                                {
                                    new TravelParticipantAssignment
                                {
                                    TravelParticipantId = "TP001"
                                }
                                },
                                #endregion booking.services,transport.travelparticipantassignment
                                FlightIdentifier = "FL001",
                                NumberOfParticipants = 4,
                                Order = 1,
                                #region booking.services,transport.remark
                                Remark = new Remark[] {
                                    new Remark
                                    {
                                        Text = "Need a cab from Airport to Hotel",
                                        RemarkType = RemarkType.TO
                                    }
                                },
                                #endregion booking.services,transport.remark
                                StartDate = "20170310",
                                TransferType = TransferType.IN,
                                TransportCode = "TR001",
                                TransportDescription = "Flight to new york"
                            }
                          },
                        #endregion booking.services,transport
                        #region booking.services.transfer
                        Transfer = new Transfer[]
                        {
                        new Transfer
                        {

                                ArrivalAirport = "ACA",
                                DepartureAirport = "MEX",
                                EndDate = "20170318",
                                #region booking.services.transfer.travelparticipantassignment
                                TravelParticipantAssignment = new TravelParticipantAssignment[]
                                {
                                    new TravelParticipantAssignment
                                {
                                    TravelParticipantId = "TP001"
                                }
                                },
                                #endregion booking.services.transfer.travelparticipantassignment
                                Order = 1,
                                #region booking.services.transfer.remark
                                Remark = new Remark[] {
                                    new Remark
                                    {
                                        Text = "Please book a direct trip",
                                        RemarkType = RemarkType.A
                                    }
                                },
                                #endregion booking.services.transfer.remark
                                StartDate = "20170312",
                                TransferType = TransferType.IN,
                                TransferCode = "TR001",
                                TransferDescription = "Flight to new york",
                                Category = "Flight"
                        }
                        },
                        #endregion booking.services.transfer
                    },
                    #endregion booking.services
                    #region booking.TravelParticipant
                    TravelParticipant = new TravelParticipant[]
                    {
                    new TravelParticipant
                    {
                        Age = "36",
                        Birthdate = "08101982",
                        FirstName = "Barney",
                        Gender = Gender.Male,
                        Language = "Bro-English",
                        LastName = "Stinston",
                        Relation =Relation.Participant,
                        #region booking.TravelParticipant.remark
                        Remark = new Remark[]
                        {
                            new Remark
                            {
                                Text = "Need window ticket",
                                RemarkType = RemarkType.TO
                            }
                        },
                        #endregion booking.TravelParticipant.remark
                        TravelParticipantIdOnTour = "TP001",
                    },
                    new TravelParticipant
                    {
                        Age = "32",
                        Birthdate = "08101984",
                        FirstName = "Kate",
                        Gender = Gender.Female,
                        Language = "Bro-English",
                        LastName = "Winslet",
                        Relation =Relation.Participant,
                        #region booking.TravelParticipant.remark
                        Remark = new Remark[]
                        {
                            new Remark
                            {
                                Text = "Need window ticket",
                                RemarkType = RemarkType.TO
                            }
                        },
                        #endregion booking.TravelParticipant.remark
                        TravelParticipantIdOnTour = "TP002",
                    },
                    new TravelParticipant
                    {
                        Age = "16",
                        Birthdate = "08102000",
                        FirstName = "Marc",
                        Gender = Gender.Male,
                        Language = "Bro-English",
                        LastName = "marquez",
                        Relation =Relation.Child,
                        #region booking.TravelParticipant.remark
                        Remark = new Remark[]
                        {
                            new Remark
                            {
                                Text = "Need window ticket",
                                RemarkType = RemarkType.TO
                            }
                        },
                        #endregion booking.TravelParticipant.remark
                        TravelParticipantIdOnTour = "TP003",
                    },
                    new TravelParticipant
                    {
                        Age = "14",
                        Birthdate = "08102002",
                        FirstName = "shender",
                        Gender = Gender.Female,
                        Language = "Bro-English",
                        LastName = "stella",
                        Relation =Relation.Child,
                        #region booking.TravelParticipant.remark
                        Remark = new Remark[]
                        {
                            new Remark
                            {
                                Text = "Need window ticket",
                                RemarkType = RemarkType.TO
                            }
                        },
                        #endregion booking.TravelParticipant.remark
                        TravelParticipantIdOnTour = "TP004",
                    }
                    }
                        #endregion booking.TravelParticipant
                };

                var bookingJsonData = JsonConvert.SerializeObject(booking);

                File.WriteAllText("booking.json", bookingJsonData);
                System.Console.WriteLine("File has been created with sample json payload for booking.");


            }

            else if (option == "2")
            {
                while (true)
                {
                    System.Console.WriteLine("Reading the Json data");
                    var data = File.ReadAllText("booking.json");
                    var api = "api/booking/update";

                    //create the token
                    var token = CreateJWTToken();

                    //Call
                    HttpClient cons = new HttpClient();

                    cons.BaseAddress = new Uri(GetUrl());

                    cons.DefaultRequestHeaders.Accept.Clear();
                    cons.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    cons.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    Task<HttpResponseMessage> t = cons.PutAsync(api, new StringContent(data, Encoding.UTF8, "application/json"));

                    var response = t.Result;

                    Task<string> task = response.Content.ReadAsStringAsync();
                    var content = task.Result;

                    System.Console.WriteLine("Response Code: {0}", response.StatusCode.GetHashCode());
                    if (response.StatusCode == HttpStatusCode.Created)
                        System.Console.WriteLine("Booking has been created with GUID::{0}", content);
                    else if (response.StatusCode == HttpStatusCode.NoContent)
                        System.Console.WriteLine("Booking has been updated.");
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                        System.Console.WriteLine("Bad Request.");
                    else if (response.StatusCode == HttpStatusCode.InternalServerError)
                        System.Console.WriteLine("Internal Server Error.");
                    else if (response.StatusCode == HttpStatusCode.Forbidden)
                        System.Console.WriteLine("Forbidden.");

                    if (string.IsNullOrWhiteSpace(content))
                        System.Console.WriteLine("No content.");
                    else
                        System.Console.WriteLine("Content:{0}", content);

                    System.Console.Write("Do one more test(y/n):");
                    var ans = System.Console.ReadLine();
                    if (ans == "n") break;

                }
            }
            #endregion main
        }

        private static string CreateJWTToken()
        {
            var payload = new Dictionary<string, object>()
            {
                {"iss", "TC"},
                {"aud", "CRM"},
                {"sub", "anonymous"},
                {"iat", GetIssuedAtTime().ToString()},
                {"nbf", GetNotBeforeTime().ToString()},
                {"exp", GetExpiry().ToString()},
            };

            var header = new Dictionary<string, object>()
            {
                {"alg", "HS256"},
                {"typ", "JWT"},
            };

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            var fileName = ConfigurationManager.AppSettings["privateKeyFileName"];
            rsa.FromXmlString(File.ReadAllText(fileName));
            return Jose.JWT.Encode(payload, rsa, Jose.JwsAlgorithm.RS256);

        }

        

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        private static double GetExpiry()
        {
            var sec = Int32.Parse(ConfigurationManager.AppSettings["expiryFromNow"]);
            return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds + sec);
        }
        private static double GetIssuedAtTime()
        {
            var sec = Int32.Parse(ConfigurationManager.AppSettings["iatSecondsFromNow"]);
            return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds + sec);
        }

        private static double GetNotBeforeTime()
        {
            var sec = Int32.Parse(ConfigurationManager.AppSettings["nbfSecondsFromNow"]);
            return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds + sec);
        }

        private static string GetUrl()
        {
            if (string.IsNullOrWhiteSpace(Url))
                Url = ConfigurationManager.AppSettings["ApiUrl"];
            return Url;
        }


    }
}
