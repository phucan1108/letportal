#!/bin/bash
publishFolder=${1:-~/.letportal-pub/web-apis}
if [ -d $publishFolder ] 
then
 rm $publishFolder -rf
fi

mkdir $publishFolder
chmod 755 -R $publishFolder

dotnet build ./LetPortal.Saturn/LetPortal.Saturn.csproj -c Release -o $publishFolder"/LetPortal.Saturn"
chmod 755 $publishFolder"/LetPortal.Saturn/LetPortal.Saturn.dll"
echo ">>> Publish sucessfully! Go to execute run-lnx.sh to run all services"
