#!/bin/bash
set -e
cd `dirname $0`

printf $1 > VERSION
sed -b -i "s/PROJECT_NUMBER = \".*\"/PROJECT_NUMBER = \"$1\"/" doc/Doxyfile
sed -b -i "s/<Version>.*<\/Version>/<Version>$1<\/Version>/" src/Common/Common.csproj
sed -b -i "s/<Version>.*<\/Version>/<Version>$1<\/Version>/" src/Common.WinForms/Common.WinForms.csproj
sed -b -i "s/<Version>.*<\/Version>/<Version>$1<\/Version>/" src/Common.SlimDX/Common.SlimDX.csproj
sed -b -i "s/<Version>.*<\/Version>/<Version>$1<\/Version>/" src/Common.Gtk/Common.Gtk.csproj
