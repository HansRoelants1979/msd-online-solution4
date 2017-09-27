using System.Collections.ObjectModel;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public interface ICustomerService
    {
        CustomerResponse Create(string customerData, ICrmService crmService);
        CustomerResponse Update(string customerId, CustomerInformation customerData, ICrmService crmService);

        Collection<string> Validate(CustomerInformation customerInformation);

        string GetStringFrom(Collection<string> strings);

        Collection<string> ValidateCustomerPatchRequest(CustomerInformation customerInfo);

        void ResolveReferences(Customer customerInfo);
    }
}