using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Tc.Crm.Plugins.MultipleEntities.Model;

namespace Tc.Crm.Plugins.MultipleEntities.Helper
{
    public class JsonHelper
    {
        /// <summary>
        /// To deserialize json to object
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static EntityModel DeserializeJson(string json, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("trace is null.");
            trace.Trace("Processing DeSerialization of json record Payload - start");
            if (string.IsNullOrWhiteSpace(json)) throw new InvalidPluginExecutionException("json is null;");

            EntityModel record = new EntityModel();
            record.Fields = new List<Field>();
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer deSerializer = new DataContractJsonSerializer(record.Fields.GetType());
                record.Fields = deSerializer.ReadObject(memoryStream) as List<Field>;
            }
            trace.Trace("Processing DeSerialization of json record Payload - start");

            return record;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        public static string SerializeJson(EntityModel record, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("trace is null;");
            if (record == null) throw new InvalidPluginExecutionException("record is null;");
            trace.Trace("Processing Serialization of SerializeJson - start");
            using (var memoryStream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(EntityModel));
                serializer.WriteObject(memoryStream, record);
                byte[] json = memoryStream.ToArray();
                trace.Trace("Processing Serialization of SerializeJson - end");
                return Encoding.UTF8.GetString(json, 0, json.Length);
            }
        }
    }
}
