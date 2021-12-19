// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections;

namespace NanoByte.Common.Dispatch;

/// <summary>
/// Splits collections into multiple buckets based on predicate matching. The first matching predicate wins. Create with <see cref="Bucketizer.Bucketize{T}"/>.
/// </summary>
/// <typeparam name="T">The common base type of all objects to be bucketized.</typeparam>
[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
public class Bucketizer<T> : IEnumerable<BucketRule<T>>
{
    private readonly IEnumerable<T> _elements;
    private readonly List<BucketRule<T>> _rules = new();

    /// <summary>
    /// Creates a new predicate-matching bucketizer.
    /// </summary>
    /// <param name="elements">The elements to be bucketized.</param>
    internal Bucketizer(IEnumerable<T> elements)
    {
        _elements = elements ?? throw new ArgumentNullException(nameof(elements));
    }

    /// <summary>
    /// Adds a new bucket rule.
    /// </summary>
    /// <param name="predicate">A condition to check elements against.</param>
    /// <param name="bucket">The collection elements are added to if they match the <paramref name="predicate"/>.</param>
    /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
    public Bucketizer<T> Add(Predicate<T> predicate, ICollection<T> bucket)
    {
        _rules.Add(new(
            predicate ?? throw new ArgumentNullException(nameof(predicate)),
            bucket ?? throw new ArgumentNullException(nameof(bucket))));

        return this;
    }

    /// <summary>
    /// Adds each element to the first bucket with a matching predicate (if any). Set up with <see cref="Add"/> first.
    /// </summary>
    public void Run()
    {
        foreach (var element in _elements)
        {
            var matchedRule = _rules.FirstOrDefault(rule => rule.Predicate(element));
            matchedRule?.Bucket.Add(element);
        }
    }

    #region IEnumerable
    public IEnumerator<BucketRule<T>> GetEnumerator() => _rules.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _rules.GetEnumerator();
    #endregion
}

/// <summary>
/// Contains extension methods for creating bucketizers.
/// </summary>
public static partial class Bucketizer
{
    /// <summary>
    /// Creates a new predicate-matching bucketizer.
    /// </summary>
    /// <param name="elements">The elements to be bucketized.</param>
    public static Bucketizer<T> Bucketize<T>(this IEnumerable<T> elements) => new(elements);
}