// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Diagnostics.CodeAnalysis;

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
#if NETSTANDARD
        [System.Diagnostics.Contracts.Pure]
#endif
        public static bool HasFlag(this Enum enumRef, Enum flag)
        {
#if NET20 || NET35
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
#if NETSTANDARD
        [System.Diagnostics.Contracts.Pure]
#endif
        public static bool HasFlag(this ushort enumRef, ushort flag) => (enumRef & flag) == flag;

        /// <summary>
        /// Checks whether a flag is set.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag"), SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flag")]
#if NETSTANDARD
        [System.Diagnostics.Contracts.Pure]
#endif
        public static bool HasFlag(this int enumRef, int flag) => (enumRef & flag) == flag;
    }
}
