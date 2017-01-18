using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.Cryptography.Console
{
    class Program
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        static void Main(string[] args)
        {
            System.Console.WriteLine("Generating RSA public and private keys.");
            var keyPath = ConfigurationManager.AppSettings["keyPath"];
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            var seconds = Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds);
            var privateKeyFileName = string.Format("RSAPrivate_{0}.xml",seconds);
            var publicKeyFileName = string.Format("RSAPublic_{0}.xml", seconds);
            if (!Directory.Exists(keyPath))
            {
                Directory.CreateDirectory(keyPath);
            }
            File.WriteAllText(Path.Combine(keyPath, privateKeyFileName),rsa.ToXmlString(true));  // Private Key
            File.WriteAllText(Path.Combine(keyPath, publicKeyFileName), rsa.ToXmlString(false));  // Public Key
            System.Console.WriteLine("RSA public and private keys have been generated at {0}", keyPath);
        }
    }
}
