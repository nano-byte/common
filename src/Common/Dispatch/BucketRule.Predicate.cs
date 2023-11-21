// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Dispatch;

/// <summary>
/// A rule for <see cref="Bucketizer{T}"/>.
/// </summary>
/// <param name="predicate">A condition to check elements against.</param>
/// <param name="bucket">The collection elements are added to if they match the <paramref name="predicate"/>.</param>
public class BucketRule<T>(Predicate<T> predicate, ICollection<T> bucket)
{
    /// <summary>
    /// A condition to check elements against.
    /// </summary>
    public readonly Predicate<T> Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

    /// <summary>
    /// The collection elements are added to if they match the <see cref="Predicate"/>.
    /// </summary>
    public readonly ICollection<T> Bucket = bucket ?? throw new ArgumentNullException(nameof(bucket));
}
