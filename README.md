NanoByte.Common
===============

NanoByte.Common provides utility classes, interfaces, controls, etc. with an emphasis on cross-platform development, OS integration and task progress UIs.

[![TeamCity Build status](https://0install.de/teamcity/app/rest/builds/buildType:(id:NanoByte_Common_Build)/statusIcon)](https://0install.de/teamcity/viewType.html?buildTypeId=NanoByte_Common_Build&guest=1)

NuGet packages:
- **[NanoByte.Common](https://www.nuget.org/packages/NanoByte.Common/)** (platform-agnostic base)
- **[NanoByte.Common.WinForms](https://www.nuget.org/packages/NanoByte.Common.WinForms/)** ([Windows Forms](https://docs.microsoft.com/en-us/dotnet/framework/winforms/)-specific features)
- **[NanoByte.Common.Gtk](https://www.nuget.org/packages/NanoByte.Common.Gtk/)** ([GTK#](http://www.mono-project.com/GtkSharp/)-specific features)
- **[NanoByte.Common.SlimDX](https://www.nuget.org/packages/NanoByte.Common.SlimDX/)** ([SlimDX](http://slimdx.org/)-specific features)

**[API documentation](http://nano-byte.de/common/api/)**

Directory structure
-------------------
- `src` contains source code.
- `lib` contains pre-compiled 3rd party libraries which are not available via NuGet.
- `doc` contains a Doxyfile project for generation the API documentation.
- `build` contains the results of various compilation processes. It is created on first usage.

Building
--------
- You need to install [Visual Studio 2017](https://www.visualstudio.com/downloads/) and [Zero Install](http://0install.de/downloads/) to build this project.
- The file `VERSION` contains the current version number of the project.
- Run `.\Set-Version.ps1 "X.Y.Z"` in PowerShall to change the version number. This ensures that the version also gets set in other locations (e.g. `.csproj` files).
- Run `.\build.ps1` in PowerShell to build everything.
