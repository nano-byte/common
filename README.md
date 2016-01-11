NanoByte.Common
===============
Utility classes, interfaces, controls, etc. with an emphasis on cross-platform development, OS integration and task progress UIs.

**[Website / Documentation](http://nano-byte.de/common/)**


Source directory structure
--------------------------
- The directory `src` contains the Visual Studio solution with the actual source code.
- The directory `lib` contains pre-compiled 3rd party libraries which are not available via NuGet.
- The directory `doc` contains scripts for generating source code documentation.
- The directory `build` contains the results of various compilation processes. It is created on first usage.

`VERSION` contains the version numbers used by build scripts.
Use `.\Set-Version.ps1 "X.Y.Z"` in PowerShall to change the version number. This ensures that the version also gets set in other locations (e.g. `AssemblyInfo`).


Building on Windows
-------------------
`build.cmd` will call build scripts in subdirectories to create a NuGet package in `build/Packages`.
You need to install [GTK# for Windows](http://download.xamarin.com/GTKforWindows/Windows/gtk-sharp-2.12.25.msi) to able to compile the code.

If you wish to add an AuthentiCode signature to the compiled binaries set the `signing_cert_path` environment variable to the certificate's file path and `signing_cert_pass` to the password used to decrypt the file before executing the build scripts.
For example:
```
set signing_cert_path=C:\mycert.pfx
set signing_cert_pass=mypass
build.cmd
```

`cleanup.cmd` will delete any temporary files created by the build process or Visual Studio.


Building on Linux
-----------------
`build.sh` will perform a partial debug compilation using Mono's xbuild. A NuGet package will not be built.

`cleanup.sh` will delete any temporary files created by the xbuild build process.

`test.sh` will run the unit tests using the NUnit console runner.
Note: You must perform a Debug build first (using `src/build.sh`) before you can run the unit tests.
