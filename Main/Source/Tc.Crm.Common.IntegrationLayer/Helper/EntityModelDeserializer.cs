using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.Common.IntegrationLayer.Model;

namespace Tc.Crm.Common.IntegrationLayer.Helper
{
	class EntityModelDeserializer : IEntityModelDeserializer
	{
		public EntityModel Deserialize(string data)
		{
			using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
			{
				DataContractJsonSerializer deSerializer = new DataContractJsonSerializer(typeof(EntityModel));
				return deSerializer.ReadObject(memoryStream) as EntityModel;
			}
		}
	}
}
