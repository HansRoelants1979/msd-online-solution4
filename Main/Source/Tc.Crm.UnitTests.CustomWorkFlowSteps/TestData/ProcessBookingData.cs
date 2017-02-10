using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.UnitTests.CustomWorkFlowSteps.TestData
{
    public class ProcessBookingData
    {
        public const string Payload_CustomerIdIsNull = @"{
  ""bookingIdentifier"": {
    ""sourceMarket"": ""AX"",
    ""sourceSystem"": ""OnTour"",
    ""bookingNumber"": ""BOO101"",
    ""bookingVersionOnTour"": ""v1"",
    ""bookingVersionTourOperator"": ""v1"",
    ""bookingUpdateDateOnTour"": ""01-01-2017"",
    ""bookingUpdateDateTourOperator"": ""01-01-2017""
  },
  ""bookingGeneral"": {
    ""bookingStatus"": 0,
    ""bookingDate"": ""01-01-2017"",
    ""departureDate"": ""01-01-2017"",
    ""returnDate"": ""01-01-2017"",
    ""duration"": ""2"",
    ""destination"": ""CDG"",
    ""toCode"": ""CDG"",
    ""brand"": ""AT"",
    ""brochureCode"": ""BROCODE1"",
    ""isLateBooking"": false,
    ""numberofParticipants"": 0,
    ""numberOfAdults"": 1,
    ""numberOfChildren"": 0,
    ""numberOfInfants"": 0,
    ""travelAmount"": 2000,
    ""currency"": ""Pounds"",
    ""hasComplaint"": false
  },
  ""bookingIdentity"": {
    ""booker"": {
      ""address"": {
        ""additionalAddressInfo"": ""ad"",
        ""flatNumberUnit"": ""2"",
        ""houseNumberBuilding"": ""3"",
        ""box"": ""2"",
        ""town"": ""t"",
        ""country"": ""AX"",
        ""county"": ""Hmm"",
        ""number"": ""1"",
        ""postalCode"": ""wa001"",
        ""street"": ""hmmmm"",
        ""type"": 0
      },
      ""email"": ""booker@tc.com"",
      ""phone"": ""1212121"",
      ""mobile"": ""2222"",
      ""emergencyNumber"": ""121212""
    }
  },
  ""travelParticipant"": [
    {
      ""firstName"": ""Barney"",
      ""lastName"": ""Stinston"",
      ""age"": 21,
      ""gender"": 0,
      ""relation"": 1,
      ""travelParticipantIDOnTour"": ""TP001"",
      ""language"": ""Bro-English"",
      ""birthDate"": ""01-01-2017"",
      ""Remark"": [
        {
          ""type"": 0,
          ""text"": ""Travel Participant Remark 1""
        }
      ]
    }
  ],
  ""services"": {
    ""accommodation"": [
      {
        ""accommodationCode"": ""acc01"",
        ""groupAccommodationCode"": ""12345"",
        ""accommodationDescription"": ""5star"",
        ""order"": 1,
        ""startDate"": ""01-01-2017"",
        ""endDate"": ""01-01-2017"",
        ""roomType"": ""sea view"",
        ""boardType"": 0,
        ""status"": 0,
        ""hasSharedRoom"": false,
        ""numberOfParticipants"": 2,
        ""numberOfRooms"": 1,
        ""withTransfer"": true,
        ""isExternalService"": false,
        ""externalServiceCode"": ""extserv02"",
        ""notificationRequired"": true,
        ""needsTourGuideAssignment"": false,
        ""isExternalTransfer"": false,
        ""transferServiceLevel"": ""TH"",
        ""travelParticipantAssignment"": [
          {
            ""travelParticipantID"": ""TP001""
          }
        ],
        ""remark"": [
          {
            ""type"": 1,
            ""text"": ""acc remark""
          }
        ],
        ""tourguideAssignment"": {
          ""tourguide"": {
            ""tourguideID"": ""tg001"",
            ""tourguideName"": ""Mori Aami"",
            ""brands"": [
              {}
            ]
          }
        }
      }
    ],
    ""transport"": [
      {
        ""transportCode"": ""TR001"",
        ""transportDescription"": ""Flight to new york"",
        ""order"": 1,
        ""startDate"": ""01-01-2017"",
        ""endDate"": ""01-01-2017"",
        ""transferType"": 0,
        ""departureAirport"": ""CDG"",
        ""arrivalAirport"": ""CDG"",
        ""carrierCode"": ""CC001"",
        ""flightNumber"": ""SGP3231"",
        ""flightIdentifier"": ""FL001"",
        ""numberOfParticipants"": 2,
        ""travelParticipantAssignment"": [
          {
            ""travelParticipantID"": ""TP001""
          }
        ],
        ""remark"": [
          {
            ""type"": 0,
            ""text"": ""Transport remark 1""
          }
        ]
      }
    ],
    ""transfer"": [
      {
        ""transferCode"": ""TR001"",
        ""transferDescription"": ""Flight to new york"",
        ""order"": 1,
        ""startDate"": ""01-01-2017"",
        ""category"": ""Flight"",
        ""endDate"": ""01-01-2017"",
        ""transferType"": 0,
        ""departureAirport"": ""Heathrow"",
        ""arrivalAirport"": ""Newark"",
        ""travelParticipantAssignment"": [
          {
            ""travelParticipantID"": ""TP001""
          }
        ],
        ""remark"": [
          {
            ""type"": 1,
            ""text"": ""Transport remark 1""
          }
        ]
      }
    ],
    ""extraService"": [
      {
        ""extraServiceCode"": {},
        ""extraServiceDescription"": {},
        ""order"": 1,
        ""startDate"": ""01-01-2017"",
        ""endDate"": ""01-01-2017"",
        ""travelParticipantAssignment"": [
          {
            ""travelParticipantID"": ""TP001""
          }
        ],
        ""remark"": [
          {
            ""type"": 1,
            ""text"": ""extra service remark 1""
          }
        ]
      }
    ]
  },
  ""customer"": {
    ""customerIdentifier"": {
      ""customerID"": """",
      ""businessArea"": ""Hotel"",
      ""sourceMarket"": ""AX"",
      ""sourceSystem"": ""OnTour""
    },
    ""customerGeneral"": {
      ""customerStatus"": 0,
      ""customerType"": 1
    },
    ""customerIdentity"": {
      ""salutation"": ""YoBro"",
      ""academictitle"": ""BTech"",
      ""firstName"": ""Barney"",
      ""middleName"": ""bro"",
      ""lastName"": ""Stinston"",
      ""language"": ""Bro-English"",
      ""gender"": 0,
      ""birthdate"": ""01-01-2017""
    },
    ""company"": {
      ""companyName"": ""Innovate Me""
    },
    ""additional"": {
      ""segment"": ""deceased"",
      ""dateOfdeath"": ""01-01-2017""
    },
    ""address"": [
      {
        ""additionalAddressInfo"": ""Nothing Man"",
        ""flatNumberUnit"": ""402"",
        ""houseNumberBuilding"": ""22"",
        ""box"": ""Hmm"",
        ""town"": ""UU Gully"",
        ""country"": ""AX"",
        ""county"": ""Worcestorshire"",
        ""number"": ""22"",
        ""postalCode"": ""WA1123"",
        ""street"": ""Village"",
        ""type"": 0
      }
    ],
    ""phone"": [
      {
        ""type"": 0,
        ""number"": ""32323232""
      }
    ],
    ""email"": [
      {
        ""type"": 0,
        ""address"": ""joe@innovateme.com""
      }
    ],
    ""social"": [
      {
        ""type"": ""I am"",
        ""value"": ""Social""
      }
    ]
  },
  ""remark"": [
    {
      ""type"": 1,
      ""text"": ""Booking Remark 1""
    }
  ]
}";
        public const string Payload_CustomerIdentifiernull = @"{
  ""bookingIdentifier"": {
    ""sourceMarket"": ""AX"",
    ""sourceSystem"": ""OnTour"",
    ""bookingNumber"": ""BOO101"",
    ""bookingVersionOnTour"": ""v1"",
    ""bookingVersionTourOperator"": ""v1"",
    ""bookingUpdateDateOnTour"": ""01-01-2017"",
    ""bookingUpdateDateTourOperator"": ""01-01-2017""
  },
  ""bookingGeneral"": {
    ""bookingStatus"": 0,
    ""bookingDate"": ""01-01-2017"",
    ""departureDate"": ""01-01-2017"",
    ""returnDate"": ""01-01-2017"",
    ""duration"": ""2"",
    ""destination"": ""CDG"",
    ""toCode"": ""CDG"",
    ""brand"": ""AT"",
    ""brochureCode"": ""BROCODE1"",
    ""isLateBooking"": false,
    ""numberofParticipants"": 0,
    ""numberOfAdults"": 1,
    ""numberOfChildren"": 0,
    ""numberOfInfants"": 0,
    ""travelAmount"": 2000,
    ""currency"": ""Pounds"",
    ""hasComplaint"": false
  },
  ""bookingIdentity"": {
    ""booker"": {
      ""address"": {
        ""additionalAddressInfo"": ""ad"",
        ""flatNumberUnit"": ""2"",
        ""houseNumberBuilding"": ""3"",
        ""box"": ""2"",
        ""town"": ""t"",
        ""country"": ""AX"",
        ""county"": ""Hmm"",
        ""number"": ""1"",
        ""postalCode"": ""wa001"",
        ""street"": ""hmmmm"",
        ""type"": 0
      },
      ""email"": ""booker@tc.com"",
      ""phone"": ""1212121"",
      ""mobile"": ""2222"",
      ""emergencyNumber"": ""121212""
    }
  },
  ""travelParticipant"": [
    {
      ""firstName"": ""Barney"",
      ""lastName"": ""Stinston"",
      ""age"": 21,
      ""gender"": 0,
      ""relation"": 1,
      ""travelParticipantIDOnTour"": ""TP001"",
      ""language"": ""Bro-English"",
      ""birthDate"": ""01-01-2017"",
      ""Remark"": [
        {
          ""type"": 0,
          ""text"": ""Travel Participant Remark 1""
        }
      ]
    }
  ],
  ""services"": {
    ""accommodation"": [
      {
        ""accommodationCode"": ""acc01"",
        ""groupAccommodationCode"": ""12345"",
        ""accommodationDescription"": ""5star"",
        ""order"": 1,
        ""startDate"": ""01-01-2017"",
        ""endDate"": ""01-01-2017"",
        ""roomType"": ""sea view"",
        ""boardType"": 0,
        ""status"": 0,
        ""hasSharedRoom"": false,
        ""numberOfParticipants"": 2,
        ""numberOfRooms"": 1,
        ""withTransfer"": true,
        ""isExternalService"": false,
        ""externalServiceCode"": ""extserv02"",
        ""notificationRequired"": true,
        ""needsTourGuideAssignment"": false,
        ""isExternalTransfer"": false,
        ""transferServiceLevel"": ""TH"",
        ""travelParticipantAssignment"": [
          {
            ""travelParticipantID"": ""TP001""
          }
        ],
        ""remark"": [
          {
            ""type"": 1,
            ""text"": ""acc remark""
          }
        ],
        ""tourguideAssignment"": {
          ""tourguide"": {
            ""tourguideID"": ""tg001"",
            ""tourguideName"": ""Mori Aami"",
            ""brands"": [
              {}
            ]
          }
        }
      }
    ],
    ""transport"": [
      {
        ""transportCode"": ""TR001"",
        ""transportDescription"": ""Flight to new york"",
        ""order"": 1,
        ""startDate"": ""01-01-2017"",
        ""endDate"": ""01-01-2017"",
        ""transferType"": 0,
        ""departureAirport"": ""CDG"",
        ""arrivalAirport"": ""CDG"",
        ""carrierCode"": ""CC001"",
        ""flightNumber"": ""SGP3231"",
        ""flightIdentifier"": ""FL001"",
        ""numberOfParticipants"": 2,
        ""travelParticipantAssignment"": [
          {
            ""travelParticipantID"": ""TP001""
          }
        ],
        ""remark"": [
          {
            ""type"": 0,
            ""text"": ""Transport remark 1""
          }
        ]
      }
    ],
    ""transfer"": [
      {
        ""transferCode"": ""TR001"",
        ""transferDescription"": ""Flight to new york"",
        ""order"": 1,
        ""startDate"": ""01-01-2017"",
        ""category"": ""Flight"",
        ""endDate"": ""01-01-2017"",
        ""transferType"": 0,
        ""departureAirport"": ""Heathrow"",
        ""arrivalAirport"": ""Newark"",
        ""travelParticipantAssignment"": [
          {
            ""travelParticipantID"": ""TP001""
          }
        ],
        ""remark"": [
          {
            ""type"": 1,
            ""text"": ""Transport remark 1""
          }
        ]
      }
    ],
    ""extraService"": [
      {
        ""extraServiceCode"": {},
        ""extraServiceDescription"": {},
        ""order"": 1,
        ""startDate"": ""01-01-2017"",
        ""endDate"": ""01-01-2017"",
        ""travelParticipantAssignment"": [
          {
            ""travelParticipantID"": ""TP001""
          }
        ],
        ""remark"": [
          {
            ""type"": 1,
            ""text"": ""extra service remark 1""
          }
        ]
      }
    ]
  },
  ""customer"": {
    ""customerGeneral"": {
      ""customerStatus"": 0,
      ""customerType"": 1
    },
    ""customerIdentity"": {
      ""salutation"": ""YoBro"",
      ""academictitle"": ""BTech"",
      ""firstName"": ""Barney"",
      ""middleName"": ""bro"",
      ""lastName"": ""Stinston"",
      ""language"": ""Bro-English"",
      ""gender"": 0,
      ""birthdate"": ""01-01-2017""
    },
    ""company"": {
      ""companyName"": ""Innovate Me""
    },
    ""additional"": {
      ""segment"": ""deceased"",
      ""dateOfdeath"": ""01-01-2017""
    },
    ""address"": [
      {
        ""additionalAddressInfo"": ""Nothing Man"",
        ""flatNumberUnit"": ""402"",
        ""houseNumberBuilding"": ""22"",
        ""box"": ""Hmm"",
        ""town"": ""UU Gully"",
        ""country"": ""AX"",
        ""county"": ""Worcestorshire"",
        ""number"": ""22"",
        ""postalCode"": ""WA1123"",
        ""street"": ""Village"",
        ""type"": 0
      }
    ],
    ""phone"": [
      {
        ""type"": 0,
        ""number"": ""32323232""
      }
    ],
    ""email"": [
      {
        ""type"": 0,
        ""address"": ""joe@innovateme.com""
      }
    ],
    ""social"": [
      {
        ""type"": ""I am"",
        ""value"": ""Social""
      }
    ]
  },
  ""remark"": [
    {
      ""type"": 1,
      ""text"": ""Booking Remark 1""
    }
  ]
}";
        public const string Payload_CustomerNull = @"{
  ""bookingIdentifier"": {
    ""sourceMarket"": ""AX"",
    ""sourceSystem"": ""OnTour"",
    ""bookingNumber"": ""BOO101"",
    ""bookingVersionOnTour"": ""v1"",
    ""bookingVersionTourOperator"": ""v1"",
    ""bookingUpdateDateOnTour"": ""01-01-2017"",
    ""bookingUpdateDateTourOperator"": ""01-01-2017""
  },
  ""bookingGeneral"": {
    ""bookingStatus"": 0,
    ""bookingDate"": ""01-01-2017"",
    ""departureDate"": ""01-01-2017"",
    ""returnDate"": ""01-01-2017"",
    ""duration"": ""2"",
    ""destination"": ""CDG"",
    ""toCode"": ""CDG"",
    ""brand"": ""AT"",
    ""brochureCode"": ""BROCODE1"",
    ""isLateBooking"": false,
    ""numberofParticipants"": 0,
    ""numberOfAdults"": 1,
    ""numberOfChildren"": 0,
    ""numberOfInfants"": 0,
    ""travelAmount"": 2000,
    ""currency"": ""Pounds"",
    ""hasComplaint"": false
  },
  ""bookingIdentity"": {
    ""booker"": {
      ""address"": {
        ""additionalAddressInfo"": ""ad"",
        ""flatNumberUnit"": ""2"",
        ""houseNumberBuilding"": ""3"",
        ""box"": ""2"",
        ""town"": ""t"",
        ""country"": ""AX"",
        ""county"": ""Hmm"",
        ""number"": ""1"",
        ""postalCode"": ""wa001"",
        ""street"": ""hmmmm"",
        ""type"": 0
      },
      ""email"": ""booker@tc.com"",
      ""phone"": ""1212121"",
      ""mobile"": ""2222"",
      ""emergencyNumber"": ""121212""
    }
  },
  ""travelParticipant"": [
    {
      ""firstName"": ""Barney"",
      ""lastName"": ""Stinston"",
      ""age"": 21,
      ""gender"": 0,
      ""relation"": 1,
      ""travelParticipantIDOnTour"": ""TP001"",
      ""language"": ""Bro-English"",
      ""birthDate"": ""01-01-2017"",
      ""Remark"": [
        {
          ""type"": 0,
          ""text"": ""Travel Participant Remark 1""
        }
      ]
    }
  ],
  ""services"": {
    ""accommodation"": [
      {
        ""accommodationCode"": ""acc01"",
        ""groupAccommodationCode"": ""12345"",
        ""accommodationDescription"": ""5star"",
        ""order"": 1,
        ""startDate"": ""01-01-2017"",
        ""endDate"": ""01-01-2017"",
        ""roomType"": ""sea view"",
        ""boardType"": 0,
        ""status"": 0,
        ""hasSharedRoom"": false,
        ""numberOfParticipants"": 2,
        ""numberOfRooms"": 1,
        ""withTransfer"": true,
        ""isExternalService"": false,
        ""externalServiceCode"": ""extserv02"",
        ""notificationRequired"": true,
        ""needsTourGuideAssignment"": false,
        ""isExternalTransfer"": false,
        ""transferServiceLevel"": ""TH"",
        ""travelParticipantAssignment"": [
          {
            ""travelParticipantID"": ""TP001""
          }
        ],
        ""remark"": [
          {
            ""type"": 1,
            ""text"": ""acc remark""
          }
        ],
        ""tourguideAssignment"": {
          ""tourguide"": {
            ""tourguideID"": ""tg001"",
            ""tourguideName"": ""Mori Aami"",
            ""brands"": [
              {}
            ]
          }
        }
      }
    ],
    ""transport"": [
      {
        ""transportCode"": ""TR001"",
        ""transportDescription"": ""Flight to new york"",
        ""order"": 1,
        ""startDate"": ""01-01-2017"",
        ""endDate"": ""01-01-2017"",
        ""transferType"": 0,
        ""departureAirport"": ""CDG"",
        ""arrivalAirport"": ""CDG"",
        ""carrierCode"": ""CC001"",
        ""flightNumber"": ""SGP3231"",
        ""flightIdentifier"": ""FL001"",
        ""numberOfParticipants"": 2,
        ""travelParticipantAssignment"": [
          {
            ""travelParticipantID"": ""TP001""
          }
        ],
        ""remark"": [
          {
            ""type"": 0,
            ""text"": ""Transport remark 1""
          }
        ]
      }
    ],
    ""transfer"": [
      {
        ""transferCode"": ""TR001"",
        ""transferDescription"": ""Flight to new york"",
        ""order"": 1,
        ""startDate"": ""01-01-2017"",
        ""category"": ""Flight"",
        ""endDate"": ""01-01-2017"",
        ""transferType"": 0,
        ""departureAirport"": ""Heathrow"",
        ""arrivalAirport"": ""Newark"",
        ""travelParticipantAssignment"": [
          {
            ""travelParticipantID"": ""TP001""
          }
        ],
        ""remark"": [
          {
            ""type"": 1,
            ""text"": ""Transport remark 1""
          }
        ]
      }
    ],
    ""extraService"": [
      {
        ""extraServiceCode"": {},
        ""extraServiceDescription"": {},
        ""order"": 1,
        ""startDate"": ""01-01-2017"",
        ""endDate"": ""01-01-2017"",
        ""travelParticipantAssignment"": [
          {
            ""travelParticipantID"": ""TP001""
          }
        ],
        ""remark"": [
          {
            ""type"": 1,
            ""text"": ""extra service remark 1""
          }
        ]
      }
    ]
  },
  ""remark"": [
    {
      ""type"": 1,
      ""text"": ""Booking Remark 1""
    }
  ]
}";
    }
}
