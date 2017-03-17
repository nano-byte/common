@echo off
::Adds AuthentiCode signatures to all binaries. Assumes "build.cmd Release" has already been executed.

if not "%1" == "" set signing_cert_path=%*
set timestamp_server=http://timestamp.comodoca.com

rem Common locations for command-line tools
path %path%;%ProgramFiles(x86)%\Windows Kits\10\bin\x64;%ProgramFiles(x86)%\Windows Kits\8.1\bin\x64


echo Signing binaries with "%signing_cert_path%"...
FOR %%A IN ("%~dp0..\build\Release\NanoByte.Common*.dll") DO signtool sign /f "%signing_cert_path%" /fd sha256 /p "%signing_cert_pass%" /tr %timestamp_server% /td sha256 /q "%%A"
FOR %%A IN ("%~dp0..\build\ReleaseNet40\NanoByte.Common*.dll") DO signtool sign /f "%signing_cert_path%" /fd sha256 /p "%signing_cert_pass%" /tr %timestamp_server% /td sha256 /q "%%A"
FOR %%A IN ("%~dp0..\build\ReleaseNet35\NanoByte.Common*.dll") DO signtool sign /f "%signing_cert_path%" /fd sha256 /p "%signing_cert_pass%" /tr %timestamp_server% /td sha256 /q "%%A"
FOR %%A IN ("%~dp0..\build\ReleaseNet20\NanoByte.Common*.dll") DO signtool sign /f "%signing_cert_path%" /fd sha256 /p "%signing_cert_pass%" /tr %timestamp_server% /td sha256 /q "%%A"
if errorlevel 1 exit /b %errorlevel%
