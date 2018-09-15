# NanoByte.Common

[![NuGet](https://img.shields.io/nuget/v/NanoByte.Common.svg)](https://www.nuget.org/packages/NanoByte.Common/) [![API documentation](https://img.shields.io/badge/api-docs-orange.svg)](http://nano-byte.de/common/api/) [![Build status](https://img.shields.io/appveyor/ci/nano-byte/common.svg)](https://ci.appveyor.com/project/nano-byte/common)  
NanoByte.Common provides various utility classes and data structures with an emphasis on:

- integration with native Windows and Linux features,
- network and disk IO,
- advanced collections and
- undo/redo logic.

The library is available for .NET Framework 2.0+ and .NET Standard 2.0+.


## NanoByte.Common.WinForms

[![NuGet](https://img.shields.io/nuget/v/NanoByte.Common.WinForms.svg)](https://www.nuget.org/packages/NanoByte.Common.WinForms/)  
NanoByte.Common.WinForms builds upon NanoByte.Common and adds various Windows Forms controls with an emphasis on:

- progress reporting and
- data binding.

The library is available for .NET Framework 2.0+.

## Building

You need to install [Visual Studio 2017](https://www.visualstudio.com/downloads/) to perform a full build of this project.  
You can build the cross-platform components on Linux using only the [.NET Core SDK 2.1+](https://www.microsoft.com/net/download). Additionally installing [Mono 5.10+](https://www.mono-project.com/download/stable/) allows more components to be built.

Run `.\build.ps1` on Windows or `./build.sh` on Linux to build and run unit tests. These scripts takes a version number as an input argument. The source code itself contains no version numbers. Instead the version is picked by continuous integration using [GitVersion](http://gitversion.readthedocs.io/).
