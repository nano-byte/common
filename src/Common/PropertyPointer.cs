// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NET20
using System.Linq.Expressions;
using System.Reflection;
#endif

namespace NanoByte.Common;

/// <summary>
/// Wraps delegate-based access to a property.
/// </summary>
/// <param name="getValue">A delegate that returns the current value.</param>
/// <param name="setValue">A delegate that sets the value.</param>
/// <typeparam name="T">The type of value the property contains.</typeparam>
public class PropertyPointer<T>(Func<T> getValue, Action<T> setValue) : MarshalByRefObject
{
    /// <summary>
    /// Transparent access to the wrapper value.
    /// </summary>
    public T Value { get => getValue(); set => setValue(value); }

    /// <summary>
    /// Temporarily changes the value of the property.
    /// </summary>
    /// <returns>Call <see cref="IDisposable.Dispose"/> to restore the original value of the property.</returns>
    /// <example>
    /// <code>
    /// using (PropertyPointer.For(() => someProperty).SetTemp(someValue))
    /// {
    ///    // ...
    /// }
    /// </code>
    /// </example>
    public IDisposable SetTemp(T value)
    {
        var backup = Value;
        Value = value;
        return new Disposable(() => Value = backup);
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
    public static PropertyPointer<T> For<T>(Func<T> getValue, Action<T> setValue)
        where T : notnull
        => new(getValue, setValue);

    /// <summary>
    /// Creates a property pointer for a nullable value.
    /// </summary>
    /// <typeparam name="T">The type of value the property contains.</typeparam>
    /// <param name="getValue">A delegate that returns the current value.</param>
    /// <param name="setValue">A delegate that sets the value.</param>
    public static PropertyPointer<T?> ForNullable<T>(Func<T?> getValue, Action<T?> setValue)
        => new(getValue, setValue);

#if !NET20
    /// <summary>
    /// Creates a property pointer.
    /// </summary>
    /// <typeparam name="T">The type of value the property contains.</typeparam>
    /// <param name="expression">An expression pointing to the property.</param>
    /// <exception cref="ArgumentException">The expression does not point to a property with a setter.</exception>
    public static PropertyPointer<T> For<T>(Expression<Func<T>> expression)
        where T : notnull
        => new(expression.Compile(), expression.ToSetValue());

    /// <summary>
    /// Creates a property pointer for a nullable value.
    /// </summary>
    /// <typeparam name="T">The type of value the property contains.</typeparam>
    /// <param name="expression">An expression pointing to the property.</param>
    /// <exception cref="ArgumentException">The expression does not point to a property with a setter.</exception>
    public static PropertyPointer<T?> ForNullable<T>(Expression<Func<T?>> expression)
        => new(expression.Compile(), expression.ToSetValue());

    /// <summary>
    /// Converts an expression pointing to a property into a delegate for setting the property's value.
    /// </summary>
    /// <exception cref="ArgumentException">The expression does not point to a property with a setter.</exception>
    public static Action<T> ToSetValue<T>(this Expression<Func<T>> expression)
    {
        var memberExpression = expression.Body as MemberExpression;
        var parameter = Expression.Parameter(typeof(T));

        return (memberExpression?.Member switch
        {
            PropertyInfo propertyInfo => Expression.Lambda<Action<T>>(
                Expression.Call(
                    memberExpression.Expression,
                    propertyInfo.GetSetMethod() ?? throw new ArgumentException($"Missing setter on {propertyInfo.Name}."),
                    parameter),
                parameter),
            FieldInfo _ => Expression.Lambda<Action<T>>(Expression.Assign(memberExpression, parameter), parameter),
            _ => throw new ArgumentException("The expression must point to a property or field.", nameof(expression))
        }).Compile();
    }
#endif
}
