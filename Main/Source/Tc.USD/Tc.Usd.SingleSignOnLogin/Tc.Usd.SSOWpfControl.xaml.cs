using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.Utilities;
using Microsoft.Uii.Desktop.SessionManager;
using Tc.Usd.SingleSignOnLogin.Models;
using Tc.Usd.SingleSignOnLogin.Services;
using Tc.Crm.Common.Constants.EntityRecords;
using Tc.Crm.Common.Constants;
using Attributes = Tc.Crm.Common.Constants.Attributes;

namespace Tc.Usd.SingleSignOnLogin
{
    public partial class USDControl
    {
        #region Vars
        /// <summary>
        /// Log writer for USD 
        /// </summary>
        private readonly TraceLogger logWriter;

        private BudgetCentreService budgetCentreService;

        #endregion

        /// <summary>
        /// UII Constructor 
        /// </summary>
        /// <param name="appId">ID of the application</param>
        /// <param name="appName">Name of the application</param>
        /// <param name="initString">Initializing XML for the application</param>
        public USDControl(Guid appId, string appName, string initString)
            : base(appId, appName, initString)
        {
            InitializeComponent();

            // This will create a log writer with the default provider for Unified Service desk
            logWriter = new TraceLogger();
            this.StoreSelector.Style = null;
        }

        /// <summary>
        /// Raised when the Desktop Ready event is fired. 
        /// </summary>
        protected override void DesktopReady()
        {
            budgetCentreService = new BudgetCentreService(_client.CrmInterface);
            var parameters = ((DynamicsCustomerRecord)((AgentDesktopSession)localSessionManager.GlobalSession).Customer.DesktopCustomer).CapturedReplacementVariables;
            var loadAllBudgetCenters = parameters.ContainsKey("$User")&& parameters["$User"].ContainsKey(Attributes.User.AllBudgetCentreAccess) &&
                bool.Parse(parameters["$User"][Attributes.User.AllBudgetCentreAccess].value);
            this.StoreSelector.ItemsSource = budgetCentreService.GetBudgetCentre(loadAllBudgetCenters);
            
            // this will populate any toolbars assigned to this control in config. 
            PopulateToolbars(ProgrammableToolbarTray);
            base.DesktopReady();
        }

        /// <summary>
        /// Raised when an action is sent to this control
        /// </summary>
        /// <param name="args">args for the action</param>
        protected override void DoAction(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            // Log process.
            logWriter.Log(string.Format(CultureInfo.CurrentCulture, "{0} -- DoAction called for action: {1}", this.ApplicationName, args.Action), System.Diagnostics.TraceEventType.Information);

            base.DoAction(args);
        }


        private void StoreSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ToggleLoginButton();
        }

        private void StoreSelector_KeyUp(object sender, KeyEventArgs e)
        {
            var storeSelector = (ComboBox)sender;

            if (string.IsNullOrEmpty(storeSelector.Text))
            {
                storeSelector.IsDropDownOpen = false;
                return;
            }
            var searchText = storeSelector.Text.ToLowerInvariant();
            var itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(storeSelector.ItemsSource);

            itemsViewOriginal.Filter = (o) => StoreSearchFilter(o, searchText);

            itemsViewOriginal.Refresh();
            storeSelector.IsDropDownOpen = true;
        }

        private bool StoreSearchFilter(object o, string searchString)
        {
            return ((BudgetCentre)o).SearchString.ToLowerInvariant().ContainsAll(searchString.Split(' '));
        }

        private void ToggleLoginButton()
        {
            this.LoginButton.IsEnabled = !(this.UserInitials.Text.Length == 0 || this.StoreSelector.SelectedIndex == -1);
        }

        private void UserInitials_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ToggleLoginButton();
        }

        private void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            UpdateContextValues();
            FireEvent(Configuration.SsoLogin);
        }

        private void UpdateContextValues()
        {
            var parameters = ((DynamicsCustomerRecord)((AgentDesktopSession)localSessionManager.GlobalSession).Customer.DesktopCustomer).CapturedReplacementVariables;
            if (!parameters.ContainsKey("$User") && parameters["$User"].ContainsKey(Attributes.User.PayrollNumber)) return;

            var initials = UserInitials.Text;
            var budgetCentreId = ((BudgetCentre)StoreSelector.SelectedItem).StoreId;
            var abta = ((BudgetCentre)StoreSelector.SelectedItem).Abta;
            var branchcode = ((BudgetCentre) StoreSelector.SelectedItem).BudgetCentreName;

            var employeeId = parameters["$User"][Attributes.User.PayrollNumber].value;
            var name = parameters["$User"][Attributes.User.FullName].value;

            var extLoginId = budgetCentreService.UpsertExternalLogin(initials, abta, budgetCentreId, branchcode, employeeId, name);

            var externalInfo =
                ((DynamicsCustomerRecord)
                    ((AgentDesktopSession) localSessionManager.GlobalSession).Customer.DesktopCustomer).Entities.Find(
                        e => e.LogicalName == EntityName.ExternalLogin);
            if (externalInfo != null)
            {
                externalInfo.data[Attributes.ExternalLogin.Id] = new CRMApplicationData { value = extLoginId.ToString(), type = "String" };
                externalInfo.data[Attributes.ExternalLogin.AbtaNumber] = new CRMApplicationData { value = abta, type = "String"};
                externalInfo.data[Attributes.ExternalLogin.BudgetCentreId] = new CRMApplicationData { value = budgetCentreId.ToString(), type = "String" };
                externalInfo.data[Attributes.ExternalLogin.Initials] = new CRMApplicationData { value = initials, type = "String" };
                externalInfo.data[Attributes.ExternalLogin.Name] = new CRMApplicationData { value = name, type = "String" };
                externalInfo.data[Attributes.ExternalLogin.EmployeeId] = new CRMApplicationData { value = employeeId, type = "String" };
                externalInfo.data["tc_externalloginid"] = new CRMApplicationData { value = extLoginId.ToString(), type = "Guid" };
            }
            //var extLoginDataToSet = new Dictionary<string, CRMApplicationData>
            //{
            //    {"tc_abtanumber", new CRMApplicationData() {value = abta, type = "String"}},
            //    {"tc_initials", new CRMApplicationData() {value = initials, type = "String"}},
            //    {"Id", new CRMApplicationData() {value = extLoginId.ToString(), type = "String"}},
            //    {"tc_externalloginid", new CRMApplicationData() {value = extLoginId.ToString(), type = "Guid"}}
            //};
            //((DynamicsCustomerRecord)((AgentDesktopSession)localSessionManager.GlobalSession).Customer.DesktopCustomer).MergeReplacementParameter(this.ApplicationName, extLoginDataToSet, true);
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.UserInitials.Text = string.Empty;
            this.StoreSelector.SelectedItem = null;
            this.LoginButton.IsEnabled = false;
            ((CollectionView)CollectionViewSource.GetDefaultView(this.StoreSelector.ItemsSource)).Filter = null;
            FireEvent(Configuration.SsoCancel);
        }
    }
}