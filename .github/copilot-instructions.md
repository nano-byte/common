# NanoByte.Common - Copilot Coding Agent Instructions

## Repository Overview

**NanoByte.Common** is a .NET library providing utility classes and data structures with emphasis on native Windows/Linux integration, network/disk IO, advanced collections, and undo/redo logic. The repository produces three NuGet packages:
- **NanoByte.Common**: Core utilities (~440 C# files, ~8.2 MB)
- **NanoByte.Common.AnsiCli**: ANSI console output using Spectre.Console
- **NanoByte.Common.WinForms**: Windows Forms controls for progress reporting and data binding

**Technology Stack:**
- .NET multi-targeting: .NET Framework 2.0-4.8, .NET 8.0, .NET 9.0
- Build: MSBuild via .NET SDK 9.0.200+
- Testing: xUnit with FluentAssertions and Moq
- Documentation: DocFX 2.78.4
- Versioning: GitVersion 5.12.x (ContinuousDeployment mode)
- CI: GitHub Actions (Windows + Linux runners)

**Repository Size:** ~45 MB total, ~440 C# files

## Build Instructions

### Prerequisites
- **Linux/macOS**: .NET SDK 9.0.x or use bundled 0install scripts
- **Windows**: Visual Studio 2022 v17.13+ for full build (including WinForms), or .NET SDK 9.0.x for partial build

### Building on Linux/macOS

**ALWAYS use these commands in this exact order:**

```bash
./build.sh 1.0-dev
```

**What it does:**
1. Builds `Common` and `UnitTests` projects (excludes WinForms)
2. Runs unit tests for net9.0 target framework
3. Takes ~15-30 seconds for clean build
4. Outputs to `artifacts/Release/`

**Expected output:** "Passed! - Failed: 0, Passed: ~364+, Skipped: 17, Total: ~381+"

**Note:** You will see a warning "You need Visual Studio 2022 to perform a full build" - this is expected on Linux/macOS and can be ignored. The build will successfully compile the cross-platform components.

### Building on Windows

```powershell
.\build.ps1 1.0-dev
```

**What it does:**
1. Locates Visual Studio 2022 v17.13+ MSBuild (or falls back to dotnet msbuild)
2. Builds all projects including WinForms
3. Runs unit tests for net48 and net9.0 targets
4. Builds API documentation with DocFX
5. Takes ~45-90 seconds for clean build

### Running Tests Only

**Linux/macOS:**
```bash
cd src && ./test.sh
```

**Windows:**
```powershell
cd src
.\test.ps1
```

**Test behavior:**
- Tests use xUnit framework
- Some tests are skipped on non-Windows platforms (17 tests)
- Test results written to `src/UnitTests/TestResults/*.trx`
- Tests run with `--no-build` flag, so **always build before testing**

### Building Documentation (Windows Only)

```powershell
cd doc
.\build.ps1
```

Requires DocFX 2.78.4 (auto-restored via .config/dotnet-tools.json).

## Project Structure

### Directory Layout

```
/
├── .github/
│   └── workflows/
│       ├── build.yml          # CI build (Windows + Linux)
│       ├── translate.yml      # Translation download
│       └── translate-upload.yml
├── src/
│   ├── Common/                # Main library (net20-net9.0)
│   │   ├── Collections/       # Advanced collection types
│   │   ├── Native/            # Windows/Linux integration
│   │   ├── Net/               # Network utilities
│   │   ├── Storage/           # File system utilities
│   │   ├── Streams/           # Stream utilities
│   │   ├── Tasks/             # Task/progress utilities
│   │   ├── Threading/         # Threading utilities
│   │   ├── Undo/              # Undo/redo framework
│   │   └── Values/            # Value types and design
│   ├── Common.AnsiCli/        # ANSI CLI package
│   ├── Common.WinForms/       # Windows Forms package
│   ├── UnitTests/             # xUnit tests (net48, net9.0)
│   ├── Samples/               # Example applications
│   ├── Directory.Build.props  # Shared MSBuild properties
│   ├── build.ps1 / build.sh   # Build scripts
│   └── test.ps1 / test.sh     # Test scripts
├── doc/                       # DocFX documentation
├── lib/                       # 3rd party DLLs (TaskDialog)
├── artifacts/                 # Build output (created during build)
│   └── Release/
├── build.ps1 / build.sh       # Root build scripts
└── 0install.ps1 / 0install.sh # Zero Install bootstrapper
```

### Key Configuration Files

- **src/Directory.Build.props**: Shared build configuration for all projects
  - Language version: C# preview features
  - Nullable reference types: annotations mode (enable for .NET 8+)
  - Strong-name signing with `sgKey.snk`
  - Package generation on build
  - Warnings as errors
  
- **src/NanoByte.Common.slnx**: Visual Studio solution file (new XML format)
- **.editorconfig**: Code style rules (4 spaces, CRLF for C#/PS1, LF for others)
- **GitVersion.yml**: Version configuration (ContinuousDeployment mode)
- **.config/dotnet-tools.json**: dotnet tool manifest (DocFX 2.78.4)

### Multi-Targeting Details

The projects target multiple frameworks. **IMPORTANT:** On Linux, only a subset can be built:
- **Common**: net20, net40, net45, net462, net472, net8.0, net9.0 (Linux: net462, net472, net8.0, net9.0 only)
- **Common.AnsiCli**: net462, net472, net8.0, net9.0
- **Common.WinForms**: net20, net40, net45, net462, net472, net8.0-windows, net9.0-windows (Windows only)
- **UnitTests**: net48, net9.0 (Linux: net9.0 only)

## Continuous Integration

### GitHub Actions Workflows

**build.yml** (runs on push and PR):
- **Windows job**: Full build including WinForms, all test targets, documentation
- **Linux job**: Partial build (excludes WinForms), net9.0 tests only
- Uses GitVersion for semantic versioning
- Publishes NuGet packages to GitHub Packages (all branches except renovate/*) and NuGet.org (tags only)
- Creates GitHub releases for tags
- Publishes documentation to GitHub Pages (tags only)

**Test reporting**: Uses dorny/test-reporter@v2 to parse .trx files

### Version Numbers

- GitVersion generates version from git history
- Pre-release builds: `{version}-{shortSha}` (e.g., 1.2.3-abc1234)
- Release builds: `{version}` (e.g., 1.2.3)
- Pass version as first argument to build scripts

## Code Style and Conventions

### EditorConfig Rules

- **Indentation**: 4 spaces (2 for XML/JSON/YAML)
- **Line endings**: CRLF for C#/PowerShell/project files, LF for scripts/markdown
- **Charset**: UTF-8 (Latin-1 for .bat/.cmd)
- **Trim trailing whitespace**: Yes (except .md and .Designer.cs)
- **Insert final newline**: Yes

### C# Conventions

- Use C# preview language features (enabled in Directory.Build.props)
- Nullable reference types: annotations mode for older frameworks, enable for .NET 8+
- Global usings defined in Directory.Build.props: System.ComponentModel, System.Diagnostics.CodeAnalysis, JetBrains.Annotations, NanoByte.Common.Collections, NanoByte.Common.Values
- Strong-name sign all assemblies
- Generate XML documentation (warnings suppressed for missing comments: 1591)
- Use JetBrains.Annotations attributes
- Framework detection: Use conditional compilation for framework-specific code

### Testing Conventions

- Tests use xUnit, FluentAssertions, and Moq
- Global usings in tests: System.Net.Http, Xunit, FluentAssertions, Moq
- Tests may be skipped on non-Windows platforms (marked with [SkippableFact])
- Expected: ~364+ passed, 17 skipped on Linux; more passed on Windows

## Common Pitfalls and Solutions

### Build Issues

1. **"You need Visual Studio 2022" warning on Linux**
   - **Expected behavior** - ignore this warning
   - The build will succeed without VS on Linux (excluding WinForms)

2. **Test failures after code changes**
   - Ensure you rebuilt before running tests (tests use `--no-build`)
   - Run: `./build.sh 1.0-dev` then `cd src && ./test.sh`

3. **Missing packages**
   - Packages restore automatically during build
   - If issues occur, ensure .NET SDK 9.0.200+ is installed
   - Or use bundled scripts: `./0install.sh run --version 9.0.200.. https://apps.0install.net/dotnet/sdk.xml ...`

4. **MSBuild not found on Windows**
   - Install Visual Studio 2022 v17.13 or newer
   - Build scripts automatically detect and use VS MSBuild

### Resource Files

- Translation files: `src/Common/Properties/Resources.{lang}.resx`
- Managed via Transifex (config in `.tx/config`)
- Image resources: `src/Common.WinForms/Properties/ImageResources.resx`

### Platform-Specific Code

- Windows-only tests are skipped on Linux (17 tests)
- Native interop in `src/Common/Native/` has platform-specific implementations
- Use `RuntimeInformation.IsOSPlatform()` for runtime checks

## Validation Checklist

Before finalizing changes, **ALWAYS** run these steps:

1. **Build the code:**
   ```bash
   ./build.sh 1.0-dev  # On Linux/macOS
   # OR
   .\build.ps1 1.0-dev  # On Windows
   ```

2. **Verify test results:** Check that passed tests ≥ 360, failed = 0, skipped = 17 on Linux

3. **Check git status:**
   ```bash
   git status
   ```
   - Ensure only intended files are modified
   - `artifacts/` directory is in .gitignore (don't commit)

4. **Review code style:** Ensure changes follow EditorConfig rules

## Additional Notes

- **0install scripts**: Helper scripts that download .NET SDK on-demand if not installed
- **PackageValidation**: Enabled via `Microsoft.DotNet.PackageValidation` SDK
- **SourceLink**: Enabled for debugging NuGet packages
- **License**: MIT (see COPYING.txt)
- **Signing key**: `src/sgKey.snk` (strong-name signing)

## Trust These Instructions

**These build instructions have been validated and tested.** Only perform additional exploration or search if:
- These instructions are incomplete for your specific task
- You encounter an error not documented here
- You need to understand implementation details beyond build/test
