$ErrorActionPreference = "Stop"
pushd $(Split-Path -Path $MyInvocation.MyCommand.Definition -Parent)

dotnet test --no-build --configuration Release Common.UnitTests\Common.UnitTests.csproj

popd
