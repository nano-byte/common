// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections;

namespace NanoByte.Common.Dispatch;

/// <summary>
/// Splits collections into multiple buckets based on value-mapping. Create with <see cref="Bucketizer.Bucketize{TElement,TValue}"/>.
/// </summary>
/// <param name="elements">The elements to be bucketized.</param>
/// <param name="valueRetriever">A function to map elements to their according values used for bucketization.</param>
/// <typeparam name="TElement">The common base type of all objects to be bucketized.</typeparam>
/// <typeparam name="TValue">The type of the values to be matched.</typeparam>
[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
public class Bucketizer<TElement, TValue>(IEnumerable<TElement> elements, Func<TElement, TValue> valueRetriever) : IEnumerable<BucketRule<TElement, TValue>>
{
    private readonly List<BucketRule<TElement, TValue>> _rules = [];

    /// <summary>
    /// Adds a new bucket rule.
    /// </summary>
    /// <param name="value">A value to compare with the result of the value retriever using <see cref="object.Equals(object,object)"/>.</param>
    /// <param name="bucket">The collection elements are added to if they match the <paramref name="value"/>.</param>
    /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
    public Bucketizer<TElement, TValue> Add(TValue value, ICollection<TElement> bucket)
    {
        _rules.Add(new(value, bucket ?? throw new ArgumentNullException(nameof(bucket))));

        return this;
    }

    /// <summary>
    /// Adds each element to the first bucket with a matching value (if any). Set up with <see cref="Add"/> first.
    /// </summary>
    public void Run()
    {
        foreach (var element in elements)
        {
            var value = valueRetriever(element);

            var matchedRule = _rules.FirstOrDefault(rule => Equals(rule.Value, value));
            matchedRule?.Bucket.Add(element);
        }
    }

    #region IEnumerable
    public IEnumerator<BucketRule<TElement, TValue>> GetEnumerator() => _rules.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _rules.GetEnumerator();
    #endregion
}

public static partial class Bucketizer
{
    /// <summary>
    /// Creates a new value-mapping bucketizer.
    /// </summary>
    /// <param name="elements">The elements to be bucketized.</param>
    /// <param name="valueRetriever">A function to map elements to their according values used for bucketization.</param>
    public static Bucketizer<TElement, TValue> Bucketize<TElement, TValue>(this IEnumerable<TElement> elements, Func<TElement, TValue> valueRetriever)
        => new(elements, valueRetriever);
}
