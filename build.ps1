$ErrorActionPreference = "Stop"
pushd $(Split-Path -Path $MyInvocation.MyCommand.Definition -Parent)

src\build.ps1
src\test.ps1
doc\build.ps1

popd
