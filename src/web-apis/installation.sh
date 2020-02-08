#!/bin/bash
if hash dotnet 2>/dev/null; then
 echo "DotNET SDK is already installed"
else
 echo "DotNET SDK isn't installed yet. We are installing DotNet SDK..."
 wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
 dpkg -i packages-microsoft-prod.deb
 add-apt-repository universe
 apt-get install apt-transport-https
 apt-get update
 apt-get install dotnet-sdk-${1:-2.2}
 echo "DotNET SDK 2.2 has been installed successfully!!!"
fi

echo "Publish LetPortal Tools to folder"${2:-~/.letportal}
# Remove folder tools if it exists
if [ -d ${2:-~/.letportal} ] 
then
 rm ${2:-~/.letportal} -rf
fi

mkdir ${2:-~/.letportal}
chmod 755 ${2:-~/.letportal}   

# Publish tools in this path
dotnet publish ./LetPortal.Tools/LetPortal.Tools.csproj -c Release -r linux-x64 -o ${2:-~/.letportal}
# Grant execute for all users
sudo chmod 755 ${2:-~/.letportal}/letportal
# Create PATH file for environment
if [ -d /etc/profile.d/letportal-tools.sh ] 
then
 rm /etc/profile.d/letportal-tools.sh -f
fi

touch /etc/profile.d/letportal-tools.sh 
echo 'export PATH=$PATH:'${2:-~/.letportal}''  >> /etc/profile.d/letportal-tools.sh
echo 'export PATH=$PATH:'${2:-~/.letportal}''  >> ~/.bashrc
source ~/.bashrc
echo 'export PATH=$PATH:'${2:-~/.letportal}''  >> ~/.profile
source ~/.profile
echo 'export PATH=$PATH:'${2:-~/.letportal}''  >> ~/.bash_profile
source ~/.bash_profile
export PATH=$PATH:${2:-~/.letportal}
echo "Publish tools completely!!!"
