NanoByte.Common
===============

NanoByte.Common provides utility classes, interfaces, controls, etc. with an emphasis on cross-platform development, OS integration and task progress UIs.

It consists of the NuGet packages `NanoByte.Common`, `NanoByte.Common.WinForms`, `NanoByte.Common.Gtk` and `NanoByte.Common.SlimDX`.

**[Website](http://nano-byte.de/common/)**

- The directory `src` contains the Visual Studio solution with the actual source code. You need [Visual Studio 2017](https://www.visualstudio.com/downloads/) to build it.
- The directory `lib` contains pre-compiled 3rd party libraries which are not available via NuGet.
- The directory `doc` contains a Doxyfile project for generating source code documentation.
- The directory `build` contains the results of various compilation processes. It is created on first usage.
  - `Debug`: Contains Debug builds in subdirectories targeting the .NET Framework 4.5, 4.0, 3.5 and 2.0 respectively.
  - `Release`: Contains Release builds in subdirectories targeting the .NET Framework 4.5, 4.0, 3.5 and 2.0 respectively.
  - `Documentation`: Contains the generated source code documentation.

You need [Visual Studio 2017](https://www.visualstudio.com/downloads/) to build the source.
