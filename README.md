# NanoByte.Common

[![Build](https://github.com/nano-byte/common/actions/workflows/build.yml/badge.svg)](https://github.com/nano-byte/common/actions/workflows/build.yml)
[![API documentation](https://img.shields.io/badge/api-docs-orange.svg)](https://common.nano-byte.net/)

[![NanoByte.Common](https://img.shields.io/nuget/v/NanoByte.Common.svg)](https://www.nuget.org/packages/NanoByte.Common/)  
**NanoByte.Common** provides various utility classes and data structures with an emphasis on:

- integration with native Windows and Linux features,
- network and disk IO,
- advanced collections and
- undo/redo logic.

[![NanoByte.Common.AnsiCli](https://img.shields.io/nuget/v/NanoByte.Common.AnsiCli.svg)](https://www.nuget.org/packages/NanoByte.Common.AnsiCli/)  
**NanoByte.Common.AnsiCli** adds ANSI console output. Powered by [Spectre.Console](https://github.com/spectresystems/spectre.console).

[![NanoByte.Common.WinForms](https://img.shields.io/nuget/v/NanoByte.Common.WinForms.svg)](https://www.nuget.org/packages/NanoByte.Common.WinForms/)  
**NanoByte.Common.WinForms** adds various Windows Forms controls with an emphasis on:

- progress reporting and
- data binding.

## Building

The source code is in [`src/`](src/), config for building the API documentation is in [`doc/`](doc/) and generated build artifacts are placed in `artifacts/`. The source code does not contain version numbers. Instead the version is determined during CI using [GitVersion](https://gitversion.net/).

To build on Windows install [Visual Studio 2022 v17.13 or newer](https://www.visualstudio.com/downloads/) and run `.\build.ps1`.  
To build on Linux or macOS run `./build.sh`. Note: Some parts of the code can only be built on Windows.

## Contributing

We welcome contributions to this project such as bug reports, recommendations, pull requests and [translations](https://www.transifex.com/eicher/0install-win/).

This repository contains an [EditorConfig](http://editorconfig.org/) file. Please make sure to use an editor that supports it to ensure consistent code style, file encoding, etc.. For full tooling support for all style and naming conventions consider using JetBrains' [ReSharper](https://www.jetbrains.com/resharper/) or [Rider](https://www.jetbrains.com/rider/) products.
