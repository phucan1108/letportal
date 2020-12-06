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

dotnet build ./LetPortal.Saturn/LetPortal.Saturn.csproj -c Release -o $publishFolder"/LetPortal.Saturn" -r win10-x64