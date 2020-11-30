// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Globalization;
using NanoByte.Common.Net;

#if !NET20
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
        public PropertyPointer(Func<T> getValue, Action<T> setValue, T defaultValue, bool needsEncoding = false)
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
        public static PropertyPointer<T> For<T>(Func<T> getValue, Action<T> setValue, T defaultValue, bool needsEncoding = false)
            => new PropertyPointer<T>(getValue, setValue, defaultValue, needsEncoding);

#if !NET20
        /// <summary>
        /// Creates a property pointer.
        /// </summary>
        /// <typeparam name="T">The type of value the property contains.</typeparam>
        /// <param name="getValue">An expression pointing to the property.</param>
        /// <param name="defaultValue">The default value of the property</param>
        /// <param name="needsEncoding">Indicates that this property needs to be encoded (e.g. as base64) before it can be stored in a file.</param>
        public static PropertyPointer<T> For<T>(Expression<Func<T>> getValue, T defaultValue, bool needsEncoding = false)
        {
            #region Sanity checks
            if (getValue == null) throw new ArgumentNullException(nameof(getValue));
            #endregion

            Expression<Action<T>> SetValue()
            {
                var memberExpression = getValue.Body as MemberExpression;
                var parameter = Expression.Parameter(typeof(T));

                return memberExpression?.Member switch
                {
                    PropertyInfo propertyInfo => Expression.Lambda<Action<T>>(
                        Expression.Call(
                            memberExpression.Expression,
                            propertyInfo.GetSetMethod() ?? throw new InvalidOperationException($"Missing setter on {propertyInfo.Name}."),
                            parameter),
                        parameter),
                    FieldInfo _ => Expression.Lambda<Action<T>>(Expression.Assign(memberExpression, parameter), parameter),
                    _ => throw new ArgumentException("The expression must point to a property or field", nameof(getValue))
                };
            }

            return new PropertyPointer<T>(getValue.Compile(), SetValue().Compile(), defaultValue, needsEncoding);
        }
#endif

        /// <summary>
        /// Wraps a <see cref="bool"/> pointer in a <see cref="string"/> pointer.
        /// </summary>
        public static PropertyPointer<string> ToStringPointer(this PropertyPointer<bool> pointer)
            => For(
                getValue: () => pointer.Value.ToString(CultureInfo.InvariantCulture),
                setValue: value => pointer.Value = (value == "1" || (value != "0" && bool.Parse(value))),
                defaultValue: pointer.DefaultValue.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Wraps an <see cref="int"/> pointer in a <see cref="string"/> pointer.
        /// </summary>
        public static PropertyPointer<string> ToStringPointer(this PropertyPointer<int> pointer)
            => For(
                getValue: () => pointer.Value.ToString(CultureInfo.InvariantCulture),
                setValue: value => pointer.Value = string.IsNullOrEmpty(value) ? default : int.Parse(value),
                defaultValue: pointer.DefaultValue.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Wraps an <see cref="long"/> pointer in a <see cref="string"/> pointer.
        /// </summary>
        public static PropertyPointer<string> ToStringPointer(this PropertyPointer<long> pointer)
            => For(
                getValue: () => pointer.Value.ToString(CultureInfo.InvariantCulture),
                setValue: value => pointer.Value = string.IsNullOrEmpty(value) ? default : long.Parse(value),
                defaultValue: pointer.DefaultValue.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Wraps a <see cref="TimeSpan"/> pointer in a <see cref="string"/> pointer.
        /// </summary>
        public static PropertyPointer<string> ToStringPointer(this PropertyPointer<TimeSpan> pointer)
            => For(
                getValue: () => ((int)pointer.Value.TotalSeconds).ToString(CultureInfo.InvariantCulture),
                setValue: value => pointer.Value = string.IsNullOrEmpty(value) ? default : TimeSpan.FromSeconds(int.Parse(value)),
                defaultValue: ((int)pointer.DefaultValue.TotalSeconds).ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Wraps an <see cref="Uri"/> pointer in a <see cref="string"/> pointer. Maps empty strings to <c>null</c> URIs.
        /// </summary>
        public static PropertyPointer<string> ToStringPointer(this PropertyPointer<Uri?> pointer)
            => For(
                getValue: () => pointer.Value?.ToStringRfc() ?? "",
                setValue: value => pointer.Value = string.IsNullOrEmpty(value) ? default : new Uri(value),
                defaultValue: pointer.DefaultValue?.ToStringRfc() ?? "");
    }
}
