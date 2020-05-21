#!/bin/bash
publishFolder=${1:-~/.letportal-pub/web-apis}
if [ -d $publishFolder ] 
then
 rm $publishFolder -rf
fi

mkdir $publishFolder
chmod 755 -R $publishFolder

dotnet build ./LetPortal.Gateway/LetPortal.Gateway.csproj -c Release -o $publishFolder"/LetPortal.Gateway"
chmod 755 $publishFolder"/LetPortal.Gateway/LetPortal.Gateway.dll"
dotnet build ./LetPortal.IdentityApis/LetPortal.IdentityApis.csproj -c Release -o $publishFolder"/LetPortal.IdentityApis"
chmod 755 $publishFolder"/LetPortal.IdentityApis/LetPortal.IdentityApis.dll"
dotnet build ./LetPortal.WebApis/LetPortal.PortalApis.csproj -c Release -o $publishFolder"/LetPortal.PortalApis"
chmod 755 $publishFolder"/LetPortal.PortalApis/LetPortal.PortalApis.dll"
dotnet build ./LetPortal.ChatApis/LetPortal.ChatApis.csproj -c Release -o $publishFolder"/LetPortal.ChatApis"
chmod 755 $publishFolder"/LetPortal.ChatApis/LetPortal.ChatApis.dll"
dotnet build ./LetPortal.ServiceManagementApis/LetPortal.ServiceManagementApis.csproj -c Release -o $publishFolder"/LetPortal.ServiceManagementApis"
chmod 755 $publishFolder"/LetPortal.ServiceManagementApis/LetPortal.ServiceManagementApis.dll"
echo ">>> Publish sucessfully! Go to execute run-lnx.sh to run all services"
