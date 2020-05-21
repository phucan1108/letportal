<# This PS file will be using for publishing LetPortal
 Author: An Quang Phuc Le
 Created Date: Feb 17 2020
 Version: 0.0.5
#>

[CmdletBinding()]
Param
(
	[String] $publishFolder = "C:\.letportal-pub\web-apis"
)

if((Test-Path $publishFolder) -eq 1){
    Remove-Item -LiteralPath $publishFolder –Force  -Recurse
    New-Item -Path $publishFolder -Type Directory
}
else{
    New-Item -Path $publishFolder -Type Directory
}

dotnet build ./LetPortal.Gateway/LetPortal.Gateway.csproj -c Release -o $publishFolder"/LetPortal.Gateway" -r win10-x64
dotnet build ./LetPortal.IdentityApis/LetPortal.IdentityApis.csproj -c Release -o $publishFolder"/LetPortal.IdentityApis" -r win10-x64
dotnet build ./LetPortal.WebApis/LetPortal.PortalApis.csproj -c Release -o $publishFolder"/LetPortal.PortalApis" -r win10-x64
dotnet build ./LetPortal.ServiceManagementApis/LetPortal.ServiceManagementApis.csproj -c Release -o $publishFolder"/LetPortal.ServiceManagementApis" -r win10-x64
dotnet build ./LetPortal.ChatApis/LetPortal.ChatApis.csproj -c Release -o $publishFolder"/LetPortal.ChatApis" -r win10-x64