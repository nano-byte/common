#!/bin/bash
set -e
cd `dirname $0`

rm -rf ../artifacts/Documentation
mkdir -p ../artifacts/Documentation

0install run http://repo.roscidus.com/devel/doxygen

cp CNAME ../artifacts/Documentation/
