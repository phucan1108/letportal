#!/bin/bash
publishFolder=${1:-~/.letportal-pub}
environment=${2:-Local}
# 1. Start Service Management
screen -dmS letportal-sm
screen -S letportal-sm -X stuff 'export ASPNETCORE_ENVIRONMENT='$environment'\n'
screen -S letportal-sm -X stuff 'cd '$publishFolder'/LetPortal.ServiceManagementApis\n'
screen -S letportal-sm -X stuff 'dotnet LetPortal.ServiceManagementApis.dll\n'

# 2. Start Portal APIs
screen -dmS letportal-pt
screen -S letportal-pt -X stuff 'export ASPNETCORE_ENVIRONMENT='$environment'\n'
screen -S letportal-pt -X stuff 'cd '$publishFolder'/LetPortal.PortalApis\n'
screen -S letportal-pt -X stuff 'dotnet LetPortal.PortalApis.dll\n'

# 3. Start Identity APIs
screen -dmS letportal-id
screen -S letportal-id -X stuff 'export ASPNETCORE_ENVIRONMENT='$environment'\n'
screen -S letportal-id -X stuff 'cd '$publishFolder'/LetPortal.IdentityApis\n'
screen -S letportal-id -X stuff 'dotnet LetPortal.IdentityApis.dll\n'

# 4. Start Gateway APIs
screen -dmS letportal-gw
screen -S letportal-gw -X stuff 'export ASPNETCORE_ENVIRONMENT='$environment'\n'
screen -S letportal-gw -X stuff 'cd '$publishFolder'/LetPortal.Gateway\n'
screen -S letportal-gw -X stuff 'dotnet LetPortal.Gateway.dll\n'
