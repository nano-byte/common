// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common;

public class PropertyPointerTest
{
    private class Sample
    {
        public string? Data { get; set; }
    }

    [Fact]
    public void Delegate()
    {
        var sample = new Sample {Data = "a"};
        var pointer = PropertyPointer.For(() => sample.Data, value => sample.Data = value);

        pointer.Value.Should().Be("a");
        pointer.Value = "b";
        sample.Data.Should().Be("b");
    }

    [Fact]
    public void DelegateNullable()
    {
        var sample = new Sample {Data = "a"};
        var pointer = PropertyPointer.ForNullable(() => sample.Data, value => sample.Data = value);

        pointer.Value.Should().Be("a");
        pointer.Value = null;
        sample.Data.Should().BeNull();
    }

    [Fact]
    public void ExpressionTree()
    {
        var sample = new Sample {Data = "a"};
        var pointer = PropertyPointer.For(() => sample.Data);

        pointer.Value.Should().Be("a");
        pointer.Value = "b";
        sample.Data.Should().Be("b");
    }

    [Fact]
    public void ExpressionTreeNullable()
    {
        var sample = new Sample {Data = "a"};
        var pointer = PropertyPointer.ForNullable(() => sample.Data);

        pointer.Value.Should().Be("a");
        pointer.Value = null;
        sample.Data.Should().BeNull();
    }

    [Fact]
    public void SetTemp()
    {
        var sample = new Sample {Data = "a"};

        using (PropertyPointer.For(() => sample.Data).SetTemp("b"))
            sample.Data.Should().Be("b");

        sample.Data.Should().Be("a");
    }
}
