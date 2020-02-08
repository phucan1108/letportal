#!/bin/bash
publishFolder=${1:-~/.letportal-pub}
if [ -d $publishFolder ] 
then
 rm $publishFolder -rf
fi

mkdir $publishFolder
chmod 755 $publishFolder

dotnet publish ./LetPortal.Gateway/LetPortal.Gateway.csproj -o $publishFolder"/LetPortal.Gateway"
dotnet publish ./LetPortal.IdentityApis/LetPortal.IdentityApis.csproj -o $publishFolder"/LetPortal.IdentityApis"
dotnet publish ./LetPortal.WebApis/LetPortal.PortalApis.csproj -o $publishFolder"/LetPortal.PortalApis"
dotnet publish ./LetPortal.ServiceManagementApis/LetPortal.ServiceManagementApis.csproj -o $publishFolder"/LetPortal.ServiceManagementApis"
echo ">>> Publish sucessfully! Go to execute run-lnx.sh to run all services"
