#!/bin/sh
#Runs the unit tests.

cd `dirname $0`/build/Debug
nunit-console NanoByte.Common.UnitTests.dll
nunit-console NanoByte.Common.SlimDX.UnitTests.dll
