// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Globalization;
using System.Reflection;

namespace NanoByte.Common.Values;

/// <summary>
/// Provides <see cref="CultureInfo"/>s.
/// </summary>
public static class Languages
{
    /// <summary>
    /// All known languages in alphabetical order.
    /// </summary>
    public static readonly IEnumerable<CultureInfo> AllKnown = GetAllKnown();

    private static IEnumerable<CultureInfo> GetAllKnown()
    {
        var cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures | CultureTypes.SpecificCultures);
        Array.Sort(cultures, CultureComparer.Instance);
        return cultures.Skip(1);
    }

    /// <summary>
    /// Creates a <see cref="CultureInfo"/> from a ISO language code either in Windows (e.g. en-US) or Unix (e.g. en_US) format.
    /// </summary>
    public static CultureInfo FromString(string langCode)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(langCode)) throw new ArgumentNullException(nameof(langCode));
        #endregion

        return new CultureInfo(langCode.Replace('_', '-'));
    }

    /// <summary>
    /// Changes the UI language used by this process. Should be called right after startup.
    /// </summary>
    /// <remarks>This sets <see cref="CultureInfo.CurrentUICulture"/> for the current and all future threads.</remarks>
    public static void SetUI(CultureInfo culture)
    {
        #region Sanity checks
        if (culture == null) throw new ArgumentNullException(nameof(culture));
        #endregion

        var type = typeof(CultureInfo);
        try
        {
            type.InvokeMember("s_userDefaultUICulture", BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static, null, culture, new object[] {culture});
        }
        catch (MissingMemberException)
        {}

        try
        {
            type.InvokeMember("m_userDefaultUICulture", BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static, null, culture, new object[] {culture});
        }
        catch (MissingMemberException)
        {}

        Thread.CurrentThread.CurrentUICulture = culture;
    }
}