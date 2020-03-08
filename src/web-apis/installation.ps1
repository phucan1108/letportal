<# This PS file will be using for installing LetPortal Tools
 Author: An Quang Phuc Le
 Created Date: Jan 28 2020
 Version: 0.0.5
#>

[CmdletBinding()]
Param
(
	[String] $sdkVersion = "3.1.1",
	[String] $sdkPath = "C:\NetCoreSDK",
	[String] $toolsPath = "C:\Tools"
)
# Step 1. Check DotNet Core has already installed
Try
{	
	Write-Host "1. Checking dotnet cli has been installed..."
	if(dotnet){
		Write-Host ">>> Dotnet CLI installed!!!"
	}
	else{
		Write-Host ">>> Dotnet CLI didn't install. We will try to install Dotnet CLI..."
		$downloadSDKPath = ($sdkPath + "\dotnet-install.ps1")
		Invoke-WebRequest -UseBasicParsing 'https://dot.net/v1/dotnet-install.ps1' -OutFile $downloadSDKPath
		powershell.exe -executionpolicy $downloadSDKPath -InstallDir $sdkPath -Version $sdkVersion
		Write-Host ">>> Dotnet CLI has been installed!!!"
	}	
}
Catch
{
    Write-Host "An error occurred:"
    Write-Host $_
}

# Step 2. Publish Tools
if((Test-Path $toolsPath) -eq 1){
    Remove-Item -LiteralPath $toolsPath –Force  -Recurse
}
Write-Host ("2. Publish Tools with path " + $toolsPath)
dotnet publish .\LetPortal.Tools\LetPortal.Tools.csproj -c Release -r win10-x64 -o $toolsPath
$allow = $env:Path.IndexOf(";" + $toolsPath) -lt 0 
if($allow -eq $true){
    [Environment]::SetEnvironmentVariable("Path", $env:Path + ";$toolsPath", "Machine")
}
Write-Host ">>> Tools has been published!!!"

