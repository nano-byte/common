#!/bin/sh
set -e
cd `dirname $0`

# Find dotnet
if command -v dotnet > /dev/null 2> /dev/null; then
    dotnet="dotnet"
else
    dotnet="../0install.sh run --version 10.0..!10.1 https://apps.0install.net/dotnet/sdk.xml"
fi

echo "Publish Native AOT smoke test"
$dotnet publish AotSmokeTest --configuration Release --output ../artifacts/AotSmokeTest -p:Version=${1:-1.0.0-pre}

echo "Run Native AOT smoke test"
../artifacts/AotSmokeTest/NanoByte.Common.AotSmokeTest
