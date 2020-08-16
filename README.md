# NanoByte.Common

[![Build](https://github.com/nano-byte/common/workflows/Build/badge.svg?branch=master)](https://github.com/nano-byte/common/actions?query=workflow%3ABuild)
[![API documentation](https://img.shields.io/badge/api-docs-orange.svg)](https://common.nano-byte.net/)

[![NanoByte.Common](https://img.shields.io/nuget/v/NanoByte.Common.svg)](https://www.nuget.org/packages/NanoByte.Common/)  
NanoByte.Common provides various utility classes and data structures with an emphasis on:

- integration with native Windows and Linux features,
- network and disk IO,
- advanced collections and
- undo/redo logic.

[![NanoByte.Common.WinForms](https://img.shields.io/nuget/v/NanoByte.Common.WinForms.svg)](https://www.nuget.org/packages/NanoByte.Common.WinForms/)  
NanoByte.Common.WinForms builds upon NanoByte.Common and adds various Windows Forms controls with an emphasis on:

- progress reporting and
- data binding.

## Building

The source code is in [`src/`](src/), config for building the API documentation is in [`doc/`](doc/) and generated build artifacts are placed in `artifacts/`. The source code does not contain version numbers. Instead the version is determined during CI using [GitVersion](http://gitversion.readthedocs.io/).

To build on Windows install [Visual Studio 2019 v16.5 or newer](https://www.visualstudio.com/downloads/) and run `.\build.ps1`.  
To build on Linux or MacOS X run `./build.sh`. Note: Some parts of the code can only be built on Windows.

## Contributing

We welcome contributions to this project such as bug reports, recommendations, pull requests and [translations](https://www.transifex.com/eicher/0install-win/).

This repository contains an [EditorConfig](http://editorconfig.org/) file. Please make sure to use an editor that supports it to ensure consistent code style, file encoding, etc.. For full tooling support for all style and naming conventions consider using JetBrain's [ReSharper](https://www.jetbrains.com/resharper/) or [Rider](https://www.jetbrains.com/rider/) products.
