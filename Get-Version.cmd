@echo off
::Reads the current version number to the console and an environment variable

set /p version= < "%~dp0VERSION"

echo ##teamcity[buildNumber '%version%_{build.number}']
echo ##teamcity[setParameter name='build.version' value='%version%']
