using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;

namespace Tc.Crm.Service.Client.Console
{
    class Program
    {
        static string Url = string.Empty;
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    System.Console.WriteLine("Do it again. (y/n):");
                    var ans = System.Console.ReadLine();
                    if (ans == "n")
                        break;

                    var b = GetBookingFromConsole();

                    var serializedString = JsonConvert.SerializeObject(b);
                    var inputMessage = new HttpRequestMessage
                    {
                        Content = new StringContent(serializedString, Encoding.UTF8, "application/json")
                    };

                    //var credentials = new NetworkCredential("TC-DEV1\\$TC-DEV1", "jP2s0bWcrbciM7MflYqSTwoERCBqa5L5bMFQ6ziyk9w92FK3iFEdeexAi9iJ");
                    //var handler = new HttpClientHandler { Credentials = credentials };


                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(GetUrl());

                    inputMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage message = client.PutAsync("api/booking", inputMessage.Content).Result;

                    if (message.IsSuccessStatusCode)
                    {
                        IEnumerable<string> headerValues = message.Headers.GetValues("Message");
                        System.Console.WriteLine(headerValues.FirstOrDefault().ToString());
                    }
                    else
                    {
                        System.Console.WriteLine(message.ReasonPhrase);
                    }




                }
                catch (Exception ex)
                {

                    System.Console.WriteLine(ex.Message);
                }
            }
            System.Console.ReadLine();
        }

        private static string GetUrl()
        {
            if(string.IsNullOrWhiteSpace(Url))
                Url =  ConfigurationManager.AppSettings["ApiUrl"];
            return Url;
        }

        private static object GetBookingFromConsole()
        {
            Booking b = new Booking();
            System.Console.Write("Enter first name:");
            b.FirstName = System.Console.ReadLine();

            System.Console.Write("Enter last name:");
            b.LastName = System.Console.ReadLine();

            System.Console.Write("Enter country:");
            b.Country = System.Console.ReadLine();

            System.Console.Write("Enter postcode:");
            b.Postcode = System.Console.ReadLine();

            System.Console.Write("Enter booking id(integer please, else might crash):");
            b.BookingId = Int32.Parse( System.Console.ReadLine());

            return b;
        }
    }
}
