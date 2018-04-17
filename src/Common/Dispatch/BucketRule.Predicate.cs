// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace NanoByte.Common.Dispatch
{
    /// <summary>
    /// A rule for <see cref="Bucketizer{T}"/>.
    /// </summary>
    public class BucketRule<T>
    {
        /// <summary>
        /// A condition to check elements against.
        /// </summary>
        [NotNull]
        public readonly Predicate<T> Predicate;

        /// <summary>
        /// The collection elements are added to if they match the <see cref="Predicate"/>.
        /// </summary>
        [NotNull]
        public readonly ICollection<T> Bucket;

        /// <summary>
        /// Creates a new bucket rule.
        /// </summary>
        /// <param name="predicate">A condition to check elements against.</param>
        /// <param name="bucket">The collection elements are added to if they match the <paramref name="predicate"/>.</param>
        public BucketRule([NotNull] Predicate<T> predicate, [NotNull] ICollection<T> bucket)
        {
            Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            Bucket = bucket ?? throw new ArgumentNullException(nameof(bucket));
        }
    }
}
