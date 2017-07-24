Constructor
			// This will create a log writer with the same name as your hosted control. 
            // LogWriter = new TraceLogger(traceSourceName:"MyTraceSource");

            // If you utilize this feature,  you would need to add a section to the system.diagnostics settings area of the UnifiedServiceDesk.exe.config
            //<source name="MyTraceSource" switchName="MyTraceSwitchName" switchType="System.Diagnostics.SourceSwitch">
            //    <listeners>
            //        <add name="console" type="System.Diagnostics.DefaultTraceListener"/>
            //        <add name="fileListener"/>
            //        <add name="USDDebugListener" />
            //        <remove name="Default"/>
            //    </listeners>
            //</source>

            // and then in the switches area : 
            //<add name="MyTraceSwitchName" value="Verbose"/>
DoAction
			//// Process Actions. 
            //if (args.Action.Equals("your action name", StringComparison.OrdinalIgnoreCase))
            //{
            //    // Do some work

            //    // Access CRM and fetch a Record
            //    Microsoft.Xrm.Sdk.Messages.RetrieveRequest req = new Microsoft.Xrm.Sdk.Messages.RetrieveRequest(); 
            //    req.Target = new Microsoft.Xrm.Sdk.EntityReference( "account" , Guid.Parse("0EF05F4F-0D39-4219-A3F5-07A0A5E46FD5")); 
            //    req.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("accountid" , "name" );
            //    Microsoft.Xrm.Sdk.Messages.RetrieveResponse response = (Microsoft.Xrm.Sdk.Messages.RetrieveResponse)this._client.CrmInterface.ExecuteCrmOrganizationRequest(req, "Requesting Account"); 


            //    // Example of pulling some data out of the passed in data array
            //    List<KeyValuePair<string, string>> actionDataList = Utility.SplitLines(args.Data, CurrentContext, localSession);
            //    string valueIwant = Utility.GetAndRemoveParameter(actionDataList, "mykey"); // asume there is a myKey=<value> in the data. 



            //    // Example of pushing data to USD
            //    string global = Utility.GetAndRemoveParameter(actionDataList, "global"); // Assume there is a global=true/false in the data
            //    bool saveInGlobalSession = false;
            //    if (!String.IsNullOrEmpty(global))
            //        saveInGlobalSession = bool.Parse(global);

            //    Dictionary<string, CRMApplicationData> myDataToSet = new Dictionary<string, CRMApplicationData>();
            //    // add a string: 
            //    myDataToSet.Add("myNewKey", new CRMApplicationData() { name = "myNewKey", type = "string", value = "TEST" });

            //    // add a entity lookup:
            //    myDataToSet.Add("myNewKey", new CRMApplicationData() { name = "myAccount", type = "lookup", value = "account,0EF05F4F-0D39-4219-A3F5-07A0A5E46FD5,MyAccount" }); 

            //    if (saveInGlobalSession) 
            //    {
            //        // add context item to the global session
            //        ((DynamicsCustomerRecord)((AgentDesktopSession)localSessionManager.GlobalSession).Customer.DesktopCustomer).MergeReplacementParameter(this.ApplicationName, myDataToSet, true);
            //    }
            //    else
            //    {
            //        // Add context item to the current session. 
            //        ((DynamicsCustomerRecord)((AgentDesktopSession)localSessionManager.ActiveSession).Customer.DesktopCustomer).MergeReplacementParameter(this.ApplicationName, myDataToSet, true);
            //    }
            //}