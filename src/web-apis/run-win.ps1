<# This PS file will be using for running all LetPortal services
 Author: An Quang Phuc Le
 Created Date: Feb 17 2020
 Version: 0.0.5
#>

[CmdletBinding()]
Param
(
	[String] $publishFolder = "C:\.letportal-pub\web-apis",
    [String] $environment = "Local"
)

if((Test-Path $publishFolder) -eq 0){
    Write-Host "Publish folder isn't exist, please check again or running publish-win.ps1.'"
}

$Env:ASPNETCORE_ENVIRONMENT = $environment

Set-Location ($publishFolder + "\LetPortal.ServiceManagementApis")
Start-Process -FilePath "dotnet" -ArgumentList "LetPortal.ServiceManagementApis.dll" -Verb RunAs

Set-Location ($publishFolder + "\LetPortal.PortalApis")
Start-Process -FilePath "dotnet" -ArgumentList "LetPortal.PortalApis.dll" -Verb RunAs

Set-Location ($publishFolder + "\LetPortal.ChatApis")
Start-Process -FilePath "dotnet" -ArgumentList "LetPortal.ChatApis.dll" -Verb RunAs

Set-Location ($publishFolder + "\LetPortal.IdentityApis")
Start-Process -FilePath "dotnet" -ArgumentList "LetPortal.IdentityApis.dll" -Verb RunAs

Set-Location ($publishFolder + "\LetPortal.Gateway")
Start-Process -FilePath "dotnet" -ArgumentList "LetPortal.Gateway.dll" -Verb RunAs