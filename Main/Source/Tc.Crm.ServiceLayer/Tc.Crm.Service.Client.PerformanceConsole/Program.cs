
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
                //read config
                //calculate threads for create and update
                var json = File.ReadAllText("booking.json");
                var booking = JsonConvert.DeserializeObject<BookingInformation>(json);

                ////create
                //for (int i = 0; i < length; i++)
                //{

                //    StartThread(booking);

                //}

                ////update
                //for (int i = 0; i < length; i++)
                //{
                //    StartThread(booking);

                //}
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Unhandled Exception:: Message: {0} ", ex.Message);
                System.Console.WriteLine("Unhandled Exception:: Stack Trace: {0} ", ex.StackTrace.ToString());
            }
            System.Console.ReadLine();
        }

        private static void StartThread(BookingInformation booking)
        {
            //for (int i = 0; i < length; i++)
            //{
            //    //modify booking
            //    var data = JsonConvert.SerializeObject(booking);
            //    CreateOrUpdate(data);
            //}
        }

        private static void CreateOrUpdate(string data)
        {
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
