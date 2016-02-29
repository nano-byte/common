Param ([Parameter(Mandatory=$True)] [string]$NewVersion)
#Sets a new version number in all relevant locations
$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent

[System.IO.File]::WriteAllText("$ScriptDir\VERSION", $NewVersion)

(Get-Content "$ScriptDir\src\GlobalAssemblyInfo.cs" -Encoding UTF8) `
  -replace 'AssemblyVersion\(".*"\)', ('AssemblyVersion("' + $NewVersion + '")') |
  Set-Content "$ScriptDir\src\GlobalAssemblyInfo.cs" -Encoding UTF8

(Get-Content "$ScriptDir\doc\Doxyfile" -Encoding UTF8) `
  -replace 'PROJECT_NUMBER = ".*"', ('PROJECT_NUMBER = "' + $NewVersion + '"') |
  Set-Content "$ScriptDir\doc\Doxyfile" -Encoding UTF8
