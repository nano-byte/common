// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Reflection;

namespace NanoByte.Common.Values;

/// <summary>
/// Contains extension methods for <see cref="Enum"/>s.
/// </summary>
public static class EnumExtensions
{
    extension(Enum value)
    {
        /// <summary>
        /// Checks whether a flag is set.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag"), SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flag")]
        [Pure]
        public bool HasFlag(Enum flag)
            => value.HasFlag(flag);

        /// <summary>
        /// Gets the first <typeparamref name="TAttribute"/> attribute set on the <paramref name="value"/> enum value.
        /// </summary>
        [Pure]
        public TAttribute? GetEnumAttribute<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TAttribute>() where TAttribute : Attribute
            => value.GetType()
                    .GetField(value.ToString())
                   ?.GetCustomAttribute<TAttribute>();
    }

    /// <summary>
    /// Checks whether a flag is set.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag"), SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flag")]
    [CLSCompliant(false)]
    [Pure]
    public static bool HasFlag(this ushort enumRef, ushort flag) => (enumRef & flag) == flag;

    /// <summary>
    /// Checks whether a flag is set.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag"), SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flag")]
    [Pure]
    public static bool HasFlag(this int enumRef, int flag) => (enumRef & flag) == flag;
}
