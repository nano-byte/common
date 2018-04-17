// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Provides extension methods for <see cref="Stack{T}"/>s.
    /// </summary>
    public static class StackExtensions
    {
        /// <summary>
        /// <see cref="Stack{T}.Pop"/>s each element of a <see cref="Stack{T}"/> and performs an action with the resulting element.
        /// </summary>
        /// <param name="stack">The stack to pop elements from.</param>
        /// <param name="action">An action to be invoked for each element on the <paramref name="stack"/>.</param>
        public static void PopEach<T>([NotNull] this Stack<T> stack, [NotNull, InstantHandle] Action<T> action)
        {
            #region Sanity checks
            if (stack == null) throw new ArgumentNullException(nameof(stack));
            if (action == null) throw new ArgumentNullException(nameof(action));
            #endregion

            while (stack.Count != 0)
                action(stack.Pop());
        }
    }
}
