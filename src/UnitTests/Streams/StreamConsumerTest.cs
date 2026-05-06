// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Streams;

public class StreamConsumerTest
{
    [Fact]
    public void ReadsAllLines()
    {
        var consumer = new StreamConsumer(new StreamReader("line1\nline2\nline3".ToStream()));
        consumer.WaitForEnd();

        consumer.ReadLine().Should().Be("line1");
        consumer.ReadLine().Should().Be("line2");
        consumer.ReadLine().Should().Be("line3");
        consumer.ReadLine().Should().BeNull();
    }

    [Fact]
    public void ToStringReturnsBufferedLinesJoined()
    {
        var consumer = new StreamConsumer(new StreamReader("a\nb\nc".ToStream()));
        consumer.WaitForEnd();

        consumer.ToString().Should().Be($"a{Environment.NewLine}b{Environment.NewLine}c");
    }

    [Fact]
    public void ReadLineConsumesQueue()
    {
        var consumer = new StreamConsumer(new StreamReader("only".ToStream()));
        consumer.WaitForEnd();

        consumer.ReadLine().Should().Be("only");
        consumer.ToString().Should().BeEmpty();
    }
}
