@echo off
::Compiles the source code and then creates NuGet packages.
::Use command-line argument "+doc" to additionally create source code documentation.
if "%1"=="+doc" set BUILD_DOC=TRUE
if "%2"=="+doc" set BUILD_DOC=TRUE
if "%3"=="+doc" set BUILD_DOC=TRUE
if "%4"=="+doc" set BUILD_DOC=TRUE

echo.
call "%~dp0src\build.cmd" Release
if errorlevel 1 pause

if defined signing_cert_path (
echo.
call "%~dp0src\sign.cmd"
if errorlevel 1 pause
)

echo.
call "%~dp0nuget\build.cmd" %*
if errorlevel 1 pause

::Optionally create debug build and documentation
if "%BUILD_DOC%"=="TRUE" (
  echo.
  call "%~dp0src\build.cmd" Debug
  if errorlevel 1 pause

  echo.
  call "%~dp0doc\build.cmd"
  if errorlevel 1 pause
)