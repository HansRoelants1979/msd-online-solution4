
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
using System.Threading;

namespace Tc.Crm.Service.Client.Console
{
    class Program
    {



        static string Url = string.Empty;
        //static int bookingnum = 0;
        static string bookingidtoUpdate = null;
        static string CustomeridtoUpdate = null;
        static void Main(string[] args)
        {
            try
            {
                //read config
                var Totalrecords = Int32.Parse(ConfigurationManager.AppSettings["TotalRecords"]);
                var Batch = Int32.Parse(ConfigurationManager.AppSettings["Batch"]);
                var Records_to_Create = Int32.Parse(ConfigurationManager.AppSettings["Records_to_Create"]);
                var Records_to_Update = Int32.Parse(ConfigurationManager.AppSettings["Records_to_Update"]);
                //calculate threads for create and update
                var totalThreadsForCreate = (((Totalrecords * Records_to_Create)/100) / Batch);
                var totalThreadsForUpdate = (((Totalrecords * Records_to_Update)/100)  / Batch);

                var json = File.ReadAllText("bookingper.json");
                var booking = JsonConvert.DeserializeObject<BookingInformation>(json);
                List<Thread> createThreads = new List<Thread>();
                List<Thread> updateThreads = new List<Thread>();
                ////create
                System.Console.WriteLine("Start:{0}", DateTime.Now.ToString());
                for (int i = 0; i < totalThreadsForCreate; i++)
                {
                    createThreads.Add(StartThread(json, Batch));

                }

                foreach (var item in createThreads)
                {
                    item.Join();
                }

                for (int i = 0; i < totalThreadsForUpdate; i++)
                {
                    updateThreads.Add(StartThreadForUpdate(json, Batch));

                }

                foreach (var item in updateThreads)
                {
                    item.Join();
                }

                System.Console.WriteLine("End:{0}",DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Unhandled Exception:: Message: {0} ", ex.Message);
                System.Console.WriteLine("Unhandled Exception:: Stack Trace: {0} ", ex.StackTrace.ToString());
            }
            System.Console.ReadLine();
        }

        private static Thread StartThread(string json, int batch)

        {

           
            Thread thread = new Thread(() =>
            {

                for (int i = 0; i < batch; i++)
                {
                    var booking = JsonConvert.DeserializeObject<BookingInformation>(json);
                    //i = i++;
                    var bookingid = "perfTest_" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff") + Thread.CurrentThread.ManagedThreadId;
                    if (bookingidtoUpdate == null)
                        bookingidtoUpdate = bookingid;
                    var customerid = "perfcust_" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff") + Thread.CurrentThread.ManagedThreadId;
                    if (CustomeridtoUpdate == null)
                        CustomeridtoUpdate = customerid;

                    booking.Booking.BookingIdentifier.BookingNumber = bookingid;
                    booking.Booking.Customer.CustomerIdentifier.CustomerId = customerid;
                    //modify booking
                    var data = JsonConvert.SerializeObject(booking);
                    CreateOrUpdate(data);
                }
            });

            thread.Start();
            return thread;


        }
        private static Thread StartThreadForUpdate(string json, int batch)

        {


            Thread thread = new Thread(() =>
            {

                for (int i = 0; i < batch; i++)
                {
                    var booking = JsonConvert.DeserializeObject<BookingInformation>(json);
                     
                    

                    booking.Booking.BookingIdentifier.BookingNumber = bookingidtoUpdate;
                    booking.Booking.Customer.CustomerIdentifier.CustomerId = CustomeridtoUpdate;
                    //modify booking
                    var data = JsonConvert.SerializeObject(booking);
                    CreateOrUpdate(data);
                }
            });

            thread.Start();
            return thread;


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
