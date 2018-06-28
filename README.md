NanoByte.Common
===============

NanoByte.Common provides utility classes, interfaces, controls, etc. with an emphasis on cross-platform development, integration with native features and down-level compatibility. It is available for .NET Framework 2.0 or later and .NET Standard 2.0 or later.

NuGet packages (for .NET Framework 2.0+ and .NET Standard 2.0+):  
[![NanoByte.Common](https://img.shields.io/nuget/v/NanoByte.Common.svg?label=NanoByte.Common)](https://www.nuget.org/packages/NanoByte.Common/)
[![NanoByte.Common.WinForms](https://img.shields.io/nuget/v/NanoByte.Common.WinForms.svg?label=NanoByte.Common.WinForms)](https://www.nuget.org/packages/NanoByte.Common.WinForms/)

[![API documentation](https://img.shields.io/badge/api-docs-orange.svg)](http://nano-byte.de/common/api/)
[![Build status](https://img.shields.io/appveyor/ci/nano-byte/common.svg)](https://ci.appveyor.com/project/nano-byte/common)

Directory structure
-------------------
- `src` contains source code.
- `lib` contains pre-compiled 3rd party libraries which are not available via NuGet.
- `doc` contains a Doxyfile project for generation the API documentation.
- `artifacts` contains the results of various compilation processes. It is created on first usage.

Building
--------
You need to install [Visual Studio 2017](https://www.visualstudio.com/downloads/) to perform a full build of this project.  
You can build the cross-platform components on Linux using only the [.NET Core SDK 2.1+](https://www.microsoft.com/net/download). Additionally installing [Mono 5.10+](https://www.mono-project.com/download/stable/) allows more components to be built.

Run `.\build.ps1` on Windows or `./build.sh` on Linux to build and run unit tests. These scripts takes a version number as an input argument. The source code itself contains no version numbers. Instead the version is picked by continuous integration using [GitVersion](http://gitversion.readthedocs.io/).
