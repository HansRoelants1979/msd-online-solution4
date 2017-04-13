using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.Common.Models
{
    public sealed class Case : EntityModel
    {
        public override string EntityName
        {
            get
            {
                return Constants.EntityName.Case;
            }
        }

        public CaseStatusCode StatusCode { get; set; }
        public CaseState State { get; set; }
    }
}
