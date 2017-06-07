$ErrorActionPreference = "Stop"
pushd $(Split-Path -Path $MyInvocation.MyCommand.Definition -Parent)

dotnet restore
. "$(. "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -property installationPath -format value)\Common7\IDE\devenv.com" NanoByte.Common.sln /Rebuild Release
dotnet test --configuration Release --no-build Common.UnitTests\Common.UnitTests.csproj
dotnet test --configuration Release --no-build Common.SlimDX.UnitTests\Common.SlimDX.UnitTests.csproj

popd
