
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
                System.Console.WriteLine("Enter 1 to process Booking OR 2 to process survey OR 3 to cache.");

                var option = System.Console.ReadLine();
                if (option == "1")
                {
                    ProcessBooking();
                }
                else if (option == "2")
                {
                    ProcessSurvey();
                }
                else if(option == "3")
                {
                    Cache();
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Unhandled Exception:: Message: {0} ", ex.Message);
                System.Console.WriteLine("Unhandled Exception:: Stack Trace: {0} ", ex.StackTrace.ToString());
            }
            System.Console.ReadLine();
        }
        private static void Cache()
        {

            System.Console.WriteLine("Cache Interface");


            while (true)
            {
                System.Console.WriteLine("Enter the bucket name:");
                var name = System.Console.ReadLine();
               
                var api = "api/cache/refresh";

                //create the token
                var token = CreateJWTTokenWithHmac();
                var pl = new Payload
                {
                    Bucket = name,
                    JWTToken = token
                };
                var data = JsonConvert.SerializeObject(pl);
                //Call
                HttpClient cons = new HttpClient();

                cons.BaseAddress = new Uri(GetUrl());

                cons.DefaultRequestHeaders.Accept.Clear();
                cons.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                Task<HttpResponseMessage> t = cons.PostAsync(api, new StringContent(data, Encoding.UTF8, "application/json"));

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
                var authHeader = bool.Parse(ConfigurationManager.AppSettings["authHeader"]);
                if (authHeader)
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

            System.Console.WriteLine("Processing Booking.");


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
                var authHeader = bool.Parse(ConfigurationManager.AppSettings["authHeader"]);
                if (authHeader)
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

        private static string CreateJWTToken()
        {
            var payload = new Dictionary<string, object>()
            {
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

        private static string CreateJWTTokenWithHmac()
        {
            byte[] secretKey = Encoding.UTF8.GetBytes(GetJwtKey());

            var payload = new Dictionary<string, object>()
            {
                {"iat", GetIssuedAtTime().ToString()},
                {"nbf", GetNotBeforeTime().ToString()},
                {"exp", GetExpiry().ToString()}
            };

            return JWT.JsonWebToken.Encode(payload, secretKey, JwtHashAlgorithm.HS256);

        }

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        private static string GetJwtKey()
        {
            return ConfigurationManager.AppSettings["jwtkey"];
        }
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
