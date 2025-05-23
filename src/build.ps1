﻿Param ($Version = "1.0-dev")
$ErrorActionPreference = "Stop"
pushd $PSScriptRoot

function Find-MSBuild {
    if (Test-Path "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe") {
        $vsDir = . "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -products * -property installationPath -format value -version 17.13
        if ($vsDir) { return "$vsDir\MSBuild\Current\Bin\amd64\MSBuild.exe" }
    }
}

function Run-DotNet {
    ..\0install.ps1 run --batch --version 9.0.200.. https://apps.0install.net/dotnet/sdk.xml @args
    if ($LASTEXITCODE -ne 0) {throw "Exit Code: $LASTEXITCODE"}
}

function Run-MSBuild {
    $msbuild = Find-MSBuild
    if ($msbuild) {
        . $msbuild @args
        if ($LASTEXITCODE -ne 0) {throw "Exit Code: $LASTEXITCODE"}
    } else {
        Write-Warning "You need Visual Studio 2022 v17.13 or newer to perform a full build of this project"
        Run-DotNet msbuild @args
    }
}

# Build
if ($env:CI) { $ci = "/p:ContinuousIntegrationBuild=True" }
Run-MSBuild /v:Quiet /t:Restore /t:Build /p:Configuration=Release /p:Version=$Version $ci

popd
