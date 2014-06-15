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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NanoByte.Common.Dispatch
{
    /// <summary>
    /// Splits collections into multiple buckets based on value-mapping.
    /// </summary>
    /// <typeparam name="TElement">The common base type of all objects to be bucketized.</typeparam>
    /// <typeparam name="TValue">The type of the values to be matched.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class Bucketizer<TElement, TValue> : IEnumerable<BucketRule<TElement, TValue>>
    {
        private readonly List<BucketRule<TElement, TValue>> _rules = new List<BucketRule<TElement, TValue>>();
        private readonly Func<TElement, TValue> _valueRetriever;

        /// <summary>
        /// Creates a new value-mapping bucketizer.
        /// </summary>
        /// <param name="valueRetriever">A function to map elements to their according values used for bucketization.</param>
        public Bucketizer(Func<TElement, TValue> valueRetriever)
        {
            #region Sanity checks
            if (valueRetriever == null) throw new ArgumentNullException("valueRetriever");
            #endregion

            _valueRetriever = valueRetriever;
        }

        /// <summary>
        /// Adds a new bucket rule.
        /// </summary>
        /// <param name="value">A value to compare with the result of the value retriever using <see cref="object.Equals(object,object)"/>.</param>
        /// <param name="bucket">The collection elements are added to if they match the <paramref name="value"/>.</param>
        /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
        public Bucketizer<TElement, TValue> Add(TValue value, ICollection<TElement> bucket)
        {
            #region Sanity checks
            if (bucket == null) throw new ArgumentNullException("bucket");
            #endregion

            _rules.Add(new BucketRule<TElement, TValue>(value, bucket));

            return this;
        }

        /// <summary>
        /// Adds each element to the first bucket with a matching value (if any). Set up with <see cref="Add"/> first.
        /// </summary>
        public void Bucketize(IEnumerable<TElement> elements)
        {
            #region Sanity checks
            if (elements == null) throw new ArgumentNullException("elements");
            #endregion

            foreach (var element in elements)
            {
                var value = _valueRetriever(element);

                var matchedRule = _rules.FirstOrDefault(rule => Equals(rule.Value, value));
                if (matchedRule != null) matchedRule.Bucket.Add(element);
            }
        }

        #region IEnumerable
        public IEnumerator<BucketRule<TElement, TValue>> GetEnumerator()
        {
            return _rules.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _rules.GetEnumerator();
        }
        #endregion
    }
}
