---
uid: backports
---

# Backports

Newer versions of .NET often introduce features that can be backported to earlier versions. NanoByte.Common simplifies this by embedding commonly used backports. Simply adding a reference to NanoByte.Common to a project targeting an older .NET version allows you to use these features.

## Language features

[LINQ](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/) was introduced in .NET Framework 3.5 and is backported to .NET Framework 2.0 by embedding [LinqBridge](http://www.albahari.com/nutshell/linqbridge.aspx).

[Nullable reference types](https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references), [Ranges](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-8.0/ranges) and [Records](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-9.0/records) were introduced in .NET (Core) and are backported to .NET Framework.

## Collections

The following types were introduced in .NET Framework 4.0 and are backported to .NET Framework 2.0:

- <xref:System.Collections.Generic.HashSet`1>
- <xref:System.Collections.Generic.SortedSet`1>

## Cancellation

The following types were introduced in .NET Framework 4.0 and are backported to .NET Framework 2.0:

- <xref:System.Threading.CancellationToken>
- <xref:System.Threading.CancellationTokenSource>
- <xref:System.Threading.CancellationTokenRegistration>

## Progress

The following types were introduced in .NET Framework 4.5 and are backported to .NET Framework 2.0 and 4.0:

- <xref:System.IProgress`1>
- <xref:System.Progress`1>

## Utils

The following types were introduced in .NET Framework 4.5 and are backported to .NET Framework 2.0 and 4.0:

- <xref:System.HashCode>

The following <xref:System.Reflection.CustomAttributeExtensions> methods were introduced in .NET Framework 4.5 and are backported to .NET Framework 2.0 and 4.0:

- `.GetCustomAttribute<T>()`
- `.GetCustomAttributes<T>()`

## Platform attributes

The following types were introduced in .NET 5.0 and are backported to .NET Framework and .NET Standard:

- <xref:System.Runtime.Versioning.OSPlatformAttribute>
- <xref:System.Runtime.Versioning.TargetPlatformAttribute>
- <xref:System.Runtime.Versioning.SupportedOSPlatformAttribute>
- <xref:System.Runtime.Versioning.UnsupportedOSPlatformAttribute>

The following types were introduced in .NET 6.0 and are backported to .NET Framework, .NET Standard and .NET 5.0:

- <xref:System.Runtime.Versioning.SupportedOSPlatformGuardAttribute>
- <xref:System.Runtime.Versioning.UnsupportedOSPlatformGuardAttribute>
