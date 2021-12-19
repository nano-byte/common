// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Collections;

/// <summary>
/// Contains test methods for <see cref="TransparentCache{TKey,TValue}"/>.
/// </summary>
public class TransparentCacheTest
{
    [Fact]
    public void Test()
    {
        int callCounter = 0;
        var cache = new TransparentCache<string, string>(input =>
        {
            callCounter++;
            return input + "X";
        });

        cache["input"].Should().Be("inputX");
        callCounter.Should().Be(1, because: "Should call retriever callback on first request");

        cache["input"].Should().Be("inputX");
        callCounter.Should().Be(1, because: "Should not call retriever callback again on subsequent requests");
    }
}