using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Tc.Crm.Common.IL.Model;

namespace Tc.Crm.Common.IL.Helper
{
    public class JsonHelper
    {
        /// <summary>
        /// To deserialize json to object
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static EntityModel DeserializeJson(string json)
        {
            EntityModel record = new EntityModel();
            record.Fields = new List<Field>();
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer deSerializer = new DataContractJsonSerializer(record.Fields.GetType());
                record.Fields = deSerializer.ReadObject(memoryStream) as List<Field>;
            }
            return record;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        public static string SerializeJson(EntityModel record)
        {            
            using (var memoryStream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(EntityModel));
                serializer.WriteObject(memoryStream, record);
                byte[] json = memoryStream.ToArray();                
                return Encoding.UTF8.GetString(json, 0, json.Length);
            }
        }
    }
}
