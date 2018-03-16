$ErrorActionPreference = "Stop"
pushd $(Split-Path -Path $MyInvocation.MyCommand.Definition -Parent)

dotnet test --no-build --configuration Release Common.UnitTests\Common.UnitTests.csproj
dotnet test --no-build --configuration Release Common.SlimDX.UnitTests\Common.SlimDX.UnitTests.csproj

popd
