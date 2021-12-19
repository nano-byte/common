// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Threading;

/// <summary>
/// Provides extension methods for <see cref="Func{TIn,TOut}"/>
/// </summary>
public static class FuncExtensions
{
    /// <summary>
    /// Wraps a delegate so that it is marshalled by reference when passed via .NET Remoting.
    /// </summary>
    public static Func<TIn, TOut> ToMarshalByRef<TIn, TOut>(this Func<TIn, TOut> func)
        => new FuncByRef<TIn, TOut>(func).Invoke;

    /// <summary>
    /// A generic wrapper to pass a value by reference when using .NET remoting.
    /// </summary>
    private class FuncByRef<TIn, TOut> : MarshalNoTimeout
    {
        private readonly Func<TIn, TOut> _func;

        public FuncByRef(Func<TIn, TOut> func)
        {
            _func = func ?? throw new ArgumentNullException(nameof(func));
        }

        public TOut Invoke(TIn obj)
            => _func(obj);
    }
}