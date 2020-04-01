#!/bin/bash
set -e
cd `dirname $0`

rm -rf ../artifacts/Documentation
mkdir -p ../artifacts/Documentation

0install run https://apps.0install.net/devel/doxygen.xml
