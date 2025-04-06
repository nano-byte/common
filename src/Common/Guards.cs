// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common;

/// <summary>
/// Guards that can be statically evaluated by a linker.
/// </summary>
internal class Guards
{
#if NET9_0_OR_GREATER
    // This feature is disabled by default in trimmed apps:
    // https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/trimming-options#framework-features-disabled-when-trimming
    private const string NotTrimmedSwitch = "System.Resources.ResourceManager.AllowCustomResourceTypes";

    /// <summary>
    /// The current app is likely to be not trimmed and therefore supports full reflection.
    /// </summary>
    [FeatureGuard(typeof(RequiresUnreferencedCodeAttribute))]
    [FeatureSwitchDefinition(NotTrimmedSwitch)]
    public static bool NotTrimmed { get; } = AppContext.TryGetSwitch(NotTrimmedSwitch, out bool value) ? value : true;
#else
    /// <summary>
    /// Always returns true.
    /// </summary>
    public static bool NotTrimmed => true;
#endif
}
