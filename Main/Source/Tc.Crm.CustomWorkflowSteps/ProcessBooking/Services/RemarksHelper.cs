using System.Text;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services
{
    public static class RemarksHelper
    {
        
        public static string GetRemarksTextFromPayload(Remark[] remarks)
        {
            if (remarks == null || remarks.Length == 0) return null;
            StringBuilder remarksText = new StringBuilder();
            
            for (int i = 0; i < remarks.Length; i++)
            {
                if(i>0)
                    remarksText.AppendLine();
                remarksText.AppendFormat("{0}:", remarks[i].RemarkType.ToString());
                remarksText.AppendLine();
                remarksText.Append(remarks[i].Text);
            }

            return remarksText.ToString();
        }
    }
}
