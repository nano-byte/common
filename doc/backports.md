---
uid: backports
---

# Backports

Newer versions of .NET often introduce features that can be backported to earlier versions. NanoByte.Common simplifies this by embedding commonly used backports. Simply adding a reference to NanoByte.Common to a project targeting an older .NET version allows you to use these features.

## Language features

[LINQ](https://docs.microsoft.com/dotnet/csharp/programming-guide/concepts/linq/) was introduced in .NET Framework 3.5 and is backported to .NET Framework 2.0 (by embedding [LinqBridge](http://www.albahari.com/nutshell/linqbridge.aspx)).

[Nullable reference types](https://docs.microsoft.com/dotnet/csharp/nullable-references), [Ranges](https://docs.microsoft.com/dotnet/csharp/language-reference/proposals/csharp-8.0/ranges) and [Records](https://docs.microsoft.com/dotnet/csharp/language-reference/proposals/csharp-9.0/records) were introduced in .NET (Core) and are backported to .NET Framework.

[Required members](https://learn.microsoft.com/dotnet/csharp/language-reference/proposals/csharp-11.0/required-members) were introduced in .NET 7.0 and are backported to .NET 6.0 and .NET Framework.

## Collections

The following types were introduced in .NET Framework 4.0 and are backported to .NET Framework 2.0:

- <xref:System.Collections.Generic.HashSet`1>
- <xref:System.Collections.Generic.SortedSet`1>

## Cancellation

The following types were introduced in .NET Framework 4.0 and are backported to .NET Framework 2.0:

- <xref:System.Threading.CancellationToken>
- <xref:System.Threading.CancellationTokenSource>
- [CancellationTokenRegistration](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtokenregistration)

## Progress

The following types were introduced in .NET Framework 4.5 and are backported to .NET Framework 2.0 and 4.0:

- <xref:System.IProgress`1>
- [Progress<T>](https://learn.microsoft.com/dotnet/api/system.progress-1)

## Utils

The following types were introduced in .NET Framework 4.5 and are backported to .NET Framework 2.0 and 4.0:

- [HashCode](https://learn.microsoft.com/dotnet/api/system.hashcode)

The following [CustomAttributeExtensions](https://learn.microsoft.com/dotnet/api/system.reflection.customattributeextensions) methods were introduced in .NET Framework 4.5 and are backported to .NET Framework 2.0 and 4.0:

- `.GetCustomAttribute<T>()`
- `.GetCustomAttributes<T>()`

## Platform attributes

The following types were introduced in .NET 5.0 and are backported to .NET Framework:

- [OSPlatformAttribute](https://learn.microsoft.com/dotnet/api/system.runtime.versioning.osplatformattribute)
- [TargetPlatformAttribute](https://learn.microsoft.com/dotnet/api/system.runtime.versioning.targetplatformattribute)
- [SupportedOSPlatformAttribute](https://learn.microsoft.com/dotnet/api/system.runtime.versioning.supportedosplatformattribute)
- [UnsupportedOSPlatformAttribute](https://learn.microsoft.com/dotnet/api/system.runtime.versioning.unsupportedosplatformattribute)
- [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis.dynamicallyaccessedmembersattribute)
- [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCodeAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis.requiresunreferencedcodeattribute)

The following types were introduced in .NET 6.0 and are backported to .NET Framework:

- [SupportedOSPlatformGuardAttribute](https://learn.microsoft.com/dotnet/api/system.runtime.versioning.supportedosplatformguardattribute)
- [UnsupportedOSPlatformGuardAttribute](https://learn.microsoft.com/dotnet/api/system.runtime.versioning.unsupportedosplatformguardattribute)
