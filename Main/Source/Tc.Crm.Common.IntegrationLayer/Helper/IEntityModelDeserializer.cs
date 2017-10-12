using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.Common.IntegrationLayer.Model;

namespace Tc.Crm.Common.IntegrationLayer.Helper
{
	public interface IEntityModelDeserializer
	{
		EntityModel Deserialize(string data);
	}
}
