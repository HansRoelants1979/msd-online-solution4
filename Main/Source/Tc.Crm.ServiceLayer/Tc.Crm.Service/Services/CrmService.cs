using System;
using Microsoft.Xrm.Sdk;
using System.Configuration;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk.Messages;
using tcm = Tc.Crm.Service.Models;
using Newtonsoft.Json;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Discovery;
using System.ServiceModel.Description;

namespace Tc.Crm.Service.Services
{

    public class CrmService : ICrmService,IDisposable
    {
        private IOrganizationService orgService;
        private string UserName = "";
        private string Password = "";
        private string Domain = "";
        private string OrganizationUniqueName = "";
        private string DiscoveryServiceAddress = "";

        public CrmService()
        {
            UserName = ConfigurationManager.AppSettings[Constants.Configuration.AppSettings.CrmUserName];
            Password = ConfigurationManager.AppSettings[Constants.Configuration.AppSettings.CrmPassword];
            OrganizationUniqueName = ConfigurationManager.AppSettings[Constants.Configuration.AppSettings.CrmOrgUniqueName];
            DiscoveryServiceAddress = ConfigurationManager.AppSettings[Constants.Configuration.AppSettings.CrmDiscoveryAddress];
            orgService = CreateOrgService();
        }

        public tcm.UpdateResponse ExecuteActionForBookingUpdate(string data)
        {
            var request = new OrganizationRequest(Constants.Crm.Actions.ProcessBooking);
            request[Constants.Crm.Actions.ParameterData] = data;
            var response = orgService.Execute(request);
            if (response.Results == null ||
                !response.Results.ContainsKey(Constants.Crm.Actions.ProcessBookingResponse) ||
                response.Results[Constants.Crm.Actions.ProcessBookingResponse] == null)
                throw new InvalidOperationException(Constants.Messages.ResponseFromCrmIsNull);

            var actionResponse = response.Results[Constants.Crm.Actions.ProcessBookingResponse].ToString();

            var responseObject = JsonConvert.DeserializeObject<tcm.UpdateResponse>(actionResponse);

            return new tcm.UpdateResponse { Created= responseObject.Created, Id=responseObject.Id,Success=responseObject.Success,ErrorMessage=responseObject.ErrorMessage};
        }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public IOrganizationService CreateOrgService()
        {
            if (orgService != null) return orgService;

            IServiceManagement<IDiscoveryService> serviceManagement =
                        ServiceConfigurationFactory.CreateManagement<IDiscoveryService>(
                        new Uri(DiscoveryServiceAddress));
            AuthenticationProviderType endpointType = serviceManagement.AuthenticationType;

            // Set the credentials.
            AuthenticationCredentials authCredentials = GetCredentials(serviceManagement, endpointType);


            String organizationUri = String.Empty;
            // Get the discovery service proxy.
            using (DiscoveryServiceProxy discoveryProxy =
                GetProxy<IDiscoveryService, DiscoveryServiceProxy>(serviceManagement, authCredentials))
            {
                // Obtain organization information from the Discovery service. 
                if (discoveryProxy != null)
                {
                    // Obtain information about the organizations that the system user belongs to.
                    OrganizationDetailCollection orgs = DiscoverOrganizations(discoveryProxy);
                    // Obtains the Web address (Uri) of the target organization.
                    organizationUri = FindOrganization(OrganizationUniqueName,
                        orgs.ToArray()).Endpoints[EndpointType.OrganizationService];

                }
            }


            if (!String.IsNullOrWhiteSpace(organizationUri))
            {
                IServiceManagement<IOrganizationService> orgServiceManagement =
                    ServiceConfigurationFactory.CreateManagement<IOrganizationService>(
                    new Uri(organizationUri));

                // Set the credentials.
                AuthenticationCredentials credentials = GetCredentials(orgServiceManagement, endpointType);

                // Get the organization service proxy.
                return GetProxy<IOrganizationService, OrganizationServiceProxy>(orgServiceManagement, credentials);
            }

            return null;


        }
        private AuthenticationCredentials GetCredentials<TService>(IServiceManagement<TService> service, AuthenticationProviderType endpointType)
        {
            AuthenticationCredentials authCredentials = new AuthenticationCredentials();

            switch (endpointType)
            {
                case AuthenticationProviderType.ActiveDirectory:
                    authCredentials.ClientCredentials.Windows.ClientCredential =
                        new System.Net.NetworkCredential(UserName,
                            Password,
                            Domain);
                    break;
                default: // For Federated and OnlineFederated environments.                    
                    authCredentials.ClientCredentials.UserName.UserName = UserName;
                    authCredentials.ClientCredentials.UserName.Password = Password;
                    // For OnlineFederated single-sign on, you could just use current UserPrincipalName instead of passing user name and password.
                    // authCredentials.UserPrincipalName = UserPrincipal.Current.UserPrincipalName;  // Windows Kerberos

                    // The service is configured for User Id authentication, but the user might provide Microsoft
                    // account credentials. If so, the supporting credentials must contain the device credentials.
                    if (endpointType == AuthenticationProviderType.OnlineFederation)
                    {
                        IdentityProvider provider = service.GetIdentityProvider(authCredentials.ClientCredentials.UserName.UserName);
                    }

                    break;
            }

            return authCredentials;
        }
        /// <summary>
        /// Discovers the organizations that the calling user belongs to.
        /// </summary>
        /// <param name="service">A Discovery service proxy instance.</param>
        /// <returns>Array containing detailed information on each organization that 
        /// the user belongs to.</returns>
        public OrganizationDetailCollection DiscoverOrganizations(
            IDiscoveryService service)
        {
            if (service == null) throw new ArgumentNullException("service");
            RetrieveOrganizationsRequest orgRequest = new RetrieveOrganizationsRequest();
            RetrieveOrganizationsResponse orgResponse =
                (RetrieveOrganizationsResponse)service.Execute(orgRequest);

            return orgResponse.Details;
        }

        public OrganizationDetail FindOrganization(string orgUniqueName,
            OrganizationDetail[] orgDetails)
        {
            if (String.IsNullOrWhiteSpace(orgUniqueName))
                throw new ArgumentNullException("orgUniqueName");
            if (orgDetails == null)
                throw new ArgumentNullException("orgDetails");
            OrganizationDetail orgDetail = null;

            foreach (OrganizationDetail detail in orgDetails)
            {
                if (String.Compare(detail.UniqueName, orgUniqueName,
                    StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    orgDetail = detail;
                    break;
                }
            }
            return orgDetail;
        }

        private TProxy GetProxy<TService, TProxy>(
            IServiceManagement<TService> serviceManagement,
            AuthenticationCredentials authCredentials)
            where TService : class
            where TProxy : ServiceProxy<TService>
        {
            Type classType = typeof(TProxy);

            if (serviceManagement.AuthenticationType !=
                AuthenticationProviderType.ActiveDirectory)
            {
                AuthenticationCredentials tokenCredentials =
                    serviceManagement.Authenticate(authCredentials);
                // Obtain discovery/organization service proxy for Federated, LiveId and OnlineFederated environments. 
                // Instantiate a new class of type using the 2 parameter constructor of type IServiceManagement and SecurityTokenResponse.
                return (TProxy)classType
                    .GetConstructor(new Type[] { typeof(IServiceManagement<TService>), typeof(SecurityTokenResponse) })
                    .Invoke(new object[] { serviceManagement, tokenCredentials.SecurityTokenResponse });
            }

            // Obtain discovery/organization service proxy for ActiveDirectory environment.
            // Instantiate a new class of type using the 2 parameter constructor of type IServiceManagement and ClientCredentials.
            return (TProxy)classType
                .GetConstructor(new Type[] { typeof(IServiceManagement<TService>), typeof(ClientCredentials) })
                .Invoke(new object[] { serviceManagement, authCredentials.ClientCredentials });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                //dispose org service
                if (orgService != null ) ((IDisposable)orgService).Dispose();
            }

            disposed = true;
        }
    }
}