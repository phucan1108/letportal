#!/bin/bash
publishFolder=${1:-~/.letportal-pub/web-apis}
environment=${2:-Local}
# 1. Start Saturn
screen -dmS letportal-sm
screen -S letportal-sm -X stuff 'export ASPNETCORE_ENVIRONMENT='$environment'\n'
screen -S letportal-sm -X stuff 'cd '$publishFolder'/LetPortal.Saturn\n'
screen -S letportal-sm -X stuff 'dotnet LetPortal.Saturn.dll\n'
