Directory structure
===================

- The directory `src` contains the Visual Studio solution with the actual source code.
- The directory `lib` contains pre-compiled 3rd party libraries which are not available via NuGet.
- The directory `doc` contains scripts for generating source code and developer documentation.
- The directory `build` contains the results of various compilation processes. It is created on first usage.

`VERSION` contains the version numbers used by build scripts.
Keep in sync with the version numbers in `src/AssemblyInfo.Global.cs`!



Windows
=======

`build.cmd` will call build scripts in subdirectories to create a NuGet package in `build/Packages`.
You need to install [GTK# for Windows](http://download.xamarin.com/GTKforWindows/Windows/gtk-sharp-2.12.25.msi) to able to compile the code.

`cleanup.cmd` will delete any temporary files created by the build process or Visual Studio.



Linux
=====

`build.sh` will perform a partial debug compilation using Mono's xbuild. A NuGet package will not be built.

`cleanup.sh` will delete any temporary files created by the xbuild build process.

`test.sh` will run the unit tests using the NUnit console runner.
Note: You must perform a Debug build first (using `src/build.sh`) before you can run the unit tests.