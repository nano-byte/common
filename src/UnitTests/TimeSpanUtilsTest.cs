// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common;

public class TimeSpanUtilsTests
{
    [Theory]
    [InlineData(1)] // second accuracy
    [InlineData(1.2345)] // millisecond accuracy
    [InlineData(1.2345678)] // sub-millisecond accuracy
    public void FromSecondsAccurate(double seconds)
    {
        var result = TimeSpanUtils.FromSecondsAccurate(seconds);
        result.TotalSeconds.Should().Be(seconds);
    }

    [Fact]
    public void Multiply()
    {
        var ts = TimeSpan.FromSeconds(10);
        var result = ts.Multiply(2.5);
        result.Should().Be(TimeSpan.FromSeconds(25));
    }

    [Fact]
    public void DivideFactor()
    {
        var ts = TimeSpan.FromSeconds(10);
        var result = TimeSpanUtils.Divide(ts, 2);
        result.Should().Be(TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void DivideTimeSpans()
    {
        var ts1 = TimeSpan.FromSeconds(2);
        var ts2 = TimeSpan.FromSeconds(10);
        double result = ts1.Divide(ts2);
        result.Should().Be(0.2);
    }
}
