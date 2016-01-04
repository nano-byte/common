@echo off
::Compiles the source documentation.
pushd "%~dp0"

echo Building source documentation...
if exist ..\build\Documentation rd /s /q ..\build\Documentation
mkdir ..\build\Documentation
0install run http://0install.de/feeds/Doxygen.xml
if errorlevel 1 exit /b %errorlevel%

popd