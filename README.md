# powershell-sudorule
PowerShell cmdlet for adding sudoRule entries to Windows AD

This is a small cmdlet to add a sudoRole entry to an existing OU=sudoers in Windows Active Directory. If you have extended your Windows AD schema to implement sudo ACL, use this cmdlet to easily add records from command line. 

# Usage

    Add-SudoRule -SudoUser [-SudoHost] [-SudoCommand] [-SudoName] [-Verbose]

Parameters:

* SudoUser: the username or group name (group names must begin wit the percent sign `%`).

* SudoHost: the host to which the ACL entry applies (default: `ALL`)

* SudoCommand: the commands to execute with elevated privileges (default: `/bin/bash`)

* SudoName: the name of the AD object (default: `<SudoUser>-bash`; the precent sign from grop name will be stripped here)

* Verbose: receive verbose output (default: on success no output is given)

# Build

You can easily build and install this cmdlet without the need for Visual Studio, NuGet or any other complex tool. You only need .NET (version as old as 3.5 will do it) and to download a library package from Microsoft's website. 

## Requriements

* Check the version of PowerShell: `$PSVersionTable.PSVersion`

* Get `System.Management.Automation.dll` which matches the PowerShell version. Official packages are here: (replace `4` with your PowerShell version, e.g. `3` or `5`): [https://az320820.vo.msecnd.net/packages/microsoft.powershell.4.referenceassemblies.1.0.0.nupkg]

* To extract the package, rename it to `.zip`, then unpack wherever you like. 

## Compilation
Check the version of .NET your PowerShell uses: `$PSVersionTable.CLRVersion`.

To compile, use `csc.exe` from the .NET version which PowerShell uses. The following example is for .NET 4.0 build 30319: 

    C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /t:library /reference:c:\path\to\extracted\dll\System.Management.Automation.dll /out:AddSudoRuleCmdlet.dll AddSudoRuleCmdlet.cs

## Usage
To import the DLL you have just created: `Import-Module AddSudoRuleCmdlet.dll`

## Package
You can use the provided NSIS script to build a GUI instaler with your DLL. NSIS is available for Windows and Linux, see [http://nsis.sourceforge.net] (on Linux, be sure to check with your distro first):

* Place the built DLL inside the `AddSudoRuleCmdlet` directory where the manifest `AddSudoRuleCmdlet.psd1` is.

* From the top level directoy of the repo, call `makensis AddSudoRuleCmdlet.nsis`.

# Bugs & Wishes

Report busg & wishes here or conatct the author via email.

# Author

Assen Totin, assen.totin@gmail.com

