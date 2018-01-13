using System;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Management;
using System.Management.Automation;
 
namespace PowerShellModuleInCSharp.CSharpCmdlets {
	[Cmdlet(VerbsCommon.Add, "SudoRule")]
	public class AddSudoRuleCmdlet : Cmdlet {
		// .NET before ver. 6 does not allow defaults in auto-constructed setters, hence use private vars
		private string _sudoHost = "ALL";
		private string _sudoName = "bash";
		private string _sudoCommand = "/bin/bash";

 		[Parameter(
			Mandatory = true,
			HelpMessage = "Username or group name (precede the latter with %)"
		)]
		public string SudoUser { get; set; }

 		[Parameter(
			HelpMessage = "Allowed command (default: /bin/bash)"
		)]
		public string SudoCommand {
			set {_sudoCommand = value;}
		}

 		[Parameter(
			HelpMessage = "Allowed Host (default: ALL)"
		)]
		public string SudoHost {
			set {_sudoHost = value;}
		}

 		[Parameter(
			HelpMessage = "Name of the rule (default: <SudoUser>-bash)"
		)]
		public string SudoName {
			set {_sudoName = value;}
		}

		private static bool CreateSudoRule(
			DirectoryEntry ldapParent,
			string sudoName,
			string sudoUser,
			string sudoCommand,
			string sudoHost,
			ref string txt) {		

			// Remove a leading % from the username if present (designates a group)
			// when constructing the CN
			string cnUser = sudoUser;
			if (sudoUser.Substring(0, 1).Equals("%", StringComparison.Ordinal))
				cnUser = sudoUser.Substring(1, sudoUser.Length - 1);

			string cn = "CN=" + cnUser + "-" + sudoName;

			// Create rule and set attributes
			DirectoryEntry sudoRule;
			try	{
				sudoRule = ldapParent.Children.Add(cn, "sudoRole");

				sudoRule.Properties["sudoUser"].Add(sudoUser);
				sudoRule.Properties["sudoCommand"].Add(sudoCommand);
				sudoRule.Properties["sudoHost"].Add(sudoHost);

				sudoRule.CommitChanges();
			}
			catch (Exception e)	{
				txt = e.ToString();
				return false;
			}

			return true;
		}

		// Pre-processing
		protected override void BeginProcessing() {
			base.BeginProcessing();
		 }
 
		// Main processing
		protected override void ProcessRecord() {
			// Get our current domain name
			string domainName = Domain.GetComputerDomain().Name;
			string[] domainParts = domainName.Split(new [] {'.'});
			WriteVerbose("Using AD domain name " + domainName);

			// Compose LDAP URL from AD domain name
			string ldapPath = "LDAP://localhost:389/OU=sudoers";
			for (int i=0; i<domainParts.Length; i++) {
				ldapPath += ",DC=" + domainParts[i];
			}
			WriteVerbose("Bind OK to " + ldapPath);

			// Get AD LDS object
			DirectoryEntry ldapParent;
			try	{
				ldapParent = new DirectoryEntry(ldapPath);
				ldapParent.RefreshCache();
			}
			catch (Exception e)	{
				Console.WriteLine("Error: Bind failed.");
				Console.WriteLine("{0}", e.Message);
				return;
			}

			string txt = "";
			if (CreateSudoRule(ldapParent, _sudoName, SudoUser, _sudoCommand, _sudoHost, ref txt))
				 WriteVerbose("Success: sudo rule created");	
			else 
				Console.WriteLine("Error: {0}", txt);
		}

		// Post-processing
		protected override void EndProcessing() {
		}

		// Handle abnormal termination
		protected override void StopProcessing() {
		}
 	}
}

