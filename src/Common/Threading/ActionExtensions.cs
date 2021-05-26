// Copyright Bastian Eicher
// Licensed under the MIT License

using System;

namespace NanoByte.Common.Threading
{
    /// <summary>
    /// Provides extension methods for <see cref="Action{T}"/>
    /// </summary>
    public static class ActionExtensions
    {
        /// <summary>
        /// Wraps a delegate so that it is marshalled by reference when passed via .NET Remoting.
        /// </summary>
        public static Action<T> ToMarshalByRef<T>(this Action<T> action)
            => new ActionByRef<T>(action).Invoke;

        /// <summary>
        /// A generic wrapper to pass a value by reference when using .NET remoting.
        /// </summary>
        private class ActionByRef<T> : MarshalNoTimeout
        {
            private readonly Action<T> _action;

            public ActionByRef(Action<T> action)
            {
                _action = action ?? throw new ArgumentNullException(nameof(action));
            }

            public void Invoke(T obj)
                => _action(obj);
        }
    }
}
