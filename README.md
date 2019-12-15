# NanoByte.Common

[![NuGet](https://img.shields.io/nuget/v/NanoByte.Common.svg)](https://www.nuget.org/packages/NanoByte.Common/)
[![API documentation](https://img.shields.io/badge/api-docs-orange.svg)](https://common.nano-byte.net/)
[![Build status](https://img.shields.io/appveyor/ci/nano-byte/common.svg)](https://ci.appveyor.com/project/nano-byte/common)  
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

The source code is in [`src/`](src/), a project for API documentation is in [`doc/`](doc/) and generated build artifacts are placed in `artifacts/`.

You need [Visual Studio 2019](https://www.visualstudio.com/downloads/) to perform a full build of this project.  
You can build the cross-platform components on Linux using just the [.NET Core SDK 3.1+](https://www.microsoft.com/net/download). Additionally installing [Mono 6.4+](https://www.mono-project.com/download/stable/) allows more components to be built. The build scripts will automatically adjust accordingly.

Run `.\build.ps1` on Windows or `./build.sh` on Linux. These scripts take a version number as an input argument. The source code itself contains only contains dummy version numbers. The actual version is picked by continuous integration using [GitVersion](http://gitversion.readthedocs.io/).

## Contributing

We welcome contributions to this project such as bug reports, recommendations, pull requests and [translations](https://www.transifex.com/eicher/0install-win/).

This repository contains an [EditorConfig](http://editorconfig.org/) file. Please make sure to use an editor that supports it to ensure consistent code style, file encoding, etc.. For full tooling support for all style and naming conventions consider using JetBrain's [ReSharper](https://www.jetbrains.com/resharper/) or [Rider](https://www.jetbrains.com/rider/) products.
