// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.CodeDom;
using System.Globalization;
using JetBrains.Annotations;
using NanoByte.Common.Net;

#if !NET20 && !NET35
using System.Linq.Expressions;
using System.Reflection;
#endif

namespace NanoByte.Common
{
    /// <summary>
    /// Wraps delegate-based access to a value as a property.
    /// </summary>
    /// <typeparam name="T">The type of value the property contains.</typeparam>
    public sealed class PropertyPointer<T> : MarshalByRefObject
    {
        private readonly Func<T> _getValue;
        private readonly Action<T> _setValue;

        /// <summary>
        /// Transparent access to the wrapper value.
        /// </summary>
        public T Value { get => _getValue(); set => _setValue(value); }

        /// <summary>
        /// The default value of the property.
        /// </summary>
        public T DefaultValue { get; }

        /// <summary>
        /// <c>true</c> if <see cref="Value"/> is equal to <see cref="DefaultValue"/>.
        /// </summary>
        public bool IsDefaultValue => Equals(Value, DefaultValue);

        /// <summary>
        /// Indicates that this property needs to be encoded (e.g. as base64) before it can be stored in a file.
        /// </summary>
        public bool NeedsEncoding { get; }

        /// <summary>
        /// Creates a property pointer.
        /// </summary>
        /// <param name="getValue">A delegate that returns the current value.</param>
        /// <param name="setValue">A delegate that sets the value.</param>
        /// <param name="defaultValue">The default value of the property</param>
        /// <param name="needsEncoding">Indicates that this property needs to be encoded (e.g. as base64) before it can be stored in a file.</param>
        public PropertyPointer([NotNull] Func<T> getValue, [NotNull] Action<T> setValue, T defaultValue = default, bool needsEncoding = false)
        {
            _getValue = getValue ?? throw new ArgumentNullException(nameof(getValue));
            _setValue = setValue ?? throw new ArgumentNullException(nameof(setValue));
            DefaultValue = defaultValue;
            NeedsEncoding = needsEncoding;
        }
    }

    /// <summary>
    /// Provides factory methods for <see cref="PropertyPointer{T}"/>.
    /// </summary>
    public static class PropertyPointer
    {
        /// <summary>
        /// Creates a property pointer.
        /// </summary>
        /// <typeparam name="T">The type of value the property contains.</typeparam>
        /// <param name="getValue">A delegate that returns the current value.</param>
        /// <param name="setValue">A delegate that sets the value.</param>
        /// <param name="defaultValue">The default value of the property</param>
        /// <param name="needsEncoding">Indicates that this property needs to be encoded (e.g. as base64) before it can be stored in a file.</param>
        public static PropertyPointer<T> For<T>([NotNull] Func<T> getValue, [NotNull] Action<T> setValue, T defaultValue = default, bool needsEncoding = false)
            => new PropertyPointer<T>(getValue, setValue, defaultValue, needsEncoding);

#if !NET20 && !NET35
        /// <summary>
        /// Creates a property pointer.
        /// </summary>
        /// <typeparam name="T">The type of value the property contains.</typeparam>
        /// <param name="getValue">An expression pointing to the property.</param>
        /// <param name="defaultValue">The default value of the property</param>
        /// <param name="needsEncoding">Indicates that this property needs to be encoded (e.g. as base64) before it can be stored in a file.</param>
        public static PropertyPointer<T> For<T>([NotNull] Expression<Func<T>> getValue, T defaultValue = default, bool needsEncoding = false)
        {
            #region Sanity checks
            if (getValue == null) throw new ArgumentNullException(nameof(getValue));
            #endregion

            Expression<Action<T>> SetValue()
            {
                var memberExpression = getValue.Body as MemberExpression;
                var parameter = Expression.Parameter(typeof(T));

                switch (memberExpression?.Member)
                {
                    case PropertyInfo propertyInfo:
                        return Expression.Lambda<Action<T>>(Expression.Call(memberExpression.Expression, propertyInfo.GetSetMethod(), parameter), parameter);
                    case FieldInfo _:
                        return Expression.Lambda<Action<T>>(Expression.Assign(memberExpression, parameter), parameter);
                    default:
                        throw new ArgumentException("The expression must point to a property or field", nameof(getValue));
                }
            }

            return new PropertyPointer<T>(getValue.Compile(), SetValue().Compile(), defaultValue, needsEncoding);
        }
#endif

        /// <summary>
        /// Wraps a <see cref="bool"/> pointer in a <see cref="string"/> pointer.
        /// </summary>
        public static PropertyPointer<string> ToStringPointer([NotNull] this PropertyPointer<bool> pointer)
        {
            if (pointer == null) throw new ArgumentNullException(nameof(pointer));

            return new PropertyPointer<string>(
                getValue: () => pointer.Value.ToString(CultureInfo.InvariantCulture),
                setValue: value => pointer.Value = (value == "1" || (value != "0" && bool.Parse(value))),
                defaultValue: pointer.DefaultValue.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Wraps an <see cref="int"/> pointer in a <see cref="string"/> pointer.
        /// </summary>
        public static PropertyPointer<string> ToStringPointer([NotNull] this PropertyPointer<int> pointer)
        {
            if (pointer == null) throw new ArgumentNullException(nameof(pointer));

            return new PropertyPointer<string>(
                getValue: () => pointer.Value.ToString(CultureInfo.InvariantCulture),
                setValue: value => pointer.Value = int.Parse(value),
                defaultValue: pointer.DefaultValue.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Wraps an <see cref="long"/> pointer in a <see cref="string"/> pointer.
        /// </summary>
        public static PropertyPointer<string> ToStringPointer([NotNull] this PropertyPointer<long> pointer)
        {
            if (pointer == null) throw new ArgumentNullException(nameof(pointer));

            return new PropertyPointer<string>(
                getValue: () => pointer.Value.ToString(CultureInfo.InvariantCulture),
                setValue: value => pointer.Value = long.Parse(value),
                defaultValue: pointer.DefaultValue.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Wraps a <see cref="TimeSpan"/> pointer in a <see cref="string"/> pointer.
        /// </summary>
        public static PropertyPointer<string> ToStringPointer([NotNull] this PropertyPointer<TimeSpan> pointer)
        {
            if (pointer == null) throw new ArgumentNullException(nameof(pointer));

            return new PropertyPointer<string>(
                getValue: () => ((int)pointer.Value.TotalSeconds).ToString(CultureInfo.InvariantCulture),
                setValue: value => pointer.Value = TimeSpan.FromSeconds(int.Parse(value)),
                defaultValue: ((int)pointer.DefaultValue.TotalSeconds).ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Wraps an <see cref="Uri"/> pointer in a <see cref="string"/> pointer.
        /// </summary>
        public static PropertyPointer<string> ToStringPointer([NotNull] this PropertyPointer<Uri> pointer)
        {
            if (pointer == null) throw new ArgumentNullException(nameof(pointer));

            return new PropertyPointer<string>(
                getValue: () => pointer.Value?.ToStringRfc(),
                setValue: value => pointer.Value = (value == null) ? null : new Uri(value),
                defaultValue: pointer.DefaultValue?.ToStringRfc());
        }
    }
}
