// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Reflection;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// Contains extension methods for <see cref="Enum"/>s.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Checks whether a flag is set.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag"), SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flag")]
        [Pure]
        public static bool HasFlag(this Enum enumRef, Enum flag)
        {
#if NET20
            long enumValue = Convert.ToInt64(enumRef);
            long flagVal = Convert.ToInt64(flag);
            return (enumValue & flagVal) == flagVal;
#else
            return enumRef.HasFlag(flag);
#endif
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

        /// <summary>
        /// Gets the first <typeparamref name="TAttribute"/> attribute set on the <paramref name="target"/> enum value.
        /// </summary>
        [Pure]
        public static TAttribute? GetEnumAttribute<TAttribute>(this Enum target) where TAttribute : Attribute
            => target.GetType()
                     .GetField((target ?? throw new ArgumentNullException(nameof(target))).ToString())
                    ?.GetCustomAttribute<TAttribute>();
    }
}
