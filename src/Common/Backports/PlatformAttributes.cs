#if NET
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Runtime.Versioning.TargetPlatformAttribute))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Runtime.Versioning.SupportedOSPlatformAttribute))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Runtime.Versioning.UnsupportedOSPlatformAttribute))]
#if NET6_0_OR_GREATER
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Runtime.Versioning.SupportedOSPlatformGuardAttribute))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Runtime.Versioning.UnsupportedOSPlatformGuardAttribute))]
#endif
#endif

#if !NET6_0_OR_GREATER
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#pragma warning disable CS3015 // Not CLS compliant
#pragma warning disable CS0436 // Type conflict
namespace System.Runtime.Versioning
{
    /// <summary>
    /// Base type for all platform-specific API attributes.
    /// </summary>
    public abstract class OSPlatformAttribute : Attribute
    {
        private protected OSPlatformAttribute(string platformName)
        {
            PlatformName = platformName;
        }
        public string PlatformName { get; }
    }

#if !NET
    /// <summary>
    /// Records the platform that the project targeted.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly,
                    AllowMultiple = false, Inherited = false)]
    public sealed class TargetPlatformAttribute : OSPlatformAttribute
    {
        public TargetPlatformAttribute(string platformName) : base(platformName)
        {
        }
    }

    /// <summary>
    /// Records the operating system (and minimum version) that supports an API. Multiple attributes can be
    /// applied to indicate support on multiple operating systems.
    /// </summary>
    /// <remarks>
    /// Callers can apply a <see cref="System.Runtime.Versioning.SupportedOSPlatformAttribute " />
    /// or use guards to prevent calls to APIs on unsupported operating systems.
    ///
    /// A given platform should only be specified once.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Assembly |
                    AttributeTargets.Class |
                    AttributeTargets.Constructor |
                    AttributeTargets.Enum |
                    AttributeTargets.Event |
                    AttributeTargets.Field |
                    AttributeTargets.Interface |
                    AttributeTargets.Method |
                    AttributeTargets.Module |
                    AttributeTargets.Property |
                    AttributeTargets.Struct,
                    AllowMultiple = true, Inherited = false)]
    public sealed class SupportedOSPlatformAttribute : OSPlatformAttribute
    {
        public SupportedOSPlatformAttribute(string platformName) : base(platformName)
        {
        }
    }

    /// <summary>
    /// Marks APIs that were removed in a given operating system version.
    /// </summary>
    /// <remarks>
    /// Primarily used by OS bindings to indicate APIs that are only available in
    /// earlier versions.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Assembly |
                    AttributeTargets.Class |
                    AttributeTargets.Constructor |
                    AttributeTargets.Enum |
                    AttributeTargets.Event |
                    AttributeTargets.Field |
                    AttributeTargets.Interface |
                    AttributeTargets.Method |
                    AttributeTargets.Module |
                    AttributeTargets.Property |
                    AttributeTargets.Struct,
                    AllowMultiple = true, Inherited = false)]
    public sealed class UnsupportedOSPlatformAttribute : OSPlatformAttribute
    {
        public UnsupportedOSPlatformAttribute(string platformName) : base(platformName)
        {
        }
    }
#endif

    /// <summary>
    /// Annotates a custom guard field, property or method with a supported platform name and optional version.
    /// Multiple attributes can be applied to indicate guard for multiple supported platforms.
    /// </summary>
    /// <remarks>
    /// Callers can apply a <see cref="System.Runtime.Versioning.SupportedOSPlatformGuardAttribute " /> to a field, property or method
    /// and use that field, property or method in a conditional or assert statements in order to safely call platform specific APIs.
    ///
    /// The type of the field or property should be boolean, the method return type should be boolean in order to be used as platform guard.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field |
                    AttributeTargets.Method |
                    AttributeTargets.Property,
                    AllowMultiple = true, Inherited = false)]
    public sealed class SupportedOSPlatformGuardAttribute : OSPlatformAttribute
    {
        public SupportedOSPlatformGuardAttribute(string platformName) : base(platformName)
        {
        }
    }

    /// <summary>
    /// Annotates the custom guard field, property or method with an unsupported platform name and optional version.
    /// Multiple attributes can be applied to indicate guard for multiple unsupported platforms.
    /// </summary>
    /// <remarks>
    /// Callers can apply a <see cref="System.Runtime.Versioning.UnsupportedOSPlatformGuardAttribute " /> to a field, property or method
    /// and use that  field, property or method in a conditional or assert statements as a guard to safely call APIs unsupported on those platforms.
    ///
    /// The type of the field or property should be boolean, the method return type should be boolean in order to be used as platform guard.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field |
                    AttributeTargets.Method |
                    AttributeTargets.Property,
                    AllowMultiple = true, Inherited = false)]
    public sealed class UnsupportedOSPlatformGuardAttribute : OSPlatformAttribute
    {
        public UnsupportedOSPlatformGuardAttribute(string platformName) : base(platformName)
        {
        }
    }
}
#endif
