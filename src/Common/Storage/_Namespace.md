---
uid: NanoByte.Common.Storage
summary: File system access and serialization.
---
The following types help with bulk operations and are also usable as <xref:NanoByte.Common.Tasks.ITask>s for progress reporting:

- <xref:NanoByte.Common.Storage.CopyDirectory>
- <xref:NanoByte.Common.Storage.MoveDirectory>
- <xref:NanoByte.Common.Storage.ReadDirectoryBase>
- <xref:NanoByte.Common.Storage.ReadFile>

## Atomic writes

To avoid leaving partially-written files behind on crashes or process kills, write to a temporary path first and only replace the target on success:

- <xref:NanoByte.Common.Storage.AtomicWrite> writes to a temporary file and atomically moves it into place on `Dispose()`.
- <xref:NanoByte.Common.Storage.AtomicRead> takes a matching read lock.

## Temporary paths

The following types create unique paths on construction and delete them on `Dispose()`:

- <xref:NanoByte.Common.Storage.TemporaryFile>
- <xref:NanoByte.Common.Storage.TemporaryDirectory>
- <xref:NanoByte.Common.Storage.TemporaryWorkingDirectory> additionally changes the current working directory while in scope.
- <xref:NanoByte.Common.Storage.TemporaryFlagFile> represents the existence of a file as a `bool`.

## Locations

<xref:NanoByte.Common.Storage.Locations> resolves cross-platform paths for application data, caches and configuration following the [XDG Base Directory Specification](https://specifications.freedesktop.org/basedir-spec/basedir-spec-latest.html) on Unix and the matching Windows conventions.

## Serialization

The following static classes provide shorthand load/save methods for the common serialization formats. They use atomic writes by default.

- <xref:NanoByte.Common.Storage.XmlStorage> for XML
- <xref:NanoByte.Common.Storage.JsonStorage> for JSON
- <xref:NanoByte.Common.Storage.BinaryStorage> for .NET Framework `BinaryFormatter`

## API
