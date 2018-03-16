#!/bin/sh
set -e
cd `dirname $0`

dotnet test --no-build Common.UnitTests/Common.UnitTests.csproj
