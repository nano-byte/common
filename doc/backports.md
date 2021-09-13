# Backports

Newer versions of .NET often introduce features that can be backported to earlier versions. NanoByte.Common simplifies this by embedding commonly used backports. Simply adding a reference to NanoByte.Common to a project targeting an older .NET version allows you to use these features.

### Language features

[LINQ](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/) was introduced in .NET Framework 3.5 and is backported to .NET Framework 2.0 by embedding [LinqBridge](http://www.albahari.com/nutshell/linqbridge.aspx).

[Nullable reference types](https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references), [Ranges](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-8.0/ranges) and [Records](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-9.0/records) were introduced in .NET (Core) and are backported to .NET Framework.

### Collections

The following types were introduced in .NET Framework 4.0 and are backported to .NET Framework 2.0:

- `System.Collections.Generic.HashSet`
- `System.Collections.Generic.SortedSet`

### Cancellation

The following types were introduced in .NET Framework 4.0 and are backported to .NET Framework 2.0:

- `System.Threading.CancellationToken`
- `System.Threading.CancellationTokenSource`
- `System.Threading.CancellationTokenRegistration`

### Progress

The following types were introduced in .NET Framework 4.5 and are backported to .NET Framework 2.0 and 4.0:

- `System.IProgress`
- `System.Progress`

### Utils

The following types were introduced in .NET Framework 4.5 and are backported to .NET Framework 2.0 and 4.0:

- `System.HashCode`
