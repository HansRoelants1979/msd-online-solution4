using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.Plugins.CacheRequest.BusinessLogic
{
    public static class JsonHelper<T>
    {
        public static string SerializeJson(T token)
        {
            if (token == null) throw new InvalidPluginExecutionException("token is null;");
            using (var memoryStream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(memoryStream, token);
                byte[] json = memoryStream.ToArray();
                return Encoding.UTF8.GetString(json, 0, json.Length);
            }
        }
    }
}
