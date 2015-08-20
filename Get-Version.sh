#!/bin/sh
#Reads the current version number to the console and an environment variable
cd `dirname $0`

export VERSION=`cat VERSION`

echo \#\#teamcity[buildNumber \'$VERSION\_{build.number}\']
echo \#\#teamcity[setParameter name=\'build.version\' value=\'$VERSION\']
