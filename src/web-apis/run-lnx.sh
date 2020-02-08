#!/bin/bash
publishFolder=${1:-~/.letportal-pub}
# 1. Start Service Management
screen -dmS letportal-sm
screen -S letportal-sm -p 0 -X stuff 'cd '$publishFolder'/LetPortal.ServiceManagementApis\n'
screen -S letportal-sm -p 0 -X stuff './LetPortal.ServiceManagementApis\n'

# 2. Start Portal APIs
screen -dmS letportal-pt
screen -S letportal-pt -p 0 -X stuff 'cd '$publishFolder'/LetPortal.PortalApis\n'
screen -S letportal-pt -p 0 -X stuff './LetPortal.PortalApis\n'

# 3. Start Identity APIs
screen -dmS letportal-id
screen -S letportal-id -p 0 -X stuff 'cd '$publishFolder'/LetPortal.IdentityApis\n'
screen -S letportal-id -p 0 -X stuff './LetPortal.IdentityApis\n'

# 4. Start Gateway APIs
screen -dmS letportal-gw
screen -S letportal-gw -p 0 -X stuff 'cd '$publishFolder'/LetPortal.Gateway\n'
screen -S letportal-gw -p 0 -X stuff './LetPortal.Gateway\n'
