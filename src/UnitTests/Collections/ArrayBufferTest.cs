// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NET20 && !NET40
namespace NanoByte.Common.Collections;

/// <summary>
/// Contains test methods for <see cref="ArrayBuffer{T}"/>.
/// </summary>
public class ArrayBufferTest
{
    [Fact]
    public void CreatesArrayWithRequestedLength()
    {
        using var buffer = new ArrayBuffer<int>(10);
        buffer.Length.Should().Be(10);
        buffer.Array.Length.Should().BeGreaterOrEqualTo(10);
    }

    [Fact]
    public void SegmentHasExactLength()
    {
        using var buffer = new ArrayBuffer<int>(10);
        var segment = buffer.Segment;
        segment.Count.Should().Be(10);
    }

    [Fact]
    public void SpanHasExactLength()
    {
        using var buffer = new ArrayBuffer<int>(10);
        var span = buffer.Span;
        span.Length.Should().Be(10);
    }

    [Fact]
    public void CanWriteAndReadData()
    {
        using var buffer = new ArrayBuffer<int>(5);
        for (int i = 0; i < 5; i++)
            buffer.Array[i] = i * 10;

        for (int i = 0; i < 5; i++)
            buffer.Array[i].Should().Be(i * 10);
    }

    [Fact]
    public void ThrowsAfterDispose()
    {
        var buffer = new ArrayBuffer<int>(10);
        buffer.Dispose();

        buffer.Invoking(b => _ = b.Array)
              .Should().Throw<ObjectDisposedException>();
    }

    [Fact]
    public void SegmentThrowsAfterDispose()
    {
        var buffer = new ArrayBuffer<int>(10);
        buffer.Dispose();

        buffer.Invoking(b => _ = b.Segment)
              .Should().Throw<ObjectDisposedException>();
    }

    [Fact]
    public void SpanThrowsAfterDispose()
    {
        var buffer = new ArrayBuffer<int>(10);
        buffer.Dispose();

        buffer.Invoking(b => _ = b.Span)
              .Should().Throw<ObjectDisposedException>();
    }

    [Fact]
    public void DoubleDisposeIsSafe()
    {
        var buffer = new ArrayBuffer<int>(10);
        buffer.Dispose();
        buffer.Dispose();
    }
}
#endif
