<# This PS file will be using for running all LetPortal services
 Author: An Quang Phuc Le
 Created Date: Feb 17 2020
 Version: 0.9.0
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

Set-Location ($publishFolder + "\LetPortal.Saturn")
Start-Process -FilePath "dotnet" -ArgumentList "LetPortal.Saturn.dll" -Verb RunAs