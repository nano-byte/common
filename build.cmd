@echo off
::Compiles the source code and then creates NuGet packages.

echo.
call "%~dp0src\build.cmd" Release
if errorlevel 1 pause
call "%~dp0src\build.cmd" ReleaseNet40
if errorlevel 1 pause
call "%~dp0src\build.cmd" ReleaseNet35
if errorlevel 1 pause
call "%~dp0src\build.cmd" ReleaseNet20
if errorlevel 1 pause

if defined signing_cert_path (
echo.
call "%~dp0src\sign.cmd"
if errorlevel 1 pause
)

echo.
call "%~dp0nuget\build.cmd" %*
if errorlevel 1 pause

echo.
call "%~dp0doc\build.cmd"
if errorlevel 1 pause