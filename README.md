NanoByte.Common
===============

NanoByte.Common provides utility classes, interfaces, controls, etc. with an emphasis on cross-platform development, integration with native features and down-level compatibility. It is available for .NET Framework 2.0 or newer and .NET Standard 2.0 or newer.

[![TeamCity Build status](https://0install.de/teamcity/app/rest/builds/buildType:(id:NanoByte_Common_Build)/statusIcon)](https://0install.de/teamcity/viewType.html?buildTypeId=NanoByte_Common_Build&guest=1)

NuGet packages:
- **[NanoByte.Common](https://www.nuget.org/packages/NanoByte.Common/)** (platform-agnostic base)
- **[NanoByte.Common.WinForms](https://www.nuget.org/packages/NanoByte.Common.WinForms/)** ([Windows Forms](https://docs.microsoft.com/en-us/dotnet/framework/winforms/)-specific features)
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
You need to install [Visual Studio 2017](https://www.visualstudio.com/downloads/) and [Zero Install](http://0install.de/downloads/) to perform a full build of this project.  
You can work on the cross-platform parts of the library using the [.NET Core SDK](https://www.microsoft.com/net/download) or [Mono](https://www.mono-project.com/download/stable/).

Run `.\build.ps1` on Windows or `./build.sh` on Linux to build and run unit tests.
