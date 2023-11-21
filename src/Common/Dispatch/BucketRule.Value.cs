// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Dispatch;

/// <summary>
/// A rule for <see cref="Bucketizer{TElement,TValue}"/>.
/// </summary>
/// <param name="value">A value to compare with the result of the value retriever using <see cref="object.Equals(object,object)"/>.</param>
/// <param name="bucket">The collection elements are added to if they match the <paramref name="value"/>.</param>
public class BucketRule<TElement, TValue>(TValue value, ICollection<TElement> bucket)
{
    /// <summary>
    /// A value to check elements against.
    /// </summary>
    public readonly TValue Value = value;

    /// <summary>
    /// The collection elements are added to if they match the <see cref="Value"/>.
    /// </summary>
    public readonly ICollection<TElement> Bucket = bucket ?? throw new ArgumentNullException(nameof(bucket));
}
