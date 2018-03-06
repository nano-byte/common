$ErrorActionPreference = "Stop"
pushd $(Split-Path -Path $MyInvocation.MyCommand.Definition -Parent)

dotnet test --configuration Release --no-build Common.UnitTests\Common.UnitTests.csproj
dotnet test --configuration Release --no-build Common.SlimDX.UnitTests\Common.SlimDX.UnitTests.csproj

popd
