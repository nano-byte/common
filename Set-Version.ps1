#Sets a new version number in all relevant locations
$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent

if ($args.Length -lt 1) { Write-Error "Missing argument" }
$NewVersion = $args[0]

[System.IO.File]::WriteAllText("$ScriptDir\VERSION", $NewVersion)

(Get-Content "$ScriptDir\src\AssemblyInfo.Global.cs" -Encoding UTF8) `
  -replace 'AssemblyVersion\(".*"\)', ('AssemblyVersion("' + $NewVersion + '")') |
  Set-Content "$ScriptDir\src\AssemblyInfo.Global.cs" -Encoding UTF8

(Get-Content "$ScriptDir\doc\Doxyfile" -Encoding UTF8) `
  -replace 'PROJECT_NUMBER = ".*"', ('PROJECT_NUMBER = "' + $NewVersion + '"') |
  Set-Content "$ScriptDir\doc\Doxyfile" -Encoding UTF8
