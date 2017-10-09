using JsonPatch;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Text;
using Tc.Crm.Service.CacheBuckets;
using Tc.Crm.Service.Models;
using static Tc.Crm.Service.Constants.Crm.Actions;

namespace Tc.Crm.Service.Services
{
    public class CustomerService : ICustomerService
    {

        CountryBucket countryBucket;
        SourceMarketBucket sourceMarketBucket;

        public CustomerService(CountryBucket countryBucket,
                                SourceMarketBucket sourceMarketBucket)
        {
            this.countryBucket = countryBucket;
            this.sourceMarketBucket = sourceMarketBucket;

            if (this.countryBucket.Items == null || this.countryBucket.Items.Count == 0)
                this.countryBucket.FillBucket();

            if (this.sourceMarketBucket.Items == null || this.sourceMarketBucket.Items.Count == 0)
                this.sourceMarketBucket.FillBucket();
        }

        public CustomerResponse Create(string customerData, ICrmService crmService)
        {
            if (string.IsNullOrWhiteSpace(customerData)) throw new ArgumentNullException(Constants.Parameters.ProcessCustomer);
            if (crmService == null) throw new ArgumentNullException(Constants.Parameters.CrmService);

            var response = crmService.ExecuteActionOnCustomerEvent(customerData, OperationType.Post);
            if (response == null) throw new InvalidOperationException(Constants.Messages.ResponseNull);
            return response;
        }

        public CustomerResponse Update(string customerId, CustomerInformation customerInformation, ICrmService crmService)
        {
            if (crmService == null) throw new ArgumentNullException(Constants.Parameters.CrmService);

            customerInformation.Customer.CustomerIdentifier.CustomerId = customerId;
            string customerData = JsonConvert.SerializeObject(customerInformation.Customer, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore });
            
            var response = crmService.ExecuteActionOnCustomerEvent(customerData, OperationType.Patch);
            if (response == null) throw new InvalidOperationException(Constants.Messages.ResponseNull);
            return response;

        }

        public Collection<string> ValidateCustomerPatchRequest(CustomerInformation customerInfo)
        {
            var validationMessages = new Collection<string>();

            if (customerInfo == null || customerInfo.Customer == null)
            {
                validationMessages.Add(Constants.Messages.CustomerDataPassedIsNullOrCouldNotBeParsed);
                return validationMessages;
            }

            if (customerInfo.Customer.CustomerGeneral.CustomerType == CustomerType.NotSpecified)
                validationMessages.Add(Constants.Messages.CustomerTypeNotPresent);
 
            return validationMessages;
        }

        public Collection<string> Validate(CustomerInformation customerInformation)
        {
            var validationMessages = new Collection<string>();
            if (customerInformation == null || customerInformation.Customer == null)
            {
                validationMessages.Add(Constants.Messages.CustomerDataPassedIsNullOrCouldNotBeParsed);
                return validationMessages;
            }
            var customer = customerInformation.Customer;

            if (customer != null && (customer.CustomerIdentifier == null
                || string.IsNullOrWhiteSpace(customer.CustomerIdentifier.CustomerId)))
                validationMessages.Add(Constants.Messages.CustomerIdIsNull);

            var customerGeneral = customer.CustomerGeneral;

            if (customerGeneral == null)
                validationMessages.Add(Constants.Messages.CustomerGeneralNotPresent);

            if (customerGeneral != null && customerGeneral.CustomerType == CustomerType.NotSpecified)
                validationMessages.Add(Constants.Messages.CustomerTypeNotPresent);
            if (customerGeneral != null && customerGeneral.CustomerType == CustomerType.Company)
                return validationMessages;
            var customerIdentity = customer.CustomerIdentity;
            if (customerIdentity == null)
                validationMessages.Add(Constants.Messages.CustomerIdentityNotPresent);

            return validationMessages;
        }

        public string GetStringFrom(Collection<string> strings)
        {
            if (strings == null || strings.Count == 0) return null;
            StringBuilder message = new StringBuilder();
            for (int i = 0; i < strings.Count; i++)
            {
                if (i == strings.Count - 1)
                    message.Append(strings[i]);
                else
                    message.AppendLine(strings[i]);
            }
            foreach (var item in strings)
            {

            }
            return message.ToString();
        }

        public void ResolveReferences(Customer customerInfo)
        {
            ResolveCountryReferences(customerInfo);
        }

        public void ResolveCountryReferences(Customer customer)
        {
            if (customer == null) return;

            if (customer.Address != null)
            {
                foreach (var address in customer.Address)
                {
                    address.Country = countryBucket.GetBy(address.Country);
                }
            }

            if (customer.CustomerIdentifier != null)
            {
                var sourceMarket = sourceMarketBucket.GetBy(customer.CustomerIdentifier.SourceMarket);
                if (sourceMarket != null)
                {
                    customer.CustomerIdentifier.SourceMarket = sourceMarket.Id;
                    if (!string.IsNullOrWhiteSpace(sourceMarket.TeamId))
                        customer.Owner = sourceMarket.TeamId;
                }
                else
                    customer.CustomerIdentifier.SourceMarket = null;
            }


        }
    }
}