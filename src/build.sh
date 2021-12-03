#!/usr/bin/env bash
set -e
cd `dirname $0`

echo "WARNING: You need Visual Studio 2022 to perform a full build of this project" >&2

# Find dotnet
if command -v dotnet > /dev/null 2> /dev/null; then
    dotnet="dotnet"
else
    dotnet="../0install.sh run --version 6.0.. https://apps.0install.net/dotnet/sdk.xml"
fi

# Build (without WinForms)
$dotnet msbuild -v:Quiet -t:Restore -t:Build -p:Configuration=Release -p:Version=${1:-1.0.0-pre} Common
$dotnet msbuild -v:Quiet -t:Restore -t:Build -p:Configuration=Release -p:Version=${1:-1.0.0-pre} UnitTests
