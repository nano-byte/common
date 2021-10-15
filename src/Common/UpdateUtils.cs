// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using JetBrains.Annotations;

#if !NET20 && !NET40
using System.Runtime.CompilerServices;
#endif

namespace NanoByte.Common
{
    /// <summary>
    /// Provides neat little code-shortcuts for updating properties.
    /// </summary>
    public static class UpdateUtils
    {
        /// <summary>
        /// Immediately invokes the specified <paramref name="action"/> with the <paramref name="value"/>. Useful for applying null-coalescing operator.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="action">The action to invoke.</param>
        /// <example>
        /// This allows you to write:
        /// <code>
        /// Uri? uri = nullableString?.To(x => new Uri(x);
        /// </code>
        /// Instead of:
        /// <code>
        /// Uri? uri = nullableString == null ? null : new Uri(nullableString);
        /// </code>
        /// </example>
#if !NET20 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TOut To<TIn, TOut>(this TIn value, [InstantHandle] Func<TIn, TOut> action)
            => (action ?? throw new ArgumentNullException(nameof(action))).Invoke(value);

        /// <summary>
        /// Updates a value and sets a boolean flag to <c>true</c> if the original value actually changed.
        /// </summary>
        /// <typeparam name="T">The type of data to update.</typeparam>
        /// <param name="value">The new value.</param>
        /// <param name="original">The original value to update.</param>
        /// <param name="updated">Gets set to <c>true</c> if value is different from original.</param>
#if !NET20 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void To<T>(this T value, ref T original, ref bool updated) where T : struct
        {
            // If the values already match, nothing needs to be done
            if (original.Equals(value)) return;

            // Otherwise set the "updated" flag and change the value
            updated = true;
            original = value;
        }

        /// <summary>
        /// Updates a value and sets two boolean flags to <c>true</c> if the original value actually changed.
        /// </summary>
        /// <typeparam name="T">The type of data to update.</typeparam>
        /// <param name="value">The new value.</param>
        /// <param name="original">The original value to update.</param>
        /// <param name="updated1">Gets set to <c>true</c> if value is different from original.</param>
        /// <param name="updated2">Gets set to <c>true</c> if value is different from original.</param>
#if !NET20 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void To<T>(this T value, ref T original, ref bool updated1, ref bool updated2) where T : struct
        {
            // If the values already match, nothing needs to be done
            if (original.Equals(value)) return;

            updated1 = true;
            updated2 = true;
            original = value;
        }

        /// <summary>
        /// Updates a value and sets a boolean flag to <c>true</c> if the original value actually changed
        /// </summary>
        /// <param name="value">The new value</param>
        /// <param name="original">The original value to update</param>
        /// <param name="updated">Gets set to <c>true</c> if value is different from original</param>
#if !NET20 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void To(this string value, ref string original, ref bool updated)
        {
            // If the values already match, nothing needs to be done
            if (original == value) return;

            // Otherwise set the "updated" flag and change the value
            updated = true;
            original = value;
        }

        /// <summary>
        /// Updates a value and sets two boolean flags to <c>true</c> if the original value actually changed
        /// </summary>
        /// <param name="value">The new value</param>
        /// <param name="original">The original value to update</param>
        /// <param name="updated1">Gets set to <c>true</c> if value is different from original</param>
        /// <param name="updated2">Gets set to <c>true</c> if value is different from original</param>
#if !NET20 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void To(this string value, ref string original, ref bool updated1, ref bool updated2)
        {
            if (original == value) return;

            updated1 = true;
            updated2 = true;
            original = value;
        }

        /// <summary>
        /// Updates a value and calls back a delegate if the original value actually changed.
        /// </summary>
        /// <typeparam name="T">The type of data to update.</typeparam>
        /// <param name="value">The new value.</param>
        /// <param name="original">The original value to update.</param>
        /// <param name="updated">Gets called if value is different from original.</param>
#if !NET20 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void To<T>(this T value, ref T original, [InstantHandle] Action updated) where T : struct
        {
            #region Sanity checks
            if (updated == null) throw new ArgumentNullException(nameof(updated));
            #endregion

            // If the values already match, nothing needs to be done
            if (original.Equals(value)) return;

            // Backup the original value in case it needs to be reverted
            var backup = original;

            // Set the new value
            original = value;

            // Execute the "updated" delegate
            try
            {
                updated.Invoke();
            }
            catch
            {
                // Restore the original value before passing exceptions upwards
                original = backup;
                throw;
            }
        }

        /// <summary>
        /// Updates a value and calls back a delegate if the original value actually changed.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <param name="original">The original value to update.</param>
        /// <param name="updated">Gets called if value is different from original.</param>
#if !NET20 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void To(this string value, ref string original, [InstantHandle] Action updated)
        {
            #region Sanity checks
            if (updated == null) throw new ArgumentNullException(nameof(updated));
            #endregion

            // If the values already match, nothing needs to be done
            if (original == value) return;

            // Backup the original value in case it needs to be reverted
            string backup = original;

            // Set the new value
            original = value;

            // Execute the "updated" delegate
            try
            {
                updated.Invoke();
            }
            catch
            {
                // Restore the original value before passing exceptions upwards
                original = backup;
                throw;
            }
        }

        /// <summary>
        /// Swaps the content of two fields.
        /// </summary>
        /// <typeparam name="T">The type of objects to swap.</typeparam>
        /// <param name="value1">The first field which will afterwards carry the content of <paramref name="value2"/>.</param>
        /// <param name="value2">The first field which will afterwards carry the content of <paramref name="value1"/>.</param>
#if !NET20 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void Swap<T>(ref T value1, ref T value2)
        {
            var tempValue = value1;
            value1 = value2;
            value2 = tempValue;
        }
    }
}
