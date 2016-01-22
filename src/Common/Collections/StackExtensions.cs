/*
 * Copyright 2006-2016 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

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
            if (stack == null) throw new ArgumentNullException("stack");
            if (action == null) throw new ArgumentNullException("action");
            #endregion

            while (stack.Count != 0)
                action(stack.Pop());
        }
    }
}
