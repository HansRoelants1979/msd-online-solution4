
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
            System.Console.Write("Customer or Booking(c/b):");
            var r = System.Console.ReadLine();
            var api = "";
            var data = "";
            if (r == "c")
            {
                Customer c = new Customer();
                //customer
                System.Console.Write("Enter First Name:");
                c.FirstName = System.Console.ReadLine();

                System.Console.Write("Enter Last Name:");
                c.LastName = System.Console.ReadLine();

                System.Console.Write("Enter Source Key:");
                c.Id = System.Console.ReadLine();

                System.Console.Write("Enter Email:");
                c.Email = System.Console.ReadLine();

                //Serialize to JSON
                data = JsonConvert.SerializeObject(c);

                api = "api/customer/update";
            }
            else if (r == "b")
            {
                Booking b = new Booking();
                //booking
                System.Console.Write("Enter Total Amount:");
                b.TotalAmount = Decimal.Parse(System.Console.ReadLine());

                System.Console.Write("Enter Customer Key:");
                b.CustomerId = System.Console.ReadLine();

                System.Console.Write("Enter Source Key:");
                b.Id = System.Console.ReadLine();

                //Serialize to JSON
                data = JsonConvert.SerializeObject(b);

                api = "api/booking/update";
            }
            else
                return;

            //create the token
            var token = CreateJWTToken();

            //Call
            HttpClient cons = new HttpClient();

            cons.BaseAddress = new Uri(GetUrl());

            cons.DefaultRequestHeaders.Accept.Clear();
            cons.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            cons.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Task<HttpResponseMessage> t = cons.PutAsync(api,new StringContent(data,Encoding.UTF8,"application/json"));

            var res = t.Result;

            res.EnsureSuccessStatusCode();

           
            System.Console.ReadLine();
        }

        private static string CreateJWTToken()
        {
            byte[] secretKey = Encoding.UTF8.GetBytes(GetJwtKey());
            
            var payload = new Dictionary<string, object>()
            {
                {"iss", "TC"},
                {"aud", "CRM"},
                {"sub", "anonymous"},
                {"iat", GetIat().ToString()},
                {"exp", GetNbf().ToString()},
            };

            var header = new Dictionary<string, object>()
            {
                {"alg", "HS256"},
                {"typ", "JWT"},
            };

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            var fileName = ConfigurationManager.AppSettings["privateKeyFileName"];
            rsa.FromXmlString(File.ReadAllText(fileName));
            return Jose.JWT.Encode(payload,rsa,Jose.JwsAlgorithm.RS256);
           
        }
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        private static double GetIat()
        {
            return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds);
        }

        private static double GetNbf()
        {
            return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds + 1800);
        }

        private static string GetUrl()
        {
            if (string.IsNullOrWhiteSpace(Url))
                Url = ConfigurationManager.AppSettings["ApiUrl"];
            return Url;
        }

        private static string GetJwtKey()
        {
            return ConfigurationManager.AppSettings["jwtkey"];
        }


    }
}
