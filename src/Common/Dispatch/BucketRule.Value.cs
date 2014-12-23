/*
 * Copyright 2006-2014 Bastian Eicher
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

namespace NanoByte.Common.Dispatch
{
    /// <summary>
    /// A rule for <see cref="Bucketizer{TElement,TValue}"/>.
    /// </summary>
    public class BucketRule<TElement, TValue>
    {
        /// <summary>
        /// A value to check elements against.
        /// </summary>
        public readonly TValue Value;

        /// <summary>
        /// The collection elements are added to if they match the <see cref="Value"/>.
        /// </summary>
        [NotNull]
        public readonly ICollection<TElement> Bucket;

        /// <summary>
        /// Creates a new bucket rule.
        /// </summary>
        /// <param name="value">A value to compare with the result of the value retriever using <see cref="object.Equals(object,object)"/>.</param>
        /// <param name="bucket">The collection elements are added to if they match the <paramref name="value"/>.</param>
        public BucketRule(TValue value, [NotNull] ICollection<TElement> bucket)
        {
            #region Sanity checks
            if (bucket == null) throw new ArgumentNullException("bucket");
            #endregion

            Value = value;
            Bucket = bucket;
        }
    }
}
