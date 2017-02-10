
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
                        BookingNumber = "BOO101",
                        BookingUpdateDateOnTour = "20170102",
                        BookingUpdateDateTourOperator = "20170102",
                        BookingVersionOnTour = "v1",
                        BookingVersionTourOperator = "v1",
                        SourceMarket = "England",
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
                            AdditionalAddressInfo = "Nothing Man",
                            Box = "Hmm",
                            Country = "England",
                            County = "Worcestorshire",
                            FlatNumberUnit = "402",
                            HouseNumberBuilding = "22",
                            Number = "22",
                            PostalCode = "WA1123",
                            Street = "Village",
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
                            CustomerId = "CON001",
                            SourceMarket = "England",
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
                            AcademicTitle = "BTech",
                            Birthdate = "19821008",
                            FirstName = "Barney",
                            Gender = Gender.M,
                            Language = "Bro-English",
                            LastName = "Stinston",
                            MiddleName = "bro",
                            Salutation = "YoBro"
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
                            EmergencyNumber = "121212",
                            Mobile = "2222",
                            Phone = "1212121",

                        }
                        #endregion booking.edentity.booker
                    },
                    #endregion booking.identity
                    #region booking.general
                    BookingGeneral = new BookingGeneral
                    {
                        BookingDate = "20170101",
                        BookingStatus = BookingStatus.B,
                        Brand = "Humma",
                        BrochureCode = "BROCODE1",
                        Currency = "Pounds",
                        DepartureDate = "20170131",
                        Destination = "NYC",
                        Duration = "2 nights",
                        HasComplaint = false,
                        IsLateBooking = false,
                        NumberOfAdults = 1,
                        NumberOfChildren = 0,
                        NumberOfInfants = 0,
                        ReturnDate = "20170202",
                        ToCode = "NYC",
                        TravelAmount = 2000

                    },
                    #endregion booking.general
                    #region booking.remark
                    Remark = new Remark[] {
                    new Remark {
                        Text = "Booking Remark 1",
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
                            AccommodationCode = "acc01",
                            AccommodationDescription ="5star",
                            BoardType = BoardType.AI,
                            EndDate = "20170202",
                            ExternalServiceCode = "extserv02",
                            GroupAccommodationCode = "grp02",
                            HasSharedRoom = false,
                            IsExternalService = false,
                            IsExternalTransfer = false,
                            NeedsTourGuideAssignment= false,
                            NotificationRequired = true,
                              NumberOfParticipants=2,
                              NumberOfRooms = 1,
                              Order = 1,
                              #region booking.services.accomodation.Remark
                              Remark = new Remark[] {
                                  new Remark
                                  {
                                      Text = "acc remark",
                                      RemarkType = RemarkType.A
                                  }
                              },
                              #endregion booking.services.accomodation.Remark
                              RoomType = "sea view",
                              StartDate = "20170131",
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

                    },
                        #endregion booking.services.accomodation
                        #region booking.services.extraservice
                        ExtraService = new ExtraService[]
                        {
                        new ExtraService {
                            EndDate = "20170202",
                            #region booking.services.extraservice.extraservicecode
                            ExtraServiceCode = new ExtraServiceCode {

                            },
                            #endregion booking.services.extraservice.extraservicecode
                            #region  booking.services.extraservicedescription
                            ExtraServiceDescription = new ExtraServiceDescription
                            {

                            },
                            #endregion  booking.services.extraservicedescription
                            Order = 1,
                            #region booking.services.remark
                            Remark = new Remark[]
                            {
                                new Remark
                                {
                                    Text = "extra service remark 1",
                                    RemarkType = RemarkType.A
                                }
                            },
                             #endregion booking.services.remark
                            StartDate = "20170131",
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
                                ArrivalAirport = "Newark",
                                CarrierCode = "CC001",
                                DepartureAirport = "Heathrow",
                                EndDate = "20170202",
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
                                NumberOfParticipants = 2,
                                Order = 1,
                                #region booking.services,transport.remark
                                Remark = new Remark[] {
                                    new Remark
                                    {
                                        Text = "Transport remark 1",
                                        RemarkType = RemarkType.T
                                    }
                                },
                                #endregion booking.services,transport.remark
                                StartDate = "20170131",
                                TransferType = TransferType.I,
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

                                ArrivalAirport = "Newark",
                                DepartureAirport = "Heathrow",
                                EndDate = "20170202",
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
                                        Text = "Transport remark 1",
                                        RemarkType = RemarkType.A
                                    }
                                },
                                #endregion booking.services.transfer.remark
                                StartDate = "20170131",
                                TransferType = TransferType.I,
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
                        Age = 21,
                        Birthdate = "08101982",
                        FirstName = "Barney",
                        Gender = Gender.M,
                        Language = "Bro-English",
                        LastName = "Stinston",
                        Relation =Relation.C,
                        #region booking.TravelParticipant.remark
                        Remark = new Remark[]
                        {
                            new Remark
                            {
                                Text = "Travel Participant Remark 1",
                                RemarkType = RemarkType.T
                            }
                        },
                        #endregion booking.TravelParticipant.remark
                        TravelParticipantIdOnTour = "TP001",
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

                    if (response.StatusCode == HttpStatusCode.Created)
                        System.Console.WriteLine("Booking has been created with GUID::{0}", content);
                    else if (response.StatusCode == HttpStatusCode.NoContent)
                        System.Console.WriteLine("Booking has been updated.");
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                        System.Console.WriteLine("Bad Request.");
                    else if (response.StatusCode == HttpStatusCode.InternalServerError)
                        System.Console.WriteLine("Internal Server Error.");

                    if (string.IsNullOrWhiteSpace(content))
                        System.Console.WriteLine("No content.");
                    else
                        System.Console.WriteLine("Content:{0}", content);

                    System.Console.Write("Do one more test(y/n):");
                    var ans = System.Console.ReadLine();
                    if (ans == "n") break;

                }
            }
            System.Console.ReadLine();
        }

        private static string CreateJWTToken()
        {
            var payload = new Dictionary<string, object>()
            {
                {"iss", "TC"},
                {"aud", "CRM"},
                {"sub", "anonymous"},
                {"iat", GetIssuedAtTime().ToString()},
                {"exp", GetNotBeforeTime().ToString()},
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
        private static double GetIssuedAtTime()
        {
            return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds);
        }

        private static double GetNotBeforeTime()
        {
            return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds + 1800);
        }

        private static string GetUrl()
        {
            if (string.IsNullOrWhiteSpace(Url))
                Url = ConfigurationManager.AppSettings["ApiUrl"];
            return Url;
        }


    }
}
